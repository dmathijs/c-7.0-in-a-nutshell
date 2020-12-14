# Chapter 11 - Other XML Technologies

## XMLReader

high-performance class for reading an XML stream in a low-level forward-only manner. It also allows asynchronous versions of most of its methods to allow reading from multiple slow sources (see chapter 14).

Use XmlReaderSettings like 'IgnoreComments' or 'IgnoreWhitespace' to customize the loading of the X-DOM. 

The XmlReaderSettings also have a property called CloseInput, which indicates wheter to close the underlying stream when teh reader is closed, by default this is false.

CDATA (character data) means that the data should be interpreted as string although it could be interpreted as XML Markup.

### Reading attributes

Attributes can be accessed by an indexer. e.g. in ```<customer id="123" status="archived" />``` the attributes can be accessed by doing ```reader["id"]``` note that after reading ReadStartElement() the attributes are gone forever.

## XMLWriter

XMLWriter is a forward-only writer of an XML Stream. The design is symmetrical to XMlReader.

Setting ConformanceLevel XmlWriterSetting to Fragment allows for multiple root nodes, something that otherwise throws an exception

### Namespaces and Prefixes

Prefixes can be added by overloading the Write* method

```csharp
writer.WriteStartElement("o", "customer", "http://oreilly.com");
writer.WriteElementString("o", "firstname", "http://oreilly.com", "Jim");
writer.WriteEndElement();
```

will result in 

```xml
<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
<o:customer xmlns:o='http://oreilly.com'>
    <o:firstname>Jim</o:firstname>
</o:customer>
```

## Patterns for using XmlReader/XmlWriter

When deserializing XML to objects, let the objects be responsible for deserializing themselves. Implement the IXmlSerializable interface to make it compatible.

Use XElement.WriteTo() to write to a XmlWriter, use XElement.LoadFrom() to parse from XmlReader.

## XSD and Schema validation

There are multiple scham's for describing XML documents. The most accepted standard is XSD, short for XML Schema Definition (vs DTD which was a precursor).

XSD documents are themselves written in XML.

### Performing schema validation

An XML document can be validated before processing it. There are a number of reasons to do so
- Less error checking
- Schema validation picks up errors you might otherwise overlook
- Error messages are detailed.

Set the XmlSchemaValidationType on the reader/writer settings.

When you are interested in picking up all the errors in the documents. XmlReader/Writer-settings allows a ValidationEventHandler delegate on which the multicast allows for handling every error when validating an exception

## XSLT (Extensible stylesheet language transformations)

It describes how to transform one XML Language into another. An example of such transformation is transforming an XML document into an XHTMLdocument