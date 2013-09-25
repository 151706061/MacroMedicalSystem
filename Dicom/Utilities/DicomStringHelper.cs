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
using Macro.Common.Utilities;
using Macro.Dicom.Iod;

namespace Macro.Dicom.Utilities
{
	/// <summary>
	/// A static helper class containing methods for converting to and from multi-valued dicom arrays.  Any VR with VM > 1 is a string VR,
	/// but many must be convertible to numbers.  For example: IS (Integer String), DS (Decimal String), etc.
	/// </summary>
	/// <remarks>
	/// In the documentation for each method, the term <B>Dicom String Array</B> is used repeatedly.  This refers to a string representation of an 
	/// array as is used in Dicom.  For example, an array of integers (1, 2, 3) would be represented as "1\2\3" in Dicom.
	/// </remarks>
	public static class DicomStringHelper
	{
		/// <summary>
		/// Gets a Dicom String Array from the input values of an arbitrary type.
		/// </summary>
		/// <typeparam name="T">Any arbitrary Type that may be used to encode a Dicom String Array.</typeparam>
		/// <param name="values">The input values.</param>
		/// <returns>A Dicom String Array representation of <see cref="values"/>.</returns>
		/// <remarks>
		/// It is assumed that the <see cref="T.ToString"/> method returns the string that is to be encoded into the Dicom String Array.
		/// </remarks>
		static public string GetDicomStringArray<T>(IEnumerable<T> values)
		{
			// TODO CR (Nov 11): this will throw an exception if T were double or float and a value has enough decimal places to break the VR
			return StringUtilities.Combine(values, "\\");
		}

		/// <summary>
		/// Gets a Dicom String Array from the input values of an arbitrary type, formatted using <paramref name="formatSpecifier"/>.
		/// </summary>
		/// <remarks>
		/// <typeparamref name="T"/> must implement <see cref="IFormattable"/>.
		/// </remarks>
		/// <typeparam name="T">Any arbitrary Type that may be used to encode a Dicom String Array.</typeparam>
		/// <param name="values">The input values.</param>
		/// <param name="formatSpecifier">A format specifier appropriate for type <typeparamref name="T"/>.</param>
		/// <returns>A Dicom String Array representation of <see cref="values"/>.</returns>
		static public string GetDicomStringArray<T>(IEnumerable<T> values, string formatSpecifier)
			where T : IFormattable
		{
			return StringUtilities.Combine(values, "\\", formatSpecifier, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Splits a Dicom String Array (<see cref="dicomStringArray"/>) into its component <see cref="string"/>s.
		/// </summary>
		/// <param name="dicomStringArray">the Dicom String Array to be split up.</param>
		/// <returns>An array of <see cref="string"/>s.</returns>
		static public string[] GetStringArray(string dicomStringArray)
		{
		    return String.IsNullOrEmpty(dicomStringArray) ? new string[0] : dicomStringArray.Split(new [] { '\\' }, StringSplitOptions.None);
		}

	    /// <summary>
		/// Splits a Dicom String Array (<see cref="dicomStringArray"/>) into its component <see cref="PersonName"/>s.
		/// </summary>
		/// <param name="dicomStringArray">The Dicom String Array to be split up.</param>
		/// <returns>An array of <see cref="PersonName"/>s.</returns>
		static public PersonName[] GetPersonNameArray(string dicomStringArray)
		{
			string[] stringValues = GetStringArray(dicomStringArray);

			List<PersonName> personNames = new List<PersonName>();
			foreach (string value in stringValues)
				personNames.Add(new PersonName(value));

			return personNames.ToArray();
		}

		/// <summary>
		/// Splits a Dicom String Array (<see cref="dicomStringArray"/>) into its component <see cref="double"/>s without throwing an exception.
		/// </summary>
		/// <param name="dicomStringArray">The Dicom String Array to be split up.</param>
		/// <param name="returnValues">The return values.</param>
		/// <returns>True if all values were parsed successfully.  Otherwise, false.</returns>
		/// <remarks>The input string must consist of valid <see cref="double"/> values.  If not, all valid values up to, but not including, the first invalid value are returned.</remarks>
		static public bool TryGetDoubleArray(string dicomStringArray, out double[] returnValues)
		{
			string[] stringValues = GetStringArray(dicomStringArray);

			List<double> doubleValues = new List<double>();
			foreach (string value in stringValues)
			{
				double outValue;
				if (!double.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out outValue))
				{
					returnValues = doubleValues.ToArray();
					return false;
				}

				doubleValues.Add(outValue);
			}

			returnValues = doubleValues.ToArray();
			return true;
		}

		/// <summary>
		/// Splits a Dicom String Array (<see cref="dicomStringArray"/>) into its component <see cref="float"/>s without throwing an exception.
		/// </summary>
		/// <param name="dicomStringArray">The Dicom String Array to be split up.</param>
		/// <param name="returnValues">The return values.</param>
		/// <returns>True if all values were parsed successfully.  Otherwise, false.</returns>
		/// <remarks>The input string must consist of valid <see cref="float"/> values.  If not, all valid values up to, but not including, the first invalid value are returned.</remarks>
		static public bool TryGetFloatArray(string dicomStringArray, out float[] returnValues)
		{
			string[] stringValues = GetStringArray(dicomStringArray);

			List<float> floatValues = new List<float>();
			foreach (string value in stringValues)
			{
				float outValue;
				if (!float.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out outValue))
				{
					returnValues = floatValues.ToArray();
					return false;
				}

				floatValues.Add(outValue);
			}

			returnValues = floatValues.ToArray();
			return true;
		}

		/// <summary>
		/// Splits a Dicom String Array (<see cref="dicomStringArray"/>) into its component <see cref="int"/>s without throwing an exception.
		/// </summary>
		/// <param name="dicomStringArray">The Dicom String Array to be split up.</param>
		/// <param name="returnValues">The return values.</param>
		/// <returns>True if all values were parsed successfully.  Otherwise, false.</returns>
		/// <remarks>The input string must consist of valid <see cref="int"/> values.  If not, all valid values up to, but not including, the first invalid value are returned.</remarks>
		static public bool TryGetIntArray(string dicomStringArray, out int[] returnValues)
		{
			string[] stringValues = GetStringArray(dicomStringArray);

			List<int> intValues = new List<int>();
			foreach (string value in stringValues)
			{
				int outValue;
				if (!int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out outValue))
				{
					returnValues = intValues.ToArray();
					return false;
				}

				intValues.Add(outValue);
			}

			returnValues = intValues.ToArray();
			return true;
		}
	}
}