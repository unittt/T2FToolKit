using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace T2FToolKit
{
    public sealed class AdManager : MonoBehaviour, IModuleManager
    {

        public string Url;
        public string AppId;
        public string EncryptionKey;
        public string ThirdPartyHelper;

        internal ADHelper Helper { get; private set; }

        public int Priority { get; }

        #region 生命周期
        public void OnAwake()
        {
            
        }

        public void OnStart()
        {
            RequestInitConfig();
        }
        
        public void OnUpdate()
        {
            Helper?.OnUpdate();
        }

        public void OnTerminate()
        {
            
        }
        #endregion

        #region  Http头部数据
        internal Dictionary<string, string> UrlencodedHeaders { get; private set; }
        internal Dictionary<string, string> JsonHeaders { get; private set; }
        
        /// <summary>
        /// 初始化请求时的头部参数
        /// </summary>
        private void InitHeadersParams()
        {
            var appId = AppId;
            var lang = LanguageUtil.ToLocale(Application.systemLanguage);
            var version = Application.version;
            var utc = "UTC+" + TimeZoneInfo.Local.BaseUtcOffset.Hours;
            
            UrlencodedHeaders = new Dictionary<string, string>
            {
                { "Content-Type", "application/x-www-form-urlencoded" },
                //应用id
                { "appId", appId },
                //语种 (Chinese,ChineseSimplified,ChineseTraditional,Portuguese,Spanish)
                { "lang", lang},
                //版本号 "1.0.0"
                { "ver", version },
                //时区
                { "zone", utc}
            };
            
            JsonHeaders = new Dictionary<string, string>
            {
                { "Content-Type", "application/json" },
                //应用id
                { "appId", appId },
                //语种 (Chinese,ChineseSimplified,ChineseTraditional,Portuguese,Spanish)
                { "lang", lang},
                //版本号 "1.0.0"
                { "ver", version },
                //时区
                { "zone", utc}
            };
        }
        #endregion

        #region 请求initconfig
        private void RequestInitConfig()
        {
            var url = GetEndPoint("/app/initConfig");
            this.SendGetRequest(url, UrlencodedHeaders, OnInitRequestCallBack);
        }
        
        private void OnInitRequestCallBack(bool result, DownloadHandler downloadHandler)
        {
            var isDefault = false;
            if (result)
            {
                try
                {
                    var responseText = downloadHandler.text;
                    var value = AesUtil.DecryptString(EncryptionKey, responseText);
                    if (value.Contains("game"))
                    {
                        isDefault = true;
                    }
                  
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                    throw;
                }
            }
            Debug.Log(isDefault);
            if (isDefault)
            {
                Helper = new DefaultADHelper();
            }
            else
            {
                Type type = ReflectionToolkit.GetTypeInRunTimeAssemblies(ThirdPartyHelper);
                if (type != null)
                {
                    if (type.IsSubclassOf(typeof(ADHelper)))
                    {
                        Helper = Activator.CreateInstance(type) as ADHelper;
                    }
                    else
                    {
                        
                        throw new Exception($"创建广告助手失败： {ThirdPartyHelper} 必须继承至广告助手基类：ADHelper！");
                    }
                }
            }
            
            Helper?.OnInit();
        }
        #endregion
        
        
        #region 广告api
        public bool IsRewarded => Helper is { IsRewarded: true };
        
        /// <summary>
        /// 显示激励视频
        /// </summary>
        /// <param name="callback"></param>
        public void ShowRewardedVideo(Action<bool> callback)
        {
            if (Helper is null)
            {
                callback?.Invoke(false);
                return;
            }
            
            Helper.ShowRewardedVideo(callback);
        }

        /// <summary>
        /// 显示Banner
        /// </summary>
        public void ShowBanner()
        {
            Helper?.ShowBanner();
        }

        /// <summary>
        /// 隐藏Banner
        /// </summary>
        public void HideBanner()
        {
            Helper?.HideBanner();
        }
        #endregion
        
        
        internal string GetEndPoint(string point)
        {
            return Url + point;
        }

        #region 打点
        /// <summary>
        /// 展示
        /// </summary>
        /// <param name="adId"></param>
        internal void ADShow(long adId)
        {
            PostEvent(adId,"1");
        }
        
        /// <summary>
        /// 播放完成
        /// </summary>
        /// <param name="adId"></param>
        internal void ADEnd(long adId)
        {
            PostEvent(adId,"2");
        }
        
        /// <summary>
        /// 点击
        /// </summary>
        /// <param name="adId"></param>
        internal void ADClick(long adId)
        {
            PostEvent(adId,"3");
        }
        
        internal void PostEvent(long adId, string eventCode)
        {
            PostEventIEnumerator(adId, eventCode);
        }


        private void PostEventIEnumerator(long aid, string eventCode)
        {
            // 构建请求体的JSON数据
            var requestBody = new EventRequestBody()
            {
                adId = aid,
                deviceId = DeviceIDUtil.DeviceID,
                @event = eventCode
            };
            var json = JsonUtility.ToJson(requestBody);
            //加密
            var encryptedData =  AesUtil.EncryptString(EncryptionKey ,json);
    
            var jsonToSend = new UTF8Encoding().GetBytes(encryptedData);
            // 创建UnityWebRequest对象
            var endPoint = GetEndPoint("/app/event");
            this.SendPostRequest(endPoint,JsonHeaders, jsonToSend, null);
        }

        [Serializable]
        internal class EventRequestBody
        {
            public long adId;
            public string deviceId;
            public string @event;
        }
        #endregion
    }
}