using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quest.ML.Clustering.Neural;

namespace Quest.ML.UnitTests
{
    public class GasNetworkTests
    {
        #region GasNeuronTests
        [Theory]
        [InlineData(new double[] { 0 }, new double[] { 0 }, 0)]
        [InlineData(new double[] { 1 }, new double[] { 1 }, 0)]
        [InlineData(new double[] { 0 }, new double[] { 1 }, 1)]
        [InlineData(new double[] { 1, 1, 1, 1 }, new double[] { 0, 0, 0, 0 }, 2)]
        public void Compute_VariousInputs_CorrectCalculation(double[] inputs, double[] weights, double expectedResult)
        {
            //Arrange
            GasNeuron gasNeuron = new GasNeuron(weights.Length);
            gasNeuron.SetWeights(weights);

            //Act
            double result = gasNeuron.Compute(inputs);
        
            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Compute_SizeOfInputsWrong_ThrowsException()
        {
            //Arrange
            GasNeuron gasNeuron = new GasNeuron(1);
            gasNeuron.SetWeights(new double[] {0} );
            double[] inputs = new double[] { 0, 0, };
            //Act
            Action AttemptComputation = () => gasNeuron.Compute(inputs);
            //Assert
            Assert.Throws<ArgumentException>(AttemptComputation);
        }


        [Fact]
        public void RandomizeWeights_Success()
        {
            //Arrange
            GasNeuron neuron = new GasNeuron(1);
            //Act
            neuron.RandomizeWeights();
            //Assert
            Assert.Single(neuron.Weights);
            
        }
        #endregion

        #region GasNetworkTests

        [Fact]
        public void InitializeNetwork_Success()
        {
            //Arrange
            GasNetwork net = new GasNetwork(1, 2, 10, 1, 1, 1);

            //Act
            net.InitializeNetwork();

            //Assert
            Assert.Equal(2, net.Count);
            Assert.Single(net.Edges);
            Assert.Equal(1, net[0].ID);
            Assert.Equal(2, net[1].ID);
            Assert.Contains(new GasEdge(net[0], net[1]), net.Edges);
        }

        [Theory]
        [InlineData(new double[] {0})]
        [InlineData(new double[] { 0, 1})]
        [InlineData(new double[] { 0, 1, 2, 3})]
        public void Compute_ProperInput_Success(double[] input)
        {
            //Arrange
            GasNetwork net = new GasNetwork(1, input.Length, 10, 1, 1, 1);
            net.InitializeNetwork();

            //Act
            net.Compute(input);

            //Assert
            Assert.NotEqual(net.BMU1, net.BMU2);

        }
        #endregion
    }
}
