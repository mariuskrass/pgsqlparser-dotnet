# PgSqlParser

.NET version of [https://github.com/pganalyze/libpg_query](https://github.com/pganalyze/libpg_query). This .NET wrapper on libpg_query C library which uses the actual PostgreSQL server source to parse SQL queries and return the internal PostgreSQL parse tree.

You can find further background to why a query's parse tree is useful here: [https://pganalyze.com/blog/parse-postgresql-queries-in-ruby.html](https://pganalyze.com/blog/pg-query-2-0-postgres-query-parser)

## Installation

```csharp
dotnet add package pgsqlparser
```

Note that the libpg_query libs for all OS'es are already packaged with the assembly.

## Usage

All functions support both sync and async versions.

### Normalize

Transform DML query (SELECT, INSERT, UPDATE, DELETE) into a canonical form by replacing literal values (constants) with placeholders ($1, $2)

```csharp
using PgSqlParser;

var query = "SELECT 1";
var result = Parser.Normalize(query);

// result: SELECT $1

```

### NormalizeUtility

Transform DDL and other utility commands (CREATE TABLE, ALTER TABLE, VACUUM, and ANALYZE et.al.) into a canonical form by replacing literal values (constants) with placeholders ($1, $2)


## License

Copyright (c) 2025, Babu Annamalai <babu.annamalai@gmail.com>

Refer to [libpg_query license](https://github.com/pganalyze/libpg_query?tab=readme-ov-file#license) for license details on libpg_query.

This project includes code derived from the [PostgreSQL project](http://www.postgresql.org/),
see LICENSE.POSTGRESQL for details.
