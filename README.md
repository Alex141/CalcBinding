# CalcBinding

CalcBinding is an advanced Binding markup extension that allows you to write binding expressions directly in xaml, without custom converters. CalcBinding can automaticaly perfom bool to visibility convertion, inverse your expression and create two way binding, and more. CalcBinding makes binding expressions shorter and more user friendly. [Release notes](https://github.com/Alex141/CalcBinding#release-notes)

## Install

CalcBinding is available at [NuGet](https://www.nuget.org/packages/CalcBinding/). You can install package using:
```
PM> Install-Package CalcBinding 
```

## Overview
Following examples show xaml snippets with standart Binding and with CalcBinding:

## Before:

```xml
<Label>
  <Label.Content>
  <MultiBinding Conveter={x:StaticResource MyCustomConverter}> 
    <Binding A/> 
    <Binding B/> 
    <Binding C/> 
  </MultiBinding>
  </Label.Content>
</Label> 
```

(without MyCustomConveter declaration and referencing to it in xaml)

## After:

```xml
<Label Content="{c:Binding A+B+C }" />
```

## Before:

```xml
<Label>
  <Label.Content>
    <MultiBinding Conveter={x:StaticResource MyCustomConverter2}> 
    <Binding A/> 
    <Binding B/> 
    <Binding C/> 
    </MultiBinding> 
  </Label.Content>
</Label> 
```

(without MyCustomConveter declaration and referencing to it in xaml)

## After:

```xml
<Label Content="{c:Binding A*0.5+(B/C - B%C) }" />
```
## Before:

```xml
<MultiBinding Conveter={x:StaticResource MyCustomConverter3}> 
    <Binding A/> 
    <Binding B/> 
    <Binding C/> 
</MultiBinding> 
```

(without MyCustomConveter declaration and referencing to it in xaml)

## After:

```xml
<c:Binding 'A and B or C' />
```

## Before:

```xml
<Button Visibility="{Binding IsFull Converter={x:StaticResource BoolToVisibilityConveter}}" /> 
<Button Visibility="{Binding IsFull Converter={x:StaticResource NegativeBoolToVisibilityConveter}}" />
```
or
```xml 
<Button Visibility="{Binding IsChecked Converter={x:StaticResource HiddenBoolToVisibilityConveter}}" />
```
## After: 

```xml 
<Button Visibility="{c:Binding IsChecked}" /> 
<Button Visibility="{c:Binding !IsChecked}" /> 
```
or
```xml 
<Button Visibility="{c:Binding IsChecked, FalseToVisibility=Hidden}" />
```
 CalcBinding determines Visibility target type and converts bool to visibility automaticaly for you
 
## Before (Automatic inverse example):
```xml
<TextBox Text = "{Binding Path=A Conveter={x:StaticResource MyMathConverter}">
```

```C#
public class MyMathConverter : IValueConverter
{
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
          var source = (int)value;
          return Math.Sin(source*2)-5;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {        
          var res = Double.Parse(value);
          return (int)(Math.Asin(res + 5) / 2);
        }
}
```

## After:
```xml
<TextBox Text = "{c:Binding 'Math.Sin(source*2)-5'}">
```

 CalcBinding automaticaly inverse your expression (only for Binding not for MultiBinding) and create two way binding. If all of operators that consist your expression have inversed operators, your expression will be automaticaly inversed and binding will be two way: from source to dependency property and from dependency propery to source too.



# Documentation

You can write any mathematic, logical and string expressions, that contains pathes (as variables), strings, digits, all method of class Math (sin, cos, PI etc) and following operators:

```
"(", ")", "+", "-", "*", "/", "%", "^", "!", "&&","||", "&", "|", "?", ":", "<", ">", "<=", ">=", "==", "!="};
```
Examples of supporting binding expressions:

##Math 
```xml
<TextBox Text="{c:Binding A+B+C}"/>
<TextBox Text="{c:Binding A-B-C}"/>
<TextBox Text="{c:Binding A*(B+C)}"/>
<TextBox Text="{c:Binding 2*A-B*0.5}"/>
<TextBox Text="{c:Binding A/B, StringFormat={}{0:n2} --StringFormat is used}"/> {with string format}
<TextBox Text="{c:Binding A%B}"/>
<TextBox Text="{c:Binding '(A == 1) ? 10 : 20'}"/> {ternary operator}
```
##Logic
```xml
<CheckBox Content="!IsChecked" IsChecked="{c:Binding !IsChecked}"/>
<TextBox Text="{c:Binding 'IsChecked and IsFull'}"/> {'and' is equvalent of '&&'}
<TextBox Text="{c:Binding '!IsChecked or (A > B)'}"/> {'or' is equvalent of '||', but you can leave '||'}
<TextBox Text="{c:Binding '(A == 1) and (B less= 5)'}"/> {'less=' is equvalent of '<=')
<TextBox Text="{c:Binding (IsChecked || !IsFull)}"/>
```

##Visibility
bool to visibility two ways convertion runs automaticly:

```xml
<Button Content="TargetButton" Visibility="{c:Binding HasPrivileges, FalseToVisibility=Collapsed}"/>
or just
<Button Content="TargetButton" Visibility="{c:Binding !HasPrivileges}"/>

<Button Content="TargetButton" Visibility="{c:Binding !HasPrivileges, FalseToVisibility=Hidden}"/>
```

##String
```xml
<TextBox Text="{c:Binding (Name + \' \' + Surname)}" />
<TextBox Text="{c:Binding (IsMan?\'Mr\':\'Ms\') + \' \' + Surname + \' \' + Name}"/>
```

##Math Class
```xml
<TextBox Text="{c:Binding Math.Sin(A*Math.PI/180), StringFormat={}{0:n5}}"/>
<TextBox Text="{c:Binding A*Math.PI}" />
<TextBox Text="{c:Binding 'Math.Sin(Math.Cos(A))'}"/>
```

##Automatic inverse binding expression

 If you have binding with expression consisting only of operators that have inversed operators, calcBinding attempt to generate inversed expression for you and create two way binding instead of one way binding. For example, if you have expression 'Path = (A + 5) / 3' inversed expression is 'source = Path * 3 - 5'.
 
 CalcBinding supports inversing of many operators:
 ```
"+", "- (binary)", "*", "/", "Math.Sin", "Math.Cos", "Math.Tan", "Math.Asin", "Math.Acos", "Math.Atan","Math.Pow", "Math.Log", "!", "- (unary)"};
```

#What is inside?

CalcBinding uses DynamicExpresso library to parse string expression to Linq Expression and compiled expression tree for binding.
DynamicExpresso is in fact a fork of DynamicLinq library, with many advantages and bug fixes compared with DynamicLinq (e.x. floating point parsing depending on CurrentCulture damn bug). 

String expression is parsed only one time, when binding is initialized. In init section string expression is parsed, property pathes are selected, variable is created for each property path. Further, expression is parsed into linq Expression which is compiled and finally represents a delegate that takes N parsed variables. When binding is triggered, created delegate is invoked with the new values of variables and result is returned.

Working with the compiled expression increases speed of binding compared with parsing of string expression each time. On the development machine, these times are 0.03s for parsing each time and 0.001-0.003 s for working with the compiled expression

The whole process can be divided into the following stages:

Stage 1: Initialization

1. String expression pre-process: deleting spacebars, replacing operators second names to original names:

```C#
Input:  exprStr = (IsChecked and !(Settings.Count > 0)) ? 'example str 1' : 'example str 2 '
Output: exprStr = (IsChecked&&!(Settings.Count>0))?"example str 1":"example str 2 "
```

2. Expression templating: searching properties pathes and replacing pathes to appropriate variables numbers:
 
```C#
Input: exprStr = (IsChecked&&!(Settings.Count>0))?"example str 1":"example str 2 "
Output: exprStr = ({0}&&!({1}>0))?"example str 1":"example str 2 "
        Pathes = IsChecked - 1, Settings.Count - 2
```

This expression template is transmitted to converter as Converter Parameter

3. (In converter) Expression template parsing and creating of expression dependencing from the variables:

```C#
Input: exprStr = ({0}&&!({1}>0))?"example str 1":"example str 2 "
Output: exprStr = (a&&!(b)>0))?"example str 1":"example str 2 "
         varList = a:Boolean, b:Integer
```

4. (In conveter) Compiling result string expression to delegate:
```C#
Lambda compiledExpression = new Interpreter().Parse(exprStr, varList);
```

Stage 2: Fires when binding Binding fires:

1. (In Converter) Run created delegate with current source values

```C#
var result = compiledExpression.Invoke(values); where values - new binding source values
```

#Q&A

```
1. I wrote logical expression A && B, A < B, A <= B, but my xaml doesn't compile, what's wrong?
```
As Xaml is generally xml format, some symbols are denied in markupExtension: &, &&, <. Therefore, these characters are replaced with the following:

```
&& -> and
|| -> or (not nessesary) 
< -> less
<= -> less=
```

See [logic](https://github.com/Alex141/CalcBinding#logic) section of examples
```
2. I wrote string expression A + " some text", but my xaml doesn't compile, what's wrong?
```

In markup extension we can't use double quotes, so we can use single quotes and backslash for escaping like this:

```xml
<c:Binding Path='A + \'some text\'' />
```

##Restrictions

1. If at least one of the properties involved in expression is null while binding initialization, then expression will not be evaluated, until all properties at least one time will not be not null. Then null property will not have any effect on epxression evaluation.

2. CalcBinding don't support your custom conveters at all now. I did not invent the case for which it would be required in CalcBinding.

3. In path expression you can't use any .Net classes except of Math class.

#Release notes

##version 2.1.1.0

* bug fixes

##version 2.1.0.0

* DataTrigger support
* bug fixes

##version 2.0.0.0

* support of two way binding, support of automatic invert binding expression and realization inverted binding from dependency property to source
* Math class support
* support of nested properties (in nested view-models)
* bug fixes

##version 1.0.0.0

* binding support
* supported features: binding/multibinding, algebraic, logic, string and ternary operators
* support of bool to visibility convertion (two way)
