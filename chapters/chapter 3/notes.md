# Chapter 3

## classes

Allowed modefiers
| modifier type | examples |
| ------------- | -------- |
| Static modifier | static|
| Access modifier | public private protected internal |
| Inheritance modifier | new |
| Unsafe code modifier | unsafe |
| Read-only modifier | readonly |
| Threading modifier | volatile |


#### Field initialization
Field initializers are run **before** the constructor
They can be intialized on one line if the modfieres are the same. e.g.
```
static readonly int legs = 8, eyes = 2;
```

## Methods

Signature needs to be unique
> Signature of a method is the name of the method and the parameter types, **NOT** the parameter names.

Allowed modefiers
| modifier type | examples |
| ------------- | -------- |
| Static modifier | static|
| Access modifier | public private protected readonly |
| Inheritance modifier | new abstract virtual override sealed |
| Partial code modifier | partial |
| Unnmanaged code modifier | unsafe extern |
| Asynchronous modifier | async |

### Expression-bodied methods (C# 6)

Methods written using the fat arrow representation
```
int Foo (int x) => x * 2;
```

**Overloading of methods** is done by having multiple methods with the same name but with a different signature

Important to note is that Foo(ref int) and Foo(out int) can not coexist, this will give a compiler error.

Local methods can be inserted in other local methods, static can't be used on local methods as they only live in the scope of their parent method.

**Overloading constructors** is a possiblity, calling :this() on an overloaded method can reduce code duplication. e.g.
```
public class Wine
{
    public Wine(decimal price) { Price = price; }
    public Wine(decimal price, int year) : this (price) { Year = year; }
}
```
In this example above, the called constructor will be executed **first**

Compiler automatically generates an parameterless public constructor. As soon as u define a custom one, this parameterless constructor is not longer automatically generated.

### Deconstructors (C# 7)
Assigns fields back to a set of variables ```public void Deconstruct (out float x, out float y)```. Then something like this could be done: ```var (x, y) = rect```. This is alled a deconstructing assignment

When using object initializers, the compiler will make a temporary variable with the objects' value and then assign the values in the initializer braces to that temporary object before setting the final object. This prevents half-initialized objects upon asignment of one of the properties which could throw an exception

> It is important that when writing code that should offer binary compability between assembly versions that the objects are correctly initialized. When using different class or method signatures this could cause a program that depends on the new assembly to fail as the compiler might compile with the old signature, it is thus important to think about this.

### Properties

Properties are read-only if they only specifiy the get accessor and write-only if they only specifie the set accessor.

### Expression-bodied properties (C#6, C#7)

```
public decimal Worth => currentPrice * sharesOwned;
```

c# 7 goes further and allows to use the fat-arrow notation on both the getter and the setter.

Property initializers allow for default values to be set on properties.
Get and set accessors can have different access levels. typically the setter could e.g. be internal or private.


### CLR property implementation

C# propertiy accessors internally compile to methods called get_XXX and set_XXX. If it's a simple non-virtual property the accessors are inlined by the JIT compiler eleminating any performance between accessing a property and a field.

### Indexers

Indexer provide a natural syntax for accessing elements in a class or struct that encapsulate a list or dictionary of values.

E.g. a string is an indexer as you can access the individual characters by doing ```string[0]```. Each type can implement multiple indexers and an indexer can also take more than one argument ```public string this [int arg1, string arg2]```

It is possible to use fat arrow notation for indexers if no setter is required.

CLR compiles indexers to ```get_Item(int x)``` and ```set_Item(int x, string value)```

### Constants

A constant is evaluated at compile time and is literally replaced everywhere it is used by the compiler. 

If a value may change in the future; use _static readonly_ as that will allow referencing assemblies to compile and use the correct versions in future versions.

### Static constructors

Only one static constructor is used. If an exception is throwed during initialization of this static constructor -> type becomes unusable for the whole life of the application. Static field initializers are run right before the static constructor

### Finalizers

finalizers are class-only methods that execute before the garbage collector reclaims the memory for an unreferenced object. The only modifier allowed on the finalizer is the 'unsafe' modifier

```
class Class1
{
    ~Class1() // <-- finalizer
    {

    }
}
```

### Partial types and methods

Partial types can be usefull if the goal is to augment a class that comes from another source. Each part must have the partial declaration.

Partial methods in partial classes allow for customizable hooks for manual authoring. Partial methods must be void and are implicitely private

## Inheritance

### Polymorphism

A subclass can be treated as it's superclass. It works on the basis that the subclasses have all the features of their base class. The inverse is not possible.

A reference can be:
- implicitly upcast to a base class reference
- explicitly downcast to a subclass reference

