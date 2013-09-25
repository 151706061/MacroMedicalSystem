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

#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using Macro.Dicom;
using Macro.ImageViewer.Graphics;
using Macro.ImageViewer.RoiGraphics;
using Macro.ImageViewer.RoiGraphics.Analyzers;
using Macro.ImageViewer.RoiGraphics.Tests;
using Macro.ImageViewer.StudyManagement;
using NUnit.Framework;
using System.Drawing.Imaging;

namespace Macro.ImageViewer.Tools.Measurement.Tests
{
	[TestFixture]
	public class ProtractorRoiTests : RoiTestBase<ProtractorRoiTests.Angle>
	{
		public static bool DumpMeasuredImages = true;

		[Test]
		public void TestStandardAngles()
		{
			RunAngleTest(ImageKey.Aspect01, 60, "standard-60",
			             new Angle(new PointF(125, 52), new PointF(25, 225), new PointF(225, 225)),
						 new Angle(new PointF(225, 225), new PointF(25, 225), new PointF(125, 52)),
						 new Angle(new PointF(84, 125), new PointF(25, 225), new PointF(225, 225)));
			RunAngleTest(ImageKey.Aspect01, 120, "standard-120",
						 new Angle(new PointF(125, 52), new PointF(125, 167.33f), new PointF(225, 225)),
						 new Angle(new PointF(225, 225), new PointF(125, 167.33f), new PointF(25, 225)),
						 new Angle(new PointF(125, 52), new PointF(125, 167.33f), new PointF(25, 225)));
			RunAngleTest(ImageKey.Aspect01, 30, "standard-30",
						 new Angle(new PointF(125, 167.33f), new PointF(125, 52), new PointF(225, 225)),
						 new Angle(new PointF(125, 167.33f), new PointF(225, 225), new PointF(25, 225)),
						 new Angle(new PointF(125, 167.33f), new PointF(125, 52), new PointF(25, 225)));
			RunAngleTest(ImageKey.Simple01, 90, "standard-90",
						 new Angle(new PointF(77, 79), new PointF(77, 179), new PointF(177, 179)),
						 new Angle(new PointF(77, 130), new PointF(77, 179), new PointF(130, 179)),
						 new Angle(new PointF(77, 179), new PointF(177, 179), new PointF(177, 79)),
						 new Angle(new PointF(177, 79), new PointF(177, 179), new PointF(77, 179)));
			RunAngleTest(ImageKey.Simple01, 45, "standard-45",
						 new Angle(new PointF(177, 79), new PointF(77, 179), new PointF(177, 179)),
						 new Angle(new PointF(177, 79), new PointF(77, 179), new PointF(77, 79)));
			RunAngleTest(ImageKey.Complex10, 32.9f, "standard-33",
						 new Angle(new PointF(125, 50), new PointF(50, 225), new PointF(125, 175)),
						 new Angle(new PointF(125, 175), new PointF(200, 225), new PointF(125, 50)));
			RunAngleTest(ImageKey.Complex10, 46.7f, "standard-47",
						 new Angle(new PointF(50, 225), new PointF(125, 50), new PointF(200, 225)),
						 new Angle(new PointF(200, 225), new PointF(125, 50), new PointF(50, 225)));
			RunAngleTest(ImageKey.Complex10, 112.6f, "standard-113",
						 new Angle(new PointF(50, 225), new PointF(125, 175), new PointF(200, 225)),
						 new Angle(new PointF(200, 225), new PointF(125, 175), new PointF(50, 225)));
		}

		[Test(Description = "Control test for measuring angles on images with isotropic pixels")]
		public void TestAnglesIsotropicPixels()
		{
			PointF[] corners = new PointF[] {new PointF(125, 52), new PointF(25, 225), new PointF(225, 225)};
			foreach (ImageKey image in new ImageKey[] {ImageKey.Aspect01, ImageKey.Aspect02, ImageKey.Aspect03, ImageKey.Aspect04})
			{
				RunAngleTest(image, 60, "iso",
							 new Angle(corners[0], corners[1], corners[2]),
							 new Angle(corners[1], corners[2], corners[0]),
							 new Angle(corners[2], corners[0], corners[1]));
			}
		}

		[Test(Description = "Tests angle measurement on images with 4:3 anisotropic pixels")]
		public void TestAngles4To3AnisotropicPixels()
		{
			PointF[] corners = new PointF[] {new PointF(166, 52), new PointF(33, 225), new PointF(300, 225)};
			foreach (ImageKey image in new ImageKey[] {ImageKey.Aspect06, ImageKey.Aspect07, ImageKey.Aspect08})
			{
				RunAngleTest(image, 60, "aniso43",
							 new Angle(corners[0], corners[1], corners[2]),
							 new Angle(corners[1], corners[2], corners[0]),
							 new Angle(corners[2], corners[0], corners[1]));
			}
		}

		[Test(Description = "Tests angle measurement on images with 3:4 anisotropic pixels")]
		public void TestAngles3To4AnisotropicPixels()
		{
			PointF[] corners = new PointF[] {new PointF(125, 69), new PointF(25, 300), new PointF(225, 300)};
			foreach (ImageKey image in new ImageKey[] {ImageKey.Aspect10, ImageKey.Aspect11, ImageKey.Aspect12})
			{
				RunAngleTest(image, 60, "aniso34",
				             new Angle(corners[0], corners[1], corners[2]),
				             new Angle(corners[1], corners[2], corners[0]),
				             new Angle(corners[2], corners[0], corners[1]));
			}
		}

