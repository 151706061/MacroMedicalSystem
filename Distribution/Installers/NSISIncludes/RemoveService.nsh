#Depends on KillProc and servicelib.nsh

!macro RemoveService ServiceProcess ServiceName

Function un.RemoveService
  !undef UN
  !define UN "un."
  StrCpy $R1 1
  ${Do}
    DetailPrint "Stopping Service: Attempt $R1"
    #!insertmacro SERVICE "running" "ClearCanvas Image Server Shred Host Service" "action=stop;"
    !insertmacro SERVICE "running" "${ServiceName}" "action=stop;"
    Pop $0
    IntOp $R1 $R1 + 1
    ${If} $0 == "true"
      DetailPrint "Success: Service stopped"
      goto uponServiceStop
    ${EndIf}

    ${If} $R1 > 3
      DetailPrint "Failed: Service could not be stopped"
      ${ExitDo}
    ${EndIf}
    Sleep 3000
  ${Loop}

   ;If stop service fails 3 times then installer attempts to kill the process
   DetailPrint "Attemping to kill the Service"
   #StrCpy $0 "ClearCanvas.ImageServer.ShredHostService.exe"
   DetailPrint "Searching for processes called '${ServiceProcess}'"
   KillProc::FindProcesses
   StrCmp $1 "-1" killError
   DetailPrint "-> Found $0 processes"

   StrCmp $0 "0" uponServiceStop
   Sleep 1500

   #StrCpy $0 "ClearCanvas.ImageServer.ShredHostService.exe"
   DetailPrint "Killing all processes called '${ServiceProcess}'"
   KillProc::KillProcesses
   StrCmp $1 "-1" killError
   DetailPrint "-> Killed $0 processes, faild to kill $1 processes"

   Goto uponServiceStop

   killError:
   DetailPrint "-> Error: Service could not be terminated.  Cancelling install."
   Abort

   uponServiceStop:
   DetailPrint "Attempting to uninstall Service"
   sleep 3000
  #!insertmacro SERVICE "installed" "ClearCanvas Image Server Shred Host Service" "action=delete;"
  !insertmacro SERVICE "installed" "${ServiceName}" "action=delete;"
  DetailPrint "Success: Service uninstalled"
FunctionEnd
!macroend