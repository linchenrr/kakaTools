﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ExcelExport.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ExcelExport.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性
        ///   重写当前线程的 CurrentUICulture 属性。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 excelExport
        ///Usage:
        ///excelExport.exe -excel &quot;D:\work\VS\mycsharp\excelExport\excelExport\sample\sample.xlsx&quot; -template &quot;D:\work\VS\mycsharp\excelExport\excelExport\sample\template_csharp.xml&quot; -codeGeneratePath &quot;J:\code&quot; -dataPath &quot;J:&quot;
        ///
        ///-excel			excel文件路径或目录，可以支持多个路径，用&quot;,&quot;分隔
        ///
        ///-template		代码模板文件路径
        ///
        ///-customerEncoder		自定义数据编码器路径
        ///
        ///-codeGeneratePath	代码输出文件夹路径
        ///
        ///可选：
        ///[-dataPath]		excel数据二进制文件输出路径，不指定则输出到源excel同目录下
        ///
        ///[-prefix_primaryKey]	主键字段的前缀 (默认[$])，没有指定主键时将第一个字段作为主键
        ///
        ///[-prefix_IgnoreSheet]	忽略的表名前缀 (默认 [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string usage {
            get {
                return ResourceManager.GetString("usage", resourceCulture);
            }
        }
    }
}
