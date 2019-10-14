using System;
using System.Text;
using System.Collections.Generic;

namespace VirtualMachine
{
    public abstract class AVariant
    {
        protected enum VarType : byte
        {
            NUM,
            STRING,
            LIST,

            NONE
        }

        protected static byte[] ConcatBytes(params byte[][] ops)
        {
            List<byte> bytes = new List<byte>();

            for (int i = 0; i < ops.Length; ++i)
            {
                for (int j = 0; j < ops[i].Length; ++j)
                {
                    bytes.Add(ops[i][j]);
                }
            }

            return bytes.ToArray();
        }

        public static AVariant FromBytes(byte[] bytes, ref int pos)
        {
            AVariant var = null;

            VarType bType = (VarType)bytes[pos++];
            switch(bType)
            {
                case VarType.NUM:
                    var = NVariant.FromBytes(bytes, ref pos);
                    break;
                case VarType.STRING:
                    var = SVariant.FromBytes(bytes, ref pos);
                    break;
                case VarType.LIST:
                    var = LVariant.FromBytes(bytes, ref pos);
                    break;
            }

            return var;
        }

        public static AVariant Parse(string str)
        {
            if (str[0] == '\'')
            {
                if (str[str.Length - 1] != '\'')
                {
                    throw new Exception("string ending error");
                }

                return new SVariant(str.Substring(1, str.Length - 1).Substring(0, str.Length - 2));
            }
            else if (str[0] == '\"')
            {
                if (str[str.Length - 1] != '\"')
                {
                    throw new Exception("string ending error");
                }

                return new SVariant(str.Substring(1, str.Length - 1).Substring(0, str.Length - 2));
            }
            else if (str[0] == '{')
            {
                if (str[str.Length - 1] != '}')
                {
                    throw new Exception("list ending error");
                }

                List<AVariant> vars = new List<AVariant>();
                string[] strVars = str.Substring(1, str.Length - 1).Substring(0, str.Length - 2).Split(',');

                for (int i = 0; i < strVars.Length; ++i)
                {
                    vars.Add(AVariant.Parse(strVars[i]));
                }

                return new LVariant(vars);
            }
            else
            {
                return new NVariant(double.Parse(str));
            }

            return null;
        }

        public abstract byte[] ToBytes();
        public abstract int GetSize();

        public static implicit operator AVariant(int var)
        {
            return new NVariant(var);
        }

        public static implicit operator AVariant(double var)
        {
            return new NVariant(var);
        }

        public static implicit operator AVariant(bool var)
        {
            return new NVariant(var ? 1.0 : 0.0);
        }

        public static implicit operator int(AVariant var)
        {
            return (int)((NVariant)var);
        }

        public static implicit operator double(AVariant var)
        {
            return (double)((NVariant)var);
        }

        public static implicit operator string(AVariant var)
        {
            return (string)((SVariant)var);
        }

        public static implicit operator AVariant(string var)
        {
            return new SVariant(var);
        }

        public static AVariant operator +(AVariant op1, AVariant op2)
        {
            if (op1 is NVariant && op2 is NVariant)
            {
                return (NVariant)op1 + (NVariant)op2;
            }
            else if (op1 is SVariant && op2 is SVariant)
            {
                return (SVariant)op1 + (SVariant)op2;
            }
            else if (op1 is LVariant && op2 is LVariant)
            {
                return (LVariant)op1 + (LVariant)op2;
            }
            else
            {
                return null;
            }
        }

        public static AVariant operator -(AVariant op1, AVariant op2)
        {
            if (op1 is NVariant && op2 is NVariant)
            {
                return (NVariant)op1 - (NVariant)op2;
            }
            else if (op1 is SVariant && op2 is SVariant)
            {
                return (SVariant)op1 - (SVariant)op2;
            }
            else if (op1 is LVariant && op2 is LVariant)
            {
                return (LVariant)op1 - (LVariant)op2;
            }
            else
            {
                return null;
            }
        }

        public static bool operator <(AVariant op1, AVariant op2)
        {
            if (op1 is NVariant && op2 is NVariant)
            {
                return (NVariant)op1 < (NVariant)op2;
            }
            else if (op1 is SVariant && op2 is SVariant)
            {
                return (SVariant)op1 < (SVariant)op2;
            }
            else if (op1 is LVariant && op2 is LVariant)
            {
                return (LVariant)op1 < (LVariant)op2;
            }
            else
            {
                return false;
            }
        }

        public static bool operator >(AVariant op1, AVariant op2)
        {
            if (op1 is NVariant && op2 is NVariant)
            {
                return (NVariant)op1 > (NVariant)op2;
            }
            else if (op1 is SVariant && op2 is SVariant)
            {
                return (SVariant)op1 > (SVariant)op2;
            }
            else if (op1 is LVariant && op2 is LVariant)
            {
                return (LVariant)op1 > (LVariant)op2;
            }
            else
            {
                return false;
            }
        }

