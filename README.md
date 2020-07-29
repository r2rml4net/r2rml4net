# r2rml4net [![Build status][build-badge]][build-link] [![NuGet version][nuget-badge]][nuget-link]

Welcome to r2rml4net, a project which brings [R2RML](http://www.w3.org/TR/r2rml/) to the .NET world.

## Installation

The preferred way to use r2rml4net is by its CLI, installed a a dotnet tool package 

```
dotnet tool install -g r2rml4net-cli
```

Alternatively, the library `r2rml4net` NuGet package can be installed and used from code.

## Usage

### Generating triples from R2RML mapping

The default command is to process a mapping graph against an SQL database to produce quad data. The CLI requires a standard .NET [connection string][conn] and a path to the R2RML mappings. The path can be a single RDF file or a directory, in which case all direct children will be loaded.

```
r2rml4net \
  -c "Server=SQLEXPRESS;Database=Northwind;User Id=myUsername;Password=myPassword;" \
  -m "./mappings/"
```

Without additional parameters the resulting data will be streamed to the standard output as n-quads.

For full help run `r2rml4net rml --help`

### Generating Direct Mapping triples

It is also possible to perform a direct mapping of the source database using a generated [default mapping](https://www.w3.org/TR/2012/REC-r2rml-20120927/#default-mappings).

In this mode only the connection string is required.

```
r2rml4net direct -c "Server=SQLEXPRESS;Database=Northwind;User Id=myUsername;Password=myPassword;"
```

Similarly, without additional parameters the resulting data will be streamed to the standard output as n-triples.

For full help run `r2rml4net direct --help`

### Generating default mapping

Finally, a default mapping document can be generated. It will be printed to standard output as turtle.

```
r2rml4net default-mapping -c "Server=SQLEXPRESS;Database=Northwind;User Id=myUsername;Password=myPassword;"
```

## Usage options

### Writing triples to file (R2RML and DM)

A `-o, --output` option can be used to write the resulting dataset using a quad serialization chosen by the file extension (`.nq`, `.trig`, `.trix` or `.jsonld`). 

```
r2rml4net \
  -c "Server=SQLEXPRESS;Database=Northwind;User Id=myUsername;Password=myPassword;" \
  -m "./mappings/" \
  -o data.trig
```

**WARNING** The file is not streamed and the entire dataset is kept in memory saved once the processing finishes. For large datasets it's better to pipe the output directly or write to SPARQL endpoint. 

### Writing triples to SPARQL Endpoint (R2RML and DM)

Generated quads can be inserted into a SPARQL endpoint in batches.

```
r2rml4net \
  -c "Server=SQLEXPRESS;Database=Northwind;User Id=myUsername;Password=myPassword;" \
  -e "http://example.com/sparql" \
  --user admin \
  --pass pass \
  -batch-size 2500
```

Only the `-e, --endpoint` parameter is required.

### Base URI

A base URI can be provided with `-b, --base-uri` to all commands. The same base will be used when generating the default mapping, parsing mapping documents as well as generating triples.

### Preserving duplicate rows (DM)

A `--preserve-duplicate-rows` flag can be used with `r2rml4net direct` to generate multiple subjects for tables without primary keys.

See the [specification](https://www.w3.org/TR/2012/REC-r2rml-20120927/#default-mappings) for details.

## Feature details

As of version 0.7 r2rml4net:

* Built as .NET Core App 2.2 (CLI) and .NET Standard 2.0 (library)
* Is nearly fully compatible with the [R2RML specifications](http://www.w3.org/TR/r2rml/),
* Can load existing R2RML mapping graphs from any RDF serialization,
* Can creating R2RML graphs programmatically using a fluent API,
* Can generate R2RML graphs from relational databases (aka [Direct Mapping](http://www.w3.org/TR/rdb-direct-mapping/)),
* Can convert data from relational databases to structured data,
* Has been tested and supports Microsoft SQL Server.

## Important stuff to be done

* Support for other database managements systems

[nuget-badge]: https://badge.fury.io/nu/r2rml4net.svg
[nuget-link]: https://badge.fury.io/nu/r2rml4net
[build-badge]: https://ci.appveyor.com/api/projects/status/8y8hj6jd6d0urw6p/branch/master?svg=true
[build-link]: https://ci.appveyor.com/project/tpluscode/r2rml4net/branch/master
[conn]: https://www.connectionstrings.com