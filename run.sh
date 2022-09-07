#!/bin/bash
set -euxo pipefail

# destroy existing containers and data.
docker compose down --volumes --remove-orphans --timeout=0

# build the container images.
docker compose build

# start postgres.
docker compose up postgres --detach

function psql {
    docker compose exec \
        -e PGHOST \
        -e PGPORT \
        -e PGDATABASE \
        -e PGUSER \
        -e PGPASSWORD \
        postgres \
        psql
}

export PGHOST=postgres
export PGPORT=5432
export PGDATABASE=postgres
export PGUSER=postgres
export PGPASSWORD=password

# wait for postgres to be ready.
while ! psql <<<'select 1' >/dev/null 2>&1; do sleep 1; done

# create database, global roles and privileges.
psql <<'EOF'
create database startrek;
create role startrek_owner login password 'password';
create role startrek_reader;
revoke all privileges on database startrek from public;
grant all privileges on database startrek to startrek_owner;
grant connect on database startrek to startrek_reader;
create role example login password 'password';
grant startrek_reader to example;
EOF

# create database roles and privileges.
psql <<'EOF'
\c startrek
grant usage on schema public to startrek_reader;
alter default privileges for role startrek_owner in schema public grant select on tables to startrek_reader;
alter default privileges for role startrek_owner in schema public grant select on sequences to startrek_reader;
alter default privileges for role startrek_owner in schema public grant execute on functions to startrek_reader;
EOF

# migrate and seed the database.
docker compose up --exit-code-from migrate migrate
docker compose up --exit-code-from seed seed

# list databases, tables, and privileges.
psql <<'EOF'
\c startrek
\l
\d
\dp
EOF

# use the database.
docker compose up --exit-code-from run run
