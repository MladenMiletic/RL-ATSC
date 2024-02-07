namespace Quest.ML.Clustering.Neural
{
    public class Node
    {
        private List<Node> connections;

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

        public Node()
        {
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
    }
}
