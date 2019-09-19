@ECHO off
set docPath=%1%
set url=%2%
set outputPath=%3%


if exist %docPath% (
  rem echo "folder exist"
) else (
@ECHO "%docPath%" 目录不存在，创建目录并检出
@ECHO.
md %docPath%
TortoiseProc.exe /command:checkout /path:"%docPath%" /url:"%url%" /closeonend:2

if %errorlevel% NEQ 0 (
@ECHO errorlevel:%errorlevel%
@pause
exit %errorlevel%
)
)

@ECHO.
@ECHO.
@ECHO 生成excel数据代码
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%" -template "ExcelTemplate_csharp.xml" -pauseOnError false -codeGeneratePath "%outputPath%"  -dataRowStartNum 5 -exportDataBytes false -exportDatajson false -exclude "i18n,Spawn,shareMode"

if %errorlevel% NEQ 0 (
@ECHO errorlevel:%errorlevel%
rem @pause
exit %errorlevel%
)


@ECHO.
@ECHO.
@ECHO.
@ECHO 生成excel数据和代码(share mode)
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%\shareMode" -shareMode true -template "ExcelTemplate_csharpShareMode.xml" -pauseOnError false -codeGeneratePath "%outputPath%\ShareMode" -dataRowStartNum 5 -exportDataBytes false -exportDatajson false -exclude "i18n,Spawn"

if %errorlevel% NEQ 0 (
@ECHO errorlevel:%errorlevel%
rem @pause
exit %errorlevel%
)

@ECHO.
@ECHO 完成
@ECHO.