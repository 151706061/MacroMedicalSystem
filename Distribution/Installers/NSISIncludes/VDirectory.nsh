!macro VDirectory
; The next two functions were taken from: http://nsis.sourceforge.net/Setting_up_a_virtual_directory
LangString ERR_VDIREXISTS ${LANG_ENGLISH} "A virtual directory named $VDIRNAME already exists. The new virtual directory will not be created."


## Will Set ErrorFlag if the Virtual Directory cannot be created.
; CreateVDir Function
Function CreateVDir

;Open a VBScript File in the temp dir for writing
DetailPrint "Creating $TEMP\createVDir.vbs";
FileOpen $0 "$TEMP\createVDir.vbs" w

;Write the script:
;Create a virtual dir named $VDIRNAME pointing to $INSTDIR\web with proper attributes
FileWrite $0 "On Error Resume Next$\n"
FileWrite $0 "Set Root = GetObject($\"IIS://LocalHost/W3SVC/1/ROOT$\")$\n"
FileWrite $0 "Set Dir = Root.Create($\"IIsWebVirtualDir$\", $\"$VDIRNAME$\")$\n"
FileWrite $0 "If (Err.Number <> 0) Then$\n"
FileWrite $0 "  If (Err.Number <> -2147024713) Then$\n"
FileWrite $0 "    message = $\"Error $\" & Err.Number$\n"
FileWrite $0 "    message = message & $\" trying to create IIS virtual directory.$\" & chr(13)$\n"
FileWrite $0 "    message = message & $\"Please check your IIS settings (inetmgr).$\"$\n"
FileWrite $0 "    MsgBox message, vbCritical, $\"${PRODUCT_NAME}$\"$\n"
FileWrite $0 "    WScript.Quit (Err.Number)$\n"
FileWrite $0 "  End If$\n"
FileWrite $0 "  ' Error -2147024713 means that the virtual directory already exists.$\n"
FileWrite $0 "  ' We will check if the parameters are the same: if so, then OK.$\n"
FileWrite $0 "  ' If not, then fail and display a message box.$\n"
FileWrite $0 "  Set Dir = GetObject($\"IIS://LocalHost/W3SVC/1/ROOT/$VDIRNAME$\")$\n"
FileWrite $0 "  If (Dir.Path <> $\"$INSTDIR\web$\") Then$\n"
FileWrite $0 "    message = $\"Virtual Directory $VDIRNAME already exists in a different folder ($\" + Dir.Path + $\").$\" + chr(13)$\n"
FileWrite $0 "    message = message + $\"Please delete the virtual directory using the IIS console (inetmgr), and install again.$\"$\n"
FileWrite $0 "    MsgBox message, vbCritical, $\"${PRODUCT_NAME}$\"$\n"
FileWrite $0 "    Wscript.Quit (Err.Number)$\n"
FileWrite $0 "  End If$\n"
FileWrite $0 "  If (Dir.AspAllowSessionState <> True  Or  Dir.AccessScript <> True) Then$\n"
FileWrite $0 "    message = $\"Virtual Directory $VDIRNAME already exists and has incompatible parameters.$\" + chr(13)$\n"
FileWrite $0 "    message = message + $\"Please delete the virtual directory using the IIS console (inetmgr), and install again.$\"$\n"
FileWrite $0 "    MsgBox message, vbCritical, $\"${PRODUCT_NAME}$\"$\n"
FileWrite $0 "    Wscript.Quit (Err.Number)$\n"
FileWrite $0 "  End If$\n"
FileWrite $0 "  Wscript.Quit (0)$\n"
FileWrite $0 "End If$\n"
FileWrite $0 "Dir.Path = $\"$INSTDIR\web$\"$\n"
FileWrite $0 "Dir.AccessRead = True$\n"
FileWrite $0 "Dir.AccessWrite = False$\n"
FileWrite $0 "Dir.AccessScript = True$\n"
FileWrite $0 "Dir.AppFriendlyName = $\"$VDIRNAME$\"$\n"
FileWrite $0 "Dir.EnableDirBrowsing = False$\n"
FileWrite $0 "Dir.ContentIndexed = False$\n"
FileWrite $0 "Dir.DontLog = True$\n"
FileWrite $0 "Dir.EnableDefaultDoc = True$\n"
FileWrite $0 "Dir.DefaultDoc = $\"default.aspx$\"$\n"
FileWrite $0 "Dir.AspBufferingOn = True$\n"
FileWrite $0 "Dir.AspAllowSessionState = True$\n"
FileWrite $0 "Dir.AspSessionTimeout = 30$\n"
FileWrite $0 "Dir.AspScriptTimeout = 900$\n"
FileWrite $0 "Dir.SetInfo$\n"
FileWrite $0 "Set IISObject = GetObject($\"IIS://LocalHost/W3SVC/1/ROOT/$VDIRNAME$\")$\n"
FileWrite $0 "IISObject.AppCreate2(2) 'Create a process-pooled web application$\n"
FileWrite $0 "If (Err.Number <> 0) Then$\n"
FileWrite $0 " message = $\"Error $\" & Err.Number$\n"
FileWrite $0 " message = message & $\" trying to create the virtual directory at 'IIS://LocalHost/W3SVC/1/ROOT/$VDIRNAME'$\" & chr(13)$\n"
FileWrite $0 " message = message & $\"Please check your IIS settings (inetmgr).$\"$\n"
FileWrite $0 " MsgBox message, vbCritical, $\"${PRODUCT_NAME}$\"$\n"
FileWrite $0 " WScript.Quit (Err.Number)$\n"
FileWrite $0 "End If$\n"

