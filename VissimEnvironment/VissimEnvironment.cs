using VISSIMLIB;

namespace VissimEnv
{
    /// <summary>
    /// The purpose of this class is to wrap Vissim simulator in a simple way to make it easier to
    /// work with. Functionalities will be addded as required.
    /// </summary>
    public class VissimEnvironment
    {
        private readonly IVissim simulator;
        private List<ILane> lanes = [];

        private double[] cavsScalingFactor = [];
        private double[] maxCavsPerLane = [];
        private readonly object[] Attributes_veh = ["No", "VehType", "VehRoutSta"];

        public VissimEnvironment()
        {
            simulator = new Vissim();
        }

        public void LoadNetwork(string path)
        {
            simulator.LoadNet(path);
            //get lanes
            foreach (ILink Link in simulator.Net.Links)
            {
                if (!Convert.ToBoolean(Link.get_AttValue("IsConn")))
                {
                    foreach (ILane Lane in Link.Lanes)
                    {
                        lanes.Add(Lane);
                    }
                }

            }
            cavsScalingFactor = new double[lanes.Count];
            maxCavsPerLane = new double[lanes.Count];
        }

        public void RunSimulationStep()
        {
            simulator.Simulation.RunSingleStep();
        }

        public void UseMaxSpeed()
        {
            simulator.SuspendUpdateGUI();
            simulator.Simulation.set_AttValue("UseMaxSimSpeed", true);
            simulator.Graphics.CurrentNetworkWindow.set_AttValue("QuickMode", 1);
        }


        public int[] GetVehicleRouteRequests(int numberOfStaticRouteDecisions)
        {
            int[] vehicleRouteRequests = new int[numberOfStaticRouteDecisions];

            IVehicleContainer vehicles = simulator.Net.Vehicles;

            foreach (IVehicle vehicle in vehicles)
            {
                //var x = vehicle.VehRoutSta.get_AttValue("No");
                string y = vehicle.get_AttValue("VehRoutSta");
                if (y == null)
                    continue;
                int index = Convert.ToInt32(y.Remove(y.Length - 2));
                vehicleRouteRequests[index-1]++;

                
            }
            return vehicleRouteRequests;
        }

        public double[] GetLaneStateInfo()
        {
            double[] cavsPerLane = new double[lanes.Count];
            Parallel.For(0, lanes.Count, i =>
            {
                cavsPerLane[i] = 0;
                object[,] all_veh_attributes = (object[,])lanes[i].Vehs.GetMultipleAttributes(Attributes_veh);
                for (int j = 0; j < all_veh_attributes.GetLength(0); j++)
                {
                    if (Convert.ToDouble(all_veh_attributes[j, 1]) == 640)
                    {
                        cavsPerLane[i]++;
                    }
                }
                if (cavsPerLane[i] > maxCavsPerLane[i])
                {
                    maxCavsPerLane[i] = cavsPerLane[i];
                    cavsScalingFactor[i] = 1.0 / cavsPerLane[i];
                }
                cavsPerLane[i] = cavsPerLane[i] * cavsScalingFactor[i];
            });
            return cavsPerLane;
        }
        public double GetTotalTravelTime()
        {
            return (double)simulator.Net.VehicleNetworkPerformanceMeasurement.get_AttValue("TravTmTot(Current, Total, All)");
        }
        public double GetAverageDelay()
        {
            return (double)simulator.Net.VehicleNetworkPerformanceMeasurement.get_AttValue("DelayAvg(Current, Average, All)");
        }
        public int GetTotalNumberOfStops()
        {
            return (int)simulator.Net.VehicleNetworkPerformanceMeasurement.get_AttValue("StopsTot(Current, Total, All");
        }
        public double GetAverageDelayOfLastTimeStep()
        {
            return (double)simulator.Net.VehicleNetworkPerformanceMeasurement.get_AttValue("DelayAvg(Current, Last, All)");
        }

        public void InitializeSimulation(int initTime)
        {
            for (int i = 0; i < initTime; i++)
            {
                RunSimulationStep();
            }
        }

        public void PerformAction(int actionId)
        {
            throw new NotImplementedException();
        }

        public int GetSimulationDuration()
        {
            return simulator.Simulation.get_AttValue("SimPeriod");
        }

        public int GetSimulationResolution()
        {
            return simulator.Simulation.get_AttValue("SimRes");
        }
    }
}
