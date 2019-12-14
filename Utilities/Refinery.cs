using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode.Utilities
{
    public class Refinery
    {
        public Dictionary<string,RefineryReaction> Reactions = new Dictionary<string,RefineryReaction>();
        public Dictionary<string, int> Surplus = new Dictionary<string, int>();
        public long OreRequired = 0;
        public Refinery(string[] reactionList)
        {
            Regex r = new Regex(@"(\d+ [A-Z]+)");
            foreach (var l in reactionList)
            {
                List<RefineryProduction> precursors = new List<RefineryProduction>();

                var matches = r.Matches(l);
                var output = matches.Last();

                var parts = output.Value.Split(' ');
                int amount = int.Parse(parts[0]);
                string chemical = parts[1];

                var outputProduction = new RefineryProduction() { Amount = amount, Chemical = chemical };

                for (var x = 0; x < matches.Count - 1; x++)
                {
                    parts = matches[x].Value.Split(' ');
                    amount = int.Parse(parts[0]);
                    chemical = parts[1];

                    var production = new RefineryProduction() { Amount = amount, Chemical = chemical };

                    precursors.Add(production);
                }
                Reactions.Add(outputProduction.Chemical, new RefineryReaction() { Output = outputProduction, Precursors = precursors });
            }
        }

        public void ProduceMaterial(RefineryProduction request)
        {
            /* if the request is to produce ORE, just add that to our OreRequired var */
            if (request.Chemical.Equals("ORE"))
            {
                OreRequired += request.Amount;
                return;
            }


            /* Check if we've overproduced the requested chemical previously */
            if (Surplus.ContainsKey(request.Chemical))
            {
                /* if the surplus has enough to satisfy the request, consume the surplus and return */
                if (Surplus[request.Chemical] >= request.Amount)
                {
                    Surplus[request.Chemical] -= request.Amount;
                    return;
                }
                else
                {
                    /* otherwise, take what surplus there is and update the requested amount to create */
                    request.Amount -= Surplus[request.Chemical];
                    Surplus[request.Chemical] = 0;
                }
            }

            /* get the reaction that can make this request */
            var reaction = Reactions[request.Chemical];

            /* determine scale based on requested amount and production amount */
            var reactionRepeat = (int)Math.Ceiling((double)request.Amount / (double)reaction.Output.Amount);

            /* create each precursor, scaling amount as needed */
            foreach (var p in reaction.Precursors)
            {
                ProduceMaterial(new RefineryProduction() { Amount = p.Amount * reactionRepeat, Chemical = p.Chemical });
            }

            /* if we produced more than needed, adjust our running surplus */
            if (reaction.Output.Amount * reactionRepeat > request.Amount)
            {
                if (Surplus.ContainsKey(reaction.Output.Chemical))
                {
                    Surplus[reaction.Output.Chemical] += (reaction.Output.Amount * reactionRepeat) - request.Amount;
                }
                else
                {
                    Surplus.Add(reaction.Output.Chemical, (reaction.Output.Amount * reactionRepeat) - request.Amount);
                }
            }
        }
    }

    public struct RefineryProduction
    {
        public int Amount;
        public string Chemical;

        public override string ToString()
        {
            return $"{Amount} {Chemical}";
        }
    }

    public class RefineryReaction
    {
        public List<RefineryProduction> Precursors = new List<RefineryProduction>();
        public RefineryProduction Output;
    }

}
