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

> When using debug mode with optimizations disabled, the lifetime of an object referenced by a local variable extends to the end of the code block to ease debugging. Otherwise, it becomes eligible for collection at the earliest point at which it's no longer used.

GC is a periodically executed procedure that doesn't happen on a fixed schedule. It depends on
- Available memory
- Amount of memory allocation
- Time since last collection

> The GC doesn't collect all garbage with every collection. Instead, the memory manger divides objects into generations and the GC collects new generations more frequently than old generations (long-lived objects).

### Roots

A root is something that keeps an object alive. It can be:
- A local variable or parameter in an executing method
- A static variable
- An object on the queue that stores objects ready for finalization

A group of objects that reference each other cyclically are considered dead without a root referee.

in WinRT objects that are instantiated from C# have their lifetime managed by the CLR's garbage collector, because the CLR mediates access tot eh CoM object through an object that it creates behind the scenes called a runtime callable wrapper.

## Finalizers

Prior to an object being released from memory its finalizer runs, if it has one.

```csharp
class Test
{
    ~Test()
    {
        // finalizer logic
    }
}
```

First GC identifies objects ripe for deletion. Those without finalizer get removed right away. Those with a finalizer are put on a special queue. When garbage collection is complete. The *finalizer thread* kicks in and starts running in parallel. The queue acts as a root object, once it's dequeued and the finalizer has been executed, the object is ready for deletion in the next collection.

Guidelines on implementing finalizers
- Ensure that a finalizer executes quickly
- Never block in your finalizer
- Don't reference other finalizable objects
- Don't throw exceptions

### Calling Dispose from a finalizer

This pattern can be used as a backup for cases when a consumer simply forgets to call Dispose. However, it's then a good idea to log the failure so that you can fix the bug.