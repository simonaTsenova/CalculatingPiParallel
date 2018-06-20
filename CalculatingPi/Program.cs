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
            var pIndex = Array.IndexOf(args, PRECISION_PARAM);
            var precision = pIndex >= 0 ? int.Parse(args[pIndex + 1]) : -1;

            var tIndex = Array.IndexOf(args, THREADS_PARAM);
            var threadsCount = tIndex >= 0 ? int.Parse(args[tIndex + 1]) : -1;

            var oIndex = Array.IndexOf(args, OUTPUT_FILE_PARAM);
            var outputFilename = oIndex >= 0 ? args[oIndex + 1] : "result.txt";

            var isQuiet = Array.IndexOf(args, QUIET_MODE_PARAM);

            // Calculate PI
            BigDecimal sum = 0;
            Parallel.For(0, precision, new ParallelOptions() { MaxDegreeOfParallelism = threadsCount }, i =>
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine("Thread-" + currentThreadId + " started.");
                BigDecimal numerator;
                if (i % 2 == 0)
                {
                    numerator = Factorial(4 * i) * (1123 + (21460 * i));
                }
                else
                {
                    numerator = -Factorial(4 * i) * (1123 + (21460 * i));
                }
                //Console.WriteLine("Num: " + numerator);

                BigDecimal denominator = (Factorial(i).Pow(4)).Multiply((new BigDecimal(14112)).Pow(2 * i));
                //Console.WriteLine("Denom: " + denominator);

                BigDecimal currentAddition = numerator / denominator;

                sum = sum.Add(currentAddition);
                Console.WriteLine("Thread-" + currentThreadId + " stopped.");
            });

            BigDecimal constant = new BigDecimal((double)1 / 3528);
            BigDecimal opposite = constant * sum;
            
            //Console.WriteLine("sum: " + sum);
            BigDecimal pi = new BigDecimal((double)1 / opposite.ToDouble());
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
