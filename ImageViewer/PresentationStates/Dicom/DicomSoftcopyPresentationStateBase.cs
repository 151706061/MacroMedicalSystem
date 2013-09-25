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
using System.Drawing;
using Macro.Common;
using Macro.Common.Utilities;
using Macro.Dicom;
using Macro.Dicom.Iod;
using Macro.Dicom.Iod.Macros;
using Macro.Dicom.Iod.Macros.PresentationStateRelationship;
using Macro.Dicom.Iod.Modules;
using Macro.Dicom.Iod.Sequences;
using Macro.ImageViewer.Graphics;
using Macro.ImageViewer.Mathematics;

namespace Macro.ImageViewer.PresentationStates.Dicom
{
	[Cloneable]
	internal abstract class DicomSoftcopyPresentationStateBase<T> : DicomSoftcopyPresentationState where T : IDicomPresentationImage
	{
		protected DicomSoftcopyPresentationStateBase(SopClass psSopClass) : base(psSopClass) {}

		protected DicomSoftcopyPresentationStateBase(SopClass psSopClass, DicomFile dicomFile) : base(psSopClass, dicomFile) {}

		protected DicomSoftcopyPresentationStateBase(SopClass psSopClass, DicomAttributeCollection dataSource) : base(psSopClass, dataSource) {}

		protected DicomSoftcopyPresentationStateBase(DicomSoftcopyPresentationStateBase<T> source, ICloningContext context) : base(source, context)
		{
			context.CloneFields(source, this);
		}

		protected override void PerformSerialization(IEnumerable<IPresentationImage> images)
		{
			DicomPresentationImageCollection<T> imageCollection = new DicomPresentationImageCollection<T>();
			foreach (IPresentationImage image in images)
			{
				if (image is T)
					imageCollection.Add((T) image);
			}

			if (imageCollection.Count == 0)
				return;

			// Initialize the Patient IE using source image information
			InitializePatientModule(new PatientModuleIod(base.DataSet), imageCollection.FirstImage);
			InitializeClinicalTrialSubjectModule(new ClinicalTrialSubjectModuleIod(base.DataSet), imageCollection.FirstImage);

			// Initialize the Study IE using source image information
			InitializeGeneralStudyModule(new GeneralStudyModuleIod(base.DataSet), imageCollection.FirstImage);
			InitializePatientStudyModule(new PatientStudyModuleIod(base.DataSet), imageCollection.FirstImage);
			InitializeClinicalTrialStudyModule(new ClinicalTrialStudyModuleIod(base.DataSet), imageCollection.FirstImage);

			this.PerformTypeSpecificSerialization(imageCollection);
		}

		protected override sealed void PerformDeserialization(IEnumerable<IPresentationImage> images)
		{
			foreach (PresentationStateRelationshipModuleIod psRelationship in this.RelationshipSets)
			{
				SeriesReferenceDictionary dictionary = new SeriesReferenceDictionary(psRelationship.ReferencedSeriesSequence);

				DicomPresentationImageCollection<T> imageCollection = new DicomPresentationImageCollection<T>();
				foreach (IPresentationImage image in images)
				{
					if (image is T)
					{
						T tImage = (T) image;
						if (dictionary.ReferencesFrame(tImage.ImageSop.SeriesInstanceUid, tImage.ImageSop.SopInstanceUid, tImage.Frame.FrameNumber))
							imageCollection.Add(tImage);
					}
				}

				this.PerformTypeSpecificDeserialization(imageCollection);
			}
		}

		protected abstract void PerformTypeSpecificSerialization(DicomPresentationImageCollection<T> images);
		protected abstract void PerformTypeSpecificDeserialization(DicomPresentationImageCollection<T> images);

		/// <summary>
		/// Gets a <see cref="PresentationStateRelationshipModuleIod"/> for this data set.
		/// </summary>
		/// <remarks>
		/// As of the 2008 version of the DICOM standard, only the Blended Softcopy Presentation State IOD defines
		/// multiple presentation state relationship modules (as part of the presentation state blending module).
		/// Thus, only the implementation of the Blended Softcopy Presentation State should override this member
		/// to provide all the individual relationship modules. The default implementation assumes the module
		/// is available as a root member of the IOD.
		/// </remarks>
		protected virtual IEnumerable<PresentationStateRelationshipModuleIod> RelationshipSets
		{
			get { return new PresentationStateRelationshipModuleIod[] {new PresentationStateRelationshipModuleIod(this.DataSet)}; }
		}

		#region Serialization of Demographic/Study Data

		private static void InitializePatientModule(PatientModuleIod patientModule, T prototypeImage)
		{
			PatientModuleIod srcPatient = new PatientModuleIod(prototypeImage.ImageSop.DataSource);
			patientModule.BreedRegistrationSequence = srcPatient.BreedRegistrationSequence;
			patientModule.DeIdentificationMethod = srcPatient.DeIdentificationMethod;
			patientModule.DeIdentificationMethodCodeSequence = srcPatient.DeIdentificationMethodCodeSequence;
			patientModule.EthnicGroup = srcPatient.EthnicGroup;
			patientModule.IssuerOfPatientId = srcPatient.IssuerOfPatientId;
			patientModule.OtherPatientIds = srcPatient.OtherPatientIds;
			patientModule.OtherPatientIdsSequence = srcPatient.OtherPatientIdsSequence;
			patientModule.OtherPatientNames = srcPatient.OtherPatientNames;
			patientModule.PatientBreedCodeSequence = srcPatient.PatientBreedCodeSequence;
			patientModule.PatientBreedDescription = srcPatient.PatientBreedDescription;
			patientModule.PatientComments = srcPatient.PatientComments;
			patientModule.PatientId = srcPatient.PatientId;
			patientModule.PatientIdentityRemoved = srcPatient.PatientIdentityRemoved;
			patientModule.PatientsBirthDateTime = srcPatient.PatientsBirthDateTime;
			patientModule.PatientsName = srcPatient.PatientsName;
			patientModule.PatientSpeciesCodeSequence = srcPatient.PatientSpeciesCodeSequence;
			patientModule.PatientSpeciesDescription = srcPatient.PatientSpeciesDescription;
			patientModule.PatientsSex = srcPatient.PatientsSex;
			patientModule.ReferencedPatientSequence = srcPatient.ReferencedPatientSequence;
			patientModule.ResponsibleOrganization = srcPatient.ResponsibleOrganization;
			patientModule.ResponsiblePerson = srcPatient.ResponsiblePerson;
			patientModule.ResponsiblePersonRole = srcPatient.ResponsiblePersonRole;
		}

