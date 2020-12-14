# Chapter 12 - Disposal and Garbage collection

Some objects require explicit teardown, e.g. locks, unmanaged objects, .. this is supported through the ```IDisposable``` interface. The memory used by these objects needs to be reclaimed this function is known as garbage collection and is performed by the CLR

Disposal: instigated <-> Garbage collection: automatic

## IDisposable, Dispose and Close

C#'s using statement provides a syntactic shortcut for calling Dispose on objects that implement IDisposable. 

### Standard Disposal Semantics

Protocol for disposal in C# ensures that disposable object x 'owns' disposable object y, x's Dispose method automatically calls y's Dispose method.

Some types define a method called Close in addition to dispose
 - Functionally identical to Dispose
 - A functional subset of Dispose

E.g. IDbConnection: A closed connection can be re-opened; a Disposed connection cannot.

### When to dispose

"In doubt, dispose". Objects wrapping unmanaged resources will nearly always require disposal.
Sometimes, it is unnecessary to dispose an object as it would increase the complexity of your program. examples are WebClient, StringReader, StringWriter.

### Opt-in disposal

when using C#'s using construct, there's a temptation to extend the reach of IDisposable to nonessential activites. A solution for this i susing the opt-in disposal pattern

```csharp
public sealed class HouseManager : IDisposable
{
    public readonly bool CheckMailOnDispose;

    public HouseManager(bool checkMailOnDispose){
        ...
    }

    public void Dispose()
    {
        if(CheckMailOnDispose) CheckTheMail();
    }
}
```

The pattern might look simple, yet until Framework 4.5, it escaped StreamReader and StreamWriter. The result is that they both expose a 'Flush' method for essential cleanup.

### Clearing Fields in Disposal

It's good practice to unsubscribe from events that the object has subscribed to internally over its lifetime ( see "Managed Memory Leaks" ). This will prevent unintentilonally keeping the object alive in the eyes of the GC

To prevent the ObjectDisposedException from raising, a property can be added which can be used to determine wheter an objects' consumer can still use the object.

```csharp
public bool IsDisposed {get; private set;}
```

## Automatic Garbage Collection