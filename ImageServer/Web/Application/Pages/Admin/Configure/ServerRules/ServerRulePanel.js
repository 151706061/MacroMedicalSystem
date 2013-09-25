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
/// This script contains the javascript component class for the study search panel

// Define and register the control type.
Type.registerNamespace('Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRulePanel = function(element) { 
    Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRulePanel.initializeBase(this, [element]);       
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRulePanel.prototype = 
{
    initialize : function() {
       
        Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRulePanel.callBaseMethod(this, 'initialize');        
            
        this._OnServerRuleListRowClickedHandler = Function.createDelegate(this,this._OnServerRuleListRowClicked);
            
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        Sys.Application.add_load(this._OnLoadHandler);
                 
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRulePanel.callBaseMethod(this, 'dispose');
            
        Sys.Application.remove_load(this._OnLoadHandler);
    },
        
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
    _OnLoad : function()
    {                    
        var serverRulelist = $find(this._ServerRuleListClientID);
        serverRulelist.add_onClientRowClick(this._OnServerRuleListRowClickedHandler);
                 
        this._updateToolbarButtonStates();
    },
        
    // called when user clicked on a row in the study list
    _OnServerRuleListRowClicked : function(sender, event)
    {    
        this._updateToolbarButtonStates();        
    },
                       
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _updateToolbarButtonStates : function()
    {
        var serverRulelist = $find(this._ServerRuleListClientID);
                                         
        this._enableEditButton(false);
        this._enableDeleteButton(false);
                                           
        if (serverRulelist!=null )
        {
            var rows = serverRulelist.getSelectedRowElements();

            if(rows != null && rows.length > 0) {
                this._enableEditButton(true);
                this._enableDeleteButton(true);
            }
        }
    },
        
    _enableDeleteButton : function(en)
    {
        var deleteButton = $find(this._DeleteButtonClientID);
        deleteButton.set_enable(en);
    },
        
    _enableEditButton : function(en)
    {
        var editButton = $find(this._EditButtonClientID);
        editButton.set_enable(en);
    },       

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
    get_DeleteButtonClientID : function() {
        return this._DeleteButtonClientID;
    },

    set_DeleteButtonClientID : function(value) {
        this._DeleteButtonClientID = value;
        this.raisePropertyChanged('DeleteButtonClientID');
    },
        
    get_EditButtonClientID : function() {
        return this._EditButtonClientID;
    },

    set_EditButtonClientID : function(value) {
        this._EditButtonClientID = value;
        this.raisePropertyChanged('EditButtonClientID');
    },
               
    get_ServerRuleListClientID : function() {
        return this._ServerRuleListClientID;
    },

    set_ServerRuleListClientID : function(value) {
        this._ServerRuleListClientID = value;
        this.raisePropertyChanged('ServerRuleListClientID');
    }
},

// Register the class as a type that inherits from Sys.UI.Control.
Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRulePanel.registerClass('Macro.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules.ServerRulePanel', Sys.UI.Control);     

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
