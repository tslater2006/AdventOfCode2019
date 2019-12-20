using System;
using System.IO; 
using System.Net;

namespace AdventOfCode.Solutions {

    abstract class ASolution {

        Lazy<string> _part1, _part2;
        Lazy<string[]> _input;
        DateTime start, stop;

        public int Day { get; }
        public int Year { get; }
        public string Title { get; }
        public string[] Input => _input.Value; 
        public string Part1 => string.IsNullOrEmpty(_part1.Value) ? "" : _part1.Value;
        public string Part2 => string.IsNullOrEmpty(_part2.Value) ? "" : _part2.Value;

        private protected ASolution(int day, int year, string title) {
            Day = day;
            Year = year; 
            Title = title;
            _input = new Lazy<string[]>(() => LoadInput());
            _part1 = new Lazy<string>(() => SolvePartOne());
            _part2 = new Lazy<string>(() => SolvePartTwo());
        }

        public void Solve(int part = 0) {
            start = DateTime.Now;
            if(Input == null) return; 

            bool doOutput = false; 
            string output = $"--- Day {Day}: {Title} --- \n";

            if(part != 2) {
                if(Part1 != "") {
                    output += $"Part 1: {Part1}\n"; 
                    doOutput= true; 
                } else {
                    output += "Part 1: Unsolved\n"; 
                    if(part == 1) doOutput= true; 
                }
            }
            if(part != 1) {
                if(Part2 != "") {
                    output += $"Part 2: {Part2}";
                    doOutput= true; 
                } else {
                    output += "Part 2: Unsolved";
                    if(part == 2) doOutput= true; 
                }
            }
            stop = DateTime.Now;
            if (doOutput)
            {
                Console.WriteLine(output);
                Console.WriteLine("Solved in: " + (stop - start).TotalMilliseconds + "ms" + Environment.NewLine);
            }
            
        }

        string[] LoadInput() {
            string INPUT_FILEPATH = $"./Solutions/Year{Year}/Day{Day.ToString("D2")}/input";
            string DAY_FOLDER = $"./Solutions/Year{Year}/Day{Day.ToString("D2")}";
            string INPUT_URL = $"https://adventofcode.com/{Year}/day/{Day}/input";
            string[] input = null; 

            if(File.Exists(INPUT_FILEPATH)) {
                input = File.ReadAllLines(INPUT_FILEPATH);
            } else {
                Directory.CreateDirectory(DAY_FOLDER);
                try {
                    using(var client = new WebClient()) {
                        client.Headers.Add(HttpRequestHeader.Cookie, Program.Config.Cookie);
                        File.WriteAllText(INPUT_FILEPATH, client.DownloadString(INPUT_URL));
                        input = File.ReadAllLines(INPUT_FILEPATH);
                    }
                } catch(WebException e) {
                    var statusCode = ((HttpWebResponse) e.Response).StatusCode;
                    if(statusCode == HttpStatusCode.BadRequest) {
                        Console.WriteLine($"Day {Day}: Error code 400 when attempting to retrieve puzzle input through the web client. Your session cookie is probably not recognized.");
                    } else if(statusCode == HttpStatusCode.NotFound) {
                        Console.WriteLine($"Day {Day}: Error code 404 when attempting to retrieve puzzle input through the web client. The puzzle is probably not available yet.");
                    } else {
                        Console.WriteLine(e.Status);
                    }
                }
            }
            return input; 
        }

        protected abstract string SolvePartOne();
        protected abstract string SolvePartTwo();
    }
}
