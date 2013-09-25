!macro TestDbConnect
# Tests connection to the database for all ClearCanvas products.
# Usage after calling TestDbConnect, Pop $0
# $0 is "error" on script engine failure
# $0 is 0 for failed connection
# $0 is 1 for success

  Function TestDbConnect
    Push $INI_HOSTINSTANCE
    Push "\"
    Push "\\"
    Call StrRep
    Pop "$R0"
    StrCpy $INI_2SLASHHOSTINSTANCE $R0

    FileOpen $0 "$TEMP\dbconnect.js" w
    FileWrite $0 "var connection = WScript.CreateObject($\"ADODB.connection$\");$\n"
    FileWrite $0 "connection.Provider = $\"sqloledb$\";$\n"
    FileWrite $0 "connection.Properties($\"Data Source$\").Value = $\"$INI_2SLASHHOSTINSTANCE$\";$\n"
    FileWrite $0 "connection.Properties($\"Initial Catalog$\").Value = $\"master$\";$\n"
    FileWrite $0 "try$\n"
    FileWrite $0 "{$\n"
    FileWrite $0 "connection.Open($\"$\", $\"$INI_ADMINID$\", $\"$INI_ADMINPASS$\");$\n"
    FileWrite $0 "}$\n"
    FileWrite $0 "catch(err)$\n"
    FileWrite $0 "{$\n"
    FileWrite $0 "WScript.Quit(1);$\n"
    FileWrite $0 "}$\n"
    FileWrite $0 "WScript.Quit(0);$\n"

    FileClose $0

    nsExec::ExecToStack '"cscript" "$TEMP\dbconnect.js"'
    Delete "$TEMP\dbconnect.js"
  FunctionEnd
!macroEnd