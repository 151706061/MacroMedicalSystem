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
using Macro.Dicom.Iod.Macros;
using Macro.Dicom.Iod.Macros.DocumentRelationship;
using Macro.Dicom.Iod.Macros.ImageReference;
using Macro.Dicom.Iod.Sequences;

namespace Macro.Dicom.Iod.Modules
{
	/// <summary>
	/// SrDocumentContent Module
	/// </summary>
	/// <remarks>As defined in the DICOM Standard 2008, Part 3, Section C.17.3 (Table C.17-4)</remarks>
	public class SrDocumentContentModuleIod : IodBase, IDocumentContentMacro, IDocumentRelationshipMacro
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SrDocumentContentModuleIod"/> class.
		/// </summary>	
		public SrDocumentContentModuleIod() : base() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SrDocumentContentModuleIod"/> class.
		/// </summary>
		public SrDocumentContentModuleIod(IDicomAttributeProvider dicomAttributeProvider) : base(dicomAttributeProvider) { }

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence using default values.
		/// </summary>
		public virtual void InitializeAttributes()
		{
			this.ConceptNameCodeSequence = null;
			this.TextValue = null;
			this.DateTime = null;
			this.Date = null;
			this.Time = null;
			this.PersonName = null;
			this.Uid = null;
			this.ObservationDateTime = null;
			this.ContentSequence = null;
		}

		#region IDocumentContentMacro Members

