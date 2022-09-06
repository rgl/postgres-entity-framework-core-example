# About

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