		private static void InitializeClinicalTrialSubjectModule(ClinicalTrialSubjectModuleIod clinicalTrialSubjectModule, T prototypeImage)
		{
			ClinicalTrialSubjectModuleIod srcTrialSubject = new ClinicalTrialSubjectModuleIod(prototypeImage.ImageSop.DataSource);
			if (srcTrialSubject.HasValues()) // clinical trial subkect module is user optional
			{
				clinicalTrialSubjectModule.ClinicalTrialProtocolId = srcTrialSubject.ClinicalTrialProtocolId;
				clinicalTrialSubjectModule.ClinicalTrialProtocolName = srcTrialSubject.ClinicalTrialProtocolName;
				clinicalTrialSubjectModule.ClinicalTrialSiteId = srcTrialSubject.ClinicalTrialSiteId;
				clinicalTrialSubjectModule.ClinicalTrialSiteName = srcTrialSubject.ClinicalTrialSiteName;
				clinicalTrialSubjectModule.ClinicalTrialSponsorName = srcTrialSubject.ClinicalTrialSponsorName;
				clinicalTrialSubjectModule.ClinicalTrialSubjectId = srcTrialSubject.ClinicalTrialSubjectId;
				clinicalTrialSubjectModule.ClinicalTrialSubjectReadingId = srcTrialSubject.ClinicalTrialSubjectReadingId;
			}
		}

		private static void InitializeGeneralStudyModule(GeneralStudyModuleIod generalStudyModule, T prototypeImage)
		{
			GeneralStudyModuleIod srcGeneralStudy = new GeneralStudyModuleIod(prototypeImage.ImageSop.DataSource);
			generalStudyModule.AccessionNumber = srcGeneralStudy.AccessionNumber;
			generalStudyModule.NameOfPhysiciansReadingStudy = srcGeneralStudy.NameOfPhysiciansReadingStudy;
			generalStudyModule.PhysiciansOfRecord = srcGeneralStudy.PhysiciansOfRecord;
			generalStudyModule.PhysiciansOfRecordIdentificationSequence = srcGeneralStudy.PhysiciansOfRecordIdentificationSequence;
			generalStudyModule.PhysiciansReadingStudyIdentificationSequence = srcGeneralStudy.PhysiciansReadingStudyIdentificationSequence;
			generalStudyModule.ProcedureCodeSequence = srcGeneralStudy.ProcedureCodeSequence;
			generalStudyModule.ReferencedStudySequence = srcGeneralStudy.ReferencedStudySequence;
			generalStudyModule.ReferringPhysicianIdentificationSequence = srcGeneralStudy.ReferringPhysicianIdentificationSequence;
			generalStudyModule.ReferringPhysiciansName = srcGeneralStudy.ReferringPhysiciansName;
			generalStudyModule.StudyDateTime = srcGeneralStudy.StudyDateTime;
			generalStudyModule.StudyDescription = srcGeneralStudy.StudyDescription;
			generalStudyModule.StudyId = srcGeneralStudy.StudyId;
			generalStudyModule.StudyInstanceUid = srcGeneralStudy.StudyInstanceUid;
		}

		private static void InitializePatientStudyModule(PatientStudyModuleIod patientStudyModule, T prototypeImage)
		{
			PatientStudyModuleIod srcPatientStudy = new PatientStudyModuleIod(prototypeImage.ImageSop.DataSource);
			if (srcPatientStudy.HasValues()) // patient study module is user optional
			{
				patientStudyModule.AdditionalPatientHistory = srcPatientStudy.AdditionalPatientHistory;
				patientStudyModule.AdmissionId = srcPatientStudy.AdmissionId;
				patientStudyModule.AdmittingDiagnosesCodeSequence = srcPatientStudy.AdmittingDiagnosesCodeSequence;
				patientStudyModule.AdmittingDiagnosesDescription = srcPatientStudy.AdmittingDiagnosesDescription;
				patientStudyModule.IssuerOfAdmissionId = srcPatientStudy.IssuerOfAdmissionId;
				patientStudyModule.IssuerOfServiceEpisodeId = srcPatientStudy.IssuerOfServiceEpisodeId;
				patientStudyModule.Occupation = srcPatientStudy.Occupation;
				patientStudyModule.PatientsAge = srcPatientStudy.PatientsAge;
				patientStudyModule.PatientsSexNeutered = srcPatientStudy.PatientsSexNeutered;
				patientStudyModule.PatientsSize = srcPatientStudy.PatientsSize;
				patientStudyModule.PatientsWeight = srcPatientStudy.PatientsWeight;
				patientStudyModule.ServiceEpisodeDescription = srcPatientStudy.ServiceEpisodeDescription;
				patientStudyModule.ServiceEpisodeId = srcPatientStudy.ServiceEpisodeId;
			}
		}

		private static void InitializeClinicalTrialStudyModule(ClinicalTrialStudyModuleIod clinicalTrialStudyModule, T prototypeImage)
		{
			ClinicalTrialStudyModuleIod srcTrialStudy = new ClinicalTrialStudyModuleIod(prototypeImage.ImageSop.DataSource);
			if (srcTrialStudy.HasValues()) // clinical trial study module is user optional
			{
				clinicalTrialStudyModule.ClinicalTrialTimePointDescription = srcTrialStudy.ClinicalTrialTimePointDescription;
				clinicalTrialStudyModule.ClinicalTrialTimePointId = srcTrialStudy.ClinicalTrialTimePointId;
			}
		}

