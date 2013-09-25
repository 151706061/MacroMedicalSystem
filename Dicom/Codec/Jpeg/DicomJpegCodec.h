#pragma region License

// Copyright (c) 2012, Macro Inc.
// All rights reserved.
// http://www.Macro.ca
//
// This file is part of the Macro RIS/PACS open source project.
//
// The Macro RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU Lesser Public
// License as published by the Free Software Foundation, either version 3 of
// the License, or (at your option) any later version.
//
// The Macro RIS/PACS open source project is distributed in the hope that
// it will be useful, but WITHOUT ANY WARRANTY; without even the implied
// warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser Public License for more details.
//
// You should have received a copy of the GNU Lesser Public License along with
// the Macro RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#pragma endregion

#pragma region Inline Attributions
// The source code contained in this file is based on an original work
// from
//
// mDCM: A C# DICOM library
//
// Copyright (c) 2008  Colby Dillion
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Author:
//    Colby Dillion (colby.dillion@gmail.com)
#pragma endregion


#ifndef __DICOMJPEGCODEC_H__
#define __DICOMJPEGCODEC_H__

#pragma once

using namespace System;
using namespace System::IO;

using namespace Macro::Dicom;
using namespace Macro::Dicom::Codec;

#include "JpegCodec.h"
#include "DicomJpegParameters.h"

namespace Macro {
namespace Dicom {
namespace Codec {
namespace Jpeg {

public ref class DicomJpegCodec abstract : public IDicomCodec {
public:
	virtual property String^ Name { String^ get(); };
	virtual property Macro::Dicom::TransferSyntax^ CodecTransferSyntax { Macro::Dicom::TransferSyntax^ get(); };

	virtual void Encode(DicomUncompressedPixelData^ oldPixelData, DicomCompressedPixelData^ newPixelData, DicomCodecParameters^ parameters);
	virtual void Decode(DicomCompressedPixelData^ oldPixelData, DicomUncompressedPixelData^ newPixelData, DicomCodecParameters^ parameters);
	virtual void DecodeFrame(int frame, DicomCompressedPixelData^ oldPixelData, DicomUncompressedPixelData^ newPixelData, DicomCodecParameters^ parameters);

	virtual IJpegCodec^ GetCodec(int bits, DicomJpegParameters^ jparams) = 0;
	unsigned char DicomJpegCodec::GetJpegBitDepth(const unsigned char *data, const unsigned int fragmentLength);
	unsigned short DicomJpegCodec::readUint16(const unsigned char *data);

};


public ref class DicomJpegProcess1Codec : public DicomJpegCodec {
public:
    property String^ Name { virtual String^ get() override;}
    property Macro::Dicom::TransferSyntax^ CodecTransferSyntax { virtual Macro::Dicom::TransferSyntax^ get() override; };


	virtual IJpegCodec^ GetCodec(int bits, DicomJpegParameters^ jparams) override {
		if (bits <= 8)
			return gcnew Jpeg8Codec(JpegMode::Baseline, 0, 0);
		else
			throw gcnew DicomCodecUnsupportedSopException(String::Format("Unable to create JPEG Baseline codec for bits stored == {0}", bits));
	}
};

public ref class DicomJpegProcess24Codec : public DicomJpegCodec {
public:
    property String^ Name { virtual String^ get() override;}
    property Macro::Dicom::TransferSyntax^ CodecTransferSyntax { virtual Macro::Dicom::TransferSyntax^ get() override; };

	virtual IJpegCodec^ GetCodec(int bits, DicomJpegParameters^ jparams) override {
		// JPEG Extended only supports 12 bit or less images.
		if (bits <= 8)
			return gcnew Jpeg8Codec(JpegMode::Sequential, 0, 0);
		else if (bits == 12)
			return gcnew Jpeg12Codec(JpegMode::Sequential, 0, 0);
		else
			throw gcnew DicomCodecUnsupportedSopException(String::Format("Unable to create JPEG Extended codec for bits stored == {0}", bits));
	}
};

public ref class DicomJpegLossless14Codec : public DicomJpegCodec {
public:
    property String^ Name { virtual String^ get() override;}
    property Macro::Dicom::TransferSyntax^ CodecTransferSyntax { virtual Macro::Dicom::TransferSyntax^ get() override; };

	virtual IJpegCodec^ GetCodec(int bits, DicomJpegParameters^ jparams) override {
		if (bits <= 8)
			return gcnew Jpeg8Codec(JpegMode::Lossless, jparams->Predictor, jparams->PointTransform);
		else if (bits <= 12)
			return gcnew Jpeg12Codec(JpegMode::Lossless, jparams->Predictor, jparams->PointTransform);
		else if (bits <= 16)
			return gcnew Jpeg16Codec(JpegMode::Lossless, jparams->Predictor, jparams->PointTransform);
		else
			throw gcnew DicomCodecUnsupportedSopException(String::Format("Unable to create JPEG codec for bits stored == {0}", bits));
	}
};

public ref class DicomJpegLossless14SV1Codec : public DicomJpegCodec {
public:
    property String^ Name { virtual String^ get() override;}
    property Macro::Dicom::TransferSyntax^ CodecTransferSyntax { virtual Macro::Dicom::TransferSyntax^ get() override; };

	virtual IJpegCodec^ GetCodec(int bits, DicomJpegParameters^ jparams) override {
		if (bits <= 8)
			return gcnew Jpeg8Codec(JpegMode::Lossless, 1, jparams->PointTransform);
		else if (bits <= 12)
			return gcnew Jpeg12Codec(JpegMode::Lossless, 1, jparams->PointTransform);
		if (bits <= 16)
			return gcnew Jpeg16Codec(JpegMode::Lossless, 1, jparams->PointTransform);
		else
			throw gcnew DicomCodecUnsupportedSopException(String::Format("Unable to create JPEG codec for bits stored == {0}", bits));
	}
};

} // Jpeg
} // Codec
} // Dicom
} // Macro

#endif