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
        private List<ILink> links = [];
        private ISignalControllerContainer signalControllers;

        private double[] cavsScalingFactor = [];
        private double[] maxCavsPerLane = [];
        private readonly object[] Attributes_veh = ["No", "VehType", "Pos", "Clear", "InQueue"];

        private List<ILink> NorthLinks = [];
        private List<ILink> EastLinks = [];
        private List<ILink> SouthLinks = [];
        private List<ILink> WestLinks = [];

        private List<IVehicle> NorthVehicles = [];
        private List<IVehicle> EastVehicles = [];
        private List<IVehicle> SouthVehicles = [];
        private List<IVehicle> WestVehicles = [];


        public VissimEnvironment()
        {
            simulator = new Vissim();
            simulator.set_AttValue("MaxNumberOfCOMRefs", 0);
        }

        public void LoadNetwork(string path)
        {
            simulator.LoadNet(path);
            //get lanes
            foreach (ILink Link in simulator.Net.Links)
            {
                links.Add(Link);
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
            signalControllers = simulator.Net.SignalControllers;
            PrintLinkIDs();
        }
        public void PrintLinkIDs()
        {
            foreach (ILink link in links)
            {
                int id = link.AttValue["No"];
                Console.WriteLine(id);
                if (id == 5 || id == 10006 || id == 10007 || id == 7 || id == 8)
                {
                    WestLinks.Add(link);
                }
                else if (id == 9 || id == 10 || id == 11 || id == 13)
                {
                    NorthLinks.Add(link);
                }
                else if (id == 1 || id == 10000 || id == 4 || id == 2)
                {
                    EastLinks.Add(link);
                }
                else if (id == 14 || id == 16)
                {
                    SouthLinks.Add(link);
                }
            }
        }
        public void GetVehicles()
        {
            NorthVehicles.Clear();
            EastVehicles.Clear();
            SouthVehicles.Clear();
            WestVehicles.Clear();
            foreach (ILink link in NorthLinks)
            {
                IIterator NorthCVsIterator = link.Vehs.GetFilteredSet("[VehType] = \"630\"").Iterator;

                while (NorthCVsIterator.Valid)
                {
                    NorthVehicles.Add(NorthCVsIterator.Item);
                    NorthCVsIterator.Next();
                }
            }
            foreach (ILink link in EastLinks)
            {
                IIterator EastCVsIterator = link.Vehs.GetFilteredSet("[VehType] = \"630\"").Iterator;

                while (EastCVsIterator.Valid)
                {
                    EastVehicles.Add(EastCVsIterator.Item);
                    EastCVsIterator.Next();
                }
            }
            foreach (ILink link in SouthLinks)
            {
                IIterator SouthCVsIterator = link.Vehs.GetFilteredSet("[VehType] = \"630\"").Iterator;

                while (SouthCVsIterator.Valid)
                {
                    SouthVehicles.Add(SouthCVsIterator.Item);
                    SouthCVsIterator.Next();
                }
            }
            foreach (ILink link in WestLinks)
            {
                IIterator WestCVsIterator = link.Vehs.GetFilteredSet("[VehType] = \"630\"").Iterator;

                while (WestCVsIterator.Valid)
                {
                    WestVehicles.Add(WestCVsIterator.Item);
                    WestCVsIterator.Next();
                }
            }
        }

        public void RunSimulationStep()
        {
            simulator.Simulation.RunSingleStep();
        }
        public void RunSimulationContinuous()
        {
            simulator.Simulation.RunContinuous();
        }
        public void RunSimulationContinuousWithBreak(int breakTime)
        {
            simulator.Simulation.set_AttValue("SimBreakAt", breakTime);
            RunSimulationContinuous();
        }

        public void UseMaxSpeed()
        {
            simulator.SuspendUpdateGUI();
            simulator.Simulation.set_AttValue("UseMaxSimSpeed", true);
            simulator.Graphics.CurrentNetworkWindow.set_AttValue("QuickMode", 1);
        }
        public double[] GetAverageNodeQueue()
        {
            double[] finalQueues = new double[36];
            int finalCounter = 0;
            foreach (INode node in simulator.Net.Nodes)
            {
                double[] nodequeues = new double[13];
                int i = 0;
                foreach (IMovement movement in node.Movements)
                {
                    nodequeues[i] = movement.get_AttValue("Qlen(Current,Last)");
                    i++;
                }
                int j = 0;
                double sum = 0;
                foreach (double q in nodequeues)
                {
                    sum += q;
                    j++;
                    if (j % 3 == 0)
                    {
                        finalQueues[finalCounter] = sum / 3.0;
                        finalCounter++;
                        sum = 0;
                        j = 0;
                    }
                }
            }
            return finalQueues;
        }
        public double[] GetDelaysFromNodes()
        {
            double[] delays = new double[9];
            int i = 0;
            foreach (INode node in simulator.Net.Nodes)
            {
                IMovement movement = node.TotRes;
                double? x = movement.get_AttValue("VehDelay(Current,Last,All)");
                if (x == null)
                    x = 0;
                delays[i] = (double)x;
                i++;
            }
            return delays;
        }
        public double[] GetVehsFromNodes()
        {
            double[] delays = new double[9];
            int i = 0;
            foreach (INode node in simulator.Net.Nodes)
            {
                IMovement movement = node.TotRes;
                double? x = movement.get_AttValue("Vehs(Current,Last,All)");
                if (x == null)
                    x = 0;
                delays[i] = (double)x;
                i++;
            }
            return delays;
        }
        public double[] GetTotalDelaysFromNodes()
        {
            double[] delays = new double[9];
            int i = 0;
            foreach (INode node in simulator.Net.Nodes)
            {
                IMovement movement = node.TotRes;
                double? x = movement.get_AttValue("VehDelay(Current,Total,All)");
                if (x == null)
                    x = 0;
                delays[i] = (double)x;
                i++;
            }
            return delays;
        }

        public double[] GetVehicleRouteRequests(int numberOfStaticRouteDecisions)
        {
            double[] vehicleRouteRequests = new double[numberOfStaticRouteDecisions];

            IVehicleContainer vehicles = simulator.Net.Vehicles;

            foreach (IVehicle vehicle in vehicles)
            {
                if (vehicle == null)
                {
                    Console.WriteLine("Vehicle null!");
                    continue;
                }
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
            return (double)simulator.Net.VehicleNetworkPerformanceMeasurement.get_AttValue("DelayAvg(Current, Total, All)");
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
            RunSimulationContinuousWithBreak(initTime);
        }

        public void PerformAction(int agentID, int actionId)
        {
            ISignalController controller = signalControllers.get_ItemByKey(agentID+1);
            
            ISignalGroup sg1 = controller.SGs.GetAll()[0];
            ISignalGroup sg2 = controller.SGs.GetAll()[1];
            switch (actionId)
            {
                case 0:
                    sg1.set_AttValue("EndGreen", 26);
                    sg2.set_AttValue("EndRed", 30);
                    break;
                case 1:
                    sg1.set_AttValue("EndGreen", 21);
                    sg2.set_AttValue("EndRed", 25);
                    break;
                case 2:
                    sg1.set_AttValue("EndGreen", 16);
                    sg2.set_AttValue("EndRed", 20);
                    break;
                case 3:
                    sg1.set_AttValue("EndGreen", 31);
                    sg2.set_AttValue("EndRed", 35);
                    break;
                case 4:
                    sg1.set_AttValue("EndGreen", 36);
                    sg2.set_AttValue("EndRed", 40);
                    break;
            }  
        }

        public int GetSimulationDuration()
        {
            return simulator.Simulation.get_AttValue("SimPeriod");
        }

        public int GetSimulationResolution()
        {
            return simulator.Simulation.get_AttValue("SimRes");
        }

        public void GetCVData()
        {
            throw new NotImplementedException();
        }
    }
}
