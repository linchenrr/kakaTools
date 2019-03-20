@ECHO off
set docPath=doc
set outputPath=Assets\bundles\excel
set ILCodePath=HotILProject\Scripts\Excel
set customerEncoder=tools\ExcelEncoder\ExcelEncoder\bin\Release\netcoreapp2.2\ExcelEncoder.dll

set url=svn://10.246.88.34/Phx/Excel


if exist %docPath% (
  rem echo "folder exist"
) else (
@ECHO "%docPath%" 目录不存在，创建目录并检出
@ECHO.
md %docPath%
TortoiseProc.exe /command:checkout /path:"%docPath%" /url:"%url%" /closeonend:2
)



@ECHO 导出语言包数据
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%\i18n"  -dataPath "%outputPath%\data\i18n" -ext bytes -dataRowStartNum 4 -exportDatajson false -mergeSheets true -writeCellLen false

@ECHO 语言包导出完成
@ECHO.
@ECHO.
@ECHO.

@ECHO 生成lua excel数据和代码
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%" -template "ExcelTemplate_lua.xml" -codeGeneratePath "%outputPath%\code" -dataPath "%outputPath%\data" -defaultArraySpliter , -dataRowStartNum 5 -addLogo true -exportDataBytes false -exportDatajson true -exclude "i18n,Spawn,shareMode"

@ECHO.
@ECHO lua 数据和代码导出完成
@ECHO.
@ECHO.


@ECHO.
@ECHO.
@ECHO 生成excel数据代码
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%" -template "ExcelTemplate_csharp.xml" -codeGeneratePath "%outputPath%\code" -dataPath "%outputPath%\data" -customerEncoder "%customerEncoder%" -ext bytes -defaultArraySpliter , -dataRowStartNum 5 -exportDatajson false -writeCellLenExclude "enum,bool,int,float,string,date,vector3" -exclude "i18n,Spawn,shareMode"

@ECHO.
@ECHO.
@ECHO.
@ECHO 生成excel数据和代码(share mode)
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%\shareMode" -shareMode true -template "ExcelTemplate_csharpShareMode.xml" -codeGeneratePath "%outputPath%\code\ShareMode" -dataPath "%outputPath%\data\ShareMode" -customerEncoder "%customerEncoder%" -ext bytes -defaultArraySpliter , -dataRowStartNum 5 -exportDatajson false -writeCellLenExclude "enum,bool,int,float,string,date,vector3" -exclude "i18n,Spawn"

@ECHO.
@ECHO 完成
@ECHO.

@pause