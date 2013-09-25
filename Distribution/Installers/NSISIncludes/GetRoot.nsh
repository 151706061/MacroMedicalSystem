# Taken from http://nsis.sourceforge.net/Get_the_Root_Directory

!macro GetRoot

Function GetRoot
  Exch $0
  Push $1
  Push $2
  Push $3
  Push $4
 
  StrCpy $1 $0 2
  StrCmp $1 "\\" UNC
    StrCpy $0 $1
    Goto done
 
UNC:
  StrCpy $2 3
  StrLen $3 $0
  loop:
    IntCmp $2 $3 "" "" loopend
    StrCpy $1 $0 1 $2
    IntOp $2 $2 + 1
    StrCmp $1 "\" loopend loop
  loopend:
    StrCmp $4 "1" +3
      StrCpy $4 1
      Goto loop
    IntOp $2 $2 - 1
    StrCpy $0 $0 $2
 
done:
  Pop $4
  Pop $3
  Pop $2
  Pop $1
  Exch $0
FunctionEnd

!macroend