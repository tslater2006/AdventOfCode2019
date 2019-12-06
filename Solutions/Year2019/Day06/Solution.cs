using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class OrbitObject
    {
        public OrbitObject Parent;
        public List<OrbitObject> Children;
        public string Name;
        public int Depth;

        public override bool Equals(object obj)
        {
            /* assume obj is Orbit Object */
            var orbit = (OrbitObject)obj;
            return orbit.Name.Equals(this.Name);
        }

    }

    class Day06 : ASolution {
        Dictionary<string, List<string>> OrbitPairs = new Dictionary<string, List<string>>();
        Dictionary<string, OrbitObject> FlatList = new Dictionary<string, OrbitObject>();
        public Day06() : base(6, 2019, "") {
            
            foreach (var s in Input)
            {
                var leftRight = s.Split(')');

                if (OrbitPairs.ContainsKey(leftRight[0]) == false)
                {
                    OrbitPairs.Add(leftRight[0], new List<string>());
                }

                OrbitPairs[leftRight[0]].Add(leftRight[1]);
            }

            OrbitObject COM = new OrbitObject() { Name = "COM", Depth = 0, Parent = null };
            FlatList.Add("COM", COM);
            AddOrbitPairs(COM);

        }

        protected override string SolvePartOne() {

            var answer = FlatList.Sum(obj => obj.Value.Depth);

            return answer.ToString(); 
        }

        protected void AddOrbitPairs(OrbitObject parent)
        {
            if (OrbitPairs.ContainsKey(parent.Name))
            {
                foreach (var child in OrbitPairs[parent.Name])
                {
                    var childObject = new OrbitObject() { Name = child, Depth = parent.Depth + 1, Parent = parent };
                    FlatList.Add(child, childObject);
                    if (parent.Children == null)
                    {
                        parent.Children = new List<OrbitObject>();
                    }

                    parent.Children.Add(childObject);
                    AddOrbitPairs(childObject);
                }
            }
        }

        protected override string SolvePartTwo() {
            List<OrbitObject> SantaOrbits = new List<OrbitObject>();
            List<OrbitObject> YouOrbits = new List<OrbitObject>();

            OrbitObject Santa = FlatList["SAN"];
            OrbitObject You = FlatList["YOU"];

            var santaParent = Santa.Parent;
            while (santaParent != null)
            {
                SantaOrbits.Add(santaParent);
                santaParent = santaParent.Parent;
            }

            var youParent = You.Parent;
            while (youParent != null)
            {
                YouOrbits.Add(youParent);
                youParent = youParent.Parent;
            }

            /* Find nodes where the chains intersect */
            var intersects = SantaOrbits.Intersect(YouOrbits);

            /* find the deepest intersection */
            var deepest = intersects.OrderByDescending(o => o.Depth).First();

            var youDistance = You.Parent.Depth - deepest.Depth;
            var santaDistance = Santa.Parent.Depth - deepest.Depth;

            var distance = youDistance + santaDistance;

            return distance.ToString(); 
        }
    }
}
