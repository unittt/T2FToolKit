using System;
using System.Collections.Generic;

namespace T2FToolKit
{
    public sealed partial class T2FToolKit : IModuleManager
    {
        public static T2FToolKit Current { get; private set; }
        
        public int Priority { get; }

        private readonly List<IModuleManager> _internalModulesList = new List<IModuleManager>();
        
        #region Module
        public static AdManager Ad { get; private set; }
        public static WebViewManager WebView { get; private set; }
        #endregion
        
        
        public void OnAwake()
        {
            if (Current == null)
            {
                Current = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                throw new Exception("框架致命错误：不能存在两个及以上T2FToolKit！");
            }
            
            var modules = transform.GetComponentsInChildren<IModuleManager>(true);
            foreach (var module in modules)
            {
                if (module != this)
                {
                    _internalModulesList.Add(module);
                }
            }
            _internalModulesList.Sort((a, b) => a.Priority.CompareTo(b.Priority));

            foreach (var module in _internalModulesList)
            {
                module.OnAwake();
            }
            
            Ad = GetInternalModule<AdManager>();
            WebView = GetInternalModule<WebViewManager>();
        }

        #region 生命周期
        public void OnStart()
        {
            foreach (var module in _internalModulesList)
            {
                module.OnStart();
            }
        }

        public void OnUpdate()
        {
            foreach (var module in _internalModulesList)
            {
                module.OnUpdate();
            }
        }

        public void OnTerminate()
        {
            foreach (var module in _internalModulesList)
            {
                module.OnTerminate();
            }
            _internalModulesList.Clear();
        }
        #endregion
        
        /// <summary>
        /// 获取内置模块
        /// </summary>
        /// <param name="moduleName">内置模块名称</param>
        /// <returns>内置模块对象</returns>
        private T GetInternalModule<T>() where T : class,IModuleManager
        {
            var type = typeof(T);
            if (type == this.GetType())
            {
                return this as T;
            }

            foreach (var module in _internalModulesList)
            {
                if (module is T result)
                {
                    return result;
                }
            }

            return null;
        }
    }
}