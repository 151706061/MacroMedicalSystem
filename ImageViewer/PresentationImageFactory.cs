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
using Macro.Dicom.Iod;
using Macro.Dicom.Iod.Iods;
using Macro.ImageViewer.KeyObjects;
using Macro.ImageViewer.PresentationStates.Dicom;
using Macro.ImageViewer.StudyManagement;
using Macro.ImageViewer.PresentationStates;

namespace Macro.ImageViewer
{
	/// <summary>
	/// Defines the factory methods for creating <see cref="IPresentationImage"/>s.
	/// </summary>
	public interface IPresentationImageFactory
	{
		/// <summary>
		/// Sets the <see cref="StudyTree"/> to be used by the <see cref="IPresentationImageFactory"/> when resolving referenced SOPs.
		/// </summary>
		/// <param name="studyTree">The <see cref="StudyTree"/> to be used for resolving referenced SOPs.</param>
		void SetStudyTree(StudyTree studyTree);

		/// <summary>
		/// Creates the presentation images for a given image SOP.
		/// </summary>
		/// <param name="sop">The image SOP from which presentation images are to be created.</param>
		/// <returns>A list of created presentation images.</returns>
		List<IPresentationImage> CreateImages(Sop sop);

		/// <summary>
		/// Creates the presentation image for a given image frame.
		/// </summary>
		/// <param name="frame">The image frame from which a presentation image is to be created.</param>
		/// <returns>The created presentation image.</returns>
		IPresentationImage CreateImage(Frame frame);
	}

	/// <summary>
	/// A factory class which creates <see cref="IPresentationImage"/>s.
	/// </summary>
	public class PresentationImageFactory : IPresentationImageFactory
	{
		private static readonly PresentationImageFactory _defaultInstance = new PresentationImageFactory();

		private StudyTree _studyTree;

		/// <summary>
		/// Constructs a <see cref="PresentationImageFactory"/>.
		/// </summary>
		public PresentationImageFactory()
		{
		}

		public PresentationState DefaultPresentationState {get; set; }

		/// <summary>
		/// Gets the <see cref="StudyTree"/> used by the factory to resolve referenced SOPs.
		/// </summary>
		protected StudyTree StudyTree
		{
			get { return _studyTree; }
		}

		#region IPresentationImageFactory Members

		void IPresentationImageFactory.SetStudyTree(StudyTree studyTree)
		{
			_studyTree = studyTree;
		}

		IPresentationImage IPresentationImageFactory.CreateImage(Frame frame)
		{
			return Create(frame);
		}

		#endregion

		/// <summary>
		/// Creates the presentation image for a given image frame.
		/// </summary>
		/// <param name="frame">The image frame from which a presentation image is to be created.</param>
		/// <returns>The created presentation image.</returns>
		protected virtual IPresentationImage CreateImage(Frame frame)
		{
			if (frame.PhotometricInterpretation == PhotometricInterpretation.Unknown)
				throw new Exception("Photometric interpretation is unknown.");

			IDicomPresentationImage image;

			if (!frame.PhotometricInterpretation.IsColor)
				image = new DicomGrayscalePresentationImage(frame);
			else
				image = new DicomColorPresentationImage(frame);

			if (image.PresentationState == null || Equals(image.PresentationState, PresentationState.DicomDefault))
				image.PresentationState = DefaultPresentationState;

			return image;
		}

		/// <summary>
		/// Creates the presentation images for a given image SOP.
		/// </summary>
		/// <param name="imageSop">The image SOP from which presentation images are to be created.</param>
		/// <returns>A list of created presentation images.</returns>
		protected virtual List<IPresentationImage> CreateImages(ImageSop imageSop)
		{
			return CollectionUtils.Map(imageSop.Frames, (Frame frame) => CreateImage(frame));
		}

		/// <summary>
		/// Creates the presentation images for a given image SOP.
		/// </summary>
		/// <param name="sop">The image SOP from which presentation images are to be created.</param>
		/// <returns>A list of created presentation images.</returns>
		public virtual List<IPresentationImage> CreateImages(Sop sop)
		{
			if (sop.IsImage)
				return CreateImages((ImageSop)sop);
			
			if (sop.SopClassUid == SopClass.KeyObjectSelectionDocumentStorageUid)
				return CreateImages(new KeyObjectSelectionDocumentIod(sop.DataSource));

			return new List<IPresentationImage>();
		}

