using System;
using System.Collections.Generic;
using System.IO;

namespace KayoCompiler
{
    class Scanner
    {
        private readonly StreamReader stream;
        private int next;

        public static int LineNum { get; private set; } = 1;

        private readonly Dictionary<string, Tag> keywords = new Dictionary<string, Tag>
        {
            { "bool", Tag.KW_BOOL },
            { "else", Tag.KW_ELSE },
            { "if", Tag.KW_IF },
            { "int", Tag.KW_INT },
            { "void", Tag.KW_VOID },
            { "fun", Tag.KW_FUN },
            { "return", Tag.KW_RETURN},
            { "read", Tag.KW_READ },
            { "while", Tag.KW_WHILE },
            { "for", Tag.KW_FOR },
            { "write", Tag.KW_WRITE },
            { "true", Tag.KW_TRUE },
            { "false", Tag.KW_FALSE }
        };

        private readonly Dictionary<string, Tag> delimiter = new Dictionary<string, Tag>
        {
            { "+", Tag.DL_PLUS },   { "-", Tag.DL_MINUS },
            { "*", Tag.DL_MULTI },  { "/", Tag.DL_OBELUS },
            { "%", Tag.DL_MOD },
            { "<", Tag.DL_LT },     { ">", Tag.DL_GT },
            { ">=", Tag.DL_NLT },   { "<=", Tag.DL_NGT },
            { "==", Tag.DL_EQ },    { "!=", Tag.DL_NEQ },
            { "=", Tag.DL_SET },    { "||", Tag.DL_OR },
            { "&&", Tag.DL_AND },   { "!", Tag.DL_NOT },
            { ";", Tag.DL_SEM },    { "(", Tag.DL_LPAR },
            { ")", Tag.DL_RPAR },   { "{", Tag.DL_LBRACE },
            { "}", Tag.DL_RBRACE }, { ":", Tag.DL_COL },
            { ",", Tag.DL_COM },    { "/**/", Tag.COMMENT }
        };

        private readonly char[] delimiterFirst = new char[]
        {
            '+', '-', '*', '/', '>',
            '<', '=', '!', '|', '&',
            ';', '(', ')', '{', '}',
            ':', ',', '%'
        };

        public Scanner(string path)
        {
            stream = new StreamReader(new FileStream(path, FileMode.Open));
            NextChar();
        }

        public Token NextToken()
        {
            Token token = new Token { Tag = Tag.NULL, Value = string.Empty };

            if (next > -1)
            {
                string value = string.Empty;

                // 跳过空白符
                while (IsSpace(next))
                {
                    NextChar();
                }

                // 自动机入口分支
                if (IsDigit(next)) // 数字
                {
                    while (IsDigit(next))
                    {
                        value += (char)next;
                        NextChar();
                    }

                    token = new Token { Tag = Tag.NUM, Value = value };
                }
                else if (IsLetter(next)) // 关键字或标识符
                {
                    while (IsLetter(next) || IsDigit(next))
                    {
                        value += (char)next;
                        NextChar();
                    }

                    token = GetIdToken(value);
                }
                else if (IsDelimiter(next)) // 操作符与注释
                {
                    value += (char)next;
                    char tmp = (char)next;
                    NextChar();

                    switch (tmp)
                    {
                        case '>':
                        case '<':
                        case '=':
                        case '!':
                            if (next == '=')
                            {
                                value += (char)next;
                                NextChar();
                            }
                            break;
                        case '&':
                            if (next == '&')
                            {
                                value += (char)next;
                                NextChar();
                            }
                            break;
                        case '|':
                            if (next == '|')
                            {
                                value += (char)next;
                                NextChar();
                            }
                            break;
                        case '/':
                            if (next == '*')
                            {
                                while (next > -1)
                                {
                                    NextChar();
                                    if (next == '*')
                                    {
                                        while (next == '*')
                                        {
                                            NextChar();
                                        }

                                        if (next == '/')
                                        {
                                            value = "/**/";
                                            NextChar();
                                            break;
                                        }
                                    }
                                }

                                // TODO: 未正常结束注释的错误处理
                            }
                            break;
                        default:
                            break;
                    }

                    token = GetDelimiterToken(value);
                }
                else
                {
                    value += (char)next;
                    NextChar();
                    token = new Token { Tag = Tag.ERROR, Value = value };
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

        private bool IsDelimiter(int ch)
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

        private bool IsDigit(int ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private bool IsLetter(int ch)
        {
            return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z');
        }

        private bool IsSpace(int ch)
        {
            return ch == ' ' || ch == '\t' || ch == '\n';
        }

        private void NextChar()
        {
            if (next == '\n')
                LineNum++;
            next = stream.EndOfStream ? -1 : stream.Read();
        }
    }
}