FileClose $0

DetailPrint "Executing $TEMP\createVDir.vbs"
nsExec::Exec /TIMEOUT=20000 '"$SYSDIR\cscript.exe" "$TEMP\createVDir.vbs"'
Pop $1
StrCmp $1 "0" CreateVDirOK
DetailPrint "Error $1 in CreateVDir.vbs - Failed to create IIS Virtual Directory"
SetErrors
Return

CreateVDirOK:
DetailPrint "Successfully created IIS virtual directory"
Delete "$TEMP\createVDir.vbs"
FunctionEnd

;--------------------------------
; DeleteVDir Function
Function un.DeleteVDir

;Open a VBScript File in the temp dir for writing
DetailPrint "Creating $TEMP\deleteVDir.vbs";
FileOpen $0 "$TEMP\deleteVDir.vbs" w

;Write the script:
;Create a virtual dir named $VDIRNAME pointing to $INSTDIR\web with proper attributes
FileWrite $0 "On Error Resume Next$\n$\n"
;Delete the application object
FileWrite $0 "Set IISObject = GetObject($\"IIS://LocalHost/W3SVC/1/ROOT/$VDIRNAME$\")$\n$\n"
FileWrite $0 "IISObject.AppDelete 'Delete the web application$\n"
FileWrite $0 "If (Err.Number <> 0) Then$\n"
FileWrite $0 " MsgBox $\"Error trying to delete the application at [IIS://LocalHost/W3SVC/1/ROOT/$VDIRNAME]$\", vbCritical, $\"${PRODUCT_NAME}$\"$\n"
FileWrite $0 " WScript.Quit (Err.Number)$\n"
FileWrite $0 "End If$\n$\n"

FileWrite $0 "Set IISObject = GetObject($\"IIS://LocalHost/W3SVC/1/ROOT$\")$\n$\n"
FileWrite $0 "IIsObject.Delete $\"IIsWebVirtualDir$\", $\"$VDIRNAME$\"$\n"
FileWrite $0 "If (Err.Number <> 0) Then$\n"
FileWrite $0 " MsgBox $\"Error trying to delete the virtual directory '$VDIRNAME' at 'IIS://LocalHost/W3SVC/1/ROOT'$\", vbCritical, $\"${PRODUCT_NAME}$\"$\n"
FileWrite $0 " Wscript.Quit (Err.Number)$\n"
FileWrite $0 "End If$\n$\n"

FileClose $0

DetailPrint "Executing $TEMP\deleteVDir.vbs"
nsExec::Exec /TIMEOUT=20000 '"$SYSDIR\cscript.exe" "$TEMP\deleteVDir.vbs"'
Pop $1
StrCmp $1 "0" +2
DetailPrint "Error $1 in deleteVDir.vbs"
goto DeleteVDirEnd
DetailPrint "Virtual Directory $VDIRNAME successfully removed."
Delete "$TEMP\deleteVDir.vbs"
DeleteVDirEnd:
FunctionEnd
!macroend

!macro RegisterMimeType FileExtension MimeType
DetailPrint "Creating $TEMP\regMime.vbs";
FileOpen $0 "$TEMP\regMime.vbs" w

FileWrite $0 "Set objMimeMap = GetObject($\"IIS://$\" & $\"localhost/MimeMap$\")$\n"
FileWrite $0 "Dim strExt, strMimeType$\n"
FileWrite $0 "arrMaps = objMimeMap.GetEx($\"MimeMap$\")$\n"
FileWrite $0 "For i = 0 to Ubound(arrMaps)$\n"
FileWrite $0 "If arrMaps(i).Extension = $\"${FileExtension}$\" Then$\n"
FileWrite $0 "Wscript.Quit$\n"
FileWrite $0 "End If$\n"
FileWrite $0 "Next$\n"
FileWrite $0 "lngMimeMapListCount = UBound(arrMaps) + 1$\n"
FileWrite $0 "Redim Preserve arrMaps(lngMimeMapListCount)$\n"
FileWrite $0 "Set arrMaps(lngMimeMapListCount) = CreateObject($\"MimeMap$\")$\n"
FileWrite $0 "arrMaps(lngMimeMapListCount).Extension = $\"${FileExtension}$\"$\n"
FileWrite $0 "arrMaps(lngMimeMapListCount).MimeType = $\"${MimeType}$\"$\n"
FileWrite $0 "objMimeMap.PutEx 2, $\"MimeMap$\", arrMaps$\n"
FileWrite $0 "objMimeMap.SetInfo$\n"

FileClose $0

DetailPrint "Executing $TEMP\regMime.vbs"
nsExec::Exec /TIMEOUT=20000 '"$SYSDIR\cscript.exe" "$TEMP\regMime.vbs"'

Delete "$TEMP\regMime.vbs"

!macroend