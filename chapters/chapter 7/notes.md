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

FIFO (First-in, first-out). Allows Enqueueing and Dequeueing. Peek returns the first item without removing it.

### Stack\<T> and Stack

LIFO (Last-in, first-out). Allow Push and Pop.

### BitArray

Dynamically sized collection of compacted bool values. More memory-efficient. Uses one bit per boolean value compared to a boolean otherwise using a whole byte.

### HashSet\<T> and SortedSet\<T>

- Contains does a hash-based lookkup
- No duplicate elements
- Not accessible by position (not implementing IList\<T>)

SortedSet keeps the set in order, HashSet does not.
SortedSet also allows for an IComparer interface implementation to be passed to its constructor.

## Dictionaries

IDictionary<TKey,TValue> defines the standard protocol for all key/value-based collections.

Calling Add twice with the same key throws an exception.

Using ContainsKey doesn't make much sense because then you'll have to do a second lookup to fetch the value.

```.ContainsValue(..)``` is a super slow operation as it has to iterate over all items, worst-case O(n).

A dictionary uses a hashtable underneath. It works by converting each element's key into an integer hashcode and than converting that hashcode into a hash key. That key is used to determine the bucket the key is part of. If a bucket contains more than one value, linear search is executed on the bucket. The goal is to have a hash algorith that can **evenly distribute** hashcodes over a 32 bit integer space. This avoids having a few **very large buckets**.

### **Ordered dictionaries**

A dictionary that maintains elements in the same order they were added. An OrderedDictionary is **NOT** a *sorted* dictionary.

It allows ```.RemoveAt()``` as well as indexing (Implementation of IList).

### **List dictionary and Hybrid dictionary**

ListDictionary uses a singly linked list to store the underlying data. It is used because it is extremely efficient with very small lists (fewer than 10 items)

HybridDictionary is a ListDictionary that automatically converts to a HashTable upon reaching a certain size. To address ListDctionarys' problem with performance. However the conversion is slow and thus using a Dictionary to begin with shouldn't really be a problem.

Both classes only exist in non-generic form.

### **Sorted dictionaries**

The framework uses 2 dictionary classes internally structured so that their content is always sorted

1. SortedDictionary<TKey, TValue>
2. SortedList<TKey, TValue>

**SortedDictionary** uses a red/black tree: a data structure designed to perform well in any insertion or retrieval scenario

**SortedList** is implemented internally with an ordered array pair, providing fast retrieval but poor insertion (because existing values have to be shifted)

The only advantage is that SortedList items can both be accessed by key and by index.
SortedDictionary has to do a lineair traversal to access by index.

## Customizable collections and Proxies

Sometimes you need control for some scenario's e.g.

- To fire an event when item is added or removed
- Update properties because of the added or removed item
- To detect an "illegal" add/remove

### Collection\<T> and CollectionBase

The Collection\<T> class is a customizable wrapper for List\<T>.
It contains 4 virtual methods that allow to hook in the list's normal behavior.

Collection<T> also has a constructur accepting an existing IList<T> unlike with other collection classes, the supplied list is proxied raterher than copied, meaning that subsequant changes will be reflected in the wrapping Collection<T>

### CollectionBase

CollectionBase is the nongeneric version of Collection\<T>.

### KeyedCollection<TKey, TItem> and dictionaryBase

KeyedCollection<TKey, TItem> is best thought of as Collection\<TItem> plus fast lookup by key. It doesn't implement IDictionary and thus has no concept of key/value pairs.

The NonGenericversion of KeyedCollection is dictionaryBase. It actually implements IDictionary.

### ReadOnlyCollection\<T>

As the name explains this is a collection type that can only be read out.

## Plugging in Equality and Order

A type that implements protocols for being equatable, hashable and comparable will function correctly in a dictionary or sorted list "out of the box."

Sometimes for sorting lists, the comparison may be done differently, for this the .NET framework defines a matching set of "plug-in" protocols.

The plug-in protocols consist of the following interfaces:

- IEqualityComparer and IQualityComparer\<T>
  - Performs plug-in equality comparison and hashing
  - Recognized by HashTable and Dictionary

- IComparer and IComparer\<T>
  - Performs plug-in order comparison
  - Recognized by sorted dictionaries and collections

Both comparers offer a default implementation which executes the 'default' comparison
```
EqualityComparer<int>().Default;
```

### IEqualityComparer

In order to implement IEqualityComparer, one could simply implement EqualityComparer which inherits IEqualityComparer\<T> and EqualityComparer

The comparer can then be passed to the collection to be used.


### IStructuralEquatable and IStructuralComparable.

Remember from previous chapter that structural comparison is used by defaults for structs; each of the values is compared. Sometimes this can also be usefull for arrays.

IStructuralEquatable takes a comparer used to compare all the members
```
...
bool Equals(object other, IEqualityComparer comparer); // e.g. EqualityComparer<int>.Default
...
```