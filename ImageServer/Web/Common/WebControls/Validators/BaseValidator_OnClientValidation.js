// License
//
// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

function @@CLIENTID@@_OnClientSideValidation()
{
    validator = new @@CLIENTID@@_ClientSideEvaluator();
    result = validator.OnEvaluate();
    
    if (result.OK)
    {
        validator.OnValidationPassed();
        return true;
     }
    else
    {
        validator.OnValidationFailed(result);
        return false;
    }
    
}

