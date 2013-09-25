#Set of macros used to present the Usage Tracking Page and make the adjustments to the app.config files based on the user selection

!macro UsageTrackingAgreementPage
  !ifndef XIANROOT
    !define XIANROOT "..\..\.."
  !endif
  
  !define USAGETRACKINGAGREEMENTFILENAME "PrivacyNotice.rtf"
  !ifndef NSISINCLUDES
    !define NSISINCLUDES "${XIANROOT}\Distribution\Installers\NSISIncludes"
  !endif

  !include "${NSISINCLUDES}\nsDialogs.nsh"
  !include "${NSISINCLUDES}\LoadRTF.nsh"
  
  # define constants for working with the rich edit control
  !define /math EM_AUTOURLDETECT ${WM_USER} + 91
  !define /math EM_GETTEXTRANGE ${WM_USER} + 75
  !define /math EM_SETEVENTMASK ${WM_USER} + 69
  !define ENM_LINK 0x4000000
  !define EN_LINK 0x70B
  
  Var UsageTrackingChecked
  Var hwndRichTextBox
  Var hwndCheckbox
  
  Page custom PageUsageTrackingAgreement ValidatePageUsageTrackingAgreement
  
  LangString PageUsageTrackingAgreement_TITLE ${LANG_ENGLISH} "Usage Tracking Agreement"
  LangString PageUsageTrackingAgreement_SUBTITLE ${LANG_ENGLISH} "Please review the usage tracking terms before installing ${PRODUCT_NAME} ${PRODUCT_VERSION}."

  Function PageUsageTrackingAgreement
    SetOutPath "$TEMP"
    File "${XIANROOT}\Docs\${USAGETRACKINGAGREEMENTFILENAME}"
    !insertmacro MUI_HEADER_TEXT "$(PageUsageTrackingAgreement_TITLE)" "$(PageUsageTrackingAgreement_SUBTITLE)"

    nsDialogs::Create 1018

    ${NSD_CreateLabel} 0 0 100% 9u "Press Page Down to see the rest of the agreement."

    nsDialogs::CreateControl /NOUNLOAD RichEdit20A ${WS_VISIBLE}|${WS_CHILD}|${WS_TABSTOP}|${WS_VSCROLL}|${ES_MULTILINE}|${ES_READONLY} ${__NSD_Text_EXSTYLE}|${WS_EX_NOPARENTNOTIFY} 0 24 100% 105u
    Pop $hwndRichTextBox
    SendMessage $hwndRichTextBox ${EM_AUTOURLDETECT} 1 0 ;tell the RTF window to automatically detect URLs and handle them as links
    SendMessage $hwndRichTextBox ${EM_SETEVENTMASK} 0 ${ENM_LINK} ;tell the RTF window to notify us of link events
    ${NSD_OnNotify} $hwndRichTextBox OnNotifyPageUsageTrackingAgreement ;register a page notification event handler
    ${LoadRTF} "$TEMP\${USAGETRACKINGAGREEMENTFILENAME}" $hwndRichTextBox

    ${NSD_CreateCheckbox} 0 205 100% 9u "Help ClearCanvas by automatically sending anonymous usage statistics."
    Pop $hwndCheckbox
    ${NSD_SetState} $hwndCheckbox ${BST_CHECKED}

    nsDialogs::Show
  FunctionEnd

  Function ValidatePageUsageTrackingAgreement
    ${NSD_GetState} $hwndCheckbox $UsageTrackingChecked
  FunctionEnd
  
  Function OnNotifyPageUsageTrackingAgreement
    Pop $0 ;window handle (hWnd)
    Pop $1 ;notification code
    Pop $2 ;pointer to NMHDR-derived structure
    
    ;verify message source and id
    ${If} $0 = $hwndRichTextBox
      ${If} $1 = ${EN_LINK}
        ;extract values from the ENLINK struct: hWndFrom, idFrom, code, msg, wParam, lParam, cpMin, cpMax
        ;N.B. nested NMHDR and CHARRANGE structs must be expanded because their individual fields are stored sequentially
        System::Call "*$2(i.r0, i.r1, i.r2, i.r3, i.r4, i.r5, i.r6, i.r7)" ;deref struct at $2 and read 8 ints into registers $0 through $7
        
        ${If} $3 = ${WM_LBUTTONDOWN} ;to be consistent with regular MUI License, we follow link on mouse down
          ${If} $7 >= 0 ;CHARRANGE allows a negative cpMax to denote everything but it wouldn't be a valid link in RichEdit20A, so just check for it
            IntOp $0 $7 - $6 ;compute the length of the link URL text
            IntOp $0 $0 + 1 ;add one for terminating NULL
            
            ;allocate a string buffer to hold the link URL
            System::Call "*(&t$0) i.R0" ;new struct defined as an array of $0 TCHARs; put resultant address in $R0
            
            ;allocate a TEXTRANGE struct and fill it with CHARRANGE from before and the allocated string buffer
            System::Call "*(i$6, i$7, i$R0) i.R1" ;new struct defined as the values of cpMin, cpMax and the buffer from before; put resultant address in $R1
            
            ;request the text selection as specified by the URL text range
            SendMessage $hwndRichTextBox ${EM_GETTEXTRANGE} 0 $R1 $1 ;function returns the number of characters copied without terminating NULL
            
            ;extract link URL from the string buffer
            System::Call "*$R0(&t$0.r2)" ;deref string buffer at $R0 and put the *TCHAR into register $2
            
            ;open the link URL
            ExecShell "open" "$2"
            
            ;free the TEXTRANGE and string buffer we allocated
            System::Free $R1
            System::Free $R0
          ${EndIf}
        ${EndIf}
      ${EndIf}
    ${EndIf}
  FunctionEnd
