using System;

namespace PSCompiler
{
    public enum Token : ushort
    {
        NUM,
        VAR,
        NAME,
        SET,
        SUM,
        SUB,
        GREATER,
        LESS,
        GREATEREQUAL,
        LESSEQUAL,
        EQUAL,
        NOTEQUAL,
        IF,
        ELSE,
        //OR,
        //AND,
        //NOT,
        WHILE,
        DO,
        FOR,
        TO,
        LBRA,
        RBRA,
        LPAR,
        RPAR,
        SEMICOLON,

        NONE
    }

    class Lexer
    {
        private readonly string code;
        private Token token;
        private string value;

        public Lexer(string _code = "")
        {
            this.code = _code;
        }

        public void DetermineNextToken()
        {
            this.token = Token.NONE;
            this.value = "";

            for (int i = 0; i < code.Length; ++i)
            {
                if (Char.IsSeparator(code[i]))
                {
                    continue;
                }
                else if (Char.IsDigit(code[i]))
                {
                    token = Token.NUM;

                    while (Char.IsDigit(code[i]))
                    {
                        value += code[i++];
                    }

                    if (code[i] == '.')
                    {
                        value += code[i++];

                        while (Char.IsDigit(code[i]))
                        {
                            value += code[i++];
                        }
                    }
                }
                else if (Char.IsLetter(code[i]))
                {
                    string word = "";

                    while (Char.IsLetterOrDigit(code[i]))
                    {
                        word += code[i++];
                    }

                    switch(word)
                    {
                        case "var":
                            token = Token.VAR;
                            break;
                        case "if":
                            token = Token.IF;
                            break;
                        case "else":
                            token = Token.ELSE;
                            break;
                        case "while":
                            token = Token.WHILE;
                            break;
                        case "do":
                            token = Token.WHILE;
                            break;
                        case "for":
                            token = Token.FOR;
                            break;
                        case "to":
                            token = Token.TO;
                            break;
                        default:
                            token = Token.NAME;
                            break;
                    }
                }
                else
                {
                    switch(code[i])
                    {
                        case '+':
                            token = Token.SUM;
                            break;
                        case '-':
                            token = Token.SUB;
                            break;
                        case '=':
                            token = Token.SET;
                            break;
                        case '>':
                            token = Token.GREATER;
                            break;
                        case '<':
                            token = Token.LESS;
                            break;
                        case ';':
                            token = Token.SEMICOLON;
                            break;
                        case '{':
                            token = Token.LBRA;
                            break;
                        case '}':
                            token = Token.RBRA;
                            break;
                        case '(':
                            token = Token.LPAR;
                            break;
                        case ')':
                            token = Token.RPAR;
                            break;
                        default:
                            throw new Exception("unexpected symbol");
                    }
                }
            }
        }

        public Token GetToken()
        {
            return this.token;
        }
        public string GetValue()
        {
            return this.value;
        }
    }
}
