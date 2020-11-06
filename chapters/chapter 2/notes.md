# Chapter 2

### identifiers

- Camel case -> parameters, local variables, private fields
- Pascal case -> everything else

C# is compiled into IL using the C# compiler (csc.exe)

### keywords are reserved
- abstract, do, in, protected, ...

### contextual keywords
- can be used as identifiers depending on context -> in, into, join, let

statements can wrap multiple lines, they are ended by the ';' punctuator

```
console.writeline
('this shows warpping');
```

### Predefined types

predefined types are types that are specially supported by the compiler e.g. int
-> stored in 32 bit.
Other examples are the string type and the bool type.

### Custom types

custom types contain data members and function members (includes ctor).
ctor is used to instantiate type and pass data. public keyword is used to mark functionality/data as publicly available. in OOP terms; public members encapsulate the private members;

## Conversions

### explicit/implicit conversions 
explicit conversoins require a cast, implicit does not
```
int x = 123;        // initialize 32-bit integer
long y = x;         // implicit conversion
short z = (short)x; // explicit conversion
```

Implicit conversion allowed if
- Compiler can guarantee it always succeeds
- No information is lost in conversion (not int to short explicit conversion above)

Explicit conversion allowed if:
- Compiler can guarantee it always succeeds
- Information can be lost during conversion


**All C# types fall into the following catagories:**
- Value type
- Reference type
- Generic type parameter (Chapter 3)
- Pointer types (Chapter 4)

A value type contains it's value. A reference type has 2 parts: an object and a reference to that object. Multiple references can point to the same object.
A reference type can be assignet the literal null. A value cannot have a null value.

> Interesting is that an int, although being only 32 bits (4 bytes), the actual space the CLR will appoint is up to 8 bytes (this behaviour can be overwritten using the StructLayout (see page 975))

A reference types' object consumes as many bytes as it fields + some administrative overhead. Minimum of the overhead is 8 bytes which contains a key to the objects' type as well as some temporary information

Value types are known as primitive types in the CLR (except for decimal). They are directly supported via instructions in compiled code. 

Numeric value types can be noted in decimal or hexadecimal format e.g. long y = 0x7F;
From C# 7 onwared numeric literals can also be made more readable 
```
int million = 1_000_000;
```
Numeric suffixes can be used to make clear what type needs to be used. e.g. var x = 1L (will be a long). The only real necessary suffixes are F (float) and M (decimal). Because doing 
```float m = 4.5;``` _is impossible_ as no implicit conversion of bool to float exists.
