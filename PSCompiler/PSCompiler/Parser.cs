﻿using System;

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
        NEQ,
        IF,
        IFELSE,
        WHILE,
        DO,
        SEQ,

        NONE
    }

    class Parser
    {
        private Lexer lexer;

        public Parser(string _code)
        {
            lexer = new Lexer(_code);
        }

        private Node CreateExpressionNode(Node op1 = null)
        {
            NodeType type = NodeType.NONE;

            if (op1 == null)
            {
                op1 = CreateVarNode();
            }

            Token token = lexer.GetToken();

            switch (token)
            {
                case Token.GREATER:
                    type = NodeType.GT;
                    break;
                case Token.GREATEREQUAL:
                    type = NodeType.GET;
                    break;
                case Token.LESS:
                    type = NodeType.LT;
                    break;
                case Token.LESSEQUAL:
                    type = NodeType.LET;
                    break;
                case Token.EQUAL:
                    type = NodeType.EQ;
                    break;
                case Token.NOTEQUAL:
                    type = NodeType.NEQ;
                    break;
                default:
                    throw new Exception("syntax error");
            }

            lexer.DetermineNextToken();

            Node op2 = CreateVarNode();

            return new Node(type, op1, op2);
        }

        private Node CreateScopedExpressionNode()
        {
            Token token = lexer.GetToken();

            if (token != Token.LPAR)
            {
                throw new Exception("syntax error");
            }

            lexer.DetermineNextToken();
            token = lexer.GetToken();

            Node node = CreateExpressionNode();

            while (token != Token.RPAR)
            {
                node = CreateExpressionNode(node);

                token = lexer.GetToken();
            }

            lexer.DetermineNextToken();

            return node;
        }

        private Node CreateScopedBlockNode()
        {
            Token token = lexer.GetToken();

            if (token != Token.LBRA)
            {
                throw new Exception("syntax error");
            }

            lexer.DetermineNextToken();
            token = lexer.GetToken();

            Node node = CreateNode();

            while (token != Token.RBRA)
            {
                node = new Node(NodeType.SEQ, node, CreateNode());
            }

            return node;
        }

        private Node CreateConditionNode()
        {
            Token token = lexer.GetToken();

            if (token != Token.IF)
            {
                throw new Exception("syntax error");
            }

            lexer.DetermineNextToken();
            token = lexer.GetToken();

            NodeType type = NodeType.IF;
            Node op1 = CreateScopedExpressionNode();
            Node op2 = CreateScopedBlockNode();
            Node op3 = null;

            if (token == Token.ELSE)
            {
                type = NodeType.IFELSE;
                op3 = CreateScopedBlockNode();
            }

            return new Node(type, op1, op2, op3);
        }

        private Node CreateCycleNode()
        {
            Token token = lexer.GetToken();

            Node op1 = null;
            NodeType type = NodeType.NONE;
            
            if (token == Token.WHILE)
            {
                type = NodeType.WHILE;

                lexer.DetermineNextToken();

                op1 = CreateScopedExpressionNode();
            }
            else
            {
                throw new Exception("syntax error");
            }

            Node op2 = CreateScopedBlockNode();

            return new Node(type, op1, op2);
        }

        private Node CreateDoNode()
        {
            Token token = lexer.GetToken();

            if (token != Token.DO)
            {
                throw new Exception("syntax error");
            }

            lexer.DetermineNextToken();

            Node op1 = CreateScopedBlockNode();

            token = lexer.GetToken();

            if (token != Token.WHILE)
            {
                throw new Exception("syntax error");
            }

            lexer.DetermineNextToken();

            Node op2 = CreateScopedExpressionNode();

            return new Node(NodeType.DO, op1, op2);
        }

        private Node CreateMathNode(Node op1 = null)
        {
            NodeType type = NodeType.NONE;

            if (op1 == null)
            {
                op1 = CreateVarNode();
            }

            Token token = lexer.GetToken();

            switch (token)
            {
                case Token.SUM:
                    type = NodeType.ADD;
                    break;
                case Token.SUB:
                    type = NodeType.SUB;
                    break;
                default:
                    throw new Exception("syntax error");
            }

            lexer.DetermineNextToken();

            VarNode op2 = CreateVarNode();

            return new Node(type, null, null, op1, op2);
        }

        private Node CreateSetNode()
        {
            Token token = lexer.GetToken();

            if (token != Token.SET)
            {
                throw new Exception("syntax error");
            }

            lexer.DetermineNextToken();

            Node node = CreateVarNode();

            token = lexer.GetToken();

            while (token != Token.SEMICOLON)
            {
                node = CreateMathNode(node);

                token = lexer.GetToken();
            }

            lexer.DetermineNextToken();

            return node;
        }

        private VarNode CreateVarNode()
        {
            Token token = lexer.GetToken();

            NodeType type = NodeType.NONE;
            string name = null;
            Variant value = null;
            Node op1 = null;

            if (token == Token.NUM)
            {
                type = NodeType.CONST;
                value = new Variant(float.Parse(lexer.GetValue()));

                lexer.DetermineNextToken();
            }
            else
            {
                if (token == Token.VAR)
                {
                    type = NodeType.DECL;

                    lexer.DetermineNextToken();
                    token = lexer.GetToken();
                }
                else if (token == Token.NAME)
                {
                    type = NodeType.VAR;
                }
                else
                {
                    throw new Exception("syntax error");
                }

                name = lexer.GetValue();

                lexer.DetermineNextToken();
                token = lexer.GetToken();

                if (token == Token.SET)
                {
                    if (type != NodeType.DECL)
                    {
                        type = NodeType.SET;
                    }

                    op1 = CreateSetNode();
                }
                else if (token == Token.SEMICOLON)
                {
                    value = new Variant(0.0);

                    lexer.DetermineNextToken();
                }
                else
                {
                    throw new Exception("syntax error");
                }
            }

            return new VarNode(type, value, name, op1);
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
                case Token.LBRA:
                    return CreateScopedBlockNode();
                default:
                    throw new Exception("syntax error");
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
