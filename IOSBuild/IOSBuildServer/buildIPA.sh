  #!/bin/bash
#只需要在终端中输入 $ sh archive.sh  即可打包成ipa

curPath=$(cd `dirname $0`;pwd)

if [ x$1 != x ]
then
ipaPath=$1
else
ipaPath=${curPath}/build/Unity-iPhone.ipa
fi

echo ${curPath}
echo ${ipaPath}

cd ${curPath}
rm -f *.ipa

#build xcode文件夹路径
xcodeProj_path=${curPath}/xcodeProj

cd ${xcodeProj_path}
 
packaging(){

#***********配置项目
#工程名称(Project的名字)
MWProjectName=$1
#scheme名字 -可以点击Product->Scheme->Manager Schemes...查看
MWScheme=$2
#Release还是Debug
MWConfiguration=$3
#日期
MWDate=`date +%Y%m%d_%H%M`
#工程路径
MWWorkspace=$4
#build路径
MWBuildDir=$5
#plist文件名，默认放在工程文件路径的位置
MBPlistName=$6

#创建构建和输出的路径
mkdir -p $MWBuildDir


chmod +x   ${MWWorkspace}/MapFileParser.sh

#pod 相关配置

#更新pod配置
#pod install

echo `date +%H:%M` "编译xcode工程..."

#构建
xcodebuild archive \
-workspace  "${MWWorkspace}/${MWScheme}.xcodeproj/$MWProjectName.xcworkspace" \
-scheme "$MWScheme" \
-configuration "$MWConfiguration" \
-archivePath "$MWBuildDir/$MWProjectName" \
clean \
build \
#-derivedDataPath "${curPath}/tmp"

echo `date +%H:%M` "编译成功，导出ipa..."
#生成ipa
xcodebuild -exportArchive \
-archivePath "$MWBuildDir/$MWProjectName.xcarchive" \
-exportPath "$MWBuildDir" \
-exportOptionsPlist "$MBPlistName"

#open $MWBuildDir

}

echo "start build ipa"

#函数调用
# $1 工程名  $2 scheme名字  $3 Release还是Debug  $4 工程路径  $5 ipa文件输出路径 $6 plist文件名字
#packaging "XXX" "XXX"  "Release"  "/Users/maowo-001/Desktop/XXX" "/Users/maowo-001/Desktop/XXX/build" "adhocExportOptions.plist"

packaging "project" "Unity-iPhone" "Release"  "/Users/linchen/Desktop/xcodeProj/xcodeProj" ${ipaPath} "${curPath}/ExportOptions.plist"

echo "ipa生成完毕"
