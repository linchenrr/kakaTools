@ECHO off
set docPath=%1%
set url=%2%
set outputPath=%3%

if exist %docPath% (
  rem echo "folder exist"
) else (
@ECHO "%docPath%" Ŀ¼�����ڣ�����Ŀ¼�����
@ECHO.
md %docPath%
TortoiseProc.exe /command:checkout /path:"%docPath%" /url:"%url%" /closeonend:2

if %errorlevel% NEQ 0 (
@ECHO errorlevel:%errorlevel%
@pause
exit %errorlevel%
)
)

@ECHO ���� excel����json
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%" -template "ExcelTemplate_lua.xml" -codeGeneratePath "%outputPath%\code" -dataPath "%outputPath%" -defaultArraySpliter , -typeRowNum 4 -dataRowStartNum 5 -addLogo true -exportDataBytes false -exportDatajson true -exclude "i18n,Spawn,shareMode"

if %errorlevel% NEQ 0 (
@ECHO errorlevel:%errorlevel%
rem @pause
exit %errorlevel%
)

@ECHO ���� Spawn����json
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%\scene\Spawn" -template "ExcelTemplate_lua.xml" -codeGeneratePath "%outputPath%\code" -dataPath "%outputPath%\Spawn" -defaultArraySpliter , -typeRowNum 4 -dataRowStartNum 5 -addLogo true -exportDataBytes false -exportDatajson true

if %errorlevel% NEQ 0 (
@ECHO errorlevel:%errorlevel%
rem @pause
exit %errorlevel%
)

@ECHO.
@ECHO json������
@ECHO.