using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace VirtualMachine
{
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    unsafe public struct Variant
    {
        public const ushort c_null = 0x7FF0;

        public enum VarType : ushort
        {
            STR,
            ARR,

            NULL
        }

        [FieldOffset(0)]
        public double m_dValue;

        [FieldOffset(0)]
        public long m_lValue;

        [FieldOffset(0)]
        public ulong m_ulValue;

        [FieldOffset(0)]
        public uint m_uValue0;
        [FieldOffset(4)]
        public uint m_uValue1;

        [FieldOffset(0)]
        public int m_nValue0;
        [FieldOffset(4)]
        public int m_nValue1;

        [FieldOffset(0)]
        public ushort m_usValue0;
        [FieldOffset(2)]
        public ushort m_usValue1;
        [FieldOffset(4)]
        public ushort m_usValue2;
        [FieldOffset(6)]
        public ushort m_usValue3;

        [FieldOffset(0)]
        public byte m_bValue0;
        [FieldOffset(1)]
        public byte m_bValue1;
        [FieldOffset(2)]
        public byte m_bValue2;
        [FieldOffset(3)]
        public byte m_bValue3;
        [FieldOffset(4)]
        public byte m_bValue4;
        [FieldOffset(5)]
        public byte m_bValue5;
        [FieldOffset(6)]
        public byte m_bValue6;
        [FieldOffset(7)]
        public byte m_bValue7;

        [FieldOffset(8)]
        public void* m_pValue;
           
        public Variant(double val) : this()
        {
            m_pValue = null;
            m_dValue = val;
        }

        public Variant(string str) : this()
        {
            m_usValue0 = c_null;
            m_usValue1 = (ushort)VarType.STR;
            m_nValue1 = str.Length;
            fixed (char* cstr = str.ToCharArray())
            {
                m_pValue = (void*)cstr;
            }
        }

        public Variant(char[] str) : this()
        {
            m_usValue0 = c_null;
            m_usValue1 = (ushort)VarType.STR;
            m_nValue1 = str.Length;
            fixed (char* p = str)
            {
                m_pValue = (void*)p;
            }
        }

        public Variant(Variant[] arr) : this()
        {
            m_usValue0 = c_null;
            m_usValue1 = (ushort)VarType.ARR;
            m_nValue1 = arr.Length;
            fixed (Variant* parr = arr)
            {
                m_pValue = (void*)parr;
            }
        }

        public Variant(Variant var) : this()
        {
            m_pValue = var.m_pValue;
            m_dValue = var.m_dValue;
        }

        public static readonly Variant s_null = new Variant { m_usValue0 = c_null, m_usValue1 = (ushort)VarType.NULL, m_pValue = null };

        public static Variant Parse(string str)
        {
            if (str[0] == '\'')
            {
                if (str[str.Length - 1] != '\'')
                {
                    throw new Exception("string ending error");
                }

                return new Variant(str.Substring(1, str.Length - 1).Substring(0, str.Length - 2));
            }
            else if (str[0] == '\"')
            {
                if (str[str.Length - 1] != '\"')
                {
                    throw new Exception("string ending error");
                }

                return new Variant(str.Substring(1, str.Length - 1).Substring(0, str.Length - 2));
            }
            else if (str[0] == '{')
            {
                if (str[str.Length - 1] != '}')
                {
                    throw new Exception("list ending error");
                }

                List<Variant> vars = new List<Variant>();
                string[] strVars = str.Substring(1, str.Length - 1).Substring(0, str.Length - 2).Split('|');

                for (int i = 0; i < strVars.Length; ++i)
                {
                    vars.Add(Variant.Parse(strVars[i]));
                }

                return new Variant(vars.ToArray());
            }
            else
            {
                return new Variant(double.Parse(str));
            }

            return s_null;
        }

        public override string ToString()
        {
            if (m_usValue0 != c_null)
            {
                return m_dValue.ToString();
            }
            else if ((VarType)m_usValue1 == VarType.STR)
            {
                return new string((char*)m_pValue);
            }
            else if ((VarType)m_usValue1 == VarType.ARR)
            {
                string str = "";

                for (int i = 0; i < m_nValue1; ++i)
                {
                    str = str + ((Variant*)m_pValue)[i].ToString() + "|";
                }

                return str;
            }
            else if ((VarType)m_usValue1 == VarType.NULL)
            {
                return "NULL";
            }

            return null;
        }
        
        public byte[] ToBytes()
        {
            byte[] bytes;

            if (m_usValue0 == c_null && m_usValue1 != (ushort)VarType.NULL)
            {
                if (m_usValue1 == (ushort)VarType.STR)
                {
                    int strSize = sizeof(char) * m_nValue1;
                    bytes = new byte[sizeof(double) + strSize];
                    fixed (byte* p = bytes)
                    {
                        Buffer.MemoryCopy(m_pValue, (void*)((double*)p + 1), strSize, strSize);
                    }
                }
                else if (m_usValue1 == (ushort)VarType.ARR)
                {
                    int size = sizeof(double);
                    bytes = new byte[size];
                    for (int i = 0; i < m_nValue1; ++i)
                    {
                        byte[] varBytes = ((Variant*)m_pValue)[i].ToBytes();
                        byte[] buf = bytes;
                        bytes = new byte[size + varBytes.Length];
                        Buffer.BlockCopy(buf, 0, bytes, 0, size);
                        Buffer.BlockCopy(varBytes, 0, bytes, size, varBytes.Length);
                        size += varBytes.Length;
                    }
                }
            }
            else
            {
                bytes = new byte[sizeof(double)];
            }

            fixed (byte* p = bytes)
            {
                *((double*)p) = m_dValue;
            }

            return bytes;
        }

        public static Variant FromBytes(ref byte* ppc)
        {
            Variant var = new Variant();
            var.m_dValue = *((double*)ppc);
            ppc += sizeof(double);
            
            if (var.m_usValue0 == c_null)
            {
                if ((VarType)var.m_usValue1 == VarType.STR)
                {
                    char[] str = new char[var.m_nValue1];
                    fixed (char* p = str)
                    {
                        int strSize = var.m_nValue1 * sizeof(char);
                        Buffer.MemoryCopy(ppc, p, strSize, strSize);
                        ppc += strSize;
                        var.m_pValue = (void*)p;
                    }
                }
                else if ((VarType)var.m_usValue1 == VarType.ARR)
                {
                    Variant[] arr = new Variant[var.m_nValue1];

                    for (int i = 0; i < var.m_nValue1; ++i)
                    {
                        arr[i] = Variant.FromBytes(ref ppc);
                    }

                    fixed (Variant* p = arr)
                    {
                        var.m_pValue = (void*)p;
                    }
                }
                else
                {
                    var.m_pValue = null;
                }
            }
            else
            {
                var.m_pValue = null;
            }

            return var;
        }
    }
}
