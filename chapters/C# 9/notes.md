# C# 9 release notes

## Record types

Reference type that provides **synthesized methods** to provide value semantics for equality and is **immutable**. It is very usefull in concurrent programs with shared data.

```
public record Person
{
    public string LastName { get; }
    public string FirstName { get; }

    public Person(string first, string last) => (FirstName, LastName) = (first, last);
}

public sealed record Student : Person
{
    public int Level { get; }

    public Student(string first, string last, int level) : base(first, last) => Level = level;
}
```

Records should have to following capabilities:
- Equality is value-based and includes a check that the types match. For example, a Student can't be equal to a Persion, even if the two records share the same name.
- Records have a consistent string representation generated for you.
- Records support copy construction (using cloning)
- Records can be copied with modification

Records support ```with expressions```. A with expression instructs the compiler to create a new copy of a record

```
Person brother = person with { FirstName = "Paul" };
```

## Init only setters

It is possible in C# 9 to create init accessors instead of set accessors for properties and indexers. Callers can use property initializers syntax to set these values in creation expressions. Init only setters provide a way to change state, that window closes when the construction phase ends (after property initializers with expressions have completed).

```
public struct WheaterObservation
{
    public DateTime RecordedAt {get; init; }
    ...
}

var now = new WeatherObservation
{
    RecordedAt = DateTime.Now,
}

// Error!!!!
now.TemparatureInCelsius = 18;

```

## Top-level methods

Replacement of the main method. No namespace definition required. If the compiler finds multiple of these files, it will throw an error.

## Pattern matchin enhancements

New and, or and not keyword. e.g. ```x = e is not null``` instead of ```x = !(e is null)```.

## Performance and interop.

You can add the System.Runtime.CompilerServices.SkipLocalsInitAttribute to instruct the compiler not to emit the localsinit flag. This flag instructs the CLR to zero-initialize all local variables, which may be costly in some cases.

## Fit and finish features.

In c#9 you can omit the type in a new expression if the type is already new
```private List<WheaterObservation> _observations = new();```