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

