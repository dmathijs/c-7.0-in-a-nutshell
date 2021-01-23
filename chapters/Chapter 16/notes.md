# Chapter 15 Streams and I/O

- The .NET stream architecture for reading and writing across a variety of I/O types.
- Classes for manipulation files and directories on disk
- Specialized streams for compression, named pipes and memory-mapped files.
#
- System.IO -> low level I/O
- LINQ2SQL, LINQ2XML -> high level I/O

## Stream architecture

The .NET stream architecture centers on three concepts: backing stores, decorators and adapters.

A **backing store** is the endpoint that makes input and output useful, such as a file or network connection. Precisely it is either of these:
- A Source from which bytes can be sequentially read
- A destination to which bytes can be sequentially written

Unlike an array, where all the backing data exists in memory at once, a stream delas with data serially (either one byte at a time or in blocks of a manageable size)

Streams fall in 2 categories
- Backing store streams
  - These are hard-wired to a particular type of backing store, such as FileStream or NetworkStream
- Decorator streams
  - These feed off another stream, transforming the data in some way, such as DeflateStream or CryptoStream

Decorator streams have the following advantages:
- They liberate backing store streams from having to implement features like compress/encrypt themselves.
- Decorators connect at runtime
- Streams don't suffer interface change when decorated
- Decorator streams can be chained together.

It is possible to used adapters to bridge the gap between streams which use bytes exclusively and e.g. the application that use text. For example, the text reader exposes a ReadLine method.

## Using Streams

The abstract Stream class is the base for all streams. It defines methods and properties for 3 fundamental operations
1. Reading
2. Writing
3. Seeking
as well as for administrative tasks such as closing, flushing and configuring timeouts.

### Reading and Writing

A stream may support both reading and/or writing. If CanWrite returns false, the stream is read-only; if CanRead returns false, the stream is write-only.

Stream.Read() will return the total number of bytes read, this may be equal or smaller than the .Count of the buffer.

### Seeking

With a seekable stream you can query or modify its Length and at any time change the Position at which you're reading or writing. 

> When moving positions a lot in a stream, it meay be interesting to use MemoryMappedFile (see later.)

When using a non seekable stream, you have to close the stream and start fresh with a new one if you want to reread a previous section.

### Closing and Flushing

In General, streams follow the following standard disposal semantics:
- Dispose and Close are identical in function
- Closing a decorator stream closes both the decorator and the backing store stream, when chaining closing the outermost stream will close them all.
- Flushing is automatically done when closing a stream

Timeout of streams is supported using Read- and Write Timeout.

### Thread safety

Streams are not thread-safe. This means that 2 threads can not read or write to the same stream. To support this, the Stream class offers a simple workaround via the static Synchronized method. This method accepts a stream and returns a thread-safe wrapper. Internally, a read/write locking happens which prevents the threads from accessing the stream concurrently but allows them to write to the same stream (albeit throttled)

## Backing store streams

### File Stream

```File.OpenRead(" ")``` opens a new stream. ```File.ReadLine("...");``` returns a lazily-evaluated ```IEnumerable<string>```. This is more efficient because it doesn't load the entire file into memory at once.

Never rely on relative paths within the application instead use something like:
```
string baseFolder = AppDomain.CurrentDomain.BaseDirectory;
string logoPath = Path.Combine(baseFolder, "test.jpg");
WriteLine(File.Exists(logoPath))
```

It is possible to lock specific parts of a requested file with the ```Lock``` and ```UnLock``` method of the FileStream. 

### Memory Stream

Memory streams aren't actually real streams as they live completely in memory. They are usefull because they can behave as streams and because random access is possible in an nonseekable stream.
Close/Flushing is optional on a MemoryStream.

### Pipestream

Pipestream provides a simple means by which one process can communicate with another through the Windows pipes protocol. There are 2 kinds of pipe:
1. Anonymous pipe (faster)
   1. One-way communcation between parent and child process
2. Named pipe (more flexible)
   1. Allows two-way communication between processes on the same computer or different computers across a windows network.

Another way to communicate between processes is using a shared block of memory **"Memory Mapped Files"**

