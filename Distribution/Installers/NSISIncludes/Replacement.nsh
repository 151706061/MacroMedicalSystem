!macro Replacement
; Function that will replace multiple patterns in file using callbacks
; taken from:
; http://nsis.sourceforge.net/Replace_multiple_patterns_in_file_using_callbacks
; Example Usage:
; Function ReplaceInSQL
;	; save R1
;	Push $R1
;	Exch
;	; A sequence of replacements.
;       ; the string to replace in is at the top of the stack
;	Push "@@foo@@" ; string to find
;	Push "bar" ; string to replace it with
;	Call StrReplace ; see elsewhere in NSIS Wiki
;       ; the string to replace in is at the top of the stack again
;	Push "@@tellytubby@@" ; string to find
;	Push "againagain" ; string to replace it with
;	Call StrReplace
;       ; and so on
;	; restore stack
;	Exch
;	Pop $R1
; FunctionEnd

Function ReplaceInFile
	Exch $R0 ;file name to search in
	Exch
	Exch $R4 ;callback function handle
	Push $R1 ;file handle
	Push $R2 ;temp file name
	Push $R3 ;temp file handle
	Push $R5 ;line read

	GetTempFileName $R2
  	FileOpen $R1 $R0 r ;file to search in
  	FileOpen $R3 $R2 w ;temp file

loop_read:
 	ClearErrors
 	FileRead $R1 $R5 ;read line
 	Push $R5 ; put line on stack
 	Call $R4
 	Pop $R5 ; read line from stack
 	IfErrors exit
 	FileWrite $R3 $R5 ;write modified line
	Goto loop_read
exit:
  	FileClose $R1
  	FileClose $R3

   	SetDetailsPrint none
  	Delete $R0
  	Rename $R2 $R0
  	Delete $R2
   	SetDetailsPrint both

	; pop in reverse order
	Pop $R5
	Pop $R3
	Pop $R2
	Pop $R1
	Pop $R4
	Pop $R0
FunctionEnd

; Taken from http://nsis.sourceforge.net/Another_String_Replace_%28and_Slash/BackSlash_Converter%29
Function StrRep
  Exch $R4 ; $R4 = Replacement String
  Exch
  Exch $R3 ; $R3 = String to replace (needle)
  Exch 2
  Exch $R1 ; $R1 = String to do replacement in (haystack)
  Push $R2 ; Replaced haystack
  Push $R5 ; Len (needle)
  Push $R6 ; len (haystack)
  Push $R7 ; Scratch reg
  StrCpy $R2 ""
  StrLen $R5 $R3
  StrLen $R6 $R1
loop:
  StrCpy $R7 $R1 $R5
  StrCmp $R7 $R3 found
  StrCpy $R7 $R1 1 ; - optimization can be removed if U know len needle=1
  StrCpy $R2 "$R2$R7"
  StrCpy $R1 $R1 $R6 1
  StrCmp $R1 "" done loop
found:
  StrCpy $R2 "$R2$R4"
  StrCpy $R1 $R1 $R6 $R5
  StrCmp $R1 "" done loop
done:
  StrCpy $R3 $R2
  Pop $R7
  Pop $R6
  Pop $R5
  Pop $R2
  Pop $R1
  Pop $R4
  Exch $R3
FunctionEnd
!macroend