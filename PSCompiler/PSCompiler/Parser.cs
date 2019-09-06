using System;

namespace PSCompiler
{
    public enum NodeType : ushort
    {
        PROGRAM,
        VAR,
        CONST,
        ADD,
        SUB,
        GT,
        LT,
        IF,
        IFELSE,
        WHILE,
        DO,

        NONE
    }

    class Parser
    {
        private Lexer lexer;

        public Parser(string _code)
        {
            lexer = new Lexer(_code);
        }

        private void BuildTree(Node head)
        {
            lexer.DetermineNextToken();
            Token token = lexer.GetToken();
            switch (token)
            {

            }
        }

        public Node Parse()
        {
            try
            {
                Node head = new Node(NodeType.PROGRAM);
                BuildTree(head);
                return head;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}
