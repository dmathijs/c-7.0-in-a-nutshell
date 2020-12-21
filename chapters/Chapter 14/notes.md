# Chapter 14 - Concurrency and Asynchrony

The general mechanism by which a program can simultaneously execute code is called multithreading. Multithreading is supported by both the CLR and the OS and is a fundamental concept in concurrency.

## Threading

A thread is an execution path that can proceed independently of others. Multithreaded program runs multiple threads in a single process and has access to the same **execution environment**

**A shared state** is the data that is shared between threads.

### Creating a thread

On a single-core computer the OS must allocate *slices* of time to each thread (typically 20ms on windows)

> A thread is said to be **preempted** at the points where its execution is interspersed (= afgewisseld) with the execution of code on another thread.

You can wait for another thread to end by calling it's **JOIN** method.

```csharp
static void Main(){
    Thread t = new Thread(() => ...);
    t.Start();
    t.Join();
    Console.WriteLine("Thread t has ended!");
}
```

> Thread.Sleep(0) relinquishes the threads' current time slice immediately handing it over to the other threads, Thread.Yield() does the same thing except they are ran on the same processor.

While waiting on a Sleep or Join a thread is *blocked*

### Blocking

A thread is deemed blocked if it's execution path is paused, it will give up its time slice until its blocking condition is satisfied. Running ```.ThreadState``` on a thread will let you know in what state the thread finds itself.

When a thread blocks or unblocks, the OS performs a context switch. This incurs a very small overhead.

> An operation that spends most of it's time waiting is **I/O bound**, an operation that spends most of its time performaing CPU-intensive work is called **computebound**

#### **Blocking versus spinning**

Spinning is having a loop that is checked every x time, e.g.
```csharp
while(DateTime.Now < nextStartTime)
    Thread.Sleep(100);

// or

while(DateTime.Now < nextStartTime);
```
Not that blocking is not a ZERO COST operation, it ties up around 1MB of memory and causes onging administrative overhead for the CLR and the OS. for this a lot of blocking in heavily I/O-bound programs can be an issue.

### Local versus Shared State

CLR assigns each thread its own memory stack for local variables

> Local variables captured by a lambda expression or anonymous delegate are converted by the compiler into fields that can also be shared.

### Locking and thread safety

When two threads simultaneously contend a lock, one threads waits or blocks until the lock becomes available, locks can be used to prevent concurrent access and will be described in later chapters more in depth.

Take care of using thread starts in a loop, the index will change while looping so the result will be nondeterministic.

### Exception handling

Don't put exception handling around thread initialization, this will ause an unhandled Exception as the execution path is different. A solution is to put the exception handling in the called method.

in WPF, UWP and windows forms applications, you can subscribe to a "global" exception handling events. These fires after an unhandled exception in any part of your program that's called via the message loop. Handling the events will preent application shutdown. This is called **Centralized exception handling**

### Foreground Versus Background Threads

By default, threads are foreground threads. Foreground threads keep the application alive for as long as any of them is running. Backgroudn threads do not. This can be toggled by using the ```.IsBackground``` flag on the thread object. 

### Thread priority

Determines how much execution time it gets relative to other threads. If you want the thread to have a higher priority than threads in other processes, you must also elevate the process priority using the process class in System.Diagnostics.

Important to note is that elevating process priority can starve other processes, slowing down the entire computer.

### Signaling

Sometimes a thread needs to wait until it gets a signal from another thread. This is called signaling. 

The simplest use is the ManualResetEvent. Calling .WaitOne() on the signal will block the thread until .Set() is ran on the signal.

### Threading in Rich-Client Applications

long-running operations on the main thread makes appliation unresponsive because the main thread also processes the message loop that performs rendering and handles keyboard and mouse events.

All worker threads need to forward UI changes to the UI thread. Viaolating this causes either unpredictable behavior or an exception to be thrown.

E.g. WPF (windows presentation foundation) needs you to run ```.BeginInvoke(action)``` on the 'Dispatcher', the 'Dispatcher' being the UI element you interacted with. 

Differenet windows can get their own UI Threads (= main threads).

### Synchronization Contexts

