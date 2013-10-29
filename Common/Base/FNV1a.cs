using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Base
{
    public class FNV1a
    {
        public static int Hash32(int value)
        {
            uint hash = 2166136261;
            byte[] data = BitConverter.GetBytes(value);
            foreach (byte b in data)
            {
                hash = hash ^ b;
                hash *= 16777619;
            }
            return BitConverter.ToInt32(BitConverter.GetBytes(hash),0);
        }
    }
}
