using System.Collections.Generic;
using System;
using System.Runtime.Versioning;
using System.Threading;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;
using System.Runtime.InteropServices.Marshalling;
#nullable enable

// * The Widow Flower. It grows perishable links at will in every variation.
// * Used to make medicine, its good for analysis purposes.
// * Generalize, but overall a bit more clunky
// * Can properly interface IList since link values are derived as read only.
// * Automatically grows everything. Why bother with dense networks of organised indexing if you're not gonna grow em? Otherwise use a hash table

// * Departed with childrens and webbed list

namespace Caprifolium {
    public class Knautia<Node, Link> : IList<Node> {
        
        // * Properties
        private List<Node> _nodes;
        private List<Link?> _links;
        private readonly int _seedCount;
        private Func<Node[], Link>? _growth; // always auto grow, and links will only ever be an output

        // * Derived Gets
        public IReadOnlyList<Node> Nodes { get => _nodes.AsReadOnly(); }
        public IReadOnlyList<Link?> Links { get => _links.AsReadOnly(); }
        public int SeedCount { get => _seedCount; }
        public Func<Node[], Link>? Growth {
            get => _growth;
            set {SetGrowthFunction(value);}
        }

        public Node GetNode(int index) { return _nodes[index]; }
        
        public List<Node> GetNodes(List<int> indices) {
            List<Node> nodes = new();
            foreach (int index in indices) {
                nodes.Add(_nodes[index]);
            }
            return nodes;
        }
        public List<Node> GetNodes(int linkIndex) {
            return GetNodes(LinkToNodeIndex(linkIndex));
        }
        
        public Link? GetLink(int index) { return _links[index]; }
        public Link? GetLink(List<int> nodeIndices) { return GetLink(NodeToLinkIndex(nodeIndices)); }


        // * Auxillaries
        public List<int> LinkToNodeIndex(int linkIndex) {
            return LinkToNodeIndex(_seedCount, linkIndex);
        }
        public int NodeToLinkIndex(List<int> nodeIndices) {
            return NodeToLinkIndex(_seedCount, nodeIndices);
        }

        // * Constructors
        public Knautia(int seedCount, Func<Node[], Link>? growth = null) {
            _nodes = new();
            _links = new();
            _seedCount = seedCount;
            if (growth != null) { SetGrowthFunction(growth); }
        }

        // * Growth Function
        public bool GrowAll() {
            if (_growth == null) {return false;}
            var exhaustiveIndices = GetExhaustiveIndices(SeedCount, _nodes.Count);
            _links.Clear();
            for (int i = 0; i < exhaustiveIndices.Count; i++) {
                var newLine = new Node[SeedCount];
                for (int j = 0; j < SeedCount; j++) {
                    newLine[j] = _nodes[exhaustiveIndices[i][j]];
                }
                _links.Add(_growth(newLine));
            }
            return true;
        }
        public bool SetGrowthFunction(Func<Node[], Link>? growth) {
            if (growth == null) {_growth = null; return true; }
            try {
                var input = new Node[SeedCount];
                for (int i = 0; i < SeedCount; i++) {
                    input[i] = default(Node);
                }
                var testlink = growth(input);
            }
            catch (Exception) {
                return false;
            }
            _growth = growth;
            return true;
        }

        // * Manipulastion Function
        public void MutateNode(int index, Node newNode) {
            _nodes[index] = newNode;
            // change all corrosponding growths
            if (_growth == null) {return;}
            var exhaustiveIndices = GetExhaustiveIndices(SeedCount, _nodes.Count, index);
            foreach (var line in exhaustiveIndices) {
                GrowLink(line);
            }
        }

        public void GrowLink(int linkIndex) {
            GrowLink(LinkToNodeIndex(linkIndex));
        }
        public void GrowLink(List<int> nodeIndices) {
            if (_growth == null) {return;}
            int linkIndex = NodeToLinkIndex(nodeIndices);
            var nodes = new Node[SeedCount];
            for (int i = 0; i < SeedCount; i++) {
                nodes[i] = _nodes[nodeIndices[i]];
            }
            _links[linkIndex] = _growth(nodes);
        }


