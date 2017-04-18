#  CSharp Play

CSharp Play is a C# script playground on Twitter. You can send a C# script to [@CSharpPlay](https://twitter.com/CSharpPlay) by mention and receive the result by reply.

## Usage

Just tweet @CSharpPlay {your C# script}. The length of script is up to 128 (= 140 - 12) characters. You can enjoy C# 6.0 features.

### Expression and Statement

In C# script, _expression_ and _statement_ will be handled in different manner. They are differentiated by ending semicolon. The rule is similar to that of LinqPad but not the same.

 - _Expression_ - No ending semicolon exists. The result will be automatically returned and outputted. The following is an expression and the result value, `3` will be outputted.
 ```csharp
1 + 2
 ```

 - _Statement_ - An ending semicolon exists. The result will not be returned and so if you intend to output the value, you will need to use `return` or  `Console.Write/WriteLine`. The following is a statement and `return` is inserted to output the result value.
  ```csharp
return 1 + 2;
 ```

In contrast to LinqPad, a C# script may contain statement(s) and an expression. The expression must be at the end because it has no ending semicolon. The following is a valid C# script and `3` will be outputted.
  ```csharp
int a = 1;
int b = 2;
a + b
 ```

### Dump extension method

In this playground, you can use LinqPad like `Dump` extension method to output values. It is shorter than `Console.Write/WriteLine` and so useful to save the number of characters. The following example will output `3`.
```csharp
(1 + 2).Dump();
```

This method can be used for any variable in a script without changing its value. Furthermore, it can enumerate each element in a sequence. So the following will output `0`, `1`, `2`, `3`, `9`.
```csharp
var numbers = Enumerable.Range(0, 3).Dump();
var sum = numbers.Sum(x => x).Dump();
Math.Pow(sum, 2).Dump();
```

For some types, there are overloads that take string format. The following will output `2016/01/01`.
```csharp
new DateTime(2016, 1, 1).Dump("yyyy/MM/dd");
```

### Imported namespaces

The following namespaces are imported by default and so you can use them without adding `using` directive.

 - `System`
 - `System.Collections`
 - `System.Collections.Generic`
 - `System.Globalization`
 - `System.Linq` (including Ix = Interactive Extensions)
 - `System.Math`
 - `System.Numerics`
 - `System.Text`
 - `System.Text.RegularExpressions`
 - `System.Xml`
 - `System.Xml.Linq`
 - `Newtonsoft.Json`
 - `Newtonsoft.Json.Linq`

Some namespaces such as `System.IO` and `System.Net` and methods are unusable in order to prevent abuses. `#r` and `#load` directives are the same.

## Libraries

 - [Microsoft.CodeAnalysis.CSharp](https://github.com/dotnet/roslyn)
 - [Interactive Extensions](http://rx.codeplex.com/)
 - [LinqToTwitter](https://linqtotwitter.codeplex.com/)
 - [Newetonsoft.Json](http://www.newtonsoft.com/json)
