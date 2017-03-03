using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GA
{
    class FitnessFunction
    {
        int id, fctEval;
        List<int> randomIndices;

        public FitnessFunction(int functionID)
        {
            id = functionID;
            fctEval = 0;
            if (id == 5 || id == 6)
                randomIndices = createRandomlyLinked();
        }

        public double getFitnessValue(String input)
        {
            fctEval++;
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

        public int getFctEval()
        {
            return fctEval;
        }

        // Uniform Scaled Counting Ones Function
        private double CO(String input)
        {
            double result = 0;
            for (int i = 0; i < input.Length; i++)
                result += Char.GetNumericValue(input[i]);
            return result;
        }
        // Linearly Scaled Counting Ones Function
        private double SCO(String input)
        {
            double result = 0;
            for (int i = 0; i < input.Length; i++)
            {
                result += Char.GetNumericValue(input[i]) * (i + 1);
            }
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
}
