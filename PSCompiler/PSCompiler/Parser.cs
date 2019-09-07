using System;

namespace PSCompiler
{
    public enum NodeType : ushort
    {
        PROGRAM,
        DECL,
        VAR,
        CONST,
        SET,
        ADD,
        SUB,
        LT,
        GT,
        LET,
        GET,
        EQ,
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

        private Node CreateCompareNode()
        {
            Token token = lexer.GetToken();

            if (token != Token.LPAR)
            {
                throw new Exception("expected (");
            }

            Node op1 = CreateVarNode();

            token = lexer.GetToken();

            NodeType nodeType = NodeType.NONE;

            switch(token)
            {
                case Token.EQUAL:
                    nodeType = NodeType.EQ;
                    break;
                case Token.LESS:
                    nodeType = NodeType.LT;
                    break;
                case Token.GREATER:
                    nodeType = NodeType.GT;
                    break;
                case Token.LESSEQUAL:
                    nodeType = NodeType.LET;
                    break;
                case Token.GREATEREQUAL:
                    nodeType = NodeType.GET;
                    break;
                default:
                    throw new Exception("wrong condition");
            }

            lexer.DetermineNextToken();

            Node op2 = CreateVarNode();

            token = lexer.GetToken();

            if (token != Token.RPAR)
            {
                throw new Exception("expected )");
            }

            lexer.DetermineNextToken();

            return new Node(nodeType, null, op1, op2);
        }

        private Node CreateConditionNode()
        {
            NodeType type = NodeType.IF;
            Node op1 = CreateCompareNode();
            Node op2 = CreateNode();
            Node op3 = null;

            Token token = lexer.GetToken();

            if (token == Token.ELSE)
            {
                type = NodeType.IFELSE;
                op3 = CreateNode();
            }

            lexer.DetermineNextToken();

            return new Node(type, null, op1, op2, op3);
        }

        private Node CreateCycleNode()
        {
            Token token = lexer.GetToken();
            Node op1 = null;
            NodeType type = NodeType.NONE;
            
            if (token == Token.WHILE)
            {
                type = NodeType.WHILE;
                op1 = CreateCompareNode();
            }
            else if (token == Token.FOR)
            {

            }

            Node op2 = CreateNode();
            return new Node(type, null, op1, op2);
        }

        private Node CreateDoNode()
        {
            Node op1 = CreateNode();

            Token token = lexer.GetToken();

            if (token != Token.WHILE)
            {
                throw new Exception("expected while statement");
            }

            lexer.DetermineNextToken();

            Node op2 = CreateCompareNode();
            return new Node(NodeType.DO, null, op1, op2);
        }

        private VarNode CreateVarNode()
        {
            Token token = lexer.GetToken();

            NodeType type = NodeType.VAR;

            if (token == Token.VAR)
            {
                type = NodeType.DECL;

                lexer.DetermineNextToken();
                token = lexer.GetToken();
            }

            string name = null;
            Variant value = null;

            if (token == Token.NAME)
            {
                name = lexer.GetValue();

                lexer.DetermineNextToken();
                token = lexer.GetToken();


                if (token == Token.SET)
                {
                    if (type != NodeType.DECL)
                    {
                        type = NodeType.SET;
                    }

                    lexer.DetermineNextToken();
                    token = lexer.GetToken();

                    if (token == Token.NUM)
                    {
                        value = new Variant(float.Parse(lexer.GetValue()));
                    }
                    else
                    {
                        throw new Exception("wrong var initializetion");
                    }
                }
            }
            else if (token == Token.NUM)
            {
                type = NodeType.CONST;
                value = new Variant(float.Parse(lexer.GetValue()));
            }
            else
            {
                throw new Exception("unexpected expression");
            }

            lexer.DetermineNextToken();

            return new VarNode(type, value, name);
        }

        private Node CreateNode()
        {
            Token token = lexer.GetToken();
            lexer.DetermineNextToken();

            switch (token)
            {
                case Token.IF:
                    return CreateConditionNode();
                case Token.WHILE:
                    return CreateCycleNode();
                case Token.DO:
                    return CreateDoNode();
                case Token.VAR:
                case Token.NAME:
                    return CreateVarNode();
            }

            return null;
        }

        public Node Parse()
        {
            lexer.DetermineNextToken();

            Node head = new Node(NodeType.PROGRAM, null, CreateNode());
            return head;
        }
    }
}
