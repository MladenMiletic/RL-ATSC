using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quest.ML.Clustering.Neural
{
    public class Edge
    {
        private int id;
        private GasNeuron source;
        private GasNeuron destination;
        private int age;
        
        public int ID
        {
            get
            {
                return id;
            }
            private set
            {
                id = value;
            }
        }
        public GasNeuron Source
        {
            get
            {
                return source;
            }
            private set
            {
                source = value;
            }
        }
        public GasNeuron Destination
        {
            get
            {
                return destination;
            }
            private set
            {
                destination = value;
            }
        }
        public int Age
        {
            get
            {
                return age;
            }
            private set
            {
                age = value;
            }
        }

        public Edge(int id, GasNeuron source, GasNeuron destination)
        {
            ID = id;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Destination = destination ?? throw new ArgumentNullException(nameof(destination));
            age = 0;
        }

        public void ResetAge()
        {
            Age = 0;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Edge edge = obj as Edge ?? throw new ArgumentNullException(nameof(obj));

            return (Source.Equals(edge.Source) && Destination.Equals(edge.Destination)) ||
                   (Source.Equals(edge.Destination) && Destination.Equals(edge.Source));
        }

        public override int GetHashCode()
        {
            return Source.GetHashCode() ^ Destination.GetHashCode();
        }

        public override string? ToString()
        {
            return $"{Source.ID} - {Destination.ID}";
        }
    }
}
