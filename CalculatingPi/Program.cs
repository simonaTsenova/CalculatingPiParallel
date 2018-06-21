using Deveel.Math;
using System;
using System.Diagnostics;
using System.IO;
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
        const string DEFAULT_FILENAME = "result.txt";

        static void Main(string[] args)
        {
            // Parse parameters
            var pIndex = Array.IndexOf(args, PRECISION_PARAM);
            var precision = pIndex >= 0 ? int.Parse(args[pIndex + 1]) : -1;

            var tIndex = Array.IndexOf(args, THREADS_PARAM);
            var threadsCount = tIndex >= 0 ? int.Parse(args[tIndex + 1]) : -1;

            var oIndex = Array.IndexOf(args, OUTPUT_FILE_PARAM);
            var outputFilename = oIndex >= 0 ? args[oIndex + 1] : DEFAULT_FILENAME;

            var isQuiet = Array.IndexOf(args, QUIET_MODE_PARAM);

            // Calculate PI
            BigDecimal sum = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Parallel.For(0, precision, new ParallelOptions() { MaxDegreeOfParallelism = threadsCount }, i =>
            {
                var currentThreadId = Thread.CurrentThread.ManagedThreadId;
                if (isQuiet < 0)
                {
                    Console.WriteLine("Thread-" + currentThreadId + " started.");
                }

                BigDecimal numerator;
                if (i % 2 == 0)
                {
                    numerator = MultiplyRange(i + 1, 4 * i) * (1123 + (21460 * i));
                }
                else
                {
                    numerator = -MultiplyRange(i + 1, 4 * i) * (1123 + (21460 * i));
                }

                BigDecimal denominator = (Factorial(i).Pow(3)).Multiply((new BigDecimal(14112)).Pow(2 * i));

                BigDecimal currentAddition = numerator / denominator;

                sum = sum.Add(currentAddition);
                if (isQuiet < 0)
                {
                    Console.WriteLine("Thread-" + currentThreadId + " stopped.");
                }
            });

            BigDecimal constant = new BigDecimal((double)1 / 3528);
            BigDecimal opposite = constant.Multiply(sum);
            BigDecimal pi = new BigDecimal(1 / opposite.ToDouble());

            stopwatch.Stop();

            if (isQuiet < 0)
            {
                Console.WriteLine("Pi is: " + pi);
            }

            Console.WriteLine("Threads used in current execution: " + threadsCount);
            var totalExecutionTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Total execution time with precision of " + precision + " terms in the series: " + totalExecutionTime + " ms");

            // ------------------------------------------------
            // Optimized
            BigDecimal sum2 = 0;
            var stopwatch2 = new Stopwatch();
            stopwatch2.Start();
            var factorials = CalculateFactorials(4 * precision);

            Parallel.For(0, precision, new ParallelOptions() { MaxDegreeOfParallelism = threadsCount }, i =>
            {
                BigDecimal numerator;
                if (i % 2 == 0)
                {
                    numerator = (factorials[4 * i] / factorials[i]) * (1123 + (21460 * i));
                }
                else
                {
                    numerator = -(factorials[4 * i] / factorials[i]) * (1123 + (21460 * i));
                }

                BigDecimal denominator = (factorials[i].Pow(3)).Multiply((new BigDecimal(14112)).Pow(2 * i));

                BigDecimal currentAddition = numerator / denominator;

                sum2 = sum2.Add(currentAddition);
            });

            BigDecimal constant2 = new BigDecimal((double)1 / 3528);
            BigDecimal opposite2 = constant2.Multiply(sum2);
            BigDecimal pi2 = new BigDecimal(1 / opposite2.ToDouble());
            stopwatch2.Stop();

            Console.WriteLine("Pi: " + pi2);
            Console.WriteLine("Threads used in current execution: " + threadsCount);
            var totalExecutionTime2 = stopwatch2.ElapsedMilliseconds;
            Console.WriteLine("Total execution time with precision of " + precision + " terms in the series: " + totalExecutionTime2 + " ms");

            // Write result to file
            //using (StreamWriter writer = new StreamWriter(outputFilename))
            //{
            //    writer.WriteLine("Pi is: " + pi);
            //    writer.WriteLine("Total execution time with " + threadsCount + " threads and precision " + precision + ": " + totalExecutionTime + " ms");
            //}
        }

        static BigInteger MultiplyRange(int start, int end)
        {
            BigInteger result = 1;
            for (int i = start; i <= end; i++)
            {
                result = result * i;
            }

            return result;
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

        static BigInteger[] CalculateFactorials(int n)
        {
            BigInteger[] factorials = new BigInteger[n+1];
            factorials[0] = 1;
            for (int i = 1; i <= n; i++)
            {
                factorials[i] = factorials[i - 1] * i;
            }

            return factorials;
        }
    }
}
