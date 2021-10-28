using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL
{
    public interface SDES
    {
        (string key1, string key2) generateKey(string mainKey, int[] P10, int[] P8);
        byte Enconde(string mainKey, string key1, string key2, int[] P4, int[] EP, int[] IP, int[] IP1);
    }

    public class ImplementationClass : SDES
    {
        public (string key1, string key2) generateKey(string mainKey, int[] P10, int[] P8)
        {
            string key1 = "", key2 = "";
            char[] key = mainKey.ToArray();
            char[] pKey = new char[key.Length];
            char[] left, right;
            for (int i = 0; i < P10.Length; i++)
            {
                pKey[i] = key[P10[i] - 1];
            }
            left = leftShift(pKey.Take((pKey.Length + 1) / 2).ToArray());
            right = leftShift(pKey.Skip((pKey.Length + 1) / 2).ToArray());
            pKey = left.Concat(right).ToArray();
            for (int i = 0; i < P8.Length; i++)
            {
                key1 += pKey[P8[i] - 1];
            }
            left = leftShift(leftShift(left));
            right = leftShift(leftShift(right));
            pKey = left.Concat(right).ToArray();
            for (int i = 0; i < P8.Length; i++)
            {
                key2 += pKey[P8[i] - 1];
            }
            return (key1, key2);
        }

        public byte Enconde(string mainKey, string key1, string key2, int[] P4, int[] EP, int[] IP, int[] IP1)
        {
            string encode = "";
            char[] key = mainKey.ToArray();
            char[] k1 = key1.ToArray();
            char[] k2 = key2.ToArray();
            char[] pKey = new char[key.Length];
            char[] p4 = new char[P4.Length];
            char[] L, R, l, r, s;
            string[,] S0 =
{
                { "01" , "00" , "11" , "10" },
                { "11" , "10" , "01" , "00" },
                { "00" , "10" , "01" , "11" },
                { "11" , "01" , "11" , "10" }
            };
            string[,] S1 =
            {
                { "00" , "01" , "10" , "11" },
                { "10" , "00" , "01" , "11" },
                { "11" , "00" , "01" , "00" },
                { "10" , "01" , "00" , "11" }
            };
            for (int i = 0; i < IP.Length; i++)
            {
                pKey[i] = key[IP[i] - 1];
            }
            L = pKey.Take((pKey.Length + 1) / 2).ToArray();
            R = pKey.Skip((pKey.Length + 1) / 2).ToArray();
            for (int i = 0; i < EP.Length; i++)
            {
                pKey[i] = R[EP[i] - 1];
            }
            for (int i = 0; i < k1.Length; i++)
            {
                if (pKey[i] == k1[i])
                {
                    pKey[i] = '0';
                }
                else
                {
                    pKey[i] = '1';
                }
            }
            l = pKey.Take((pKey.Length + 1) / 2).ToArray();
            r = pKey.Skip((pKey.Length + 1) / 2).ToArray();
            s = (S0[Convert.ToInt32(l[0].ToString() + l[3].ToString(), 2), Convert.ToInt32(l[1].ToString() + l[2].ToString(), 2)] + S1[Convert.ToInt32(r[0].ToString() + r[3].ToString(), 2), Convert.ToInt32(r[1].ToString() + r[2].ToString(), 2)]).ToArray();
            for (int i = 0; i < P4.Length; i++)
            {
                p4[i] = s[P4[i] - 1];
            }
            for (int i = 0; i < L.Length; i++)
            {
                if (p4[i] == L[i])
                {
                    p4[i] = '0';
                }
                else
                {
                    p4[i] = '1';
                }
            }
            pKey = R.Concat(p4).ToArray();
            L = pKey.Take((pKey.Length + 1) / 2).ToArray();
            R = pKey.Skip((pKey.Length + 1) / 2).ToArray();
            for (int i = 0; i < EP.Length; i++)
            {
                pKey[i] = R[EP[i] - 1];
            }
            for (int i = 0; i < k2.Length; i++)
            {
                if (pKey[i] == k2[i])
                {
                    pKey[i] = '0';
                }
                else
                {
                    pKey[i] = '1';
                }
            }
            l = pKey.Take((pKey.Length + 1) / 2).ToArray();
            r = pKey.Skip((pKey.Length + 1) / 2).ToArray();
            s = (S0[Convert.ToInt32(l[0].ToString() + l[3].ToString(), 2), Convert.ToInt32(l[1].ToString() + l[2].ToString(), 2)] + S1[Convert.ToInt32(r[0].ToString() + r[3].ToString(), 2), Convert.ToInt32(r[1].ToString() + r[2].ToString(), 2)]).ToArray();
            for (int i = 0; i < P4.Length; i++)
            {
                p4[i] = s[P4[i] - 1];
            }
            for (int i = 0; i < L.Length; i++)
            {
                if (p4[i] == L[i])
                {
                    p4[i] = '0';
                }
                else
                {
                    p4[i] = '1';
                }
            }
            pKey = p4.Concat(R).ToArray();
            for (int i = 0; i < IP1.Length; i++)
            {
                key[i] = pKey[IP1[i] - 1];
            }
            foreach (var item in key)
            {
                encode += item;
            }
            return Convert.ToByte(encode, 2);
        }

        static char[] leftShift(char[] array)
        {
            int size = array.Length;
            char[] shift = new char[size];
            for (int i = 0; i < size; i++)
            {
                shift[i] = array[(i + 1) % size];
            }
            return shift;
        }
    }
}