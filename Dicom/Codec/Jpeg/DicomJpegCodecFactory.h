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

#pragma once

using namespace System;
using namespace System::IO;
using namespace System::Xml;

using namespace Macro::Dicom;
using namespace Macro::Dicom::Codec;

#include "DicomJpegCodec.h"
#include "DicomJpegParameters.h"

namespace Macro {
namespace Dicom {
namespace Codec {
namespace Jpeg {

[Macro::Common::ExtensionOf(DicomCodecFactoryExtensionPoint::typeid)]
public ref class DicomJpegProcess1CodecFactory : public IDicomCodecFactory {
public:
    virtual property String^ Name { String^ get() ;}
    virtual property bool Enabled { bool get(); }
    virtual property Macro::Dicom::TransferSyntax^ CodecTransferSyntax {  Macro::Dicom::TransferSyntax^ get(); };

    virtual DicomCodecParameters^ GetCodecParameters(DicomAttributeCollection^ dataSet);
	virtual DicomCodecParameters^ GetCodecParameters(XmlDocument^ parms);
    virtual IDicomCodec^ GetDicomCodec();
};

[Macro::Common::ExtensionOf(DicomCodecFactoryExtensionPoint::typeid)]
public ref class DicomJpegProcess24CodecFactory : public IDicomCodecFactory {
public:
    virtual property String^ Name { String^ get();}
    virtual property bool Enabled { bool get(); }
    virtual property Macro::Dicom::TransferSyntax^ CodecTransferSyntax { Macro::Dicom::TransferSyntax^ get(); };

    virtual DicomCodecParameters^ GetCodecParameters(DicomAttributeCollection^ dataSet);
	virtual DicomCodecParameters^ GetCodecParameters(XmlDocument^ parms);
	virtual IDicomCodec^ GetDicomCodec();
};

[Macro::Common::ExtensionOf(DicomCodecFactoryExtensionPoint::typeid)]
public ref class DicomJpegLossless14CodecFactory : public IDicomCodecFactory {
public:
    virtual property String^ Name { String^ get();}
    virtual property bool Enabled { bool get(); }
    virtual property Macro::Dicom::TransferSyntax^ CodecTransferSyntax { Macro::Dicom::TransferSyntax^ get(); };

    virtual DicomCodecParameters^ GetCodecParameters(DicomAttributeCollection^ dataSet);
    virtual DicomCodecParameters^ GetCodecParameters(XmlDocument^ parms);
	virtual IDicomCodec^ GetDicomCodec();
};

[Macro::Common::ExtensionOf(DicomCodecFactoryExtensionPoint::typeid)]
public ref class DicomJpegLossless14SV1CodecFactory : public IDicomCodecFactory {
public:
    virtual property String^ Name { String^ get();}
    virtual property bool Enabled { bool get(); }
    virtual property Macro::Dicom::TransferSyntax^ CodecTransferSyntax { Macro::Dicom::TransferSyntax^ get(); };

    virtual DicomCodecParameters^ GetCodecParameters(DicomAttributeCollection^ dataSet);
    virtual DicomCodecParameters^ GetCodecParameters(XmlDocument^ parms);
	virtual IDicomCodec^ GetDicomCodec();
};

} // Jpeg
} // Codec
} // Dicom
} // Macro
