# Chapter 5 Framework overview

The .NET Framework is exposed via a set of managed types organized in hierarchical namespaces and packaged in set of assemblies, which together with the CLR comprise the .NET platform

- mscorlib.dll (Multi-language Standard Common Object Runtime Library) (C#'s built-in types, serialization, reflection, threading..) **-> necessary by the CLR**
- system.dll (Additional types providing XML, networking, Linq) **-> necessary for programmer**
- system.core.dll -> **necessary for programmer**

### .NET Standard 2

Making .NET Standard-compliant libraries, these will be usable across .NET framework, .NET core and Xamarin.

### Reference Assemblies

Assemblies are refrerence to be able to use their functionality. The ones you load at runtime do not have to be the same as the one at compile time. This is how .NET standard works, you load the netstandard.dll which contains all of the allowable types and members and then at runtime you load in the 'framework-specific' real assemblies.

## The CLR and Core Framework

System namespace contains C#'s built-in types, the Exception base class, the Enum, Array and Delegate, Nullable, Type, DateTime, TimeSpan and Guid.. GC class for interacting with the garbage collector.

## ASP.NET

asp.net core is not dependent on System.Web and the historical baggage of web forms.

## WPF

- Is the new version of windows forms
- Allows 3D rendering
- Fexible layout support
- Reliable data binding

WPF's size and complexity, however, create a big learning curve.

## ADO.NET

ADO.NET is the managed data access API

It has 2 layers
- Provider layer: The provider model defines common classes and interfaces for low-level access to database providers.
- DataSet model: A dataset is a structured cache of data

Above the provider layer are three APIs that offer the ability to uery databases using LINQ:
- Entity Framework (.NET framework only)
- Entity Framework Core
- LINQ to SQL (.NET framework only)

LINQ to SSQL is simpler than entity framework and has historically produced better SQL. Entity framework is more flexible in that you can create ellaborate mappings.

## Web API vs MVC API

Web API runs over ASP.NET/ASP.NET Core and is architecturally similar to Microsoft's MVC API, except that it's designed to expose services and data instead of web pages.