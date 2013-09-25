; Script generated by the HM NIS Edit Script Wizard.

; defines required for upgrade process
; the '1 ##VERSION string is used by the build script as a token
!define MAJOR_VERSION 1 ##MAJOR
!define MINOR_VERSION 1 ##MINOR
!define BUILD 1 ##BUILDNUMBER
!define REVISION 1 ##REVISIONNUMBER

; define for build process
; the "Ris" ##VERSION string is used by the build script as a token
!define BUILDER "Ris" ##BUILDER

; HM NIS Edit Wizard helper defines
!define PRODUCT_NAME "ClearCanvas ${BUILDER} Sample Data"
!define PRODUCT_VERSION "${MAJOR_VERSION}.${MINOR_VERSION}"
!define PRODUCT_PUBLISHER "ClearCanvas Inc"
!define PRODUCT_WEB_SITE "http://www.clearcanvas.ca"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

; MUI 1.67 compatible ------
!include "MUI.nsh"
!include "DumpLog.nsh"

; MUI Settings
!define MUI_ABORTWARNING
!define MUI_ICON "${NSISDIR}\Contrib\Graphics\Icons\modern-install.ico"
!define MUI_UNICON "${NSISDIR}\Contrib\Graphics\Icons\modern-uninstall.ico"

; Platform-specific Settings (x86 vs. x64)
!ifdef PLATFORM_X64
!define PLATFORM_NAME "x64"
!define PLATFORM_PROGRAMFILES $PROGRAMFILES64
!define PLATFORM_SUBFOLDER "x64"
!define PLATFORM_VC2008 "{350AA351-21FA-3270-8B7A-835434E766AD}"
!define PLATFORM_VC2008SP1 "{8220EEFE-38CD-377E-8595-13398D740ACE}"
!else
!define PLATFORM_X64 "false"
!define PLATFORM_NAME "x86"
!define PLATFORM_PROGRAMFILES $PROGRAMFILES
!define PLATFORM_SUBFOLDER ""
!define PLATFORM_VC2008 "{FF66E9F6-83E7-3A3E-AF14-8DE9A809A6A4}"
!define PLATFORM_VC2008SP1 "{9A25302D-30C0-39D9-BD6F-21E6EC160475}"
!endif

; Welcome page
!insertmacro MUI_PAGE_WELCOME
; License page
!insertmacro MUI_PAGE_LICENSE "..\..\..\..\License.rtf"
; Directory page
#!insertmacro MUI_PAGE_DIRECTORY
; Custom page - asks whether to import sample data
Page custom PageSampleDataImport ValidatePageSampleDataImport
; Instfiles page
!insertmacro MUI_PAGE_INSTFILES
; Finish page
!insertmacro MUI_PAGE_FINISH

; Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

; Language files
!insertmacro MUI_LANGUAGE "English"

; Reserve files
ReserveFile "PageSampleDataImport.ini"
!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS
; MUI end ------

; Variables
; Collected from PageSampleDataImport.ini
Var INI_INSTDIR

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "${PRODUCT_NAME}.exe"
InstallDir "${PLATFORM_PROGRAMFILES}\ClearCanvas\ClearCanvas Ris Server\SampleData"
ShowInstDetails show
ShowUnInstDetails show

SectionGroup "Database Components" GRP03
  Section "SampleDataDir"
    SetOutPath "$INSTDIR"
    File /r /x *.svn* "..\..\..\..\Ris\sampledata\${BUILDER}\*.*"
  SectionEnd

  Section "ImportSampleData"
    DetailPrint "Importing Sample Data..."
    nsExec::ExecToLog '"$INSTDIR\ImportSampleData.bat" ..\ClearCanvas.Ris.Server.Executable.exe'
  SectionEnd
SectionGroupEnd

Section -AdditionalIcons
  SetShellVarContext all
  CreateDirectory "$SMPROGRAMS\ClearCanvas\${PRODUCT_NAME}"
  CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"

  StrCpy $0 "$INSTDIR\install.log"
  Push $0
  Call DumpLog
SectionEnd

LangString PageSampleDataImport_TITLE ${LANG_ENGLISH} "Ris Sample Data Import page"
LangString PageSampleDataImport_SUBTITLE ${LANG_ENGLISH} "This page is used to select where to install the sample data set."

Function .onInit
  !insertmacro MUI_INSTALLOPTIONS_EXTRACT "PageSampleDataImport.ini"
FunctionEnd

Function PageSampleDataImport
  !insertmacro MUI_HEADER_TEXT "$(PageSampleDataImport_TITLE)" "$(PageSampleDataImport_SUBTITLE)"
  !insertmacro MUI_INSTALLOPTIONS_WRITE "PageSampleDataImport.ini" "Field 3" "State" "${PLATFORM_PROGRAMFILES}\ClearCanvas\ClearCanvas Ris Server"
  !insertmacro MUI_INSTALLOPTIONS_DISPLAY "PageSampleDataImport.ini"
FunctionEnd

Function ValidatePageSampleDataImport
  !insertmacro MUI_INSTALLOPTIONS_READ $INI_INSTDIR "PageSampleDataImport.ini" "Field 3" "State"
  StrCmp $INI_INSTDIR "" "" +3
    MessageBox MB_ICONEXCLAMATION|MB_OK "Directory cannot be left blank."
    Abort
    
  IfFileExists "$INI_INSTDIR\ClearCanvas.Ris.Server.Executable.exe" installDirFound
    MessageBox MB_ICONEXCLAMATION|MB_OK "Directory specified does not contain ClearCanvas.Ris.Server.Executable.exe.  Please browse to the directory that does."
    Abort
  
  installDirFound:
  StrCpy $INSTDIR $INI_INSTDIR\SampleData
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  SetShellVarContext all

  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"

  Delete "$SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk"

  RMDir "$SMPROGRAMS\${PRODUCT_NAME}"

  RMDir /r "$INSTDIR"

  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  SetAutoClose true
  
  StrCpy $0 "$INSTDIR\uninstall.log"
  Push $0
  Call un.DumpLog
SectionEnd