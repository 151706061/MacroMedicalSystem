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
Type.registerNamespace('Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel = function(element) { 
    Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.initializeBase(this, [element]);
       
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.prototype = 
{
    initialize : function() {
        Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.callBaseMethod(this, 'initialize');        
            
        this._OnUserGroupsListRowClickedHandler = Function.createDelegate(this,this._OnUserGroupsListRowClicked);
            
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        Sys.Application.add_load(this._OnLoadHandler);
                 
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.callBaseMethod(this, 'dispose');
            
        Sys.Application.remove_load(this._OnLoadHandler);
    },
        
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
    _OnLoad : function()
    {                    
        var userlist = $find(this._UserGroupsListClientID);
        userlist.add_onClientRowClick(this._OnUserGroupsListRowClickedHandler);
                 
        this._updateToolbarButtonStates();
    },
        
    // called when user clicked on a row in the study list
    _OnUserGroupsListRowClicked : function(sender, event)
    {    
        this._updateToolbarButtonStates();        
    },
                       
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _updateToolbarButtonStates : function()
    {
        var userlist = $find(this._UserGroupsListClientID);
                      
        this._enableEditButton(false);
        this._enableDeleteButton(false);
                               
        if (userlist!=null )
        {
            var rows = userlist.getSelectedRowElements();

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
        
    get_UserGroupsListClientID : function() {
        return this._UserGroupsListClientID;
    },

    set_UserGroupsListClientID : function(value) {
        this._UserGroupsListClientID = value;
        this.raisePropertyChanged('UserGroupsListClientID');
    }
},

// Register the class as a type that inherits from Sys.UI.Control.
Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.registerClass('Macro.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel', Sys.UI.Control);
     
if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();