        public static bool operator <=(AVariant op1, AVariant op2)
        {
            if (op1 is NVariant && op2 is NVariant)
            {
                return (NVariant)op1 <= (NVariant)op2;
            }
            else if (op1 is SVariant && op2 is SVariant)
            {
                return (SVariant)op1 <= (SVariant)op2;
            }
            else if (op1 is LVariant && op2 is LVariant)
            {
                return (LVariant)op2 <= (LVariant)op2;
            }
            else
            {
                return false;
            }
        }

        public static bool operator >=(AVariant op1, AVariant op2)
        {
            if (op1 is NVariant && op2 is NVariant)
            {
                return (NVariant)op1 >= (NVariant)op2;
            }
            else if (op1 is SVariant && op2 is SVariant)
            {
                return (SVariant)op1 >= (SVariant)op2;
            }
            else if (op1 is LVariant && op2 is LVariant)
            {
                return (LVariant)op2 >= (LVariant)op2;
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(AVariant op1, AVariant op2)
        {
            if (op1 is NVariant && op2 is NVariant)
            {
                return (NVariant)op1 == (NVariant)op2;
            }
            else if (op1 is SVariant && op2 is SVariant)
            {
                return (SVariant)op1 == (SVariant)op2;
            }
            else if (op1 is LVariant && op2 is LVariant)
            {
                return (LVariant)op1 == (LVariant)op2;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(AVariant op1, AVariant op2)
        {
            if (op1 is NVariant && op2 is NVariant)
            {
                return (NVariant)op1 != (NVariant)op2;
            }
            else if (op1 is SVariant && op2 is SVariant)
            {
                return (SVariant)op1 != (SVariant)op2;
            }
            else if (op1 is LVariant & op2 is LVariant)
            {
                return (LVariant)op1 != (LVariant)op2;
            }
            else
            {
                return false;
            }
        }
    }

    public class NVariant : AVariant
    {      
        private double m_dValue;

        public NVariant(double value = 0.0)
        {
            m_dValue = value;
        }

        public static NVariant FromBytes(byte[] bytes, ref int pos)
        {
            NVariant var = new NVariant(BitConverter.ToDouble(bytes, pos));
            pos += sizeof(double);

            return var;
        }

        public override byte[] ToBytes()
        {
            byte[] b = { (byte)VarType.NUM };
            return ConcatBytes(b, BitConverter.GetBytes(m_dValue));
        }

        public override int GetSize()
        {
            return sizeof(double);
        }

        public override string ToString()
        {
            string str = "";
            str += m_dValue.ToString();
            return str;
        }

        public override bool Equals(object obj)
        {
            if (obj is NVariant)
            {
                return ((NVariant)obj).m_dValue == m_dValue; 
            }

            return false;
        }

        public static NVariant operator +(NVariant op1, NVariant op2)
        {
            return new NVariant(op1.m_dValue + op2.m_dValue);
        }

        public static NVariant operator -(NVariant op1, NVariant op2)
        {
            return new NVariant(op1.m_dValue - op2.m_dValue);
        }

        public static bool operator >(NVariant op1, NVariant op2)
        {
            return op1.m_dValue > op2.m_dValue;
        }

        public static bool operator <(NVariant op1, NVariant op2)
        {
            return op1.m_dValue < op2.m_dValue;
        }

        public static bool operator >=(NVariant op1, NVariant op2)
        {
            return op1.m_dValue >= op2.m_dValue;
        }

        public static bool operator <=(NVariant op1, NVariant op2)
        {
            return op1.m_dValue < op2.m_dValue;
        }

        public static bool operator ==(NVariant op1, NVariant op2)
        {
            return op1.m_dValue == op2.m_dValue;
        }

        public static bool operator !=(NVariant op1, NVariant op2)
        {
            return op1.m_dValue != op2.m_dValue;
        }

        public static implicit operator int(NVariant var)
        {
            return (int)var.m_dValue;
        }

        public static implicit operator double(NVariant var)
        {
            return var.m_dValue;
        }
    }

    public class SVariant : AVariant
    {
        private string m_sValue;

        public SVariant(string value)
        {
            m_sValue = value;
        }

        public static SVariant FromBytes(byte[] bytes, ref int pos)
        {
            int size = BitConverter.ToInt32(bytes, pos);
            pos += sizeof(int);

            SVariant var = new SVariant(Encoding.ASCII.GetString(bytes, pos, size));
            pos += size;

            return var;
        }

        public override byte[] ToBytes()
        {
            byte[] b = { (byte)VarType.STRING };
            return ConcatBytes(b, BitConverter.GetBytes(m_sValue.Length), 
                Encoding.ASCII.GetBytes(m_sValue));
        }
        
        public override int GetSize()
        {
            return Encoding.ASCII.GetByteCount(m_sValue);
        }

        public override string ToString()
        {
            return m_sValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is SVariant)
            {
                return ((SVariant)obj).m_sValue == m_sValue;
            }

            return false;
        }

        public static SVariant operator +(SVariant op1, SVariant op2)
        {
            return new SVariant(op1.m_sValue + op2.m_sValue);
        }

        public static SVariant operator -(SVariant op1, SVariant op2)
        {
            string res = op1.m_sValue;

            if (res.Contains(op2.m_sValue))
            {
                res = res.Replace(op2.m_sValue, "");
            }

            return new SVariant(res);
        }

        public static bool operator <(SVariant op1, SVariant op2)
        {
            return op1.m_sValue.Length < op2.m_sValue.Length;
        }

        public static bool operator >(SVariant op1, SVariant op2)
        {
            return op1.m_sValue.Length > op2.m_sValue.Length;
        }

        public static bool operator <=(SVariant op1, SVariant op2)
        {
            return op1.m_sValue.Length <= op2.m_sValue.Length;
        }

        public static bool operator >=(SVariant op1, SVariant op2)
        {
            return op1.m_sValue.Length >= op2.m_sValue.Length;
        }

        public static bool operator ==(SVariant op1, SVariant op2)
        {
            return op1.m_sValue == op2.m_sValue;
        }

        public static bool operator !=(SVariant op1, SVariant op2)
        {
            return op1.m_sValue != op2.m_sValue;
        }

        public static implicit operator string(SVariant var)
        {
            return var.m_sValue;
        }
    }

    public class LVariant : AVariant
    {
        private List<AVariant> m_lValue;

        public LVariant(List<AVariant> value)
        {
            m_lValue = value;
        }

        public static LVariant FromBytes(byte[] bytes, ref int pos)
        {
            int size = BitConverter.ToInt32(bytes, pos);
            pos += sizeof(int);

            List<AVariant> var = new List<AVariant>();

            for (int i = 0; i < size; ++i)
            {
                var.Add(AVariant.FromBytes(bytes, ref pos));
            }

            return new LVariant(var);
        }

        public override byte[] ToBytes()
        {
            byte[] b = { (byte)VarType.LIST };
            byte[] bytes = ConcatBytes(b, BitConverter.GetBytes(m_lValue.Count));

            for (int i = 0; i < m_lValue.Count; ++i)
            {
                bytes = ConcatBytes(bytes, m_lValue[i].ToBytes());
            }

            return bytes;
        }

        public override int GetSize()
        {
            int size = 0;

            for (int i = 0; i < m_lValue.Count; ++i)
            {
                size += m_lValue[i].GetSize();
            }

            return size;
        }

        public override string ToString()
        {
            string str = "";

            for (int i = 0; i < m_lValue.Count; ++i)
            {
                str = str + m_lValue[i].ToString() + " ";
            }

            return str;
        }

        public override bool Equals(object obj)
        {
            if (obj is LVariant)
            {
                if (((LVariant)obj).m_lValue.Count == m_lValue.Count)
                {
                    for (int i = 0; i < m_lValue.Count; ++i)
                    {
                        if (!((LVariant)obj).m_lValue[i].Equals(m_lValue[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public static LVariant operator +(LVariant op1, LVariant op2)
        {
            List<AVariant> res = new List<AVariant>(op1.m_lValue);
            res.AddRange(op2.m_lValue);

            return new LVariant(res);
        }

        public static LVariant operator -(LVariant op1, LVariant op2)
        {
            List<AVariant> res = new List<AVariant>(op1.m_lValue);

            for (int i = 0; i < op2.m_lValue.Count; ++i)
            {
                while (res.Contains(op2.m_lValue[i]))
                {
                    res.Remove(op2.m_lValue[i]);
                }
            }

            return new LVariant(res);
        }

        public static bool operator <(LVariant op1, LVariant op2)
        {
            return op1.m_lValue.Count < op2.m_lValue.Count;
        }

        public static bool operator >(LVariant op1, LVariant op2)
        {
            return op1.m_lValue.Count > op2.m_lValue.Count;
        }

        public static bool operator <=(LVariant op1, LVariant op2)
        {
            return op1.m_lValue.Count <= op2.m_lValue.Count;
        }

        public static bool operator >=(LVariant op1, LVariant op2)
        {
            return op1.m_lValue.Count >= op2.m_lValue.Count;
        }

        public static bool operator ==(LVariant op1, LVariant op2)
        {
            return op1.m_lValue.Equals(op2.m_lValue);
        }

        public static bool operator !=(LVariant op1, LVariant op2)
        {
            return !op1.m_lValue.Equals(op2.m_lValue);
        }
    }
}
