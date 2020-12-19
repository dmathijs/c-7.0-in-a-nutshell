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