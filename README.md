# Skytable client for .NET

### This library is a work in progress. Usable but not battle tested. Be wary of breaking changes. See [TODO](#todo)

## Introduction

The .NET driver for Skytable is a client driver for the free and open source NoSQL database [Skytable](https://github.com/skytable/skytable) ported from the official [Skytable client](https://github.com/skytable/client-rust). First, go ahead and install Skytable by following the instructions [here](https://docs.skytable.io/getting-started). This library supports all Skytable versions that work with the [Skyhash 1.1 Protocol](https://docs.skytable.io/protocol/skyhash).
This version of the library was tested with the latest Skytable release (release [0.7.0](https://github.com/skytable/skytable/releases/tag/v0.7.0)).

## Using this library

Once you have Skytable installed and running, you're ready to try the example!

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
        connection.Connect();

        // Serializes and stores the Person as a JSON string in Key `P`.
        // Response is a SkyResult<Response> which indicates the result of the action.
        var setResponse = connection.Set("P", setPerson);

        // Contains a SkyResult<Person> deserialized from the JSON string retrieved with the Key `P`.
        var getResponse = connection.Get<Person>("P");
    }
}
```
Keep in mind that you can always set up your own custom queries to perform other queries than `SET/GET`, see the projects in `Examples` for more details on how to use this library.

<a name="todo"></a>
## TODO:
- [ ] Examples
  - [ ] TLS ConnectionBuilder Example
- [X] Create Benchmark project
- [ ] Set up Releases
    - [ ] Set up GHA
    - [ ] Set up nuget
- [X] Add TLS support (Can connect over TLS but certificate validation might not be secure)
- [ ] Parse element types
    - [ ] RECURSIVE ARRAYS
- [X] Pipelined queries
- [ ] Actions
  - [ ] DBSIZE
  - [X] DEL
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
  - [X] POP
  - [ ] SDEL
  - [ ] SET
  - [ ] SSET
  - [ ] SUPDATE
  - [ ] UPDATE
  - [X] USET
- [ ] DDL
  - [ ] CREATE
  - [X] USE
  - [ ] INSPECT
  - [ ] DROP
- [ ] Tests
    - [ ] Queries
    - [ ] Parser

## Contributing

Feel free to contribute by creating Issues to report bugs or wanted features. Enjoy!