		#endregion

		#region Serialization of Presentation States

		private DisplayAreaSerializationOption _displayAreaSerializationOption = DisplayAreaSerializationOption.SerializeAsDisplayedArea;

		public DisplayAreaSerializationOption DisplayAreaSerializationOption
		{
			get { return _displayAreaSerializationOption; }
			set { _displayAreaSerializationOption = value; }
		}

		protected void SerializePresentationStateRelationship(PresentationStateRelationshipModuleIod presentationStateRelationshipModule, DicomPresentationImageCollection<T> images)
		{
			presentationStateRelationshipModule.InitializeAttributes();
			List<IReferencedSeriesSequence> seriesReferences = new List<IReferencedSeriesSequence>();
			foreach (string seriesUid in images.EnumerateSeries())
			{
				IReferencedSeriesSequence seriesReference = presentationStateRelationshipModule.CreateReferencedSeriesSequence();
				seriesReference.SeriesInstanceUid = seriesUid;
				List<ImageSopInstanceReferenceMacro> imageReferences = new List<ImageSopInstanceReferenceMacro>();
				foreach (T image in images.EnumerateImages(seriesUid))
				{
					imageReferences.Add(CreateImageSopInstanceReference(image.Frame));
				}
				seriesReference.ReferencedImageSequence = imageReferences.ToArray();
				seriesReferences.Add(seriesReference);
			}
			presentationStateRelationshipModule.ReferencedSeriesSequence = seriesReferences.ToArray();
		}

		protected void SerializePresentationStateShutter(PresentationStateShutterModuleIod presentationStateShutterModule)
		{
			presentationStateShutterModule.InitializeAttributes();
		}

		protected void SerializeDisplayedArea(DisplayedAreaModuleIod displayedAreaModule, DicomPresentationImageCollection<T> images)
		{
			displayedAreaModule.InitializeAttributes();
			List<DisplayedAreaModuleIod.DisplayedAreaSelectionSequenceItem> displayedAreas = new List<DisplayedAreaModuleIod.DisplayedAreaSelectionSequenceItem>();
			foreach (T image in images)
			{
				DisplayedAreaModuleIod.DisplayedAreaSelectionSequenceItem displayedArea = new DisplayedAreaModuleIod.DisplayedAreaSelectionSequenceItem();
				displayedArea.InitializeAttributes();
				displayedArea.ReferencedImageSequence = new ImageSopInstanceReferenceMacro[] {CreateImageSopInstanceReference(image.Frame)};

				if (image is IImageGraphicProvider)
				{
					ImageGraphic imageGraphic = ((IImageGraphicProvider) image).ImageGraphic;
					Size imageSize = new Size(imageGraphic.Columns, imageGraphic.Rows);

					// compute the visible boundaries of the image as a positive rectangle in screen space
					RectangleF visibleBounds = imageGraphic.SpatialTransform.ConvertToDestination(new Rectangle(new Point(0, 0), imageSize));
					visibleBounds = RectangleUtilities.Intersect(visibleBounds, image.ClientRectangle);
					visibleBounds = RectangleUtilities.ConvertToPositiveRectangle(visibleBounds);

					// compute the visible area of the image as a rectangle oriented positively in screen space
					RectangleF visibleImageArea = imageGraphic.SpatialTransform.ConvertToSource(visibleBounds);
					visibleImageArea = RectangleUtilities.RoundInflate(visibleImageArea);

					// compute the pixel addresses of the visible area by intersecting area with actual pixel addresses available
					//Rectangle visiblePixels = Rectangle.Truncate(RectangleUtilities.Intersect(visibleImageArea, new RectangleF(_point1, imageSize)));
					Rectangle visiblePixels = ConvertToPixelAddressRectangle(Rectangle.Truncate(visibleImageArea));
					displayedArea.DisplayedAreaTopLeftHandCorner = visiblePixels.Location;
					displayedArea.DisplayedAreaBottomRightHandCorner = visiblePixels.Location + visiblePixels.Size;
				}
				else
				{
					displayedArea.DisplayedAreaTopLeftHandCorner = image.ClientRectangle.Location + new Size(1, 1);
					displayedArea.DisplayedAreaBottomRightHandCorner = image.ClientRectangle.Location + image.ClientRectangle.Size;
				}

				ISpatialTransform spatialTransform = image.SpatialTransform;
				switch (_displayAreaSerializationOption)
				{
					case DisplayAreaSerializationOption.SerializeAsMagnification:
						displayedArea.PresentationSizeMode = DisplayedAreaModuleIod.PresentationSizeMode.Magnify;
						displayedArea.PresentationPixelMagnificationRatio = spatialTransform.Scale;
						break;
					case DisplayAreaSerializationOption.SerializeAsTrueSize:
						displayedArea.PresentationSizeMode = DisplayedAreaModuleIod.PresentationSizeMode.TrueSize;
						displayedArea.PresentationPixelSpacing = image.Frame.PixelSpacing;
						break;
					case DisplayAreaSerializationOption.SerializeAsDisplayedArea:
					default:
						displayedArea.PresentationSizeMode = DisplayedAreaModuleIod.PresentationSizeMode.ScaleToFit;
						break;
				}

				PixelAspectRatio pixelAspectRatio = image.Frame.PixelAspectRatio;
				if (pixelAspectRatio == null || pixelAspectRatio.IsNull)
					pixelAspectRatio = PixelAspectRatio.FromString(image.ImageSop[DicomTags.PixelAspectRatio].ToString());
				if (pixelAspectRatio == null || pixelAspectRatio.IsNull)
					pixelAspectRatio = new PixelAspectRatio(1, 1);
				displayedArea.PresentationPixelAspectRatio = pixelAspectRatio;

				displayedAreas.Add(displayedArea);
			}
			displayedAreaModule.DisplayedAreaSelectionSequence = displayedAreas.ToArray();
		}

