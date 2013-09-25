# Heavily modified version of DotNet (taken from http://nsis.sourceforge.net/DotNET) - Original comments maintained below
# Updated to check for .NET Framework 3.0 Redistributable for use in ClearCanvas Workstation (www.clearcanvas.ca)
# ===================================================
# DotNET version checking macro.
# Written by AnarkiNet(AnarkiNet@gmail.com) originally, modified by eyal0 (for use in http://www.sourceforge.net/projects/itwister)
# Downloads and runs the Microsoft .NET Framework version 2.0 Redistributable and runs it if the user does not have the correct version.
# To use, call the macro with a string:
# !insertmacro CheckDotNET "2"
# !insertmacro CheckDotNET "2.0.9"
# (Version 2.0.9 is less than version 2.0.10.)
# All register variables are saved and restored by CheckDotNet
# No output
 
!macro CheckDotNET DotNetReqVer CheckOnly
!define DOTNET_URL "http://www.microsoft.com/downloads/info.aspx?na=90&p=&SrcDisplayLang=en&SrcCategoryId=&SrcFamilyId=10CC340B-F857-4A14-83F5-25634C3BF043&u=http%3a%2f%2fdownload.microsoft.com%2fdownload%2f4%2fd%2fa%2f4da3a5fa-ee6a-42b8-8bfa-ea5c4a458a7d%2fdotnetfx3setup.exe"
  DetailPrint "Checking your .NET Framework version..."

  ;callee register save
  Push $0
  Push $1
  Push $2
  Push $3
  Push $4
  Push $5
  Push $6 ;backup of intsalled ver
  Push $7 ;backup of DoNetReqVer
  Push $8 ;holds test of install sucess
  Push $9 ;return value for .NET install
 
  StrCpy $7 ${DotNetReqVer}
  ; retired system call as per http://msdn2.microsoft.com/en-us/library/aa964978.aspx#DetectingTheMicrosoftDotNETFramework3_0
  ; System::Call "mscoree::GetCORVersion(w .r0, i ${NSIS_MAX_STRLEN}, *i r2r2) i .r1 ?u"
  
  ; Initially thought to use the method described here: http://msdn2.microsoft.com/en-us/library/aa480173.aspx
  ; But ReadRegStr $0 HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\5.0\User Agent\Post Platform" Version doesn't exist
  ; and HKLM "Software\Microsoft\NET Framework Setup\NDP\v3.0\Setup\InstallSuccess\" only exists if .NET 3.0 was manually installed i.e. not on Vista
  ; since we only use the WCF portion of .NET 3.0 opted to use specifically that check instead as mentioned on this site: http://www.codeproject.com/useritems/frameworkversiondetection.asp

  ReadRegDWORD $8 HKLM "Software\Microsoft\NET Framework Setup\NDP\v3.0\Setup\Windows Communication Foundation" InstallSuccess
  ReadRegStr $0 HKLM "Software\Microsoft\NET Framework Setup\NDP\v3.0\Setup\Windows Communication Foundation" Version
  ${If} $8 != 1
        DetailPrint ".NET Framework 3.0 or greater not found, install is required for program to run."
    Goto NoDotNET
  ${EndIf}

  ${If} $0 == 0
  	DetailPrint ".NET Framework 3.0 or greater not found, install is required for program to run."
    Goto NoDotNET
  ${EndIf}
 
  ;at this point, $0 has maybe v2.345.678.
  ;StrCpy $0 $0 $2 1 ;remove the starting "v", $0 has the installed version num as a string -- not needed
  StrCpy $6 $0
  StrCpy $1 $7 ;$1 has the requested verison num as a string
 
  ;MessageBox MB_OKCANCEL "found $0" \
  ;  IDCANCEL GiveUpDotNET
    
  ;MessageBox MB_OKCANCEL "looking for $1" \
  ;  IDCANCEL GiveUpDotNET
 
  ;now let's compare the versions, installed against required <part0>.<part1>.<part2>.
  ${Do}
    StrCpy $2 "" ;clear out the installed part
    StrCpy $3 "" ;clear out the required part
 
    ${Do}
      ${If} $0 == "" ;if there are no more characters in the version
        StrCpy $4 "." ;fake the end of the version string
      ${Else}
        StrCpy $4 $0 1 0 ;$4 = character from the installed ver
        ${If} $4 != "."
          StrCpy $0 $0 ${NSIS_MAX_STRLEN} 1 ;remove that first character from the remaining
        ${EndIf}
      ${EndIf}
      
      ${If} $1 == ""  ;if there are no more characters in the version
        StrCpy $5 "." ;fake the end of the version string
      ${Else}
        StrCpy $5 $1 1 0 ;$5 = character from the required ver
        ${If} $5 != "."
          StrCpy $1 $1 ${NSIS_MAX_STRLEN} 1 ;remove that first character from the remaining
        ${EndIf}
      ${EndIf}
      ;MessageBox MB_OKCANCEL "installed $2,$4,$0 required $3,$5,$1" \
      ;    IDCANCEL GiveUpDotNET
      ${If} $4 == "."
      ${AndIf} $5 == "."
        ${ExitDo} ;we're at the end of the part
      ${EndIf}
 
      ${If} $4 == "." ;if we're at the end of the current installed part
        StrCpy $2 "0$2" ;put a zero on the front
      ${Else} ;we have another character
        StrCpy $2 "$2$4" ;put the next character on the back
      ${EndIf}
      ${If} $5 == "." ;if we're at the end of the current required part
        StrCpy $3 "0$3" ;put a zero on the front
      ${Else} ;we have another character
        StrCpy $3 "$3$5" ;put the next character on the back
      ${EndIf}
    ${Loop}
    ;MessageBox MB_OKCANCEL "finished parts: installed $2,$4,$0 required $3,$5,$1" \
    ;     IDCANCEL GiveUpDotNET
 
    ${If} $0 != "" ;let's remove the leading period on installed part if it exists
      StrCpy $0 $0 ${NSIS_MAX_STRLEN} 1
    ${EndIf}
    ${If} $1 != "" ;let's remove the leading period on required part if it exists
      StrCpy $1 $1 ${NSIS_MAX_STRLEN} 1
    ${EndIf}
 
    ;$2 has the installed part, $3 has the required part
    ${If} $2 S< $3
      IntOp $0 0 - 1 ;$0 = -1, installed less than required
      ${ExitDo}
    ${ElseIf} $2 S> $3
      IntOp $0 0 + 1 ;$0 = 1, installed greater than required
      ${ExitDo}
    ${ElseIf} $2 == ""
    ${AndIf} $3 == ""
      IntOp $0 0 + 0 ;$0 = 0, the versions are identical
      ${ExitDo}
    ${EndIf} ;otherwise we just keep looping through the parts
  ${Loop}
 
  ${If} $0 < 0
    DetailPrint ".NET Framework Version found: $6, but is older than the required version: $7"
    Goto OldDotNET
  ${Else}
    DetailPrint ".NET Framework Version found: $6, equal or newer to required version: $7."
    Goto NewDotNET
  ${EndIf}
 