This is called **reference conversions**. A new reference is created that point to the same object .
If a downcast fails, a InvalidCastException is thrown.

### The as operator

The as operator allows for conversions but doesn't allow custom conversions or numeric conversions
```
long x = 3 as long; // Compile-time error
```

### the is operator

The is operator tests wheter a reference conversion would succeed;
```If (a is Stock s)``` allows for s to be used as a stock and ensure that a is of type Stock

### Virutal function members

Functions marked as virtual can be overridden by subclasses wanting to provide a specialized implementation. A subclass overrides a virtual method by using the override modifier.

It is dangerous to call virutal methods in a constructor as the author of subclasses don't knwo that it will be accessing methods or properties that rely on fields not yet initialized by the constructor

### Abstract classes

Subclasses must implement methods with a abstract modifier (which is an access modifier).

### Hiding inherited members

When overriting members of a baseclass, the compiler will take the field of the subclass. **When adding the new keyword this does nothing more than remove the warning**, the compiled code remains the same. The new modifier communicates your intent to the compiler.

Note that the new operator is different from the new _member_ modifier

### Sealing functions and classes

An overrridden function member may seal its implementation with the sealed keyword to prevent from being further overwritten by further subclasses.
```public sealed override decimal Liablility => return Mortgage;```

> Important to note is that you can't seal a member against being hidden. So if the new keyword is used as an access modifier in one of the subclasses, the behaviour can still be altered for the actual implementation of the subclass

### The base keyword

The base keyword can have to purposes 
- Accessing an overriden function member from the subclass
- Calling a base-class constructor

When calling base on the constructor, the base ctor will always be called first. This ensure that _base_ initialization occurs before _specialized_ intialization.

When a base class has no accessible parameterless constructor, subclasses are forced to call a base constructor with values.

Order of ctor and field initializaiton
1. From subclass to base class:
   1. Fields are intialized
   2. Arguments to base-class constructor calls are evaluated
2. From base class to subclass:
   1. Constructor bodies execute

When calling overloads with sub/base classes, the most specific type will always have precedence.

## The object Type

object _(System.Object)_ is the ultimate base calss for all types any type can be upcast to object.
A stack is LIFO.

Int can also be casted to an object (even though being a value type). This feature of C# is calle dtype unification.

When you cast between a value type and an object, the CLR must perform some special work to bridge the difference in semantics between reference and value type. This process is called boxing and unboxing.

### Boxing and unboxing

Boxing is the act of converting value-type to reference-type instance. The reference type may be either the object class or an interface. (can be done implicitely)

Unboxing reverses the operation and creates a value type from a reference type. Needs to happen explicitly.

```
object obj = 3.5;
int x = (int) (double) obj;
```

_The first conversion to double is called unboxing. The second conversion to int is a numeric conversion_

### Static and runtime type checking

C# programs are type-checked both statically and at runtime.
Runtime type checking is possible because each object on the heap internally stores a little type token. This token can be retrieved by calling the GetType method of the object.

GetType() is evaluated at runtime, typeof() is evaluated at compile time.

### ToString() method

Every object has a ToString() method this method can be overriden by using the inheritance modifier 'override'

### Structs

struct is value type. And does not support inheritance.
Structs can not have
- A parameterless constructor
- Field initializers (class fields)
- A finalizer
- Virtual or protected members

Advantage is that not every struct gets an allocation of the heap (contrary to an object). For instance, creating an array of value type requires only a single heap allocation.

### Struct Construction semantics
- A parameterless constructor that you can't override implicitly exists. This performs a bitwise-zeroing of its fields
- When you define a struct constructor you must explicitly assign every field.

Private is the default accessibility for members of a class or struct. When no accessibility is specified on a class, the default is internal.

### Friend Assemblies

In advanced scenarios, you may want to expose members to a friend assembly, this is done by using the InternalVisibleTo attribute ```[assembly:; InternalsVisibleTo("Friend")``` If the assembly has a strong name, you need to specify it's full 160-byte public key (see Chapter 18)

### Accessibility Capping

A type always caps accessibility of it's members. As a class is by default internal, having a public member would actually 'cap' the member to be internal. It may be usefull to have a member being public as in the future it could be used for when the class is refactored to being public.

A subclass can be less accessible than a base class, but not more.

## Interfaces

A interfaces provides a specification for its members instead of an implementation. Classes can inherit multiple interfaces. An interface can contain only methods, properties, events and indexers, which non-coincidentally are precisely the members of a class that can be abstract.

Interface members are always implicitly public and cannot declare any access modifiers. Casting from implementing type to interface can be done implicitly.

Interfaces can inherit other interfaces

Implementing multiple interfaces can sometimes result in a collision between member signatures. You can resolve such collisions by explicitly implementing an interface member. See example.
