# Chapter 6

All types in this section reside in the *System* namespace, except for the following 
- Stringbuilder: System.Text
- CultureInfo: System.Globalization
- XmlConvert: System.Xml

## String and Text Handling

### **Char**

char represents a single Unicdoe character and aliases the System.Char struct

The validity of a char can be checked by calling ```char.GetUnicodeCategory``` if the result is UnicodeCategory.OtherNotAssigned, the character is invalid.

a char is 16 bits wide, enough to represent any Unicode character in the Basic Multilingual Plane.

### **String**

A string is an immutable sequence of characters

#### manipulating strings

As strings are immutable, all methods that "manipulate" a string return a new one, leaving the original untouched. e.g. using ```.Substring(0,3);``` will return a new string.

Using Split also optionally accepts a StringSplitOptions enum which allows to remove empty entries.

Concat is similar to Join but accepts only a params string array and applies no separator. Concat is exactly the same as using the + operator.

Using ```string.Format``` the 'master' string that includes the embedded variables is called the composite format string. each item that is filled in, is called format item.

#### **Ordinal versus culture comparison**

There are 2 basic algorithms for string comparison: ordinal and culture-sensitive. 
- Ordinal comparisons interpret characters simply as numbers
- culture-sensitive comparisons interpret characters with reference to a specific alphabet
- culture-insensitive comparisons interpret characters with reference to a "invariant" culture that is the same on any computer

For ordening, Ordinal groups letters by case (A...Z, a...z) so e.g. for the words "Atom", "atom", and "Zamia" the invariant-culture will order "atom", "Atom", "Zamia" while the ordinal algorithm is going to orden "Atom", "Zamia", "atom".

### String equality comparison

Despite ordinal's limiations, string's == operator always performs **ordinal case sensitive** comparison. The ordinal algorithm was chosen for string's == and Equals functions because it's both highly efficient and deterministic.

the static version of .Equals is advantageous as it will still work if one of the strings is null.

When doing .CompareTo, a culture can be plugged in to allow string comparison for the current user.

#### **StringBuilder**

The stringbuilder represents a mutable string that is much more efficient for editing strings than using operators like '+'

#### **Text Encodings and Unicode**

A character set is an allocation of characters, each with a numeric code or *code point*.
The 2 main sets that are used are ASCII and Unicode. ASCII is the 128 first characters from the Unicode set. In ASCII each character is represented in one byte.

Default in C# is the Unicode character set.

A *text encoding* maps characters from their numeric code point to a binary representation.

There are 2 categories of text encoding .NET
- Those that map Unicode characters to another character set (e.g. ASCII)
- Those that use standard Unicode encoding schemes (e.g. UTF-8, UTF-16, UTF-32)

UTF-8 is the most-efficient and uses between 1 and 4 bytes, it is also the most popular and the default for stream I/O in .NET

UTF-16 is the default for representing characters in .NET.
UTF-32 is less efficient as it uses 32 bits (every character uses 4 bytes).

UTF-16 is represented in code by 'Encoding.Unicode'

It is possible to convert stirngs to a byte[] array and back.

```
byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes("234");
string original = System.Text.Encoding.UTF8.GetString(utf8Bytes);
```

### **UTF-16 and surrogate pairs.**

.NET stores characters and strings in UTF-16. Because an UTF-16 requires **one or two** 16-bit words per character and a char is only 16 bits length, 2 chars are necessary to represent a character. This means

- A string's Length property may be greater than its real character count.
- A single char is not always enough to represent one Unicode character.

Most applications ignore this because nearly all commonly used characters fit into a section of Unicode called the *Basic Multilingual Plane* (BMP), which requires only on 16-bit word.
There are 15 other planes called the "supplementary planes"

two-word characters are called *surrogates* and can be saved in an UTF-32 character.
They can be checked with: ```.IsSurrogate(char c)```

## Dates And Times

3 structs in the System namespace do the job of representing time and dates: DateTime, DateTimeOffset and TimeSpan.

### TimeSpan

Timespan requires an interval of time.
There are 3 ways of creating a timespan:
- By using one of its constructors
- By calling one of the static From... methods
- By substracting one DateTime from another

The timespan allows operators like '<', '>', '+' and ...

### DateTime and DateTimeOffset

immutable structs for representing a date. DateTimeOffset also stores a UTC offset; this allows more meaningful results when comparing values across different time zones.

Internally, DateTimeOffset uses a short integer to store the UTC offset in minutes.

DateTime constructors also allow you to specify a DateTimeKind, an enum with the following values: *Unspecificied (default), Local, Utc*

Implicit casting possible from DateTime to DateTimeOffset which is handy as mosft of.NET Framework supports DateTime not DateTimeOffset.

This means that if the DateTimeKind is Local, the Offset is taken from the current local time zone.

### Formatting and aprsing

Calling .ToString() will return a short date followed by a long time e.g. '11/11/2016 11:50:30 AM'
Calling .ToString() on a DateTimeOffset will return the same but with an offset integrated.

You can also pass e.g. 'o' to the ToString() method which will make it culture invariant.

### Null DateTime and DateTimeOffset

As DateTime and DateTimeOffset are structs, they are not intrinsically nullable, instead you can:

- Use a nullable type (DateTime? and DateTimeOffset?)
- Use the static field DateTime.MinValue or DateTimeOffset.MinValue (the default value for these types)

## Dates and Time Zones

DateTime is stored in 8 bytes. 62 bits for the number of ticks since 1/1/0001 and a 2-biit enum indicating the DateTimeKind

When comparing, only their ticks values are compared. **NOT** the kinds.

### TimeZone and TimeZoneInfo

TimeZoneInfo is the older one and lets you access only the current local time zone. TimeZoneInfo provides access to all the world's time zones

TimeZone.CurrentTimeZone returns a TimeZone object based on your current TimeZone.

TimeZoneInfo.Local does the same thing. But TimeZoneInfo also has the .FindSystemTimeZoneById() which allows additional information

## Formatting and Parsing

### Formatting providers

Format providers allow extensive control over formatting and parsing. The gateway to using a format provider is IFormattable, All numeric types and DateTime(Offset) implement this interface.

```
public interface Iformattable{
    string ToString(string format, IFormatProvider formatProvider);
}
```

e.g.

```
NumberFormatInfo f = new NumberFormatInfo();
f.CurrencySymbol = "$$";
3.ToString("C", f);
```

In the example above "C" means *currency* and the NumberFormatInfo is the formatprovider deciding how it's going to be represented.

.NET Framework defines three format providers,
- NumberFormatInfo
- DateTimeFormatInfo
- CultureInfo

### Composite formatting

Composite formatting allows string.Format to be used with formatProviders
```
string composite = "Credit={0:C}";
WriteLine(string.Format(composite, 500));
```

A stringbuilder can also use the composite format using 'AppendFormat'

### Parsing with format providers

Each type overloads its static Parse and TryParse method to accept a format provider in which such things as whether parantheses or a currency symbol can appear in the input string.