using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class CaricNetwork<T> : ICollection<CaricTree<T>> where T : IComparable<T>
    {
        public List<CaricTree<T>> Trees
        {
            get;
            private set;
        } = null!;

        public CaricNetwork()
        {
            Trees = new List<CaricTree<T>>();
        }

        public void AddUnconnetedNode(T newNode)
        {
            CaricTree<T> newTree = new CaricTree<T>(Trees.Count(), newNode);
            Trees.Add(newTree);
        }
        public void AddConnectedNode(int oldNodeId,T oldNode, T newNode)
        {
            CaricTree<T> newTree = new CaricTree<T>(Trees.Count, newNode);
            Trees.Add(newTree);

            //Check all old trees for oldNode if there is one add newNode as child to it

            //Select all oldNodes in all trees

            var TreesWithOldNode = (from tree in Trees
                                    where tree.Any(node => node.ID == oldNodeId)
                                   //where tree.Contains(new CaricNode<T>(oldNodeId, oldNode))
                                   select tree).ToList();
            foreach (var tree in TreesWithOldNode)
            {
                tree.AddNode(oldNodeId, new CaricNode<T>(newTree.ID, newNode));
            }
            CopyTreeNodes(Trees[oldNodeId].Root, newTree, newTree.Root);

        }

        private void CopyTreeNodes(CaricNode<T> sourceNode, CaricTree<T> destinationTree, CaricNode<T> destinationNode)
        {
            CaricNode<T>? sourceNodeInDestination = (from node in destinationTree.Nodes
                                                   where sourceNode.CompareTo(node) == 0
                                                   select node).FirstOrDefault();
            if (sourceNodeInDestination == null || sourceNodeInDestination.Level > destinationNode.Level + 1)
            {
                var addedNode = destinationTree.AddNode(destinationNode, new CaricNode<T>(sourceNode.ID, sourceNode.Data));
                foreach (var child in sourceNode.Children)
                {
                    CopyTreeNodes(child.Value, destinationTree, addedNode);
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Entire Network: \n");
            foreach (var item in Trees)
            {
                sb.Append(item.ToString());
                sb.Append('\n');
            }
            return sb.ToString();
        }
        public int Count => Trees.Count;

        public bool IsReadOnly => false;

        public void Add(CaricTree<T> item)
        {
            Trees.Add(item);
        }

        public void Clear()
        {
            Trees.Clear();
        }

        public bool Contains(CaricTree<T> item)
        {
            return Trees.Contains(item);
        }

        public void CopyTo(CaricTree<T>[] array, int arrayIndex)
        {
            Trees.CopyTo(array, arrayIndex);
        }

        public IEnumerator<CaricTree<T>> GetEnumerator()
        {
            return Trees.GetEnumerator();
        }

        public bool Remove(CaricTree<T> item)
        {
            return Trees.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Trees).GetEnumerator();
        }
    }
}
