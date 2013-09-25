// License
//
// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

function @@CLIENTID@@_ShowOtherNameFormats()
{
    var row=$get('@@PHONETIC_ROW_CLIENTID@@');
    row.style.visibility='visible';
    if (/MSIE (\d+\.\d+)/.test(navigator.userAgent))
    { 
        row.style.display='block';
    }
    else 
    { 
        row.style.display='table-row';
    }

    row=$get('@@IDEOGRAPHY_ROW_CLIENTID@@');
    row.style.visibility='visible';
    if (/MSIE (\d+\.\d+)/.test(navigator.userAgent))
    { 
        row.style.display='block';
    }
    else 
    { 
        row.style.display='table-row';
    }
}
