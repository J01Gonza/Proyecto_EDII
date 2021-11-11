using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace DLL
{
    public interface RSA
    {
        List<byte> Cipher(byte[] code, int key, int key2);
        List<byte> Decipher(byte[] allbytes, int key, int key2);
    }

    public class rsa : RSA
    {
        public List<byte> Cipher(byte[] code, int n, int k)
        {
            List<byte> outb = new List<byte>();
            int qtybytes = 0;
            if (n > 255)
            {
                int pos = 0;
                int copia = n;
                while (copia > 0)
                {
                    copia -= 255;
                    qtybytes++;
                }
                while (pos < code.Length)
                {
                    var concat = outb.Concat(CipherM(code[pos], n, k, qtybytes));
                    pos++;
                    outb = concat.ToList();
                }
                return outb;
            }
            else
            {
                int pos = 0;
                int copia = 255;
                while (copia > 0)
                {
                    copia -= n;
                    qtybytes++;
                }
                while (pos < code.Length)
                {
                    var concat2 = outb.Concat(Cipherm(code[pos], n, k, qtybytes));
                    outb = concat2.ToList();
                    pos++;
                }
                return outb;
            }

        }
        List<byte> CipherM(byte code, int n, int k, int qtybytes)
        {
            List<byte> outb = new List<byte>();
            int i = (int)BigInteger.ModPow(code, k, n);
            if (i > 255)
            {
                while (i > 255)
                {
                    outb.Add(Convert.ToByte(255));
                    i -= 255;
                }
            }
            outb.Add((byte)i);
            while (outb.Count != qtybytes)
            {
                outb.Add(0);
            }
            return outb;
        }

        List<byte> Cipherm(byte code, int n, int k, int qtybytes)
        {
            int i = 0;
            List<byte> outb = new List<byte>();
            if (code > n - 1)
            {
                i = (int)BigInteger.ModPow(n - 1, k, n);
                outb.Add((byte)i);
                int c = Convert.ToInt32(code);
                c -= n - 1;
                while (c > n)
                {
                    i = (int)BigInteger.ModPow(n - 1, k, n);
                    outb.Add((byte)i);
                    c -= n - 1;
                }
                int x = (int)BigInteger.ModPow(c, k, n);
                outb.Add((byte)x);
            }
            else
            {
                i = (int)BigInteger.ModPow(code, k, n);
                outb.Add((byte)i);
            }
            while (outb.Count != qtybytes)
            {
                outb.Add(0);
            }
            return outb;
        }
        public List<byte> Decipher(byte[] allbytes, int n, int k)
        {
            List<byte> outb = new List<byte>();
            int pos = 0;
            int qtybytes = 0;
            if (n > 255)
            {
                int copia = n;
                while (copia > 0)
                {
                    qtybytes++;
                    copia -= 255;
                }
                while (pos < allbytes.Length)
                {
                    int cont = 0;
                    int descifrado = 0;
                    int descifrar = 0;
                    while (cont < qtybytes)
                    {
                        descifrar += Convert.ToInt32(allbytes[pos]);
                        pos++;
                        cont++;
                    }
                    descifrado += (int)BigInteger.ModPow(descifrar, k, n);
                    outb.Add(Convert.ToByte(descifrado));
                }
            }
            else
            {
                int copia = 255;
                while (copia > 0)
                {
                    qtybytes++;
                    copia -= n;
                }
                while (pos < allbytes.Length)
                {
                    int cont = 0;
                    int descifrado = 0;
                    while (cont < qtybytes)
                    {
                        descifrado += (int)BigInteger.ModPow(allbytes[pos], k, n);
                        pos++;
                        cont++;
                    }
                    outb.Add(Convert.ToByte(descifrado));
                }
            }
            return outb;
        }

        public (int n, int e, int d) generarLlaves(int p, int q)
        {
            if (primo(p) && primo(q) && p <= 512 && q <= 512)
            {
                int n = p * q;
                int phi = (p - 1) * (q - 1);
                int E = 2;
                int alpha = 1;
                int theta = 1;
                while (alpha == 1 || theta == 1)
                {
                    while (E < phi)
                    {
                        if (primo(E) && MCD(E, phi) == 1)
                        {
                            if (E != p && E != q) break;
                            else E++;
                        }
                        else
                        {
                            E++;
                        }
                    }
                    alpha = phi - (phi / E) * E;
                    theta = E - (E / alpha) * alpha;
                    if (alpha == 1 || theta == 1) E++;
                }
                int beta = phi - (phi / E);
                int omega = 1 - (E / alpha) * beta;
                while (omega < 0) omega += phi;
                int ro = 0;
                int lambda = 2;
                while (lambda > 1)
                {
                    lambda = alpha - (alpha / theta) * theta;
                    ro = beta - (alpha / theta) * omega;
                    while (ro < 0) ro += phi;
                    if (lambda > 1)
                    {
                        alpha = theta;
                        theta = lambda;
                        beta = omega;
                        omega = ro;
                    }
                }
                return (n, E, ro);
            }
            else
            {
                throw new Exception();
            }
        }
        bool primo(int primo)
        {
            for (int i = 2; i < primo; i++)
            {
                if ((primo % i) == 0)
                {
                    return false;
                }
            }
            return true;
        }
        int MCD(int m, int n)
        {
            int a = Math.Max(m, n);
            int b = Math.Min(m, n);
            int mcd = 0;
            do
            {
                mcd = b;
                b = a % b;
                a = mcd;
            } while (b != 0);
            return mcd;
        }
    }
}
