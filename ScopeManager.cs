using System.Collections.Generic;

namespace KayoCompiler
{
	static class ScopeManager
	{
		// 函数局部变量数量，仅在Parse阶段使用，Generate阶段不使用
		internal static int LocalVarCount { get; set; } = 0;

		// 当前所在作用域编号
		internal static int CurrentScope
		{
			get
			{
				return scopePath.Peek();
			}
		}

		internal static string CurrentFun = "test_main";

		private static Stack<int> scopePath = new Stack<int>();

		// 作用域编号计数器，结束Parse后使用ScopeManager.ScopeIdReset()重置
		private static int ScopeId { get; set; } = 0;

		internal static bool IsVisibleNow(int scopeId)
		{
			return scopePath.Contains(scopeId);
		}

		internal static void LocalVarCountReset()
		{
			LocalVarCount = 0;
		}

		internal static void ScopeEnter()
		{
			ScopeId++;
			scopePath.Push(ScopeId);
		}

		internal static void ScopeLeave()
		{
			scopePath.Pop();
		}

		internal static void ScopeIdReset()
		{
			scopePath.Clear();
			ScopeId = 0;
		}
	}
}