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
        static readonly Dictionary<string, List<TableVarItem>> vars = new Dictionary<string, List<TableVarItem>>();
        
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

            if (!vars.ContainsKey(varItem.name))
                vars.Add(varItem.name, new List<TableVarItem>());
            
            vars[varItem.name].Add(varItem);
            vars[varItem.name].Sort((x, y) => y.field - x.field );

            return TableAddStatus.SUCCEED;
        }

        internal static TableVarItem? FindVar(string name, int field)
        {
            if (!vars.ContainsKey(name))
                return null;
            
            foreach (var item in vars[name])
            {
                if (item.name == name && item.field <= field)
                    return item;
            }

            return null;
        }

        internal static int GetVarIndex(string name, int field)
        {
            if (!vars.ContainsKey(name))
                return -1;
            
            for (int i = 0; i < vars[name].Count; i++)
            {
                if (vars[name][i].name == name && vars[name][i].field <= field)
                    return i;
            }

            return -1;
        }
    }
}
