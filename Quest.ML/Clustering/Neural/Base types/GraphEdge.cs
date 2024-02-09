namespace Quest.ML.Clustering.Neural
{
    public class GraphEdge<T>(T source, T target) :IEquatable<GraphEdge<T>>
    {
        private T source = source ?? throw new ArgumentNullException(nameof(source));
        private T target = target ?? throw new ArgumentNullException(nameof(target));

        public T Source
        {
            get
            {
                return source; 
            }
            protected set
            {
                source = value; 
            }
        }
        public T Target
        {
            get
            {
                return target;
            }
            protected set
            {
                target = value;
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            GraphEdge<T> edge = obj as GraphEdge<T> ?? throw new ArgumentNullException(nameof(obj));
            if (Source == null || Target == null)
            {
                throw new ArgumentNullException("Target and Source cant be null");
            }
            return (Source.Equals(edge.Source) && Target.Equals(edge.Target)) ||
                   (Source.Equals(edge.Target) && Target.Equals(edge.Source));
        }

        public bool Equals(GraphEdge<T>? other)
        {
            if (other == null || GetType() != other.GetType())
            {
                return false;
            }
            
            if (Source == null || Target == null)
            {
                throw new ArgumentNullException("Target and Source cant be null");
            }
            return (Source.Equals(other.Source) && Target.Equals(other.Target)) ||
                   (Source.Equals(other.Target) && Target.Equals(other.Source));
        }

        public override int GetHashCode()
        {
            if (Source == null || Target == null)
            {
                throw new ArgumentNullException("Target and Source cant be null");
            }
            return Source.GetHashCode() ^ Target.GetHashCode();
        }
    }
}
