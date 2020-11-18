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

## The ICollection and IList interfaces

Enumerables don't provide mechanisms to determine the size of the collection, access members by index, search or modify the collection.

- ICollection\<T> 
  - Provides medium functionality (e.g. count)
- IList\<T>
  - Provide maximum functionality

You will probably never implement any of these interfaces, subclassing Collection\<T> should reach most goals.

Generics don't inherit non-generic; ICOllection\<T> doesn't inherit from ICollection. This was done because hindsight allowed for better implementation. Other than that you would be able to call Add(object) on an collection which would defeat type safety.

### ICollection and ICollection\<T>

ICollection defines properties that assist with synchronization, something that has been dropped in the generic version as thread safety is no longer considered intrinsic to the collection.

### IList and IList\<T>

Standard interface for access on indexable position.

C# arrays implement both the generic and the nongeneric ILists although the Add and remove elements are hidden by using explicit interface implementation. (private)

### IReadOnlyList\<T>

It would make sense that IList\<T> inherits IReadOnlyList\<T>. However, this would have introduced a breaking change in CLR 4.5 as members of IList would have had to be moved to IReadOnlyList.

## The array class

The CLR treats array types specially upon construction, assigning them a contiguous space in memory. This makes indexing into arrays highly efficient, but prevents them from being resized later on.

Because Array is a class, arrays are always reference types. This means that by default referential equality happens. However, using *StructuralComparisons* allows for checking content.

```
IStructuralEquatable se1 = a1;
se1.Equals(a2, StructuralComparisons.StructuralEqualityComparer)); // True
```
Using .Clone() on an array is a shallow clone, it will copy the memory. This means that new value types are created and that references to the same objects are part of the cloned array.

**The CLR does not permit any object including arrays to exceed 2GB** this means that a long indexer doesn't have much sense.

fastest way to create an index is by using C#'s languages construct
```
int[] array = { 1,2,3 };
```

### Searching

The array class offers a range of methods for finding items.

- BinarySearch methods
  - Rapidly searching in a sorted array
- IndexOf / LastIndex methods
  - For searching unsorted arrays
- Find/FindLast/FindIndex
  - For searching unsorted arrays for item(s) that satisfy a given Predicate\<T>

Sort method has overload that allows 2 arrays to be ordered depending on keys of first array.

Calling sort can also be done with a custom comparison provider 
- IComparer/IComparer\<T> helper object
- Via Comparison delegate
  
Array.Sort will alter the existing array, LINQ operators don't alter the original array but will emit a sorted fresh IEnumerable\<T>

### Converting and Resizing

Array.ConvertAll creates and returns a new array of element type TOutput. The supplied delegate copies the elements.

Resize takes a copy of the specific size, all values will be kept.

## List, Queues, Stacks and Sets

fList contains an internal array of objects replaced with a larger array upon reaching capacity.

The nongeneric ArrayList is primarily kept for backwards compability.

### LinkedList\<T>

Chain of nodes in which each node references the previous and the next element.
LinkedList implements both ICollection and IEnumerable but doesn't implement IList as access by index is not supported.

### Queue\<T> and Queue

