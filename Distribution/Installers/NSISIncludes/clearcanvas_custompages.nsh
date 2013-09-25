!ifndef __CLEARCANVAS_CUSTOMPAGES
  !define __CLEARCANVAS_CUSTOMPAGES
  
  !include LogicLib.nsh
  !include "nsDialogs.nsh"
  
  !macro CC_PAGE_INSTTYPE
    Page custom CCInstallTypeShow CCInstallTypeLeave
  !macroend
  
  !macro CC_GET_INSTTYPE type

  !macroend
  
  Function CCInstallTypeShow
    Push $R1
  
    ; creates the dialog we will use
    nsDialogs::Create 1018
    Pop $R0

    ; abort if dialog creation failed
    ${If} $R0 == error
      Abort
    ${EndIf}
    
    ; disable the standard next button
    GetDlgItem $R1 $HWNDPARENT 1
    EnableWindow $R1 0
    
    ; set the MUI page header title
    GetDlgItem $R1 $HWNDPARENT 0x40D
    StrCpy $R2 "Select your installation"
    SendMessage $R1 ${WM_SETTEXT} 0 STR:$R2

    ; set the MUI page header description
    GetDlgItem $R1 $HWNDPARENT 0x40E
    StrCpy $R2 "Select the type of installation you would like to perform."
    SendMessage $R1 ${WM_SETTEXT} 0 STR:$R2
    
    ; create our R+V button, and install a click handler
    ${NSD_CreateButton} 10% 15% 80% 25% "ImageViewer Only"
    Pop $R1
    ${NSD_OnClick} $R1 CCInstallTypeViewerOnlyClick
    
    ; create out V-only button, and install a click handler
    ${NSD_CreateButton} 10% 50% 80% 25% "ImageViewer && RIS Client"
    Pop $R1
    ${NSD_OnClick} $R1 CCInstallTypeRisViewerClick
    
    ; now show the dialog
    nsDialogs::Show
    
    Pop $R1
  FunctionEnd
  
  Function CCInstallTypeRisViewerClick
    StrCpy $R0 "RISVIEWER"
    SendMessage $HWNDPARENT "0x408" "1" ""
  FunctionEnd
  
  Function CCInstallTypeViewerOnlyClick
    StrCpy $R0 "VIEWERONLY"
    SendMessage $HWNDPARENT "0x408" "1" ""
  FunctionEnd
!endif