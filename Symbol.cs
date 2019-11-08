using System.Collections.Generic;

namespace KayoCompiler
{
    class VarSymbol
    {
        internal VarType type;
        internal string name;
        internal int scopeId;
		internal int offsetInFun;
		internal bool isConst;
		internal int eleSize;
		internal VarType eleType;
    }

	class FunSymbol
	{
		internal bool isExtern;
		internal VarType returnType;
		internal string name;
		internal List<VarType> parasType;
		internal int localVarSize;

		internal FunSymbol()
		{
			name = string.Empty;
			parasType = new List<VarType>();
		}
	}
}