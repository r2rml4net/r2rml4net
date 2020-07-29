# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/saintedlama/versionize) for commit guidelines.

<a name="0.7.0"></a>
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

