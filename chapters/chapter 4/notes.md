# Chapter 4 Advanced C#

## 4.1 Delegates

A delegate is an object that knows how to call a method. A delegate type defines the kind of method that delegate instances can call.

The caller calls the delegate which in its turn calls the method. This decouples the caller from the target method.

When we specify a method without brackets or arguments e.g. **Square**, this is called a _method group_ 

calling a delegate: 
```
delegate int Transform(int x);

Transform t = ...
// will be invoked by using
t(3)
// or
t.Invoke(3)

```

### Multicast Delegates

All delegates have multicast capability, this means that they cannot only call one method but can actually call multiple. (e.g. using the += operator on the delegate instance to add a method group or -= to remove a method group)

When the return type is not Void, only the result of the last function called will be returned.

### Instance Versus Static Method Targets

When an instance method is assigned to a delegate object, it doesn't only keep reference to that method but also to the type of which the method is a member. ```t.Target``` will return the type, while ```t.Method``` will return the method.

### Generic delegate types

A delegate type can also contain generic parameters
```
public delegate T Transformer<T>(t arg);
```

### Func and Action

Funcs and action are delegates, the ```Func<in T, in T2, out Result>``` and ```Action<in T>``` are general delegates that can be used in a general context.

### Delegates versus Interfaces

A delegate can be a better design choice if one or more of these is true
- The interface defines only a single method
- Multicast capability is needed
- The subscriber needs to implement the interface multiple times

### Delegate Compability

Delegates are all incompatible even if they have the same signature. They are equal to each other if their methods are the same, _in the same order_.

### Parameter compability

Like default polymorphic behavior, a delegate can accept more specific parameter types than its method target, this is called contravariance (see chapter 3).

The default event pattern is designed to help you use this advantage by having the default 'EventArgs' and calling the delegates by passing e.e. MouseEventArgs or KeyEventArgs.

### Return type compability

When you call a method, there is a possibility that you receive a more specific type. This is called covariance

It is good practice to
- Mark a type parameter used only as return value as covariant (out)
- Mark any type parameters used only on parameters as contravariant (in)


## **4.2 Events**

When using delegates, 2 apparent roles; *broadcaster* and *subscriber* 

- Broadcaster contains a delegate field and invokes it when necesary
- Subscriber decides when to start/stop listening by calling += and -= (see multicast delegates).

```
// Delegate definition
public delegate void PriceChangedHandler (decimal oldPrice, decimal newPrice);

public class Broadcaster
{
    // Event declaration
    public event PriceChangedHandler PriceChanged;
}
```

Removing the event keyword from the broadcaster would make it less robust, the subscribers would be able to:
- Replace other subscribers by reassigning PriceChanged
- Clear all subscribers
- Broadcast to other subscribers by invoking the delegate

WinRT events have different slightly semantics, they keep track of a token required to detach from the event. This gap is closed by the compiler keeping track of an internal dictionary of tokens.

### Event accessors

Event accessors are like property accessors but instead of get and set you use add and remove. Important is that when using add and remove is that the event needs a private delegate field in which to store the values (Exactly like when using a back-ing field for the property)

This can be usefull in the following scenario's
- When the event accessors are merely relays for another class that is broadcasting the event
- When the class exposes a large number of events, it's better to use a dictionary to store the delegates instances as a dictionary will contain less memory overhead
- When explicitly implementing an interface that declares an event.

> Nice to know is that like with properties that get compile to their respective *get_XXX* and *set_XXX*, event accessors get compiled to *add_XXX* and *remove_XXX*

### Event modifiers

just like methods events can be made virtual, overriden, abstract, sealed or static.

## 4.3 Lambda Expressions

Lambda expression is an unnamed method written in place of a delegate instance

The compiler converst the lambda expression to either
- A delegate instance
- An expression tree of type ```Expression<TDelegate>``` that can be interpreted at runtime

The compiler compiles lambda expressions to a private method.

Lambda expressions are most commonly used with Func and Action delegates.
```
Func<int, int> sqr = x => x * x;
```

### Capturing outer variables

It is possible for a lambda to use variables from outside it's scope, these variables are called captured variables. A lambda expression that captures variables is called *a closure.*

> Variables can also be captured by anonymous methods and local methods.

