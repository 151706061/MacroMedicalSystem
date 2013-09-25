/* License
*
* Copyright (c) 2012, ClearCanvas Inc.
* All rights reserved.
* http://www.ClearCanvas.ca
*
* This software is licensed under the Open Software License v3.0.
* For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
*
*/

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Define and Register the control type.
//

// Register the namespace for the control.
Type.registerNamespace('Macro.ImageServer.Web.Common.WebControls.UI');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Define the control constructor
Macro.ImageServer.Web.Common.WebControls.UI.ToolbarButton = function(element) { 
    Macro.ImageServer.Web.Common.WebControls.UI.ToolbarButton.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
Macro.ImageServer.Web.Common.WebControls.UI.ToolbarButton.prototype = 
{
    initialize : function() {
        Macro.ImageServer.Web.Common.WebControls.UI.ToolbarButton.callBaseMethod(this, 'initialize');
            
            $addHandlers(this.get_element(), 
                        { 
                        'mouseover' : this._onMouseOver,
                        'mousedown' : this._onMouseDown,
//                            'mouseup' : this._onMouseUp,
                        'mouseout'  : this._onMouseOut,
                        'click'     : this._onClick
                        }, 
                        this);
                         
    },
       

    dispose : function() {
        $clearHandlers(this.get_element());

        Macro.ImageServer.Web.Common.WebControls.UI.ToolbarButton.callBaseMethod(this, 'dispose');
    },
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
    add_onClientClick : function(handler) {
        this.get_events().removeHandler('onClientClick', handler); // make sure we don't add it twice
        this.get_events().addHandler('onClientClick', handler);
            
    },
    remove_onClientClick  : function(handler) {
        this.get_events().removeHandler('onClientClick', handler);
    },
    raiseonClientClick  : function(eventArgs) {   
        var handler = this.get_events().getHandler('onClientClick');
        if (handler) {
            handler(this, eventArgs);
        }
    },
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // public methods
        set_enable : function(value) {
            
        if (value)
            this.get_element().removeAttribute('disabled');
        else
            this.get_element().setAttribute('disabled',  'disabled');
        this.get_element().setAttribute('src', value? this._EnabledImageUrl:this._DisabledImageUrl);
                
        this.raisePropertyChanged('enabled');
    },
        

    ////////////////////////////////////////////////////////////////////////////////////////////
    // Event delegates
    _onClick : function(e) 
    {
        if (this.get_element() && !this.get_element().disabled) {
            this.raiseonClientClick(e);
        }
    },

    _onMouseOver : function(e) 
    {
        if (this.get_element() && !this.get_element().disabled) {     
            if (this._HoverImageUrl!=undefined && this._HoverImageUrl!=null  && this._HoverImageUrl!='')
                this.get_element().src =  this._HoverImageUrl;
        }
    },

    _onMouseOut : function(e) 
    {
        if (this.get_element() && !this.get_element().disabled) {     
            this.get_element().src =  this._EnabledImageUrl;
        }
        else
        {
            this.get_element().src =  this._DisabledImageUrl;
        }
    },
        
    _onMouseDown : function(e) 
    {
/*            if (this.get_element() && !this.get_element().disabled) {     
            this.get_element().src =  this._ClickedImageUrl;
        }
        else if (this.get_element() && this.get_element().disabled)
        {
            this.get_element().src =  this._DisabledImageUrl;
        }
        else {
            this.get_element().src =  this._EnabledImageUrl;            
        }*/
    },
        
    _onMouseUp : function(e)
    {
        alert("MouseUp");
        this._onClick(e);
    },

    ////////////////////////////////////////////////////////////////////////////////////////////
    // properties

    get_EnabledImageUrl : function() {
        return this._EnabledImageUrl;
    },

    set_EnabledImageUrl : function(value) {
        this._EnabledImageUrl = value;
        this.raisePropertyChanged('EnabledImageUrl');
    },

    get_DisabledImageUrl : function() {
        return this._DisabledImageUrl;
    },

    set_DisabledImageUrl : function(value) {
        this._DisabledImageUrl = value;
        this.raisePropertyChanged('DisabledImageUrl');
    },
        
        
    get_HoverImageUrl : function() {
        return this._HoverImageUrl;
    },

    set_HoverImageUrl : function(value) {
        this._HoverImageUrl = value;
        this.raisePropertyChanged('HoverImageUrl');
    },
        
    get_ClickedImageUrl : function() {
        return this._ClickedImageUrl;
    },

    set_ClickedImageUrl : function(value) {
        this._ClickedImageUrl = value;
        this.raisePropertyChanged('ClickedImageUrl');
    },
        
    get_OnClientClick : function() {
        return this._OnClientClick;
    }
},

// Optional descriptor for JSON serialization.
//Macro.ImageServer.Web.Common.WebControls.UI.ToolbarButton.descriptor = {
//    properties: [   {name: 'onClientRowClick', type: events}                    ]
//}

// Register the class as a type that inherits from Sys.UI.Control.
Macro.ImageServer.Web.Common.WebControls.UI.ToolbarButton.registerClass('Macro.ImageServer.Web.Common.WebControls.UI.ToolbarButton', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
