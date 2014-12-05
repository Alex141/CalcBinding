# Overview

CalcBinding is a library that contains advanced Binding markup extension that allows you to write binding expressions directly in xaml, without custom converters and stringFormats. CalcBinding makes binding expressions shorter and user friendly. See:

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
<Label> 
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
<Label> 
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

CalcBinding determines Visibility target type and converts bool to visibility automaticly for you

# Documentation

You can write any mathematic, logical and string expressions, that contains pathes (as variables), strings, digits and following operators:

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

See Logic section of examples
```
2. I wrote string expression A + " some text", but my xaml doesn't compile, what's wrong?
```

In markup extension we can't use double quotes, so we can you single quotes and backslash for escaping like this:

```xml
<c:Binding Path='A + \'some text\'' />
```

##Restrictions

1. If at least one of the properties involved in expression is null while binding initialization, then expression will not be evaluated.
 (fix in the future)

2. CalcBinding don't support your custom conveters at all now. I hope this will be fixed in nearly future.

3. In path expression you can't use .Net classes and methods such as Math or method ToString(). I hope this will be fixed in nearly future.
