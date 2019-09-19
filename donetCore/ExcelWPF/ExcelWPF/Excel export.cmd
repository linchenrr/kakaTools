@ECHO off
set docPath=%1%
set url=%2%
set outputPath=%3%
set ILCodePath=HotILProject\Scripts\Excel
set customerEncoder=tools\ExcelEncoder\ExcelEncoder\bin\Release\netcoreapp2.2\ExcelEncoder.dll


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


@ECHO �������԰�����
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%\i18n"  -dataPath "%outputPath%\data\i18n" -pauseOnError false -ext bytes -dataRowStartNum 4 -exportDatajson false -mergeSheets true -writeCellLen false

if %errorlevel% NEQ 0 (
@ECHO errorlevel:%errorlevel%
rem @pause
exit %errorlevel%
)


@ECHO ���԰��������
@ECHO.
@ECHO.
@ECHO.


@ECHO.
@ECHO.
@ECHO ����excel���ݴ���
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
@ECHO ����excel���ݺʹ���(share mode)
@ECHO.
tools\ExcelExport\ExcelExport.exe -excel "%docPath%\shareMode" -shareMode true -pauseOnError false -dataPath "%outputPath%\data\ShareMode" -customerEncoder "%customerEncoder%" -ext bytes -defaultArraySpliter , -dataRowStartNum 5 -exportDatajson false -writeCellLenExclude "enum,bool,int,float,string,date,vector3" -exclude "i18n,Spawn"

if %errorlevel% NEQ 0 (
@ECHO errorlevel:%errorlevel%
rem @pause
exit %errorlevel%
)

@ECHO.
@ECHO ���
@ECHO.