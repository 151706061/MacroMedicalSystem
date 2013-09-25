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
using System.Reflection;

namespace Macro.Web.Common
{
	#region Contract Level

	[ExtensionPoint]
	public sealed class KnownTypeProviderExtensionPoint<TBaseType> : ExtensionPoint<IKnownTypeProvider>
	{
		public KnownTypeProviderExtensionPoint()
		{}

		public static IEnumerable<Type> GetKnownTypes()
		{

			var types = new Dictionary<Type, Type>();

			try
			{
				var xp = new KnownTypeProviderExtensionPoint<TBaseType>();
				foreach (IKnownTypeProvider provider in xp.CreateExtensions())
				{
					foreach (var type in provider.GetKnownTypes())
						types[type] = type;
				}
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, String.Format("No extensions found for type {0}", typeof(TBaseType)));
			}

			foreach (var type in types.Keys)
				yield return type;
		}
	}

	public interface IKnownTypeProvider
	{
		IEnumerable<Type> GetKnownTypes();
	}

	#endregion

	public sealed class ServiceKnownTypeExtensionPoint : ExtensionPoint<IServiceKnownTypeProvider>
	{
		public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider ignored)
		{

			var types = new Dictionary<Type, Type>();

			try
			{
				var xp = new ServiceKnownTypeExtensionPoint();
				foreach (IServiceKnownTypeProvider provider in xp.CreateExtensions())
				{
					foreach (var type in provider.GetKnownTypes(ignored))
						types[type] = type;
				}
			}
			catch (NotSupportedException)
			{
				Platform.Log(LogLevel.Debug, String.Format("No service known type extensions found"));
			}

			foreach (var type in types.Keys)
				yield return type;
		}
	}

	public interface IServiceKnownTypeProvider
	{
		IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider);
	}
}