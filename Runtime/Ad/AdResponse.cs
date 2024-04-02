using System.Collections.Generic;


namespace T2FToolKit
{
    [System.Serializable]
    public class AdResponseList
    {
        public List<AdResponse>  Ads;
    }
    
    [System.Serializable]
    public class AdResponse
    {
        /// <summary>
        /// 广告id
        /// </summary>
        public long adId;

        /// <summary>
        /// 广告类型 0 banner 1 video
        /// </summary>
        public int type;
        /// <summary>
        /// 广告详情
        /// </summary>
        public AdDetail detail;
        /// <summary>
        /// 广告地址
        /// </summary>
        public string url;
        
        /// <summary>
        /// 本地地址
        /// </summary>
        public string localUrl;
        
        /// <summary>
        /// 获取实际url
        /// </summary>
        public string GetUrl => !string.IsNullOrEmpty(localUrl) ? localUrl : url;

        /// <summary>
        /// app icon
        /// </summary>
        public string localIconUrl;
    }
    
    
    [System.Serializable]
    public class AdDetail
    {
        /// <summary>
        /// 广告描述
        /// </summary>
        public string adDesc;
        /// <summary>
        /// 广告素材icon
        /// </summary>
        public string adIcon;
        /// <summary>
        /// 广告名称
        /// </summary>
        public string adName;
        /// <summary>
        /// 落地地址
        /// </summary>
        public string adUrl;
    }
}