		protected void RunAngleTest(ImageKey key, float expectedAngle, string dumpFilename, params Angle[] shapes)
		{
			ProtractorAngleCalculator analyzer = new ProtractorAngleCalculator();
			Trace.WriteLine(string.Format("Using {0} for test case {1}", key, dumpFilename), "UNIT_TESTS");
			using (IPresentationImage image = GetImage(key))
			{
				IImageSopProvider sopProvider = (IImageSopProvider) image;
				DicomAttribute attribute;
				if (sopProvider.Sop.DataSource.TryGetAttribute(DicomTags.PixelAspectRatio, out attribute))
					Trace.WriteLine("PixelAspectRatio (0028,0034) PRESENT", "UNIT_TESTS");
				if (!sopProvider.Frame.PixelSpacing.IsNull)
					Trace.WriteLine("PixelSpacing (0028,0030) or equivalent PRESENT", "UNIT_TESTS");

				try
				{
					foreach (Angle shapeData in shapes)
					{
						Roi roi = CreateRoiFromGraphic((IOverlayGraphicsProvider) image, shapeData);
						var actualResult = analyzer.Analyze(roi, RoiAnalysisMode.Normal);

						float actualAngle = float.Parse(RegexAngleMeasurement.Match(actualResult.SerializedAsString()).Groups[1].Value, CultureInfo.InvariantCulture);
						Trace.WriteLine(String.Format("Actual: {0} degrees, Expected: {1} degrees", actualAngle, expectedAngle));
						Assert.AreEqual(expectedAngle, actualAngle, 0.5f, "Testing Angle {0}", shapeData); // allow half degree tolerance
					}
				}
				finally
				{
					if (DumpMeasuredImages && !string.IsNullOrEmpty(dumpFilename))
					{
						using (Bitmap bmp = image.DrawToBitmap(256, 256))
						{
							bmp.Save(string.Format("{1}-{0}.bmp", key, dumpFilename), ImageFormat.Bmp);
						}
					}
				}
			}
		}

		protected override string ShapeName
		{
			get { return "angle"; }
		}

		protected override Roi CreateRoiFromGraphic(IOverlayGraphicsProvider overlayGraphics, Angle shapeData)
		{
			ProtractorGraphic graphic = new ProtractorGraphic();
			overlayGraphics.OverlayGraphics.Add(graphic);
			graphic.CoordinateSystem = CoordinateSystem.Source;
			graphic.Points.Add(shapeData.Value1);
			graphic.Points.Add(shapeData.Value2);
			graphic.Points.Add(shapeData.Value3);
			graphic.ResetCoordinateSystem();
			return graphic.GetRoi();
		}

		protected override Roi CreateRoiFromImage(IPresentationImage image, Angle shapeData)
		{
			return new ProtractorRoiInfo(shapeData.Value1, shapeData.Value2, shapeData.Value3, image);
		}

		protected override Angle CreateCoreSampleShape(PointF location, int imageRows, int imageCols)
		{
			throw new IgnoreException("This class of ROI tests does not apply to the protractor angle shape.");
		}

		protected override void AddShapeToGraphicsPath(GraphicsPath graphicsPath, Angle shapeData)
		{
			throw new IgnoreException("This class of ROI tests does not apply to the protractor angle shape.");
		}

		/// <summary>
		/// Gets a test image for use in <see cref="CalibrationTest"/>.
		/// </summary>
		/// <param name="pixelShape">Available pixel shapes are ISO, 4:3 and 3:4</param>
		/// <param name="uncalibrated">Whether or not the image should specify Pixel Spacing already.</param>
		internal static IPresentationImage GetCalibrationTestImage(string pixelShape, bool uncalibrated)
		{
			ImageKey imageKey;
			switch (pixelShape.ToLowerInvariant())
			{
				case "iso":
					imageKey = uncalibrated ? ImageKey.Aspect01 : ImageKey.Aspect03;
					break;
				case "4:3":
					imageKey = uncalibrated ? ImageKey.Aspect06 : ImageKey.Aspect07;
					break;
				case "3:4":
					imageKey = uncalibrated ? ImageKey.Aspect10 : ImageKey.Aspect11;
					break;
				default:
					throw new ArgumentException("Unsupported pixel shape");
			}
			return GetImage(imageKey);
		}

		protected static readonly Regex RegexAngleMeasurement;

		static ProtractorRoiTests()
		{
			// this converts arg0 of the angle degrees measurement format string into a general floating point number regex
			string parsePattern = Regex.Replace(
				SR.ToolsMeasurementFormatDegrees,
				@"(.*?)([{]0(?:[:].+)?[}])(.*)",
				m => Regex.Escape(m.Groups[1].Value) + @"([+-]?\d+(?:[.]\d*)?(?:[Ee][+-]?\d+)?)" + Regex.Escape(m.Groups[3].Value));
			RegexAngleMeasurement = new Regex(parsePattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);
		}

		public struct Angle
		{
			public readonly PointF Value1;
			public readonly PointF Value2;
			public readonly PointF Value3;

			public Angle(float x1, float y1, float x2, float y2, float x3, float y3)
				: this(new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y3)) {}

			public Angle(PointF value1, PointF value2, PointF value3)
			{
				this.Value1 = value1;
				this.Value2 = value2;
				this.Value3 = value3;
			}

			public override string ToString()
			{
				return string.Format("{0} {1} {2}", Value1, Value2, Value3);
			}
		}
	}
}

#endif