# Chapter 8 - LINQ Queries

Language integrated query, introduced in Framework 3.5, enables you to query any collection implementing IEnumerable\<T>. LINQ offers compile-time type checking (static typing) and dynamic query composition.

The basic unit of data in LINQ are **sequences** and **elements**. Sequences is any object that implements IEnumerable\<T> and an element is an item from the sequence.

A *query operator* is a method that transforms the sequence. 
> In System.Linq there are 40 query operators implemented as static extension methods these are called the standard query operators.

## **Fluent Syntax**

Fluent syntax refers to the chaining of query operators to form more complex queries.

> A query operator never alters the input sequence, it always returns a new sequence. This is consistent with the funcitonal programming paradigm.

**A lambda expression that takes a value and returns a bool is called a predicate**

## Query Expressions

C# provides a syntactic shortut for writing LINQ queries called query epxressions. (It was inspired by list compreshensions from functional programming languages such as LISP)

The compiler processes a query expression by translating it into fluent syntax (Chaining extension methods, just like foreach is compiled into .MoveNext() and .Current() calls).

### Range variables

The identifier immediately following the form keyword syntax is called the range variable

```
from   n in names // n is our range variable
select n.ToUpper()
```

Query expressions let you introduce new range variables
- ```let```
- ```into```
- an additional ```from``` clause
- ```join```

### Query Syntax vs SQL syntax

with LINQ data logically flows from left to right through the query. With SQL, order is less well-structured. LINQ comprises a *pipeline* while SQL is more a network of clauses that work mostly with *unordered sets*.

### Query Syntax vs Fluent Syntax

Query syntax is much easier for let clauses that introduce a new variable alongside the range variable. Or another example is a Join followed by an outer range variable reference.

There are many operators that have no keyword in query syntax. e.g. Where, Select, SelectMany, OrderBy, ThenBy

### Mixed-Syntax queries

If a query operator doesn't exist for a query expression, the synax can be mixed.
```
int matches = (from n in names where n.Contains("a") select n).Count();
```

## Deferred Execution

An important feature of most query operators is that they don't execute when constructed but when enumerated.

This is called deferred or lazy execution and is the same as what happens with delegates.

All query operators provide deferred execution except:
- Operators that return a single element or scalar value, such as First() or Count()
- The following conversion operators: ToArray(), ToList(), ToDictionary()

Deferred execution decouples query construction from query execution. This allows to construct a query in multiple steps.

### **How Deferred Execution Works**

Query operators provide deferred execution by returning decorator sequences. Decorator sequences contain the input sequence and a lambda expression which is applied upon the input sequence only when the decorator is enumerated.

### Chaining decorators

Chaining query operators creates layering of decorators.

> Adding ToList() onto the end of a query would cause the preceding operators to execute right away, collapsing the whole object model into a single list

LINQ query is a lazy production line where everything rolls of the belt *on demand*.

## Subqueries

A subquery is a query contained within another query's lambda expression.
```
IEnumerable<string> query = test.OrderBy(m => m.Split().Last());
```
Variables from the outer lambda function are accessible in the inner lambda function.

! There is a a danger with LINQ which is re-enumeration
```
.Where(n => n.Length == names.orderBy(n2 => n2.Length).Select(n2 => n2.Length).First())
```
Remember deferred execution; for every number in the outer query, the inner query will be re-executed. This is handy for translation to a SQL query (Interpreted queries) as everything can be executed in one query. Locally, however, this causes overhead.

The solution for this is to pull out the outer lambda function if possible.

## Composite strategies

3 strategies to build more complex queries:
- Progressive query construction
- Using the into keyword
- Wrapping queries

All are chaining strategies and product **identical** runtime queries.

### The into keyword

In a query expression a select statement has to come after the where and orderby clauses.
The into keyword lets you "continue" a query after projection and is a shortcut for progressively querying.

```
.. select n.Replace("a", "")
   into noVowel
    where noVowel.Length > 2
    orderby ...
```

into can be used after a elect or a groupe clause, it "restarts" a query.

## Projection strategies

### Object intializers

Project objects instead of scalar types, output is IEnumerable\<T> where T:class
It is also possible to use anonymous types. Note that when using anonymous types. The compiler uses a self-generated type. This means that we do not know the return type of our IEnumerable. This is a scenario where using var is the only solution.

it is possible to use 'let' and introduce a new variable next to the initial range variable.

```
from n in names
let vowelless = n.Replace("a", "").Replace("e", "")..
orderby vowelless
select n;

```

## Interpreted queries

LINQ provides 2 architectures: local queries for local object collections and interpreted queries for remote data sources.

Interpreted queries are descriptive. They operate over sequences that implement IQueryable\<T> and they resolve to the query operators in the Queryable class which emits expression trees that are interpreted at runtime.

There are 2 IQueryable<T> implementations in the .NET Framework
- LINQ to SQL
- Entity Framework (EF)

> IQueryable\<T> is an extension of IEnumerable\<T> with additional methods for constructing expression trees.

example:

```
DataContext dataContext = new DataContext("Connection string");
Table<Customer> customers = dataContext.GetTable<Customer>();

IQueryable<string> query = from c in customers
where c.Name.Contains("a")
select c.Name.ToUpper();
```

### How interpreted queries work

First the compiler converts query syntax to fluent syntax. This is exactly as what is done in local queries.

Next the compiler resolves the query operator methods. For local queries the query operators are resolved in the Enumerable Class, for interpreted queries the query operators are resolved in the Queryable. The compiler chooses the Queryable because Table implements IQueryable and IQueryable has a more specific match.

The compiler translates the lambda function to an expression tree. **An expression tree is an object model based on the types in System.Linq.Epxressions** that can be inspected at runtime.

### Execution

Interpreted queries also follow a deferred execution model. This means that the SQL statement is not generated until the query is enumerated.

Interpreted queries work differently in that they don't have the 'belt' system local queries have. The expression tree gets translated once to a complete SQL query.

Some LINQ queries have no SQL translation, this will result in a runtime error

### Combining interpreted and local queries

```
IEnumerable<string> q = customers
    .Select(c => c.Name.ToUpper())
    .Pair()                           // Custom extension method: becomes local
    .Select((n, i) => "Pair " + i.ToString())
```

### .AsEnumerable

It's purpose is to cast an IQueryable\<T> sequence to IEnumerable\<T>, forcing subsequent query operators to bind to Enumerable operators instead of queryable operators.

Casting to AsEnumerable doesn't force immediate query execution.

## LINQ to SQL and Entity Framework

In this chapter, we examine L2S and EF to demonstrate interpreted queries. EF and L2S are currently supported by the ADO.NET team. The team focused its efforts on the EF and thus L2S hasn't been getting anymore big updates.

EF supports other database servers than SQL server via a provider model.

### DataContext and ObjectContext

After defining the entity classes, the querying can begin. For this in L2S you create a DataContext, for EF you use the ObjectContext

DataContext/ObjectContext does 2 things
- Works as factory for genrating objects that you can query
- Keeps track of changes so they can be persisted

In EFCore instead of objectcontext, we use an "DbContext"

DataContext/ObjectContext can be disposed but this is almost never necessary as EF and L2S close connections automatically whenever you finish retrieving results.

#### **Object tracking**

The DataContext/ObjectContext instance keeps track if the entities in instantiates and returns the same entity for the same row that is requested.

This means that querying the same object twice will return the same object even though the second time, the object has potentially changed in the database.

You can use the refresh call to refresh an entity to ensure that no old data is used.