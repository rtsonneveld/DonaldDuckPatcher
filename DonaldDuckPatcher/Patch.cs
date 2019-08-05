using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DonaldDuckPatcher
{
    public class Patch
    {
        public string Name;
        public int Offset;
        public byte[] OriginalBytes;
        public byte[] PatchedBytes;
    }
}
