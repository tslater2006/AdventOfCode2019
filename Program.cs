using AdventOfCode.Solutions;
using System;

namespace AdventOfCode {

    class Program {

        public static Config Config = Config.Get("config.json"); 
        static SolutionCollector Solutions = new SolutionCollector(Config.Year, Config.Days); 

        static void Main(string[] args) {
            foreach(ASolution solution in Solutions) {
                DateTime start = DateTime.Now;
                solution.Solve();
                DateTime stop = DateTime.Now;
                Console.WriteLine("Solved in: " + (stop - start).TotalMilliseconds + "ms");
            }
        }
    }
}
