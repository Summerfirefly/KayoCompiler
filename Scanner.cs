using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KayoCompiler
{
    class Scanner
    {
        private StreamReader stream;

        private Dictionary<string, Tag> keywords = new Dictionary<string, Tag>
        {
            { "bool", Tag.KW_BOOL },
            { "else", Tag.KW_ELSE },
            { "if", Tag.KW_IF },
            { "int", Tag.KW_INT },
            { "read", Tag.KW_READ },
            { "while", Tag.KW_WHILE },
            { "write", Tag.KW_WRITE }
        };

        private Dictionary<string, Tag> delimiter = new Dictionary<string, Tag>
        {
            { "+", Tag.DL_PLUS },   { "-", Tag.DL_MINUS },
            { "*", Tag.DL_MULTI },  { "/", Tag.DL_OBELUS },
            { "<", Tag.DL_LT },     { ">", Tag.DL_GT },
            { ">=", Tag.DL_NLT },   { "<=", Tag.DL_NGT },
            { "==", Tag.DL_EQ },    { "!=", Tag.DL_NEQ },
            { "=", Tag.DL_SET },    { "||", Tag.DL_OR },
            { "&&", Tag.DL_AND },   { "!", Tag.DL_NOT },
            { ";", Tag.DL_SEM },    { "(", Tag.DL_LPAR },
            { ")", Tag.DL_RPAR },   { "{", Tag.DL_LBRACE },
            { "}", Tag.DL_RBRACE }, { "/**/", Tag.COMMENT }
        };

        private char[] delimiterFirst = new char[]
        {
            '+', '-', '*', '/', '>',
            '<', '=', '!', '|', '&',
            ';', '(', ')', '{', '}'
        };

        public Scanner(string path)
        {
            stream = new StreamReader(new FileStream(path, FileMode.Open));
        }

        public List<Token> Scan()
        {
            List<Token> token = new List<Token>();
            char ch = NextChar();

            while (!stream.EndOfStream)
            {
                string value = string.Empty;

                while (IsSpace(ch))
                {
                    ch = NextChar();
                }

                if (IsDigit(ch))
                {
                    while (IsDigit(ch))
                    {
                        value += ch;
                        ch = NextChar();
                    }

                    token.Add(new Token { Tag = Tag.NUM, Value = value });
                }
                else if (IsLetter(ch))
                {
                    value += ch;
                    ch = NextChar();

                    while (IsLetter(ch) || IsDigit(ch))
                    {
                        value += ch;
                        ch = NextChar();
                    }

                    token.Add(GetIdToken(value));
                }
                else if (IsDelimiter(ch))
                {
                    value += ch;

                    switch (ch)
                    {
                        case '>':
                        case '<':
                        case '=':
                        case '!':
                        case '&':
                        case '|':
                            ch = NextChar();
                            switch (ch)
                            {
                                case '=':
                                case '&':
                                case '|':
                                    value += ch;
                                    ch = NextChar();
                                    break;
                            }
                            break;
                        case '/':
                            ch = NextChar();
                            if (ch == '*')
                            {
                                while (!stream.EndOfStream)
                                {
                                    ch = NextChar();
                                    if (ch == '*')
                                    {
                                        while (ch == '*')
                                        {
                                            ch = NextChar();
                                        }

                                        if (ch == '/')
                                        {
                                            value = "/**/";
                                            ch = NextChar();
                                            break;
                                        }
                                    }
                                }

                                // TODO: 未正常结束注释的错误处理
                            }
                            break;
                        default:
                            ch = NextChar();
                            break;
                    }

                    token.Add(GetDelimiterToken(value));
                }
                else
                {
                    value += ch;
                    ch = NextChar();
                    token.Add(new Token { Tag = Tag.ERROR, Value = value });
                }
            }

            return token;
        }

        private Token GetDelimiterToken(string value)
        {
            return new Token
            {
                Tag = delimiter.ContainsKey(value) ? delimiter[value] : Tag.ERROR,
                Value = value
            };
        }

        private Token GetIdToken(string value)
        {
            return new Token
            {
                Tag = keywords.ContainsKey(value) ? keywords[value] : Tag.ID,
                Value = value
            };
        }

        private bool IsDelimiter(char ch)
        {
            foreach (char c in delimiterFirst)
            {
                if (c == ch)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsDigit(char ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private bool IsLetter(char ch)
        {
            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');
        }

        private bool IsSpace(char ch)
        {
            return ch == ' ' || ch == '\t' || ch == '\n';
        }

        private char NextChar()
        {
            return Convert.ToChar(stream.Read());
        }
    }
}
