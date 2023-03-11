
# Brief

# Sectors

## Root Sector

### Syntaxes

Model Define Syntax: {BaseModelType} {ModelType}

Global Object Define Syntax: ${ModelType} {GlobalObjectName}

Singleton Syntax: ${ModelType}

### 

## Element Sector

### Syntaxes

#### DefTags

- Member/Method

+ Components or Sub-Element like UI-controls

-& Reference to a Member/Method (Function Pointer)

> Event

>- Event Attach

` Function body

``` <<Multi lines>>  ``` : Code block

{} Flow Graph

o=o StateMachine

o- A state in StateMachine

#### Member/SubElement syntaxes
{DefTag}{ElementType} {ElementName}|[{Key0} ... {KeyN}]| |= {Expr}|
{DefTag}{ElementName} |= {Expr}|
{DefTag}{ElementName}:{ElementType}[{Key0} ... {KeyN}] |= {Expr}|

#### Method/Event syntaxes
{DefTag}{ReturnType} {ElementName}(%Param0% ... %ParamN%) |= {CodeBlocks}|
{DefTag}{ElementName}(%Param0% ... %ParamN%) |= {CodeBlocks}|

Where Param:
|[Attr0=AttrVal0 ... AttrN=AttrValN]| {ParamName}:{ParamType}|[{Key0} ... {KeyN}]|

#### Expression and Codeblock syntaxes



