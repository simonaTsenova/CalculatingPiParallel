using Deveel.Math;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CalculatingPi
{
    class Program
    {
        const string PRECISION_PARAM = "-p";
        const string THREADS_PARAM = "-t";
        const string OUTPUT_FILE_PARAM = "-o";
        const string QUIET_MODE_PARAM = "-q";

        static void Main(string[] args)
        {
            // Parse parameters
            //var pIndex = Array.IndexOf(args, PRECISION_PARAM);
            //var precision = pIndex >= 0 ? int.Parse(args[pIndex + 1]) : -1;

            //var tIndex = Array.IndexOf(args, THREADS_PARAM);
            //var threadsCount = tIndex >= 0 ? int.Parse(args[tIndex + 1]) : -1;

            //var oIndex = Array.IndexOf(args, OUTPUT_FILE_PARAM);
            //var outputFilename = oIndex >= 0 ? args[oIndex + 1] : "result.txt";

            //var isQuiet = Array.IndexOf(args, QUIET_MODE_PARAM);

            var precision = 5;
            var threadsCount = 1;

            // Calculate PI
            //Console.WriteLine(Factorial(precision).ToString());
            BigDecimal sum = 0;
            Parallel.For(0, precision, new ParallelOptions() { MaxDegreeOfParallelism = threadsCount }, i =>
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine("Thread-" + currentThreadId + " started.");
                //BigDecimal numerator = i % 2 == 0 ? Factorial(4 * i).Multiply(1123 + (21460 * i)) : Factorial(4 * i).Multiply(1123 + (21460 * i));
                BigDecimal numerator;
                if (i % 2 == 0)
                {
                    numerator = Factorial(4 * i).Multiply(1123 + (21460 * i));
                }
                else
                {
                    numerator = -Factorial(4 * i).Multiply(1123 + (21460 * i));
                }
                Console.WriteLine("Num: " + numerator);
                BigDecimal denominator = (Factorial(i).Pow(4)).Multiply((new BigDecimal(14112)).Pow(2 * i));
                Console.WriteLine("Denom: " + denominator);

                BigDecimal currentAddition = numerator / denominator;

                sum = sum.Add(currentAddition);
                //Console.WriteLine(currentAddition.ToString());
                //Console.WriteLine("Thread-" + currentThreadId + " stopped.");
            });

            BigDecimal constant = (BigDecimal)1 / (BigDecimal)3528;
            Console.WriteLine("const: " + constant);
            Console.WriteLine("sum: " + sum);
            Console.WriteLine("Results:");
            BigDecimal opposite = constant.Multiply(sum);
            Console.WriteLine("opposite: " + opposite);

            BigDecimal pi = new BigDecimal(1) / opposite;
            Console.WriteLine("Pi: " + pi);
        }

        static BigInteger Factorial(int n)
        {
            if (n == 0 || n == 1)
            {
                return 1;
            }

            BigInteger result = 1;
            for (int i = 1; i <= n; i++)
            {
                result = result * i;
            }

            return result;
        }
    }
}
