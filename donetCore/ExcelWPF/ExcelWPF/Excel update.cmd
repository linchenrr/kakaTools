@ECHO off
set docPath=%1%
set url=%2%

if exist %docPath% (
TortoiseProc.exe /command:update /path:"%docPath%" /url:"%url%" /closeonend:2
) else (
@ECHO "%docPath%" 目录不存在，创建目录并检出
@ECHO.
md %docPath%
TortoiseProc.exe /command:checkout /path:"%docPath%" /url:"%url%" /closeonend:2
)