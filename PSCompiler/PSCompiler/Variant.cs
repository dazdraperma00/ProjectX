using System;
using System.Runtime.InteropServices;

namespace PSCompiler
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    class Variant
    {
        [FieldOffset(0)]
        private double value;

        public Variant(double _value = 0.0)
        {
            this.value = _value;
        }
    }
}
