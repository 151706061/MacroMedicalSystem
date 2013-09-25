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
using System.Linq;
using Macro.Dicom;
using Macro.Dicom.Iod;
using Macro.Dicom.Utilities;

namespace Macro.ImageViewer.Imaging
{
	/// <summary>
	/// Represents a window centre/width pair, with accompanying descriptive explanation.
	/// </summary>
	public class VoiWindow : IEquatable<VoiWindow>
	{
		private readonly double _width;
		private readonly double _center;
		private readonly string _explanation;

		/// <summary>
		/// Constructs a new instance of a <see cref="VoiWindow"/> with no explanation.
		/// </summary>
		/// <param name="width">The window width.</param>
		/// <param name="center">The window centre.</param>
		public VoiWindow(double width, double center) : this(width, center, string.Empty) {}

		/// <summary>
		/// Constructs a new instance of a <see cref="VoiWindow"/>.
		/// </summary>
		/// <param name="width">The window width.</param>
		/// <param name="center">The window centre.</param>
		/// <param name="explanation">A descriptive explanation for the window.</param>
		public VoiWindow(double width, double center, string explanation)
		{
			_width = width;
			_center = center;
			_explanation = explanation ?? string.Empty;
		}

		/// <summary>
		/// Gets the window width.
		/// </summary>
		public double Width
		{
			get { return _width; }
		}

		/// <summary>
		/// Gets the window centre.
		/// </summary>
		public double Center
		{
			get { return _center; }
		}

		/// <summary>
		/// Gets a descriptive explanation for the window.
		/// </summary>
		public string Explanation
		{
			get { return _explanation; }
		}

		/// <summary>
		/// Determines whether or not the current window has the same width and centre as another window.
		/// </summary>
		/// <param name="other">The other window.</param>
		/// <returns>True if the window width and centre are the same.</returns>
		public bool Equals(VoiWindow other)
		{
			return (_width == other._width && _center == other._center);
		}

		/// <summary>
		/// Determines whether or not the current window has the same width and centre as another window.
		/// </summary>
		/// <param name="obj">The other window.</param>
		/// <returns>True if the window width and centre are the same.</returns>
		public override bool Equals(object obj)
		{
			if (obj is VoiWindow)
				return this.Equals((VoiWindow) obj);
			return false;
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>The hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return _width.GetHashCode() ^ _center.GetHashCode() ^ -0x4B056F4A;
		}

		/// <summary>
		/// Formats the window width and centre as a string.
		/// </summary>
		public override string ToString()
		{
			return String.Format(@"{0:F2}/{1:F2}", _width, _center);
		}

		/// <summary>
		/// Converts the specified window into a <see cref="Window">DICOM window IOD</see>.
		/// </summary>
		public static explicit operator Window(VoiWindow window)
		{
			return new Window(window._width, window._center);
		}

		/// <summary>
		/// Converts a window from the specified <see cref="Window">DICOM window IOD</see>.
		/// </summary>
		public static explicit operator VoiWindow(Window window)
		{
			return new VoiWindow(window.Width, window.Center);
		}

		/// <summary>
		/// Gets an enumeration of <see cref="VoiWindow"/>s.
		/// </summary>
		/// <param name="centers">An enumeration of window centres.</param>
		/// <param name="widths">An enumeration of window widths.</param>
		/// <returns>An enumeation of <see cref="VoiWindow"/>s.</returns>
		public static IEnumerable<VoiWindow> GetWindows(IEnumerable<double> centers, IEnumerable<double> widths)
		{
			return GetWindows(centers, widths, null);
		}

