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

using System;
using System.Collections.Generic;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.Dicom;
using Macro.ImageViewer.Volume.Mpr.Utilities;

namespace Macro.ImageViewer.Volume.Mpr
{
	//TODO (cr Oct 2009): realized after code review that these could be display set descriptors ... not sure how it would work, though.
	public interface IMprSliceSet : IDisposable
	{
		string Uid { get; }
		string Description { get; }
		Volume Volume { get; }
		IMprVolume Parent { get; }
		IList<MprSliceSop> SliceSops { get; }
		event EventHandler SliceSopsChanged;
	}

	public abstract class MprSliceSet : IInternalMprSliceSet
	{
		private event EventHandler _sliceSopsChanged;
		private readonly string _uid = DicomUid.GenerateUid().UID;
		private bool _sliceSopsChangedSuspended = false;
		private string _description = string.Empty;
		private IMprVolume _parent;
		private IVolumeReference _volume;
		private ObservableDisposableList<MprSliceSop> _sliceSops;

		protected MprSliceSet(Volume volume)
		{
			Platform.CheckForNullReference(volume, "volume");
			_volume = volume.CreateTransientReference();

			_sliceSops = new ObservableDisposableList<MprSliceSop>();
			_sliceSops.EnableEvents = true;
			_sliceSops.ItemAdded += OnItemAdded;
			_sliceSops.ItemChanged += OnItemChanged;
			_sliceSops.ItemChanging += OnItemChanging;
			_sliceSops.ItemRemoved += OnItemRemoved;
		}

		public string Uid
		{
			get { return _uid; }
		}

		public string Description
		{
			get { return _description; }
			protected set { _description = value; }
		}

		public Volume Volume
		{
			get { return _volume.Volume; }
		}

		public IList<MprSliceSop> SliceSops
		{
			get { return _sliceSops; }
		}

		public event EventHandler SliceSopsChanged
		{
			add { _sliceSopsChanged += value; }
			remove { _sliceSopsChanged -= value; }
		}

		public IMprVolume Parent
		{
			get { return _parent; }
		}

		IMprVolume IInternalMprSliceSet.Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		protected void SuspendSliceSopsChangedEvent()
		{
			_sliceSopsChangedSuspended = true;
		}

		protected void ResumeSliceSopsChangedEvent(bool fireNow)
		{
			_sliceSopsChangedSuspended = false;
			if (fireNow)
				this.OnSliceSopsChanged();
		}

		protected void ClearAndDisposeSops()
		{
			// not quite the same as ObservableDisposableList<Sop>.Dispose() since we want to keep our list!
			bool enableEvents = _sliceSops.EnableEvents;
			_sliceSops.EnableEvents = false;
			try
			{
				List<MprSliceSop> temp = new List<MprSliceSop>(_sliceSops);
				_sliceSops.Clear();
				foreach (MprSliceSop sop in temp)
				{
					sop.Parent = null;
					sop.Dispose();
				}
			}
			finally
			{
				_sliceSops.EnableEvents = enableEvents;
			}
		}

		protected virtual void OnSliceSopRemoved(MprSliceSop item)
		{
			item.Parent = null;
		}

		protected virtual void OnSliceSopAdded(MprSliceSop item)
		{
			item.Parent = this;
		}

		protected virtual void OnSliceSopsChanged()
		{
			EventsHelper.Fire(_sliceSopsChanged, this, EventArgs.Empty);
		}

		private void OnItemAdded(object sender, ListEventArgs<MprSliceSop> e)
		{
			this.OnSliceSopAdded(e.Item);
			if (!_sliceSopsChangedSuspended)
				this.OnSliceSopsChanged();
		}

		private void OnItemRemoved(object sender, ListEventArgs<MprSliceSop> e)
		{
			this.OnSliceSopRemoved(e.Item);
			if (!_sliceSopsChangedSuspended)
				this.OnSliceSopsChanged();
		}

		private void OnItemChanging(object sender, ListEventArgs<MprSliceSop> e)
		{
			this.OnSliceSopRemoved(e.Item);
		}

		private void OnItemChanged(object sender, ListEventArgs<MprSliceSop> e)
		{
			this.OnSliceSopAdded(e.Item);
			if (!_sliceSopsChangedSuspended)
				this.OnSliceSopsChanged();
		}

		#region Disposal

		public void Dispose()
		{
			try
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_sliceSops != null)
				{
					_sliceSops.ItemAdded -= OnItemAdded;
					_sliceSops.ItemChanged -= OnItemChanged;
					_sliceSops.ItemChanging -= OnItemChanging;
					_sliceSops.ItemRemoved -= OnItemRemoved;
					_sliceSops.Dispose();
					_sliceSops = null;
				}

				if (_volume != null)
				{
					_volume.Dispose();
					_volume = null;
				}
			}
		}

		#endregion
	}
}