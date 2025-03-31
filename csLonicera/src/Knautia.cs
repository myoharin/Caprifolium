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
        private List<Link> _links;
        private readonly int _seedCount;
        private Func<Node[], Link>? _growth; // always auto grow, and links will only ever be an output

        // * Derived Gets
        public IReadOnlyList<Node> Nodes { get => _nodes.AsReadOnly(); }
        public IReadOnlyList<Link> Links { get => _links.AsReadOnly(); }
        public int SeedCount { get => _seedCount; }
        public Func<Node[], Link>? Growth {
            get => _growth;
            set {SetGrowthFunction(value);}
        }

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
            SetGrowthFunction(growth);
        }

        // * Growth Function
        public bool SetGrowthFunction(Func<Node[], Link>? growth) { // ! NOT DONE
            if (growth == null) {return false;}
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
        public void MutateNode(int index, Node newNode) { // ! NOT DONE
            _nodes[index] = newNode;
            // change all corrosponding growths
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

        public void Add(Node item) {
            Insert(_nodes.Count, item);
        }
        public void Insert(int index, Node item) {  // ! NEED TO UPDATE LINKS AS WELL
            if (index < 0 || index > Count) {throw new ArgumentOutOfRangeException($"Index {index} is out of range.");}
            _nodes.Insert(index, item);
        }
        public void AddRange(List<Node> collection) {
            InsertRange(Count, collection);
        }
        public void InsertRange(int index, List<Node> collection) {
            if (index < 0 || index > Count) {throw new ArgumentOutOfRangeException($"Index {index} is out of range.");}
            for (int i = 0; i < collection.Count; i++) {
                Insert(index + i, collection[i]);
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
        public void RemoveAt(int index) {  // ! NEED TO UPDATE LINKS AS WELL
            if (index < 0 || index >= Count) {throw new ArgumentOutOfRangeException($"Index {index} is out of range.");}
            _nodes.RemoveAt(index);
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