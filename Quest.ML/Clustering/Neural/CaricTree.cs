using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Quest.ML.Clustering.Neural
{
    public class CaricTree<T> : ICollection<CaricNode<T>> where T : IComparable<T>
    {
        public int ID
        {
            get => Root.ID;
        }
        public List<CaricNode<T>> Nodes
        {
            get; private set;
        } = null!;

        public CaricNode<T> Root
        {
            get; private set;
        }

        public int Count => Nodes.Count();

        public bool IsReadOnly => false;

        public CaricTree(int id,T rootData)
        {
            Root = new CaricNode<T>(id,rootData, 0);
            Nodes = new List<CaricNode<T>>
            {
                Root
            };
        }

        

        public override string ToString()
        {
            return $"Tree with ID {ID} contains {Count} elements\n" + Root!.PrintAllChildren();
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

        public CaricNode<T> AddNode(CaricNode<T> parent, CaricNode<T> child)
        {
            child.Level = parent.Level + 1;
            this.Add(child);
            parent.AddChild(child);
            return child;
        }

        public CaricNode<T> AddNode(int idParent, CaricNode<T> child)
        {
            this.Add(child);
            CaricNode<T>? parent = (from node in Nodes
                         where node.ID == idParent
                         select node).FirstOrDefault();
            child.Level = parent.Level + 1;
            parent.AddChild(child);
            return child;
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
        public int Level
        {
            get;
            set;
        }
        public CaricNode(int id, T data, int level = 0)
        {
            ID = id;
            Data = data;
            Level = level;
            Children = new SortedList<int, CaricNode<T>>();
        }
        public void AddChild(CaricNode<T> child)
        {
            Children.Add(child.ID, child);
            child.Level = this.Level + 1;
        }
        public void RemoveChild(CaricNode<T> child)
        {
            Children.Remove(child.ID);
        }

        public string PrintAllChildren(string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(indent);
            sb.Append(" =");
            indent += "  ";
            sb.AppendLine(this.ToString());

            foreach (var child in Children.Values)
            {
                sb.Append(child.PrintAllChildren(indent));
            }

            return sb.ToString();
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
