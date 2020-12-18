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

### Resurrection

Finalizer modifies a living objects so that it refers back to the dying object. The reference causes the GC to skip the object on the next collection. 

What to do if finalizer contains exception prone code? Add object to static list of failed finalizations that can than be read out.

> ConcurrentQueue can be used to achieve this, the CLR reserves the right to execute finalizaers on more than one thread in parallel. Also, dequeueing may happen while another thread adds to the queue, ConcurrentQueue will ensure that this doesn't throw exceptions.

### GC.ReRegisterForFinalize

A resurrection's finalizer will not run a second time, unless GC.ReRegisterForFinalize is called on the object.

Take care when using this. **Calling GC.ReRegisterForFinalize twice will cause the finalizer to be runned twice.**

## How the garbage collector works

The standard CLR uses a generational mark-and-compact GC that performs automatic memory management. The GC initiates a garbage collection upon memory allocation (using the new keyword), after a certain threshold of memory is used, or to reduce the memory footprint of the application. The collection can be started manually by calling System.GC.Collect. During GC all threads may be frozen.

The GC begins with the root object references and marking all the objects that it touches as reachable. All the objects than have not been marked are considered unused and are collected.

The remainder of the existing items is shifted to the start of the heap to free space for more objects. **The compaction** serves 2 purposes, it avoids memory fragmentation and allows the GC to allocate memory to the end of the heap for any new items. THis prevents a time-consuming task of listing all free memory segments.

If insufficient space to allocate memory. OutOfMemoryException is thrown.

### **Optimization techniques**

### General collection

The GC is generational. The managed heap gets divided in 3 generations. Gen 0 for new objects, Gen 1 for objects that have survived 1 collection and Gen 2 for all other objects.

Gen 0 is added upon a new allocation, it is capped in size and overflow will cause GC to run, moving the Gen 0 objects to Gen 1 if not deleted, same for Gen1 to Gen2.

### The large object heap

For objects bigger than 85KB, the GC uses a separate heap called the **Large Object Heap** (LOH) which prevents excessive Gen0 collection.

LOH is not subject to compaction, moving large blocks in memory to the beginning of the heap space would be prohibitively expensive. This has 2 consequences

- Allocations are slower, the GC can't simply allocate to the end of the heap but has to look for gaps. For this a linked list of free memory block is kept.
- LOH is subject to fragmentation. That means that freeing an object can create a hole in the LOH that may be hard to fill. E.g. a hole left by a 86Kbyte object can be filled only by an object of between 85K (minimum size for using LOH) and 86K.

In cases where the latter can cause issues, you can call

```csharp
GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
```

The large object heap is also nongenerational all objects are treated as Gen2.

#### **Concurrent and background collection**

The GC must freeze execution theads for periods during a collection. This includes the entire period during which a Gen0 or Gen1 collection takes place.

The GC makes a special attempt to allow threads during a Gen2 collection as it's undesirable to freeze the application for 100ms+

This optimization (called background collection) only applies to the workstation version of the CLR. The rationale is that the latency from a blocking collection is less likely to be a problem for server applications that don't have a user interface.

#### **GC notifications (Server CLR)**

The server version of the CLR can notify you just before a full GC will occur. The idea is that requests can be diverted to another server just before collection. To start notification, GC.RegisterForFullGCNotification, this will return a GCNotificationStatus that will allow re-routing of requests in case of necessary.

### **Forcing Garbage collection**

System.GC.Collect, can be used. If integer passed only passing generation is collection GC.Collect(0) will run collection against generation 0.

It's better to not use it as it may upset the GC's self-tuning ability, where the GC tweaks itself for each generation to maximize performance as the application executes.

The Collect() call may be usefull when running tasks every 24h. After the task the GC will not collect and thus the application will keep running with high memory consumption. This can be done by doing

```csharp
GC.Collect();
GC.WaitForPendingFinalizers(); -> allows finalizer queue to finish
GC.Collect(); -> collect objects for which finalizers have run
```
### **Tuning Garbage Collection**

This can be done using the GCSettings.LatencyMode. E.g. changing from the *default* Interactive to LowLatency. Instructs the CLR that quicker collections are favored, this may be usefull for real-time applications

### **Memory Pressure**

When using unmanaged memory to the application. The CLR will get a unrealistically optimistic perception of its memory usage, because the CLR knows only about managed memory. This can be mitigated by telling the CLR to assumea specified quantity of unmanaged memory has been allocated, this can be done by calling GC.AddMemoryPressure.

## Managed Memory Leaks

Usually managed memory leaks are easier to diagnose and prevent. Managed memory leaks  are caused by unused objects remaining alive. A common candidate is event handlers, these hold reference to the target object.

Solution is to make objects IDisposable and remove them during the dispose call
```csharp
public void Dispose() { _host.Click -= HostClicked; }
```

Another example is Timers, starting a timer without disposing it, will prevent objects containing the time object to be GC. (System.Timers). That is because the .NET Framework itself holds a reference to active timers so that their Elapsed events can be raised.

The Timer in System.Threading is different, the .NET Frameworks keeps no references to active threading timers and thus a finalizer will be ran, which automatically disposes and stops a timer.

Memory profiling tools: windbg.exe, Microsoft's CLR Profiler, SciTech's Memory Profiler and Red Gate's ANTS Memory Profiler.

## Weak References

```WeakReference``` class allows to keep a weak reference to an object, it can be usefull to keep track to keep of all items that have been instantiated without preventing the items to be collected by the GC.

```csharp
var weak = new WeakReference(new StringBuilder("weak"));
var sb = (StringBuilder) weak.Traget;
if(sb != null) { /* so something with wb */ }
```

### Weak References and Caching

Use a weakreference to keep a field value cached, ofcourse this may not really be effective in practice as the GC fires and selects what generation to collect.

### Weak References and Events

Before it was clear that events would keep strong reference and thus prevent items from being collected.
