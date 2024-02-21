namespace VissimEnv
{
    /// <summary>
    /// The purpose of this class is to wrap Vissim simulator in a simple way to make it easier to
    /// work with. Functionalities will be addded as required.
    /// </summary>
    public class VissimEnvironment
    {
        private IVissim simulator;

        public VissimEnvironment()
        {
            simulator = new Vissim();
        }

        public void LoadNetwork(string path)
        {
            simulator.LoadNet(path);
        }

        public void RunSimulationStep()
        {
            simulator.Simulation.RunSingleStep();
        }
        
    }
}