		/// <summary>
		/// Creates the presentation images for a given key object selection document.
		/// </summary>
		/// <param name="keyObjectDocument">The key object selection document from which presentation images are to be created.</param>
		/// <returns>A list of created presentation images.</returns>
		protected virtual List<IPresentationImage> CreateImages(KeyObjectSelectionDocumentIod keyObjectDocument)
		{
			List<IPresentationImage> images = new List<IPresentationImage>();
			if (_studyTree == null)
			{
				Platform.Log(LogLevel.Warn, "Key object document cannot be used to create images because there is no study tree to build from.");
			}
			else
			{
				IList<IKeyObjectContentItem> content = new KeyImageDeserializer(keyObjectDocument).Deserialize();
				foreach (IKeyObjectContentItem item in content)
				{
					if (item is KeyImageContentItem)
						images.AddRange(CreateImages((KeyImageContentItem) item));
					else
						Platform.Log(LogLevel.Warn, "Unsupported key object content value type");
				}
			}

			return images;
		}

		protected virtual List<IPresentationImage> CreateImages(KeyImageContentItem item)
		{
			List<IPresentationImage> images = new List<IPresentationImage>();

			ImageSop imageSop = FindReferencedImageSop(item.ReferencedImageSopInstanceUid, item.Source.GeneralStudy.StudyInstanceUid);
			if (imageSop != null)
			{

				int frameNumber = item.FrameNumber.GetValueOrDefault(-1);
				if (item.FrameNumber.HasValue)
				{
					// FramesCollection is a 1-based index!!!
					if (frameNumber > 0 && frameNumber <= imageSop.Frames.Count)
					{
						images.Add(Create(imageSop.Frames[frameNumber]));
					}
					else
					{
						Platform.Log(LogLevel.Error, "The referenced key image {0} does not have a frame {1} (referenced in Key Object Selection {2})", item.ReferencedImageSopInstanceUid, frameNumber, item.Source.SopCommon.SopInstanceUid);
						images.Add(new KeyObjectPlaceholderImage(SR.MessageReferencedKeyImageFrameNotFound));
					}
				}
				else
				{
					foreach (Frame frame in imageSop.Frames)
					{
						images.Add(Create(frame));
					}
				}

				Sop presentationStateSop = FindReferencedSop(item.PresentationStateSopInstanceUid, item.Source.GeneralStudy.StudyInstanceUid);
				if (presentationStateSop != null)
				{
					foreach (IPresentationImage image in images)
					{
						if (image is IPresentationStateProvider)
						{
							try
							{
								IPresentationStateProvider presentationStateProvider = (IPresentationStateProvider)image;
								presentationStateProvider.PresentationState = DicomSoftcopyPresentationState.Load(presentationStateSop.DataSource);
							}
							catch (Exception ex)
							{
								Platform.Log(LogLevel.Warn, ex, SR.MessagePresentationStateReadFailure);
							}
						}
					}
				}
			}
			else
			{
				Platform.Log(LogLevel.Warn, "The referenced key image {0} is not loaded as part of the current study (referenced in Key Object Selection {1})", item.ReferencedImageSopInstanceUid, item.Source.SopCommon.SopInstanceUid);
				images.Add(new KeyObjectPlaceholderImage(SR.MessageReferencedKeyImageFromOtherStudy));
			}

			return images;
		}

		/// <summary>
		/// Creates an appropriate subclass of <see cref="BasicPresentationImage"/>
		/// for each <see cref="Frame"/> in the input <see cref="ImageSop"/>.
		/// </summary>
		public static List<IPresentationImage> Create(ImageSop imageSop)
		{
			return _defaultInstance.CreateImages(imageSop);
		}

		/// <summary>
		/// Creates an appropriate subclass of <see cref="BasicPresentationImage"/>
		/// based on the <see cref="Frame"/>'s photometric interpretation.
		/// </summary>
		public static IPresentationImage Create(Frame frame)
		{
			return _defaultInstance.CreateImage(frame);
		}

		#region Private KO Helpers

		//TODO (CR Mar 2010): return Sop and check IsImage.
		private ImageSop FindReferencedImageSop(string sopInstanceUid, string studyInstanceUid)
		{
			if (string.IsNullOrEmpty(sopInstanceUid))
				return null;

			string sameStudyUid = studyInstanceUid;
			Study sameStudy = _studyTree.GetStudy(sameStudyUid);

			if (sameStudy != null)
			{
				foreach (Series series in sameStudy.Series)
				{
					Sop referencedSop = series.Sops[sopInstanceUid];
					if (referencedSop != null)
						return referencedSop as ImageSop;
				}
			}

			return null;
		}

		private Sop FindReferencedSop(string sopInstanceUid, string studyInstanceUid)
		{
			if (string.IsNullOrEmpty(sopInstanceUid))
				return null;

			string sameStudyUid = studyInstanceUid;
			Study sameStudy = _studyTree.GetStudy(sameStudyUid);

			if (sameStudy != null)
			{
				foreach (Series series in sameStudy.Series)
				{
					Sop referencedSop = series.Sops[sopInstanceUid];
					if (referencedSop != null)
						return referencedSop;
				}
			}

			return null;
		}

		#endregion
	}
}