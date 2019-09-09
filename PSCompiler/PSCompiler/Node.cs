namespace PSCompiler
{
    class Node
    {
        private NodeType type;
        private Node[] operands;

        public Node(NodeType _type, params Node[] _operands)
        {
            this.type = _type;
            operands = _operands;
        }

        public NodeType GetNodeType()
        {
            return type;
        }
    }

    class VarNode : Node
    {
        private Variant value;
        private string varName;

        public VarNode(NodeType _type, Variant _value, string _name = null, params Node[] _operands)
            :base(_type, _operands)
        {
            value = _value;
            varName = _name;
        }
    }
}
