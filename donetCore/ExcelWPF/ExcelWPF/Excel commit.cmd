@ECHO off
set docPath=%1%

if not exist %docPath% (
@ECHO  要提交的目录"%docPath%"不存在！
@pause
exit 9
) 

TortoiseProc.exe /command:commit /path:"%docPath%"  /closeonend:2
