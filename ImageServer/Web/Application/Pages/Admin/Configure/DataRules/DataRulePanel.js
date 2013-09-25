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
Type.registerNamespace('Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.DataRulePanel = function(element) {
Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.DataRulePanel.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.DataRulePanel.prototype =
{
    initialize: function() {

    Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.DataRulePanel.callBaseMethod(this, 'initialize');

        this._OnDataRuleListRowClickedHandler = Function.createDelegate(this, this._OnDataRuleListRowClicked);

        this._OnLoadHandler = Function.createDelegate(this, this._OnLoad);
        Sys.Application.add_load(this._OnLoadHandler);

    },

    dispose: function() {
        $clearHandlers(this.get_element());

        Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.DataRulePanel.callBaseMethod(this, 'dispose');

        Sys.Application.remove_load(this._OnLoadHandler);
    },

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
    _OnLoad: function() {
        var serverRulelist = $find(this._DataRuleListClientID);
        serverRulelist.add_onClientRowClick(this._OnDataRuleListRowClickedHandler);

        this._updateToolbarButtonStates();
    },

    // called when user clicked on a row in the study list
    _OnDataRuleListRowClicked: function(sender, event) {
        this._updateToolbarButtonStates();
    },

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _updateToolbarButtonStates: function() {
        var serverRulelist = $find(this._DataRuleListClientID);

        this._enableEditButton(false);
        this._enableDeleteButton(false);
        this._enableCopyButton(false);

        if (serverRulelist != null) {
            var rows = serverRulelist.getSelectedRowElements();

            if (rows != null && rows.length > 0) {
                this._enableEditButton(true);
                this._enableDeleteButton(true);
                this._enableCopyButton(true); 
            }
        }
    },

    _enableDeleteButton: function(en) {
        var deleteButton = $find(this._DeleteButtonClientID);
        deleteButton.set_enable(en);
    },

    _enableEditButton: function(en) {
        var editButton = $find(this._EditButtonClientID);
        editButton.set_enable(en);
    },

    _enableCopyButton: function(en) {
        var editButton = $find(this._CopyButtonClientID);
        editButton.set_enable(en);
    },

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
    get_DeleteButtonClientID: function() {
        return this._DeleteButtonClientID;
    },

    set_DeleteButtonClientID: function(value) {
        this._DeleteButtonClientID = value;
        this.raisePropertyChanged('DeleteButtonClientID');
    },

    get_EditButtonClientID: function() {
        return this._EditButtonClientID;
    },

    set_EditButtonClientID: function(value) {
        this._EditButtonClientID = value;
        this.raisePropertyChanged('EditButtonClientID');
    },

    get_CopyButtonClientID: function() {
        return this._CopyButtonClientID;
    },

    set_CopyButtonClientID: function(value) {
        this._CopyButtonClientID = value;
        this.raisePropertyChanged('CopyButtonClientID');
    },
        
    get_DataRuleListClientID: function() {
        return this._DataRuleListClientID;
    },

    set_DataRuleListClientID: function(value) {
    this._DataRuleListClientID = value;
        this.raisePropertyChanged('DataRuleListClientID');
    }
},

// Register the class as a type that inherits from Sys.UI.Control.
Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.DataRulePanel.registerClass('Macro.ImageServer.Web.Application.Pages.Admin.Configure.DataRules.DataRulePanel', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();