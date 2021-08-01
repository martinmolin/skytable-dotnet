# Skytable client for .NET

### This library is a work in progress. It is not ready for usage yet. See [TODO](#todos)

## Introduction

The .NET driver for Skytable is a client driver for the free and open source NoSQL database [Skytable](https://github.com/skytable/skytable) ported from the official [Skytable client](https://github.com/skytable/client-rust). First, go ahead and install Skytable by following the instructions [here](https://docs.skytable.io/getting-started). This library supports all Skytable versions that work with the [Skyhash 1.0 Protocol](https://docs.skytable.io/protocol/skyhash).
This version of the library was tested with the latest Skytable release (release [0.6](https://github.com/skytable/skytable/releases/v0.6.0)).

## Using this library

This library only ships with the bare minimum that is required for interacting with Skytable. Once you have
Skytable installed and running, you're ready to try the example!

Example usage:
```cs
using System;
using Skytable.Client;

class Person : Skyhash
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
Keep in mind that you can always set up your own custom queries to perform other queries than `SET/GET`, see the `Skytable.Client.Example` project for more details.

<a name="todos"></a>
## TODO:
- [ ] Add TLS support
- [X] Document public API
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
