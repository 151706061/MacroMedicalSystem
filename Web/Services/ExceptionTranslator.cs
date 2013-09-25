#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using Macro.Common;

namespace Macro.Web.Services
{
	[ExtensionPoint]
	public sealed class ExceptionTranslatorExtensionPoint : ExtensionPoint<IExceptionTranslator>
	{}

	public interface IExceptionTranslator
	{
		string Translate(Exception e);
	}

	public class ExceptionTranslator
	{
		private static readonly List<IExceptionTranslator> _translators;

		static ExceptionTranslator()
		{
			_translators = new List<IExceptionTranslator>();
			try
			{
				foreach (IExceptionTranslator translator in new ExceptionTranslatorExtensionPoint().CreateExtensions())
					_translators.Add(translator);
			}
			catch (NotSupportedException)
			{
			}
		}

		public static string Translate(Exception e)
		{
			return Translate(e, false);
		}

		public static string Translate(Exception e, bool returnExceptionMessage)
		{
            //TODO: returnExceptionMessage is always false
		    Platform.Log(LogLevel.Error, e, "Exception has occurred");
			return DoTranslate(e) ?? (returnExceptionMessage ? e.Message : SR.MessageUnexpectedError);
		}

		private static string DoTranslate(Exception e)
		{
			foreach (var translator in _translators)
			{
				string message = translator.Translate(e);
				if (!String.IsNullOrEmpty(message))
					return message;
			}

			if (e.InnerException != null)
				return DoTranslate(e.InnerException);

			return null;
		}
	}
}
