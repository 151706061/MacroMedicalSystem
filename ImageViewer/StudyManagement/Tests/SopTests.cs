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

#if UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using NUnit.Framework;
using Macro.Dicom;
using Macro.Dicom.Tests;
using System.Collections.Generic;
using System;

namespace Macro.ImageViewer.StudyManagement.Tests
{
	public class TestDataSource : DicomMessageSopDataSource
	{
		public DicomFile _file;

		public TestDataSource(DicomFile file)
			: base(file)
		{
			_file = file;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			_file = null;
		}

		public bool IsDisposed { get { return _file == null; } }
	}

	[TestFixture]
	public class SopTests : AbstractTest
	{
		public SopTests()
		{
		}

        [TestFixtureSetUp]
        public void Initialize()
        {
            GC.Collect();
        }

	    [Test]
		public void TestDisposal()
		{
			IList<DicomAttributeCollection> collections = base.SetupMRSeries(1, 1, "test");
			DicomFile file = new DicomFile(null, new DicomAttributeCollection(), collections[0]);
			TestDataSource dataSource = new TestDataSource(file);
			Sop sop = Sop.Create(dataSource);

			Assert.IsFalse(dataSource.IsDisposed);
			Assert.IsFalse(SopDataCache.ItemCount == 0, "The Sop data cache is empty.");

			sop.Dispose();
			
			Assert.IsTrue(dataSource.IsDisposed);
			Assert.IsTrue(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
		}

		[Test]
		public void TestReferences1()
		{
			IList<DicomAttributeCollection> collections = base.SetupMRSeries(1, 1, "test");
			DicomFile file = new DicomFile(null, new DicomAttributeCollection(), collections[0]);
			TestDataSource dataSource = new TestDataSource(file);
			Sop sop = Sop.Create(dataSource);

			ISopReference reference1 = sop.CreateTransientReference();
			ISopReference reference2 = sop.CreateTransientReference();
			
			reference1.Dispose();
			Assert.IsFalse(dataSource.IsDisposed);
			Assert.IsFalse(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
			
			reference2.Dispose();
			Assert.IsFalse(dataSource.IsDisposed);
			Assert.IsFalse(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");

			sop.Dispose();
			Assert.IsTrue(dataSource.IsDisposed);
			Assert.IsTrue(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
		}

		[Test]
		public void TestReferences2()
		{
			IList<DicomAttributeCollection> collections = base.SetupMRSeries(1, 1, "test");
			DicomFile file = new DicomFile(null, new DicomAttributeCollection(), collections[0]);
			TestDataSource dataSource = new TestDataSource(file);
			Sop sop = Sop.Create(dataSource);

			ISopReference reference1 = sop.CreateTransientReference();
			ISopReference reference2 = sop.CreateTransientReference();

			sop.Dispose();
			Assert.IsFalse(dataSource.IsDisposed);
			Assert.IsFalse(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");

			reference1.Dispose();
			Assert.IsFalse(dataSource.IsDisposed);
			Assert.IsFalse(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");

			reference2.Dispose();
			Assert.IsTrue(dataSource.IsDisposed);
			Assert.IsTrue(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
		}

		[Test]
		public void TestCaching()
		{
			IList<DicomAttributeCollection> collections = base.SetupMRSeries(1, 1, "test");
			DicomFile file1 = new DicomFile(null, new DicomAttributeCollection(), collections[0].Copy());
			DicomFile file2 = new DicomFile(null, new DicomAttributeCollection(), collections[0].Copy());

			TestDataSource dataSource1 = new TestDataSource(file1);
			TestDataSource dataSource2 = new TestDataSource(file2);

			Sop sop1 = Sop.Create(dataSource1);
			Assert.IsTrue(SopDataCache.ItemCount == 1, "The Sop data cache should have 1 entry.");
			
			Sop sop2 = Sop.Create(dataSource2);
			Assert.IsTrue(SopDataCache.ItemCount == 1, "The Sop data cache should have 1 entry.");
			Assert.IsTrue(dataSource2.IsDisposed, "The data source has not been disposed.");
			Assert.IsFalse(dataSource1.IsDisposed, "The data source should not be disposed.");

			sop1.Dispose();
			Assert.IsTrue(SopDataCache.ItemCount == 1, "The Sop data cache should have 1 entry.");
			
			ISopReference reference21 = sop2.CreateTransientReference();
			ISopReference reference22 = sop2.CreateTransientReference();

			reference21.Dispose();
			Assert.IsFalse(dataSource1.IsDisposed, "The data source has been disposed.");
			Assert.IsTrue(SopDataCache.ItemCount == 1, "The Sop data cache should have 1 entry.");
			
			sop2.Dispose();
			Assert.IsFalse(dataSource1.IsDisposed, "The data source has been disposed.");
			Assert.IsTrue(SopDataCache.ItemCount == 1, "The Sop data cache should have 1 entry.");
			
			reference22.Dispose();
			Assert.IsTrue(dataSource1.IsDisposed, "The data source has not been disposed.");
			Assert.IsTrue(SopDataCache.ItemCount == 0, "The Sop data cache is NOT empty.");
		}

		[Test]
		public void TestGrayscalePixelNormalizationUnsigned8()
		{
			// test 8-bit identity
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				7, 8, 8, false, false);

			// test 8-bit, lower nibble
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0x0f, 0x0b, 0x0c, 0x09, 0x07, 0x08, 0x04, 0x0f, 0x05, 0x01},
				3, 4, 8, false, false);

			// test 8-bit, middle nibble
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0x0b, 0x0e, 0x07, 0x06, 0x09, 0x06, 0x0d, 0x0f, 0x09, 0x00},
				5, 4, 8, false, false);

			// test 8-bit, upper nibble
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0x0a, 0x0b, 0x09, 0x0d, 0x02, 0x01, 0x03, 0x0f, 0x06, 0x08},
				7, 4, 8, false, false);
		}

