# CalcBinding

CalcBinding is an advanced Binding markup extension that allows you to write calculated binding expressions in xaml, without custom converters. CalcBinding can automaticaly perfom bool to visibility convertion, different algebraic operations, inverse your expression and more. CalcBinding makes binding expressions shorter and more user-friendly. [Release notes](#release-notes)

## Install

CalcBinding is available at [NuGet](https://www.nuget.org/packages/CalcBinding/). You can install package using:
```
PM> Install-Package CalcBinding 
```

## Overview
Following example shows xaml snippets with standart Binding and with CalcBinding in very simple case:

### Before:

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

### After:

```xml
<Label Content="{c:Binding A+B+C }" />
```

### Key features and restrictions:

1. One or **many** source properties in Path with many available operators: [description](#1-source-properties-and-operators)

  ```xml
  <Label Content="{c:Binding A*0.5+(B.NestedProp1/C - B.NestedProp2 % C) }" />
  ```
  ```xml
  <c:Binding 'A and B or C' />
  ```
2. One or **many static properties** in Path: [description](#2-static-properties)

  ```xml
  <TextBox Text="{c:Binding 'local:StaticClass.Prop1 + local:OtherStaticClass.NestedProp.PropB + PropC'}"/>
  ```
  ```xml
  <Button Background="{c:Binding '(A > B ? media:Brushes.LightBlue : media:Brushes.White)'}"/>
  ```
3. Properties and methods of class **System.Math** in Path: [description](#3-math-class-members)

  ```xml
  <TextBox Text="{c:Binding 'Math.Sin(Math.Cos(A))'}"/>
  ```
4. **Enum** types like constants or source properties in Path: [description](#4-enums)

  ```xml
  <TextBox Text="{c:Binding '(EnumValue == local:CustomEnum.Value1 ? 10 : 20)'}"/>
  ```
5. **Automatic inversion** of binding expression if it's possible: [description](#5-automatic-inversion-of-binding-expression)

  ```xml
  <TextBox Text = "{c:Binding 'Math.Sin(A*2)-5'}"/> {two way binding will be created}
  ```
6. Automatic two way convertion of **bool** expression **to Visibility** and back if target property has such type: [description](#6-bool-to-visibility-automatic-convertion)

  ```xml
  <Button Visibility="{c:Binding !IsChecked}" /> 
  <Button Visibility="{c:Binding IsChecked, FalseToVisibility=Hidden}" />
  ```
7. Other features such as **string and char constants support** and other: [description](#7-other-feautures)

8. General restrictions: [description](#8-general-restrictions)

# Documentation

## 1. Source properties and operators

You can write any algebraic, logical and string expressions, that contain source property pathes, strings, digits, all members of class Math and following operators:

```
"(", ")", "+", "-", "*", "/", "%", "^", "!", "&&","||",
"&", "|", "?", ":", "<", ">", "<=", ">=", "==", "!="};
```
and ternary operator in form of **'bool_expression ? expression_1 : expression_2'**

One should know, that xaml is generally xml format, and xml doesn't support using of following symbols when setting attribute value: **&, <**. Therefore, CalcBinding supports following aliases for operators that contain these symbols:

| operator | alias | comment |
| -------- |:-----:| :-----:|
| && | and |  |
| \|\|      | or      |   not nessesary, just for symmetry |
| < | less      |     |
| <= | less= |        |

### Examples

#### Algebraic 
```xml
<TextBox Text="{c:Binding A+B+C}"/>
<TextBox Text="{c:Binding A-B-C}"/>
<TextBox Text="{c:Binding A*(B+C)}"/>
<TextBox Text="{c:Binding 2*A-B*0.5}"/>
<TextBox Text="{c:Binding A/B, StringFormat={}{0:n2} --StringFormat is used}"/> {with string format}
<TextBox Text="{c:Binding A%B}"/>
<TextBox Text="{c:Binding '(A == 1) ? 10 : 20'}"/> {ternary operator}
```
#### Logic
```xml
<CheckBox Content="!IsChecked" IsChecked="{c:Binding !IsChecked}"/>
<TextBox Text="{c:Binding 'IsChecked and IsFull'}"/> {'and' is equvalent of '&&'}
<TextBox Text="{c:Binding '!IsChecked or (A > B)'}"/> {'or' is equvalent of '||', but you can leave '||'}
<TextBox Text="{c:Binding '(A == 1) and (B less= 5)'}"/> {'less=' is equvalent of '<='}
<TextBox Text="{c:Binding (IsChecked || !IsFull)}"/>
```

### Restrictions:

1. Identifiers that make up the source property path, should be separated from operator ':' by any operator or delimititer (single quote, space etc.) in ternary operator:

#### right:
```<xml>
<TextBox Text="{c:Binding '(A == 2)?IsChecked : IsFull}"/> <!-- right -->
<TextBox Text="{c:Binding '(A == 2)?IsChecked :!IsFull}"/> <!-- right -->
<TextBox Text="{c:Binding '(A == 2) ? IsChecked :4 + IsFull}"/> <!-- right -->
```

#### wrong:
```<xml>
<TextBox Text="{c:Binding '(A == 2)?IsChecked:IsFull}"/> <!-- wrong -->
```

That restricition is caused by path analyzer work that finds [static properties](#2-static-properties)

## 2. Static properties

  Beginning with version 2.3 CalcBinding supports static properties in binding expression. You can write pathes that begin with static property of any class and have any number of properties following behind static property. CalcBinding uses following syntax of static property path declaration:
  
  **'xmlNamespace:Class.StaticProperty.NestedProperty'** etc.
  
where:
  
  1. **xmlNamespace** - usual xml namespace that is mapped to normal namespace in a header of xaml file with other namespaces definitions.   
  
  2. **Class** - name of class that exists in namespace whereto xmlNamespace is mapped
  
  3. **StaticProperty** - static property of class **Class**
  
  4. **.NestedProperty etc** - chain of properties following behind **StaticProperty**
  
### Examples:  
  ```xml
  <TextBox Text="{c:Binding 'local:Class.NestedProp.Prop1 + local:OtherStaticClass.PropB + PropC'}"/>
  ```
  ```xml
  <Button Background="{c:Binding '(A > B ? media:Brushes.LightBlue : media:Brushes.White)'}"/>
  ```
  
  where **local** and **media** are defined in a header of xaml file:
  ```xml
  <<UserControl x:Class="WpfExample.FifthPage"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:local="clr-namespace:WpfExample"
             xmlns:media ="clr-namespace:System.Windows.Media;assembly=PresentationCore">
     ...
  </UserControl>
  ```  
  
### Restrictions
1. As for non-static property pathes for static property pathes following rule is applied: you should put any delimiter or operator between ':' operator of ternary operator and identifiers (namespace or property) that make up static property path:

#### right:
```<xml>
<TextBox Text="{c:Binding '(A == 2)?local:Class.Prop1 : local:Class.Prop2}"/> <!-- right -->
<TextBox Text="{c:Binding '(A == 2)?local:OtherClass.IsChecked :!local.OtherClass.IsFull}"/> <!-- right -->
<TextBox Text="{c:Binding '(A == 2) ? local:Class.A :4 + local:Class.B}"/> <!-- right -->
```

#### wrong:
```<xml>
<TextBox Text="{c:Binding '(A == 2)?local:Class.Prop1: local:Class.Prop2}"/> <!-- wrong -->
<TextBox Text="{c:Binding '(A == 2)?local:OtherClass.IsChecked:local.OtherClass.IsFull}"/> <!-- wrong -->
<TextBox Text="{c:Binding '(A == 2) ? local:Class.A:4+local:Class.B}"/> <!-- wrong -->
```

## 3. Math class members

You can use in path property any members of System.Math class in native form as if you are writing usual C# code:

```xml
<TextBox Text="{c:Binding Math.Sin(A*Math.PI/180), StringFormat={}{0:n5}}"/>
<TextBox Text="{c:Binding A*Math.PI}" />
```

## Restrictions
1. Although CalcBinding supports static properties, Math class is a standalone feature that was created and used before static properties were supported. For this reason you shouldn't use static property syntax with members of Math class. 

#### right:
```xml
<TextBox Text="{c:Binding A*Math.PI}" /> <!-- right -->
<TextBox Text="{c:Binding Math.Sin(10)+20}" /> <!-- right -->
```

#### wrong:
```xml
<xmlns:sys="clr-namespace:System;assembly=mscorlib">
...
<TextBox Text="{c:Binding A*sys:Math.PI}" /> <!-- wrong -->
<TextBox Text="{c:Binding sys:Math.Sin(10)+20}" /> <!-- wrong -->
```

## 4. Enums

Beginning with version 2.3 CalcBinding supports Enums expressions in binding expression. You can write enum values or properties that have Enum type (static properties too). CalcBinding uses following syntax of declaration enum value:
  
  **'xmlNamespace:EnumClass.Value'**
  
where:
  
  1. **xmlNamespace** - usual xml namespace that is mapped to normal namespace in a header of xaml file with other namespaces definitions.
  
  2. **EnumClass** - name of enum class that exists in namespace whereto xmlNamespace is mapped
  
### Examples:  
  ```xml
  <CheckBox Content="Started" IsChecked="{c:Binding 'State==local:StateEnum.Start'}" />
  ```
  ```xml
  <Button Background="{c:Binding 'EnumValue == local:MyEnum.Value1 ? media:Brushes.Green : media:Brushes.Red'}"/>
  ```
  
  where 
  
  1. **local** and **media** are defined in a header of xaml file:
    ```xml
    <<UserControl x:Class="WpfExample.FifthPage"
               xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
              xmlns:local="clr-namespace:WpfExample"
              xmlns:media ="clr-namespace:System.Windows.Media;assembly=PresentationCore">
      ...
    </UserControl>
    ```  
    
  2. **StateEnum, MyEnum** - custom Enums
  
  3. **StateEnum.Start, MyEnum.Value1** - values of custom Enums
  
  4. **Brushes** - standart class with static Brush properties
  
  5. **Brushes.Green, Brushes.Red** - static properties of class Brushes
  
### Restrictions
1. As for static property pathes for Enum constants following rule is applied: you should put any delimiter or operator between ':' operator of ternary operator and identifiers (namespace or property) that make up Enum path:

#### right:
```<xml>
<TextBox Text="{c:Binding '(A == 2)?sys:Visibility.Visible : sys:Visibility.Hidden}"/> <!-- right -->
<TextBox Text="{c:Binding '(A == 2)?local:MyEnum.Value1 : local.MyEnum.Value2}"/> <!-- right -->
```

#### wrong:

```<xml>
<TextBox Text="{c:Binding '(A == 2)?sys:Visibility.Visible:sys:Visibility.Hidden}"/> <!-- wrong -->
<TextBox Text="{c:Binding '(A == 2)?local:MyEnum.Value1: local.MyEnum.Value2}"/> <!-- wrong -->
<TextBox Text="{c:Binding '(A == 2)?local:MyEnum.Value1 :local.MyEnum.Value2}"/> <!-- wrong -->
```

## 5. Automatic inversion of binding expression

  For examle, you have to create two way binding from viewModel with double property A and Content property of TextBox.
  TextBox.Content depends on property 'A' by following formula:
    'Math.Sin(A*2)-5'
  
  All you have to do is to write:
  
  ```xml
  <TextBox Text = "{c:Binding 'Math.Sin(A*2)-5'}">
  ```

CalcBinding recognizes that this expression has inversed expression 'A = Math.Asin(TextBox.Content + 2) / 2' and will use this expression for convertion dependency property TextBox.Text to property A of ViewModel when Text of textBox changes.

Previous expression equivalents to following usual code:

```xml
<TextBox Text = "{Binding Path=A, Conveter={x:StaticResource MyMathConverter}">
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

### Restrictions of creating inversed expression
1.  Binding must include only one property path (static or non-static) and only one entry of it

2. Binding can contain only following operators and methods:

  ```
  "+", "- (binary)", "*", "/", "Math.Sin", "Math.Cos", "Math.Tan", "Math.Asin", 
  "Math.Acos", "Math.Atan","Math.Pow", "Math.Log", "!", "- (unary)"};
  ```

## 6. Bool to Visibility automatic convertion

CalcBinding recognizes if dependency property with Visibility type binds to bool expression. If it's true then bool expression is converted to Visibility automaticaly.

Obsiously **true** expression result is converted to **Visibility.Visible**

Property **FalseToVisibility** of CalcBinding specifies state in which **false** expression result is converted. Flag can have one of the following values:

1. FalseToVisibility.Collapsed (default)
2. FalseToFisibility.Hidden

### Examples
```xml
<Button Content="TargetButton" Visibility="{c:Binding HasPrivileges, FalseToVisibility=Collapsed}"/>
or just
<Button Content="TargetButton" Visibility="{c:Binding !HasPrivileges}"/>

<Button Content="TargetButton" Visibility="{c:Binding !HasPrivileges, FalseToVisibility=Hidden}"/>
```

Automatic inversion is distributed to this convertion too. If dependency property equals to Visibility.Visible, then it's converted to **true**, otherwise - to **false**.

## 7. Other feautures

### String, Char and SingleQuotes mode

Xaml is markup language based on xml language and xml doesn't support double-quotes signs in attribute values. Xaml doesn't support double-quotes too, futhermore it has problems with supporting single-quote character in Path value: in one expressions is works, in other - no. In order to give an opportunity of writing the most compact and readable string constants in the Path (\\', or \&apos; or \&quot;) CalcBinding doesn't make difference between double and single quotes - all quotes are considered as double quotes by defaults. For example:

```xml
<TextBox Text="{c:Binding (Name + \' \' + Surname)}" />
<TextBox Text="{c:Binding (IsMan?\'Mr\':\'Ms\') + \' \' + Surname + \' \' + Name}"/>
```

However, in this case we loose the ability of supporting Char constants. Therefore beginning with version 2.3 CalcBinding has new property - SingleQuotes. If property is true, CalcBinding considers that all quotes - double and single, are single quotes. So \\'A\\' and \&quot;A\&quot; are Char symbols in that mode. If property is false, then single and double quotes are considered as double quotes, it is variant by defaults. So \\'A\\' and \&quot;A\&quot; are String constants in that mode. Examples of Char supporting:

```xml
<TextBox Text="{c:Binding 'Symbol == &quot;S&quot;?4:5', SingleQuotes=True}"/> {can't use no \' nor &apos; symbols because of xaml compiler generates error when parses == operator}
```
where Symbol - Char property.

#### Restrictions:
1. Simultaneous using of Char and String constants is not supported in this version.

### TemplateBinding
Althouth CalcBinding hasn't yet analog for TemplateBinding, as temporary solution you can write as follow: 
```xml
<Button Content="Button" Width="100">
    <Button.Template>
        <ControlTemplate>
            <TextBox Width="{c:Binding Width+10, RelativeSource={RelativeSource TemplatedParent}}"/>
        </ControlTemplate>
    </Button.Template>
</Button> 
```
Setting RelativeSource property to TemplatedParent value makes CalcBinding similar to TemplateBinding

## 8. Tracing
  
  All calcbinding traces are disabled by default due to huge amount of trace messages in some scenarios (see [bug 44](https://github.com/Alex141/CalcBinding/issues/44)).
  
  To enable traces, you need to specify minimal tracing level. Add this code to your app.config file to see all Information or higher priority logs:

```xml
  <system.diagnostics>
    <switches>
      <add name="CalcBindingTraceLevel" value="Information"/>
    </switches>
  </system.diagnostics>
```
  
  Other available tracing levels:
  * All,
  * Off,
  * Critical,
  * Error,
  * Warning,
  * Information,
  * Verbose,
  
  For more information, go to msdn: [SourceSwitch](https://docs.microsoft.com/ru-ru/dotnet/api/system.diagnostics.sourceswitch?view=netframework-4.8)
  
## 9. General restrictions

1. Nullable value types doesn't supported in reverse binding (e.g. mode OneWayToSource)

2. CalcBinding doesn't support your custom conveters at all now. If you need this feature, create new issue and put your using scenario in order to I can see that it is necessary

3. In path expression you can't use any methods of .Net classes except of Math class.

## What is inside?

CalcBinding uses DynamicExpresso library to parse string expression to Linq Expression and compiled expression tree for binding.
DynamicExpresso is in fact a fork of DynamicLinq library, with many advantages and bug fixes compared with DynamicLinq (e.x. floating point parsing depending on CurrentCulture damn bug). 

String expression is parsed only one time, when binding is initialized. In init section CalcBinding analyzer finds tokens in path expression: property path, static property path, Math expression and Enum expression. When binding is triggered first time, special binding converter replaces each property path and static propert path with variable of appropriate type and call DynamicExpresso to compile expression into delegate that takes new variables. 

Working with the compiled expression increases speed of binding compared with parsing of string expression each time. On the development machine, these times are 0.03s for parsing each time and 0.001-0.003 s for working with the compiled expression

### Notes 
  1. Enum constants are using in expression for Dynamic Expresso directly, with collection of types of known Enums.
  2. Binding for collections (ListView, ListBox, DataGrid etc) are created as many times how many times it were declared in xaml. For example, if you have ListView with 10000 elements, and each element have template consisting of 5 controls which are all binded then only 5 Binding instances would be created.
  3. If one or more property pathes changes type of resulting property then compiling expression is recompilied.

# Q&A
```
1. I wrote logical expression A && B, A < B, A <= B, but my xaml doesn't compile, what's wrong?
```
As Xaml is generally xml format, some symbols are denied and one should use it's aliases instead os its. See operators aliases table in section [Source properties and operators](#1-source-properties-and-operators)
```
2. I wrote string expression A + " some text", but my xaml doesn't compile, what's wrong?
```
In markup extension we can't use double quotes, so we can use single quotes and backslash for escaping \\' or xml escape symbol \&quot;. See section [String, Char and SingleQuotes mode](#string-char-and-singlequotes-mode)
```
3. Can I use CalcBinding instead of TemplateBinding?
```
Yes, you can, but with setting RelativeSource property, see section [TemplateBinding](#templatebinding)

# Release notes

## version 2.5.2.0

* Add FallbackValue [#37](https://github.com/Alex141/CalcBinding/issues/37), 
* Disable traces by default, add traces configuration [#44](https://github.com/Alex141/CalcBinding/issues/44)
  
Thanks to [metal450](https://github.com/metal450) for waiting 2 years!!

## version 2.5.1.0
  
* Support <b>.net core 3.0! </b>
  
PR [#57](https://github.com/Alex141/CalcBinding/pull/57). Thanks to [bigworld12](https://github.com/bigworld12) !
  
## version 2.4.0.0

* Fix performance issues: add parsed expressions cache 

bug [#56](https://github.com/Alex141/CalcBinding/issues/56). Thanks to jtorjo!
  
## version 2.3.0.1

* Add support of implicitly and explicitly castable to boolean types in BoolToVisibilityConverter 

(bug [#53](https://github.com/Alex141/CalcBinding/issues/53). Thanks to rstroilov!)



## version 2.3.0.0

* Add support of [Static properties](#2-static-properties), [Enums](#4-enums), [Char constants](#string-char-and-singlequotes-mode). 

Possible problems of switching to this version from older versions:

It is important that names of properties, classes and namespaces that make up sources pathes, would be separated from operator ':' in ternary operator (at least one space or parenthesis) for this version. See section [Restrictions](#restrictions)

## version 2.2.5.2

* fix defect with exception in binding to readonly properties with BindingMode.Default (#41) (thanks to maurosampietro and earthengine!)

Possible problems of switching to this version from older versions:

In older versions CalcBinding creates Binding with BindingMode.TwoWay by default. In new version Binding is created with BindingMode.Default by default (which is more right and standart Binding is doing quite so). Mode = Default means that each DependencyProperty can manage personally in which mode it should be translated. For example, DefaultMode of TextBox.Text is TwoWay, but Label.Content, TextBox.Visibility - OneWay. 
If you used in you applications TwoWay Binding with DependencyProperty that has DefaultMode = BindingMode.OneWay and you didn't specify Mode=TwoWay in xaml so you need to do it in this version for same work.

example.
```xml
<Button Content="TargetButton" Visibility="{c:Binding !HasPrivileges}"/>
```
In older version it creates BindingMode.TwoWay binding, in new version it creates BindingMode.Default, which translated by Label.Visibility in BindingMode.OneWayMode. If you need to stay on BindingMode.TwoWay then just specify it:

```xml
<Button Content="TargetButton" Visibility="{c:Binding !HasPrivileges, Mode=TwoWay}"/>
```
  
## version 2.2.5.1

* fix defect with special characters in const strings in path expression (thanks to xmedeko!)

## version 2.2.5.0

* add support of properties that contain digits in there names (thanks to halllo!)

## version 2.2.4.0

* add support of 'null' values: objects that can be null, nullable value types. For nullable value types there are resctrictions, see restrictions page

## version 2.2.3.0

* add 'not' alias for '!' operator

## version 2.2.2.0

* version for .net 4.0

## version 2.1.2.0

* RelativeSource in multibinding support

## version 2.1.1.0

* bug fixes

## version 2.1.0.0

* DataTrigger support
* bug fixes

## version 2.0.0.0

* support of two way binding, support of automatic invert binding expression and realization inverted binding from dependency property to source
* Math class support
* support of nested properties (in nested view-models)
* bug fixes

## version 1.0.0.0

* binding support
* supported features: binding/multibinding, algebraic, logic, string and ternary operators
* support of bool to visibility convertion (two way)

# Donation

If you like this project you are welcome to support it!

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;USD:

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[![paypal](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=GH8PLP5ZFAJ8Y)

&nbsp;

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;RUB:

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[![paypal](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=WYDUQS5M7QNNQ)
```
