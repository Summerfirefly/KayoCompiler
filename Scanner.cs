using System.Collections.Generic;
using System.IO;
using KayoCompiler.Errors;

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
            { "char", Tag.KW_CHAR },
            { "int", Tag.KW_INT },
            { "long", Tag.KW_LONG },
            { "void", Tag.KW_VOID },
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
            { ")", Tag.DL_RPAR },   { "[", Tag.DL_LSQU },
            { "]", Tag.DL_RSQU },   { "{", Tag.DL_LBRACE },
            { "}", Tag.DL_RBRACE }, { ":", Tag.DL_COL },
            { ",", Tag.DL_COM },    { "/**/", Tag.COMMENT }
        };

        private readonly char[] delimiterFirst = new char[]
        {
            '+', '-', '*', '/', '>',
            '<', '=', '!', '|', '&',
            ';', '(', ')', '[', ']',
            '{', '}', ':', ',', '%'
        };

        public Scanner(string path)
        {
            stream = new StreamReader(new FileStream(path, FileMode.Open));
            NextChar();
        }

        public Token NextToken()
        {
            Token token = new Token { Tag = Tag.NULL, Value = string.Empty, LineNum = LineNum };

            if (next > -1)
            {
                string value = string.Empty;

                // 跳过空白符
                while (IsSpace(next))
                {
                    NextChar();
                }

                token.LineNum = LineNum;
                // 自动机入口分支
                if (IsDigit(next)) // 数字
                {
                    while (IsDigit(next))
                    {
                        value += (char)next;
                        NextChar();
                    }

                    token = new Token { Tag = Tag.NUM, Value = value, LineNum = LineNum };
                }
                else if (IsLetter(next) || next == '_') // 关键字或标识符
                {
                    while (IsLetter(next) || IsDigit(next) || next == '_')
                    {
                        value += (char)next;
                        NextChar();
                    }

                    token = GetIdToken(value);
                }
                else if (next == '\'') // 字符常量
                {
                    NextChar();
                    if (next == '\\')
                    {
                        NextChar();
                        switch (next)
                        {
                            case 'a':
                                value = ((int)'\a').ToString();
                                break;
                            case 'b':
                                value = ((int)'\b').ToString();
                                break;
                            case 'f':
                                value = ((int)'\f').ToString();
                                break;
                            case 'n':
                                value = ((int)'\n').ToString();
                                break;
                            case 'r':
                                value = ((int)'\r').ToString();
                                break;
                            case 't':
                                value = ((int)'\t').ToString();
                                break;
                            case 'v':
                                value = ((int)'\v').ToString();
                                break;
                            case '0':
                                value = ((int)'\0').ToString();
                                break;
                            case '\'':
                                value = ((int)'\'').ToString();
                                break;
                            case '\\':
                                value = ((int)'\\').ToString();
                                break;
                            default:
                                new Error().PrintErrMsg();
                                break;
                        }
                    }
                    else if (next == '\'')
                    {
                        new Error().PrintErrMsg();
                    }
                    else
                    {
                        value = next.ToString();
                    }

                    NextChar();
                    if (next != '\'')
                    {
                        new Error().PrintErrMsg();
                        while (next != '\'' || next != '\n')
                        {
                            NextChar();
                        }
                    }
                    else
                    {
                        NextChar();
                    }

                    token = new Token { Tag = Tag.NUM, Value = value, LineNum = LineNum };
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
                                value = "/**/";
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
                                            break;
                                        }
                                    }
                                }

                                if (next == -1)
                                {
                                    new Error().PrintErrMsg();
                                }
                                NextChar();
                            }
                            else if (next == '/')
                            {
                                value = "/**/";
                                while (next != '\n' && next != '\n')
                                {
                                    NextChar();
                                }
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
                    token = new Token { Tag = Tag.ERROR, Value = value, LineNum = LineNum };
                }
            }

            return token;
        }

        private Token GetDelimiterToken(string value)
        {
            return new Token
            {
                Tag = delimiter.ContainsKey(value) ? delimiter[value] : Tag.ERROR,
                Value = value,
                LineNum = LineNum
            };
        }

        private Token GetIdToken(string value)
        {
            return new Token
            {
                Tag = keywords.ContainsKey(value) ? keywords[value] : Tag.ID,
                Value = value,
                LineNum = LineNum
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
