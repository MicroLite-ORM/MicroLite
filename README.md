MicroLite ORM Framework
=======================

[![NuGet version](https://badge.fury.io/nu/MicroLite.svg)](http://badge.fury.io/nu/MicroLite)

MicroLite is a small lightweight or "micro" object relational mapping (ORM) framework written in C# for the Microsoft .NET framework. Its purpose is to provide a flexible and powerful feature set whilst maintaining a simple and concise API.

_Headline Features_

* It only references the .NET base class libraries (no dependencies outside the .NET framework itself).
* Extensions to allow MicroLite log messages to be logged via [log4net](https://github.com/TrevorPilley/MicroLite.Logging.Log4Net#microlitelogginglog4net) or [NLog](https://github.com/TrevorPilley/MicroLite.Logging.NLog#microliteloggingnlog)
* Extension support to quickly develop applications using [ASP.NET MVC](https://github.com/TrevorPilley/MicroLite.Extensions.Mvc#microliteextensionsmvc) and [WebApi](https://github.com/TrevorPilley/MicroLite.Extensions.WebApi#microliteextensionswebapi)
* Native support for Enum, Uri, and XDocument properties on mapped classes
* An extensible pipeline which allows 'plugging in' custom [Type Converters](https://github.com/TrevorPilley/MicroLite/wiki/Type-Converters) or [Session Listeners](https://github.com/TrevorPilley/MicroLite/wiki/Listeners)
* Easily managed by IOC containers
* Fluent configuration API
* Fluent SQL Builder which makes it easier to create more complex queries
* Interface based API which makes unit testing easy

## Supported .NET Framework Versions

The NuGet Package contains binaries compiled against:

* .NET 3.5 (Client Profile)
* .NET 4.0 (Client Profile)
* .NET 4.5

## SQL Support

* [Microsoft SQL Server](https://github.com/TrevorPilley/MicroLite/wiki#configuring-the-connection) 2005 or newer
* [MySql](https://github.com/TrevorPilley/MicroLite/wiki/Using-MySql) 5.5 or newer
* [SQLite](https://github.com/TrevorPilley/MicroLite/wiki/Using-SQLite) 3.7 or newer
* [PostgreSQL](https://github.com/TrevorPilley/MicroLite/wiki/Using-PostgreSQL) 9.2 or newer
* [Firebird](https://github.com/TrevorPilley/MicroLite/wiki/Using-Firebird) 2.5 or newer
* [SQL Server Compact Edition](https://github.com/TrevorPilley/MicroLite/wiki/Using-SqlServerCE) 4.0 or newer

To find out more, head over to the [Getting Started](https://github.com/TrevorPilley/MicroLite/wiki) guide on the wiki and see how easy it is to use!

[![Powered by NDepend](https://github.com/TrevorPilley/MicroLite/raw/master/tools/PoweredByNDependLogo.PNG)](http://ndepend.com/)