PipeStream is abstract and has 4 implementations
- Anonymous pipes
  - AnonymousPipeServerStream and AnonymousPipeClientStream
- Named pipes
  - NamedPipeServerStream and NamedPipeClientStream

### Named Pipes

The named pipes protocol defines 2 distinct roles: the client and the server. 
- The server instantiates a NamedPipeServerStream and then calls WaitForConnection.
- The client instantiates a NamedPipeClientStream and then calls Connect

PipeTransmissionMode Enum can be changed to allow greater messages -> Byte vs Message.

### Anonymous types

Anonymous types do not allow an PipeTransmissionMode which means that message size has to controlled by the developer.

### Buffered Stream

Buffered stream is a decorator stream, it wraps a stream with buffering capability inherently making a stream "seekable"

Using a bufferedstream on a FileStream doesn't make much sense as a FileStream already uses a buffer internally.

## Stream Adapters

A stream deals only in bytes; to read or write in another format you must plug in an adapter.

- Text adapters
  - TextReader, TextWriter, StringReader, StringWriter, StreamReader, StreamWriter
- Binary adapters
  - BinaryReader, BinaryWriter
- XML adapters
  - XmlReader, XmlWriter

TextReader and TextWriter are abstract classes. StreamReader and StreamWriter are the adapter implementations for streams while StringReader and StringWriter work in-memory.

To construct the StreamReader or StreamWriter a encoding can be passed to ensure a correct parsing.

> Remember: ASCII is one byte, the 127 first characters of the Unicode set. UTF-16 string uses a 2-byte prefix to identify wheter the byte pairs are written in a "little-endian" or "big-endian" order (least significant byte first or last). The default little-endian order is standard for Windows-based systems.

The advantage of a StringReader of StringWriter is e.g. when we want to transform a string to an XMLDocument

XMLReader r = XmlReader.Create(new StringReader(myString));

### Binary Adapters

BinaryReader and BinaryWriter read and write native data types: bool, byte, char, decimal, float, double, short, int, long, .. aswell as strings and arrays of primitive dat atypes.

### Closing and Disposing Stream adapters

You have 4 choices:
1. Close the adapter only
2. Close the adapter, then close the stream
3. Flush the adapter, then close the stream
4. Close the stream

Options 1 and 2 are equals as closing an adapter automatically closes the underlying stream. Options 3 & 4 are valid because adapters are *optionally disposable objects*. Since .NET 4.5 it is also possible to pass a flag to the constructor of an adapter that leaves the stream open.


## Compression streams

DeflateStream and GZipStream. Decorators can be used together with adapters and backing stores.
.NET also offers ZipArchive and ZipFile utility class to manipulate ZIP archives.

## File and directory operations

System.IO contains utility classes for performing file and directory operations. E.g. ```File/Directory/Path``` or the ```FileInfo``` and ```DirectoryInfo``` instance method classes.

### The File class

Lot's of utility methods to manipulate files. 

Transparent encryption relies on a key seeded from the logged-in user's password. Transparent encryption and compression require special file system support.

### The Directory Class

The ```EnumerateFiles, EnumerateDirectories and EnumerateFileSystemEntries``` were added and are more efficient than e.g. GetFiles() because they're lazilly evaluated.

### FileInfo and FileDirectory

FileInfo and FileDirectory are ideal for chained operations on single files or directories

### Path

```Path.Combine``` Let's you combine folder/file names without having to check slashes. Can be used to get random file/directory names.

### Special folders

One thing missing from Path and Directory is a means to locate folders such as MyDocuments, Program Files, Application Data, and so on. Thi s is provided by the GetFolderPath method in the System.Environment class.

Application data (AppData) folder is usefull as it can allow a user to store settings which travel with the user accross a network (if roaming profiles are enabled).

### Catching FileSystem Events.

The ```FileSystemWatcher``` class lets you monitor a directory for activity. Important to note is that filesystemwatcher launches it's events on new threads, this means that exception handling needs to happen within the event's method.

## File I/O in UWP (universal windows platform)

In Windows 8 abnd 8.1 using FileStream or the Directory/File classes couldn't be done. This was relaxed on windows 10 although limitations still apply.

