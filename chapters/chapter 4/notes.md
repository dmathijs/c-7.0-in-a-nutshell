# Chapter 4 Advanced C#

## Delegates

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


## **Events**

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

WinRT events have different slightly semantics, they keep track of a tocker required to detach from the event. This gapped is closed by the compiler keeping track of an internal dictionary of tokens.
