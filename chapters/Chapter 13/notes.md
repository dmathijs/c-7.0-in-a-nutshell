# Chapter 13 - Diagnostics

the .NET Framework provides a set of facilities to log diagnostic information, monitor application behavior, detect runtime errors, and integrate with debugging tools if available.

The .NET Framework also allows code contracts to be inforced, fiolating these will fail early.
Types in this chapter are defined primarily in the System.Diagnostics namespace.

## Conditional Compilation

Preprocessor directives can be used to allow code to run in certain scenario, e.g. add logging when build in to debugging mode.

use the /define: switch to add symbols assembly-wide.

### Conditional Compilation Versus Static Variable Flags

Why not use static variable flags instead of preprocessor directives?
- Conditionally including an attribute
- Changing the declared type of variable
- Switching between different namespaces or type aliases
- Debugging code can refer to assemblies that are not included in deployment

### The Conditional attribute

```csharp
[Conditional("Logging Mode")]
static void LogStatus (string msg)
{

}
```

If "Logging Mode" symbol is not defined, all calls to LogStatus get ommitted during the compilation phase, including their argument evaluation expressions. The conditional attribute is ignored at runtime it's purely an instruction to the compiler.

## Debug and Trace Classes

Debug and Trace are static classes that provide basic logging and assertion capabilities. Debug class is intended for debug builds, trace for both.

The Debug and Trace classes both provide Fail and Assert methods.

### TraceListener

The Debug and Trace classes each have a Listeners property, comprising a static collection of TraceListener instances. By default each collection includes a single listener. When a Fail method is called a dialog appears asking if the user want to continue, *independent of wheter a debugger is attached.*

This behavior can be changed by writing an own tracelistener or using one of the predefined types.
- TextWriterTraceListener
- EvenetLogTraceListener
- EventProviderTraceListener
- WebPageTraceListener

TraceListener also has a Filter of type TraceFilter that can be used to filter messages out of the logs. Other options as indentLevel and IndentSize can be controlled in TraceOutputOptions property.

### Flusing and closing listeners

When writing to a tracelistener, it is possible that the output stream doesn't appear in a file immidiately. To 'force' this, the flush or close (implicitely calls flush) method can be called.

> It is good policy to set autoflush to true on Debug and Trace if you're using any file- or stream-based listeners. Otherwise, if an unhandled exception or critical error occurs, the last 4KB of diagnostic information may be lost.

## Debugger Integration

In deployment, the debugger is likely to be:
- DbgCLR
- One of the lower-level debugging tools, such as WinDbg, Cordbg or Mdbg

Calling Debugger.Code will automatically attach the debugger to that execution point, once attached the debugger's output window will be lused for logging messages.

### Debugger Attributes

2 debugger attributes
- [DebuggerStepThrough], step through a function without any user interaction
- [DebuggerHidden] will hide the method in the callstack and operate as it was not there.

## Processes and Process Threads

Chapter 6 -> Process.Start.

### Examining Running Processes

List processes with Process.GetProcesses(), if it need to be killed, call .Kill() method.

### Examining Threads in a Process

Access threads within process by calling Process.Threads, this will return a ProcessThread object (**not** a System.Threading.Thread). Gives some diagnostic information about the thread

### StackTrace and StackFrame

StackTrace represents the complete call stack while StackFrame represents a single method call within the stack. 

PDB Files are generated if build with the /debug switch.
Interesting to know is that in DEBUG, No-ops are added to the IL to be able to determine the line number of the call. When using optimization this advantage drops and makes it harded for the CLR to determine the line number of a call.

Pass Thread to StackTrace's constructor to get stacktrace of a thread.

## Windows Event Logs

The EventLog is not available to .NET Core apps!
The 3 standard windows event logs are:
- Application
- System
- Security

As the name implies, applications often write to the application windows event log.

### Write to the event log

1. Choose one of 3 even logs
2. Decide on a source name and create if necessary
3. Call EventLog.WriteEntry with the log name, source name and message data

### Reading the event log

Via the EntryWritten EventHandler, events can be tracked
```csharp
log.EnableRaisingEvents = true;
log.EntryWritten += DisplayEntry;
```

## Performance Counters

Performance counters allow checking **real-time** information about the health state of the application. E.g of a performance counter is the .NET CLR Memory

In the following sections, the usage of performance counters is described in more detail.
