using Quest.ML.Clustering.Neural;
using Quest.ML.ReinforcementLearning;
using Quest.ML.ReinforcementLearning.Interfaces;
using Quest.ML.ReinforcementLearning.SelectionPolicies;
using VissimEnv;

namespace MOBDOK
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Load network properly
            VissimEnvironment env = new VissimEnvironment();

            env.LoadNetwork("\"C:\\Users\\mmiletic\\Desktop\\Mobility\\model3by3.inpx\"");

            Console.WriteLine("Load ok!");
            env.UseMaxSpeed();
            Console.WriteLine("Max speed on!");

            int numberOfRouteDecisions = 36;
            int[] vehicleRouteRequests = new int[numberOfRouteDecisions];
            int initTime = 900;
            env.InitializeSimulation(initTime);
            int simRes = env.GetSimulationResolution();
            int simDuration = env.GetSimulationDuration();
            int controlTimeStep = 3000;

            for (int i = 0; i < (simDuration * simRes) - initTime; i++)
            {
                env.RunSimulationStep();
                if (i == simDuration - initTime - 1)
                {
                    break;
                }
                if (i % controlTimeStep != 0)
                {
                    continue;
                }
                env.GetVehicleRouteRequests(numberOfRouteDecisions);
                int[] requests = env.GetVehicleRouteRequests(numberOfRouteDecisions);
                PrintVehicleRouteRequests(requests);

            }
            env.RunSimulationStep();

            
            Console.ReadKey();

        }

        private static void PrintVehicleRouteRequests(int[] vehicleRequests)
        {
            foreach (int vehicleRequest in vehicleRequests)
            {
                Console.Write(vehicleRequest + " ");
            }
            Console.WriteLine();
        }
    }
}
