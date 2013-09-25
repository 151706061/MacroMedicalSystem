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
/// This script contains the javascript component class for the work queue search panel

Type.registerNamespace('Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel = function(element) { 
    Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.initializeBase(this, [element]);       
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.prototype = 
{
    initialize : function() {
        
        Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.callBaseMethod(this, 'initialize');        
            
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        this._OnViewDetailsButtonClickedHandler = Function.createDelegate(this,this._OnViewDetailsButtonClickedHandler);
                        
        Sys.Application.add_load(this._OnLoadHandler);        
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.callBaseMethod(this, 'dispose');
            
        Sys.Application.remove_load(this._OnLoadHandler);
    },
        
        
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events
        
    /// called whenever the page is reloaded or partially reloaded
    _OnLoad : function()
    {         
        var viewDetailsButton = $find(this._ViewStudiesButtonClientID);
        if(viewDetailsButton != null) viewDetailsButton.add_onClientClick( this._OnViewDetailsButtonClickedHandler );  
    },    

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Public methods
    _OnViewDetailsButtonClickedHandler : function(sender, event)
    {
        this._openStudy();
    },
                
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods
    _openStudy : function()
    {
        var url = String.format('{0}?serverae={1}&siuid={2}', this._OpenStudyPageUrl, this._ServerAE, this._StudyInstanceUid);
        window.open(url);
    },
                
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
    get_ViewStudiesButtonClientID : function() {
        return this._ViewStudiesButtonClientID;
    },
    set_ViewStudiesButtonClientID : function(value) {
        this._ViewStudiesButtonClientID = value;
        this.raisePropertyChanged('ViewStudiesButtonClientID');
    },
    
    get_OpenStudyPageUrl : function() {
        return this._OpenStudyPageUrl;
    },
    set_OpenStudyPageUrl : function(value) {
        this._OpenStudyPageUrl = value;
        this.raisePropertyChanged('OpenStudyPageUrl');
    },
    
    get_ServerAE : function() {
        return this._ServerAE;
    },
    set_ServerAE : function(value) {
        this._ServerAE = value;
        this.raisePropertyChanged('ServerAE');
    },      
        
    get_StudyInstanceUid : function() {
        return this._StudyInstanceUid;
    },
    set_StudyInstanceUid : function(value) {
        this._StudyInstanceUid = value;
        this.raisePropertyChanged('StudyInstanceUid');
    }                              
},

// Register the class as a type that inherits from Sys.UI.Control.

Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.registerClass('Macro.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
