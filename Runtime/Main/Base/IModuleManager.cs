namespace T2FToolKit
{
    internal interface IModuleManager
    {
        /// <summary>
        /// 模块的优先级（越小越优先）
        /// </summary>
        int Priority { get; }
        
        /// <summary>
        /// 初始化模块
        /// </summary>
        void OnAwake();
        
        /// <summary>
        /// 模块准备工作
        /// </summary>
        void OnStart();
        
        /// <summary>
        /// 更新模块
        /// </summary>
        void OnUpdate();
        /// <summary>
        /// 终结模块
        /// </summary>
        void OnTerminate();
    }
}