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
Type.registerNamespace('Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel = function(element) {
    Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel.prototype =
{
    initialize: function() {

        Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel.callBaseMethod(this, 'initialize');

        this._OnDeviceListRowClickedHandler = Function.createDelegate(this, this._OnDeviceListRowClicked);

        this._OnLoadHandler = Function.createDelegate(this, this._OnLoad);
        Sys.Application.add_load(this._OnLoadHandler);

    },

    dispose: function() {
        $clearHandlers(this.get_element());

        Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel.callBaseMethod(this, 'dispose');

        Sys.Application.remove_load(this._OnLoadHandler);
    },

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
    _OnLoad: function() {
        var devicelist = $find(this._DeviceListClientID);
        devicelist.add_onClientRowClick(this._OnDeviceListRowClickedHandler);

        this._updateToolbarButtonStates();
    },

    // called when user clicked on a row in the study list
    _OnDeviceListRowClicked: function(sender, event) {
        this._updateToolbarButtonStates();
    },

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _updateToolbarButtonStates: function() {
        var devicelist = $find(this._DeviceListClientID);

        this._enableEditButton(false);
        this._enableDeleteButton(false);

        if (devicelist != null) {
            var rows = devicelist.getSelectedRowElements();

            if (rows != null && rows.length > 0) {
                this._enableEditButton(true);
                this._enableDeleteButton(true);
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

    get_DeviceListClientID: function() {
        return this._DeviceListClientID;
    },

    set_DeviceListClientID: function(value) {
        this._DeviceListClientID = value;
        this.raisePropertyChanged('DeviceListClientID');
    }
},

// Register the class as a type that inherits from Sys.UI.Control.
Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel.registerClass('Macro.ImageServer.Web.Application.Pages.Admin.Configure.Devices.DevicePanel', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
