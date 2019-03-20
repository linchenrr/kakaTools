@ECHO off
set docPath=doc
set outputPath=serverJson
set url=svn://10.246.88.34/Phx/Json

if exist %outputPath% (
TortoiseProc.exe /command:update /path:"%outputPath%" /url:"%url%" /closeonend:2
) else (
@ECHO "%outputPath%" Ŀ¼�����ڣ�����Ŀ¼�����
@ECHO.
md %outputPath%
TortoiseProc.exe /command:checkout /path:"%outputPath%" /url:"%url%" /closeonend:2
)

@ECHO ���� excel����json
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%" -template "ExcelTemplate_lua.xml" -codeGeneratePath "%outputPath%\code" -dataPath "%outputPath%" -defaultArraySpliter , -typeRowNum 4 -dataRowStartNum 5 -addLogo true -exportDataBytes false -exportDatajson true -exclude "i18n,Spawn,shareMode"

@ECHO ���� Spawn����json
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%\scene\Spawn" -template "ExcelTemplate_lua.xml" -codeGeneratePath "%outputPath%\code" -dataPath "%outputPath%\Spawn" -defaultArraySpliter , -typeRowNum 4 -dataRowStartNum 5 -addLogo true -exportDataBytes false -exportDatajson true

@ECHO.
@ECHO json������
@ECHO.

TortoiseProc.exe /command:commit /path:"%outputPath%" /closeonend:2

rem set /p choice=�Ƿ���Ҫ�ύ��svn��(Y/N)
rem if %choice%==y (TortoiseProc.exe /command:commit /path:"%outputPath%" /closeonend:2)
rem if %choice%==Y (TortoiseProc.exe /command:commit /path:"%outputPath%" /closeonend:2)

@pause