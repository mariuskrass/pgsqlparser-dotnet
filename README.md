# pg_query for .NET

.NET version of https://github.com/pganalyze/pg_query

This .NET wrapper on pg_query C library which uses the actual PostgreSQL server source to parse SQL queries and return the internal PostgreSQL parse tree.

You can find further background to why a query's parse tree is useful here: https://pganalyze.com/blog/parse-postgresql-queries-in-ruby.html

## Installation

```csharp
dotnet add package pg_query 
```

Note that the libpg_query libs for all OS'es are already packaged with the assembly.

## Usage

TODO

## License

Copyright (c) 2025, Babu Annamalai <babu.annamalai@gmail.com>

Refer to [pg_query license](https://github.com/pganalyze/pg_query?tab=readme-ov-file#license) for license details on pg_query.

This project includes code derived from the [PostgreSQL project](http://www.postgresql.org/),
see LICENSE.POSTGRESQL for details.