		protected void SerializeSpatialTransform(SpatialTransformModuleIod spatialTransformModule, DicomPresentationImageCollection<T> images)
		{
			foreach (T image in images)
			{
				// spatial transform defines rotation in cartesian space - dicom module defines rotation as clockwise in image space
				// spatial transform defines both horizontal and vertical flip - dicom module defines horizontal flip only (vertical flip is 180 rotation plus horizontal flip)
				ISpatialTransform spatialTransform = image.SpatialTransform;
				int rotationBy90 = (((spatialTransform.RotationXY%360) + 360)%360)/90;
				int flipState = (spatialTransform.FlipX ? 2 : 0) + (spatialTransform.FlipY ? 1 : 0);
				spatialTransformModule.ImageRotation = _spatialTransformRotationTranslation[rotationBy90 + 4*flipState];
				spatialTransformModule.ImageHorizontalFlip = spatialTransform.FlipY ^ spatialTransform.FlipX ? ImageHorizontalFlip.Y : ImageHorizontalFlip.N;
				break;
			}
		}

		private static readonly int[] _spatialTransformRotationTranslation = new int[] {0, 90, 180, 270, 0, 270, 180, 90, 180, 90, 0, 270, 180, 270, 0, 90};

		private static readonly string _annotationsLayerId = "USER ANNOTATIONS";

		protected void SerializeGraphicLayer(GraphicLayerModuleIod graphicLayerModule, DicomPresentationImageCollection<T> images)
		{
			Dictionary<string, string> layerIndex = new Dictionary<string, string>();
			List<GraphicLayerSequenceItem> layerSequences = new List<GraphicLayerSequenceItem>();

			int order = 1;
			foreach (T image in images)
			{
				DicomGraphicsPlane psGraphic = DicomGraphicsPlane.GetDicomGraphicsPlane(image, false);
				if (psGraphic != null)
				{
					foreach (ILayer layerGraphic in (IEnumerable<ILayer>) psGraphic.Layers)
					{
						// do not serialize the inactive layer, and do not serialize layers more than once
						if (!string.IsNullOrEmpty(layerGraphic.Id) && !layerIndex.ContainsKey(layerGraphic.Id))
						{
							GraphicLayerSequenceItem layerSequence = new GraphicLayerSequenceItem();
							layerSequence.GraphicLayer = layerGraphic.Id.ToUpperInvariant();
							layerSequence.GraphicLayerDescription = layerGraphic.Description;
							layerSequence.GraphicLayerOrder = order++;
							layerSequence.GraphicLayerRecommendedDisplayCielabValue = null;
							layerSequence.GraphicLayerRecommendedDisplayGrayscaleValue = null;
							layerSequences.Add(layerSequence);
							layerIndex.Add(layerGraphic.Id, null);
						}
					}
				}

				if (image.OverlayGraphics.Count > 0)
				{
					if (!layerIndex.ContainsKey(_annotationsLayerId))
					{
						layerIndex.Add(_annotationsLayerId, null);
						GraphicLayerSequenceItem layerSequence = new GraphicLayerSequenceItem();
						layerSequence.GraphicLayer = _annotationsLayerId;
						layerSequence.GraphicLayerOrder = order++;
						layerSequences.Add(layerSequence);
						break;
					}
				}
			}

			if (layerSequences.Count > 0)
				graphicLayerModule.GraphicLayerSequence = layerSequences.ToArray();
		}

		protected void SerializeGraphicAnnotation(GraphicAnnotationModuleIod graphicAnnotationModule, DicomPresentationImageCollection<T> images)
		{
			List<GraphicAnnotationSequenceItem> annotations = new List<GraphicAnnotationSequenceItem>();

			foreach (T image in images)
			{
				DicomGraphicsPlane psGraphic = DicomGraphicsPlane.GetDicomGraphicsPlane(image, false);
				if (psGraphic != null)
				{
					foreach (ILayer layerGraphic in (IEnumerable<ILayer>) psGraphic.Layers)
					{
						foreach (IGraphic graphic in layerGraphic.Graphics)
						{
							GraphicAnnotationSequenceItem annotation = new GraphicAnnotationSequenceItem();
							if (GraphicAnnotationSerializer.SerializeGraphic(graphic, annotation))
							{
								SetAllSpecificCharacterSets(annotation, DataSet.SpecificCharacterSet);
								annotation.GraphicLayer = layerGraphic.Id.ToUpperInvariant();
								annotation.ReferencedImageSequence = new ImageSopInstanceReferenceMacro[] {CreateImageSopInstanceReference(image.Frame)};
								annotations.Add(annotation);
							}
						}
					}
				}

				foreach (IGraphic graphic in image.OverlayGraphics)
				{
					GraphicAnnotationSequenceItem annotation = new GraphicAnnotationSequenceItem();
					if (GraphicAnnotationSerializer.SerializeGraphic(graphic, annotation))
					{
						SetAllSpecificCharacterSets(annotation, DataSet.SpecificCharacterSet);
						annotation.GraphicLayer = _annotationsLayerId;
						annotation.ReferencedImageSequence = new ImageSopInstanceReferenceMacro[] {CreateImageSopInstanceReference(image.Frame)};
						annotations.Add(annotation);
					}
				}
			}

			if (annotations.Count > 0)
				graphicAnnotationModule.GraphicAnnotationSequence = annotations.ToArray();
		}

