using System.Collections.Generic;
using System;
using System.Runtime.Versioning;
using System.Threading;
#nullable enable

// * The Widow Flower. It grows perishable links at will in every variation.
// * Used to make medicine, its good for analysis purposes.
// * Generalize, but overall a bit more clunky
// * Can properly interface IList since link values are derived as read only.

// * Departed with childrens and webbed list

namespace Caprifolium {
    public class Knautia<Node,Link> : IList<Node> {
        
        // * Properties
        private List<Node> _nodes;
        private int _seedCount;
        private Func<Node[], Link>? _growth;

        // * Derived Gets
        public IReadOnlyList<Node> Nodes { get => _nodes.AsReadOnly(); }
        public int SeedCount { get => _seedCount; }
        public Func<Node[], Link>? Growth {
            get => _growth;
            set {SetGrowthFunction(value);}
        }

        // * Constructors
        public Knautia(int seedCount, Func<Node[], Link>? Growth) {
            _nodes = new List<Node>();
            _seedCount = seedCount;
        }

        // * Growth Function
        public bool SetGrowthFunction(Func<Node[], Link>? growth) { // ! NOT DONE
            if (growth == null) {return false;}
            try {
                var input = new Node[SeedCount];
                for (int i = 0; i < SeedCount; i++) {
                    input[i] = default(Node);
                }
            }
            catch (Exception) {
                return false;
            }
            return true;
        }

        // * Manipulation Function
        public void MutateNode(int index, Node newNode) { // ! NOT DONE
            _nodes[index] = newNode;
        }


        // * Override
        public Node this[int index] {
            get => _nodes[index];
            set => MutateNode(index, value);
        }
        public int Count => _nodes.Count;
        public bool IsReadOnly => false;
        public void Add(Node item) { // ! NEED TO UPDATE LINKS AS WELL
            _nodes.Add(item);
        }

        public void Clear() { // ! NEED TO UPDATE LINKS AS WELL
            _nodes.Clear();
        }

        public bool Contains(Node item) {
            return _nodes.Contains(item);
        }

        public void CopyTo(Node[] array, int arrayIndex) {
            _nodes.CopyTo(array, arrayIndex);
        }

        public IEnumerator<Node> GetEnumerator() {
            return _nodes.GetEnumerator();
        }

        public int IndexOf(Node item) {
            return _nodes.IndexOf(item);
        }

        public void Insert(int index, Node item) {
            _nodes.Insert(index, item);
        }

        public bool Remove(Node item) {
            return _nodes.Remove(item);
        }

        public void RemoveAt(int index) {
            _nodes.RemoveAt(index);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _nodes.GetEnumerator();
        }
        
    }
}