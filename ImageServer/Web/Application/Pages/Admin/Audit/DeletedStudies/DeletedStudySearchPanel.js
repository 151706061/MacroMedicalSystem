/* License
 *
 * Copyright (c) 2011, ClearCanvas Inc.
 * All rights reserved.
 * http://www.ClearCanvas.ca
 *
 * This software is licensed under the Open Software License v3.0.
 * For the complete license, see http://www.ClearCanvas.ca/OSLv3.0
 *
 */ 

/////////////////////////////////////////////////////////////////////////////////////////////////////////
/// This script contains the javascript component class for the deleted study search panel

// Define and register the control type.
Type.registerNamespace('Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel = function(element) { 
    Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.prototype = 
{
    initialize : function() {
        Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.callBaseMethod(this, 'initialize');        
            
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        this._OnListControlRowClickedHandler = Function.createDelegate(this,this._OnListControlRowClicked);
        Sys.Application.add_load(this._OnLoadHandler);                 
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.callBaseMethod(this, 'dispose');
            
        Sys.Application.remove_load(this._OnLoadHandler);
    },
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
        
    /// called whenever the page is reloaded or partially reloaded
    _OnLoad : function()
    {
        // hook up the events... It is necessary to do this every time 
        // because NEW instances of the button and the study list components
        // may have been created as the result of the post-back
        var listCtrl = $find(this.get_ListClientID());
        listCtrl.add_onClientRowClick(this._OnListControlRowClickedHandler);
            
        this._updateToolbarButtonStates();
    },
        
    // called when user clicked on a row in the study list
    _OnListControlRowClicked : function(sender, event)
    {    
        this._updateToolbarButtonStates();        
    },
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _updateToolbarButtonStates : function()
    {
        var listCtrl = $find(this.get_ListClientID());
                      
        this._enableDeleteButton(false);
        this._enableDetailsButton(false);
            
        if (listCtrl!=null )
        {
            var rows = listCtrl.getSelectedRowElements();
                
            if (rows!=null && rows.length>0)
            {
		        this._enableDetailsButton(true);
                this._enableDeleteButton(true);                   
            }
                
        }
    },
        
    _enableDetailsButton : function(en)
    {
        var button = $find(this._ViewDetailsButtonClientID);
        if(button != null) button.set_enable(en);
    },
        
    _enableDeleteButton : function(en)
    {
        var button = $find(this._DeleteButtonClientID);
        if(button != null) button.set_enable(en);
    },
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
        
    get_ListClientID : function() {
        return this._ListClientID;
    },

    set_ListClientID : function(value) {
        this._ListClientID = value;
        this.raisePropertyChanged('ListClientID');
    },
        
    get_ViewDetailsButtonClientID : function() {
        return this._ViewDetailsButtonClientID;
    },

    set_ViewDetailsButtonClientID : function(value) {
        this._ViewDetailsButtonClientID = value;
        this.raisePropertyChanged('ViewDetailsButtonClientID');
    },
        
    get_DeleteButtonClientID : function() {
        return this._DeleteButtonClientID;
    },

    set_DeleteButtonClientID : function(value) {
        this._DeleteButtonClientID = value;
        this.raisePropertyChanged('DeleteButtonClientID');
    }
        
},

// Register the class as a type that inherits from Sys.UI.Control.
Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel.registerClass('Macro.ImageServer.Web.Application.Pages.Admin.Audit.DeletedStudies.DeletedStudySearchPanel', Sys.UI.Control);
     
if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

