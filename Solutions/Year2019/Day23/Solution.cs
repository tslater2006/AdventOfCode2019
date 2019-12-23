using AdventOfCode.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day23 : ASolution {

        public Day23() : base(23, 2019, "Category Six") {
            
        }

        protected override string SolvePartOne() {
            IntCodeVM[] vmList = new IntCodeVM[50];
            Dictionary<int, IntCodeVM> VMLookup = new Dictionary<int, IntCodeVM>();
            DefaultDictionary<int, Queue<NetworkPacket>> PendingPackets = new DefaultDictionary<int, Queue<NetworkPacket>>();
            long ans = 0;
            for(var x = 0; x <vmList.Length; x++)
            {
                vmList[x] = new IntCodeVM(Input[0]);
                /* assign VM number */
                vmList[x].WriteInput(x);
                VMLookup.Add(x, vmList[x]);
            }
            while (true)
            {
                /* network timestep */
                for (var index = 0; index < vmList.Length; index++)
                {
                    var curVM = vmList[index];
                    /* any pending packets ? */
                    for (var x = 0; x < PendingPackets[index].Count; x++)
                    {
                        var packet = PendingPackets[index].Dequeue();
                        curVM.WriteInput(packet.X);
                        curVM.WriteInput(packet.Y);
                    }


                    /* run it */
                    var haltType = curVM.RunProgram();
                    if (haltType == HaltType.HALT_WAITING)
                    {
                        curVM.WriteInput(-1);
                    }

                    /* process this VMs outputs */
                    var outputs = curVM.ReadOutputs();

                    while (outputs.Count > 0)
                    {
                        /* read an address */
                        var addr = outputs.Dequeue();
                        var x = outputs.Dequeue();
                        var y = outputs.Dequeue();

                        if (addr == 255)
                        {
                            return y.ToString();
                        }

                        /* make packet */
                        PendingPackets[(int)addr].Enqueue(new NetworkPacket() { X = x, Y = y });
                    }

                }
            }

            return null;
        }

        protected override string SolvePartTwo() {
            IntCodeVM[] vmList = new IntCodeVM[50];
            Dictionary<int, IntCodeVM> VMLookup = new Dictionary<int, IntCodeVM>();
            DefaultDictionary<int, Queue<NetworkPacket>> PendingPackets = new DefaultDictionary<int, Queue<NetworkPacket>>();
            NetworkPacket NAT = new NetworkPacket();
            HashSet<long> NATY = new HashSet<long>();
            long ans = 0;
            for (var x = 0; x < vmList.Length; x++)
            {
                vmList[x] = new IntCodeVM(Input[0]);
                /* assign VM number */
                vmList[x].WriteInput(x);
                VMLookup.Add(x, vmList[x]);
            }
            while (true)
            {
                /* network timestep */
                for (var index = 0; index < vmList.Length; index++)
                {
                    var curVM = vmList[index];
                    /* any pending packets ? */
                    for (var x = 0; x < PendingPackets[index].Count; x++)
                    {
                        var packet = PendingPackets[index].Dequeue();
                        curVM.WriteInput(packet.X);
                        curVM.WriteInput(packet.Y);
                    }


                    /* run it */
                    var haltType = curVM.RunProgram();
                    if (haltType == HaltType.HALT_WAITING)
                    {
                        curVM.WriteInput(-1);
                    }

                    /* process this VMs outputs */
                    var outputs = curVM.ReadOutputs();

                    while (outputs.Count > 0)
                    {
                        /* read an address */
                        var addr = outputs.Dequeue();
                        var x = outputs.Dequeue();
                        var y = outputs.Dequeue();

                        if (addr == 255)
                        {
                            NAT.X = x;
                            NAT.Y = y;
                            continue;
                        }

                        /* make packet */
                        PendingPackets[(int)addr].Enqueue(new NetworkPacket() { X = x, Y = y });
                    }

                    /* check idle state */
                    if (PendingPackets.Sum(kvp => kvp.Value.Count) == 0)
                    {
                        /* detected idle network, put NAT on queue */
                        if (NATY.Contains(NAT.Y))
                        {
                            return NAT.Y.ToString();
                        }
                        else
                        {
                            NATY.Add(NAT.Y);
                            PendingPackets[0].Enqueue(NAT);
                        }
                    }

                }
            }

            return null;
        }
    }

    class NetworkPacket
    {
        public long X;
        public long Y;

    }

}
