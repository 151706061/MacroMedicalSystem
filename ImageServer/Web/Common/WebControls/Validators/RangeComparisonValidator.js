// License
//
// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0


// This script defines the client-side validator extension class @@CLIENTID@@_ClientSideEvaludator
// to validate an input within a specified range and is greater or less than another input.
//
// This class defines how the validation is carried and what to do afterwards.


// derive this class from BaseClientValidator
ClassHelper.extend(@@CLIENTID@@_ClientSideEvaluator, BaseClientValidator);

//constructor
function @@CLIENTID@@_ClientSideEvaluator()
{
    BaseClientValidator.call(this, 
            '@@INPUT_CLIENTID@@',
            '@@INPUT_NORMAL_BKCOLOR@@',
            '@@INPUT_INVALID_BKCOLOR@@',
            '@@INPUT_NORMAL_BORDERCOLOR@@',
            '@@INPUT_INVALID_BORDERCOLOR@@', 
            '@@INPUT_NORMAL_CSS@@',
            '@@INPUT_INVALID_CSS@@',                                               
            '@@INVALID_INPUT_INDICATOR_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_CLIENTID@@'),
            '@@INVALID_INPUT_INDICATOR_TOOLTIP_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_TOOLTIP_CLIENTID@@'),
            '@@INVALID_INPUT_INDICATOR_TOOLTIP_CONTAINER_CLIENTID@@'=='null'? null:document.getElementById('@@INVALID_INPUT_INDICATOR_TOOLTIP_CONTAINER_CLIENTID@@'),
            '@@IGNORE_EMPTY_VALUE@@',
            '@@CONDITION_CHECKBOX_CLIENTID@@'=='null'? null:document.getElementById('@@CONDITION_CHECKBOX_CLIENTID@@'),
            @@VALIDATE_WHEN_UNCHECKED@@
    );
}


// override BaseClientValidator.OnEvaludate() 
// This function is called to evaluate the input
@@CLIENTID@@_ClientSideEvaluator.prototype.OnEvaluate = function()
{
    result = BaseClientValidator.prototype.OnEvaluate.call(this);
    
    if (result.OK==false)
    {
        return result;
    }
        
    compareCtrl = document.getElementById('@@COMPARE_INPUT_CLIENTID@@');
   
    result = new ValidationResult();
    if (this.input.value!=null && this.input.value!='')
    {
        controlValue = parseNumber(this.input.value);
        if (compareCtrl!=null && compareCtrl.value!='' &&  !isNaN(controlValue))
        {
            compareValue = parseNumber(compareCtrl.value);
            result.OK = controlValue >= @@MIN_VALUE@@ && controlValue<= @@MAX_VALUE@@ && controlValue @@COMPARISON_OP@@ compareValue;
            
            if (result.OK == false)
            {
                result.Message = '@@ERROR_MESSAGE@@';
                
            }
        }
        else
        {
            result.OK = false;
            result.Message = '@@ERROR_MESSAGE@@';
        }
    }   
    else
    {
          result.OK = false;  
    }
    
    
    
    return result;

};
