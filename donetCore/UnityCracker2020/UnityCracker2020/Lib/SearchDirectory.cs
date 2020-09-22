using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Unity3dPacth.Lib
{
    /// <summary>
    /// 搜索Unity及Hub目录，失败返回MyComputer目录
    /// </summary>
    public class SearchDirectory
    {
        private static string hubAppData;
        private static string HubAppData => hubAppData ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UnityHub");
        private static bool IsHubAppDataExists => Directory.Exists(HubAppData);

        private static string hubInfoFile;
        private static string HubInfoFile => hubInfoFile ??= IsHubAppDataExists ? Path.Combine(HubAppData, "hubInfo.json") : string.Empty;
        private static bool IsHubInfoFileExists => !string.IsNullOrEmpty(HubInfoFile) && File.Exists(HubInfoFile);


        private static string installPathFile;
        private static string InstallPathFile => installPathFile ??= IsHubAppDataExists ? Path.Combine(HubAppData, "secondaryInstallPath.json") : string.Empty;
        private static bool IsInstallPathFileExists => !string.IsNullOrEmpty(HubInfoFile) && File.Exists(HubInfoFile);


        private static string myComputer;
        public static string MyComputer => myComputer ??= Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
        struct ExePath { public string executablePath; }
        private static string hubPath;
        public static string HubResourcesPath
        {
            get
            {
                if (!string.IsNullOrEmpty(hubPath))
                    return hubPath;
                if (!IsHubInfoFileExists)
                    return hubPath = MyComputer;
                hubPath = JsonConvert.DeserializeObject<ExePath>(File.ReadAllText(HubInfoFile)).executablePath;
                if (!File.Exists(hubPath))
                    return hubPath = MyComputer;
                hubPath = Path.Combine(Path.GetDirectoryName(hubPath), "resources");
                if(!Directory.Exists(hubPath))
                    return hubPath = MyComputer;
                return hubPath;
            }
        }
        private static string unityInstallPath;
        public static string UnityInstallPath
        {
            get 
            {
                if (!string.IsNullOrEmpty(unityInstallPath))
                    return unityInstallPath;
                if (!IsInstallPathFileExists)
                    return unityInstallPath = MyComputer;
                unityInstallPath = JsonConvert.DeserializeObject<string>(File.ReadAllText(InstallPathFile));
                if (!Directory.Exists(unityInstallPath))
                    return unityInstallPath = MyComputer;
                return unityInstallPath;
            }

        }
    }
}
