using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL
{
    public interface CAESAR<T>
    {
        byte[] Cipher(byte[] bytes, T key);
        byte[] Decipher(byte[] bytes, T key);
    }

    public class caesar : CAESAR<string>
    {
        public byte[] Cipher(byte[] bytes, string key)
        {
            Dictionary<char, int> original = new Dictionary<char, int>();
            Dictionary<int, char> caesar = new Dictionary<int, char>();
            byte[] encrypted = new byte[bytes.Length];
            //abecedario caesar
            for (int i = 0; i < key.Length; i++)
            {
                if (!caesar.ContainsValue(key[i]))
                {
                    caesar.Add(caesar.Count, key[i]);
                }
            }
            for (int i = 0; i < 256; i++)
            {
                if (!caesar.ContainsValue(Convert.ToChar(i)))
                {
                    caesar.Add(caesar.Count, Convert.ToChar(i));
                }
            }
            //abecedario original
            for (int i = 0; i < 256; i++)
            {
                original.Add(Convert.ToChar(i), i);
            }
            //cifrado
            for (int i = 0; i < bytes.Length; i++)
            {
                encrypted[i] = (byte)caesar[Convert.ToChar(original[Convert.ToChar(bytes[i])])];
            }
            return encrypted;
        }
        public byte[] Decipher(byte[] bytes, string key)
        {
            Dictionary<int, char> original = new Dictionary<int, char>();
            Dictionary<char, int> caesar = new Dictionary<char, int>();
            byte[] decrypted = new byte[bytes.Length];
            //abecedario caesar
            for (int i = 0; i < key.Length; i++)
            {
                if (!caesar.ContainsKey(key[i]))
                {
                    caesar.Add(key[i], caesar.Count);
                }
            }
            for (int i = 0; i < 256; i++)
            {
                if (!caesar.ContainsKey(Convert.ToChar(i)))
                {
                    caesar.Add(Convert.ToChar(i), caesar.Count);
                }
            }
            //abecedario original
            for (int i = 0; i < 256; i++)
            {
                original.Add(i, Convert.ToChar(i));
            }
            //descifrado
            for (int i = 0; i < bytes.Length; i++)
            {
                decrypted[i] = (byte)caesar[Convert.ToChar(original[Convert.ToInt32(bytes[i])])];
            }
            return decrypted;
        }
    }
}