In the System.ComponentModel namespace, there’s an abstract class called SynchronizationContext that enables the generalization of thread marshaling. (more later) Read more here https://docs.microsoft.com/en-us/archive/msdn-magazine/2011/february/msdn-magazine-parallel-computing-it-s-all-about-the-synchronizationcontext

### The Thread Pool

The thread pool is a pool of threads for which the instantiation of the call stack is already been done thus saving time. Pooled threads are always background threads. Blocking pooled threads can degrade performance.

Easiest way to run something in the threadpool is to run ```Task.Run(() => ...);``` an alternative is to use ```ThreadPool.QueueUserWorkItem(x => ..)``` 

#### **Hygiene in the threadpool**

The threadpool also ensures that there's no CPU oversubscription. Oversubscription is when there are more active threads than CPU cores, which causes the OS to have to time-slice. Oversubscripition hurts performance because time-slicing requires expensive context swithces and can invalidate CPU caches that have become essential in delevering performance to modern processors.

CLR avoids oversubscription by queueing taks and throttling their startup. **Hill-climbing algorithm** is used to adjust the workload in a particular direction.

This works best if:
- Work items are mostly short-running so that the CLR has plenty of opportunities to measure and adjust.
- Jobs that spend most of their time blocked do not dominate the pool.

Blocking is troublesome because the CLR thinks that it's loading up the CPU, therefore it will add new threads to the threadpool potentially rising the chances of oversubscription.

**When fully utilizing the CPU, good hygiene in the thread pool is very important**

## Tasks

Problem with threads
- It's easy to start thread with data but it's not so easy to get data out of the thread. Exception handling is also painfull as it needs to be done inside the execution path.
- You can't tell a thread to start something else when it's finished instead it must be joined.

These limitations, together with performance implications, discourage direct usage of threads. The ```Task``` class helps to solve the problems. Compared to a thread, a Task is a higher-level abstration. It represents a concurrent operation that may or may not be backed by a thread. Tasks are compositional (they can be chained together using continuations). They can use the threadpool to lessen startup latency and use TaskCompletionSource to leverage a callback approach that avoids threads.

A tasks status can be checked by calling the Status property. Calling ```.Wait()``` on a taks will block until the taks is complete, ```.Wait()``` also allows a timeout and acancellation token to end the wait early.

#### **Long-running tasks**

When using the threadpool, multiple long-running tasks may cause performance to suffer. Using the TaskCreateOptions.LongRunning will instantiate a new thread.

Other solutions are
- If I/O bound, TaskCompletionSource and asynchronous functions which let you implement concurrency with callbacks instead of threads.
- If compute-bound, a producer/consumer queue lets you throttle the concurrency for those tasks, avoiding starvation for other threads and processes.

### Returning a result

Task has a generic subclass called Task\<TResult> that allows to emit a return value.

### Exceptions

Tasks propagate exceptions, this means that exceptions are automatically re-thrown to whever calls Wait() or accesses the Result property of a Task\<TResult>

(!) Unhandled exceptions on autonomous tasks are called unobserved exceptions and in CLR 4.0, they would actually terminate your program (The CLR would re-throw in the finalizer thread). However, for certain patterns this caused issues so it was dropped in CLR 4.5.

### Continuations

Continuation allows to do something when finishing a task. It is usually implemented by a callback that is ran after execution of the task. The first way is

```csharp
var awaiter = taskX.GetAwaiter(); // antecedent
awaiter.OnCompleted(() => {  // OnComplete of the awaiter, 'Continuation' is ran.
    int result = awaiter.GetResult();
    Console.WriteLine(result);
})
```

An Awaiter has only 2 methods. OnCompleted and GetResult and a boolean property called IsCompleted.

If a synchronization context is present, OnCompleted automatically captures it and posts the continuation to that context. This means that the continuation will execute in the main thread. Preventing this can be done using the ```.ConfigureAwait(false)```. If no synchronization context is present or you use ```.ConfigureAwait(false)```, the continuation will execute on the same thread as the antecedent, avoiding unnecessary overhead.

Another way of attaching a continuation is by calling the tasks' ```ContinueWith``` method