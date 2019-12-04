using System;
using System.Linq;

namespace AdventOfCode.Solutions.Year2019 {

    class Day04 : ASolution {
        
        public Day04() : base(4, 2019, "Secure Container") {
            
        }

        protected override string SolvePartOne() {
            int[] range = Input[0].Split('-').Select(s => int.Parse(s)).ToArray();

            return Enumerable.Range(range[0], range[1] - range[0]).Where(i => ValidPassword(i)).Count().ToString();
        }

        protected override string SolvePartTwo() {
            int[] range = Input[0].Split('-').Select(s => int.Parse(s)).ToArray();

            return Enumerable.Range(range[0], range[1] - range[0]).Where(i => ValidPassword2(i)).Count().ToString();
        }

        protected bool ValidPassword(int n)
        {
            int temp = n;

            /* check for digit ascending */
            bool isAscending = true;
            bool hasDuplicate = false;
            var lastDigit = 0;
            while (temp != 0)
            {
                lastDigit = temp % 10;
                temp = temp / 10;

                if (temp % 10 > lastDigit)
                {
                    isAscending = false;
                    break;
                }

                if (temp % 10 == lastDigit)
                {
                    hasDuplicate = true;
                }
            }
            return isAscending && hasDuplicate;
        }

        protected bool ValidPassword2(int n)
        {
            int temp = n;

            /* check for digit ascending */
            bool isAscending = true;
            bool hasDuplicate = false;
            bool checkingForDups = true;

            int duplicateCount = 0;

            var lastDigit = 0;
            while (temp != 0)
            {
                lastDigit = temp % 10;
                temp = temp / 10;

                if (temp % 10 > lastDigit)
                {
                    isAscending = false;
                    break;
                }

                if (temp % 10 == lastDigit && checkingForDups)
                {
                    if (hasDuplicate)
                    {
                        duplicateCount++;
                    }
                    else
                    {
                        hasDuplicate = true;
                        duplicateCount = 2;
                    }
                } else
                {
                    if ( duplicateCount == 2)
                    {
                        checkingForDups = false;
                    } else
                    {
                        duplicateCount = 0;
                        hasDuplicate = false;
                    }
                }


            }
            return isAscending && hasDuplicate;
        }

    }
}
