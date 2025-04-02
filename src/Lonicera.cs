using System.Collections.Generic;
using System;
using System.Runtime.Versioning;
using System.Linq;
#nullable enable
namespace Caprifolium {

    public class Lonicera<Node, Link> : IList<Node> {
        protected List<Node> _nodes;
        protected List<Link> _links;
        public Func<Node, Node, Link>? Growth { get; set; } // (n0, n1) - n0 < n1
        private bool _growthSynced = false;

        // * Derived Gets
        public bool GrowthSynced { get { return _growthSynced; } }
        public int NodeCount { get {return _nodes.Count;} }
        public int LinkCount { get {return _links.Count;} }
    
        public IReadOnlyList<Node> Nodes => _nodes.AsReadOnly();
        public IReadOnlyList<Link> Links => _links.AsReadOnly();

        public Lonicera(Func<Node, Node, Link>? growth = null, bool grow = false, List<Node>? nodes = null, List<Link>? links = null) {
            Growth = growth;
            if (nodes != null) {_nodes = nodes;} else {_nodes = new List<Node>();}
            if (links != null) {_links = links;} else {_links = new List<Link>();}

            for (int i = 0; i < CalculateVineCount(); i++) {
                _links[i] = default(Link);
            }
            if (Growth != null && grow) {GrowAll();}
        }

        public int CalculateVineCount() {return (int)Math.Floor(NodeCount * (NodeCount + 1) * 0.5f);}       
        public Link GetValue(int n0, int n1) {
            if (n0 >= NodeCount || n1 >= NodeCount) {
                throw new ArgumentOutOfRangeException(nameof(n0), $"Node indices {n0} or {n1} are out of range.");
            }
            return _links[NodesToLinkIndex(n0, n1)];
        }
        public Link GetLink(int i1, int i2) {
            return _links[NodesToLinkIndex(i1, i2)];
        }
        
        public bool ContainsNode(Node n) {
            return _nodes.Contains(n);
        }
        public bool ContainsLink(Link l) {
            return _links.Contains(l);
        }

        // * Overrides
        public Node this[int index] {
            get => _nodes[index];
            set => MutateNode(index, value);
        }
        public Node RootNode { 
            get => _nodes[0];
            set => MutateNode(0, value);
        }   

        public int Count => _nodes.Count;
        public bool IsReadOnly => false;

        public IEnumerator<Node> GetEnumerator() {
            return _nodes.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _nodes.GetEnumerator();
        }

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

        
        
        
        
        // * Node Insertion Function

        public void Add(Node item) {Add(item, true);}
        public void Add(Node newNode, bool growNew = true) {
            for (int i = 0; i < NodeCount; i++) {
                if (growNew && Growth != null) {
                    _links.Add(Growth(_nodes[i], newNode));
                }
                else {
                    _links.Add(default!);
                }
            }
            _nodes.Add(newNode);
        }
        public void Insert(int index, Node item) {Insert(index, item, true);}
        public void Insert(int index, Node newNode, bool growNew = true) {
            if (index < 0 || index > NodeCount) {
                throw new ArgumentOutOfRangeException(nameof(index), $"Index {index} must be within {0}to {NodeCount}.");
            }
            _nodes.Insert(index, newNode);
            int rowPivot;
            if (index == 0) {rowPivot = 0;} 
            else if (index == 1) {rowPivot = 0;}
            else {rowPivot = NodesToLinkIndex(0, index-1);}
            
            // add rows
            Link cacheLink = default!;
            for (int i = 0; i < index; i++) {
                if (growNew && Growth != null) {cacheLink = Growth(_nodes[i], _nodes[index]);}
                _links.Insert(rowPivot+i, cacheLink);
            }

            // add collumns
            for (int i = index+1; i < NodeCount; i++) {
                if (growNew && Growth != null) {cacheLink = Growth(_nodes[index], _nodes[i]);}
                _links.Insert(NodesToLinkIndex(index, i), cacheLink);
            }
        }
        public void AddRange(List<Node> collection) {AddRange(collection, true);}
        public void AddRange(List<Node> newNodeRange, bool growNew = true) {
            foreach (Node node in newNodeRange) {
                Add(node, growNew);
            }
        }
        public void InsertRange(int index, List<Node> collection) {InsertRange(index, collection, true);}
        public void InsertRange(int index, List<Node> newNodeRange, bool growNew = true) {
            foreach (Node node in newNodeRange) {
                Insert(index, node, growNew);
                index++;
            }
        }
        
