using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class Graph<T1,T2> : IList<T1> where T1 : Node where T2: GraphEdge<T1>
    {
        private List<T1> nodes;
        private List<T2> edges;

        public List<T1> Nodes
        {
            get
            {
                return nodes;
            }
            private set
            {
                nodes = value;
            }
        }
        public List<T2> Edges
        {
            get
            {
                return edges;
            }
            private set
            {
                edges = value;
            }
        }

        public Graph()
        {
            nodes = [];
            edges = [];
        }

        public T1 this[int index] { get => Nodes[index]; set => Nodes[index] = value; }

        public int Count => Nodes.Count;

        public bool IsReadOnly => false;

        public virtual void Add(T1 item)
        {
            Nodes.Add(item);
        }

        public void Clear()
        {
            Nodes.Clear();
        }

        public bool Contains(T1 item)
        {
            return Nodes.Contains(item);
        }

        public void CopyTo(T1[] array, int arrayIndex)
        {
            Nodes.CopyTo(array,arrayIndex);
        }

        public IEnumerator<T1> GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }

        public int IndexOf(T1 item)
        {
            return Nodes.IndexOf(item);
        }

        public void Insert(int index, T1 item)
        {
            Nodes.Insert(index, item);
        }

        public bool Remove(T1 item)
        {
            if (!this.Contains(item))
            {
                throw new Exception("Node does not exist in the graph");
            } 
            //Remove item from its connections //also delete edges
            foreach (var node in item.Connections)
            {
                node.RemoveConnection(item);
                Edges.Remove((T2)Activator.CreateInstance(typeof(T2), new object[] { item, node }));
            }
            //Remove item from the mainlist
            return Nodes.Remove(item);
        }

        public void RemoveAt(int index)
        {
            Remove(Nodes[index]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
           return ((IEnumerable)Nodes).GetEnumerator();
        }
        public virtual T2 AddEdge(T1 source, T1 target)
        {
            if (source == null || target == null)
            {
                throw new ArgumentNullException();
            }
            if (!this.Contains(source) || !this.Contains(target))
            {
                throw new Exception();
            }
            T2 edge = (T2)Activator.CreateInstance(typeof(T2), new object[] {source, target});
            if (!edges.Contains(edge))
            {
                edges.Add(edge);
                source.AddConnection(target);
                target.AddConnection(source);

            }
            return edge;
        }
        public virtual void RemoveEdge(T1 source, T1 target)
        {
            if (source == null || target == null)
            {
                throw new ArgumentNullException();
            }
            T2 edge = (T2)Activator.CreateInstance(typeof(T2), new object[] { source, target });
            if (edges.Contains(edge))
            {
                edges.Remove(edge);
                source.RemoveConnection(target);
                target.RemoveConnection(source);
                if (target.Connections.Count == 0)
                {
                    this.Remove(target);
                }
                if (source.Connections.Count == 0)
                {
                    this.Remove(source);
                }
            }
            else
            {
                throw new Exception("Edge does not exist");
            }
        }
    }
}
