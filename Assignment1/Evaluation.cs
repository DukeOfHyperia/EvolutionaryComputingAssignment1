using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    class Evaluation
    {
        int populationSize, succes;
        String crossovertype, fitnessFunction;
        List<int> firstHit, converge, fctEvals, cpuTime;

        public Evaluation(int PopulationSize, int CrossoverType, int FitnessFunction)
        {
            populationSize = PopulationSize;

            if (CrossoverType == 1)
                crossovertype = "UX";
            else
                crossovertype = "2X";

            switch (FitnessFunction)
            {
                case 1:
                    fitnessFunction = "Uniformly Scaled Counting Ones Function";
                    break;
                case 2:
                    fitnessFunction = "Linearly Scaled Counting Ones Function";
                    break;
                case 3:
                    fitnessFunction = "Deceptive Trap Function (thightly linked)";
                    break;
                case 4:
                    fitnessFunction = "Non-deceptive Trap Function (tightly linked)";
                    break;
                case 5:
                    fitnessFunction = "Deceptive Trap Function (randomly linked)";
                    break;
                case 6:
                    fitnessFunction = "Non-deceptive Trap Function (randomly linked)";
                    break;
                default:
                    fitnessFunction = "Something went wrong!";
                    break;
            }
            firstHit = new List<int>();
            converge = new List<int>();
            fctEvals = new List<int>();
            cpuTime = new List<int>();

            succes = 0;
        }

        public void AddRun(Tuple<int,int,int,int> data)
        {
            if (data.Item1 != -1)
            {
                succes++;
                firstHit.Add(data.Item1);
            }

            converge.Add(data.Item2);
            fctEvals.Add(data.Item3);
            cpuTime.Add(data.Item4);

        }
        public void outputResults()
        {
            Console.WriteLine(crossovertype + ": " + fitnessFunction);
            Console.WriteLine("Population size: " + populationSize);
            Console.WriteLine("Succes   : " + succes + "/25");
            Console.WriteLine("First hit: " + calculateStatistics(firstHit));
            Console.WriteLine("Converge : " + calculateStatistics(converge));
            Console.WriteLine("Fct Evals: " + calculateStatistics(fctEvals));
            Console.WriteLine("CPU time : " + calculateStatistics(cpuTime) + "ms");
            Console.WriteLine("");

            // Output LaTeX-code for tables
            /*Console.WriteLine(" & " + String.Join(" & ", populationSize,
                                                            succes + "/25",
                                                            calculateStatistics(firstHit),
                                                            calculateStatistics(converge),
                                                            calculateStatistics(fctEvals),
                                                            calculateStatistics(cpuTime)) + " \\\\");
            Console.WriteLine("");*/
        }

        private String calculateStatistics(List<int> data)
        {
            if (data.Count != 0)
            {
                double avg = data.Average();
                double std;
                if (data.Count > 1)
                    std = Math.Sqrt(data.Select(x => (x - avg) * (x - avg)).Sum() / (data.Count - 1));
                else
                    std = 0;
                return (Math.Round(avg, 2) + " (" + Math.Round(std, 2) + ")").Replace(',', '.');
            }
            return "NA (NA)";
            
        }
    }
}