Variables are captured when the delegate is invoked, not when it is created.

Capturing variables is internally implemented by "hoisting" that is implementing the values in a private class which is instantiated and lifetime-bound to the delegate instance.

### Capturing iteration variables

If variables are captured inside the iteration of a for loop and later used, they will all return the last loops' value. (see example), To prevent this, you can define a local function value that is scoped **inside** the for loop, this will keep it's specific value.

This isn't the case with foreach loops, here the variables used are immutable and thus their values are kept.

### Lambda expressions versus local methods

Advantages of using local methods instead of lambda expressions
- They can be recursive without ugly hacks
- They avoid the clutter of specifying a delegate type
- They incur slightly less overhead (The CPU needs some additional cycles to allocate a delegate for the lambda expression)


### Anonymous methods

Anonymous methods (C#2) are the precurser of lambda expressions (C#3),  they lack the following features:

- Implicitly typed parameters
- Expression syntax (an anonymous method must always be a statement block)
- The ability to compile to an expression tree by assingig to ```Expression<T>```

```
delegate int Transformer (int i);

Transformer sqr = delegate (int x) { return x * x;} // Notice the statement block and the explicit types

```

## Try statements and exceptions

finally is usefull for task cleanup. Checking for preventable errors is preferabel to relying on try/catch because exceptions are relatively expensive to handle, taking hundreds of clock cycles or more.

If exception is thrown, the CLR performs a test: Is execution currently within a try statement that can catch the exception?

- If so, execution is passed to the compatible catch block
- If not, execution jumps back to the caller of the function and the test is repeated (is the caller part of a try statement)

Catching ```System.Exception``` catches all possible errors. 

Since C#6, there is also the possibility to add **exception filter** in a catch clause
```
catch(WebException ex) when (ex.Status == WebExceptionStatus.Timeout)
{
    ...
}
```

### The finally block
The finally block always executes--wheter or not an exception is thrown and whether or not the try block runs to completion.

Exceptions are thrown either by the CLR or by user code (using the throw statement)

Rethrowing a less specific exception (using throw; vs using throw ex;) is something you might do when crossing a
trust boundary, so as not to leak technical information to potential hackers.

### The TryXXX Method Pattern

Return a bool value and the specific return value with the out keyword.

## Enumeration and iterators

An enumerator is a read-only, forward-only cursor over a sequence of values. It implements either of the following interfaces

- ```System.Collections.IEnumerator```
- ```System.Collections.Generic.IEnumerator<T>```

An enumerablel is a logical representation of a sequence that produces cursors over itself.
- ```Implements IEnumerable or IEnumerable<T>```
- Has a method named ```GetEnumerator``` that returns an enumerator.

### Iterators

Iterators are producers of an enumerator. Foreach statement is a consumer of the enumerator

The compiler converts iterator methods into private class that implement ```IEnumerable<T>``` and/or ```IEnumerator<T>``` None of the code actually runs when you call an iterator method, only the class gets initialized. When the enumeration starts, only then do the items get created.

**yield return statement cannot appear in a try block that has a catch clause. Nor can the yield return appear in a catch or finally block.** These restrictions are due to the fact the compiler must translate iterators into ordinary clasess with MoveNext, Current and Dispose members. adding exception handling would create **exessive complexity**

### Composing sequences

*The important lesson is that the next iteration is not called until MoveNext() is called.*

## Nullable types

Using the Nullable<T> struct allows for value types to be nullable.

The conversion from T to T? is implicit but from T? to T the conversion has to be explicit.

When T? is boxed (value to reference conversion) the heap contains T, not T?.

### **Operator Lifting**

The Nullable<T> struct does not define operators such as <, > or even == Despite that two nullables can still be compared e.g. ```int? x = 5, y = 2; bool b = x < y // True```

This works because the compiler borrows or lifts, the less-than operator from the underlying value type.

the preceding comparison is semantically translated into this:
```bool b = (x.HasValue && y.HasValue) ? (x.Value < y.Value) : false; ```

> Each of the predefined types (int, double, ..) is shorthand for a system-provided type (Int32, Double64). For example the keyword int refers to the struct System.Int32. As a matter of style, use of the keyword is favoured over use of the complete system name.

It is possible to mix nullable and non-nullable types because there's an implicit conversion from non-nullable to nullable.

### Nullable types and null operators

Nullable types can be usefull when working with e.g. database programming, there one would probably have nullable collumns to indicate that the column doesn't contain any value.

**An ambient property** is a property that returns it's parent Value if itself is null.

It is possible to generate an array whose lower bound is 1 instead of 0:
```
Array a = Array.CreateInstance(typeof(string), new int[] {2}, new int[] {1});
```

Before nullable types (Structs) were part of C#, there was a strategy to designate a particular non-null value as the "null value"; an example is in an array that whe doing x.IndexOf() it will return -1 if it's not in the collection.

Nominating a "magic value" like this is problematic for the following reasons:
- Each value type has it's own different representation of the value "Null"
- There may be no reasonable designated values
- Forgetting to check the magic number would result in an incorrect valuel that may go unnoticed. Using Nullable, calling the value without checking hasValue would throw ``` InvalidOperationException```.

## Extension Methods

Extension methods allow an existing type to be extended with new methods without altering the definition of the original type.

The compiler will compile extension methods as static methods on the type itself. **Interfaces can also be extended**

Extension methods allow for chaining.

Important ambiguity
- An extension method cannot be accessed untill its class is in scope
- Instance method will always take precedence over extension method
- The more specific implementation of an extension method will take precedence
  
## Anonymous types

Anonymous type is a simple class created by the compiler to store a set of values on the fly.
```
var dude = new { Name = "Bob", Age = 23}; // will get compiled to class with 2 properties

var Y = 4;
// Underlying anonymous types will have the same type
var a1 = new { X = 2, Y }
var a2 = new { X = 3, Y = 2}

```

If a method should return an anonymous object you should use **dynamic**, this is because functions can't return var. Anonymous types are used primarily when writing LINQ queries.

## Tuples

tuples are value types, with mutable elements. 
Optionally a name can be given to tuples.

```
var tuple = (Name:"Bob", Age:23)

WriteLine(tuple.Name); // Bob
```
Note that these elements can still be treated as unnamed and refered to as Item1 and Item2. Tuples are type-compatible (assignable) if their element types match up.

(string, int) is an alias for the ```ValueTuple<string, int> tuple```. At runtime references to the "names" of tuple names will be removed. When debugging an assembly you'll see that the tuple names are not there.

### Deconstructing tuples.

Tuples can be deconstructed into their separate variables
```
(string name, int age) = bob;
```

### System.Tuple vs ValueTuple

The system.tuple is a class that represents a tuple, after its introduction it was considered a mistake. The structs had a slight performance advantage compared to the class with almost no downside. That's why you can still come across System.Tuple in some code.

## Attributes

Attributes are an extensible mechanism for adding custom information to code elements.

A good example of attributes usage is serialization. In this scenario, an attribute on a field can specify the translation between C#'s representation of the field an the format's representation of the field.

Attributes inherit System.Attribute. **By convention, all attribute types end in the word Attribute, C# recognizes this and allow you to omit the suffix when attaching an attribute**

### Named and positional attribute parameters

Positional parameters correspond to parameters of the attribut type's public constructors. Named parameters correspond to public fields or public properties on the attribute type.

Multiple attributes can be specified by using commas between the attributes and/or using multiple pairs of square brackets

## Caller Info Attributes

from C# 5 you can tag optional parameters with one of the three caller info attributes
which instructs the compiler to feed information obtained from the caller's source code into the parameter's default value

```
public void Foo (
    [CallerMemberName] string memberName = null,
    [CallerFilePath] string filePath = null,
    [CallerLineNumber] int lineNumber = 0
)
```

## Dynamic Binding

Dynamic binding deferes binding (the process of resolving types, members and operations) from compile time to runtime. Dynamic binding is useful when at compile time you know that a certain function, member, or operation exists, but the compiler does not. Usefull when operating with dynamic languages and COM.

A dynamic type is declared with the contextual keyword dynamic ```dynamic d = GetSomeObject();```

### Static binding

When calling a method on an object. The compiler is going to search on the types for that method. Failing that, it will continue it's search on extension methods or base classes. If not found it will throw a compilation error. The binding depends on statically knowing the types of the operands. This makes it static binding.