		private static void SetAllSpecificCharacterSets(GraphicAnnotationSequenceItem annotation, string specificCharacterSet)
		{
            if (annotation.TextObjectSequence == null)
                return;

			foreach (var textItem in annotation.TextObjectSequence)
			{
			    var attributeCollection = textItem.DicomAttributeProvider as DicomAttributeCollection;
                if (attributeCollection != null)
			        attributeCollection.SpecificCharacterSet = specificCharacterSet;
			}
		}

		protected void SerializeDisplayShutter(DisplayShutterModuleIod displayShutterModule, DicomPresentationImageCollection<T> images)
		{
			// Doesn't support multiframe or whatever case it is when we get more than one image serialized to one state
			CircularShutter circular = null;
			RectangularShutter rectangular = null;
			PolygonalShutter polygonal = null;
			int unserializedCount = 0;

			foreach (T image in images)
			{
				DicomGraphicsPlane dicomGraphics = DicomGraphicsPlane.GetDicomGraphicsPlane(image, false);
				if (dicomGraphics != null)
				{
					// identify visible geometric shutter if exists
					GeometricShuttersGraphic geometricShutters = dicomGraphics.Shutters.ActiveShutter as GeometricShuttersGraphic;
					if (geometricShutters != null)
					{
						// we can only save the first of each
						foreach (GeometricShutter shutter in geometricShutters.CustomShutters)
						{
							if (shutter is CircularShutter && circular == null)
								circular = (CircularShutter) shutter;
							else if (shutter is RectangularShutter && rectangular == null)
								rectangular = (RectangularShutter) shutter;
							else if (shutter is PolygonalShutter && polygonal == null)
								polygonal = (PolygonalShutter) shutter;
							else
								unserializedCount++;
						}
						foreach (GeometricShutter shutter in geometricShutters.DicomShutters)
						{
							if (shutter is CircularShutter && circular == null)
								circular = (CircularShutter) shutter;
							else if (shutter is RectangularShutter && rectangular == null)
								rectangular = (RectangularShutter) shutter;
							else if (shutter is PolygonalShutter && polygonal == null)
								polygonal = (PolygonalShutter) shutter;
							else
								unserializedCount++;
						}
					}
				}
			}

			ShutterShape shape = ShutterShape.None;
			if (circular != null)
			{
				shape |= ShutterShape.Circular;

				displayShutterModule.CenterOfCircularShutter = circular.Center;
				displayShutterModule.RadiusOfCircularShutter = circular.Radius;
			}
			if (rectangular != null)
			{
				shape |= ShutterShape.Rectangular;

				Rectangle r = rectangular.Rectangle;
				displayShutterModule.ShutterLeftVerticalEdge = r.Left;
				displayShutterModule.ShutterRightVerticalEdge = r.Right;
				displayShutterModule.ShutterUpperHorizontalEdge = r.Top;
				displayShutterModule.ShutterLowerHorizontalEdge = r.Bottom;
			}
			if (polygonal != null)
			{
				shape |= ShutterShape.Polygonal;

				List<Point> vertices = new List<Point>();
				vertices.AddRange(polygonal.Vertices);
				displayShutterModule.VerticesOfThePolygonalShutter = vertices.ToArray();
			}

			if (shape != ShutterShape.None)
			{
				displayShutterModule.ShutterShape = shape;
				displayShutterModule.ShutterPresentationValue = 0;
			}
			else
			{
				foreach (uint tag in DisplayShutterMacroIod.DefinedTags)
					displayShutterModule.DicomAttributeProvider[tag] = null;
			}

			if (unserializedCount > 0)
			{
				Platform.Log(LogLevel.Warn, "Attempt to serialize presentation state with an unsupported combination of shutters - some information may be lost.");
			}
		}

		protected void SerializeBitmapDisplayShutter(BitmapDisplayShutterModuleIod bitmapDisplayShutterModule, IOverlayMapping overlayMapping, DicomPresentationImageCollection<T> images)
		{
			// Doesn't support multiframe or whatever case it is when we get more than one image serialized to one state
			for (int n = 0; n < 16; n++)
			{
				OverlayPlaneGraphic overlay = overlayMapping[n];
				if (overlay != null)
				{
					if (overlay.ParentGraphic is IDicomGraphicsPlaneShutters)
					{
						bitmapDisplayShutterModule.ShutterShape = ShutterShape.Bitmap;
						bitmapDisplayShutterModule.ShutterOverlayGroupIndex = n;
						bitmapDisplayShutterModule.ShutterPresentationValue = overlay.GrayPresentationValue;
						bitmapDisplayShutterModule.ShutterPresentationColorCielabValue = null;
						break; // there can only be one
					}
				}
			}
		}

