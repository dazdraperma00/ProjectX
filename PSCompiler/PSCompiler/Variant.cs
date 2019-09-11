using System;
using System.Runtime.InteropServices;

namespace PSCompiler
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public class Variant
    {
        public static readonly ushort size = 8;

        [FieldOffset(0)]
        private double value;

        public Variant(double _value = 0.0)
        {
            this.value = _value;
        }

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(value);
        }

        public override string ToString()
        {
            string str = "";
            str += value.ToString();
            return str;
        }

        public static Variant operator +(Variant op1, Variant op2)
        {
            return new Variant(op1.value + op2.value);
        }

        public static Variant operator -(Variant op1, Variant op2)
        {
            return new Variant(op1.value - op2.value);
        }

        public static bool operator >(Variant op1, Variant op2)
        {
            return op1.value > op2.value;
        }

        public static bool operator <(Variant op1, Variant op2)
        {
            return op1.value < op2.value;
        }

        public static bool operator ==(Variant op1, Variant op2)
        {
            return op1.value == op2.value;
        }

        public static bool operator !=(Variant op1, Variant op2)
        {
            return op1.value != op2.value;
        }

        public static implicit operator Variant(double var)
        {
            return new Variant(var);
        }
    }
}
