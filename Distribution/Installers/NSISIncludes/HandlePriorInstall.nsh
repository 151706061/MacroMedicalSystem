#Usage:
; Push "Path to version determining assembly from installdir"
; Push "Function Address for callback function to handle an older version installed"
; Push "Function Address for callback function to handle the same version already installed"
; Push "Function Address for callback function to handle a higher version already installed"
; Push "Function Address for callback function to handle an uninstall failure"
; Call HandlePriorInstall

#Behaviour:
; Default behaviour is that upon any detection an uninstall will be triggered.
; Installation can otherwise be Aborted on callback operation

###Variables that must be defined
; ${PRODUCT_NAME}
; ${MAJOR_VERSION}
; ${MINOR_VERSION}
; ${REVISION_NUMBER}
; ${BUILD_NUMBER}

;${PRODUCT_UNINST_ROOT_KEY}
;${PRODUCT_UNINST_KEY}

Function HandlePriorInstall
  ## Test for existence using the UninstallString
  ReadRegStr $R0 ${PRODUCT_UNINST_ROOT_KEY} \
  "${PRODUCT_UNINST_KEY}" \
  "UninstallString"
  StrCmp $R0 "" done

  Exch 4
  Pop $R2 ;Path
  Pop $R5 ;higher
  Pop $R4 ;same
  Pop $R3 ;older
  Pop $R6 ;failure

  ;MessageBox MB_OK "$R2 $R3 $R4 $R5 $R6"

  ## Get file version
  ;Making use of the Uninstall path to build the path to the exe
  StrCpy $R1 $R0 -10 #because "uninst.exe" is 10 characters long
  GetDllVersion "$R1$R2" $R7 $R9
  IntOp $R2 $R7 / 0x00010000
  IntOp $R7 $R7 & 0x0000FFFF
  IntOp $R8 $R9 / 0x00010000
  IntOp $R9 $R9 & 0x0000FFFF
  ;StrCpy $R5 "$R2.$R7.$R8.$R9"

  IntCmp ${MAJOR_VERSION} $R2 +1 installedVersionHigher installedVersionLower
  IntCmp ${MINOR_VERSION} $R7 +1 installedVersionHigher installedVersionLower
  IntCmp ${BUILD_NUMBER} $R8 +1 installedVersionHigher installedVersionLower
  IntCmp ${REVISION_NUMBER} $R9 sameVersionInstalled installedVersionHigher installedVersionLower


  installedVersionHigher:
  Call $R5
  GoTo done

  sameVersionInstalled:
  Call $R4
  GoTo done

  installedVersionLower:
  Call $R3
  GoTo done
  
  done:
FunctionEnd