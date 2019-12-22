using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Utilities
{
    
    class SpaceCards
    {
        public int[] Cards = null;
        int[] Table = null;
        public SpaceCards(int count)
        {
            Table = new int[count];
            Cards = new int[count];

            for(var x = 0; x < count; x++)
            {
                Table[x] = -1;
                Cards[x] = x;
            }
        }

        public void DealNewStack()
        {
            Cards = Cards.Reverse().ToArray();
        }

        public void CutCards(int n)
        {
            var cardsDealt = 0;
            var cutIndex = n;
            if (n < 0)
            {
                cutIndex = Cards.Length + n;
            }
            var tableIndex = 0;
            while (cardsDealt < Cards.Length)
            {
                Table[tableIndex++] = Cards[cutIndex++];
                cardsDealt++;

                if (cutIndex >= Cards.Length)
                {
                    cutIndex = 0;
                }

            }

            /* copy the table back to Cards */
            Array.Copy(Table, 0, Cards, 0, Cards.Length);
        }

        public void DealByIncrement(int inc)
        {
            int incrementPos = 0;
            for(var x = 0; x < Cards.Length; x++)
            {
                Table[incrementPos] = Cards[x];
                incrementPos += inc;

                while(incrementPos >= Cards.Length)
                {
                    incrementPos -= Cards.Length;
                }
            }

            /* copy the table back to Cards */
            Array.Copy(Table, 0, Cards, 0, Cards.Length);

        }


    }

}
