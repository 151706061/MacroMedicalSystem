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
/// This script contains the javascript component class for the move panel
Type.registerNamespace('Macro.ImageServer.Web.Application.Pages.Studies.Move');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
Macro.ImageServer.Web.Application.Pages.Studies.Move.MovePanel = function(element) { 
    Macro.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
Macro.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.prototype =
{
    initialize: function() {

        Macro.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.callBaseMethod(this, 'initialize');

        this._OnDeviceListRowClickedHandler = Function.createDelegate(this, this._OnDeviceListRowClicked);
        this._OnLoadHandler = Function.createDelegate(this, this._OnLoad);
            
        Sys.Application.add_load(this._OnLoadHandler);

    },

    dispose: function() {
        $clearHandlers(this.get_element());

        Macro.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.callBaseMethod(this, 'dispose');

        Sys.Application.remove_load(this._OnLoadHandler);
    },


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
   
    /// called whenever the page is reloaded or partially reloaded
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

        this._enableMoveButton(false);

        if (devicelist != null) {
            var rows = devicelist.getSelectedRowElements();

            if (rows != null && rows.length > 0) {
                this._enableMoveButton(true);
            } else {
                this._enableMoveButton(false);
            }
        }
    },

    _enableMoveButton: function(en) {
        var moveButton = $find(this._MoveButtonClientID);
        moveButton.set_enable(en);
    },

    get_MoveButtonClientID: function() {
        return this._MoveButtonClientID;
    },

    set_MoveButtonClientID: function(value) {
        this._MoveButtonClientID = value;
        this.raisePropertyChanged('MoveButtonClientID');
    },

    set_DeviceListClientID: function(value) {
        this._DeviceListClientID = value;
        this.raisePropertyChanged('DeviceListClientID');
    },

    get_DeviceListClientID: function() {
        return this._DeviceListClientID;
    }
},

// Register the class as a type that inherits from Sys.UI.Control.
Macro.ImageServer.Web.Application.Pages.Studies.Move.MovePanel.registerClass('Macro.ImageServer.Web.Application.Pages.Studies.Move.MovePanel', Sys.UI.Control);     

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
