## Parts & PN
cplx model **Parts**. Parts are plans to build or procure a thing. Defining a Part is a design activity.[^1]

[^1]: Parts are NOT references tracking real, physical objects, such as inventory items, [digital twins](https://en.wikipedia.org/wiki/Digital_twin), or anything with a [serial number ](https://en.wikipedia.org/wiki/Serial_number)

Parts are identified through an unique [**part number**](https://en.wikipedia.org/wiki/Part_number) (PN).
Part number are generaly defined by the part's manufacturer.

```
TODO : Diagram here, a part
```

## Components & CN
Part are composed of other parts. These are the part's **Components**. Each component is identified inside of the Part through a localy unique **Component Number** (CN). Each component is an **Instance** of the Part it reference : it carries information about the properties and usage of the part in the local context.

```
TODO : Diagram here, a component inside the same part
```

## Properties 
Parts have properties. Properties of the part and its components are used by [**Concepts**](2_DataConcepts) to calculate the overall properties of each part instance. These instances properties are then used to generate documentation.

```
TODO : Diagram here, a component inside the same part
```

## Component Tree & CID
Composing multiple part together produces a **Component Tree**. Each component of the tree is identified through a path-like **Component Identifier** (CID), that aggregate the chain of CN required to reach it from the root part instance.


```
TODO : Diagram, Add another level to the part tree
```

```
TODO : Exemple here of a component tree, with extensive property calculation
```