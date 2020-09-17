curPath=$(cd `dirname $0`;pwd)

if [ x$1 != x ]
then
ipaPath=$1
else
ipaPath=${curPath}/Unity-iPhone.ipa
fi

echo ${curPath}
echo ${ipaPath}

cd ${curPath}
rm -f *.ipa

#build xcode文件夹路径
xcodeProj_path=${curPath}/xcodeProj

cd ${xcodeProj_path}

project_Name="Unity-iPhone"
build_debug="Release"
#清理
#xcodebuild clean -project ${project_Name}.xcodeproj -scheme ${project_Name} -configuration ${build_debug}
xcodebuild clean -workspace ${xcodeProj_path}/${project_Name}.xcodeproj/project.xcworkspace -scheme ${project_Name} -configuration ${build_debug}

#编译工程
#ARCHIVEPATH=${WORKSPACE_PATH}/output
#codeSignIdentity="iPhone Developer: He BoKai (JT8R738GA6)"
#codeSignIdentity="iPhone Developer"
#provisioningProfile="EMA PHX DEV"
#devel_Team="KHQZBTCGQ5"

#archive导出.xcarchive文件#
#xcodebuild archive -project ${project_Name}.xcodeproj -scheme ${project_Name} -archivePath ${ARCHIVEPATH} CODE_SIGN_IDENTITY="$codeSignIdentity" PROVISIONING_PROFILE="$provisioningProfile" DEVELOPMENT_TEAM="$devel_Team"

#sudo xcodebuild || exit
echo "build ipa..."
#打包#
xcrun -sdk iphoneos PackageApplication -v ${xcodeProj_path}/build/Release-iphoneos/*.app -o ${ipaPath}

echo "ipa生成完毕"