        // * Statics
        private static bool nodeIndicesAreValid(int seedCount, List<int> nodeIndices) {
            // * Data Validation -> check for duplicate indices
            if (seedCount != nodeIndices.Count) {
                return false;
                throw new ArgumentException("Number of indices does not match the number of seeds.");
            }
            var uniqueIndices = new HashSet<int>();
            foreach (var index in nodeIndices) {
                if (!uniqueIndices.Add(index)) {
                    return false;
                    throw new ArgumentException("Duplicate values found in nodeIndices.");
                }
            }
            return true;
        }
        private static int combination(int n, int r) {
            if (n < 0 || r < 0) {
                throw new ArgumentException("Invalid values for n and r.");
            }

            if (r > n) {return 0;}
            if (r == 0 || r == n) {return 1;}
                
            
            r = Math.Min(r, n - r);
            
            double result = 1;
            for (double i = 0; i < r; i++) {
                result = result * ((double)n - i) / (i + 1d);
            }
            return (int)Math.Round(result);
        }
        
        public static List<int> LinkToNodeIndex(int seedCount, int linkIndex) {
            List<int> nodeIndices = new();
            for (int i = seedCount; i > 0; i--) {
                int j = i;
                while (combination(j, i) <= linkIndex) {
                    j++;
                }
                nodeIndices.Add(j-1);
                linkIndex -= combination(j-1, i);
            }
            
            nodeIndices.Reverse();
            return nodeIndices;
        }
        public static int NodeToLinkIndex(int seedCount, List<int> nodeIndices) {
            // * Data Validation
            if (!nodeIndicesAreValid(seedCount, nodeIndices)) {
                return -1;
            }
            if (seedCount > nodeIndices.Count) {
                throw new ArgumentException("Number of Node Indices is less than seed count.");
            }

            // * indices are commutative, and hence in sorted order, excess at the end is discarded
            nodeIndices.Sort();

            double accumulator = 0;
            for (int i = 0; i < seedCount; i++) {
                if (nodeIndices[i] >= i+1) {
                    accumulator += combination(nodeIndices[i], i+1);
                }
            }
            return (int)Math.Round(accumulator);
        }
        
        public static List<List<int>> GetExhaustiveIndices(int seedCount, int range) {
            // * Data Validation
            if (range < seedCount) {
                throw new ArgumentOutOfRangeException(
                    $"Range {range} must be larger or equal to SeedCount {seedCount}."
                );
            }
            if (seedCount <= 0) {
                throw new ArgumentOutOfRangeException(
                    $"SeedCount {seedCount} must be larger or equal to 1;"
                );
            }

            // * Create root iteration
            var iteration = new List<int>();
            for (int i = 0; i < seedCount; i++) {
                iteration.Add(i);  // default value
            }

            
            // * Iterate
            List<List<int>> returnList = new();
            do {
                var copyList = new List<int>(iteration);
                returnList.Add(copyList);

                // when increment, all preceding items reset to default
                // only do one increment each cycle. So we'd have to find which one to increment
                int incrementIndex = seedCount-1;

                for (int i = 0; i < seedCount-1; i++) {
                    if (iteration[i]+1 != iteration[i+1]) {
                        incrementIndex = i;
                        break;
                    }
                }

                // increment
                iteration[incrementIndex]++;
                for (int i = 0; i < incrementIndex; i++) {
                    iteration[i] = i; // reset to default value
                }

            } while (!iteration.Contains(range));

            return returnList;
        }
        
        public static List<List<int>> GetExhaustiveIndices(int seedCount, int range, int necessaryIndex) {
            List<int> rangeIndices = new();
            for (int i = 0; i < range; i++) {
                rangeIndices.Add(i);
            }
            return GetExhaustiveIndices(seedCount, rangeIndices, new List<int>(necessaryIndex));
        }
        public static List<List<int>> GetExhaustiveIndices(int seedCount, List<int> rangeIndices, int necessaryIndex) {
            return GetExhaustiveIndices(seedCount, rangeIndices, new List<int>(necessaryIndex));
        }
        public static List<List<int>> GetExhaustiveIndices(int seedCount, int range, List<int> necessaryIndices) {
            List<int> rangeIndices = new();
            for (int i = 0; i < range; i++) {
                rangeIndices.Add(i);
            }
            return GetExhaustiveIndices(seedCount, rangeIndices, necessaryIndices);
        }
        
