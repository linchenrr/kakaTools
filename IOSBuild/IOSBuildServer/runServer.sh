curPath=$(cd `dirname $0`;pwd)
echo ${curPath}

dotnet ${curPath}/publish/IOSBuildServer.dll
