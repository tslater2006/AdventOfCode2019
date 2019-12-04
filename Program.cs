using AdventOfCode.Solutions;
using System;

namespace AdventOfCode {

    class Program {

        public static Config Config = Config.Get("config.json"); 
        static SolutionCollector Solutions = new SolutionCollector(Config.Year, Config.Days); 

        static void Main(string[] args) {
            DateTime start = DateTime.Now;
            foreach(ASolution solution in Solutions) {
                solution.Solve();
            }
            Console.WriteLine("Full duration: " + (DateTime.Now - start).TotalMilliseconds);
        }
    }
}
