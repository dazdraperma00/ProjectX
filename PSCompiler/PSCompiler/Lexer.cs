using System;

namespace PSCompiler
{
    public enum Token : ushort
    {
        NUM,
        VAR,
        NAME,
        SET,
        PLUS,
        MINUS,
        GREATER,
        LESS,
        IF,
        ELSE,
        WHILE,
        DO,
        FOR,
        TO,

        NONE
    }

    class Lexer
    {
        public static readonly Exception ues = new Exception("Unexpected symbol");

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
                            token = Token.PLUS;
                            break;
                        case '-':
                            token = Token.MINUS;
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
                        default:
                            throw Lexer.ues;
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