		[Test]
		public void TestGrayscalePixelNormalizationSigned8()
		{
			// test 8-bit identity
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				7, 8, 8, false, true);

			// test 8-bit, lower nibble
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xff, 0xfb, 0xfc, 0xf9, 0x07, 0xf8, 0x04, 0xff, 0x05, 0x01},
				3, 4, 8, false, true);

			// test 8-bit, middle nibble
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xfb, 0xfe, 0x07, 0x06, 0xf9, 0x06, 0xfd, 0xff, 0xf9, 0x00},
				5, 4, 8, false, true);

			// test 8-bit, upper nibble
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xfa, 0xfb, 0xf9, 0xfd, 0x02, 0x01, 0x03, 0xff, 0x06, 0xf8},
				7, 4, 8, false, true);
		}

		[Test]
		public void TestGrayscalePixelNormalizationUnsigned16()
		{
			// test 16-bit identity, big endian
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				15, 16, 16, true, false);

			// test 16-bit, upper byte in big endian stream
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0x00, 0xaf, 0x00, 0x9c, 0x00, 0x27, 0x00, 0x34, 0x00, 0x65},
				15, 8, 16, true, false);

			// test 16-bit, lower byte in big endian stream
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0x00, 0xbb, 0x00, 0xd9, 0x00, 0x18, 0x00, 0xff, 0x00, 0x81},
				7, 8, 16, true, false);

			// test 16-bit, middle byte in big endian stream
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0x00, 0xfb, 0x00, 0xcd, 0x00, 0x71, 0x00, 0x4f, 0x00, 0x58},
				11, 8, 16, true, false);

			// test 16-bit identity, little endian
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				15, 16, 16, false, false);

			// test 16-bit, upper byte in little endian stream
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xbb, 0x00, 0xd9, 0x00, 0x18, 0x00, 0xff, 0x00, 0x81, 0x00},
				15, 8, 16, false, false);

			// test 16-bit, lower byte in little endian stream
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0x00, 0x9c, 0x00, 0x27, 0x00, 0x34, 0x00, 0x65, 0x00},
				7, 8, 16, false, false);

			// test 16-bit, middle byte in little endian stream
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xba, 0x00, 0x99, 0x00, 0x82, 0x00, 0xf3, 0x00, 0x16, 0x00},
				11, 8, 16, false, false);
		}

		[Test]
		public void TestGrayscalePixelNormalizationSigned16()
		{
			// test 16-bit identity, big endian
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				15, 16, 16, true, true);

			// test 16-bit, upper byte in big endian stream
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xff, 0xaf, 0xff, 0x9c, 0x00, 0x27, 0x00, 0x34, 0x00, 0x65},
				15, 8, 16, true, true);

			// test 16-bit, lower byte in big endian stream
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xff, 0xbb, 0xff, 0xd9, 0x00, 0x18, 0xff, 0xff, 0xff, 0x81},
				7, 8, 16, true, true);

			// test 16-bit, middle byte in big endian stream
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xff, 0xfb, 0xff, 0xcd, 0x00, 0x71, 0x00, 0x4f, 0x00, 0x58},
				11, 8, 16, true, true);

			// test 16-bit identity, little endian
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				15, 16, 16, false, true);

			// test 16-bit, upper byte in little endian stream
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xbb, 0xff, 0xd9, 0xff, 0x18, 0x00, 0xff, 0xff, 0x81, 0xff},
				15, 8, 16, false, true);

			// test 16-bit, lower byte in little endian stream
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xaf, 0xff, 0x9c, 0xff, 0x27, 0x00, 0x34, 0x00, 0x65, 0x00},
				7, 8, 16, false, true);

			// test 16-bit, middle byte in little endian stream
			TryGrayscalePixelNormalization(
				new byte[] {0xaf, 0xbb, 0x9c, 0xd9, 0x27, 0x18, 0x34, 0xff, 0x65, 0x81},
				new byte[] {0xba, 0xff, 0x99, 0xff, 0x82, 0xff, 0xf3, 0xff, 0x16, 0x00},
				11, 8, 16, false, true);
		}

		private static void TryGrayscalePixelNormalization(byte[] input, byte[] expected, int highBit, int bitsStored, int bitsAllocated, bool bigEndian, bool signed)
		{
			DicomAttributeCollection coll = new DicomAttributeCollection();
			coll[DicomTags.BitsAllocated].SetInt32(0, bitsAllocated);
			coll[DicomTags.BitsStored].SetInt32(0, bitsStored);
			coll[DicomTags.HighBit].SetInt32(0, highBit);
			coll[DicomTags.PixelRepresentation].SetInt32(0, signed ? 1 : 0);

			byte[] output = new byte[input.Length];
			input.CopyTo(output, 0);
			DicomMessageSopDataSource.TestNormalizeGrayscalePixels(coll, output, bigEndian ? Endian.Big : Endian.Little);
			AssertArrayEquals(expected, output, string.Format("{0} Stored, {1} Allocated, High={2}, BigEndian={3}", bitsStored, bitsAllocated, highBit, bigEndian));
		}

		private static void AssertArrayEquals<T>(T[] expected, T[] actual, string message)
		{
			Assert.IsTrue(!(expected == null ^ actual == null), "Either both are null, or neither are null : {0}", message);
			if (expected == null)
				return;
			Assert.AreEqual(expected.Length, actual.Length, "Lengths differ : {0}", message);
			for (int n = 0; n < expected.Length; n++)
			{
				Assert.AreEqual(expected[n], actual[n], "Data differs at index {0} : {1}", n, message);
			}
		}
	}
}

#endif