curPath=$(cd `dirname $0`;pwd)
echo ${curPath}

dotnet ${curPath}/Deploy/IOSBuildServer.dll
