using System;

namespace PSCompiler
{
    class Node
    {
        private readonly NodeType type;
        private readonly Variant value;
        private readonly Node op1, op2, op3;

        public Node(NodeType _type, Variant _variant = null, Node _op1 = null, Node _op2 = null, Node _op3 = null)
        {
            this.type = _type;
            this.value = _variant;
            this.op1 = _op1;
            this.op2 = _op2;
            this.op3 = _op3;
        }
    }
}
