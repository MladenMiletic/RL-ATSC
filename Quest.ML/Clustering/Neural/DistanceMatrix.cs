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

        public DistanceMatrix()
        {
            matrix = new SortedDictionary<int, SortedDictionary<int, int?>>();   
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

        public void UpdateAfterEdgeAdded(int source, int destination, int level)
        {
            matrix[source][destination] = level;
            matrix[destination][source] = level;
            foreach (var col in Matrix[source])
            {
                int? sourceValue = Matrix[source][col.Key];
                int? destinationValue = Matrix[destination][col.Key] + level;
                if ((sourceValue ?? int.MaxValue) > (destinationValue ?? int.MaxValue))
                {
                    UpdateAfterEdgeAdded(col.Key, source, (int)sourceValue);
                }
            }
        }
    }
}
