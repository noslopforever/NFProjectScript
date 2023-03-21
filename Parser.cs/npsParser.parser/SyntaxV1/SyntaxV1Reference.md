# Brief

# Root Sectors

## Model Define Sector
```
{BaseModelType} {ModelType}
```

## Global Object Define Sector
```
${ModelType} {GlobalObjectName}
```

## Singleton Sector
```
${ModelType}
```

# Non-Root Sectors

## Member Sector
```
-{Type} {Name} |= {Expr}|
-{Name} |= {Expr}|
-{Name}:{Type} |= {Expr}|
- {Name} |= {Expr}|
```

## Method Sector
```
+{Name} |= {Expr}|
+{Name}({Param}...)} |= {Expr}|
+{Name}:{ReturnType} |= {Expr}|
+{Name}({Param}...)}:{ReturnType} |= {Expr}|
+ {Name} |= {Expr}|
+ {Name}({Param}...)} |= {Expr}|
+{ReturnType} {Name} |= {Expr}|
+{ReturnType} {Name}({Param}...)} |= {Expr}|
```

## Component or Child-Element Sector
```
*{ComponentType} |= {Expr}|
*{ComponentType} {ComponentName} |= {Expr}|
--{ChildType} |= {Expr}|
--{ChildType} {ChildName} |= {Expr}|
```

## Event Sector
```
>{EventName} |({Param}...)|
> {EventName} |({Param}...)|
```

## Event Attach Sector
```
>{EventName} += {Expr}
> {EventName} += {Expr}

>+{EventName}
>+ {EventName}
    > {Expr} # Body or Multiline-body
```


## Flow Graph Sector
```
[]{Flowgraph Name}
[] {Flowgraph Name}
```

## StateMachine Sector
```
o=o{StateMachine Name}
o=o {StateMachine Name}
    o- {State Name} # A state in StateMachine
```

## Codeblock sector
\```
```
<<Multi lines>> 
```
\```

## FunctionCall sector
```
>{Expression}
> {Expression}
```
