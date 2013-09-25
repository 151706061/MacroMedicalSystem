@ECHO OFF

ECHO Importing Enumeration.xml
%1 Import /class:Enumeration Enumeration.xml >> ImportData.log

ECHO Importing Facility.xml
%1 Import /class:Facility Facility.xml >> ImportData.log

ECHO Importing Location.xml
%1 Import /class:Location Location.xml >> ImportData.log

ECHO Importing Modality.xml
%1 Import /class:Modality Modality.xml >> ImportData.log

ECHO Importing Department.xml
%1 Import /class:Department Department.xml >> ImportData.log


ECHO Importing ProcedureTypes.xml
%1 Import /class:ProcedureType ProcedureTypes.xml >> ImportData.log

ECHO Importing DiagnosticServices.xml
%1 Import /class:DiagnosticService DiagnosticServices.xml >> ImportData.log

ECHO Importing ProcedureTypeGroups.xml
%1 Import /class:ProcedureTypeGroup ProcedureTypeGroups.xml >> ImportData.log



pause
goto :eof