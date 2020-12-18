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
('this shows wrapping');
```

### Predefined types

predefined types are types that are specially supported by the compiler e.g. int
-> stored in 32 bit. int is an alias for System.Int32 (which is a struct)
Other examples are the string type and the bool type, and all other numeric types except decimal.

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
A reference type can be assigned the literal null. A value cannot have a null value.

> Interesting is that an int, although being only 32 bits (4 bytes), the actual space the CLR will appoint is up to 8 bytes (this behaviour can be overwritten using the StructLayout (see page 975))

A reference types' object consumes as many bytes as it fields + some administrative overhead. Minimum of the overhead is 8 bytes which contains a key to the objects' type as well as some temporary information

Value types are known as primitive types in the CLR (except for decimal). They are directly supported via instructions in compiled code. 

Numeric value types can be noted in decimal or hexadecimal format e.g. long y = 0x7F;
From C# 7 onwared numeric literals can also be made more readable 
```
int million = 1_000_000;
```
Numeric suffixes can be used to make clear what type needs to be used. e.g. var x = 1L (will be a long). The only real necessary suffixes are F (float) and M (decimal). Because doing 
```float m = 4.5;``` _is impossible_ as no implicit conversion of double to float exists.

## Arithmetic Operators

Integral types
-> int, uint, long, ulong, short, ushort, byte, sbyte (signed byte [instead of 0-256 -> -128-128])

Overflow on integral types is silent, they will just exhibit "wraparound behaviour"
- Solution is to use the ```checked(x*y)``` operator

> Checked can be made the default for all expressions in a program by compiling with the /checked+ flag

If no Exceptions are desired, the user can also use the unchecked operator ```unchecked(int.MaxValue + 1)``` -> will not throw any exceptions.

constants will always be checked on overflow at compile time unless unchecked flag is used

double and float have reserved NAN, +infinity, -infinity and -0 values. NAN can be checked by using ```double.IsNan(0.0/0.0);```

!! float & double are base 2 -> lower precision. decimal are base 10 which allows that to precisely represent numbers expressible in base 10 (as well as its factors, base 2 and base 5)

## Boolean Type and Operators

Boolean uses one bit but is stored in a byte, which is the minimum chunck the runtime and processor can efficiently work with. BitArray uses bits in a byte and prevents allocation of multiple bytes.

By default 'or' = '||' and 'and' = '&&', it is also possible to use '|' and '&' respectively. The difference is that they will not 'shortcut' and thus execute the full boolean expression. The '|' and '&' operator will perform bitwise operations if used with numbers!

**Conditional operator (ternary operator)**
```
static int Max (int a, int b)
{
    return (a > b)? a:b;
}
```

## Strings and Characters

Some characters need escape sequences as they are not interpreted literally. e.g. '\n'

**verbatim string literals** -> string is prefixed with '@' and prevents use of escape sequences.

Concatenating strings with the '+' operator is inneficient, better to use the _System.Text.StringBuilder_

### String interpolation

Since C# 6.0 it is poissible to use string interpollation writing ```..WriteLine($"this is my {x} test")``` where x is an expression

Verbattim literal can also be used together with interpolation. In that case the @ has to be after the $.

## Arrays

Elements stored in contiguous block of memory, providing highly efficient access.
Creating an array always preinitializes the elements with default values this is equal to a bitwise zeroing of memory. E.g. for int this will return 0 for an object this will return _null_;

Arrays can hold value types and reference types -> reference type will require initialization otherwise a NullRefException will be throwed.

### multi-dimensional arrays

- rectangular -> n-dimensional block of momery ```int[,] matrix = new int[3,3];```
- jagged -> array or arrays ```int[][] matrix = new int[3][]``` notice that the inner dimension is not specified; this is because, unlike for a rectangular array, all inner array can be an arbitrary length

var keyword implicitly types a variable -> var i = 3; -> compiler will set this to int, same is possible for arrays

bounds checking is necessary to prevent IndexOutOfRangeExceptions, the performance hit is minor and JIT compiler can perform optimizations such as determining if the loop will exceed a bound.

## Variables and Parameters

### Stack

block of memory for storing local variables and parameters, the stack logically grows.

### Heap

block of memory in which objects (i.e. reference-type instances reside). References are cleaned up by the garbage collector. It's impossible to explicitely delete objects in c#

**The heap also contains all static fields. These live until the appplication domain is torn down.**

default values is equal to bitwise zeroing of memory. e.g. for reference types this will mean that all the values are null. Default value can be obtained by writing ```default(string)```

In C# arguments are passed by values; if a value type is given to a method, the value is copied. If a reference type is passed to a method, the reference get copied **not the object**.

When using the **out**, it is possible that when using multiple ones, some are not interesting. Since C# 7, these can be discarded by using the _ symbol, called the discard

```
Split("This is a test", out string a, out string b, out string c, out _)
```

if a real underscore variable is in scope, then for backwards compability the code will not compile.

### params modifier

The params parameter modifier may be specified on the last parameter of a method so that the method accepts any number of arguments of a particular type.

```
class Test{
    static int Sum(params int[] ints){
        // Perform sum
    }
}
```

Optional parameters can be passed using a default value e.g. void ```Foo(int x = 23)```, the x parameter is optional in this method call. Optional parameters must always be placed _after_ mandatory parameters

Arguments can be identified by it's name e.g.
```
void Foo(int x, int y) { ... }

