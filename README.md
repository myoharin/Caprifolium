# SineVita.Lonicera

Lonicera is a data structure used to contain interlinked nodes, where every possible pair of nodes is ordered in a consistent list and interacts via a "Growth" lambda function. This is particularly useful when tracking the number of nodes in a relation web or the intervals between every note in a chord.

## Type Parameters

- **Node**: The type representing the nodes in the structure.
- **Link**: The type representing the links between nodes.

## Properties

- **NodeCount**: 
  - Returns the number of nodes in the structure.
  
- **LinkCount**: 
  - Returns the number of links in the structure.
  
- **RootNode**: 
  - Gets the root node of the structure. Returns `null` if there are no nodes.

- **Nodes**: 
  - Provides a read-only view of the list of nodes.

- **Links**: 
  - Provides a read-only view of the list of links.

- **Growth**: 
  - A function that defines how links are created between nodes. Takes two nodes as input and returns a link.

- **GrowthSynced**: 
  - A boolean that indicates whether the growth has been synchronized.

## Constructor

```csharp
public Lonicera(Func<Node, Node, Link>? growth = null, bool grow = false, List<Node>? nodes = null, List<Link>? links = null);
```

### Parameters

- **growth**: A delegate for custom growth logic.
- **grow**: A boolean indicating whether to grow the structure immediately.
- **nodes**: An optional list of initial nodes.
- **links**: An optional list of initial links.

## Methods

### `int CalculateVineCount()`

Calculates the number of links in a complete graph based on the current number of nodes.

### `Link GetValue(int n0, int n1)`

Retrieves the link value between two nodes.

#### Parameters

- **n0**: The index of the first node.
- **n1**: The index of the second node.

#### Returns

- The link between the two nodes.

#### Exceptions

- Throws `ArgumentOutOfRangeException` if either index is out of bounds.

### `void Add(Node newNode, bool growNew = true)`

Adds a new node to the structure.

#### Parameters

- **newNode**: The node to add.
- **growNew**: Indicates whether to grow the links for the new node.

### `void Insert(int index, Node newNode, bool growNew = true)`

Inserts a new node at a specified index.

#### Parameters

- **index**: The index at which to insert the new node.
- **newNode**: The node to insert.
- **growNew**: Indicates whether to grow the links for the new node.

### `void AddRange(List<Node> newNodeRange, bool growNew = true)`

Adds a range of nodes to the structure.

### `void InsertRange(int index, List<Node> newNodeRange, bool growNew = true)`

Inserts a range of nodes starting at a specified index.

### `bool Remove(Node target)`

Removes a specified node from the structure.

#### Returns

- `true` if the node was removed; otherwise, `false`.

### `void RemoveAt(int index)`

Removes a node at a specified index.

### `void RemoveRange(int index, int count)`

Removes a range of nodes starting at a specified index.

#### Parameters

- **index**: The starting index.
- **count**: The number of nodes to remove.

### `void Update_nodes(Func<Node, Node> func)`

Updates the nodes in the structure using a specified function.

#### Parameters

- **func**: A function that takes a node and returns an updated node.

### `void Update_links(Func<Link, Link> func)`

Updates the links in the structure using a specified function.

#### Parameters

- **func**: A function that takes a link and returns an updated link.

### `void Grow()`

Applies growth logic to update the links based on the current nodes.

### `void Clear()`

Clears all nodes and links in the structure.

## Static Methods

### `static int NodesToLinkIndex(int n0, int n1)`

Converts two node indices to a single link index.

#### Parameters

- **n0**: The first node index.
- **n1**: The second node index.

#### Returns

- The calculated link index.

#### Exceptions

- Throws `ArgumentException` if the two node indices are the same.

## Example Usages

### Example 1: Combining Nodes

```csharp
Lonicera<int, Tuple<int, int>> loniTuple = new Lonicera<int, int>(combineToTuple);
loniTuple.AddRange(new List<int>(){0,1,2,3,4,5,6,7,8,9});
print() // assume method exist for Lonicera<int, int>
```
| | | | | | | | | | |
|-|-|-|-|-|-|-|-|-|-|
| (0, 1) |
| (0, 2) | (1, 2) |
| (0, 3) | (1, 3) | (2, 3) |
| (0, 4) | (1, 4) | (2, 4) | (3, 4) |
| (0, 5) | (1, 5) | (2, 5) | (3, 5) | (4, 5) |
| (0, 6) | (1, 6) | (2, 6) | (3, 6) | (4, 6) | (5, 6) |
| (0, 7) | (1, 7) | (2, 7) | (3, 7) | (4, 7) | (5, 7) | (6, 7) |
| (0, 8) | (1, 8) | (2, 8) | (3, 8) | (4, 8) | (5, 8) | (6, 8) | (7, 8) |
| (0, 9) | (1, 9) | (2, 9) | (3, 9) | (4, 9) | (5, 9) | (6, 9) | (7, 9) | (8, 9) |

### Example 2: Multiplying Nodes

```csharp
Lonicera<int, int> loniMulti = new Lonicera<int, int>(multiply);
loniMulti.AddRange(new List<int>(){0,1,2,3,4,5,6,7,8,9});
print() // assume method exist for Lonicera<int, int>
```

| | | | | | | | | | |
|-|-|-|-|-|-|-|-|-|-|
| 0 |
| 0 | 2 |
| 0 | 3 | 6 |
| 0 | 4 | 8 | 12 |
| 0 | 5 | 10 | 15 | 20|
| 0 | 6 | 12 | 18 | 24| 30|
| 0 | 7 | 14 | 21 | 28| 35| 42|
| 0 | 8 | 16 | 24 | 32| 40| 48| 56|
| 0 | 9 | 18 | 27| 36| 45| 54| 63| 72|