        public bool Remove(Node target) {
            for (int i = 0; i < NodeCount; i++) {
                if (_nodes[i] != null) {
                    if (_nodes[i]!.Equals(target)) {
                        RemoveAt(i);
                        return true; 
                    }
                }
            }
            return false;
        }
        public void RemoveAt(int index) {
            // remove row
            if (index != 0) {
                _links.RemoveRange(NodesToLinkIndex(0, index), index);
            }
            // remove collumn
            for (int i = 0; i < NodeCount-index-1; i++) {
                _links.RemoveAt(NodesToLinkIndex(0, i+index+1)-i);
            }
            // remove from node list
            _nodes.RemoveAt(index);
        }
        public void RemoveRange(int index, int count) {
            for (int _ = 0; _ < count; _++) {
                RemoveAt(index);
            }
        }

        public bool MutateNode(int index, Func<Node, Node> func, bool growLinkedLinks = false) {
            _nodes[index] = func(_nodes[index]);
            if (growLinkedLinks && Growth != null) {
                int updateIndex;
                for (int i = 0; i < NodeCount; i++) {
                    if (i != index) {
                        updateIndex = NodesToLinkIndex(index, i);
                        _links[updateIndex] = Growth(_nodes[Math.Min(i,index)], _nodes[Math.Max(i,index)]);
                    }
                }
                return true;
            }
            else {_growthSynced = false; return false;}
        }
        public bool MutateNode(int index, Node newNode, bool growLinkedLinks = false) {
            _nodes[index] = newNode;
            if (growLinkedLinks && Growth != null) {
                int updateIndex;
                for (int i = 0; i < NodeCount; i++) {
                    if (i != index) {
                        updateIndex = NodesToLinkIndex(index, i);
                        _links[updateIndex] = Growth(_nodes[Math.Min(i,index)], _nodes[Math.Max(i,index)]);
                    }
                }
                return true;
            }
            else {_growthSynced = false; return false;}
        }
        public void MutateLink(int index, Func<Link, Link> func) {
            _links[index] = func(_links[index]);
            _growthSynced = false;
        }
        public void MutateLink(int index, Link newLink) {
            _links[index] = newLink;
            _growthSynced = false;
        }
        
        public void UpdateAllNodes(Func<Node, Node> func) {
            _growthSynced = false;
            for (int i = 0; i < NodeCount; i++) {_nodes[i] = func(_nodes[i]);}
        }
        public void UpdateAllLinks(Func<Link, Link> func) {
            _growthSynced = false;
            for (int i = 0; i < LinkCount; i++) {_links[i] = func(_links[i]);}
        }

        // * Growth Function
        public void GrowLink(int n0, int n1) {
            if (Growth == null) {return;}
            if (n0 >= NodeCount || n1 >= NodeCount) {
                throw new ArgumentOutOfRangeException(nameof(n0), $"Node indices {n0} or {n1} are out of range.");
            }
            int linkIndex = NodesToLinkIndex(n0, n1);
            _links[linkIndex] = Growth(_nodes[n0], _nodes[n1]);
        }
        public void GrowLink(int linkIndex) {
            if (Growth == null) {return;}
            if (linkIndex > LinkCount) {
                throw new ArgumentOutOfRangeException(nameof(linkIndex), $"Link index {linkIndex} is out of range.");
            }
            var nodeIndices = LinkToNodesIndex(linkIndex);
            _links[linkIndex] = Growth(_nodes[nodeIndices.Item1], _nodes[nodeIndices.Item2]);
            
        }
        public void GrowAll() {
            if (NodeCount <= 1 || Growth == null) {return;}
            _growthSynced = true;
            int index = 0;
            for (int i = 0; i < NodeCount; i++) {
               for (int j = i+1; j < NodeCount; j++) {
                    _links[index] = Growth(_nodes[i], _nodes[j]);
                } 
            }
        }
        public void Clear() {
            _nodes.Clear();
            _links.Clear();
        }

        
        // * static functions
        public static int NodesToLinkIndex(int n0, int n1) {
            if (n1 < n0) {
                int n = n1;
                n1 = n0;
                n0 = n;
            }
            if (n1 == n0) {
                throw new ArgumentException("The two node index cannot be the same.");
            }
            return (int)Math.Floor(n1 * (n1 - 1) * 0.5f) + n0;}
        public static Tuple<int, int> LinkToNodesIndex(int index) {
            int n1 = (int)Math.Floor(Math.Sqrt(2 * index + 0.25) + 0.5);
            int n0 = index - (int)Math.Floor(n1 * (n1 - 1) * 0.5f);
            return new Tuple<int, int>(n0, n1);
        }
        
    }
}