		/// <summary>
		/// Gets or sets the value of ValueType in the underlying collection. Type 1.
		/// </summary>
		public virtual ValueType ValueType
		{
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.ValueType].GetString(0, string.Empty), ValueType.None); }
			set
			{
				if (value == ValueType.None)
					throw new ArgumentNullException("value", "ValueType is a required Type 1.");
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.ValueType], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ConceptNameCodeSequence in the underlying collection. Type 1C.
		/// </summary>
		public CodeSequenceMacro ConceptNameCodeSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ConceptNameCodeSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}
				return new CodeSequenceMacro(((DicomSequenceItem[]) dicomAttribute.Values)[0]);
			}
			set
			{
				if (value == null)
				{
					base.DicomAttributeProvider[DicomTags.ConceptNameCodeSequence] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ConceptNameCodeSequence].Values = new DicomSequenceItem[] {value.DicomSequenceItem};
			}
		}

		/// <summary>
		/// Gets or sets the value of TextValue in the underlying collection. Type 1C.
		/// </summary>
		public string TextValue
		{
			get { return base.DicomAttributeProvider[DicomTags.TextValue].ToString(); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.TextValue] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.TextValue].SetStringValue(value);
			}
		}

		/// <summary>
		/// Gets or sets the value of DateTime in the underlying collection. Type 1C.
		/// </summary>
		public DateTime? DateTime
		{
			get { return base.DicomAttributeProvider[DicomTags.Datetime].GetDateTime(0); }
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.Datetime] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.Datetime].SetDateTime(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of Date in the underlying collection. Type 1C.
		/// </summary>
		public DateTime? Date
		{
			get { return base.DicomAttributeProvider[DicomTags.Date].GetDateTime(0); }
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.Date] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.Date].SetDateTime(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of Time in the underlying collection. Type 1C.
		/// </summary>
		public DateTime? Time
		{
			get { return base.DicomAttributeProvider[DicomTags.Time].GetDateTime(0); }
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.Time] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.Time].SetDateTime(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of PersonName in the underlying collection. Type 1C.
		/// </summary>
		public string PersonName
		{
			get { return base.DicomAttributeProvider[DicomTags.PersonName].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.PersonName] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.PersonName].SetString(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of Uid in the underlying collection. Type 1C.
		/// </summary>
		public string Uid
		{
			get { return base.DicomAttributeProvider[DicomTags.Uid].GetString(0, string.Empty); }
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					base.DicomAttributeProvider[DicomTags.Uid] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.Uid].SetString(0, value);
			}
		}

		#endregion

		#region IIodMacro Members

		/// <summary>
		/// Gets the dicom attribute collection as a dicom sequence item.
		/// </summary>
		/// <value>The dicom sequence item.</value>
		DicomSequenceItem IIodMacro.DicomSequenceItem
		{
			get { return DicomAttributeProvider as DicomSequenceItem; }
			set { DicomAttributeProvider = value; }
		}

		#endregion

		#region IDocumentRelationshipMacro Members

		/// <summary>
		/// Gets or sets the value of ObservationDateTime in the underlying collection. Type 1C.
		/// </summary>
		public DateTime? ObservationDateTime
		{
			get { return base.DicomAttributeProvider[DicomTags.ObservationDateTime].GetDateTime(0); }
			set
			{
				if (!value.HasValue)
				{
					base.DicomAttributeProvider[DicomTags.ObservationDateTime] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ObservationDateTime].SetDateTime(0, value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContentSequence in the underlying collection. Type 1C.
		/// </summary>
		public IContentSequence[] ContentSequence
		{
			get
			{
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ContentSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0)
				{
					return null;
				}

				IContentSequence[] result = new IContentSequence[dicomAttribute.Count];
				DicomSequenceItem[] items = (DicomSequenceItem[]) dicomAttribute.Values;
				for (int n = 0; n < items.Length; n++)
					result[n] = new ContentSequenceType(items[n]);

				return result;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					base.DicomAttributeProvider[DicomTags.ContentSequence] = null;
					return;
				}

				DicomSequenceItem[] result = new DicomSequenceItem[value.Length];
				for (int n = 0; n < value.Length; n++)
					result[n] = value[n].DicomSequenceItem;

				base.DicomAttributeProvider[DicomTags.ContentSequence].Values = result;
			}
		}

		/// <summary>
		/// Creates a single instance of a ContentSequence item. Does not modify the ContentSequence in the underlying collection.
		/// </summary>
		public IContentSequence CreateContentSequence()
		{
			ContentSequenceType iodBase = new ContentSequenceType(new DicomSequenceItem());
			iodBase.InitializeAttributes();
			return iodBase;
		}

		#endregion

		#region IImageReferenceMacro Members

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence with a value type of IMAGE using default values.
		/// </summary>
		public IImageReferenceMacro InitializeImageReferenceAttributes() {
			this.InitializeAttributes();
			this.ValueType = ValueType.Image;
			((IImageReferenceMacro)this).CreateReferencedSopSequence();
			((IImageReferenceMacro)this).ReferencedSopSequence.InitializeAttributes();
			return this;
		}

		/// <summary>
		/// Gets or sets the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		ISopInstanceReferenceMacro ICompositeObjectReferenceMacro.ReferencedSopSequence {
			get { return ((IImageReferenceMacro)this).ReferencedSopSequence; }
			set { ((IImageReferenceMacro)this).ReferencedSopSequence = (IReferencedSopSequence)value; }
		}

		/// <summary>
		/// Gets or sets the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		IReferencedSopSequence IImageReferenceMacro.ReferencedSopSequence {
			get {
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedSopSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0) {
					return null;
				}
				return new ImageReferenceMacro.ReferencedSopSequenceType(((DicomSequenceItem[])dicomAttribute.Values)[0]);
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value", "ReferencedSopSequence is Type 1 Required.");
				base.DicomAttributeProvider[DicomTags.ReferencedSopSequence].Values = new DicomSequenceItem[] { value.DicomSequenceItem };
			}
		}

		/// <summary>
		/// Creates the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		ISopInstanceReferenceMacro ICompositeObjectReferenceMacro.CreateReferencedSopSequence() {
			return ((IImageReferenceMacro)this).CreateReferencedSopSequence();
		}

		/// <summary>
		/// Creates the value of ReferencedSopSequence in the underlying collection. Type 1.
		/// </summary>
		IReferencedSopSequence IImageReferenceMacro.CreateReferencedSopSequence() {
			DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ReferencedSopSequence];
			if (dicomAttribute.IsNull || dicomAttribute.Count == 0) {
				DicomSequenceItem dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new DicomSequenceItem[] { dicomSequenceItem };
				ImageReferenceMacro.ReferencedSopSequenceType iodBase = new ImageReferenceMacro.ReferencedSopSequenceType(dicomSequenceItem);
				iodBase.InitializeAttributes();
				return iodBase;
			}
			return new ImageReferenceMacro.ReferencedSopSequenceType(((DicomSequenceItem[])dicomAttribute.Values)[0]);
		}

		#endregion

		#region IContainerMacro Members

		/// <summary>
		/// Initializes the underlying collection to implement the module or sequence with a value type of CONTAINER using default values.
		/// </summary>
		public IContainerMacro InitializeContainerAttributes() {
			this.InitializeAttributes();
			this.ValueType = ValueType.Container;
			((IContainerMacro)this).ContinuityOfContent = ContinuityOfContent.Separate;
			((IContainerMacro)this).ContentTemplateSequence = null;
			return this;
		}

		/// <summary>
		/// Gets or sets the value of ContinuityOfContent in the underlying collection. Type 1.
		/// </summary>
		ContinuityOfContent IContainerMacro.ContinuityOfContent {
			get { return ParseEnum(base.DicomAttributeProvider[DicomTags.ContinuityOfContent].GetString(0, string.Empty), ContinuityOfContent.Unknown); }
			set {
				if (value == ContinuityOfContent.Unknown)
					throw new ArgumentNullException("value", "Continuity of Content is Type 1 Required.");
				SetAttributeFromEnum(base.DicomAttributeProvider[DicomTags.ContinuityOfContent], value);
			}
		}

		/// <summary>
		/// Gets or sets the value of ContentTemplateSequence in the underlying collection. Type 1C.
		/// </summary>
		ContentTemplateSequence IContainerMacro.ContentTemplateSequence {
			get {
				DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ContentTemplateSequence];
				if (dicomAttribute.IsNull || dicomAttribute.Count == 0) {
					return null;
				}
				return new ContentTemplateSequence(((DicomSequenceItem[])dicomAttribute.Values)[0]);
			}
			set {
				if (value == null) {
					base.DicomAttributeProvider[DicomTags.ContentTemplateSequence] = null;
					return;
				}
				base.DicomAttributeProvider[DicomTags.ContentTemplateSequence].Values = new DicomSequenceItem[] { value.DicomSequenceItem };
			}
		}

		/// <summary>
		/// Creates the value of ContentTemplateSequence in the underlying collection. Type 1C.
		/// </summary>
		ContentTemplateSequence IContainerMacro.CreateContentTemplateSequence() {
			DicomAttribute dicomAttribute = base.DicomAttributeProvider[DicomTags.ContentTemplateSequence];
			if (dicomAttribute.IsNull || dicomAttribute.Count == 0) {
				DicomSequenceItem dicomSequenceItem = new DicomSequenceItem();
				dicomAttribute.Values = new DicomSequenceItem[] { dicomSequenceItem };
				ContentTemplateSequence iodBase = new ContentTemplateSequence(dicomSequenceItem);
				iodBase.InitializeAttributes();
				return iodBase;
			}
			return new ContentTemplateSequence(((DicomSequenceItem[])dicomAttribute.Values)[0]);
		}

		#endregion
	}
}