namespace KayoCompiler
{
	internal static class Utils
	{
        internal static bool IsNumType(VarType type)
        {
            return type == VarType.TYPE_INT || type == VarType.TYPE_LONG || type == VarType.TYPE_CHAR;
        }

		internal static bool IsTypeTag(Tag tag, bool includeVoid)
		{
			Tag[] typeTags =
			{
				Tag.KW_BOOL,
				Tag.KW_CHAR,
				Tag.KW_INT,
				Tag.KW_LONG
			};
			bool result = false;

			if (includeVoid)
				result = tag == Tag.KW_VOID;
			foreach (Tag typeTag in typeTags)
			{
				result = result || tag == typeTag;
			}

			return result;
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
	}
}