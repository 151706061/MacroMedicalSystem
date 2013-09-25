!ifndef __CLEARCANVAS_VSTEMPLATES
    !define __CLEARCANVAS_VSTEMPLATES

    #########################################################################################
    # Enumerates available project template paths for Visual Studio
    #########################################################################################
    # Enumerated paths are placed on the stack using an empty string as a terminator.
    # The macro uses the $R9 register as a temporary variable, so store it if you care about
    # its contents.
    # This version handles Visual Studio and VC# Express versions 2005 through 2010 Beta 2.
    #########################################################################################
    # Usage Example:
    #  Push $R9                                 ; save contents of $R9
    #  !insertmacro CC_ENUM_VCS_PROJTEMPLATES   ; call macro
    #  Pop $0                                   ; pop stack
    #  StrCmp $0 "" +3                          ; if empty, end
    #  CopyFiles "myProjectTemplates\*.*" "$0"  ; copy project templates over
    #  Goto -3                                  ; go back to pop the next item
    #  Pop $R9                                  ; restore contents of $R9
    #########################################################################################
    !macro CC_ENUM_VCS_PROJTEMPLATES
        Push "" ; this marks the end of the enumeration

        ; try to find any registry-specified project template paths, expand them for environment variables, then push onto the stack
        ReadRegStr $R9 HKCU "Software\Microsoft\VisualStudio\8.0\" "UserProjectTemplatesLocation"
        StrCmp $R9 "" +3
        ExpandEnvStrings $R9 $R9
        Push $R9
        ReadRegStr $R9 HKCU "Software\Microsoft\VCSExpress\8.0\" "UserProjectTemplatesLocation"
        StrCmp $R9 "" +3
        ExpandEnvStrings $R9 $R9
        Push $R9
        ReadRegStr $R9 HKCU "Software\Microsoft\VisualStudio\9.0\" "UserProjectTemplatesLocation"
        StrCmp $R9 "" +3
        ExpandEnvStrings $R9 $R9
        Push $R9
        ReadRegStr $R9 HKCU "Software\Microsoft\VCSExpress\9.0\" "UserProjectTemplatesLocation"
        StrCmp $R9 "" +3
        ExpandEnvStrings $R9 $R9
        Push $R9
        ReadRegStr $R9 HKCU "Software\Microsoft\VisualStudio\10.0\" "UserProjectTemplatesLocation"
        StrCmp $R9 "" +3
        ExpandEnvStrings $R9 $R9
        Push $R9
        ReadRegStr $R9 HKCU "Software\Microsoft\VCSExpress\10.0\" "UserProjectTemplatesLocation"
        StrCmp $R9 "" +3
        ExpandEnvStrings $R9 $R9
        Push $R9

        ; peek at the top of the stack
        Pop $R9
        Push $R9

        ; if the stack is still empty (i.e. the stack top is our terminator), try a bunch of standard paths
        StrCmp $R9 "" 0 +13 ; the 2nd jump value should to the Nop below (can't use labels here)
        ReadRegStr $R9 HKCU "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" "Personal"
        StrCpy $R9 "$R9\Visual Studio 2005\Templates\ProjectTemplates"
        IfFileExists $R9 0 +2
        Push $R9
        ReadRegStr $R9 HKCU "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" "Personal"
        StrCpy $R9 "$R9\Visual Studio 2008\Templates\ProjectTemplates"
        IfFileExists $R9 0 +2
        Push $R9
        ReadRegStr $R9 HKCU "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" "Personal"
        StrCpy $R9 "$R9\Visual Studio 2010\Templates\ProjectTemplates"
        IfFileExists $R9 0 +2
        Push $R9
        Nop
    !macroend

    #########################################################################################
    # Enumerates available item template paths for Visual Studio
    #########################################################################################
    # Enumerated paths are placed on the stack using an empty string as a terminator.
    # The macro uses the $R9 register as a temporary variable, so store it if you care about
    # its contents.
    # This version handles Visual Studio and VC# Express versions 2005 through 2010 Beta 2.
    #########################################################################################
    # Usage Example:
    #  Push $R9                                 ; save contents of $R9
    #  !insertmacro CC_ENUM_VCS_ITEMTEMPLATES   ; call macro
    #  Pop $0                                   ; pop stack
    #  StrCmp $0 "" +3                          ; if empty, end
    #  CopyFiles "myItemTemplates\*.*" "$0"     ; copy project templates over
    #  Goto -3                                  ; go back to pop the next item
    #  Pop $R9                                  ; restore contents of $R9
    #########################################################################################
    !macro CC_ENUM_VCS_ITEMTEMPLATES
        Push "" ; this marks the end of the enumeration

        ; try to find any registry-specified item template paths, expand them for environment variables, then push onto the stack
        ReadRegStr $R9 HKCU "Software\Microsoft\VisualStudio\8.0\" "UserItemTemplatesLocation"
        StrCmp $R9 "" +3
        ExpandEnvStrings $R9 $R9
        Push $R9
        ReadRegStr $R9 HKCU "Software\Microsoft\VCSExpress\8.0\" "UserItemTemplatesLocation"
        StrCmp $R9 "" +3
        ExpandEnvStrings $R9 $R9
        Push $R9
        ReadRegStr $R9 HKCU "Software\Microsoft\VisualStudio\9.0\" "UserItemTemplatesLocation"
        StrCmp $R9 "" +3
        ExpandEnvStrings $R9 $R9
        Push $R9
        ReadRegStr $R9 HKCU "Software\Microsoft\VCSExpress\9.0\" "UserItemTemplatesLocation"
        StrCmp $R9 "" +3
        ExpandEnvStrings $R9 $R9
        Push $R9
        ReadRegStr $R9 HKCU "Software\Microsoft\VisualStudio\10.0\" "UserItemTemplatesLocation"
        StrCmp $R9 "" +3
        ExpandEnvStrings $R9 $R9
        Push $R9
        ReadRegStr $R9 HKCU "Software\Microsoft\VCSExpress\10.0\" "UserItemTemplatesLocation"
        StrCmp $R9 "" +3
        ExpandEnvStrings $R9 $R9
        Push $R9

        ; peek at the top of the stack
        Pop $R9
        Push $R9

        ; if the stack is still empty (i.e. the stack top is our terminator), try a bunch of standard paths
        StrCmp $R9 "" 0 +13 ; the 2nd jump value should to the Nop below (can't use labels here)
        ReadRegStr $R9 HKCU "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" "Personal"
        StrCpy $R9 "$R9\Visual Studio 2005\Templates\ItemTemplates"
        IfFileExists $R9 0 +2
        Push $R9
        ReadRegStr $R9 HKCU "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" "Personal"
        StrCpy $R9 "$R9\Visual Studio 2008\Templates\ItemTemplates"
        IfFileExists $R9 0 +2
        Push $R9
        ReadRegStr $R9 HKCU "SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" "Personal"
        StrCpy $R9 "$R9\Visual Studio 2010\Templates\ItemTemplates"
        IfFileExists $R9 0 +2
        Push $R9
        Nop
    !macroend
!endif