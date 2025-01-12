using System.Collections.Generic;
using System;
using System.Runtime.Versioning;
#nullable enable
namespace SineVita.Lonicera {

    public class Lonicera<Node, Link> {
        protected List<Node?> _nodes;
        protected List<Link?> _links;
        public Func<Node?, Node?, Link?>? Growth { get; set; } // (n0, n1) - n0 < n1
        public bool GrowthSynced = false;

        public int NodeCount { get {return _nodes.Count;} }
        public int LinkCount { get {return _links.Count;} }
        public Node? RootNode { get {
            if (NodeCount > 0) {return _nodes[0];} 
            else {return default(Node);}}}   
        public IReadOnlyList<Node?> Nodes => _nodes.AsReadOnly();
        public IReadOnlyList<Link?> Links => _links.AsReadOnly();

        public Lonicera(Func<Node?, Node?, Link?>? growth = null, bool grow = false, List<Node?>? nodes = null, List<Link?>? links = null) {
            Growth = growth;
            if (nodes != null) {_nodes = nodes;} else {_nodes = new List<Node?>();}
            if (links != null) {_links = links;} else {_links = new List<Link?>();}

            for (int i = 0; i < CalculateVineCount(); i++) {
                _links[i] = default(Link);
            }
            if (Growth != null && grow) {Grow();}
        }

        public int CalculateVineCount() {return (int)Math.Floor(NodeCount * (NodeCount + 1) * 0.5f);}       
        public Link? GetValue(int n0, int n1) {
            if (n0 >= NodeCount || n1 >= NodeCount) {
                throw new ArgumentOutOfRangeException("");
            }
            return _links[NodesToLinkIndex(n0, n1)];
        }

        // * Node Insertion Function
        public void Add(Node newNode, bool growNew = true) {
            for (int i = 0; i < NodeCount; i++) {
                if (growNew && Growth != null) {
                    _links.Add(Growth(_nodes[i], newNode));
                }
                else {
                    _links.Add(default(Link));
                }
            }

            
            _nodes.Add(newNode);
        }
        public void Insert(int index, Node newNode, bool growNew = true) {
            _nodes.Insert(index, newNode);
            int rowPivot;
            if (index == 0) {rowPivot = 0;} 
            else {rowPivot = NodesToLinkIndex(0, index-1);}
            
            // add rows
            Link? cacheLink = default(Link);
            for (int i = 0; i < index; i++) {
                if (growNew && Growth != null) {cacheLink = Growth(_nodes[i], _nodes[index]);}
                _links.Insert(rowPivot+i, cacheLink);
            }

            // add collumns
            for (int i = index+1; i < NodeCount+1; i++) {
                if (growNew && Growth != null) {cacheLink = Growth(_nodes[index], _nodes[i]);}
                _links.Insert(NodesToLinkIndex(index, i), cacheLink);
            }
        }
        public void AddRange(List<Node> newNodeRange, bool growNew = true) {
            foreach (Node node in newNodeRange) {
                Add(node, growNew);
            }
        }
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

        public bool MutateNode(int index, Func<Node?, Node?> func, bool growLinkedLinks = false) {
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
            else {GrowthSynced = false; return false;}
        }
        public bool MutateNode(int index, Node? newNode, bool growLinkedLinks = false) {
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
            else {GrowthSynced = false; return false;}
        }
        public void MutateLink(int index, Func<Link?, Link?> func) {
            _links[index] = func(_links[index]);
            GrowthSynced = false;
        }
        public void MutateLink(int index, Link? newLink) {
            _links[index] = newLink;
            GrowthSynced = false;
        }
        
        public void UpdateNodes(Func<Node?, Node?> func) {
            GrowthSynced = false;
            for (int i = 0; i < NodeCount; i++) {_nodes[i] = func(_nodes[i]);}
        }
        public void UpdateLinks(Func<Link?, Link?> func) {
            GrowthSynced = false;
            for (int i = 0; i < LinkCount; i++) {_links[i] = func(_links[i]);}
        }

        public void Grow() {
            if (NodeCount <= 1 || Growth == null) {return;}
            GrowthSynced = true;
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
    }
}