		protected void SerializeOverlayPlane(OverlayPlaneModuleIod overlayPlaneModule, out IOverlayMapping overlayMapping, DicomPresentationImageCollection<T> images)
		{
			// Doesn't support multiframe or whatever case it is when we get more than one image serialized to one state
			List<OverlayPlaneGraphic> visibleOverlays = new List<OverlayPlaneGraphic>();
			foreach (T image in images)
			{
				DicomGraphicsPlane dicomGraphics = DicomGraphicsPlane.GetDicomGraphicsPlane(image, false);
				if (dicomGraphics != null)
				{
					// identify visible bitmap shutter if exists
					OverlayPlaneGraphic bitmapShutter = dicomGraphics.Shutters.ActiveShutter as OverlayPlaneGraphic;
					if (bitmapShutter != null)
						visibleOverlays.Add(bitmapShutter);

					// identify any visible overlays
					foreach (ILayer layer in
						CollectionUtils.Select((IEnumerable<ILayer>) dicomGraphics.Layers, delegate(ILayer test) { return test.Visible; }))
					{
						foreach (OverlayPlaneGraphic overlay in
							CollectionUtils.Select(layer.Graphics, delegate(IGraphic test) { return test is OverlayPlaneGraphic && test.Visible; }))
							visibleOverlays.Add(overlay);
					}
				}
			}

			OverlayMapping overlayMap = new OverlayMapping();
			Queue<OverlayPlaneGraphic> overlaysToRemap = new Queue<OverlayPlaneGraphic>();

			// user and presentation state overlays are high priority items to remap
			foreach (OverlayPlaneGraphic overlay in CollectionUtils.Select(visibleOverlays, delegate(OverlayPlaneGraphic t) { return t.Source != OverlayPlaneSource.Image; }))
				overlaysToRemap.Enqueue(overlay);
			foreach (OverlayPlaneGraphic overlay in CollectionUtils.Select(visibleOverlays, delegate(OverlayPlaneGraphic t) { return t.Source == OverlayPlaneSource.Image; }))
			{
				if (overlayMap[overlay.Index] == null)
					overlayMap[overlay.Index] = overlay;
				else
					overlaysToRemap.Enqueue(overlay); // image overlays are lower priority items to remap, since they will be included in the header anyway
			}

			// seed the overlays to remap into the remaining available overlay groups
			for (int n = 0; n < 16 && overlaysToRemap.Count > 0; n++)
			{
				if (overlayMap[n] == null)
					overlayMap[n] = overlaysToRemap.Dequeue();
			}

			// serialize the overlays
			for (int n = 0; n < 16; n++)
			{
				OverlayPlaneGraphic overlay = overlayMap[n];
				if (overlay != null)
				{
					if (overlay.Source != OverlayPlaneSource.Image || overlay.Index != n)
					{
						// only record this overlay in the presentation state if it is being remapped to another group or is not already in the image.
						OverlayPlane overlayIod = overlayPlaneModule[n];
						overlayIod.OverlayData = overlay.CreateOverlayData(overlayIod.IsBigEndianOW).Raw;
						overlayIod.OverlayBitPosition = 0;
						overlayIod.OverlayBitsAllocated = 1;
						overlayIod.OverlayColumns = overlay.Columns;
						overlayIod.OverlayDescription = overlay.Description;
						overlayIod.OverlayLabel = overlay.Label;
						overlayIod.OverlayOrigin = Point.Round(overlay.Origin);
						overlayIod.OverlayRows = overlay.Rows;
						overlayIod.OverlaySubtype = overlay.Subtype;
						overlayIod.OverlayType = overlay.Type;
						overlayIod.RoiArea = null;
						overlayIod.RoiMean = null;
						overlayIod.RoiStandardDeviation = null;
					}
					else
					{
						overlayPlaneModule.Delete(n);
					}
				}
			}

			if (overlaysToRemap.Count > 0)
			{
				Platform.Log(LogLevel.Warn, "Attempt to serialize presentation state with more than 16 visible overlays - some information may be lost.");
			}

			overlayMapping = overlayMap;
		}

		protected void SerializeOverlayActivation(OverlayActivationModuleIod overlayActivationModule, IOverlayMapping overlayMapping, DicomPresentationImageCollection<T> images)
		{
			// Doesn't support multiframe or whatever case it is when we get more than one image serialized to one state
			for (int n = 0; n < 16; n++)
			{
				OverlayPlaneGraphic overlay = overlayMapping[n];
				if (overlay != null)
				{
					if (overlay.ParentGraphic is ILayer)
					{
						overlayActivationModule[n].OverlayActivationLayer = ((ILayer) overlay.ParentGraphic).Id;
					}
					else
					{
						overlayActivationModule.Delete(n);
					}
				}
			}
		}

		protected interface IOverlayMapping
		{
			OverlayPlaneGraphic this[int index] { get; }
		}

		private class OverlayMapping : IOverlayMapping
		{
			private readonly OverlayPlaneGraphic[] _map = new OverlayPlaneGraphic[16];

			public OverlayPlaneGraphic this[int index]
			{
				get { return _map[index]; }
				set { _map[index] = value; }
			}
		}

		/// <summary>
		/// Computes a rectangle specifying the 1-based pixel address and the row and column offset to get the pixel address of the opposite corner
		/// given a rectangle specifying the 0-based coordinate and size (whose width and height need not be positive).
		/// </summary>
		/// <param name="rectangle">The 0-based rectangle specified as a 0-based coordinate and a size.</param>
		/// <returns>An equivalent rectangle specifying the 1-based pixel address of the coordinate and row and column offset to get the pixel address of the opposite corner.</returns>
		/// <exception cref="ArgumentException">Thrown if the given rectangle has zero-area, as it cannot be represented in a 1-based pixel address rectangle.</exception>
		/// <remarks>
		/// <para>In DICOM and certain other applications, areas of images are identified with the first pixel being at position row 1 and column 1.
		/// In most Windows graphics rendering systems, imaging coordinates are identified as an infinitesimal point at the top-left corner of a given pixel
		/// and the coordinates are given in a 0-based system (that is, the first pixel has a coordinate of (0,0)).</para>
		/// <para>It is trivial to compute the addresses of the pixels included within the rectangle when the rectangle is positively-oriented
		/// (has positive width and height) since the left most pixels are in column <see cref="Rectangle.X"/>+1, the right most pixels are in
		/// column <see cref="Rectangle.X"/>+<see cref="Rectangle.Width"/>, and so on. It is somewhat more complicated to compute when the
		/// rectangle is not positively-oriented due to the singularities when either <see cref="Rectangle.Width"/> or <see cref="Rectangle.Height"/>
		/// is 0 (and hence rectangles having zero-area cannot be specified as a pixel address rectangle - a pixel address rectangle having size
		/// 0x0 is considered as containing 1 row and 1 column).</para>
		/// </remarks>
		private static Rectangle ConvertToPixelAddressRectangle(Rectangle rectangle)
		{
			if (rectangle.Width*rectangle.Height == 0)
				throw new ArgumentException("Zero-area rectangles cannot be specified in terms of a pixel address rectangle.", "rectangle");

			Size locationOffset = new Size(rectangle.Width > 0 ? 1 : 0, rectangle.Height > 0 ? 1 : 0);
			Size sizeOffset = new Size(rectangle.Width > 0 ? -1 : 1, rectangle.Height > 0 ? -1 : 1);

			return new Rectangle(rectangle.Location + locationOffset, rectangle.Size + sizeOffset);
		}

