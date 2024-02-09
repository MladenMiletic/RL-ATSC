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
            GasNeuron gasNeuron = new GasNeuron(1);
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
        #endregion
    }
}
