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
    
        internal static int CurFunVarSize
        {
            get
            {
                return funs[ScopeManager.CurrentFun].localVarSize;
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

        // 仅在代码生成阶段使用，即已经保证程序的正确性
        // 用于生成局部变量/函数参数相对栈基地址的偏移
        internal static int GetVarOffset(string name)
        {
            if (!vars.ContainsKey(name))
                return 0;
            
            foreach (var item in vars[name])
            {
                if (ScopeManager.IsVisibleNow(item.scopeId))
                    return item.offsetInFun;
            }

            return 0;
        }
    }
}
