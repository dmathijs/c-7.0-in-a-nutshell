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

Transformer sqr = delegate(int x) { return x * x;} // Notice the statement block and the explicit types

```