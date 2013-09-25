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
Type.registerNamespace('Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel = function(element) { 
    Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel.initializeBase(this, [element]);
       
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel.prototype = 
{
    initialize : function() {
        Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel.callBaseMethod(this, 'initialize');        
            
        this._OnUserListRowClickedHandler = Function.createDelegate(this,this._OnUserListRowClicked);
            
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        Sys.Application.add_load(this._OnLoadHandler);
                 
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel.callBaseMethod(this, 'dispose');
            
        Sys.Application.remove_load(this._OnLoadHandler);
    },
        
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
    _OnLoad : function()
    {                    
        var userlist = $find(this._UserListClientID);
        userlist.add_onClientRowClick(this._OnUserListRowClickedHandler);
                 
        this._updateToolbarButtonStates();
    },
        
    // called when user clicked on a row in the study list
    _OnUserListRowClicked : function(sender, event)
    {    
        this._updateToolbarButtonStates();        
    },
                       
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _updateToolbarButtonStates : function()
    {
        var userlist = $find(this._UserListClientID);
                      
        this._enableEditButton(false);
        this._enableDeleteButton(false);
        this._enableResetPasswordButton(false);
                               
        if (userlist!=null )
        {
            var rows = userlist.getSelectedRowElements();

            if(rows != null && rows.length > 0) {
                this._enableEditButton(true);
                this._enableDeleteButton(true);
                this._enableResetPasswordButton(true);
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

    _enableResetPasswordButton : function(en)
    {
        var resetPasswordButton = $find(this._ResetPasswordButtonClientID);
        resetPasswordButton.set_enable(en);
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
        
    get_ResetPasswordButtonClientID : function() {
        return this._ResetPasswordButtonClientID;
    },

    set_ResetPasswordButtonClientID : function(value) {
        this._ResetPasswordButtonClientID = value;
        this.raisePropertyChanged('ResetPasswordButtonClientID');
    },
        
    get_UserListClientID : function() {
        return this._UserListClientID;
    },

    set_UserListClientID : function(value) {
        this._UserListClientID = value;
        this.raisePropertyChanged('UserListClientID');
    }
},

// Register the class as a type that inherits from Sys.UI.Control.
Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel.registerClass('Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel', Sys.UI.Control);     

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
