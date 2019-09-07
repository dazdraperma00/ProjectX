using System;

namespace PSCompiler
{
    class Node
    {
        public NodeType type;
        public Node[] operands;

        public Node(NodeType _type, params Node[] _operands)
        {
            this.type = _type;
            operands = _operands;
        }
    }

    class VarNode : Node
    {
        public Variant value;
        public string varName;

        public VarNode(NodeType _type, Variant _value, string _name = null, params Node[] _operands)
            :base(_type, _operands)
        {
            value = _value;
            varName = _name;
        }
    }
}
