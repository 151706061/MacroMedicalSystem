#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System.Collections.Generic;
using System.Threading;

namespace Macro.ImageViewer.Common
{
	public class BufferCache<T>
	{
		private readonly int _maxSize;
		private readonly bool _synchronized;

		private readonly object _syncLock = new object();
		private readonly List<T[]> _buffers = new List<T[]>();

		public BufferCache(int maxSize, bool synchronized)
		{
			_maxSize = maxSize;
			_synchronized = synchronized;
		}

		public T[] Allocate(int length)
		{
			if (_synchronized)
				Monitor.Enter(_syncLock);

			try
			{
				foreach (T[] buffer in _buffers)
				{
					if (buffer.Length == length)
					{
						_buffers.Remove(buffer);
						//Trace.WriteLine("BufferCache: allocated from cache", "Memory");
						return buffer;
					}
				}

				//Trace.WriteLine("BufferCache: allocated from MemoryManager", "Memory");
				return MemoryManager.Allocate<T>(length);
			}
			finally
			{
				if (_synchronized)
					Monitor.Exit(_syncLock);
			}
		}

		public void Return(T[] buffer)
		{
			if (_synchronized)
				Monitor.Enter(_syncLock);

			try
			{
				if (_buffers.Count >= _maxSize)
					_buffers.RemoveAt(0);

				//Trace.WriteLine("BufferCache: buffer returned", "Memory");
				_buffers.Add(buffer);
			}
			finally
			{
				if (_synchronized)
					Monitor.Exit(_syncLock);
			}
		}
	}
}
