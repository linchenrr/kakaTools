dotnet bin\publish\IOSBuildClient.dll -requestURL "http://10.246.89.26:8001/build/BuildIPA" -outputURL "http://10.246.89.26:8001/build/BuildOutput" -shellPath "/Volumes/LocalCD/buildServer/buildIPA.sh" -ipaPath "/Volumes/LocalCD/buildServer/currentBuild.ipa" -writeLocalIpa "J:/other/output.ipa"

@pause