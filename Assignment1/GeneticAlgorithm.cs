using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    class GeneticAlgorithm
    {
        int l, N, crossoverType;
        public FitnessFunction ff;
        public string globalOptimum;

        public GeneticAlgorithm(int StringLength, int PopulationSize, int CrossoverType, int fitnessFunction)
        {
            l = StringLength;
            N = PopulationSize;
            ff = new FitnessFunction(fitnessFunction);
            crossoverType = CrossoverType;
            GlobalOptimum();
        }

        public List<Individual> GenerateInitialPopulation()
        {
            List<Individual> Population = new List<Individual>();
            for (int i = 0; i < N; i++)
            {
                Individual gn = new Individual();
                gn.Binarystring = RandomBinary(l);
                gn.Fitness = ff.getFitnessValue(gn.Binarystring);
                Population.Add(gn);
            }
            return Population;
        }

        //generate random binarystring for the initial population
        private static readonly Random rand = new Random();
        private static readonly object syncLock = new object();
        private string RandomBinary(int l)
        {
            string result = "";
            for (int i = 0; i < l; i++)
                result += ((rand.Next() % 2 == 0) ? "0" : "1"); //get random 0 and 1
            return result;
        }
        private void GlobalOptimum()
        {
            for (int i = 0; i < l; i++)
                globalOptimum += "1";
        }

        //generate offspring by doing crossover
        public List<Individual> Recombination(List<Individual> ParentPopulation)
        {
            List<Individual> Offspring = new List<Individual>();
            for (int i = 0; i < N; i += 2)
                DoCrossover((Individual)ParentPopulation[i], (Individual)ParentPopulation[i + 1], Offspring);
            return Offspring;
        }

        public void DoCrossover(Individual parent1, Individual parent2, List<Individual> Offspring)
        {
            Individual child1 = new Individual();
            Individual child2 = new Individual();
            if (crossoverType == 1) //UX (Unifom Crossover)
            {
                for (int i = 0; i < l; i++)
                {
                    bool flip = rand.NextDouble() >= 0.5; //get random true or false
                    if (flip)
                    {
                        child1.Binarystring += parent2.Binarystring[i];
                        child2.Binarystring += parent1.Binarystring[i];
                    }
                    else
                    {
                        child1.Binarystring += parent1.Binarystring[i];
                        child2.Binarystring += parent2.Binarystring[i];
                    }
                }
            }
            else if (crossoverType == 2) //2X (2-Point Crossover)
            {
                Random cp = new Random();

                //get random 2 crossover points from 0 - 100
                int crossoverpoint1 = cp.Next(0, 99);
                int crossoverpoint2 = cp.Next(crossoverpoint1, 100);

                for (int i = 0; i < crossoverpoint1; i++)
                {
                    child1.Binarystring += parent1.Binarystring[i];
                    child2.Binarystring += parent2.Binarystring[i];
                }
                for (int i = crossoverpoint1; i < crossoverpoint2; i++)
                {
                    child1.Binarystring += parent2.Binarystring[i];
                    child2.Binarystring += parent1.Binarystring[i];
                }
                for (int i = crossoverpoint2; i < l; i++)
                {
                    child1.Binarystring += parent1.Binarystring[i];
                    child2.Binarystring += parent2.Binarystring[i];
                }
            }
            child1.Fitness = ff.getFitnessValue(child1.Binarystring);
            child2.Fitness = ff.getFitnessValue(child2.Binarystring);
            Offspring.Add(child1);
            Offspring.Add(child2);
        }

        //generate next generation (N+N-selection)
        public List<Individual> GenerateNextGeneration(List<Individual> InitialPopulation, List<Individual> GeneratedOffspring)
        {
            List<Individual> NplusN = InitialPopulation;
            NplusN.AddRange(GeneratedOffspring);
            NplusN = NplusN.OrderByDescending(x => x.Fitness).Take(N).ToList(); //sort the fitness value and select only N individual

            // randomize the population
            int n = NplusN.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                Individual value = NplusN[k];
                NplusN[k] = NplusN[n];
                NplusN[n] = value;
            }

            return NplusN;
        }

        public Individual GetFittest(List<Individual> Population)
        {
            Individual fittest = Population.OrderByDescending(x => x.Fitness).First(); //get the idividual having the highest fitness value from the population
            return fittest;
        }
    }
}
