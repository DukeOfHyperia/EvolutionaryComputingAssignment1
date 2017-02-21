using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Testing - Uniformly Scaled Counting Ones Function");

            int generation = 1;
            Console.WriteLine("Generation {0}\n", generation);
            GeneticAlgorithm ga = new GeneticAlgorithm(100, 250, 1, 1);
            List<Genotype> Population = ga.GenerateInitialPopulation();

            List<float> GenerationFittest = new List<float>(); //store the fittest value from each generation
            Genotype Fittest = ga.GetFittest(Population);

            float CurrentFittest = Fittest.Fitness;
            GenerationFittest.Add(CurrentFittest); //add fittest value from initial population/generation 1

            float MaxFittest = 0; //store maximum fittest value
            string Solution = "";
            while (CurrentFittest > MaxFittest)
            {
                generation++;
                MaxFittest = CurrentFittest;

                List<Genotype> Offspring = ga.Recombination(Population);
                List<Genotype> NextGeneration = ga.GenerateNextGeneration(Population, Offspring, generation); //selection
                
                Genotype RunFittest = ga.GetFittest(NextGeneration);
                CurrentFittest = RunFittest.Fitness;
                Solution = RunFittest.binarystring;

                GenerationFittest.Add(CurrentFittest);
            }

            Console.WriteLine("\n");
            for (int i = 0; i < GenerationFittest.Count; i++)
            {
                Console.WriteLine("Generation: {0}  Fittest: {1}", i + 1, GenerationFittest[i].ToString());
            }

            Console.WriteLine("Solution found!");
            Console.WriteLine("Generation: {0}", generation);
            Console.WriteLine("Fitness Value: {0}\nGenotype: {1}", MaxFittest, Solution);
            Console.ReadLine();
        }
    }

    class GeneticAlgorithm
    {
        int l;
        int N;
        int functiontype;
        int crossovertype;
        public GeneticAlgorithm(int stringLength, int populationSize, int crossoverType, int costFunction)
        {
            // Enter here the code for the genetic algorithm
            l = stringLength;
            N = populationSize;
            functiontype = costFunction;
            crossovertype = crossoverType;
        }

        public List<Genotype> GenerateInitialPopulation()
        {
            List<Genotype> Population = new List<Genotype>();
            for (int i = 0; i < N; i++)
            {
                Genotype gn = new Genotype();
                gn.binarystring = RandomBinary(l);
                gn.Fitness = CalculateFitness(gn.binarystring);
                Console.WriteLine(gn.binarystring + " --> " + gn.Fitness);
                Population.Add(gn);
            }
            return Population;
        }

        private static readonly Random rand = new Random();
        private static readonly object syncLock = new object();
        private string RandomBinary(int l)
        {
            string result = string.Empty;

            for (int i = 0; i < l; i++)
            {
                result += ((rand.Next() % 2 == 0) ? "0" : "1");
            }

            return result;
        }

        public float CalculateFitness(string binarystring)
        {
            //testing using the first function
            float fitnessvalue = 0;
            switch (functiontype)
            {
                case 1:
                    fitnessvalue = binarystring.Select(x => (int)x - '0').Sum();
                    break;
            }

            return fitnessvalue;
        }

        public List<Genotype> Recombination(List<Genotype> ParentPopulation)
        {
            List<Genotype> Offspring = new List<Genotype>();
            for (int i = 0; i < N; i += 2)
            {
                DoCrossover((Genotype)ParentPopulation[i], (Genotype)ParentPopulation[i + 1], Offspring);
            }

            Console.WriteLine("\nOffspring\n");
            for (int i = 0; i < N; i++)
            {
                Genotype child = (Genotype)Offspring[i];
                Console.WriteLine(child.binarystring + " --> " + child.Fitness);
            }

            return Offspring;
        }

        public void DoCrossover(Genotype parent1, Genotype parent2, List<Genotype> Offspring)
        {
            Genotype child1 = new Genotype();
            Genotype child2 = new Genotype();
            if (crossovertype == 1) //UX
            {
                for (int i = 0; i < l; i++)
                {
                    bool flip = rand.NextDouble() >= 0.5; //get random true or false
                    if (flip)
                    {
                        child1.binarystring += parent2.binarystring[i];
                        child2.binarystring += parent1.binarystring[i];
                    }
                    else
                    {
                        child1.binarystring += parent1.binarystring[i];
                        child2.binarystring += parent2.binarystring[i];
                    }
                }
            }
            else if (functiontype == 2) //2X
            {
                //will be added later
            }
            child1.Fitness = CalculateFitness(child1.binarystring);
            child2.Fitness = CalculateFitness(child2.binarystring);
            Offspring.Add(child1);
            Offspring.Add(child2);
        }

        public List<Genotype> GenerateNextGeneration(List<Genotype> InitialPopulation, List<Genotype> GeneratedOffspring, int generation)
        {
            List<Genotype> NplusN = InitialPopulation;
            NplusN.AddRange(GeneratedOffspring);
            List<Genotype> SelectedNplusN = NplusN.OrderByDescending(x => x.Fitness).Take(N).ToList();

            Console.WriteLine("\nGeneration {0}\n", generation);
            for (int i = 0; i < N; i++)
            {
                Genotype gn = (Genotype)SelectedNplusN[i];
                Console.WriteLine(gn.binarystring + " --> " + gn.Fitness);
            }

            return NplusN;
        }

        public Genotype GetFittest(List<Genotype> Population)
        {
            Genotype fittest = Population.OrderByDescending(x => x.Fitness).First();
            Console.WriteLine("Fittest :{0}", fittest.Fitness);
            return fittest;
        }

    }

    class CostFunction
    {
        public CostFunction()
        {
        }
    }

    class Genotype
    {
        public string binarystring { get; set; }
        public float Fitness { get; set; }
    }
}
