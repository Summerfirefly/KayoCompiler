using System.Collections.Generic;
using KayoCompiler.Ast;

namespace KayoCompiler
{
    enum TableAddStatus
    {
        SUCCEED,
        SYMBOL_EXIST
    }

    struct TableVarItem
    {
        internal VarType type;
        internal string name;
        internal int field;
    }

    static class SymbolTable
    {
        static readonly List<TableVarItem> vars = new List<TableVarItem>();
        
        internal static int VarCount
        {
            get
            {
                return vars.Count;
            }
        }

        internal static TableAddStatus AddVar(TableVarItem varItem)
        {
            if (FindVar(varItem.name, varItem.field) != null)
                return TableAddStatus.SYMBOL_EXIST;

            vars.Add(varItem);
            return TableAddStatus.SUCCEED;
        }

        internal static TableVarItem? FindVar(string name, int field)
        {
            foreach (var item in vars)
            {
                if (item.name == name && item.field == field)
                    return item;
            }

            return null;
        }

        internal static TableVarItem GetVar(int index)
        {
            return vars[index % vars.Count];
        }
    }
}
