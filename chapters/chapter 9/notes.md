# Chapter 9 - LINQ Operators

All examples use the following array

```
    string[] names = { "Tom", "Dick", "Harry", "Mary", "Jay" };
```

The standard query operators fall into three categories
- Sequence in, sequence out (sequence-to-sequence)
- Sequence in, single element or scalar value out
- Nothing in, sequence out (generation methods)


### Quantifiers
An aggregation returning true or false
- All, Any, Contains, SequenceEqual

### Generation methods
Manufactures a simple sequence
- Empty, Range, Repeat


## Filtering

### Where()

Where allows a predicate that optionally takes a second element which is it's position in the array ```Where((n, i) => i % 2 == 0);```

### TakeWhile and SkipWhile()

TakeWhile and SkipWhile have no translation in LINQ2SQL and will throw an exception when used

### Subqueries in select

When using SubQueries in select statements, double-deferred execution happens. This means that the second local query isn't executed until it is requested.

### SelectMany

With SelectMany, each input element is the trigger for the introduction of fresh material. The fresh material is emitted by the selector lambda expression and must be a sequence. In other words the lambda expression must emit a child sequence per input *element*.

### Multiple range variables

```
IEnumerable<string> query = 
    from fullName in fullNames
    from name in fullName.Split()
    select name + " came from " + fullName;
```

**non-equi join** join for which no equality comparer is used

```
var query = from c in dataContext.Customers
    from p in dataContext.Purchases
    where c.ID == p.CustomerID
    select c.Name + " bought a " + p.Description;
```
This is an equi join because it uses the equality comparer. However, above is a bad example as you would use the association property instead of the equality comparer.

> With local queries it is important to filter BEFORE joining (Otherwise the join will potentially increase the number of rows)

### Outer joins with selectMany

When using an outer join with multiple range variables. Use .DefaultIfEmpty() if you want the other range variable to appear although one part of the join is empty.

```
from c in dataContext.Customers
from p in c.Purchases.DefaultIfEmpty()
```
-> will return a result for all customers, even though the user has no purchases.

## Joining

### Join and GroupJoin

Join and groupjoin have as advantage that they execute efficiently locally since they first load the inner sequence as a keyed lookup. The disadvantage is that they offer only inner and left outer joins only. Cross joins and non-equi joins must still be done with select/selectmany.

### Join

Join operator performs an inner join, emitting a flat output sequence.

```
IQueryable<string> query =
    from c in dataContext.Customers
    join p in dataContext.Purchases on c.ID equals p.CustomerID
    select c.Name + " bought a " + p.Description;
```

For local queries, using join is much faster because of the fact that a keyed lookup is created from the enumerable

```
var slowQuery = from c in customers
    from p in purchases where c.ID == p.CustomerID
    select c.Name + " bought a " + p.Description;
var fastQuery = from c in customers
    join p in purchases on c.ID equals p.CustomerID
    select c.Name + " bought a " + p.Description; // Project into string
```

- The outer sequence is the input sequence (customers)
- The inner sequence is the newly introduced sequence (purchases)

### GroupJoin

GroupJoin does the same as Join but returns a hierarchical result instead of a flat result.
The syntax for GroupJoin is the same as for Join but is followed by the into keyword

> A into clause translates only to a groupjoin if it appears directly after the join clause.

### Joining with lookups

The inner sequence gets converted to a lookup with interface ILookup when using join. Important to note is that join also is deferred execution. This means that to **lookup table gets build at enumeration time**.

A lookup can be created before to be used in multiple queries. ```ILookup<int?, Purchase> purchLookup = purchases.ToLookup(p => p.CustomerID, p => p)```

### The Zip Operator

the zip operator just runs over 2 collections and groups items at index x.

## Ordering

.ThenBy only sorts item that had the same sorting key in .OrderBy. .ThenBy Operators can be chained.

### Comparers and collations

The ordening is determined by the the implementation of the IComparable Interface, this behavior can be overwritten by using an IComparer. Important to note is that the query syntax, LINQ2SQL and EF have no support for comparers.

If the last query operator used is .OrderBy or .ThenBy, the returned type will be IOrderedEnumerable\<T>. Use .AsEnumerable() to ensure that the sequence can be used as an enumerable

## Grouping

grouping allows to group entities using the IGrouping Interface which contains a key and a collection of entities

## Set (Collection) Operators

Union, intersect, except, ..

## Conversion Methods

### OfType and Cast

Both methods are going to try converting the types. The difference is that OfType will skip wrong elements while Cast is going to throw an exception whenever a conversion action is impossible.

Important to not is that because of the implementation numeric or custom conversions can not be used when doing the casting.

e.g.
```
int[] integers = { 1, 2, 3 };

IEnumerable<long> test1 = integers.OfType<long>();

```

This wont return any elmements as there's no inheritance relationship between int and long and the ```element is long``` check will fail.

## Element operators

Methods engin in "OrDefault" return default(TSource) instead of throwing an exception when no elements match the predicate, think First() and FirstOrDefault()

## Aggregation methods

### Aggregate

Aggregate allow you to specify a custom accumulation algorithm for implementing unusual aggregations.

```
int[] numbers = { 2, 3, 4 };
int sum = numbers.Aggregate(0, (total, n) => total + n);
```

The first argument is the Aggregates's *seed*, from which accumulation starts

### Unseeded aggregations

The seed value can be ommited, in which case the first element becomes the implicit seed.
In the example above,
when using seeded aggregations what will happen is 0 + 2 + 3 + 4, with unseeded aggregations the result will be 2 + 3 + 4 

It is important with **unseeded aggregations** that the accumulation lambda is both associative as commutative.

## Quantifiers

SequenceEqual compares 2 enumerables and ensures that they have all same elements in the same order.

## Generation methods