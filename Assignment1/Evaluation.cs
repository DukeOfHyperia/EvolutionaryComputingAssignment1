﻿using System;
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

        public void AddRun(int FirstHit, int Converge, int FctEvals, int CPUTime, Boolean Succes)
        {
            firstHit.Add(FirstHit);
            converge.Add(Converge);
            fctEvals.Add(FctEvals);
            cpuTime.Add(CPUTime);
            if (Succes)
                succes++;
        }
        public void outputResults()
        {
            Console.WriteLine(crossovertype + ": " + fitnessFunction);
            Console.WriteLine("Population size: " + populationSize);
            Console.WriteLine("Succes   : " + succes + "/25");
            Console.WriteLine("First hit: " + calculateStatistics(firstHit));
            Console.WriteLine("Converge : " + calculateStatistics(converge));
            Console.WriteLine("Fct Evals: " + calculateStatistics(fctEvals));
            Console.WriteLine("CPU time : " + calculateStatistics(cpuTime));
        }

        private Tuple<double, double> calculateStatistics(List<int> data)
        {
            double avg = data.Average();
            double std = Math.Sqrt(data.Select(x => (x - avg) * (x - avg)).Sum() / (data.Count - 1));

            Console.Write(avg + " (" + std + ")");
            return new Tuple<double, double>(avg, std);
        }
    }
}
