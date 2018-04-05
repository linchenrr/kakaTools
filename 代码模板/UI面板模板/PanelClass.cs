using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using KLib;


/// <summary>
/// author      $username$
/// date        $time$$if$ ($username$ == linchen)
/// Email       superkaka.org@gmail.com$endif$
/// 代码模板v1.5
/// 注意事项：不要覆写Awake、Start、OnDestroy等函数，使用相应的替代函数。
/// </summary>

public class $safeitemname$ : PanelEx<object>//通过泛型指定Data属性的类型
{
    
    /**               组件定义                  */
    
    
    /**               变量定义                  */
    
    
    
    /// <summary>绑定完成，资源已经可以访问。替代Awake，在onStart之前被调用</summary>
    override protected void onBindComplete()
    {
        base.onBindComplete();
        
        
        
    }
    
    /// <summary>替代Start，初始化逻辑都写在这里</summary>
    override protected void onStart()
    {
        base.onStart();
        
        
        
    }
    
    /// <summary>根据新的Data属性更新显示</summary>
    override protected void onData()
    {
        base.onData();
        
        
        
    }
    
    /// <summary>界面被激活</summary>
    override protected void onActive()
    {
        base.onActive();
        
        
        
    }
    
    private void _ClickHandler(GameObject btn)
    {
        
        
        
    }
    
    /// <summary>界面休眠</summary>
    override protected void onSleep()
    {
        base.onSleep();
        
        
        
    }
    
    /// <summary>界面即将被销毁</summary>
    override protected void beforeDispose()
    {
        base.beforeDispose();



    }
    
    /// <summary>界面正在被销毁，替代OnDestroy</summary>
    override protected void onDispose()
    {
        base.onDispose();
        
        
        
    }
    
}
