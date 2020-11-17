# Chapter 7 - Collections

Only arrays form part of the C# language; the remaining collections are just classes you instantiate like any other.

The types for collection can be divided in the following categories:
- Interfaces that define standard collection protocols
- Ready-to-use collection classes (lists, dictionaries, ..)
- Base classes for writing application-specific collections

The framework supports the traversel of a collection via a pair of interfaces that allow different data structures to expose a common traversal API.

## Enumeration

### IEnumerable and IEnumerator

Collections do not usually *implement* enumerators; instead, they *provide* enumerators via the interface IEnumerable. In other words the IEnumerable interface can be thought of as a IEnumeratorProvider.

Arrays return a nongeneric IEnumerator for backwards-compability purposes.

IEnumerable\<T> inherits IDisposable

To implement IEnumerable/IEnumerable\<T>, you must provide an enumerator. You can do this in one of three ways:
- If the class is "wrapping" another collection, by returning the wrapped collection's enumerator
- Via an iterator using yield return
- By instantiating your own IEnumerator/IEnumerator\<T> which takes your enumerable type as constructor variable.

Enumerators only iterate a collection once.