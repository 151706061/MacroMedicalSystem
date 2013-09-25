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

using Macro.Dicom;
using Macro.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace Macro.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    class PixelDataLoader
    {
        private readonly ImageStreamingContext _context;

        internal PixelDataLoader(ImageStreamingContext context)
        {
            _context = context;
        }

        public byte[] ReadFrame(int frame)
        {
            DicomPixelData pd = _context.PixelData;
            if (pd is DicomCompressedPixelData)
            {
                byte[] buffer = (pd as DicomCompressedPixelData).GetFrameFragmentData(frame);
                return buffer;
            }
            else
            {
                return pd.GetFrame(frame);
            }

        }

        public byte[] ReadUncompressedFrame(int frame)
        {
            return _context.PixelData.GetFrame(frame);
        }

    }
}