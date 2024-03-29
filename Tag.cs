﻿namespace KayoCompiler
{
    enum Tag
    {
        NULL,

        // Keywords:
        // bool, else, for, if, char, int, long, void
        // return, read, while, write, true, false, const
        KW_BOOL,
        KW_ELSE,
        KW_FOR,
        KW_IF,
        KW_CHAR,
        KW_INT,
        KW_LONG,
        KW_VOID,
        KW_RETURN,
        KW_READ,
        KW_WHILE,
        KW_WRITE,
        KW_TRUE,
        KW_FALSE,
        KW_CONST,

        // Delimiter
        DL_PLUS,        // +
        DL_MINUS,       // -
        DL_MULTI,       // *
        DL_OBELUS,      // /
        DL_MOD,         // %
        DL_LT,          // <
        DL_GT,          // >
        DL_NLT,         // >=
        DL_NGT,         // <=
        DL_EQ,          // ==
        DL_NEQ,         // !=
        DL_SET,         // =
        DL_OR,          // ||
        DL_AND,         // &&
        DL_NOT,         // !
        DL_SEM,         // ;
        DL_LPAR,        // (
        DL_RPAR,        // )
        DL_LSQU,        // [
        DL_RSQU,        // ]
        DL_LBRACE,      // {
        DL_RBRACE,      // }
        DL_COL,         // :
        DL_COM,         // ,

        // Other type
        ID,
        NUM,
        COMMENT,

        // Error tag
        ERROR
    }
}