NoDotNET:
  ${If} ${CheckOnly} == "True"
    MessageBox MB_OK|MB_ICONEXCLAMATION \
    ".NET Framework 3.0 or greater was not found.$\n$\nThe .NET Framework is required by this product.  Please download and install the .NET Framework to continue."
    Abort
  ${Else}
    MessageBox MB_YESNOCANCEL|MB_ICONEXCLAMATION \
    ".NET Framework 3.0 or greater was not found.$\n$\nRequired Version: $7 or greater.$\n$\nProceed with download and install of .NET Framework?" \
    /SD IDYES IDYES DownloadDotNET IDNO FailDotNET
    goto GiveUpDotNET ;IDCANCEL
  ${EndIf}
OldDotNET:
  ${If} ${CheckOnly} == "True"
    MessageBox MB_OK|MB_ICONEXCLAMATION \
    "A newer version of the .NET Framework is required by this product.$\n$\nYour .NET Framework version: $6.$\nRequired Version: $7 or greater.$\n$\nPlease download and install the required version or higher of the .NET Framework to continue."
    Abort
  ${Else}
    MessageBox MB_YESNOCANCEL|MB_ICONEXCLAMATION \
    "Your .NET Framework version: $6.$\nRequired Version: $7 or greater.$\n$\nProceed with download and install of .NET Framework?" \
    /SD IDYES IDYES DownloadDotNET IDNO FailDotNET
    goto GiveUpDotNET ;IDCANCEL
  ${EndIf}
DownloadDotNET:
  DetailPrint "Beginning download of latest .NET Framework version."
  NSISDL::download ${DOTNET_URL} "$TEMP\dotnetfx.exe"
  DetailPrint "Completed download."
  Pop $0
  ${If} $0 == "cancel"
    MessageBox MB_OK "Download cancelled."
    goto GiveUpDotNET
  ${ElseIf} $0 != "success"
    MessageBox MB_OK "Download failed."
    goto FailDotNET 
  ${EndIf}
  DetailPrint "Pausing installation while downloaded .NET Framework installer runs."
  ExecWait '$TEMP\dotnetfx.exe /norestart /c:"install /norestart"' $9
  ${If} $9 != 0
    goto FailDotNet
  ${EndIf}
  DetailPrint "Completed .NET Framework install/update. Removing .NET Framework installer."
  Delete "$TEMP\dotnetfx3setup.exe"
  DetailPrint ".NET Framework installer removed."
  goto NewDotNet
  
FailDotNET:
  MessageBox MB_OK "Install of .NET Framework was refused or not successful. Installation cannot continue and is aborting."
  Delete "$TEMP\dotnetfx3setup.exe"
  Abort "Install of .NET Framework was refused or not successful."
 
GiveUpDotNET:
  Delete "$TEMP\dotnetfx3setup.exe"
  Abort "Installation cancelled by user."
 
NewDotNET:
  DetailPrint "Proceeding with remainder of installation."
  Pop $0
  Pop $1
  Pop $2
  Pop $3
  Pop $4
  Pop $5
  Pop $6 ;backup of intsalled ver
  Pop $7 ;backup of DoNetReqVer
  Pop $8
  Pop $9
!macroend