!macroend

!macro ConfigureUsageTracking ConfigFile
  ${xml::LoadFile} "${ConfigFile}" $0
  #Setting Explorer Behaviour
  ${xml::GotoPath} "/configuration/applicationSettings/ClearCanvas.Common.UsageTracking.UsageTrackingSettings" $0
  ${xml::FirstChildElement} "setting" $0 $1

  loopEnabled:
  ${xml::GetAttribute} "name" $0 $1
  StrCmp "Enabled" $0 foundEnabled ;breaks loop

  ${xml::NextSiblingElement} "setting" $0 $1
  StrCmp $1 "0" loopEnabled findFailed ;loops unless an error occurred

  foundEnabled:
  ${xml::FirstChildElement} "value" $0 $1
  StrCmp $UsageTrackingChecked "1" userAgreed
  StrCmp $UsageTrackingChecked "0" userDeclined
  
  userAgreed:
    ;Enable Usage Tracking
    ${xml::SetText} "True" $0
    
    ; proceed to update other related setting
    ${xml::GotoPath} "/configuration/applicationSettings/ClearCanvas.Common.UsageTracking.UsageTrackingSettings" $0
    ${xml::FirstChildElement} "setting" $0 $1

    loopDisplayMessages:
    ${xml::GetAttribute} "name" $0 $1
    StrCmp "DisplayMessages" $0 foundDisplayMessages ;breaks loop

    ${xml::NextSiblingElement} "setting" $0 $1
    StrCmp $1 "0" loopDisplayMessages findFailed ;loops unless an error occurred

    foundDisplayMessages:
    ${xml::FirstChildElement} "value" $0 $1
    ${xml::SetText} "True" $0
    goto saveUsageTrackingValues

  userDeclined:
    ;Disable Usage Tracking
    ${xml::SetText} "False" $0
    goto saveUsageTrackingValues


  saveUsageTrackingValues:
  ${xml::SaveFile} "${ConfigFile}" $0

  findFailed:
  ${xml::Unload}
!macroend