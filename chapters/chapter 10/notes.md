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

### Functional Construction

Although it is possible to instantiate node per node. LINQ2XML allows to create a DOM in one single expression.

```
XElement customer = 
    new XElement("Customer", new XAttribute ("id", 123),
        new XElement("firstname","joe"),
        new XElement("lastname", "bloggs",
            new xComment("nice name")
        )
    );
```

Through params object[], the XML can be build. Everything ends up in one of the two buckets: Nodes or Attributes.

## Querying and navigating

### Automatic deep cloning

When adding an element to a parent, that parent is set on the Element. When you add a node to a second parent, that node will be deep-cloned. This keeps the X-DOM free of side effects, another hallmark of functional programming.

When using the ```.Element(xyz)``` query operator remember that if the xyz element does not exist, this will throw an exception. This can be solved by doing ```Element("xyz")?.Value``` instead. 

To Access the XDocument (root), a .Document propoerty is available on any of the nodes.

When using the Ancestors() query operator, the first element will be the Parent, followed by the Parent.Parent and so on. If the highest parent is wished for, another possibility is use the Document.Root (which is only possible if an XDocument is present).

**Internally nodes are stored as a linked list**

## Updating an X-DOM

- Call SetValue or reassign the Value
- Call SetElementValue or SetAttributeValue
- Call one of the RemoveXXX methods
- Call one of the AddXXX or ReplaceXXX methods

Using ```.SetValue()``` will replace al childnodes

When using ```.Remove()``` all matching elements are first added to a temporary list after which they are enumerated and the elements removed. This avoids errors that could otherwise result from querying and deleting at the same time

## Working with values

.SetValue() is more flexible because it accepts simple data types too, not just strings

Getting values can be done by explicitly casting to specific types. Supported are
- All standard numeric types, string, bool, DateTime, DateTimeOffset, TimeSpan and Guid, Nullable<>

*Casting to a nullable type* will ensure that even though an element may not exist, a null is returned instead of throwing an exception.

## Documents and Delcarations

### XDocument

XDocument wraps a root XElement and allows you to add an XDeclaration, processing instructions, root level comments and a document type. an XDocument is optional.

Only the root XElement is mandatory on the XDocument

### XML Declarations

```
<?xml version="1.0" encoding="utf-8" standalone="yes" ?>
```

An XML declaration ensures that the file will be correctly parsed and understood. By default, using the XMLWriter will add a declaration to the xml.

The purpose of an XDeclaration is to hint the XML Serialization by indicating which text encoding to use.

Internally strings are stored as UTF-16, thus encoding will set the declaration to UTF-16 if writing to a string.

## Names and Namespaces

XML Namespaces offer (just like C# namespaces) a way to avoid naming collisions.

```
<customer xmlns="OReilly.Nutshell.CSharp" />
```

A first way of defining the namespace is using the xmlns attribute, which is a reserved attribute. It serves 2 functions.
- Specifying a namespace for the element.
- Specifying a default namespace **for all descendants**
  
To prevent using the same namespace in underlying elements set **xmlns=""** on direct children.

### Prefixes

Another way is to use prefixes. There are 2 steps in using a prefix. Defining the prefix and using it. Both can be done together:

```
<nut:customer xmlns:nut="OReilly.Nutshell.CSharp" />
```

xmlns:nut= ... defines the prefix and nut:customer assigns the allocated prefix to the customer element

Prefix does *not* define a default namespace for it's descendants.

Namespaces can also be appointed to attributes. e.g.
```
<customer xmlns:nut="OReilly.Nutshell.CSharp" nut:id="123" />
```

### Specifying namespaces in the X-DOM