		#endregion

		#region Deserialization of Presentation States

		private bool _overlayPlanesDeserialized = false;

		private static PointF GetRectangleCenter(RectangleF rectangle)
		{
			return new PointF((rectangle.Left + rectangle.Right)/2f, (rectangle.Top + rectangle.Bottom)/2f);
		}

		protected void DeserializeDisplayedArea(DisplayedAreaModuleIod dispAreaMod, out RectangleF displayedArea, T image)
		{
			ISpatialTransform spatialTransform = image.SpatialTransform;
			foreach (DisplayedAreaModuleIod.DisplayedAreaSelectionSequenceItem item in dispAreaMod.DisplayedAreaSelectionSequence)
			{
				if (item.ReferencedImageSequence[0].ReferencedSopInstanceUid == image.ImageSop.SopInstanceUid)
				{
					// get the displayed area of the image in source coordinates (stored values do not have sub-pixel accuracy)
					var displayRect = RectangleF.FromLTRB(item.DisplayedAreaTopLeftHandCorner.X,
					                                      item.DisplayedAreaTopLeftHandCorner.Y,
					                                      item.DisplayedAreaBottomRightHandCorner.X,
					                                      item.DisplayedAreaBottomRightHandCorner.Y);
					displayRect = RectangleUtilities.ConvertToPositiveRectangle(displayRect);
					displayRect.Location = displayRect.Location - new SizeF(1, 1);
					displayRect.Size = displayRect.Size + new SizeF(1, 1);

					var centerDisplay = true;

					switch (item.PresentationSizeMode)
					{
						case DisplayedAreaModuleIod.PresentationSizeMode.Magnify:
							// displays selected area at specified magnification factor
							spatialTransform.Scale = (float) item.PresentationPixelMagnificationRatio.GetValueOrDefault(1);
							break;
						case DisplayedAreaModuleIod.PresentationSizeMode.TrueSize:
							// currently no support for determining true size, so default to scale area to fit
						case DisplayedAreaModuleIod.PresentationSizeMode.ScaleToFit:
						case DisplayedAreaModuleIod.PresentationSizeMode.None:
						default:
							if (spatialTransform is IImageSpatialTransform && displayRect.Location == new PointF(0, 0) && displayRect.Size == new SizeF(image.ImageGraphic.Columns, image.ImageGraphic.Rows))
							{
								// if the display rect is the whole image, then take advantage of the built-in scale image to fit functionality
								IImageSpatialTransform iist = (IImageSpatialTransform) spatialTransform;
								iist.ScaleToFit = true;
								centerDisplay = false; // when ScaleToFit is true, the image will automatically be positioned correctly in the client area
							}
							else
							{
								var clientArea = image.ClientRectangle.Size;
								var displaySize = displayRect.Size;

								// if image is rotated 90 or 270, transpose width/height
								if (Math.Abs(Math.Round(Math.Sin(spatialTransform.RotationXY*Math.PI/180))) > 0)
									displaySize = new SizeF(displaySize.Height, displaySize.Width);

								// compute the maximum magnification that allows the entire displayRect to be visible
								spatialTransform.Scale = Math.Min(clientArea.Width/displaySize.Width, clientArea.Height/displaySize.Height);
							}

							break;
					}

					if (centerDisplay && spatialTransform is SpatialTransform)
					{
						// compute translation so that the displayRect is centered in the clientRect
						var displayCentre = GetRectangleCenter(displayRect);
						var clientCentre = ((SpatialTransform) spatialTransform).ConvertToSource(GetRectangleCenter(image.ClientRectangle));
						spatialTransform.TranslationX = clientCentre.X - displayCentre.X;
						spatialTransform.TranslationY = clientCentre.Y - displayCentre.Y;
					}

					displayedArea = displayRect;
					return;
				}
			}
			displayedArea = new RectangleF(0, 0, image.ImageGraphic.Columns, image.ImageGraphic.Rows);
		}

		protected void DeserializeSpatialTransform(SpatialTransformModuleIod module, T image)
		{
			ISpatialTransform spatialTransform = image.SpatialTransform;
			if (spatialTransform is IImageSpatialTransform)
			{
				IImageSpatialTransform iist = (IImageSpatialTransform) spatialTransform;
				iist.ScaleToFit = false;
			}

			if (module.ImageHorizontalFlip == ImageHorizontalFlip.Y)
			{
				spatialTransform.FlipX = false;
				spatialTransform.FlipY = true;
				spatialTransform.RotationXY = (360 - module.ImageRotation)%360;
			}
			else
			{
				spatialTransform.FlipX = false;
				spatialTransform.FlipY = false;
				spatialTransform.RotationXY = module.ImageRotation;
			}
		}

		protected void DeserializeGraphicLayer(GraphicLayerModuleIod module, T image)
		{
			GraphicLayerSequenceItem[] layerSequences = module.GraphicLayerSequence;
			if (layerSequences == null)
				return;

			SortedDictionary<int, GraphicLayerSequenceItem> orderedSequences = new SortedDictionary<int, GraphicLayerSequenceItem>();
			foreach (GraphicLayerSequenceItem sequenceItem in layerSequences)
			{
				orderedSequences.Add(sequenceItem.GraphicLayerOrder, sequenceItem);
			}

			DicomGraphicsPlane graphic = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
			foreach (GraphicLayerSequenceItem sequenceItem in orderedSequences.Values)
			{
				ILayer layer = graphic.Layers[sequenceItem.GraphicLayer];
				layer.Description = sequenceItem.GraphicLayerDescription;
				// we don't support sequenceItem.GraphicLayerRecommendedDisplayCielabValue
				// we don't support sequenceItem.GraphicLayerRecommendedDisplayGrayscaleValue
			}
		}

