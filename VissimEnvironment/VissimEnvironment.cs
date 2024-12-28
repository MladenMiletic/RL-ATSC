﻿using VISSIMLIB;

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

        private List<CVVehicle> NorthVehicles = [];
        private List<CVVehicle> EastVehicles = [];
        private List<CVVehicle> SouthVehicles = [];
        private List<CVVehicle> WestVehicles = [];

        private int timeStepCounter = 0;

        public double NorthQueueLengthAvg = 0;
        public double EastQueueLengthAvg = 0;
        public double SouthQueueLengthAvg = 0;
        public double WestQueueLengthAvg = 0;

        public double NorthQueueLengthMax = 0;
        public double EastQueueLengthMax = 0;
        public double SouthQueueLengthMax = 0;
        public double WestQueueLengthMax = 0;

        Dictionary<int, double> distanceLookup = new Dictionary<int, double>
        {
                    // West distances
            {5, 285.314},
            {10006, 90.853},
            {10007, 91.525},
            {7, 46.126},
            {8, 17.835},
    
            // East distances
            {1, 323.532},
            {10000, 89.471},
            {4, 58.503},
            {2, 12.923},
    
            // South distances
            {14, 308.144},
            {16, 56.976},
    
            // North distances
            {9, 173.748},
            {10, 48.712},
            {11, 25.35},
            {13, 6.185}
        };


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
        void ProcessLinks(IEnumerable<ILink> links, List<CVVehicle> vehiclesList)
        {
            foreach (ILink link in links)
            {
                int id = link.AttValue["No"];
                if (!distanceLookup.TryGetValue(id, out double distance))
                {
                    // If the id is not found in the dictionary, skip to the next link
                    continue;
                }

                // Create the iterator once per link
                IIterator cvIterator = link.Vehs.GetFilteredSet("[VehType] = \"630\"").Iterator;

                // Iterate through the filtered vehicles
                while (cvIterator.Valid)
                {
                    // Add each vehicle to the list with the precomputed distance
                    vehiclesList.Add(new CVVehicle(cvIterator.Item, distance));
                    cvIterator.Next();
                }
            }
        }
        public void GetVehiclesAndDistances()
        {
            NorthVehicles.Clear();
            EastVehicles.Clear();
            SouthVehicles.Clear();
            WestVehicles.Clear();

            ProcessLinks(WestLinks, WestVehicles);
            ProcessLinks(EastLinks, EastVehicles);
            ProcessLinks(SouthLinks, SouthVehicles);
            ProcessLinks(NorthLinks, NorthVehicles);
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

        public void GetCVDataUMQLS()
        {
            GetVehiclesAndDistances();
            timeStepCounter++;
            double NorthQueueLength = NorthVehicles.Where(v => v.queued == 1).DefaultIfEmpty(new CVVehicle()).Max(v => v.distanceToIntersection);
            double EastQueueLength = EastVehicles.Where(v => v.queued == 1).DefaultIfEmpty(new CVVehicle()).Max(v => v.distanceToIntersection);
            double SouthQueueLength = SouthVehicles.Where(v => v.queued == 1).DefaultIfEmpty(new CVVehicle()).Max(v => v.distanceToIntersection);
            double WestQueueLength = WestVehicles.Where(v => v.queued == 1).DefaultIfEmpty(new CVVehicle()).Max(v => v.distanceToIntersection);

            //THIS IS UMQLS
            NorthQueueLength = NorthVehicles.Count == 0
                            ? NorthQueueLength
                            : NorthQueueLength * (NorthVehicles.Count + 1) / ((double)NorthVehicles.Count);
            EastQueueLength = EastVehicles.Count == 0
                            ? EastQueueLength
                            : EastQueueLength * (EastVehicles.Count + 1) / ((double)EastVehicles.Count);
            SouthQueueLength = SouthVehicles.Count == 0
                            ? SouthQueueLength
                            : SouthQueueLength * (SouthVehicles.Count + 1) / ((double)SouthVehicles.Count);
            WestQueueLength = WestVehicles.Count == 0
                            ? WestQueueLength
                            : WestQueueLength * (WestVehicles.Count + 1) / ((double)WestVehicles.Count);

            NorthQueueLengthAvg = (NorthQueueLengthAvg * (timeStepCounter - 1) + NorthQueueLength) / timeStepCounter;
            EastQueueLengthAvg = (EastQueueLengthAvg * (timeStepCounter - 1) + EastQueueLength) / timeStepCounter;
            SouthQueueLengthAvg = (SouthQueueLengthAvg * (timeStepCounter - 1) + SouthQueueLength) / timeStepCounter;
            WestQueueLengthAvg = (WestQueueLengthAvg * (timeStepCounter - 1) + WestQueueLength) / timeStepCounter;

            NorthQueueLengthMax = Math.Max(NorthQueueLengthMax, NorthQueueLength);
            EastQueueLengthMax = Math.Max(EastQueueLengthMax, EastQueueLength);
            SouthQueueLengthMax = Math.Max(SouthQueueLengthMax, SouthQueueLength);
            WestQueueLengthMax = Math.Max(WestQueueLengthMax, WestQueueLength);
        }
        public void GetCVDataUMQLSSWPC()
        {
            GetVehiclesAndDistances();
            timeStepCounter++;

            double NorthTheory = NorthVehicles.Where(v => v.queued == 0).DefaultIfEmpty(new CVVehicle()).Min(v => v.distanceToIntersection - v.clearence);
            double EastTheory = EastVehicles.Where(v => v.queued == 0).DefaultIfEmpty(new CVVehicle()).Min(v => v.distanceToIntersection - v.clearence);
            double SouthTheory = SouthVehicles.Where(v => v.queued == 0).DefaultIfEmpty(new CVVehicle()).Min(v => v.distanceToIntersection - v.clearence);
            double WestTheory = WestVehicles.Where(v => v.queued == 0).DefaultIfEmpty(new CVVehicle()).Min(v => v.distanceToIntersection - v.clearence);

            //THIS IS SWPC
            double NorthQueueLength = NorthVehicles.Where(v => v.queued == 1 && v.distanceToIntersection <= NorthTheory).DefaultIfEmpty(new CVVehicle()).Max(v => v.distanceToIntersection);
            double EastQueueLength = EastVehicles.Where(v => v.queued == 1 && v.distanceToIntersection <= EastTheory).DefaultIfEmpty(new CVVehicle()).Max(v => v.distanceToIntersection);
            double SouthQueueLength = SouthVehicles.Where(v => v.queued == 1 && v.distanceToIntersection <= SouthTheory).DefaultIfEmpty(new CVVehicle()).Max(v => v.distanceToIntersection);
            double WestQueueLength = WestVehicles.Where(v => v.queued == 1 && v.distanceToIntersection <= WestTheory).DefaultIfEmpty(new CVVehicle()).Max(v => v.distanceToIntersection);

            //THIS IS UMQLS
            NorthQueueLength = NorthVehicles.Count == 0
                            ? NorthQueueLength
                            : NorthQueueLength * (NorthVehicles.Count + 1) / ((double)NorthVehicles.Count);
            EastQueueLength = EastVehicles.Count == 0
                            ? EastQueueLength
                            : EastQueueLength * (EastVehicles.Count + 1) / ((double)EastVehicles.Count);
            SouthQueueLength = SouthVehicles.Count == 0
                            ? SouthQueueLength
                            : SouthQueueLength * (SouthVehicles.Count + 1) / ((double)SouthVehicles.Count);
            WestQueueLength = WestVehicles.Count == 0
                            ? WestQueueLength
                            : WestQueueLength * (WestVehicles.Count + 1) / ((double)WestVehicles.Count);

            NorthQueueLengthAvg = (NorthQueueLengthAvg * (timeStepCounter - 1) + NorthQueueLength) / timeStepCounter;
            EastQueueLengthAvg = (EastQueueLengthAvg * (timeStepCounter - 1) + EastQueueLength) / timeStepCounter;
            SouthQueueLengthAvg = (SouthQueueLengthAvg * (timeStepCounter - 1) + SouthQueueLength) / timeStepCounter;
            WestQueueLengthAvg = (WestQueueLengthAvg * (timeStepCounter - 1) + WestQueueLength) / timeStepCounter;

            NorthQueueLengthMax = Math.Max(NorthQueueLengthMax, NorthQueueLength);
            EastQueueLengthMax = Math.Max(EastQueueLengthMax, EastQueueLength);
            SouthQueueLengthMax = Math.Max(SouthQueueLengthMax, SouthQueueLength);
            WestQueueLengthMax = Math.Max(WestQueueLengthMax, WestQueueLength);
        }
        public void ResetTimeStepCounter()
        {
            timeStepCounter = 0;
            NorthQueueLengthAvg = 0;
            EastQueueLengthAvg = 0;
            SouthQueueLengthAvg = 0;
            WestQueueLengthAvg = 0;

            NorthQueueLengthMax = 0;
            EastQueueLengthMax = 0;
            SouthQueueLengthMax = 0;
            WestQueueLengthMax = 0;
        }
    }
    public class CVVehicle
    {
        public IVehicle? vehicle;
        public double distanceToIntersection;
        public double clearence
        {
            get;
        }
        public int queued
        {
            get => vehicle.AttValue["InQueue"];
        }
        public CVVehicle(IVehicle vehicle, double distance)
        {
            this.vehicle = vehicle;
            this.distanceToIntersection = distance - vehicle.AttValue["Pos"];
            clearence = vehicle.AttValue["Clear"];
            //queued = vehicle.AttValue["InQueue"];
        }
        public CVVehicle()
        {
            this.vehicle = null;
            this.distanceToIntersection = 0;
            clearence = -500;
            //            //queued = false;
        }
    }
}
