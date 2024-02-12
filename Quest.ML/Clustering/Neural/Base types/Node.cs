namespace Quest.ML.Clustering.Neural
{
    public class Node : IEquatable<Node>
    {
        private List<Node> connections;
        private int id;

        public int ID
        {
            get
            {
                return id; 
            }
            set
            {
                id = value; 
            }
        }

        public List<Node> Connections
        {
            get
            {
                return connections; 
            }
            private set
            {
                connections = value; 
            }
        }

        public Node(int id)
        {
            ID = id;
            connections = new List<Node>();
        }

        public void AddConnection(Node node)
        {
            if (!connections.Contains(node))
            {
                Connections.Add(node);
            } 
        }
        public bool RemoveConnection(Node node)
        {
            return Connections.Remove(node);
        }

        public bool Equals(Node? other)
        {
            if (other == null)
            {
                return false;
            }
            else
            {
                return other.ID == id;
            }
            
        }
    }
}
