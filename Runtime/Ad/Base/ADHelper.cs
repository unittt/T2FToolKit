using System;

namespace T2FToolKit
{
    public abstract class ADHelper
    {
        /// <summary>
        /// 是否能够播放激励广告
        /// </summary>
        public abstract bool IsRewarded { get; }
        
        public abstract void OnInit();
        
        public abstract void ShowRewardedVideo(Action<bool> callback);
        
        /// <summary>
        /// 显示Banner
        /// </summary>
        public abstract void ShowBanner();

        /// <summary>
        /// 隐藏Banner
        /// </summary>
        public abstract void HideBanner();

        public abstract void OnUpdate();
    }
}