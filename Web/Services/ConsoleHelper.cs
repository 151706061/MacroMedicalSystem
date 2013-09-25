#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using Macro.Common;

namespace Macro.Web.Services
{
	internal class ConsoleHelper
	{
		public static void Log(LogLevel logLevel, ConsoleColor color, string format, params object[] args)
		{
#if DEBUG
			var oldConsoleColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Platform.Log(logLevel, format, args);
			Console.ForegroundColor = oldConsoleColor;
#endif
		}

		public static void WriteLine(ConsoleColor color, string format, params object[] args)
		{
#if DEBUG
			var oldConsoleColor = Console.ForegroundColor;
			Console.ForegroundColor = color;
			Console.WriteLine(format, args);
			Console.ForegroundColor = oldConsoleColor;
#endif
		}
	}
}