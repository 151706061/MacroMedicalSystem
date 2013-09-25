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

Type.registerNamespace('Macro.ImageServer.Web.Application.Pages.Admin.Alerts');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
Macro.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel = function(element) {
    Macro.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
Macro.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.prototype =
    {
        initialize: function() {
            Macro.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.callBaseMethod(this, 'initialize');

            this._OnAlertListRowClickedHandler = Function.createDelegate(this, this._OnAlertListRowClicked);

            this._OnLoadHandler = Function.createDelegate(this, this._OnLoad);
            Sys.Application.add_load(this._OnLoadHandler);
        },

        dispose: function() {
            $clearHandlers(this.get_element());

            Macro.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.callBaseMethod(this, 'dispose');

            Sys.Application.remove_load(this._OnLoadHandler);
        },

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Events
        _OnLoad: function() {
            var userlist = $find(this._AlertListClientID);
            userlist.add_onClientRowClick(this._OnAlertListRowClickedHandler);

            this._updateToolbarButtonStates();
        },

        // called when user clicked on a row in the study list
        _OnAlertListRowClicked: function(sender, event) {
            this._updateToolbarButtonStates();
        },

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Private Methods
        _updateToolbarButtonStates: function() {
            var alertlist = $find(this._AlertListClientID);

            this._enableDeleteButton(false);
            this._enableDeleteAllButton(false);

            if (alertlist != null) {
                var rows = alertlist.getSelectedRowElements();

                if (rows != null && rows.length > 0) {
                    this._enableDeleteButton(true);
                }

                if (alertlist.getNumberOfRows() > 0) {
                    this._enableDeleteAllButton(true);
                }
            }
        },

        _enableDeleteButton: function(en) {
            var deleteButton = $find(this._DeleteButtonClientID);
            if (deleteButton != null) deleteButton.set_enable(en);
        },

        _enableDeleteAllButton: function(en) {
            var deleteAllButton = $find(this._DeleteAllButtonClientID);
            if (deleteAllButton != null) deleteAllButton.set_enable(en);
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

        get_DeleteAllButtonClientID: function() {
            return this._DeleteAllButtonClientID;
        },

        set_DeleteAllButtonClientID: function(value) {
            this._DeleteAllButtonClientID = value;
            this.raisePropertyChanged('DeleteAllButtonClientID');
        },

        get_AlertListClientID: function() {
            return this._AlertListClientID;
        },

        set_AlertListClientID: function(value) {
            this._AlertListClientID = value;
            this.raisePropertyChanged('AlertListClientID');
        }
    },

// Register the class as a type that inherits from Sys.UI.Control.
Macro.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel.registerClass('Macro.ImageServer.Web.Application.Pages.Admin.Alerts.AlertsPanel', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

