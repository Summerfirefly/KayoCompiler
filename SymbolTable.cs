using System.Collections.Generic;

namespace KayoCompiler
{
    enum TableAddStatus
    {
        SUCCEED,
        SYMBOL_EXIST
    }

    static class SymbolTable
    {
        static readonly Dictionary<string, List<VarSymbol>> vars = new Dictionary<string, List<VarSymbol>>();
        static readonly Dictionary<string, FunSymbol> funs = new Dictionary<string, FunSymbol>();
    
        internal static int CurFunVarCount
        {
            get
            {
                return funs[ScopeManager.CurrentFun].localVarCount;
            }
        }

        internal static TableAddStatus AddFun(FunSymbol varItem)
        {
            if (FindFun(varItem.name) != null)
                return TableAddStatus.SYMBOL_EXIST;
            
            funs.Add(varItem.name, varItem);
            return TableAddStatus.SUCCEED;
        }

        internal static TableAddStatus AddVar(VarSymbol varItem)
        {
            if (FindVar(varItem.name)?.scopeId == ScopeManager.CurrentScope)
                return TableAddStatus.SYMBOL_EXIST;

            if (!vars.ContainsKey(varItem.name))
                vars.Add(varItem.name, new List<VarSymbol>());
            
            vars[varItem.name].Add(varItem);
            vars[varItem.name].Sort((x, y) => y.scopeId - x.scopeId );

            return TableAddStatus.SUCCEED;
        }

        internal static FunSymbol FindFun(string name)
        {
            if (!funs.ContainsKey(name))
                return null;

            return funs[name];
        }

        internal static VarSymbol FindVar(string name)
        {
            if (!vars.ContainsKey(name))
                return null;
            
            foreach (var item in vars[name])
            {
                if (ScopeManager.IsVisibleNow(item.scopeId))
                    return item;
            }

            return null;
        }

        internal static int GetVarIndex(string name)
        {
            if (!vars.ContainsKey(name))
                return -1;
            
            foreach (var item in vars[name])
            {
                if (ScopeManager.IsVisibleNow(item.scopeId))
                    return item.indexInFun;
            }

            return -1;
        }
    }
}
