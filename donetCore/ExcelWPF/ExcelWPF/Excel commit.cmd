@ECHO off
set docPath=%1%

if not exist %docPath% (
@ECHO  Ҫ�ύ��Ŀ¼"%docPath%"�����ڣ�
@pause
exit 9
) 

TortoiseProc.exe /command:commit /path:"%docPath%"  /closeonend:2
