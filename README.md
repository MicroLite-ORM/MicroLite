MicroLite ORM Framework
=======================

MicroLite is a small lightweight or "micro" object relational mapping (ORM) framework written in C# for the Microsoft .NET framework.

_Headline Features_

* It only references the .NET base class libraries (no dependencies outside the .NET framework itself).
* Extension support for logging via [log4net](https://github.com/TrevorPilley/MicroLite.Logging.Log4Net#microlitelogginglog4net) or [NLog](https://github.com/TrevorPilley/MicroLite.Logging.NLog#microliteloggingnlog)
* Extension support to quickly develop applications using [ASP.NET MVC](https://github.com/TrevorPilley/MicroLite.Extensions.Mvc#microliteextensionsmvc) and [WebApi](https://github.com/TrevorPilley/MicroLite.Extensions.WebApi#microliteextensionswebapi)
* Native support for Enum, Uri, and XDocument conversion
* An extensible pipeline which allows 'plugging in' custom [Type Converters](http://microliteorm.wordpress.com/type-converters/) or [Session Listeners](http://microliteorm.wordpress.com/listeners/)
* Easily managed by IOC containers
* Interface based API which makes unit testing easy

## Supported .NET Framework Versions

The NuGet Package contains binaries compiled against:

* .NET 3.5 (Client Profile)
* .NET 4.0 (Client Profile)
* .NET 4.5

## SQL Support

* Microsoft SQL Server 2005 or newer
* SQLite 3.7.14 or newer (_older versions may work but are un-tested_)
* PostgreSQL 9.2 or newer (_older versions may work but are un-tested_)
* MySql 5.5 or newer (_older versions may work but are un-tested_)

To find out more, head over to the [Getting Started](https://github.com/TrevorPilley/MicroLite/wiki/Getting-Started) guide on the wiki and see how easy it is to use!

[![Powered by NDepend](https://github.com/TrevorPilley/MicroLite/raw/master/tools/PoweredByNDependLogo.PNG)](http://ndepend.com/)