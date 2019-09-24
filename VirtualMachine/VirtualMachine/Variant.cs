using System;
using System.Runtime.InteropServices;

namespace VirtualMachine
{
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public class Variant
    {
        public static readonly ushort s_size = 8;
        
        public static Variant FromBytes(byte[] bytes, int pos)
        {
            return new Variant(BitConverter.ToDouble(bytes, pos));
        }

        [FieldOffset(0)]
        private double m_dValue;

        public Variant(double value = 0.0)
        {
            m_dValue = value;
        }

        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(m_dValue);
        }

        public override string ToString()
        {
            string str = "";
            str += m_dValue.ToString();
            return str;
        }

        public static Variant Parse(string str)
        {
            return new Variant(double.Parse(str));
        }

        public static Variant operator +(Variant op1, Variant op2)
        {
            return new Variant(op1.m_dValue + op2.m_dValue);
        }

        public static Variant operator -(Variant op1, Variant op2)
        {
            return new Variant(op1.m_dValue - op2.m_dValue);
        }

        public static bool operator >(Variant op1, Variant op2)
        {
            return op1.m_dValue > op2.m_dValue;
        }

        public static bool operator <(Variant op1, Variant op2)
        {
            return op1.m_dValue < op2.m_dValue;
        }

        public static bool operator >=(Variant op1, Variant op2)
        {
            return op1.m_dValue >= op2.m_dValue;
        }

        public static bool operator <=(Variant op1, Variant op2)
        {
            return op1.m_dValue < op2.m_dValue;
        }

        public static bool operator ==(Variant op1, Variant op2)
        {
            return op1.m_dValue == op2.m_dValue;
        }

        public static bool operator !=(Variant op1, Variant op2)
        {
            return op1.m_dValue != op2.m_dValue;
        }

        public static implicit operator Variant(double var)
        {
            return new Variant(var);
        }

        public static implicit operator double(Variant var)
        {
            return var.m_dValue;
        }

        public static implicit operator uint(Variant var)
        {
            return (uint)(var.m_dValue);
        }
    }
}
