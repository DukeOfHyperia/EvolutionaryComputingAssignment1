using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace GA
{
    class Program
    {
        static void Main()
        {
            int[] size = { 50, 100, 250, 500};
            //fitness function
            for (int f = 1; f <= 6; f++)
            {
                //crossover type
                for (int c = 2; c > 0; c--)
                {
                    //population size
                    for (int p = 0; p < 4; p++)
                    {
                        Evaluation eval = new Evaluation(size[p], c, f);
                        // run 25 times
                        for(int i = 0; i < 24; i++)
                            eval.AddRun(Execute(new GeneticAlgorithm(100, size[p], c, f)));
                        eval.outputResults();
                    }
                }
            }
            Console.ReadLine();
        }

        static Tuple<int, int, int, int> Execute(GeneticAlgorithm ga)
        {
            int firstHit = -1;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int generation = 0;
            List<Individual> population = ga.GenerateInitialPopulation(); //generate initial population
            Individual Fittest = ga.GetFittest(population); //get the individual having the fittest value from initial the population

            double lowestParent, highestChild;
            while (true) //repeat while the highest fitness value of the offspring is bigger than the lowest fitness value of the parent
            {
                generation++;
                List<Individual> offspring = ga.Recombination(population); //do recombination (crossover)
                List<Individual> nextGeneration = ga.GenerateNextGeneration(population, offspring); //do selection

                Individual runFittest = ga.GetFittest(nextGeneration); //ge the fittest individual from population

                if (runFittest.Binarystring == ga.globalOptimum && firstHit == -1)
                    firstHit = generation;

                lowestParent = population.Select(x => x.Fitness).Min();
                highestChild = offspring.Select(x => x.Fitness).Max();

                if (highestChild <= lowestParent)
                    break;

                population = nextGeneration;
            }
            sw.Stop();

            return new Tuple<int, int, int, int>(firstHit, generation, ga.ff.getFctEval(), (int)sw.ElapsedMilliseconds);
        }
    }
}