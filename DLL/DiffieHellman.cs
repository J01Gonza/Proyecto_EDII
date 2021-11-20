using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace DLL
{
    public interface DiffieHellman
    {
        int secretKey(int key1, int key2, int g, int p);
        int gBase(int p);
        int pNumber();
    }

    public class diffiehellman : DiffieHellman
    {
        public int secretKey(int key1, int key2, int g, int p)
        {
            key1 = key1 % 256;
            key2 = key2 % 256;
            return (int)BigInteger.ModPow(g, key1 * key2, p);
        }

        public int gBase(int p)
        {
            Random random = new Random();
            int g;
            do
            {
                g = random.Next(2, 255);
            } while (g > p);
            return g;
        }

        public int pNumber()
        {
            Random random = new Random();
            int p;
            do
            {
                p = random.Next(2, 255);
            } while (!isPrime(p));
            return p;
        }

        public bool isPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;
            var boundary = (int)Math.Floor(Math.Sqrt(number));
            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
