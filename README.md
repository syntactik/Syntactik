# ![Syntactik Logo](https://www.syntactik.com/img/syntactik_small.png "Syntactik Logo") [![Join the chat at https://gitter.im/syntactik/Syntactik](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/syntactik/Syntactik) [![Build status](https://ci.appveyor.com/api/projects/status/n9feo35bjsv2gdvq/branch/master?svg=true)](https://ci.appveyor.com/project/syntactik/syntactik/branch/master)
**Syntactik** - if you want a better XML or JSON, but don't want a change.
# Overview
Syntactik is a people friendly markup language and preprocessor with code reuse features designed to be semantically compatible with XML and JSON.

- Define data using clean and intuitive syntax.
- Create aliases and reuse them as code fragments or document templates.
- Compile Syntactik documents into XML or JSON files.
- Validate Syntactik documents against XML schema.

Syntactik uses indents to define document structure similarly to Python and YAML. Inline and whitespace agnostic syntax is also available if it's needed to keep it short.

The purpose of the language is:
- to expand the audience of people working with data structures.
- to improve the productivity of individuals working with XML and JSON.
- creating new use cases by introducing the data-oriented markup language with people-friendly syntax and code reuse features.
 
# Table of contents
- [Language design objectives](#language-design-objectives)
- [Language design principles](#language-design-principles)
- [Example](#example)
- [Text encoding](#text-encoding)
- [Comments](#comments)
- [Module](#module)
- [Name/value pair](#namevalue-pair)
- [Name](#name)
- [Value](#value)
  * [Literal](#literal)
  * [Block](#block)
  * [Pair value](#pair-value)
- [Assignment](#assignment)
- [String literals](#string-literals)
- [Indent](#indent)
- [Namespace definition](#namespace-definition)
- [Document](#document)
- [Element](#element)
- [Attribute](#attribute)
- [Alias](#alias)
- [Alias definition](#alias-definition)
- [Parameter](#parameter)
- [Argument](#argument)
- [Namespace scope](#namespace-scope)
- [Array](#array)
- [Array assignment](#array-assignment)
- [XML array](#xml-array)
- [Concatenation](#concatenation)
- [Choice](#choice)
- [Literal choice](#literal-choice)
- [Inline syntax](#inline-syntax)
- [WSA mode](#wsa-mode)
- [WSA region](#wsa-region)
- [Comma](#comma)
- [XML mixed content](#xml-mixed-content)
- [Name literal](#name-literal)
- [Command line tool](#command-line-tool)


# Language design objectives
- Must be people friendly
- Minimal number of syntax rules
- Semantic compatibility with XML and JSON
- Must have code reuse features
- Simple implementation of parser. No dependencies on parser generators.

# Language design principles
- Syntactik uses indents to define document structure (like Python, YAML, etc.). 
- Inline and whitespace agnostic syntax is also available to satisfy needs of advanced users.
- Quotes are not required to define literals. Quotes should be used on special occasions like inline definitions, strings with special (escaped) symbols, string interpolation, etc.
- Syntactik supports basic primitives of both XML (like namespaces, elements, and attributes) and JSON (like collections and lists).
- Syntactik has code reuse features in the form of [parameterized aliases](#parameterized-alias) and [string interpolation](#string-interpolation).

# Example

```
'''Namespace definitions
!#ipo = http://www.example.com/ipo 
''' "xsi" is a namespace prefix.
!#xsi = http://www.w3.org/2001/XMLSchema-instance

"""Document "PurchaseOrder" starts on the next line. 
You can define several documents per file."""
!PurchaseOrder: 
    ipo.purchaseOrder: ''' Xml element "purchaseOrder" with namespace prefix "ipo"
        @orderDate == 2016-12-20 '''This is attribute
        shipTo:
            @xsi.type = ipo:EU-Address
            @export-code = 1
            ipo.name = Helen Zoe
            ipo.street = 47 Eden Street
            ipo.city = Cambridge
            ipo.postcode = 126            
        billTo:
            $Address.US.NJ '''Alias with compound name
        Items:
            $Items.Jewelry.Necklaces.Lapis:
                %quantity == 2 '''Argument of the alias
            item:
                @partNum = 748-OT
                productName = Diamond heart
                quantity = 1
                price = 248.90
                ipo.comment = Valentine's day packaging.
                shipDate = 2016-12-24
'''Alias definitions                
!$Address.US.NJ:
    @xsi.type = ipo:US-Address
    ipo.name = Robert Smith
    ipo.street = 8 Main St
    ipo.city = Fort Lee
    ipo.state = NJ
    ipo.zip = 07024            

!$Items.Jewelry.Necklaces.Lapis:
    item:
        @partNum = 833-AA
        productName = Lapis necklace
        quantity := %quantity == 1 '''Parameter with default value
        price = 99.95
        ipo.comment = Need this for the holidays!
        shipDate = 2016-12-12                
```
# Text encoding
- Currently the parser supports **UTF-8**.
- Syntactik is **case sensitive**.
- Indents are defined by tabs (**ASCII 0x9**, \t) or spaces (**ASCII 0x20**) symbols. The parser checks the consistency of indents. It returns an error if indents are defined by both tabs and spaces in the same file. The parser also checks that same number of tabs or spaces is used for one indentation.
- Syntactik supports "Windows" (ASCII **0x0D0A**, \r \n) and "Linux" (ASCII **0x0A**, \n) **newline** symbols. 

# Comments

Single line comments start with ```'''```
```
'''The whole line is a comment
name == John Smith '''The rest of the line is a comment
```
Multiline comments start and end with ```"""```.
```
""" 
If the comment is too long, it can be continued on the 
next line.
"""
name == John Smith """ The multiline comment can start anywhere
and continue as far as needed.
"""
```

# Module
Syntactik has a notion of a **module**. 
A **module** is physically represented by a file with extension **"s4x"** or **"s4j"**.
Module can include [namespace definitions](#namespace-definition), [documents](#document), [alias definitions](#alias-definition), [elements](#element), [attributes](#attribute), [aliases](#alias), [namespace scopes](#namespace-scope).

## s4x-module

**s4x-module** is physically represented by a file with extension **"s4x"** (meaning "syntactik for XML"). Text in **s4x-module** semantically represents XML nodes. 

## s4j-module

**s4j-module** is physically represented by a file with extension **"s4j"** (meaning "syntactik for JSON"). Text in **s4j-module** semantically represents JSON name/value pairs.

The difference between the S4X and S4J semantics is explained later in this document.

# Name/value pair
The **name/value pair** abstraction is a cornerstone of Syntactik language.
This abstraction consists of 3 parts:
- [Name](#name)
- [Assignment](#assignment)
- [Value](#value)

Any part could be omitted. For example, omitted **name** or **value** means that the **name** or **value** is empty. The **assignment** can be only omitted if the **value** is omitted.

# Name
A **name** can be either [open](#open-name) or [quoted](#quoted-name). 

## Open name
An **open name** is defined using [open string](#open-string) rules. **Open name** starts with an optional [prefix](#prefix) followed by an [identifier](#identifier).

### Prefix
A **prefix** defines a type of the language construct. 
If it is omitted, then the language construct is an [element](#element).
This is the list of the prefixes and the corresponding language constructs:

| Prefix        | Type           |
| :-------------: |:-------------|
|      | [Element](#element) |
| `!`   | [Document](#document) |
| `!$` | [Alias Definition](#alias-definition)      |
| `!#` | [Namespace Definition](#namespace-definition) |
| `!%` | [Parameter](#parameter) |
| `#`  | [Namespace Scope](#namespace-scope) |
| `@`  | [Attribute](#attribute)   |
| `$` | [Alias](#alias) |
| `%` | [Argument](#argument) |

The symbols used to define prefix are easy to find on top of the buttons with numbers from 1 to 5:

![prefix symbols](https://www.syntactik.com/img/prefix.png "prefix symbols")

### Identifier

**IDs** are defined by the same rules as for an XML element name:
- Element id is case-sensitive
- Element id must start with a letter or underscore
- Element id can contain letters, digits, hyphens, underscores, and periods
- Element names cannot contain spaces
- See the full spec for the XML Name here http://www.w3.org/TR/REC-xml/#NT-Name


### Compound identifier
An **identifier** with the dot(s) is called a **compound identifier**. Dots split a compound identifier into several parts.
In the  [element](#element), [attribute](#attribute) or [namespace scope](#namespace-scope) a **compound identifier** is used to specify [namespace prefix](#namespace-prefix). 
For example, in the following code, the attribute and the element have the namespace prefix `ipo`.
```
@ipo.export-code = 1
ipo.name = Robert Smith
```
Only first dot is used to define namespace prefix. All following dots are part of the name. So if an identifier starts with `.`, then the name has no namespace prefix, and all following dots will be included in the element's name. The following example shows how to define attributes and elements which doesn't have namespace prefix and have a dot in the name:

```
''' Attribute "name.first"
@.name.first = Robert
''' Elements "name.first" and "name.last"
.name.first = Robert
''' Quoted name is used to create the name with dot
"name.last" = Smith
```

All language constructs except a [namespace definition](#namespace-definition) can have a **compound identifier**.
The compound IDs are used in [alias definitions](#alias-definition) to structure them in the same way classes are structured using namespaces in programming languages:
```
!$Items.Jewelry.Necklaces.Lapis:
...
!$Items.Jewelry.Necklaces.Pearl:
...
!$Items.Jewelry.Diamonds.Heart:
...
!$Items.Jewelry.Diamonds.Uncut:
...
!$Items.Jewelry.Rings.Amber:
...
!$Items.Jewelry.Earrings.Jade:
...
```
>Code editors can take advantage of compound IDs to provide a better code completion.

### Simple identifier
The term **simple identifier** is used as the opposite to the  [compound identifier](#compound-identifier), meaning that the identifier doesn't have any dots in it. Thus it is not divided into parts and representing a single word identifier.
In the example below the `name` is a simple identifier:
```
name = John Smith
```
## Quoted name
A **quoted name** is defined by a [single quoted](#single-quoted-string) or a [double quoted string](#double-quoted-string).
Pair with a **quoted name** always represents an [element](#element).
Elements with a **quoted name** don't have a [namespace prefix](#namespace-prefix).
# Value
There are three types of values: [literal](#literal), [block](#block) and [pair](#pair-value).

## Literal
In Syntactik, **literals** can have the following meaning:

- **string** (*in s4x and s4j-modules*)
- **number** (*only in s4j-modules*)
- **boolean** (*only in s4j-modules*)
- **null** (*only in s4j-modules*)

**Strings** are defined using [string literals](#string-literals). **Number**, **boolean** and **null** literals are defined by [JSON literals](#json-literals).
The omitted [value](#value) represents the **empty string**.

## Block
A **block** is a syntax construct that defines parent/child relationship between pairs. **Blocks** define tree structure of the document. 
**Block** starts with the [object assignment](#object-assignment) `:` or one of the [auxiliary assignments](#auxiliary-assignment) (`::`, `:::`, `=:` or `=::`) followed by indented [name/value pairs](#name-value-pair).
As mentioned in the section ["language design principles"](#language-design-principles), Syntactik uses indents to define document structure. The lines of code, having the same indentation, belong to the same block.

In the following example, the element `billTo` has a block with one attribute and five elements.
```
billTo:
    @xsi.type = ipo:US-Address
    ipo.name = Robert Smith
    ipo.street = 8 Main St
    ipo.city = Fort Lee
    ipo.state = NJ
    ipo.zip = 07024
```

### Object
Syntactik has a notion of an **object**. An **object** is a set of **name/value pairs** where each pair corresponds to XML element, XML attribute, JSON name/value pair or JSON array item.
**Objects** are defined by [blocks](#block) that starts from [object assignment](#object-assignment) `:`, [choice](#choice) `::` or [array assignment](#array-assignment) `:::`.

### Empty object
An empty object is an [object](#object) that doesn't contain any name/value pairs.
The [empty block](#empty-block) represents the **empty object**.

### Empty block
Block can be empty. In this case, it represents the [empty object](#empty-object).
In the example below, the element `items` has the **empty block**.
```
items:
comments = this is the empty order
```

## Pair value
**Pair value** represents a single pair. It can be confused with a [block](#block) with one name/value pair, but a **pair value** has a different meaning. It means that value of the pair is another pair. It is used when we need to specify a [literal alias](#literal-alias) or a [literal parameter](#literal-parameter) as the [literal value](#literal-value) of the pair. See [pair assignment](#pair-assignment) for more details.

# Assignment
Syntactik has three types of assignment operators, one for each [value](#value) type: 
- [Literal Assignment](#literal-assignment)
- [Block Assignment](#block-assignment)
- [Pair Assignment](#pair-assignment).

## Literal assignment
A **literal assignment** is used when the right side of the assignment is a [literal](#literal). There are two subtypes of a literal assignment.
### Free open string assignment
Equality sign `=` is used for :
- [free open strings](#free-open-string)
- [double quoted string](#double-quoted-string)
- [single quoted string](#single-quoted-string)

and their multiline versions:

- [multiline open string](#multiline-open-string)
- [multiline double quoted string](#multiline-double-quoted-string)
- [multiline single quoted string](#multiline-single-quoted-string)
```
name = John Smith
address = 1100 Main St
    Prescott, AZ, 86303
description = 'Text in single quotes' '''Here could be your comment
```
### Open string assignment
Double equality sign `==` is used for:
- [open strings](#open-string)
- [double quoted string](#double-quoted-string)
- [single quoted string](#single-quoted-string)

and their multiline versions:

- [folded open string](#folded-open-string)
- [multiline double quoted string](#multiline-double-quoted-string)
- [multiline single quoted string](#multiline-single-quoted-string)

```
name == John Smith '''Open strings end on quotes so you can put comments at the end of the line
address == "1100 Main St\nPrescott, AZ, 86303" '''Address string has the new line symbol \n
comment == This is a long string which continues
    on the next line, but still, it is a single line string.
```

## Block assignment
A **block assignment** is used to define a [block value](#block) of a pair. **Block** can start with the [object assignment](#object-assignment) `:` or one of the [auxiliary assignments](#auxiliary-assignment) (`::`, `:::`, `=:`, `=::`).

### Object assignment
An **object assignment** is used when the right side of the assignment is an [object](#object). Colon `:` is used for the assignment of an object value.
In the following example, the element `address` has an [object value](#object) which consists of three elements: `street`, `city`, and `state`.
```
address:
    street = 1100 Main St
    city = Prescott
    state = AZ
```

### Auxiliary assignments
**Auxiliary assignments** are the following types of assignment:
- [Array assignment](#array-assignment) `:::`
- [Choice](#choice) `::`
- [Concatenation](#Concatenation) `=:`
- [Literal choice](#literal-choice) `=::`

## Pair assignment
The **pair assignment** `:=` is used when the right side of the assignment is a name/value pair. Assigning [pair value](#pair-value) makes sense when the literal value is stored in [alias](#alias) or [parameter](#parameter).
The **pair assignment** `:=` must be used whenever a [literal alias](#literal-alias) or a [literal parameter](#literal-parameter) is assigned as a value. In the example below, the alias `$ZipCodes.NJ.Fort_Lee` is assigned as a value to the element 'zipCode'.
```
zipCode := $ZipCodes.NJ.Fort_Lee
```
In the next example, the element `quantity` takes its value from parameter `!%quantity` with the default value `1`. In other words, the name/value pair `!%quantity = 1` is assigned to the element `quantity`.
```
item:
	@partNum = 238-KK
	productName = Amber ring
	quantity := !%quantity = 1
	price = 89.90
	ipo.comment = With no inclusions, please.
```
> A [literal alias](#literal-alias) or a [literal parameter](#literal-parameter) can't be assigned with a [literal assignment](#literal-assignment) because they are not literals but pairs.

## Assignment naming convention
There are 8 types of assignments in total: `=` `==` `:` `::` `:::` `:=` `=:` `=::`
It easier to remember the meaning of the assignment if you remember the following rules: 
- Assignments starting with the equality sign `=` assign a literal value.
- Assignments starting with the colon `:` assign an object value. 
- Pair assignment `:=` is a hybrid which assigns literal value stored in a pair.

# String literals
Syntactik has two types of string literals: **quoted string literals** and **open string literals**. 
## Quoted string literals
**Quoted string literals** use single or double quotes to define a literal. So there are [single quoted strings](#single-quoted-string) and [double quoted string](#double-quoted-string).
## Open string literals
**Open string literals** do not use quotes to define a literal. An **open string literal** can be an [open string](#open-string) or a [free open string](#free-open-string).

## Multiline string literal
There is also a **multiline** version of each string type, so in total there are eight variations of **string literals**:
+ the [open string](#open-string) has multiline version called [folded open string](#folded-open-string)
+ the [free open string](#free-open-string) has [multiline open string](#multiline-open-string)
+ the [double quoted string](#double-quoted-string) -  [multiline double quoted string](#multiline-double-quoted-string)
+ the [single quoted string](#single-quoted-string) - [multiline single quoted string](#multiline-single-quoted-string)

## Open string
An **open string** defines a literal without the help of quotes. **Open string** rules apply to the [literals](#literal) that are defined after [literal assignment](#literal-assignment) `==`. **Open string** rules also apply to an [open name](#open-name).
If the [literal value](#literal-value) is omitted, then the **open string** represents the **empty string**.

There are several restrictions on the **open strings**:
+  it can't start or end with whitespace. If a string value starts or ends with the whitespace, then [quoted string](#quoted-string) or [indented open string](#indented-open-string) is the rigth choice
+  it can’t have any special non-visible symbols that require escaping. [Double quoted string](#double-quoted-string) has to be used in this case. If only the new line symbol has to be escaped then [multiline open string](#multiline-open-string) or [folded open string](#folded-open-string) can be used.
+  it can't include any of the [control symbols](#control-symbols): `'` `"` `=` `:` `(` `)` `,`. This is the important rule as it is mostly applied to [open names](#open-name). [Open names](#open-name) will usually end with `=` or `:` symbols, that are the first symbols of a [literal assignment](#literal-assignment) or an [object assignments](#object-assignment). This rule is also important for [inline syntax](#inline-syntax) because it allows ending of one pair and beginning of another in the same line.

In the example below names `items`, `productName`, `quantity`, `price`, and literals `Diamond heart`, `1`, `248.90` are defined by the **open strings**:
```
items:
    productName == Diamond heart
    quantity == 1
    price == 248.90 '''It's possible to add the comment here because open string ends on a single quote
```

There is a multiline version of the **open string** called [folded open string](#folded-open-string).
There is also a [free open string](#free-open-string) which is a less restrictive version of an **open string**.

### Control symbols
Structure of the Syntactik document is defined by space and/or tabs, newline and the following control symbols: `'` `"` `=` `:` `(` `)` `,`. When the parser is processing a text, it is particularly searching for these symbols. Other symbols don't change a state of the parser.
It is important to remember these seven symbols because they are the only visible symbol that can't be included in [open names](#open-name) and [open strings](#open-string). 

## Free open string
A **free open string** defines a literal without the help of quotes like an [open string](#open-string) but with fewer restrictions. **Free open string** rules apply to [literals](#literal) that are defined after [literal assignment](#literal-assignment) `=` if they are not beginning with the quotes `'` `"`.
If the [literal value](#literal-value) is omitted, then the **free open string** represents the **empty string**.
There are just two constraints for the **free open strings**:
+  it can't start or end with whitespace because parser ignores leading and trailing whitespaces. If a string value starts or ends with the whitespace, then [quoted string](#quoted-string) has to be used instead.
+  it can't start with the quotes `'` `"`. Only [quoted string literals](#quoted-string-literals) start with the quotes.
+  it can’t have any non-visible symbols that require escaping. [Double quoted string](#double-quoted-string) has to be used in this case. If only the new line symbol has to be escaped then [multiline open string](#multiline-open-string) or [folded open string](#folded-open-string) can be used.

A **free open string** starts after [literal assignment](#literal-assignment) `=` and continues till the end of the line. There is no way to terminate a **free open string** before the end of the line.

In the example below all literals are defined by the **free open strings**.
```
url = https://www.google.com/
equalitySign = =
emptyString = ''' This is comment. Free open string can't start with quotes.
```
There is the multiline version of the **free open string** called [multiline open string](#multiline-open-string).
> An [indented open string](#indented-open-string) can be used as a workaround for the first two constraints.

## Single quoted string
A **single quoted string** defines a literal using single quote `'`. A **single quoted string** is used to create a [quoted name](#quoted-name) or a [quoted string literal](#quoted-string-literal). 
A **single quoted string** can’t include a single quote or any special non-visible symbols that require escaping (like newline).
In the following example, all literals are defined using a **single quoted string**.
```
dirName == 'c:\Windows'
factOfTheDay == '"Stranger Things" is a science fiction-horror television series.'
```

There is the multiline version of the **single quoted string** called [multiline single quoted string](#multiline-single-quoted-string).

## Double quoted string
A **double quoted string** defines a literal using double quotes `"`. A **double quoted string** is used to create a [quoted name](#quoted-name) or a [quoted string literal](#quoted-string-literal).
A **double quoted string** can include [escape sequences](https://en.wikipedia.org/wiki/Escape_sequence#Programming_languages). 

#### Escape sequences
A [double quoted string](#double-quoted-string) and [multiline double quoted string](#multiline-double-quoted-string) support JSON escape sequences:

| Unicode character value        | Escape sequence           | Meaning  |
| :-------------: |:-------------:|:-------------:|
| `\u0022` | `\"` | quotation mark `"` |
| `\u005C` | `\\` | reverse solidus `\`
| `\u005C` | `\/` | solidus `/`
| `\u0008` | `\b` | backspace |
| `\u000C` | `\f` | formfeed |
| `\u000A` | `\n` | newline |
| `\u000D` | `\r` | carriage return |
| `\u0009` | `\t` | horizontal Tab |
| `\uxxxx` | xxxx | Unicode escape sequence |


## String interpolation 
A **double quoted string** supports [string interpolation](https://en.wikipedia.org/wiki/String_interpolation). A **string interpolation** allows you to insert a value of an alias or parameter in the string. **String interpolation** starts with `\$` (for alias) or `\!%` (for parameter) followed by an [identifier](#identifier) of an alias or parameter. The parentheses `()` can surround the identifier.  The parentheses are optional if the interpolation is followed by space or another symbol that can’t be interpreted as part of the [identifier](#identifier).

Example:

```
line1 = "The customer name is \!%CustomerName"
line2 = "The customer's phone number is $(Phone.AreaCode)-$(Phone.LocalNumber)-$Phone.Extention"
```

## Multiline open string
In a **multiline open string**, each line represents a line of text. The second, third, etc. lines must be indented by precisely one [indent](#indent). Extra indents will be included in the line text.  For example:
```
Comment = This is line number 1. 
    This is line number 2. 
```

The parser ignores the leading whitespaces in the first line and trailing spaces of the last line.

If the first line doesn’t have any symbols, then it will be ignored, and the string will start from the next line.
The **multiline string terminator** `===` is used to specify the end of the string. If it's omitted, then the string will end based on indents, and the last newline symbol will be ignored. The **multiline string terminator** must have the same indent as the name of the current [pair](#namevalue-pair). 

```
Text = 
    This is line number 1. 
    This is line number 2. 
    New line symbol at the end of this line is preserved because === ends this multiline string.
===
```

## Folded open string
Sometimes strings are too long and hard to read and edit. The **folded open string** can help in this case. **Folded open string** is led by the [literal assignment](#literal-assignment) `==`. In the folded open string, a single newline (*\r\n* or *\n*) becomes a space. Two consecutive newline symbols are treated as one; three consequent newlines are treated as two etc. For example:
```
Comment ==
    Line 1 starts here  
    and continues here  
    still line 1  
    finally line 1 end here because next line is empty. 

    Line 2 starts here  
    and continues here.
    Still line 2.
    Line 2 ends.
===
```
Please note that in the example above the first line is empty and, therefore, is ignored. Also, the output string will end with new line symbol because **multiline string terminator** `===` is used to end the **folded open string**.
Indented lines (like second, third, etc) of a **folded open string** can be terminated with no symbols and continues until the end of a line.

## Indented open string
An **indented open string** is a special use case of a [multiline open string](#multiline-open-string) or a [folded open string](#folded-open-string) when the first line of a multiline string is empty (*whitespaces are ignored*). 
In this case, all visible symbols are allowed in the string. Each line can start with and include any visible symbols. It can be used as a workaround for the following limitations:
- [open string](#open-string) and [free open string](#free-open-string) can't start with quotes or whitespace
- [open string](#open-string) can't include any of the [control symbols](#control-symbols): `'` `"` `=` `:` `(` `)` `,`
```
element ==
    "this single line string starts with " and includes = : that are not allowed in the open string"
```
## Multiline single quoted string
A **multiline single quoted string** can be used to define multiline strings. It works like a hybrid of [single quoted string](#single-quoted-string) and [multiline open string](#multiline-open-string). The first line of the string starts after the first single quote `'`. The second and the next lines of the string must be indented. The string ends with the single closing quote. Single quote or any special non-visible symbols that require escaping are not allowed in the string.
```
LoremIpsum = 'Lorem ipsum dolor sit amet, consectetur ""adipiscing"" elit. Pellentesque a congue. 
    Quisque mollis ut odio sed facilisis. Donec dictum ullamcorper lectus, convallis volutpat at'
```
## Multiline double quoted string
A **Multiline double quoted string** allows usage of the [string interpolation](#string-interpolation) and [escape sequences](#escape-sequences) in the multiline string.  At the same time, it works with new lines like [folded open string](#folded-open-string).

```
CheckoutOfferMessage = "\tHi, \$(CustomerName)! Welcome to the check out page!     
    Get a \$OfferName upon approval for the \$CCName. 
    
    The Cart is a temporary place to store a list of your items and 
    reflects each item's most recent price."
```
## JSON literals
Besides **strings**, JSON recognizes **numbers** and literals `true`, `false`, `null`. To be semantically compatible with JSON, Syntactik automatically recognizes **JSON literals** in [open strings](#open-string) inside [s4j-modules](#s4j-modules). **JSON literals** can't be defined in the *quoted string*. 
The example below shows cases when the literal is recognized as a **JSON literal** or a **string**:
```
json_literal_number = 123
string == '123'
json_literal_true = true
string == "true"
json_literal_true == true
json_literal_false = false
string == 'null'
json_literal_null == null
```

# Indent
**Indents** are used to define [blocks](#block) and [multiline string literals](#multiline-string-literal).
Syntactik parser works with both tab and space **indents** like most computer formats and languages. But unlike other formats, Syntactik enforces the same style of indentation in the file. Therefore there can't be indents defined by both tabs and spaces in the same file. The parser also checks that the same number of tabs or spaces is used to define one indent.
The first indent that the parser finds in the file defines the **indent symbol** (space or tab) and the **indent multiplicity** (the number of symbols used for one indent).

# Namespace definition
**Overview**

| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `#` |
| [Identifier](#identifier)    | [simple](#simple-identifier) |
| [Value](#value) | [literal](#literal)      |
| Can be child of | [Module](#module) (only **s4x**), [Document](#document), [Alias Definition](#alias-definition) |

**Description**

Namespaces can be declared in the [Module](#module), [Document](#document) or [Alias Definition](#alias-definition).
Definition of a namespace starts with `!#` followed by a [simple identifier](#simple-identifier) and a [string literal](#literal). A [simple identifier](#simple-identifier) represents an [XML namespace prefix](https://www.w3.org/TR/xml-names11/#NT-Prefix), and a [string literal](#literal) represents an [XML namespace name](https://www.w3.org/TR/xml-names11/#dt-NSName).
For example:
```
!#xsi = http://www.w3.org/2001/XMLSchema-instance
!#ipo = http://www.example.com/myipo
```

## Namespace prefix
A **namespace prefix** is a [simple identifier](#simple-identifier).
The namespace declared in the module is visible in the all documents and alias definitions in this module. The namespace declared in the document or alias definitions is visible only inside the document or alias definition.  The namespace declared in the document or alias definition overrides the namespace with the same namespace prefix declared in the module.

# Document
**Overview**

| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `!` |
| [Identifier](#identifier)    | [simple](#simple-identifier) or [compound](#compound-identifier) |
| [Value](#value) | [block](#block) or [literal](#literal) (*only in s4j-module*)      |
| Can be child of | [Module](#module) |
| Can be parent of | [Namespace Definition](#namespace-definition), [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope) |

**Description**

A **document** starts with the exclamation mark `!` followed by [identifier](#identifier) and [block](#block).
```
!PurchaseOrder: '''Document "PurchaseOrder" starts here
    ipo.purchaseOrder:
        @orderDate = 1999-12-01
        shipTo:
            @xsi.type = ipo:EU-Address
            @export-code = 1
            ipo.name = Helen Zoe
            ipo.street = 47 Eden Street
            ipo.city = Cambridge
            ipo.postcode = 126
```
## s4x-document
An **s4x-document** represents XML file. It is defined in the [**s4x-module**](#s4x-module).
s4x-document must have one root element.

## s4j-document
An **s4j-document** represent then JSON file. It is defined in [**s4j-modules**](#s4j-module).

> Because in JSON the literal value represents the valid document, an **s4j-document** pair can have a [literal assignment](#literal-assignment) and a [literal value](#literal).
```
!document = This is a valid JSON document.
```

## Module document
If a **module** has any [elements](#element), [attributes](#attribute), [aliases](#alias) or [namespace scopes](#namespace-scope) as direct children, then a [document](#document) with the same name as module's file is implicitly declared. This implicitly declared document is called a **module document**. All those pairs will become direct children of the **module document**.

```
''' Module's file name is "purchaseOrder.s4x"
''' The document "purchaseOrder" is implicitly declared.
ipo.purchaseOrder:
    @orderDate = 1999-12-01
    shipTo:
        @xsi.type = ipo:EU-Address
        @export-code = 1
        ipo.name = Helen Zoe
        ipo.street = 47 Eden Street
        ipo.city = Cambridge
        ipo.postcode = 126
```

# Element
**Overview**

| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | none |
| [Identifier](#identifier)    | [simple](#simple-identifier), [compound](#compound-identifier), [quoted name](#quoted-name) |
| [Value](#value) | [literal](#literal) or [block](#block)    |
| Can be child of | [Document](#document), [Element](#element), [Alias Definition](#alias-definition), [Namespace Scope](#namespace-scope), [Alias](#alias), [Argument](#argument), [Parameter](#parameter) |
| Can be parent of | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope), [Parameter](#parameter) |

**Description**

An **element** is the most used type of a [name/value pair](#name-value-pair). It corresponds to XML element and a name\value pair in a JSON object. 
An **element** doesn't have a [prefix](#prefix) in its [name](#name).
If the name of the element is a [compound identifier](#compound-identifier), then the first part of the name represents a [namespace prefix](#namespace-prefix). The [namespace prefix](#namespace-prefix) has to be declared in the [module](#module), [document](#document) or [alias definition](#alias-definition). 
```
ipo.name = Robert Smith
```
An **element** can be defined with a [quoted name](#quoted-name). It is useful in S4J-module because JSON doesn't restrict format of names.
A [quoted name](#quoted-name) can also be used in S4X-modules to define names with a dot(s). Dot defines a [namespace prefix](#namespace-prefix) in an [open name](#open-name), but in a [quoted name](#quoted-name) dot is just part of the name.
```
''' "ipo" is a namespace prefix
ipo.name = John Smith
''' "first" is not a namespace prefix but part of the name
"first.name" = John Smith
```

If [assignment](#assignment) and [value](#value) are omitted, then the element has the [empty object value](#empty-object).

# Attribute
**Overview**

| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `@` |
| [Identifier](#identifier)    | [simple](#simple-identifier) or [compound](#compound-identifier) |
| [Value](#value) | [literal](#literal)    |
| Can be child of | [Element](#element), [Alias Definition](#alias-definition), [Namespace Scope](#namespace-scope), [Alias](#alias), [Argument](#argument), [Parameter](#parameter) |

**Description**

An **attribute** corresponds to an attribute in XML and a name\value pair in JSON object.
If [s4j-document](#s4j-document) has an attribute, then the attribute will be treated as an [element](#element) with the same name.
An attribute starts with "at sign", `@`, followed by an [identifier](#identifier). 
The **attribute** can have only [literal value](#literal). If [assignment](#assignment) or [value](#value) are omitted, then the attribute has the **empty string value**.
```
@orderDate = 2016-12-01
@ipo.export-code = 1
@emptyString = 
@empty
```

# Alias
**Overview**

| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `$` |
| [Identifier](#identifier)    | [simple](#simple-identifier) or [compound](#compound-identifier) |
| [Value](#value) | [literal](#literal) or [block](#block)    |
| Can be child of | [Document](#document), [Element](#element), [Alias Definition](#alias-definition), [Namespace Scope](#namespace-scope), [Alias](#alias), [Argument](#argument), [Parameter](#parameter) |
| Can be parent of | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope), [Argument](#argument), [Parameter](#parameter) |

**Description**

An **alias** is a useful code reuse feature of Syntactik language. An Alias is a short name for a fragment of code.
An alias starts with a dollar sign `$` followed by a [simple identifier](#simple-identifier) or [compound identifier](#compound-identifier). 
In some cases, an **alias** doesn't have a [value](#value) or [assignment](#assignment). In other cases, an **alias** has a [block](#block) or [literal value](#literal) defining a [block of arguments](#block-of-arguments) or [default argument](#default-argument).

An **alias** has to be defined by an [alias definition](#alias-definition). 
It is recommended to use [compound identifiers](#compound-identifier) to organize aliases in a tree structure.
An **alias** can be either a [literal alias](#literal-alias) or an [object alias](#object-alias).

## Literal alias
A **Literal alias** represents a [literal](#literal).
A **Literal alias** must be assigned with [pair assignment](#pair-assignment) `:=`. 
```
''' Alias CurrentDate has the simple identifier and the literal value
@orderDate := $CurrentDate
```
 A **literal alias** can also be used in the [string interpolation](#string-interpolation):
```
''' The alias $customer.firstName has the compound identifier. 
''' The alias is used in the string interpolation.
customerGreating == "Hello, \$(customer.firstName)."
```
## Object alias

An **object alias** has a [block value](#block).
[Block](#block) of an **object alias** can include [elements](#element), [namespace scopes](#namespace-scope), [attributes](#attribute) or [aliases](#alias).
An **object alias** can also represent an [empty object](#empty-object).
Example:
```
ipo.purchaseOrder:
	shipTo: 
	    $Address.US.AK '''This is alias for US address
```
# Alias definition
**Overview**

| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `!$` |
| [Identifier](#identifier)    | [simple](#simple-identifier) or [compound](#compound-identifier) |
| [Value](#value) | [literal](#literal) or [block](#block)    |
| Can be child of | [Module](#module) |
| Can be parent of | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope),  [Parameter](#parameter) |

**Description**

[Aliases](#alias) are defined in a [module](#module). An **alias definition** starts with the exclamation and dollar signs `!$` followed by an [Identifier](#identifier).

## Object alias definition 
## Literal alias definition
**Alias definition** declares either a [literal alias](#literal-alias) or an [object alias](#object-alias).
In the example below, the alias definition declares the object alias `$Address.US` and the literal alias `$Pi`:
```
'''Object Alias Definition
!$Address.US:
    @xsi.type = ipo:US-Address
    ipo.name = Robert Smith
    ipo.street = 8 Oak Avenue
    ipo.city = Old Town
    ipo.state = AK
    ipo.zip = 95819

'''Literal Alias Definition
!$Pi = 3.14159265359
```
# Parameter
**Overview**

| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `!%` |
| [Identifier](#identifier)    | [simple](#simple-identifier) or [compound](#compound-identifier) |
| [Value](#value) | [literal](#literal) or [block](#block)    |
| Can be child of | [Element](#element), [Alias Definition](#alias-definition), [Namespace Scope](#namespace-scope), [Attribute](#attribute), [Alias](#alias), [Argument](#argument), [Parameter](#parameter) |
| Can be parent of | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope),  [Parameter](#parameter) |

**Description**

A **parameter** can be declared only in [alias definition](#alias-definition).
## Parameterized alias
## Parameterized alias definition
An [alias definition](#alias-definition) with **parameters** is called a **parameterized alias definition**. An [alias] that is defined by **parameterized alias definition** is called **parameterized alias**.
There are two types of parameters: [object parameter](#object-parameter) and [literal parameter](#literal-parameter). 
Two or more parameters with the same name are allowed if they have the same type. 

## Object parameter
**Object parameter** represents an [object](#object).
In the example below the alias definition `!$Templates.PurchaseOrder.With.Necklace.Lapis` has 3 object parameters: `!%shipTo`, `!%billTo` and `!%items`.
```
!$Templates.PurchaseOrder.With.Necklace.Lapis:
    purchaseOrder:
        shipTo:
            !%shipTo
        billTo:
            !%billTo
        Items:
            $Items.Jewelry.Necklaces.Lapis
            !%items
```
Parameters `!%shipTo` and `!%billTo` represent the whole object. The parameter `!%items` represents the trailing part of the object where the leading part of the object is defined by the alias `$Items.Jewelry.Necklaces.Lapis`.

## Literal parameter
A **Literal parameter** represents a [literal](#literal). 
A **Literal parameter** must be assigned with [pair assignment](#pair-assignment) `:=`.
In the example below, the alias definition `!$Address.US.NJ` has two literal parameters: `!%name` and `!%street`.
```
!$Address.US.NJ:
    @xsi.type = ipo:US-Address
    ipo.name := !%name
    ipo.street := !%street
    ipo.city = Fort Lee
    ipo.state = NJ
    ipo.zip = 07024
```
 A **Literal parameter** can be also used in the [string interpolation](#string-interpolation).
 In the following example, the literal parameter `!%customerName` is defined in the [literal alias definition](#literal-alias-definition) `!$CustomerGreating` inside of the [string interpolation](#string-interpolation) `'Hello \!%customerName'`.

```
!$CustomerGreating == "Hello \!%customerName"
```

## Default value of parameter
Parameters can have a default value.
In the example below, the parameter `!%shipTo` has the **default block value** assigned by the alias `$Address.UK.Cambridge`. The parameter `!%billTo` has the default block value which consists of 5 elements. The parameter `!%items` has the [empty object](#empty-object) as a default value.
```
!$Templates.PurchaseOrder.International:
    purchaseOrder:
        shipTo:
            !%shipTo:
                $Address.UK.Cambridge
        billTo:
            !%billTo:
                name = Robert Smith
                street = 8 Oak Avenue
                city = Old Town
                state = AK
                zip = 95819
        Items:
            !%items:
```
The [literal parameter](#literal-parameter) can have a default value too.
In the following example, the parameter `!%name` has the default string value `John Smith`. The parameter `!%street` has the default value assigned by the alias `$DefaultStreetAddress`.
```
!$Address.US.NJ:
    @xsi.type = ipo:US-Address
    ipo.name := !%name = John Smith
    ipo.street := !%street := $DefaultStreetAddress
    ipo.city = Fort Lee
    ipo.state = NJ
    ipo.zip = 07024
```

# Argument
**Overview**

| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `%` |
| [Identifier](#identifier)    | [simple](#simple-identifier) or [compound](#compound-identifier) |
| [Value](#value) | [literal](#literal) or [block](#block)    |
| Can be child of | [Alias](#alias) |
| Can be parent of | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope),  [Parameter](#parameter) |

**Description**

An argument starts with `%` followed by an [identifier](#identifier).
If an [alias definition](#alias-definition) has [parameters](#parameter), then the corresponding [Alias](#alias) can have **arguments**. Each argument corresponds to the parameter with the same name. 
There are two types of arguments: [object argument](#object-argument) and [literal argument](#literal-argument). 
The following rules are applied to all arguments:
1. The argument must have the same [value type](#value) (*block* or *literal*) as the corresponding parameter.
2. There can't be two or more arguments with the same name.
3. If the parameter doesn't have a default value, then the corresponding argument must be specified in the alias.
4. If the parameter does have a default value, then the corresponding argument can be omitted.

All arguments are passed to the alias in the [Block of Arguments](#block-of-arguments). 

## Block of arguments
A **block of arguments** is a [block](#block) where each [pair](#namevalue-pair) is an [argument](#argument).
```
$Templates.PurchaseOrder:
    ''' Block of Arguments starts here
	%orderDate = 2017-02-13
	%shipTo: $Address.UK.Cambridge
	%billTo: $Address.US.AK
	%items: 
		$Items.Jewelry.Necklaces.Lapis: %quantity = 1
```
## Object argument
An **object argument** is used to specify the value of an [object parameter](#object-parameter).
In the example below, the alias `$Templates.PurchaseOrder` has the [block of arguments](#block-of-arguments) which consists of 3 **object arguments**: `.shipTo`, `.billTo` and `.items`. 
```
$Templates.PurchaseOrder:
    %shipTo:
        $Address.UK.Cambridge
    %billTo:
        name = Robert Smith
        street = 8 Oak Avenue
        city = Old Town
        state = NJ
        zip = 95819
    %items:
        $Items.Jewelry.Necklaces.Lapis
        $Items.Jewelry.Diamonds.Heart
```
## Literal argument
The **literal argument** is used to specify the value of the [literal parameter](#literal-parameter).
In the following example, the alias `$Items.Jewelry.Necklaces.Lapis` has the [block of arguments](#block-of-arguments) which consists of one **literal argument**: `%quantity` with the assigned [literal](#literal) value `2`. 
```
Items:
    $Items.Jewelry.Necklaces.Lapis: 
        %quantity = 2
```

## Default parameter
## Default argument
If the [alias definition](#alias-definition) has the only [parameter](#parameter) with name `%_` then this parameter is called a **default parameter**. In the corresponding [alias](#alias), you don't need to specify an [argument](#argument) for the **default parameter**. Instead, the value of the argument has to be assigned directly to the alias. 
In the example below:
+ the alias definition `!$Greating` has the default literal parameter `!%_`
+ the alias definition `!$Bold` has the default object parameter `!%_`
+ in the document `!htmlDocument`, the alias `$Bold` has the value of default object parameter defined as `span := $Greating = Hello, World!`
+ the alias `$Greating`, in the alias `$Bold`, has the value of default literal parameter defined as `Hello, World!`
```
!$Greating := !%_

!$Bold:
    b:
        !%_

!htmlDocument:
    html:
        body:
            $Bold:
                span := $Greating = Hello, World!
```

# Namespace scope
**Overview**

| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | `#` |
| [Identifier](#identifier)    | [simple](#simple-identifier), [compound](#compound-identifier) or [omitted](#empty-namespace-scope) |
| [Value](#value) | [literal](#literal) or [block](#block)    |
| Can be child of | [Document](#document), [Element](#element), [Alias Definition](#alias-definition), [Namespace Scope](#namespace-scope), [Alias](#alias), [Argument](#argument), [Parameter](#parameter) |
| Can be parent of | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope), [Parameter](#parameter) |

**Description**

A **namespace scope** is analog of [default namespace](https://www.w3.org/TR/REC-xml-names/#defaulting) in XML. It defines the default namespace for the [elements](#element) that have no [namespace prefix](#namespace-prefix).
A **namespace scope** starts with the hash symbol `#` followed by [simple](#simple-identifier) or [compound identifier](#compound-identifier).
If the **namespace scope**  has a **simple identifier**, then the **simple identifier** represent the [namespace prefix](#namespace-prefix) and is always followed by a [block](#block). All the elements inside this [block](#block) that don't have a [namespace prefix](#namespace-prefix) will get the **default namespace prefix** defined by this **simple identifier**.
In the following example, the scope `#ipo` defines the default namespace for all elements in its [block](#).
```
#ipo:
    purchaseOrder:
        shipTo:
            name = Helen Zoe
            street = 47 Eden Street
            city = Cambridge
            postcode = 126
```
A scope with a **compound identifier** is used to declare a **default namespace prefix** and an [element](#element) at the same time. The first part of the [compound identifier](#compound-identifier) represents the **default namespace prefix**, and the rest represent the name of the element. 
The example below uses the namespace scope with the [compound identifier](#compound-identifier). The scope `#ipo.purchaseOrder` declares the **default namespace prefix** `ipo` and the element `purchaseOrder`. This example will generate the same XML as the example above.
```
#ipo.purchaseOrder:
        shipTo:
            name = Helen Zoe
            street = 47 Eden Street
            city = Cambridge
            postcode = 126
```
The **namespace scope** impacts only elements that are defined directly inside its [block](#block). It doesn't effect elements and attributes defined in [alias definitions](#alias-definitions) of [aliases](#alias) defined in the block.
**Namespace scopes** can be nested. The inner scope override the action the outer scope.

## Empty namespace scope
To clear default namespace prefix value, the name of the scope has to be empty. For example: 
```
#:
    purchaseOrder:
```
**or**
```
#.purchaseOrder:
```
# Array
An **array** is an [object](#object) where each pair represents an array item. Like [objects](#object), **arrays** are defined by [blocks](#block). All root [pairs](#namevalue-pair) of **array**, have no name. A [pair](#namevalue-pair) without name is called an [array item](#array-item).
An **array** can start either with an [object assignment](#object-assignment) `:` or an [array assignment](#array-assignment) `:::`.  
When the parser meets an [object assignment](#object-assignment) `:` it can't tell if the block represents an **array** or a regular object until it processes the first pair in the block. If the first pair of the block is an [array item](#array-item), then the whole block is considered to be an **array**.
The [array assignment](#array-assignment) `:::` explicitly tells parser that the block is an array.
## Empty array
The [empty block](#empty-block) which starts with [array assignment](#array-assignment) `:::` declares an **empty array**.

## Array item
**Overview**

| Syntax feature        | Options           |
| :-------------: |:-------------:|
| [Prefix](#prefix)     | none |
| [Identifier](#identifier)    |  omitted |
| [Value](#value) | [literal](#literal) or [object](#object)    |
| Can be child of | [Document](#document), [Element](#element), [Alias Definition](#alias-definition), [Namespace Scope](#namespace-scope), [Alias](#alias), [Argument](#argument), [Parameter](#parameter) |
| Can be parent of | [Element](#element), [Attribute](#attribute), [Alias](#alias), [Namespace Scope](#namespace-scope), [Parameter](#parameter) |

**Description**

An **array item** is a [name/value pair](#namevalue-pair) without name.
There are two types of the array items: [Literal Array Item](#literal-array-item) and [Object Array Item](#object-array-item).

### Literal array item
A **literal array item** has no [name](#name), so it starts with a [literal assignment](#literal-assignment) followed by an optional [literal value](#literal-value). If the [literal value](#literal-value) is omitted, then the item represents the empty string.
In the example below, [block](#block) of the element `colors` defines **array** of 7 **literal array items**.
```
colors:
    == red ''' primary color
    = orange
    = yellow
    == green  ''' primary color
    == blue  ''' primary color
    = indigo
    = violet
```
Value of a **literal array item** can be also represented by an [alias](#alias) or [parameter](#parameter). In this case it has to start from [pair assignment](#pair-assignment) `:=` followed by an [alias](#alias) or [parameter](#parameter)
```
color_codes:
    := $Colors.Red
    := $Colors.Green
    := $Colors.Blue
```
A **literal array item** can be also implemented as [concatenation](#concatenation) or [literal choice](#literal-choice).

### Object array item
An **object array item** has no [name](#name). It can be implemented as a [block](#block) with [object assignment](#object-assignment) `:`, [choice](#choice) `::` or array assignment `:::`
In the example below, the element `Items` has the value defined as the **array** of 3 **object array items**.
```
Items:
    :
        productName = Lapis necklace
        quantity = 1
        price = 99.95
        ipo.comment = Need this for the holidays!
        shipDate = 1999-12-05
    :
        productName = Diamond heart
        quantity = 1
        price = 248.90
        ipo.comment = Valentine's day packaging.
        shipDate = 2000-02-14
    :
        productName = Uncut diamond
        quantity = 1
        price = 79.90
        shipDate = 2000-01-07
```
**Object array item** starting from the array assignment `:::` represents an [array](#array).
If an **object array item** is a [choice](#choice) `::` then this choice must produce an [array](#array).

## Array block
An **array block** is a block where each pair has no name. **Array block** doesn't always represent an [array](#array). If **block array** starts with [choice](#choice) `::` then it can represent either an [object](#object) or an [array](#array). If **block array** starts with [concatenation](#concatenation) `=:` or [literal choice](#literal-choice) `=::`  then it represents a [literal](#literal).

# Array assignment
The [array assignment](#array-assignment) `:::` explicitly tells the parser that the block is an array.
An [empty block](#empty-block) that starts with the [array assignment](#array-assignment) `:::` declares an [empty array](#empty-array).

```
colors:::
    == red ''' primary color
    = orange
    = yellow
    == green  ''' primary color
    == blue  ''' primary color
    = indigo
    = violet
empty_array:::
```

# XML array
Unlike JSON, XML doesn't support arrays but is used to serialize arrays anyway. For example, the array with color names can be represented like this:
```xml
<colors>
	<color>red</color>
	<color>orange</color>
	<color>yellow</color>
	<color>green</color>
	<color>blue</color>
	<color>indigo</color>
	<color>violet</color>
</colors>
```
In the example above, each element `color` represents an array item.
When Syntactik compiler finds an array that is lead by [array assignment](#array-assignment) `:::`, it adds the name of the pair of the [array block](#array-block) to each [array item](#array-item). So the previous XML can be represented in the [s4x-module](#s4x-module) like this:
```
colors:
    color:::
        == red ''' primary color
        = orange
        = yellow
        == green  ''' primary color
        == blue  ''' primary color
        = indigo
        = violet
```
With the use of the [inline syntax](#inline-syntax) **XML arrays** can be defined in more compact form:
```
primary_colors: color:::
    = red
    = green
    = blue
```
# Concatenation
[Pair](#name-value-pair) with **concatenation** assignment `=:` always has an [array block](#array-block) where each [array item](#array-item) represents a [string literal](#string-literal). All the [string literals](#string-literal) from the [array block](#array-block) are concatenated, and the result value is assigned to the [pair](#name-value-pair).
```
''' abc = abc
abc =:
    = a
    = b
    = c
```
**Concatenation** can be used in the [alias definition](#alias-definition):
```
!$message =:
	= Dear Mr.
	:= !%name
	= ". Your order "
	:= !%orderId
	== " will be shipped on "
	:= !%date
	= .
```

**Concatenation** is the only way to calculate a [string literal](#string-literal) if it includes the value of a [parameterized alias](#parameterized-alias) because a [parameterized alias](#parameterized-alias) can't be used in a [string interpolation](#string-interpolation). Only [alias](#alias) without arguments can be used in a [string interpolation](#string-interpolation).
```
bold_message =:
    = <b>
    := $message:
		%name = John Smith
		%orderId = 123
		%date = 2017-07-13
    = </b>
```

# Choice
A **choice** assignment `::` allows the creation of [object alias definitions](#object-alias-definition) that can be used with different sets of [arguments](#argument). A **choice** assignment can be used in the [alias definition](#alias-definition) pair or/and anywhere in its [block](#block). There can be as many **choices** as needed in one [alias definition](#alias-definition).
[Pair](#namevalue-pair) with **choice** assignment `::` always has an [array block](#array-block). Each [array item](#array-item) in this block represents a **case**. A **case** is a block with or without parameters. When Syntactik compiler is processing an [alias](#alias), and there is a **choice** in the corresponding [alias definition](#alias-definition), then the compiler tries to *resolve* cases in the choice one after another. The first **case** that is successfully resolved represents the value of the **choice** and the processing of the **choice** stops. A **case** is considered to be resolved if all [parameters](#parameters) (*without a [default value](#default-value-of-parameter)*) in the **case** have corresponding [arguments](#argument) in the [alias](#alias). If a **case** has no [parameters](#parameters), then it will always be resolved unless any sibling cases are resolved before.

```
!$coffee_drink::
    :
        capuchino:
            foamed_milk := !%foamed_milk
            steamed_milk := !%steamed_milk
            espresso := !%espresso
    :
        mocha:
            steamed_milk := !%steamed_milk
            chocolate := !%chocolate
            espresso := !%espresso
    :
        americano:
            hot_water := !%hot_water
            espresso := !%espresso
    :
        espresso:
            espresso = 30
coffee_drinks:
    ''' capuchino
    $coffee_drink: 
        %foamed_milk = 60
        %steamed_milk = 60
        %espresso = 60
    ''' mocha
    $coffee_drink: 
        %steamed_milk = 30
        %chocolate = 60
        %espresso = 60
    ''' americano
    $coffee_drink: 
        %hot_water = 90
        %espresso = 60
    ''' espresso is a coffe_drink by default
    $coffee_drink
```
The example above generates the following JSON fragment. Notice that alias `$coffee_drink` is replaced with the correct name of the drink based on the list of ingredients specified as arguments.
```json
{
  "coffee_drinks": {
    "capuchino": {
      "foamed_milk": 60,
      "steamed_milk": 60,
      "espresso": 60
    },
    "mocha": {
      "steamed_milk": 30,
      "chocolate": 60,
      "espresso": 60
    },
    "americano": {
      "hot_water": 90,
      "espresso": 60
    },
    "espresso": {
      "espresso": 30
    }
  }
}
```
# Literal choice
A **literal choice** assignment `=::` allows the creation of [literal alias definitions](#literal-alias-definition) that can be used with different sets of [arguments](#argument). A **literal choice** works exactly like the [choice](#choice) with the exception that each **case** represents not an object but a [literal](#literal).
```
!$url =::
	= "www.\!%(domain)/\!%(path)?\!%params"
	= "www.\!%(domain)/\!%(path)"
	=:	
		= www.
		:= !%domain = example.com
root:
	url1 := $url: 
		%domain = google.com
		%path = search
		%params = q = foo
	url2 := $url: 
		%domain = google.com
		%path = search
	url3 := $url: 
		%domain = google.com
	url4 := $url
```
The example above generates the following JSON fragment:
```json
{
  "root": {
    "url1": "www.google.com/search?q = foo",
    "url2": "www.google.com/search",
    "url3": "www.google.com",
    "url4": "www.example.com"
  }
}
```


# Inline syntax
The **inline syntax** allows defining several language constructs in one line of code.

## Inline definitions 
Two or more [name/value pairs](#namevalue-pair) can be defined on the same line. In this case, they are treated like they are defined in separate lines with the same indent. The first example below shows regular, "one definition per line" way to define a [block](#block) of [elements](#element). The second example shows the example of the **inline syntax** used to define the same [block](#block) of [elements](#element).

```
name = Helen Zoe
street = 47 Eden Street
city = Cambridge
postcode = 126
```
```
name == Helen Zoe, street == 47 Eden Street, city == Cambridge, postcode = 126
```
In the second example, the [open strings](#open-string) are used to define the [string literals](#string-literals). The usage of the [open strings](#open-string) instead of [free open strings](#free-open-string) is essential in this case because it makes possible for the parser to end the strings with a [comma](#comma) `,`. 
Please also note that the last pair in the second example is still using the [free open string](#free-open-string). It works because it is the last pair in the line.
In the previous example, literals were ended by a [comma](#comma). **Quoted literals** end with the closing quote. In this case, Syntactik parser still requires [commas](#comma) between inline pairs. 
```
name == "Helen Zoe", street == "47 Eden Street", city == "Cambridge", postcode == "126"
```
> [Comma](#comma) is required before any new pair that is defined in the same line with its previous sibling pair.

## Inline block
An **inline block** is a [block](#block) that is defined in the same line with the [name](#name) and the [block assignment](#block-assignment).
The first example below shows the element `shipTo` with the regular block definition. The second example shows the same data structure defined using the **inline block** syntax.

```
shipTo:
    name = Helen Zoe
    street = 47 Eden Street
    city = Cambridge
    postcode = 126
```
```
shipTo: name == Helen Zoe, street == 47 Eden Street, city == Cambridge, postcode == 126
```
### Inline name/value pair
[Pair](#namevalue-pair) defined in the inline block is called **inline name/value pair** or **inline pair**.
### Closed inline pair
### Unclosed inline pair
[Inline pair](#inline-namevalue-pair) followed by a [comma](#comma) `,` is called **closed inline pair**. If it is not followed by a [comma](#comma), then it is called **unclosed inline pair**. 
A pair is also called **closed** if it is the last pair of a [closed wsa region](#closed-wsa-region). In other words, closing bracket `)` closes an [inline pair](#inline-namevalue-pair).

## Nested inline blocks
**Nested inline blocks** are nested [blocks](#block) defined in the same line. Any [pair](#namevalue-pair) in the inline block can also have its [inline block](#inline-block). A [pair](#namevalue-pair) in the inline block belongs to the *last declared unclosed* block.
In the following example, elements `name`, `street`, `city` and `postcode` are defined in the [inline block](#inline-block) of the element `shipTo` which itself is in the [inline block](#inline-block) of the element `purchaseOrder`.
```
purchaseOrder: shipTo: name == Helen Zoe, street == 47 Eden Street, city == Cambridge, postcode == 126        
```
With the use of [comma](#comma) `,` it is possible to create the **nested inline block** of any topology. [Comma](#comma) `,` terminates the last unclosed current pair in the [inline block](#inline-block). Just one comma `,` is needed to close a current **empty** inline block. If the inline block is not empty, then two commas `,` are needed (the first comma will close current pair and the second comma will close the current inline block).
In the first example below, the nested blocks are defined by indents. The second example defines the same data structure using the **nested inline block** syntax with commas `,`. The block of `el0` is empty, so it is closed with one comma `,`. The block of `el1` is closed with two commas `,,`. The first comma is used to end pair `el1_2`. The second comma is closing the inline block of `el1`.
```
root:
    el0:
    el1:
        el1_1 = text1_1
        el1_2 = text1_2
    el2:
        el2_1 = text2_1
        el2_2 = text2_2
```
```
root: el0:, el1: el1_1 == text1_1, el1_2 == text1_2,, el2: el2_1 == text2_1, el2_2 == text2_2
```
The readability of the inline block can be improved with parentheses `()`:
```
root: (el0:), (el1: el1_1 == text1_1, el1_2 == text1_2), (el2: el2_1 == text2_1, el2_2 == text2_2)
```
Parentheses `()` are used to create [wsa-region](#wsa-mode). 
The [comma](#comma) `,` is used as a separator between [wsa-regions](#wsa-region).

## Hybrid block
A **hybrid block** is a block that starts as an **inline block** and continues on the next lines like a regular block with indented [pairs](#inline-namevalue-pair).
In the following example, a [block](#block) of the element `shipTo` is a **hybrid block**. The attribute `@export-code` starts on the same line as its parent element `shipTo`. Then block continues as a set of indented pairs.
```
shipTo: @export-code = 1 
	name = Helen Zoe
	street = 47 Eden Street
	city = Cambridge
	postcode = 126
```
There are many ways to represent the same data structure using a **hybrid block**:
```
shipTo: @export-code == 1, name = Helen Zoe
	street == 47 Eden Street
	city = Cambridge
	postcode = 126
```
**or**
```
shipTo: @export-code == 1
    name == Helen Zoe, street == 47 Eden Street
    city == Cambridge, postcode == 126
```

## Inline syntax and multiline literals
[Multiline string literals](#multiline-string-literal) can't be defined in the [inline block](#inline-block).

# WSA mode
# WSA region
**WSA** is an acronym for **whitespace agnostic mode**. It is a mode when the Syntactik parser ignores indents and dedents.
Parentheses `()` define a **wsa-region**. Parentheses can be nested. Parser quits the **whitespace agnostic mode** when the number of closing parentheses `)` is equal the number of opening parentheses `(`.
In a [wsa-region](#wsa-region), the [free open string assignment](#free-open-string-assignment) `=` works like [open string assignment](#open-string-assignment) `==`. It means that it is not possible to define [free open string](#free-open-string) inside [wsa-region](#wsa-region).
It is not possible to define a [multiline string literal](#multiline-string-literal) in a **wsa-region**, because the parser works in **wsa-mode** and ignores indents and dedents.
In the first example below, the blocks are defined by indents. The second example defines the same data structure using the **whitespace agnostic mode**:
```
root:
    el1:
        el1_1 = text1_1
        el1_2 = text1_2
        el1_3 = text1_3
        el1_4 = text1_4
    el2:
        el2_1 = text2_1
        el2_2 = text2_2
```
```
root:
    el1:(
                el1_1 = text1_1,
    el1_2 = text1_2,
            el1_3 = "text1_3", el1_4 = text1_4
    )
    el2: (
        el2_1 = text2_1,
            el2_2 = text2_2
        )
```
> You can think about pairs defined in **wsa-region**, like pairs defined in the [inline block](#inline-block) because indents are ignored and newline just ends the current pair (but does not close it. See [Closed Inline Pair](#closed-inline-pair)).

### Inline WSA region
[WSA-region](#wsa-region) defined in the inline block is called **inline Wsa-Region**.
### Closed WSA Region
### Unclosed WSA Region
[Inline WSA-region](#inline-wsa-region) that followed by a comma `,` is called **closed wsa-region**. If it is not followed by a comma, then it is called **unclosed wsa-region**

# Comma
The **comma** `,` is required between:
- two sibling [inline name/value pairs](#inline-namevalue-pair) 
- two sibling [inline wsa regions](#inline-wsa-regions)
- sibling [inline name/value pairs](#inline-namevalue-pair) and [inline wsa regions](#inline-wsa-regions) (and vice versa)

# XML mixed content
Syntactik allows you to create XML elements with [mixed content](#https://www.w3.org/TR/REC-xml/#sec-mixed-content). The **mixed content** means that XML element's content has at least one text node and at least one element or an attribute. 
To create a **mixed content**, text nodes have to be included in the element's [block](#block) in the form of [literal array item](#literal-array-item). 
Example XML:
```xml
<message>
Dear Mr.<name>John Smith</name>. Your order <orderid>1032</orderid>
will be shipped on <shipdate>2017-07-13</shipdate>.
</message>
```
In Syntactik, the previous XML fragment can be represented like this:
```
message:
    = Dear Mr.
    name = John Smith
    == ". Your order "
    orderid = 1032
    == " will be shipped on "
    shipdate = 2001-07-13
    = .
```

The same data structure defined using the [inline syntax](#inline-syntax):

```
message: == Dear Mr., name == John Smith, == ". Your order ", orderid == 1032, == " will be shipped on ", shipdate == 2001-07-13, == .
```

The same data structure defined using [WSA mode](#wsa-mode):

```
message: ( 
    = "Dear Mr.", name = John Smith,
    = ". Your order ", orderid = 1032,
    = " will be shipped on " shipdate = "2001-07-13" )
```

# Name literal
In [s4j-modules](#s4j-module), if the [assignment](#assignment) and the [value](#value) of the [name/value pair](#name-value-pair) are omitted then the [name](#name) represents a [literal](#literal) and is called **name literal**.
**Name literals** can be used to make [arrays](#array) more compact:
```
colors: red, orange, yellow, green, blue, indigo, violet
```
The same data structure in JSON:
```json
{
  "colors": [
    "red",
    "orange",
    "yellow",
    "green",
    "blue",
    "indigo",
    "violet"
  ]
}
```


# Command line tool
The executable file `slc.exe` is a command line tool that compiles specified files, stores results in the output directory and validate output XML against XML schema if XSD files are listed in the options.
The command line format is:

`slc [options] [inputFiles]`

Options:
+ `-i=DIR`             Input directory with mlx, mlj and xsd files
+ `-o=DIR`             Output directory
+ `-r`                 Turns on recursive search of files in the input directories

You can specify one or many input directories or files of type .s4x, .s4j and xsd. If neither directories nor files are given, then compiler takes them from the current directory. If s4x files are found, then s4j files are ignored.