        public static List<List<int>> GetExhaustiveIndices(int seedCount, List<int> rangeIndices, List<int> necessaryIndices) {
            // * Clean Input
            rangeIndices.AddRange(necessaryIndices);

            rangeIndices = new HashSet<int>(rangeIndices).ToList();
            necessaryIndices = new HashSet<int>(necessaryIndices).ToList();
            rangeIndices.Sort();
            necessaryIndices.Sort();

            // * Data Validation
            if (rangeIndices.Count < seedCount) {
                throw new ArgumentOutOfRangeException(
                    $"Combined indices {rangeIndices.Count} is less than necessary SeedCount {seedCount}."
                );
            }
            foreach (var index in rangeIndices) {
                if (index < 0) {
                    throw new ArgumentOutOfRangeException(
                        $"Index {index} must be 0 or above."
                    );
                }
            }

            // * List Exhaustively
            List<List<int>> returnList = new();
            
            int rangeSize = rangeIndices.Count;

            foreach(var combination in GetExhaustiveIndices(seedCount, rangeSize)) {
                List<int> newLine = new();
                var necessaryIndicesCopy = necessaryIndices;
                foreach(int i in combination) {
                    newLine.Add(rangeIndices[i]);
                    necessaryIndicesCopy.Remove(rangeIndices[i]);
                }
                if (necessaryIndicesCopy.Count <= 0) {
                    returnList.Add(new List<int>(newLine));
                }
            }
            return returnList;
        }


        // * Override
        public Node this[int index] {
            get => _nodes[index];
            set => MutateNode(index, value);
        }

        public int Count => _nodes.Count;
        public bool IsReadOnly => false;

        public bool Contains(Node item) {
            return _nodes.Contains(item);
        }
        public bool Contains(Link item) {
            return _links.Contains(item);
        }

        public void CopyTo(Node[] array, int arrayIndex) {
            _nodes.CopyTo(array, arrayIndex);
        }
        public void CopyTo(Link[] array, int arrayIndex) {
            _links.CopyTo(array, arrayIndex);
        }

        public int IndexOf(Node item) {
            return _nodes.IndexOf(item);
        }
        public int IndexOf(Link item) {
            return _links.IndexOf(item);
        }

        // ? Nodes only - not links
        public IEnumerator<Node> GetEnumerator() {
            return _nodes.GetEnumerator();
        }

        public void Clear() {
            _nodes.Clear();
            _links.Clear();
        }

        public void Add(Node item) { Add(item, true); }
        public void Add(Node item, bool grow = true) {
            Insert(_nodes.Count, item, grow);
        }
        public void Insert(int index, Node item) { Insert(index, item, true); }
        public void Insert(int index, Node item, bool grow = true) {
            if (index < 0 || index > Count) {throw new ArgumentOutOfRangeException($"Index {index} is out of range.");}
            _nodes.Insert(index, item);

            // Get exhaustive range and add from ground up in links
            var exhautiveIndices = GetExhaustiveIndices(SeedCount, _nodes.Count, index);
            foreach (var nodeIndices in exhautiveIndices) {
                int linkIndex = NodeToLinkIndex(nodeIndices);

                var nodeArguments = new Node[SeedCount];
                for (int i = 0; i < SeedCount; i++) {
                    nodeArguments[i] = _nodes[nodeIndices[i]];
                }
                if (_growth != null && grow) {
                   _links.Insert(linkIndex, _growth(nodeArguments)); 
                }
                else {
                    _links.Insert(linkIndex, default(Link)); 
                }
                
            }
        }
        public void AddRange(List<Node> collection) { AddRange(collection, true); }
        public void AddRange(List<Node> collection, bool grow = true) {
            InsertRange(Count, collection, grow);
        }
        public void InsertRange(int index, List<Node> collection) { InsertRange(index, collection, true); }
        public void InsertRange(int index, List<Node> collection, bool grow = true) {
            if (index < 0 || index > Count) {throw new ArgumentOutOfRangeException($"Index {index} is out of range.");}
            for (int i = 0; i < collection.Count; i++) {
                Insert(index + i, collection[i], grow);
            }
        }

        public bool Remove(Node item) {
            int index = _nodes.IndexOf(item);
            if (index >= 0) {
                RemoveAt(index);
                return true;
            }
            return false;
        }
        public void RemoveAt(int index) {
            if (index < 0 || index >= Count) {throw new ArgumentOutOfRangeException($"Index {index} is out of range.");}
            _nodes.RemoveAt(index);

            // get index and remove from top down
            var exhautiveIndices = GetExhaustiveIndices(SeedCount, _nodes.Count+1, index);
            for (int i = exhautiveIndices.Count-1; i >= 0; i--) {
                int linkIndex = NodeToLinkIndex(exhautiveIndices[i]);
                _links.RemoveAt(linkIndex);
            }

        }
        public void RemoveRange(int index, int count) {
            if (index < 0 || index+count > Count) {throw new ArgumentOutOfRangeException($"Index {index} and count {count} is out of range.");}
            for (int _ = 0; _ < count; _++) {
                RemoveAt(index);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _nodes.GetEnumerator();
        }
        
    }
}