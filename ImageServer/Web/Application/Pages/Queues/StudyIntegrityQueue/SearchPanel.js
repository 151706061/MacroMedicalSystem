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
Type.registerNamespace('Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue');
        
        
/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel = function(element) { 
    Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel.initializeBase(this, [element]);
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel.prototype = 
{
    initialize : function() {
        Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel.callBaseMethod(this, 'initialize');        
            
        this._OnItemListRowClickedHandler = Function.createDelegate(this,this._OnItemListRowClicked);
        this._OnItemListRowDblClickedHandler = Function.createDelegate(this,this._OnItemListRowDblClicked);
        this._OnLoadHandler = Function.createDelegate(this,this._OnLoad);
        Sys.Application.add_load(this._OnLoadHandler);        
    },
        
    dispose : function() {
        $clearHandlers(this.get_element());

        Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel.callBaseMethod(this, 'dispose');
            
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
        var itemlist = $find(this._ItemListClientID);
        itemlist.add_onClientRowClick(this._OnItemListRowClickedHandler);
        itemlist.add_onClientRowDblClick(this._OnItemListRowDblClickedHandler);
            
        this._updateToolbarButtonStates();
    },
                     
    // called when user clicked on a row in the study list
    _OnItemListRowClicked : function(sender, event)
    {           
        this._updateToolbarButtonStates();        
    },
        
    // called when user double-clicked on a row in the study list
    _OnItemListRowDblClicked : function(sender, event)
    {
        this._updateToolbarButtonStates();
    },
                      
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Methods
    _updateToolbarButtonStates : function()
    {
        var itemlist = $find(this._ItemListClientID);
                      
        this._enableReconcileButton(false);
        if (itemlist!=null )
        {
            var rows = itemlist.getSelectedRowElements();
            if (rows.length>0)
            {
                for(i=0; i<rows.length; i++)
                {
                    this._enableReconcileButton(this._canReconcileStudy(rows[i]));
                }
            }
        }
    },     
        
    _enableReconcileButton : function(en)
    {
        var reconcileButton = $find(this._ReconcileButtonClientID);
        if(reconcileButton != null) reconcileButton.set_enable(en);
    },
        
    _canReconcileStudy:function(row)
    {
        //"canreconcile" is a custom attribute injected by the list control
        return row.getAttribute('canreconcile')=='true';
    },
                     
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
    get_ReconcileButtonClientID : function() {
        return this._ReconcileButtonClientID;
    },

    set_ReconcileButtonClientID : function(value) {
        this._ReconcileButtonClientID = value;
        this.raisePropertyChanged('ReconcileButtonClientID');
    },
                        
    get_ItemListClientID : function() {
        return this._ItemListClientID;
    },

    set_ItemListClientID : function(value) {
        this._ItemListClientID = value;
        this.raisePropertyChanged('ItemListClientID');
    }
},
   
// Register the class as a type that inherits from Sys.UI.Control.
Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel.registerClass('Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SearchPanel', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();
