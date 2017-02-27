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
            int generation = 1;
            Console.WriteLine("Generation {0}\n", generation);
            GeneticAlgorithm ga = new GeneticAlgorithm(100, 50, 1, 1);
            List<Individual> Population = ga.GenerateInitialPopulation(); //generate initial population

            List<double> GenerationFittest = new List<double>(); //list to store the fittest value from each generation to be written on the console
            Individual Fittest = ga.GetFittest(Population); //get the individual having the fittest value from initial the population
            double CurrentFittest = Fittest.Fitness; 
            GenerationFittest.Add(CurrentFittest); 
            string Solution = ""; //binarystring of the solution
            
            double lowestparent = 0;
            double highestchildren = 1;
            while (highestchildren > lowestparent) //repeat while the highest fitness value of the offspring is bigger than the lowest fitness value of the parent
            {
                generation++;

                List<Individual> Offspring = ga.Recombination(Population); //do recombination (crossover)
                List<Individual> NextGeneration = ga.GenerateNextGeneration(Population, Offspring, generation); //do selection

                Individual RunFittest = ga.GetFittest(NextGeneration); //ge the fittest individual from population
                CurrentFittest = RunFittest.Fitness;
                Solution = RunFittest.Binarystring;

                GenerationFittest.Add(CurrentFittest);

                lowestparent = Population.Select(x => x.Fitness).Min();
                highestchildren = Offspring.Select(x => x.Fitness).Max();
                
                Population = NextGeneration;
            }

            //write the final solution and the fitness value of each generation
            Console.WriteLine("\n");
            for (int i = 0; i < GenerationFittest.Count; i++)
            {
                Console.WriteLine("Generation: {0}  Fittest: {1}", i + 1, GenerationFittest[i].ToString());
            }

            Console.WriteLine("Solution found!");
            Console.WriteLine("Generation: {0}", generation);
            Console.WriteLine("Fitness Value: {0}\nIndividual: {1}", CurrentFittest, Solution);
            Console.ReadLine();
        }
    }

    class GeneticAlgorithm
    {
        int l;
        int N;
        CostFunction cf;
        int crossovertype;
        public GeneticAlgorithm(int stringLength, int populationSize, int crossoverType, int costFunction)
        {
            l = stringLength;
            N = populationSize;
            cf = new CostFunction(costFunction);
            crossovertype = crossoverType;
        }

        public List<Individual> GenerateInitialPopulation()
        {
            List<Individual> Population = new List<Individual>();
            for (int i = 0; i < N; i++)
            {
                Individual gn = new Individual();
                gn.Binarystring = RandomBinary(l);
                gn.Fitness = cf.getFitnessValue(gn.Binarystring);
                Console.WriteLine(gn.Binarystring + " --> " + gn.Fitness);
                Population.Add(gn);
            }
            return Population;
        }


        //generate random binarystring for the initial population
        private static readonly Random rand = new Random();
        private static readonly object syncLock = new object();
        private string RandomBinary(int l)
        {
            string result = string.Empty;

            for (int i = 0; i < l; i++)
            {
                result += ((rand.Next() % 2 == 0) ? "0" : "1"); //get random 0 and 1
            }

            return result;
        }

        //generate offspring by doing crossover
        public List<Individual> Recombination(List<Individual> ParentPopulation)
        {
            List<Individual> Offspring = new List<Individual>();
            for (int i = 0; i < N; i += 2)
            {
                DoCrossover((Individual)ParentPopulation[i], (Individual)ParentPopulation[i + 1], Offspring);
            }

            return Offspring;
        }

        public void DoCrossover(Individual parent1, Individual parent2, List<Individual> Offspring)
        {
            Individual child1 = new Individual();
            Individual child2 = new Individual();
            if (crossovertype == 1) //UX (Unifom Crossover)
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
            else if (crossovertype == 2) //2X (2-Point Crossover)
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
            child1.Fitness = cf.getFitnessValue(child1.Binarystring);
            child2.Fitness = cf.getFitnessValue(child2.Binarystring);
            Offspring.Add(child1);
            Offspring.Add(child2);
        }

        //generate next generation (N+N-selection)
        public List<Individual> GenerateNextGeneration(List<Individual> InitialPopulation, List<Individual> GeneratedOffspring, int generation)
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

            Console.WriteLine("\nGeneration {0}\n", generation);
            for (int i = 0; i < N; i++)
            {
                Individual gn = (Individual)NplusN[i];
                Console.WriteLine(gn.Binarystring + " --> " + gn.Fitness);
            }

            return NplusN;
        }

        public Individual GetFittest(List<Individual> Population)
        {
            Individual fittest = Population.OrderByDescending(x => x.Fitness).First(); //get the idividual having the highest fitness value from the population
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
        public double Fitness { get; set; }
    }
}