// can be called, these are the same

Foo(x:1, y:2);
Foo(y:2, x:1);

```

Positional arguments must always come before named arguments

### ref locals

a local variable can defined a reference to an element in an array or field in an object:
```ref int numRef = ref numbers[2]```. Changing the value of numRef will change the value in the numbers array.

implicit typed variables (var keyword) are statically typed, change there 'type' will arise a Compile-time error.

## Expressions and Operators

Expressions essentially denote a value. e.g. **12** is a constant expression. Complexer expressions can be built using multiple operands **1 + (12 * 30)**. Always use infix notation (operand - operator - operand). 

### Primary expressions
Primary expressions include expressions composed of operators that are intrinsic to the basic plumbing of the language. e.g. ```Math.Log(1)```. 
1. First expression is a member-lookup using the '.' operator
2. Second expression performs a method call using the () operator

### Void expressions
Expressions which have no value

### Assignment expressions
Result of expression is assigned to a variable

### Association & Precedence

Association and precedence determine in what order expressions are executed. Binary operators are left-associative.

assignment operators, lambda, null coalescing and conditional operator are right-associative

```
x = y = 3 // will first put y equal to 3
```

## Null Operators

?. is called the null-conditional or the "Elvis" operator (C# 6), it allows for quick null checking to prevent NullReferenceExceptions.

## Statements

### Selection statements

C# has the following mechanisms to conditionally control the flow fo program execution
- Selection statements (if, switch)
- Conditional operator (?.)
- Loop statements (while, do..while, for)

the switch statement can beused with patterns
```
switch(x){
    case int i:
        ...
        break;
    case string s:
        ...
        break;
    case bool b when b == true:
        ...
        break;
    default:
        ...
}
```

case clauses can also be stacked.

### for loops

```
for(initialization-clause; condition-clause; iteration-clause)
    statement or statement block
```

### jump statements
jump statements are break, continue, goto, return and throw

**goto** statement can be used to jump to a specific place, check examples;


## Namespaces
Namespace is a domain for type names. A namespace forms an integra part of a type's name.

### the using directive
```using Outer.Middle.Inner;``` -> imports types from the Outer.Middle.Inner namespace

Since c#6, it is possible to import not just a namespace but a specific type and all it's static members can be used through the code. This includes it's fields, properties and nested types.

```
using static System.Console;

...
WriteLine("This works");
...

```

Classes in the Inner namespace can always access classes in the outer namespace. If you want to refer to a type in a different branch of the namespace hierarchy, you can use a **partially qualified name**. e.g. Middle.Inner.xxx;

When refering to type with the same name in inner and outer namespaces. The inner one will always win. Using the outer one will have to be done using it's qualified name.

Namespaces can be repeated as long as there are no conflict in class names.

Types and namespaces can be aliased
```using PropertyInfo2 = System.Reflextion.PropertyInfo```, this allows for types that have the same name to be imported under a different name

### Extern namespace alias

Allow (using compiler flags) to appoint external namespaces to another namespace

```
// csc /r:W1=Widgets1.dll /r:W2=Widgets2.dll application.cs
extern alias W1;
extern alias W2;

Class Test
{
    static void Main()
    {
        W1.Widgets.Widget w1 = new W1.Widgets.Widget();
        W2.Widgets.Widget w2 = new W2.Widgets.Widget();
    }
```
### ::token

As stated before, inner namespaces will always get precedence. However, if we want to change that we can do so by doing ```Console.WriteLine(new global::Outer.Type())```