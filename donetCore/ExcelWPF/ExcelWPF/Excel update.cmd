@ECHO off
set docPath=%1%
set url=%2%

if exist %docPath% (
TortoiseProc.exe /command:update /path:"%docPath%" /url:"%url%" /closeonend:2
) else (
@ECHO "%docPath%" Ŀ¼�����ڣ�����Ŀ¼�����
@ECHO.
md %docPath%
TortoiseProc.exe /command:checkout /path:"%docPath%" /url:"%url%" /closeonend:2
)