echo off
set HOST_NAME=LOCALHOST

certmgr.exe -del -r CurrentUser -s TrustedPeople -c -n %HOST_NAME%
certmgr.exe -del -r LocalMachine -s TrustedPeople -c -n %HOST_NAME%
certmgr.exe -del -r LocalMachine -s My -c -n %HOST_NAME%
certmgr.exe -del -r CurrentUser -s My -c -n %HOST_NAME%
makecert.exe -sr LocalMachine -ss MY -a sha1 -n CN=%HOST_NAME% -sky exchange -pe
certmgr.exe -add -r LocalMachine -s My -c -n %HOST_NAME% -r Currentuser -s TrustedPeople
certmgr.exe -add -r LocalMachine -s My -c -n %HOST_NAME% -r LocalMachine -s TrustedPeople
certmgr.exe -add -r LocalMachine -s My -c -n %HOST_NAME% -r Currentuser -s My

pause