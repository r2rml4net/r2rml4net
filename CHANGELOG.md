# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/saintedlama/versionize) for commit guidelines.

<a name="0.8.0"></a>
## [0.8.0](https://www.github.com/r2rml4net/r2rml4net/releases/tag/v0.8.0) (2022-10-25)

### Features

* BREAKING CHANGE: support only net6 ([ec42cc6](https://www.github.com/r2rml4net/r2rml4net/commit/ec42cc6f8f548b4556410d90a7939cbe5e23f3a3))

### Bug Fixes

* do not throw when value is absolute IRI (11.2 in spec) ([d973d54](https://www.github.com/r2rml4net/r2rml4net/commit/d973d54e451681775183313e6fec3e090dd13f79))

<a name="0.7.1"></a>
## 0.7.1 (2020-8-1)

### Bug Fixes

* query endpoint cannot be null

## 0.7.0 (2020-7-29)

### Bug Fixes

* writing to console as n-quad stream
* assume UTC date/times by default

### Features

* add option to write directly to triple store over sparql
* write output only mapping succeeds
* add flag to preserve duplicate rows
* write to standard output when no out file is specified

## 0.6.3 (2020-7-25)

### Bug Fixes

* direct mapping would not save out file
* not all characters were correctly %-encoded

## 0.6.2 (2020-7-23)

### Features

* added base URI option to commands

## 0.6.1 (2020-7-23)

### Bug Fixes

* do not run direct on rml command

## 0.6 (2020-7-23)

### Bug Fixes

* select triples map by subjectMap

### Features

* added direct mapping options
* add some useful logging to CLI
* very first draft of CLI

