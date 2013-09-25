#Usage:
; Push "database name"
; Push "userID"
; Push "userPass"
; Push "hostname"
; Call GetDbVersion
; Pop $0  ### returns version number

#Behaviour:
; queries database provided for contents of table DatabaseVersion_
; returns string in the form [Major_] + '.' + [Minor_] + '.' + [Build_] + '.' + [Revision_]

###Variables that must be defined
; none

Function GetDbVersion
  Pop $R0   ### gets hostname name
  Pop $R1   ### gets userPass name
  Pop $R2   ### gets userID name
  Pop $R3   ### gets database name
  
  StrCmp "" "$R0" badData +1
  ;StrCmp "" "$R1" badData +1
  StrCmp "" "$R2" badData +1
  StrCmp "" "$R3" badData proceed
  
badData:
    Push ""
    Return

proceed:
  Push $R0
  Push "\"
  Push "\\"
  Call StrRep
  Pop "$R0"

  Push $TEMP
  Push "\"
  Push "\\"
  Call StrRep
  Pop "$R4"

  FileOpen $0 "$TEMP\getdbversion.js" w
  FileWrite $0 "var result = new ActiveXObject($\"ADODB.Recordset$\");$\n"
  FileWrite $0 "var connection = WScript.CreateObject($\"ADODB.connection$\");$\n"
  FileWrite $0 "connection.ConnectionString = $\"PROVIDER=SQLOLEDB.1;USER ID=$R2;PASSWORD=$R1;INITIAL CATALOG=$R3;DATA SOURCE=$R0$\"$\n"
  FileWrite $0 "connection.Open();$\n"
  FileWrite $0 "result = connection.execute($\"select [Major_] + '.' + [Minor_] + '.' + [Build_] + '.' + [Revision_] as [Version] from dbo.DatabaseVersion_$\");$\n"
  FileWrite $0 "var fso, tf;$\n"
  FileWrite $0 "fso = new ActiveXObject($\"Scripting.FileSystemObject$\");$\n"
  FileWrite $0 "tf = fso.CreateTextFile($\"$R4\\version.txt$\", true);$\n"
  FileWrite $0 "tf.Write(result($\"Version$\"));$\n"
  FileWrite $0 "tf.Close();$\n"
  FileWrite $0 "WScript.Quit(0);$\n"
  FileClose $0

  nsExec::ExecToStack '"cscript" "$TEMP\getdbversion.js"'
  Delete "$TEMP\getdbversion.js"

  FileOpen $1 "$TEMP\version.txt" r
  FileRead $1 $2
  FileClose $1

  Push $2

  Delete "$TEMP\version.txt"
FunctionEnd