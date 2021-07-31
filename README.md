# Skytable client for .NET

### This library is a work in progress. It is not ready for usage yet. See [TODO](#todos)

## Introduction

This library is an inofficial client for the free and open-source NoSQL database
[Skytable](https://github.com/skytable/skytable). First, go ahead and install Skytable by
following the instructions [here](https://docs.skytable.io/getting-started). This library supports
all Skytable versions that work with the [Skyhash 1.0 Protocol](https://docs.skytable.io/protocol/skyhash).
This version of the library was tested with the latest Skytable release
(release [0.6](https://github.com/skytable/skytable/releases/v0.6.0)).

## Using this library

This library only ships with the bare minimum that is required for interacting with Skytable. Once you have
Skytable installed and running, you're ready to follow this guide!

Example usage:
```cs
using System;
using Skytable.Client;

class Person : Skyhash<Person>
{
    public string Name { get; set; }
    public int Age { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        var setPerson = new Person();
        setPerson.Name = "John Doe";
        setPerson.Age = 30;

        var connection = new Connection("127.0.0.1", 2003);
        var setResponse = connection.Set("P", setPerson);
        var getResponse = connection.Get<Person>("P");
    }
}
```
<a name="todos"></a>
## TODO:
- [X] Parse element types
  - [X] STRING
  - [X] U64
  - [X] ARRAY
  - [X] RESPCODE
  - [X] FLATARRAY
- [ ] Pipelined queries
- [ ] Actions
  - [ ] DBSIZE
  - [ ] DEL
  - [ ] EXISTS
  - [ ] FLUSHDB
  - [X] GET
  - [ ] HEYA
  - [X] SET
  - [ ] KEYLEN
  - [ ] LSKEYS
  - [ ] MGET
  - [ ] MKSNAP
  - [ ] MSET
  - [ ] MUPDATE
  - [ ] POP
  - [ ] SDEL
  - [ ] SET
  - [ ] SSET
  - [ ] SUPDATE
  - [ ] UPDATE
  - [ ] USET
- [ ] DDL
  - [ ] CREATE
  - [ ] USE
  - [ ] INSPECT
  - [ ] DROP
- [ ] Tests
    - [ ] Queries
    - [ ] Parser
