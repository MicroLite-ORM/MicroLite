MicroLite
=========

|Service|Status|
|-------|------|
||![Nuget](https://img.shields.io/nuget/dt/MicroLite)![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/MicroLite)![Nuget](https://img.shields.io/nuget/v/MicroLite)|
|/develop|[![Build Status](https://dev.azure.com/trevorpilley/MicroLite-ORM/_apis/build/status/MicroLite-ORM.MicroLite?branchName=develop)](https://dev.azure.com/trevorpilley/MicroLite-ORM/_build/latest?definitionId=36&branchName=develop)|
|/master|[![Build Status](https://dev.azure.com/trevorpilley/MicroLite-ORM/_apis/build/status/MicroLite-ORM.MicroLite?branchName=master)](https://dev.azure.com/trevorpilley/MicroLite-ORM/_build/latest?definitionId=36&branchName=master)|
||![GitHub last commit](https://img.shields.io/github/last-commit/MicroLite-ORM/MicroLite) ![GitHub Release Date](https://img.shields.io/github/release-date/MicroLite-ORM/MicroLite)|

MicroLite is a .NET 4.5 library providing a small lightweight or "micro" object relational mapping (ORM) for the Microsoft .NET framework. Its purpose is to provide a flexible and powerful feature set whilst maintaining a simple and concise API.

_Headline Features_

* It only references the .NET base class libraries (no dependencies outside the .NET framework itself).
* Extensions to allow MicroLite log messages to be logged via [log4net](https://github.com/MicroLite-ORM/MicroLite.Logging.Log4Net#microlitelogginglog4net) or [NLog](https://github.com/MicroLite-ORM/MicroLite.Logging.NLog#microliteloggingnlog)
* Extension support to quickly develop applications using [ASP.NET MVC](https://github.com/MicroLite-ORM/MicroLite.Extensions.Mvc#microliteextensionsmvc) and [WebApi](https://github.com/MicroLite-ORM/MicroLite.Extensions.WebApi#microliteextensionswebapi)
* Native support for Enum, Uri, and XDocument properties on mapped classes
* An extensible pipeline which allows 'plugging in' custom [Type Converters](https://github.com/MicroLite-ORM/MicroLite/wiki/Type-Converters) or [Session Listeners](https://github.com/MicroLite-ORM/MicroLite/wiki/Listeners)
* Easily managed by IOC containers
* Fluent configuration API
* Fluent SQL Builder which makes it easier to create more complex queries
* Interface based API which makes unit testing easy

## SQL Support

* [Microsoft SQL Server](https://github.com/MicroLite-ORM/MicroLite/wiki#configuring-the-connection) 2005 or newer
* [MySql](https://github.com/MicroLite-ORM/MicroLite/wiki/Using-MySql) 5.5 or newer
* [SQLite](https://github.com/MicroLite-ORM/MicroLite/wiki/Using-SQLite) 3.7 or newer
* [PostgreSQL](https://github.com/MicroLite-ORM/MicroLite/wiki/Using-PostgreSQL) 9.2 or newer
* [Firebird](https://github.com/MicroLite-ORM/MicroLite/wiki/Using-Firebird) 2.5 or newer

## Supported .NET Versions

The NuGet Package contains binaries compiled against:

* .NET Framework 4.5

To find out more, head over to the [Wiki](https://github.com/MicroLite-ORM/MicroLite/wiki).
