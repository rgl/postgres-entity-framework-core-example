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
-- list relations.
\d
-- list relations access privileges.
\dp
-- list default access privileges.
\ddp
```

You should see something alike:

```plain
startrek=# -- list roles.
startrek=# \dg
                                          List of roles
    Role name    |                         Attributes                         |     Member of     
-----------------+------------------------------------------------------------+-------------------
 example         |                                                            | {startrek_reader}
 postgres        | Superuser, Create role, Create DB, Replication, Bypass RLS | {}
 startrek_owner  |                                                            | {}
 startrek_reader | Cannot login                                               | {}

startrek=# -- list databases.
startrek=# \l
                                    List of databases
   Name    |  Owner   | Encoding |  Collate   |   Ctype    |      Access privileges      
-----------+----------+----------+------------+------------+-----------------------------
 postgres  | postgres | UTF8     | en_US.utf8 | en_US.utf8 | 
 startrek  | postgres | UTF8     | en_US.utf8 | en_US.utf8 | postgres=CTc/postgres      +
           |          |          |            |            | startrek_owner=CTc/postgres+
           |          |          |            |            | startrek_reader=c/postgres
 template0 | postgres | UTF8     | en_US.utf8 | en_US.utf8 | =c/postgres                +
           |          |          |            |            | postgres=CTc/postgres
 template1 | postgres | UTF8     | en_US.utf8 | en_US.utf8 | =c/postgres                +
           |          |          |            |            | postgres=CTc/postgres
(4 rows)

startrek=# -- list relations.
startrek=# \d
                     List of relations
 Schema |         Name          |   Type   |     Owner      
--------+-----------------------+----------+----------------
 public | CharacterPhoto        | table    | startrek_owner
 public | CharacterPhoto_Id_seq | sequence | startrek_owner
 public | Characters            | table    | startrek_owner
 public | Characters_Id_seq     | sequence | startrek_owner
 public | Series                | table    | startrek_owner
 public | Series_Id_seq         | sequence | startrek_owner
 public | __EFMigrationsHistory | table    | startrek_owner
(7 rows)

startrek=# -- list access privileges.
startrek=# \dp
                                                Access privileges
 Schema |         Name          |   Type   |           Access privileges           | Column privileges | Policies 
--------+-----------------------+----------+---------------------------------------+-------------------+----------
 public | CharacterPhoto        | table    | startrek_owner=arwdDxt/startrek_owner+|                   | 
        |                       |          | startrek_reader=r/startrek_owner      |                   | 
 public | CharacterPhoto_Id_seq | sequence | startrek_owner=rwU/startrek_owner    +|                   | 
        |                       |          | startrek_reader=r/startrek_owner      |                   | 
 public | Characters            | table    | startrek_owner=arwdDxt/startrek_owner+|                   | 
        |                       |          | startrek_reader=r/startrek_owner      |                   | 
 public | Characters_Id_seq     | sequence | startrek_owner=rwU/startrek_owner    +|                   | 
        |                       |          | startrek_reader=r/startrek_owner      |                   | 
 public | Series                | table    | startrek_owner=arwdDxt/startrek_owner+|                   | 
        |                       |          | startrek_reader=r/startrek_owner      |                   | 
 public | Series_Id_seq         | sequence | startrek_owner=rwU/startrek_owner    +|                   | 
        |                       |          | startrek_reader=r/startrek_owner      |                   | 
 public | __EFMigrationsHistory | table    | startrek_owner=arwdDxt/startrek_owner+|                   | 
        |                       |          | startrek_reader=r/startrek_owner      |                   | 
(7 rows)

startrek=# -- list default access privileges.
startrek=# \ddp
                       Default access privileges
     Owner      | Schema |   Type   |        Access privileges         
----------------+--------+----------+----------------------------------
 startrek_owner | public | function | startrek_reader=X/startrek_owner
 startrek_owner | public | sequence | startrek_reader=r/startrek_owner
 startrek_owner | public | table    | startrek_reader=r/startrek_owner
(3 rows)
```

Destroy everything:

```bash
docker compose down --volumes --remove-orphans --timeout=0
```

# References

* [Database Roles](https://www.postgresql.org/docs/14/user-manag.html)
* [Role Membership](https://www.postgresql.org/docs/14/role-membership.html)
* [Privileges](https://www.postgresql.org/docs/14/ddl-priv.html)
* [ALTER DEFAULT PRIVILEGES](https://www.postgresql.org/docs/14/sql-alterdefaultprivileges.html)
* [GRANT](https://www.postgresql.org/docs/14/sql-grant.html)
* [REVOKE](https://www.postgresql.org/docs/14/sql-revoke.html)
