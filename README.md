CalcBinding is a library that contains advanced Binding markup extension allows you to write binding expressions directly in xaml, without custom converters and stringFormats. CalcBinding make binding expressions shorter and user friendly. See:

Before:

<MultiBinding Conveter={x:StaticResource MyCustomConverter}>
    <Binding A/>
    <Binding B/>
    <Binding C/>
</MultiBinding>
(without MyCustomConveter declaration and referencing to it in xaml)
 
After: 

<c:Binding A+B+C />

Before: 

<Binding IsChecked Converter={x:StaticResource BoolToVisibilityConveter} />
<Binding IsChecked Converter={x:StaticResource NegativeBoolToVisibilityConveter} />
or
<Binding IsChecked Converter={x:StaticResource HiddenBoolToVisibilityConveter} />

After:
<c:Binding IsChecked />
<c:Binding !IsChecked />
<c:Binding IsChecked, FalseToVisibility=Hidden />

CalcBinding determines Visibility target type and converts bool to visibility automaticly for you

#Overview#

You can write any mathematic, logical and string expressions, that contains pathes (as variables) and following operators:


#TODO#

* нужно выделить классы конвертеров отдельно

* порефакторить разросшиеся методы

# README #

This README would normally document whatever steps are necessary to get your application up and running.

### What is this repository for? ###

* Quick summary
* Version
* [Learn Markdown](https://bitbucket.org/tutorials/markdowndemo)

### How do I get set up? ###

* Summary of set up
* Configuration
* Dependencies
* Database configuration
* How to run tests
* Deployment instructions

### Contribution guidelines ###

* Writing tests
* Code review
* Other guidelines

### Who do I talk to? ###

* Repo owner or admin
* Other community or team contact