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
Type.registerNamespace('Macro.ImageServer.Web.Application.Pages.WebViewer');

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constructor
Macro.ImageServer.Web.Application.Pages.WebViewer.SearchPanel = function(element) { 
    Macro.ImageServer.Web.Application.Pages.WebViewer.SearchPanel.initializeBase(this, [element]);
       
},

/////////////////////////////////////////////////////////////////////////////////////////////////////////
// Create the prototype for the control.
Macro.ImageServer.Web.Application.Pages.WebViewer.SearchPanel.prototype =
{
    initialize: function() {
        Macro.ImageServer.Web.Application.Pages.WebViewer.SearchPanel.callBaseMethod(this, 'initialize');

        this._OnViewImagesButtonClickedHandler = Function.createDelegate(this, this._OnViewImagesButtonClicked);
        this._OnStudyListRowClickedHandler = Function.createDelegate(this, this._OnStudyListRowClicked);
        this._OnStudyListRowDblClickedHandler = Function.createDelegate(this, this._OnStudyListRowDblClicked);
        this._OnLoadHandler = Function.createDelegate(this, this._OnLoad);
        Sys.Application.add_load(this._OnLoadHandler);

    },

    dispose: function() {
        $clearHandlers(this.get_element());

        Macro.ImageServer.Web.Application.Pages.WebViewer.SearchPanel.callBaseMethod(this, 'dispose');

        Sys.Application.remove_load(this._OnLoadHandler);
    },


    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Events

    /// called whenever the page is reloaded or partially reloaded
    _OnLoad: function() {
        // hook up the events... It is necessary to do this every time 
        // because NEW instances of the button and the study list components
        // may have been created as the result of the post-back
        var viewButton = $find(this._ViewImageButtonClientID);
        if (viewButton != null) viewButton.add_onClientClick(this._OnViewImagesButtonClickedHandler);

        var studylist = $find(this._StudyListClientID);
        studylist.add_onClientRowClick(this._OnStudyListRowClickedHandler);

        studylist.add_onClientRowDblClick(this._OnStudyListRowDblClickedHandler);

        this._updateToolbarButtonStates();
    },

    // called when the View Images button is clicked
    _OnViewImagesButtonClicked: function(src, event) {
        this._viewSelectedStudies();
        return false;
    },

    // called when user clicked on a row in the study list
    _OnStudyListRowClicked: function(sender, event) {
        this._updateToolbarButtonStates();
    },

    // called when user double-clicked on a row in the study list
    _OnStudyListRowDblClicked: function(sender, event) {
        this._updateToolbarButtonStates();
        this._viewSelectedStudies();
    },

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Private Methods

    // return the study instance uid of the row 
    _getInstanceUid: function(row) {
        //"instanceuid" is a custom attribute injected by the study list control
        return row.getAttribute('instanceuid');
    },

    _getServerPartitionAE: function(row) {
        //"serverae" is a custom attribute injected by the study list control
        return row.getAttribute('serverae');
    },

    _canViewImages: function(row) {
        //"canviewimages" is a custom attribute injected by the study list control
        return row.getAttribute('canviewimages') == 'true';
    },

    _canViewImagesMessage: function(row) {
        //"canviewimagesreason" is a custom attribute injected by the study list control
        return row.getAttribute('canviewimagesreason');
    },

    _viewSelectedStudies: function() {
        var studylist = $find(this._StudyListClientID);
        // open the selected studies
        if (studylist != null) {
            var rows = studylist.getSelectedRowElements();
            var serverae;
            var urlPartStudies = '';

            if (rows.length > 0) {
                for (i = 0; i < rows.length; i++) {

                    if (!this._canViewImages(rows[i])) {
                        alert("The selected study cannot be viewed at this time: " + this._canViewImagesMessage(rows[i]));
                        return;
                    }

                    serverae = this._getServerPartitionAE(rows[i]);
                    var instanceuid = this._getInstanceUid(rows[i]);
                    if (instanceuid != undefined && serverae != undefined) {
                        if (i == 0)
                            urlPartStudies += String.format('study={0}', instanceuid);
                        else
                            urlPartStudies += String.format(',study={0}', instanceuid);
                    }
                }
                var url = String.format('{0}?WebViewerInitParams=aetitle={3},{4},username={1},session={2}', this._ViewImagePageUrl, this._Username, this._SessionId, serverae, urlPartStudies);
                window.open(url, '_self', '', false);
            }
        }
        return false;
    },

    _updateToolbarButtonStates: function() {
        var studylist = $find(this._StudyListClientID);
        this._enableViewImageButton(false);

        if (studylist != null) {
            var rows = studylist.getSelectedRowElements();

            if (rows != null && rows.length > 0) {
                var selectedStudyCount = rows.length;
                var canViewImagesCount = 0;

                if (rows.length > 0) {
                    for (i = 0; i < rows.length; i++) {
                        if (this._canViewImages(rows[i])) {
                            canViewImagesCount++;
                        }
                    }
                }
                this._enableViewImageButton(canViewImagesCount == selectedStudyCount);
            }
        }
    },

    _enableViewImageButton: function(en) {
        var button = $find(this._ViewImageButtonClientID);
        if (button != null) button.set_enable(en);
    },

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Properties
    get_ViewImageButtonClientID: function() {
        return this._ViewImageButtonClientID;
    },

    set_ViewImageButtonClientID: function(value) {
        this._ViewImageButtonClientID = value;
        this.raisePropertyChanged('ViewImageButtonClientID');
    },

    get_StudyListClientID: function() {
        return this._StudyListClientID;
    },

    set_StudyListClientID: function(value) {
        this._StudyListClientID = value;
        this.raisePropertyChanged('StudyListClientID');
    },

    get_ViewImagePageUrl: function() {
        return this._ViewImagePageUrl;
    },

    set_ViewImagePageUrl: function(value) {
        this._ViewImagePageUrl = value;
        this.raisePropertyChanged('ViewImagePageUrl');
    },

    get_Username: function() {
        return this._Username;
    },

    set_Username: function(value) {
        this._Username = value;
        this.raisePropertyChanged('Username');
    },

    get_SessionId: function() {
        return this._SessionId;
    },

    set_SessionId: function(value) {
        this._SessionId = value;
        this.raisePropertyChanged('SessionId');
    },

    get_CanViewImages: function() {
        return this._CanViewImages;
    },

    set_CanViewImages: function(value) {
        this._CanViewImages = value;
        this.raisePropertyChanged('CanViewImages');
    }
},

// Register the class as a type that inherits from Sys.UI.Control.
Macro.ImageServer.Web.Application.Pages.WebViewer.SearchPanel.registerClass('Macro.ImageServer.Web.Application.Pages.WebViewer.SearchPanel', Sys.UI.Control);
     
if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();