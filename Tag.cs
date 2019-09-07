namespace KayoCompiler
{
    enum Tag
    {
        // Keywords:
        // bool, else, if, int, read,
        // while, write
        KW_BOOL,
        KW_ELSE,
        KW_IF,
        KW_INT,
        KW_READ,
        KW_WHILE,
        KW_WRITE,

        // Delimiter
        DL_PLUS,        // +
        DL_MINUS,       // -
        DL_MULTI,       // *
        DL_OBELUS,      // /
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
        DL_LBRACE,      // {
        DL_RBRACE,      // }

        // Other type
        ID,
        NUM,
        COMMENT,

        // Error tag
        ERROR
    }
}