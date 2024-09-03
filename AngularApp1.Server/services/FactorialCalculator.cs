using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

public static class FactorialCalculator
{
    // A cache to store already calculated factorials
    private static readonly ConcurrentDictionary<BigInteger, BigInteger> FactorialCache = new ConcurrentDictionary<BigInteger, BigInteger>();

    // Main method to calculate factorial using Prime Swing and caching
    public static BigInteger PrimeSwingFactorial(BigInteger n)
    {
        if (n < 2) return BigInteger.One;
        if (FactorialCache.ContainsKey(n)) return FactorialCache[n];

        BigInteger result = OptimizedRecFactorial(n);
        FactorialCache[n] = result;
        return result;
    }

    // Optimized recursive factorial calculation with parallel processing
    private static BigInteger OptimizedRecFactorial(BigInteger n)
    {
        if (n < 2) return BigInteger.One;

        // Using a parallelized swing method to compute the product
        var swings = ParallelSwing(n);
        BigInteger product = BigInteger.One;

        // Parallel multiplication of swings
        Parallel.ForEach(swings, swing =>
        {
            product *= swing;
        });

        // Recurse and square the half factorial, then multiply by the product
        var halfFactorial = OptimizedRecFactorial(n / 2);
        return BigInteger.Pow(halfFactorial, 2) * product;
    }

    // Swing function optimized for parallel processing
    private static List<BigInteger> ParallelSwing(BigInteger n)
    {
        var swings = new ConcurrentBag<BigInteger>();

        Parallel.For(2, (int)n + 1, p =>
        {
            if (IsPrime(p))
            {
                BigInteger q = p;
                while (q <= n)
                {
                    swings.Add(q);
                    q *= p;
                }
            }
        });

        return new List<BigInteger>(swings);
    }

    // Basic primality test optimized for large numbers
    private static bool IsPrime(BigInteger n)
    {
        if (n < 2) return false;
        if (n == 2 || n == 3) return true;
        if (n % 2 == 0 || n % 3 == 0) return false;

        BigInteger i = 5;
        BigInteger w = 2;

        while (i * i <= n)
        {
            if (n % i == 0) return false;
            i += w;
            w = 6 - w;
        }

        return true;
    }
}