		/// <summary>
		/// Gets an enumeration of <see cref="VoiWindow"/>s.
		/// </summary>
		/// <param name="centers">An enumeration of window centres.</param>
		/// <param name="widths">An enumeration of window widths.</param>
		/// <param name="explanations">An enumeration of window explanations.</param>
		/// <returns>An enumeation of <see cref="VoiWindow"/>s.</returns>
		public static IEnumerable<VoiWindow> GetWindows(IEnumerable<double> centers, IEnumerable<double> widths, IEnumerable<string> explanations)
		{
			if (centers == null || widths == null)
				yield break;

			double[] windowCenters = new List<double>(centers).ToArray();
			double[] windowWidths = new List<double>(widths).ToArray();

			List<string> windowExplanations = new List<string>();
			if (explanations != null)
				windowExplanations.AddRange(explanations);

			foreach (VoiWindow window in GetWindows(windowCenters, windowWidths, windowExplanations.ToArray()))
				yield return window;
		}

		/// <summary>
		/// Gets an enumeration of <see cref="VoiWindow"/>s defined in the specified data source.
		/// </summary>
		/// <param name="dataset">A DICOM data source.</param>
		/// <returns>An enumeration of <see cref="VoiWindow"/>s.</returns>
		public static IEnumerable<VoiWindow> GetWindows(IDicomAttributeProvider dataset)
		{
			string windowCenterValues = dataset[DicomTags.WindowCenter].ToString();

			if (string.IsNullOrEmpty(windowCenterValues))
				yield break;

			string windowWidthValues = dataset[DicomTags.WindowWidth].ToString();

			if (string.IsNullOrEmpty(windowWidthValues))
				yield break;

			string windowExplanationValues = dataset[DicomTags.WindowCenterWidthExplanation].ToString();

			double[] windowCenters;
			DicomStringHelper.TryGetDoubleArray(windowCenterValues, out windowCenters);

			double[] windowWidths;
			DicomStringHelper.TryGetDoubleArray(windowWidthValues, out windowWidths);

			string[] windowExplanations = DicomStringHelper.GetStringArray(windowExplanationValues);

			foreach (VoiWindow window in GetWindows(windowCenters, windowWidths, windowExplanations))
				yield return window;
		}

		private static IEnumerable<VoiWindow> GetWindows(double[] windowCenters, double[] windowWidths, string[] windowExplanations)
		{
			if (windowCenters.Length == windowWidths.Length)
			{
				for (int i = 0; i < windowWidths.Length; ++i)
				{
					if (i < windowExplanations.Length)
						yield return new VoiWindow(windowWidths[i], windowCenters[i], windowExplanations[i]);
					else
						yield return new VoiWindow(windowWidths[i], windowCenters[i]);
				}
			}
		}

		/// <summary>
		/// Sets the VOI window attributes in the data source with the specified windows.
		/// </summary>
		/// <param name="windows">The list of <see cref="VoiWindow"/>s to be set.</param>
		/// <param name="dataset">A DICOM data source.</param>
		public static void SetWindows(IEnumerable<VoiWindow> windows, IDicomAttributeProvider dataset)
		{
			var windowCenters = DicomStringHelper.GetDicomStringArray(windows.Select(s => s.Center));
			var windowWidths = DicomStringHelper.GetDicomStringArray(windows.Select(s => s.Width));
			var windowExplanations = DicomStringHelper.GetDicomStringArray(windows.Select(s => s.Explanation));

			dataset[DicomTags.WindowCenter].SetStringValue(windowCenters);
			dataset[DicomTags.WindowWidth].SetStringValue(windowWidths);
			dataset[DicomTags.WindowCenterWidthExplanation].SetStringValue(windowExplanations);
		}

		/// <summary>
		/// Creates a <see cref="VoiWindow"/> from the specified value range.
		/// </summary>
		/// <param name="minimumValue">The minimum value in the range.</param>
		/// <param name="maximumValue">The maximum value in the range.</param>
		/// <param name="explanation">A description explanation for the window.</param>
		/// <returns>A <see cref="VoiWindow"/> defining the specified value range.</returns>
		public static VoiWindow FromWindowRange(double minimumValue, double maximumValue, string explanation)
		{
			return new VoiWindow(Math.Abs(maximumValue - minimumValue) + 1, (maximumValue + minimumValue + 1)/2, explanation);
		}
	}
}