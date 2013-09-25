#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using System.Collections.Generic;
using ClearCanvas.Web.Common;
using NUnit.Framework;


namespace ClearCanvas.Web.Services.Tests
{
	[TestFixture]
	public class ApplicationServicesUnitTests
	{
		public ApplicationServicesUnitTests()
		{}

		[Test]
		public void TestIncomingMessageQueue()
		{
			var processed = new List<MessageSet>();
			var testQueue = new IncomingMessageQueue(processed.Add);

			var testSets = new[]
			{
				new MessageSet {Number = 2},
				new MessageSet {Number = 1},
				new MessageSet {Number = 3},
				new MessageSet {Number = 4},
				new MessageSet {Number = 5},
				new MessageSet {Number = 7},
				new MessageSet {Number = 8},
				new MessageSet {Number = 6},
				new MessageSet {Number = 9},
				new MessageSet {Number = 10}
			};

			foreach (var messageSet in testSets)
				testQueue.ProcessMessageSet(messageSet);

			Assert.AreEqual(processed.Count, 10);

			int numberTest = 1;
			foreach (var eventSet in processed)
				Assert.AreEqual(eventSet.Number, numberTest++);

			Assert.AreEqual(testQueue.NextMessageSetNumber, 11);
		}
	}
}

#endif