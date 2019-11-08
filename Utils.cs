using KayoCompiler.Ast;

namespace KayoCompiler
{
	internal static class Utils
	{
        internal static bool IsNumType(VarType type)
        {
            return type == VarType.TYPE_INT || type == VarType.TYPE_LONG || type == VarType.TYPE_CHAR;
        }

		internal static bool IsTypeTag(Tag tag)
		{
			Tag[] typeTags =
			{
				Tag.KW_VOID,
				Tag.KW_BOOL,
				Tag.KW_CHAR,
				Tag.KW_INT,
				Tag.KW_LONG
			};

			foreach (Tag typeTag in typeTags)
			{
				if (tag == typeTag)
					return true;
			}

			return false;
		}

		internal static int SizeOf(VarType type)
        {
            switch (type)
            {
                case VarType.TYPE_BOOL:
                case VarType.TYPE_CHAR:
                    return 1;
                case VarType.TYPE_INT:
                    return 4;
                case VarType.TYPE_LONG:
				case VarType.TYPE_PTR:
                    return 8;
                default:
                    return 0;
            }
        }

		internal static VarType TagToType(Tag tag)
		{
			switch (tag)
			{
				case Tag.KW_VOID:
					return VarType.TYPE_VOID;
				case Tag.KW_CHAR:
					return VarType.TYPE_CHAR;
				case Tag.KW_INT:
					return VarType.TYPE_INT;
				case Tag.KW_LONG:
					return VarType.TYPE_LONG;
				case Tag.KW_BOOL:
					return VarType.TYPE_BOOL;
				default:
					return VarType.TYPE_ERROR;
			}
		}

		internal static VarType TypeCalc(VarType typeA, VarType typeB, bool logicOp)
		{
			if (typeA == VarType.TYPE_NULL)
				return typeB;
			if (typeB == VarType.TYPE_NULL)
				return typeA;

			if (typeA == VarType.TYPE_VOID || typeB == VarType.TYPE_VOID)
				return VarType.TYPE_ERROR;

			if (typeA == VarType.TYPE_BOOL && typeB == VarType.TYPE_BOOL && logicOp)
				return VarType.TYPE_BOOL;

			if (IsNumType(typeA) && IsNumType(typeB) && !logicOp)
				return typeA > typeB ? typeA : typeB;

			return VarType.TYPE_ERROR;
		}

		internal static VarType TypeCmp(VarType typeA, VarType typeB, bool equalOp)
		{
			if (typeA == VarType.TYPE_NULL)
				return typeB;
			if (typeB == VarType.TYPE_NULL)
				return typeA;

			if (typeA == VarType.TYPE_VOID || typeB == VarType.TYPE_VOID)
				return VarType.TYPE_ERROR;

			if (typeA == typeB && equalOp)
				return VarType.TYPE_BOOL;

			if (IsNumType(typeA) && IsNumType(typeB))
				return VarType.TYPE_BOOL;

			return VarType.TYPE_ERROR;
		}

		internal static bool IsValidLeftValue(UnaryNode unary)
		{
			if (unary.unaryOp.Count > 0)
				return false;
			if (!(unary.factor.value is IdNode))
				return false;
			if (SymbolTable.FindVar((unary.factor.value as IdNode).name).isConst)
				return false;
			return true;
		}
	}
}