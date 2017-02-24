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
            GeneticAlgorithm ga = new GeneticAlgorithm(100, 1, 1);

            CostFunction cf = null;

            for(int i = 0; i < 60; i++)
            {
                if (i % 5 == 0)
                    cf = new CostFunction(i / 10 + 1);
                String input = ga.generateBinString();
                Console.WriteLine(input + " => " + cf.getFitnessValue(input));

            }
        Console.ReadLine();
        }
    }

    class GeneticAlgorithm
    {

        public GeneticAlgorithm(int N, int crossoverType, int costFunction)
        {
            // Enter here the code for the genetic algorithm
        }

        public String generateBinString()
        {
            Thread.Sleep(1);
            Random rand = new Random(DateTime.Now.Millisecond);
            String output = "";
            for (int i = 0; i < 100; i++)
                output += rand.Next(0, 2);
            return output;
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
            switch(id)
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
            for(int i = 0; i < input.Length; i += k)
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
}
