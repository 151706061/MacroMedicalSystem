// License
//
// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.ClearCanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.ClearCanvas.ca/OSLv3.0

// This script defines the client-side validator extension class @@CLIENTID@@_ClientSideEvaludator
// to validate whether or not a date is in the future.
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
            @@IGNORE_EMPTY_VALUE@@,
            '@@CONDITION_CHECKBOX_CLIENTID@@'=='null'? null:document.getElementById('@@CONDITION_CHECKBOX_CLIENTID@@'),
            @@VALIDATE_WHEN_UNCHECKED@@
    );
}

// override BaseClientValidator.OnEvaludate() 
// This function is called to evaluate the input
@@CLIENTID@@_ClientSideEvaluator.prototype.OnEvaluate = function()
{
        var validDateFormat = true;
        var dateInFuture = false;
        
        result = BaseClientValidator.prototype.OnEvaluate.call(this);
    
        if (result.OK==false)
        {
            return result;
        }
                                 
        if (this.input.value==null)
        {
            result.OK = false;
        } 
        else if(isDate(this.input.value, '@@DATE_FORMAT@@') == false)
        {
            result.OK = false;
            validDateFormat = false;
        } 
        else {       
            var today = new Date();
            
            if(today.getTime() - getDateFromFormat(this.input.value, '@@DATE_FORMAT@@') < 0) {
                result.OK = false;
                dateInFuture = true;
            }
        }
    
        if (result.OK == false)
        {        
            if ('@@ERROR_MESSAGE@@' == null || '@@ERROR_MESSAGE@@'=='')
            {
                if(!validDateFormat) result.Message = 'Provided date \'' + this.input.value + '\' is not in the format \'@@DATE_FORMAT@@\'.';
                else if (dateInFuture) result.Message = 'Provided date cannot be in the future.';
                else if (this.input.value == null) result.Message = 'Date cannot be empty.';
                else result.Message = 'Provided date is invalid.';
            }
            else
                result.Message = '@@ERROR_MESSAGE@@';
        }
        
        return  result;
};


