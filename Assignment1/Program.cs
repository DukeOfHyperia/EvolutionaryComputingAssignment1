using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GA
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Testing - Uniformly Scaled Counting Ones Function");

            int generation = 1;
            Console.WriteLine("Generation {0}\n", generation);
            GeneticAlgorithm ga = new GeneticAlgorithm(100, 50, 1, 1);
            List<Individual> Population = ga.GenerateInitialPopulation();

            List<float> GenerationFittest = new List<float>(); //store the fittest value from each generation
            Individual Fittest = ga.GetFittest(Population);

            float CurrentFittest = Fittest.Fitness;
            GenerationFittest.Add(CurrentFittest); //add fittest value from initial population/generation 1

            float MaxFittest = 0; //store maximum fittest value
            string Solution = "";
            while (CurrentFittest > MaxFittest)
            {
                generation++;
                MaxFittest = CurrentFittest;

                List<Individual> Offspring = ga.Recombination(Population);
                List<Individual> NextGeneration = ga.GenerateNextGeneration(Population, Offspring, generation); //selection

                Individual RunFittest = ga.GetFittest(NextGeneration);
                CurrentFittest = RunFittest.Fitness;
                Solution = RunFittest.Binarystring;

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

        public List<Individual> GenerateInitialPopulation()
        {
            List<Individual> Population = new List<Individual>();
            for (int i = 0; i < N; i++)
            {
                Individual gn = new Individual();
                gn.Binarystring = RandomBinary(l);
                gn.Fitness = CalculateFitness(gn.Binarystring);
                Console.WriteLine(gn.Binarystring + " --> " + gn.Fitness);
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

        public List<Individual> Recombination(List<Individual> ParentPopulation)
        {
            List<Individual> Offspring = new List<Individual>();
            for (int i = 0; i < N; i += 2)
            {
                DoCrossover((Individual)ParentPopulation[i], (Individual)ParentPopulation[i + 1], Offspring);
            }

            Console.WriteLine("\nOffspring\n");
            for (int i = 0; i < N; i++)
            {
                Individual child = (Individual)Offspring[i];
                Console.WriteLine(child.Binarystring + " --> " + child.Fitness);
            }

            return Offspring;
        }

        public void DoCrossover(Individual parent1, Individual parent2, List<Individual> Offspring)
        {
            Individual child1 = new Individual();
            Individual child2 = new Individual();
            if (crossovertype == 1) //UX
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
            else if (crossovertype == 2) //2X
            {
                Random cp = new Random();

                int crossoverpoint1 = cp.Next(0, 49); //get random number for 2 crossover points
                int crossoverpoint2 = cp.Next(50, 99);
                //while (crossoverpoint1 >= crossoverpoint2)
                //{
                //    crossoverpoint2 = cp.Next(0, 100);
                //}

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
            child1.Fitness = CalculateFitness(child1.Binarystring);
            child2.Fitness = CalculateFitness(child2.Binarystring);
            Offspring.Add(child1);
            Offspring.Add(child2);
        }

        public List<Individual> GenerateNextGeneration(List<Individual> InitialPopulation, List<Individual> GeneratedOffspring, int generation)
        {
            List<Individual> NplusN = InitialPopulation;
            NplusN.AddRange(GeneratedOffspring);
            List<Individual> SelectedNplusN = NplusN.OrderByDescending(x => x.Fitness).Take(N).ToList();

            Console.WriteLine("\nGeneration {0}\n", generation);
            for (int i = 0; i < N; i++)
            {
                Individual gn = (Individual)SelectedNplusN[i];
                Console.WriteLine(gn.Binarystring + " --> " + gn.Fitness);
            }

            return NplusN;
        }

        public Individual GetFittest(List<Individual> Population)
        {
            Individual fittest = Population.OrderByDescending(x => x.Fitness).First();
            Console.WriteLine("Fittest :{0}", fittest.Fitness);
            return fittest;
        }
    }

    class CostFunction
    {
        int id;
        List<int> randomIndices;

        public CostFunction(int functionID)
        {
            id = functionID;
            if (id == 5 || id == 6)
                randomIndices = createRandomlyLinked();
        }

        public double getFitnessValue(String input)
        {
            switch (id)
            {
                case 1:
                    return CO(input);
                case 2:
                    return SCO(input);
                case 3:
                    return TF(input, false);
                case 4:
                    return TF(input, false, 2.5);
                case 5:
                    return TF(input, true);
                case 6:
                    return TF(input, true, 2.5);
                default:
                    return -1;
            }
        }

        // Uniform Scaled Counting Ones Function
        private double CO(String input)
        {
            double result = 0;
            for (int i = 0; i < input.Length; i++)
                result += Convert.ToDouble(input[i].ToString());
            return result;
        }
        // Linearly Scaled Counting Ones Function
        private double SCO(String input)
        {
            double result = 0;
            for (int i = 0; i < input.Length; i++)
                result += Convert.ToDouble(input[i] * (i + 1));
            return result;
        }
        // Trap Function
        private double TF(String input, Boolean random, double d = 1)
        {
            int k = 4;
            double result = 0;
            for (int i = 0; i < input.Length; i += k)
            {
                double ones;
                if (!random)
                    ones = CO(input.Substring(i, k));
                else
                {
                    String randomInput = "";
                    for (int j = 0; j < k; j++)
                        randomInput += input[randomIndices[i + j]];

                    ones = CO(randomInput);
                }

                if (ones == k)
                    result += k;
                else
                    result += k - d - ((k - d) / (k - 1)) * ones;
            }
            return result;

        }

        private List<int> createRandomlyLinked()
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < 100; i++)
                indices.Add(i);

            Random rand = new Random(DateTime.Now.Millisecond);

            // randomize the indices
            int n = indices.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                int value = indices[k];
                indices[k] = indices[n];
                indices[n] = value;
            }

            return indices;
        }
    }

    class Individual
    {
        public string Binarystring { get; set; }
        public float Fitness { get; set; }
    }
}
