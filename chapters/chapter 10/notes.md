# Chapter 10 - LINQ to XML

## Architectural overview

### What is DOM?

All XML files start with a declaration and then a root element with attributes that can contain child elements. A tree of objects can fully describe a document. This is called a document object model or DOM.

### The LINQ to XML DOM

LINQ to XML comprises two things:
- XML DOM which is called X-DOM
- 10 supplementary query operators

X-DOM is LINQ-friendly and contains types such as XDocument, XElement, XAttribute.
X-DOM is LINQ-friendly because;
- It has methods that emit IEnumerable sequences which can be queried
- It's constructors are designed as such that you can build an X-DOM tree through a LINQ projection.

XObject is the abstract base class for all XML content. XDocument represents the root of an XML tree, it wraps the root XElement.

### Loading and Parsing

Load builds an X-DOM from a file, URI, stream or XmlRedeader. Parse builds an x-DOM from a string.

## Instantiating an X-DOM

