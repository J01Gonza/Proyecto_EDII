using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL
{
    public interface ILZW
    {
        public byte[] Compress(byte[] bytes);
        public byte[] Decompress(byte[] bytes);
    }
    public class lzw : ILZW
    {
        public byte[] Compress(byte[] bytes)
        {
            int byteQty;
            int origDict;
            List<int> comp = new List<int>();
            List<byte> output = new List<byte>();
            Dictionary<string, int> Legend = new Dictionary<string, int>();
            for (int i = 0; i < bytes.Length; i++)
            {
                if (!Legend.ContainsKey(((char)bytes[i]).ToString()))
                {
                    Legend.Add(((char)bytes[i]).ToString(), Legend.Count + 1);
                }
            }
            origDict = Legend.Count;
            string w = "";
            foreach (char k in bytes)
            {
                string wk = w + k;
                if (Legend.ContainsKey(wk))
                {
                    w = wk;
                }
                else
                {
                    comp.Add(Legend[w]);
                    Legend.Add(wk, Legend.Count + 1);
                    w = k.ToString();
                }
            }
            if (!string.IsNullOrEmpty(w))
            {
                comp.Add(Legend[w]);
            }
            byteQty = Convert.ToInt32((uint)Math.Log(Legend.Count, 2.0) + 1);
            output.Add((byte)byteQty);
            output.Add((byte)origDict);
            int c = 0;
            foreach (var item in Legend.Keys)
            {
                if (origDict > c)
                {
                    output.Add((byte)item[0]);
                    c++;
                }
                else
                {
                    break;
                }
            }
            string codes = "";
            foreach (var item in comp)
            {
                codes += DecBin(item, byteQty);
            }
            string bfin = "";
            for (int i = 0; i < codes.Length; i++)
            {
                if (bfin.Length < 8)
                {
                    bfin += codes[i];
                }
                else
                {
                    long B = Convert.ToInt32(bfin);
                    output.Add((byte)BinDec(B));
                    bfin = codes[i].ToString();
                }
            }
            if (!string.IsNullOrEmpty(bfin))
            {
                while (bfin.Length < 8)
                {
                    bfin += 0;
                }
                long B = Convert.ToInt32(bfin);
                output.Add((byte)BinDec(B));
            }
            return output.ToArray();
        }

        public byte[] Decompress(byte[] bytes)
        {
            int bytesQty = Convert.ToInt32(bytes[0]);
            int origDict = Convert.ToInt32(bytes[1]);
            string codes = string.Empty;
            string Out = string.Empty;
            List<int> comp = new List<int>();
            List<byte> output = new List<byte>();
            Dictionary<int, string> Legend = new Dictionary<int, string>();
            for (int i = 0; i < origDict; i++)
            {
                Legend.Add(Legend.Count + 1, ((char)bytes[2 + i]).ToString());
            }
            for (int i = 2 + origDict; i < bytes.Length; i++)
            {
                codes += DecBin(bytes[i], 8);
            }
            while (!string.IsNullOrEmpty(codes) && codes.Length >= bytesQty)
            {
                comp.Add(Convert.ToInt32(codes.Substring(0, bytesQty), 2));
                codes = codes.Substring(bytesQty);
            }
            string w = Legend[comp[0]];
            int p = comp[0];
            comp.RemoveAt(0);
            Out += w;
            string k = "";
            foreach (var item in comp)
            {
                if (item != 0)
                {
                    if (!Legend.ContainsKey(item))
                    {
                        k = Legend[p];
                        k = k + w;
                    }
                    else
                    {
                        k = Legend[item];
                    }
                    Out += k;
                    w = k.Substring(0, 1);
                    Legend.Add(Legend.Count + 1, Legend[p] + w);
                    p = item;
                }
                else
                {
                    break;
                }
            }
            foreach (var item in Out)
            {
                output.Add((byte)item);
            }
            return output.ToArray();
        }

        string DecBin(int decim, int m)
        {
            long bin = 0;
            const int div = 2;
            long digit = 0;
            for (int i = decim % div, j = 0; decim > 0; decim /= div, i = decim % div, j++)
            {
                digit = i % div;
                bin += digit * (long)Math.Pow(10, j);
            }
            string Bin = bin.ToString();
            while (Bin.Length < m)
            {
                Bin = "0" + Bin;
            }
            return Bin;
        }

        int BinDec(long binary)
        {
            int decim = 0;
            int digit = 0;
            const int div = 10;

            for (long i = binary, j = 0; i > 0; i /= div, j++)
            {
                digit = (int)i % div;
                if (digit != 1 && digit != 0)
                {
                    return -1;
                }
                decim += digit * (int)Math.Pow(2, j);
            }
            return decim;
        }

    }
}