@ECHO off
set docPath=%1%
set url=%2%
set outputPath=%3%
set ILCodePath=HotILProject\Scripts\Excel
set customerEncoder=tools\ExcelEncoder\ExcelEncoder\bin\Release\netcoreapp2.2\ExcelEncoder.dll


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


@ECHO 导出语言包数据
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%\i18n"  -dataPath "%outputPath%\data\i18n" -pauseOnError false -ext bytes -dataRowStartNum 4 -exportDatajson false -mergeSheets true -writeCellLen false

if %errorlevel% NEQ 0 (
@ECHO errorlevel:%errorlevel%
rem @pause
exit %errorlevel%
)


@ECHO 语言包导出完成
@ECHO.
@ECHO.
@ECHO.


@ECHO.
@ECHO.
@ECHO 生成excel数据代码
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%" -pauseOnError false -dataPath "%outputPath%\data" -customerEncoder "%customerEncoder%" -ext bytes -defaultArraySpliter , -dataRowStartNum 5 -exportDatajson false -writeCellLenExclude "enum,bool,int,float,string,date,vector3" -exclude "i18n,Spawn,shareMode"

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
tools\ExcelExport\ExcelExport.exe -excel "%docPath%\shareMode" -shareMode true -pauseOnError false -dataPath "%outputPath%\data\ShareMode" -customerEncoder "%customerEncoder%" -ext bytes -defaultArraySpliter , -dataRowStartNum 5 -exportDatajson false -writeCellLenExclude "enum,bool,int,float,string,date,vector3" -exclude "i18n,Spawn"

if %errorlevel% NEQ 0 (
@ECHO errorlevel:%errorlevel%
rem @pause
exit %errorlevel%
)

@ECHO.
@ECHO 完成
@ECHO.