using Quest.ML.Clustering.Neural;
namespace Quest.ML.UnitTests
{
    public class GraphTests
    {
        [Fact]
        public void Add_NewNode_NodeAdded()
        {
            //Arrange
            Graph<Node,GraphEdge<Node>> graph = new Graph<Node,GraphEdge<Node>>();
            Node newNode = new Node(1);
            //Act
            graph.Add(newNode);
            //Assert
            Assert.Contains(newNode, graph);
            Assert.Single(graph);
            Assert.Empty(newNode.Connections);
        }

        [Fact]
        public void AddEdge_NodesNotINGraph_ThrowsException()
        {
            //Arrange
            Graph<Node, GraphEdge<Node>> graph = new Graph<Node, GraphEdge<Node>>();
            Node newNode1 = new Node(1);
            Node newNode2 = new Node(2);
            graph.Add(newNode1);

            //Act
            Action edgeAdditionAttempt = () => graph.AddEdge(newNode1, newNode2);
            //Assert
            Assert.Throws<Exception>(edgeAdditionAttempt);
        }

        [Fact]
        public void AddEdge_NodesInGraph_EdgeAdded()
        {
            //Arrange
            Graph<Node, GraphEdge<Node>> graph = new Graph<Node, GraphEdge<Node>>();
            Node newNode1 = new Node(1);
            Node newNode2 = new Node(2);
            graph.Add(newNode1);
            graph.Add(newNode2);

            //Act
            GraphEdge<Node> edge = graph.AddEdge(newNode1, newNode2);

            //Assert
            Assert.Contains(edge, graph.Edges);
            Assert.Single(graph.Edges);
            Assert.Contains(newNode1, newNode2.Connections);
            Assert.Contains(newNode2, newNode1.Connections);
        }

        [Fact]
        public void RemoveEdge_EdgeExists_EdgeRemoved()
        {
            //Arrange
            Graph<Node, GraphEdge<Node>> graph = new Graph<Node, GraphEdge<Node>>();
            Node newNode1 = new Node(1);
            Node newNode2 = new Node(2);
            graph.Add(newNode1);
            graph.Add(newNode2);
            GraphEdge<Node> edge = graph.AddEdge(newNode1, newNode2);

            //Act
            graph.RemoveEdge(newNode1, newNode2);

            //Assert

            Assert.DoesNotContain(edge, graph.Edges);
            Assert.DoesNotContain(newNode2, newNode1.Connections);
            Assert.DoesNotContain(newNode1, newNode2.Connections);
        }

        [Fact]
        public void RemoveEdge_EdgeDoesNotExist_ThrowsException()
        {
            //Arrange
            Graph<Node, GraphEdge<Node>> graph = new Graph<Node, GraphEdge<Node>>();
            Node newNode1 = new Node(1);
            Node newNode2 = new Node(2);
            graph.Add(newNode1);
            graph.Add(newNode2);

            //Act
            Action edgeRemovalAttempt = () => graph.RemoveEdge(newNode1, newNode2);

            //Assert
            Assert.Throws<Exception>(edgeRemovalAttempt);
            
        }

        [Fact]
        public void Remove_NodeExitsNotConnected_NodeRemoved()
        {
            //Arrange
            Graph<Node, GraphEdge<Node>> graph = new Graph<Node, GraphEdge<Node>>();
            Node newNode1 = new Node(1);
            Node newNode2 = new Node(2);
            graph.Add(newNode1);
            graph.Add(newNode2);

            //Act
            graph.Remove(newNode1);

            //Assert
            Assert.DoesNotContain(newNode1, graph);
        }

        [Fact]
        public void Remove_NodeExitsConnected_NodeRemoved()
        {
            //Arrange
            Graph<Node, GraphEdge<Node>> graph = new Graph<Node, GraphEdge<Node>>();
            Node newNode1 = new Node(1);
            Node newNode2 = new Node(2);
            graph.Add(newNode1);
            graph.Add(newNode2);
            var edge = graph.AddEdge(newNode1, newNode2);

            //Act
            graph.Remove(newNode1);

            //Assert
            Assert.DoesNotContain(newNode1, graph);
            Assert.DoesNotContain(edge, graph.Edges);
            Assert.DoesNotContain(newNode1, newNode2.Connections);
        }

        [Fact]
        public void Remove_NodeDoesNotExist_ThrowsException()
        {
            //Arrange
            Graph<Node, GraphEdge<Node>> graph = new Graph<Node, GraphEdge<Node>>();
            Node newNode1 = new Node(1);

            //Act
            Action NodeRemovalAttempt = () => graph.Remove(newNode1);

            //Assert

            Assert.Throws<Exception>(NodeRemovalAttempt);
        }
    }
}