using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class CaricTree<T> : ICollection<CaricNode<T>> where T : IComparable<T>
    {
        public int ID
        {
            get; set; 
        }
        public List<CaricNode<T>> Nodes
        {
            get; set;
        } = null!;

        public CaricNode<T>? Root
        {
            get; private set;
        } = null!;

        public int Count => Nodes.Count();

        public bool IsReadOnly => false;

        public CaricTree(int id,T rootData)
        {
            Root = new CaricNode<T>(id,rootData);
            Nodes = new List<CaricNode<T>>
            {
                Root
            };
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Tree: ");
            foreach (var node in Nodes)
            {
                stringBuilder.Append(node.ToString());
                stringBuilder.Append(' ');
            }

            return stringBuilder.ToString();
        }

        public void Clear()
        {
            Nodes.Clear();
            Root = null;
        }

        public void Add(CaricNode<T> item)
        {
            ((ICollection<CaricNode<T>>)Nodes).Add(item);
        }

        public bool Contains(CaricNode<T> item)
        {
            return ((ICollection<CaricNode<T>>)Nodes).Contains(item);
        }

        public void CopyTo(CaricNode<T>[] array, int arrayIndex)
        {
            ((ICollection<CaricNode<T>>)Nodes).CopyTo(array, arrayIndex);
        }

        public bool Remove(CaricNode<T> item)
        {
            return ((ICollection<CaricNode<T>>)Nodes).Remove(item);
        }

        public IEnumerator<CaricNode<T>> GetEnumerator()
        {
            return ((IEnumerable<CaricNode<T>>)Nodes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Nodes).GetEnumerator();
        }

        public void AddNode(CaricNode<T> parent, CaricNode<T> child)
        {
            this.Add(child);
            parent.AddChild(child);
        }
    }

    public class CaricNode<T> : IComparable<CaricNode<T>>
    {
        public int ID
        {
            get; set; 
        }
        public SortedList<int, CaricNode<T>> Children;
        public T Data
        {
            get; set;
        }
        public CaricNode(int id, T data)
        {
            ID = id;
            Data = data;
            Children = new SortedList<int, CaricNode<T>>();
        }
        public void AddChild(CaricNode<T> child)
        {
            Children.Add(child.ID, child);
        }
        public void RemoveChild(CaricNode<T> child)
        {
            Children.Remove(child.ID);
        }
        public override string ToString()
        {
            return Data.ToString();
        }

        public int CompareTo(CaricNode<T> other)
        {
            if (Data is IComparable<T> comparableData)
            {
                return comparableData.CompareTo(other.Data);
            }

            throw new ArgumentException("Data type T must implement IComparable<T>");
        }
    }
}