		protected void DeserializeGraphicAnnotation(GraphicAnnotationModuleIod module, RectangleF displayedArea, T image)
		{
			DicomGraphicsPlane graphic = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
			foreach (DicomGraphicAnnotation annotation in DicomGraphicsFactory.CreateGraphicAnnotations(image.Frame, module, displayedArea))
			{
				graphic.Layers[annotation.LayerId].Graphics.Add(annotation);
			}
		}

		protected void DeserializeDisplayShutter(DisplayShutterModuleIod displayShutterModule, T image)
		{
			ShutterShape shape = displayShutterModule.ShutterShape;
			if (shape != ShutterShape.Bitmap && shape != ShutterShape.None)
			{
				IShutterGraphic shutter = DicomGraphicsFactory.CreateGeometricShuttersGraphic(displayShutterModule, image.Frame.Rows, image.Frame.Columns);
				// Some day, we will properly deserialize CIELab colours - until then, leave PresentationColor default black

				DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
				dicomGraphicsPlane.Shutters.Add(shutter);
				dicomGraphicsPlane.Shutters.Activate(shutter);
			}
		}

		/// <summary>
		/// Deserializes the specified bitmap display shutter module.
		/// </summary>
		/// <remarks>
		/// This method must be called after <see cref="DeserializeOverlayPlane">the overlay planes have been deserialized</see>.
		/// </remarks>
		/// <param name="bitmapDisplayShutterModule"></param>
		/// <param name="image"></param>
		protected void DeserializeBitmapDisplayShutter(BitmapDisplayShutterModuleIod bitmapDisplayShutterModule, T image)
		{
			if (!_overlayPlanesDeserialized)
				throw new InvalidOperationException("Overlay planes must be deserialized first.");

			if (bitmapDisplayShutterModule.ShutterShape == ShutterShape.Bitmap)
			{
				DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
				int overlayIndex = bitmapDisplayShutterModule.ShutterOverlayGroupIndex;
				if (overlayIndex >= 0 && overlayIndex < 16)
				{
					IShutterGraphic shutter = null;
					if (dicomGraphicsPlane.PresentationOverlays.Contains(overlayIndex))
					{
						shutter = dicomGraphicsPlane.PresentationOverlays[overlayIndex];
						dicomGraphicsPlane.PresentationOverlays.ActivateAsShutter(overlayIndex);
						dicomGraphicsPlane.ImageOverlays.Deactivate(overlayIndex);
					}
					else if (dicomGraphicsPlane.ImageOverlays.Contains(overlayIndex))
					{
						shutter = dicomGraphicsPlane.ImageOverlays[overlayIndex];
						dicomGraphicsPlane.ImageOverlays.ActivateAsShutter(overlayIndex);
					}

					// Some day, we will properly deserialize CIELab colours - until then, handle only a specified presentation value
					if (shutter != null)
					{
						shutter.PresentationColor = Color.Empty;
						shutter.PresentationValue = bitmapDisplayShutterModule.ShutterPresentationValue ?? 0;
					}
				}
			}
		}

		protected void DeserializeOverlayPlane(OverlayPlaneModuleIod overlayPlaneModule, T image)
		{
			DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
			foreach (OverlayPlaneGraphic overlay in DicomGraphicsFactory.CreateOverlayPlaneGraphics(image.Frame, overlayPlaneModule))
			{
				// the results are a mix of overlays from the image itself and the presentation state
				if (overlay.Source == OverlayPlaneSource.Image)
					dicomGraphicsPlane.ImageOverlays.Add(overlay);
				else
					dicomGraphicsPlane.PresentationOverlays.Add(overlay);

				// the above lines will automatically add the overlays to the inactive layer
			}
			_overlayPlanesDeserialized = true;
		}

		/// <summary>
		/// Deserializes the specified overlay activation module.
		/// </summary>
		/// <remarks>
		/// This method must be called after <see cref="DeserializeOverlayPlane">the overlay planes have been deserialized</see>.
		/// </remarks>
		/// <param name="overlayActivationModule"></param>
		/// <param name="image"></param>
		protected void DeserializeOverlayActivation(OverlayActivationModuleIod overlayActivationModule, T image)
		{
			if (!_overlayPlanesDeserialized)
				throw new InvalidOperationException("Overlay planes must be deserialized first.");

			DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, true);
			for (int n = 0; n < 16; n++)
			{
				if (overlayActivationModule.HasOverlayActivationLayer(n))
				{
					string targetLayer = overlayActivationModule[n].OverlayActivationLayer ?? string.Empty;
					if (dicomGraphicsPlane.PresentationOverlays.Contains(n))
					{
						if (string.IsNullOrEmpty(targetLayer))
							dicomGraphicsPlane.PresentationOverlays.Deactivate(n);
						else
							dicomGraphicsPlane.PresentationOverlays.ActivateAsLayer(n, targetLayer);
						dicomGraphicsPlane.ImageOverlays.Deactivate(n);
					}
					else if (dicomGraphicsPlane.ImageOverlays.Contains(n))
					{
						if (string.IsNullOrEmpty(targetLayer))
							dicomGraphicsPlane.ImageOverlays.Deactivate(n);
						else
							dicomGraphicsPlane.ImageOverlays.ActivateAsLayer(n, targetLayer);
					}
				}
				else
				{
					// if the module is missing entirely, then the presentation state is poorly encoded.
					// for patient safety reasons, we override the DICOM stipulation that only one of
					// these two should be shown and show both instead.
					dicomGraphicsPlane.PresentationOverlays.ActivateAsLayer(n, "OVERLAY");
					dicomGraphicsPlane.ImageOverlays.ActivateAsLayer(n, "OVERLAY");
				}
			}
		}

		#endregion
	}
}