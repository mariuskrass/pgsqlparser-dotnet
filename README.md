# pg_query for.NET

.NET version of https://github.com/pganalyze/pg_query

This .NET wrapper on libpg_query C library which uses the actual PostgreSQL server source to parse SQL queries and return the internal PostgreSQL parse tree.

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

### pg_query Library License

Copyright (c) 2016-2025, Duboce Labs, Inc. (pganalyze) <team@pganalyze.com>
pg_query_go is licensed under the 3-clause BSD license, see LICENSE file for details.

This project includes code derived from the [PostgreSQL project](http://www.postgresql.org/),
see LICENSE.POSTGRESQL for details.
