@ECHO off
set docPath=doc
set url=svn://10.246.88.34/Phx/Excel

if exist %docPath% (
TortoiseProc.exe /command:update /path:"%docPath%" /url:"%url%" /closeonend:0
) else (
@ECHO "%docPath%" Ŀ¼�����ڣ�����Ŀ¼�����
@ECHO.
md %docPath%
TortoiseProc.exe /command:checkout /path:"%docPath%" /url:"%url%" /closeonend:2
)


@ECHO.
@ECHO ���
@ECHO.

@pause