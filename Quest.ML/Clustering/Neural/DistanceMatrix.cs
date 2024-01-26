using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class DistanceMatrix
    {
        private SortedDictionary<int, SortedDictionary<int, int?>> matrix;
        public SortedDictionary<int, SortedDictionary<int, int?>> Matrix
        {
            get
            {
                return matrix;
            }
            private set
            {
                matrix = value;
            }
        }

        

        private SortedDictionary<int, int> reached;

        public SortedDictionary<int, int> Reached
        {
            get
            {
                return reached;
            }
            private set
            {
                reached = value;
            }
        }


        public DistanceMatrix()
        {
            matrix = new SortedDictionary<int, SortedDictionary<int, int?>>();
            reached = new SortedDictionary<int, int>();
        }

        public void ResetReached()
        {
            reached = new SortedDictionary<int, int>();
            foreach (var key in matrix.Keys)
            {
                reached.Add(key, 0);
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var row in Matrix)
            {
                foreach (var col in row.Value)
                {
                    stringBuilder.Append(col.Value ?? -1);
                    stringBuilder.Append(' ');
                }
                stringBuilder.Append('\n');
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// This hapens only when new neuron is added without edges, no path is represented by null?
        /// </summary>
        /// <param name="id">id of new neuron</param>
        public void AddRow(int id)
        {
            SortedDictionary<int, int?> newRow = new SortedDictionary<int, int?>();
            foreach (var row in Matrix)
            {
                row.Value.Add(id, null);
                newRow.Add(row.Key, null);
            }
            newRow.Add(id, 0); //distance zero from oneself
            Matrix.Add(id, newRow);
        }

        /// <summary>
        /// This happens only when new neuron is added and connected to another with an edge
        /// </summary>
        /// <param name="id"></param>
        /// <param name="referenceRow"></param>
        public void AddRow(int id, int referenceRowId)
        {
            SortedDictionary<int, int?> newRow = new SortedDictionary<int, int?>();
            SortedDictionary<int, int?> referenceRow = Matrix[referenceRowId];

            foreach (var col in referenceRow)
            {
                newRow.Add(col.Key, col.Value + 1);
            }

            newRow.Add(id, 0);

            foreach (var row in Matrix)
            {
                row.Value.Add(id, newRow[row.Key]);
            }

            Matrix.Add(id, newRow);
        }

        public void RemoveRow(int id)
        {
            foreach (var row in Matrix)
            {
                row.Value.Remove(id);
            }
            Matrix.Remove(id);
        }

        //TODO: Tested this, works ok if source and destination exists
        public int? GetDistance(int idSource, int idDestination)
        {
            return Matrix[idSource][idDestination];
        }
        public bool IsConnected(int idSource)
        {
            foreach (var row in Matrix[idSource])
            {
                if (row.Value != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Updates the distance matrix after a new edge is added. Before runing the methods that reset reached should be run.
        /// </summary>
        /// <param name="source">ID of source vertex</param>
        /// <param name="destination">ID of distance vertex</param>
        /// <param name="level">Current level should be set to 1 initially</param>
        public void UpdateAfterEdgeAdded(int source, int destination, int? level=1)
        {
            //this row is already being updated recursively, this is used to speed up the computation and prevent stackoverflow exception
            //if (reached[source] == 1)
            //{
            //    return;
            //}
            //there is already a calculated link between source and destination
            if (matrix[source][destination] == level && matrix[destination][source] == level)
            {
                return;
            }
            //nothing needs to be done in this case, should not happen in actual scenarios but is here for testing purposes
            if (source == destination) 
            {
                return;
            }
            foreach (var key in Matrix.Keys)
            {
                reached[source] = 1;
                if ((Matrix[source][key] ?? int.MaxValue) > (Matrix[destination][key] + level ?? int.MaxValue))
                {
                    Console.WriteLine($"CHANGE: [{source},{key}] = [{Matrix[destination][key]} + {level} = {Matrix[destination][key] + level}]");
                    Console.ReadKey();
                    matrix[source][key] = Matrix[destination][key] + level;
                    UpdateAfterEdgeAdded(key, source, Matrix[destination][key] + level);
                }
            }
        }
        public void UpdateAfterEdgeAddedNew(int source, int destination, int? level = 1)
        {
            //this row is already being updated recursively, this is used to speed up the computation and prevent stackoverflow exception
            //if (reached[source] == 1)
            //{
            //    return;
            //}
            //there is already a calculated link between source and destination
            if (matrix[source][destination] == level && matrix[destination][source] == level)
            {
                return;
            }
            //nothing needs to be done in this case, should not happen in actual scenarios but is here for testing purposes
            if (source == destination)
            {
                return;
            }
            foreach (var key in Matrix.Keys)
            {
                reached[source] = 1;
                //Matrix[destination][source] = level;
                Matrix[source][destination] = level;
                if ((Matrix[source][key] ?? int.MaxValue) > (Matrix[destination][key] + level ?? int.MaxValue))
                {
                    Console.WriteLine($"CHANGE: [{source},{key}] = [{Matrix[destination][key]} + {level} = {Matrix[destination][key] + level}]");
                    Console.ReadKey();
                    matrix[source][key] = Matrix[destination][key] + level;
                    
                    UpdateAfterEdgeAddedNew(key, source, Matrix[destination][key] + level);

                }
                

            }
        }
    }
}
