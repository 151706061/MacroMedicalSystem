# macro taken from http://nsis.sourceforge.net/Check_if_the_current_user_is_an_Administrator
;
; Usage:
; !insertmacro IsUserAdmin $0
; $0 = 1 if the user belongs to the administrator's group
; $0 = 0 if not
; $0 = -1 if there was an error (only for the 1st Macro)

!macro IsUserAdmin RESULT
 !define Index "Line${__LINE__}"
   StrCpy ${RESULT} 0
   System::Call '*(&i1 0,&i4 0,&i1 5)i.r0'
   System::Call 'advapi32::AllocateAndInitializeSid(i r0,i 2,i 32,i 544,i 0,i 0,i 0,i 0,i 0, \
   i 0,*i .R0)i.r5'
   System::Free $0
   System::Call 'advapi32::CheckTokenMembership(i n,i R0,*i .R1)i.r5'
   StrCmp $5 0 ${Index}_Error
   StrCpy ${RESULT} $R1
   Goto ${Index}_End
 ${Index}_Error:
   StrCpy ${RESULT} -1
 ${Index}_End:
   System::Call 'advapi32::FreeSid(i R0)i.r5'
 !undef Index
!macroend