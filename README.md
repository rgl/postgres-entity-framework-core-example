# About

[![Build status](https://github.com/rgl/postgres-entity-framework-core-example/workflows/build/badge.svg)](https://github.com/rgl/postgres-entity-framework-core-example/actions?query=workflow%3Abuild)

This shows how to use PostgreSQL from an C# Entity Framework Core project.

# Usage

Install docker and docker compose.

Run the whole shebang:

```bash
./run.sh
```

Poke the database:

```bash
docker compose exec --user postgres postgres psql startrek
```
```sql
-- list roles.
\dg
-- list databases.
\l
-- list tables.
\d
-- list tables and privileges.
\dp
```

Destroy everything:

```bash
docker compose down --volumes --remove-orphans --timeout=0
```

# References

* [Database Roles](https://www.postgresql.org/docs/14/user-manag.html)
* [Privileges](https://www.postgresql.org/docs/14/ddl-priv.html)
* [ALTER DEFAULT PRIVILEGES](https://www.postgresql.org/docs/14/sql-alterdefaultprivileges.html)
