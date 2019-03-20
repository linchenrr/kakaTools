@ECHO off
set docPath=doc
set outputPath=serverJson
set url=svn://10.246.88.34/Phx/Json

if exist %outputPath% (
TortoiseProc.exe /command:update /path:"%outputPath%" /url:"%url%" /closeonend:2
) else (
@ECHO "%outputPath%" 目录不存在，创建目录并检出
@ECHO.
md %outputPath%
TortoiseProc.exe /command:checkout /path:"%outputPath%" /url:"%url%" /closeonend:2
)

@ECHO 生成 excel数据json
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%" -template "ExcelTemplate_lua.xml" -codeGeneratePath "%outputPath%\code" -dataPath "%outputPath%" -defaultArraySpliter , -typeRowNum 4 -dataRowStartNum 5 -addLogo true -exportDataBytes false -exportDatajson true -exclude "i18n,Spawn,shareMode"

@ECHO 生成 Spawn数据json
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%\scene\Spawn" -template "ExcelTemplate_lua.xml" -codeGeneratePath "%outputPath%\code" -dataPath "%outputPath%\Spawn" -defaultArraySpliter , -typeRowNum 4 -dataRowStartNum 5 -addLogo true -exportDataBytes false -exportDatajson true

@ECHO.
@ECHO json已生成
@ECHO.

TortoiseProc.exe /command:commit /path:"%outputPath%" /closeonend:2

rem set /p choice=是否需要提交至svn？(Y/N)
rem if %choice%==y (TortoiseProc.exe /command:commit /path:"%outputPath%" /closeonend:2)
rem if %choice%==Y (TortoiseProc.exe /command:commit /path:"%outputPath%" /closeonend:2)

@pause