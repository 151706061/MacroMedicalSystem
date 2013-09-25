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

using System.Diagnostics;
using System.IO;
using Macro.Dicom.Iod.Modules;
using Macro.ImageViewer.Tests;
using NUnit.Framework;

namespace Macro.ImageViewer.PresentationStates.Dicom.Tests
{
	[TestFixture]
	public class PresentationStateTests
	{
		[Test]
		public void TestIodRoundtripSpatialTransform()
		{
			TestPresentationState ps = new TestPresentationState();

			for (int rotation = 0; rotation < 360; rotation += 90)
			{
				for (int flipH = 0; flipH < 2; flipH++)
				{
					Trace.WriteLine(string.Format("Testing Roundtrip IOD->IMG->IOD with params Rot={0}, fH={1}", rotation, flipH));

					SpatialTransformModuleIod original = new SpatialTransformModuleIod();
					original.ImageHorizontalFlip = (flipH == 1) ? ImageHorizontalFlip.Y : ImageHorizontalFlip.N;
					original.ImageRotation = rotation;

					TestPresentationImage image = ps.DeserializeSpatialTransform(original);
					using (image)
					{
						SpatialTransformModuleIod actual = ps.SerializeSpatialTransform(image);
					Assert.AreEqual(original.ImageHorizontalFlip, actual.ImageHorizontalFlip, string.Format("Roundtrip IOD->IMG->IOD FAILED: Rot={0}, fH={1}", rotation, flipH));
					Assert.AreEqual(original.ImageRotation, actual.ImageRotation, string.Format("Roundtrip IOD->IMG->IOD FAILED: Rot={0}, fH={1}", rotation, flipH));
					}
				}
			}
		}

		[Test]
		public void TestImageRoundtripSpatialTransform()
		{
			TestPresentationState ps = new TestPresentationState();

			foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.test.bmp"))
				File.Delete(file);

			for (int rotation = 0; rotation < 360; rotation += 90)
			{
				for (int flipX = 0; flipX < 2; flipX ++)
				{
					for (int flipY = 0; flipY < 2; flipY++)
					{
						Trace.WriteLine(string.Format("Testing Roundtrip IMG->IOD->IMG with params Rot={0}, fX={1}, fY={2}", rotation, flipX, flipY));

						TestPresentationImage original = new TestPresentationImage();
						original.SpatialTransform.FlipX = (flipX%2 == 1);
						original.SpatialTransform.FlipY = (flipY%2 == 1);
						original.SpatialTransform.RotationXY = rotation;
						original.SaveBitmap(string.Format("{0:d2}-{1}-{2}-original.test.bmp", rotation / 10, flipX, flipY));

						TestPresentationImage actual = ps.DeserializeSpatialTransform(ps.SerializeSpatialTransform(original));
						actual.SaveBitmap(string.Format("{0:d2}-{1}-{2}-actual.test.bmp", rotation / 10, flipX, flipY));

						Statistics stats = original.Diff(actual);
						Trace.WriteLine(string.Format("DIFF STATS {0}", stats));
						Assert.IsTrue(stats.IsEqualTo(0, 5), string.Format("Roundtrip IMG->IOD->IMG FAILED: Rot={0}, fX={1}, fY={2}", rotation, flipX, flipY));

						actual.Dispose();
						original.Dispose();
					}
				}
			}
		}
	}
}

#endif