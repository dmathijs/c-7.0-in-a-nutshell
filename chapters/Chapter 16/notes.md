# Chapter 16 Networking

> Review of the OSI model
> - Application Layer (HTTP, FTP, DNS) [data]
> - Presentation Layer (SSL, TLS) [data]
> - Session Layer (NetBIOS, PPTP[Point to point tunelling protocol]) [data]
> - Transport Layer (TCP, UDP) [segments]
> - Network Layer (IP) [packets]
> - Data Link Layer (PPP, Ethernet) [frames]
> - Physical Layer (Bluetooth, Ethernet) [bits]

All System.Net.* classes apply to either the applciation layer or the transport layer. The reason you would use a class from the transport layer (e.g. TCP listener) is because a protocol you want to use hasn't been implemented in .NET (e.g. Peer to peer).

> URL is a subset of URI, URI can be e.g. http://www.amazon.com but also mailto:joe@bloggs.org

an IP address and port combination is represented in the .NET Framework by the IPEndPoint class which takes an IP address in it's constructor.

## Client-Side Classes

WebRequest and WebResponse are the common base classes for managing both HTTP and FTP client-side activity.

HTTPClient was added in Framework 4.5 and add functionality to help you work with HTTP-based web APIs, REST-based services and authentication. Something that was missing from WebClient.

> By default, the CLR throttles HTTP concurrency. If you plan
to use asynchronous methods or multithreading to make
more than two requests at once (whether via WebRequest , Web
Client or HttpClient ), you’ll need to first increase the concurrency limit via the static property ServicePointManager
.DefaultConnectionLimit.

## WebClient

WebClient does have a dispose method but it does nothing useful at runtime, so there's no need to dispose.

> Important to note is that in e.g. ```await wc.DownloadFileTaskAsync(..., ...)``` the suffix **TaskAsync** disambiguates the method from the old EAP-based (Event-based asynchronous pattern) asynchronous methods with the **Async** suffix.

using the same ```WebClient``` object to perform more than one operation in sequence should be avoided if you're relying on cancellation or progress reporting as it may result in race conditions.

## WebRequest and WebResponse

The static ```Create``` method instantiates a subclass of the WebRequest and depending of the format of the URI will create either a HttpWebRequest, a FtpWebRequest or a FileWebRequest.
Prefixes can be added by calling the ```.RegisterPrefix``` method on the WebRequest.

A WebRequest can only be used once.

## HttpClient

HttpClient was developt to provide a better experience than webClient which was made for fetching simple pages. 

- A single HttpClient instance supports concurrent requests. With webclients a new one needs to be instantiated for every concurrent request
- HttpClient lets you write custom message handlers.
- HttpClient has a richer and extensible type system for headers and content.

To get the best performance the same HttpClient *must* be used repeatedly.

To instantiate the HttpClient with custom options. First a HttpClientHandler needs to be instantiated that is than passed to the constructor of the HttpClient.

Send a message by instantiating a HttpRequestMessage, this allows to add custom headers and changing the content itself.

### HttpMessageHandler

HttpClient's SendAsync calls the HttpMessageHandler's SendAsync. Subclassing this allows for effective extensibility.

It is possible to chain handlers by using DelegatingHandler. This can be done to implement custom authentication, compression and encryption protocols. 

A proxy can be set on the HttpClientHandler to allow specific settings. Important is that you need to set your Proxy property to null on WebCliend and WebRequest or otherwise .NET may try to *"auto-detect"* your proxy settings which may add 30 seconds to the request.

### Exception handling

Exception handling can be done by accessing a WebException's Status property, this returns an Enum that can have a lot off different values. Getting the status code can be done by casting the enum value to an int.

## Working with Http

### Headers

WebClient allows to maipulate both the Headers property on the WebClient and read the ResponseHeaders from the response by accessing the ResponseHeaders property.

HttpClient allows setting defaults for all requests using the ```DefaultRequestHeaders``` property. The HttpRequestMessage can be used to send specific messages.

Encoding/Decoding can be done by using Uri extension methods.

### Cookies

by default cookies get discarded by the HttpWebRequest. Add a ```.CookieContainer``` to ensure that the cookies are correctly received.

### SSL

WebClient, HttpClient and WebRequest all use SSL automatically when you specify an "https:" prefix. It is possible to add a Custom Certificate Validation Callback as fallback.

## Writing an HTTP Server

It is possible to write our own Http Server by using HttpListener. Internally, HttpListener does not use .NET Socket objects; instead it uses windows HTTP Server API which allows applications on a computer to listen on the same IP address and port--as long as each registers different address prefixes. 

## Using DNS

```DNS.GetHostAddresses()``` returns all IP addresses for a domain name. It can be an advantage to resolve the IP of a domain name using DNS, this will prevent excessive round-tripping

## Sending mail with SMTP client

The ```System.Net.Mail.SmtpClient``` allows for sending email through Simple Mail Transfer Protocol.

## Using TCP

with TCP in .NET, either the TcpClient and TcpListener facade classes can be used or the Socket class which is exposed on the TcpClient and -Listener using the Client property.

An example of a TCPClient would be:

```csharp
using (TcpClient client = new TcpClient())
{
    client.Connect ("address", port);
    using (NetworkStream n = client.GetStream())
    {
    // Read and write to the network stream...
    }
}

```

```await Task.Yield()``` ensures that the rest of a method is executed asynchronously.



## Extra: The synchronization context: https://hamidmosalla.com/2018/06/24/what-is-synchronizationcontext/


