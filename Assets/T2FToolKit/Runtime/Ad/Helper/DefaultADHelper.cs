using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace T2FToolKit
{
    /// <summary>
    /// 内置的广告助手
    /// </summary>
    internal class DefaultADHelper : ADHelper
    {
        public override bool IsRewarded => _videoLogic.IsCanShow();

        private VideoLogic _videoLogic;
        private BannerLogic _bannerLogic;
        private readonly Dictionary<int, ADEntityLogic> _logics = new();
        
        /// <summary>
        /// 单线下载中
        /// </summary>
        private bool _isLoading;
        /// <summary>
        /// 单线下载等待
        /// </summary>
        private WaitUntil _loadWait;
        

        public override void OnInit()
        {
            _videoLogic = new VideoLogic();
            _bannerLogic = new BannerLogic();
            
            _loadWait = new WaitUntil(() => !_isLoading);
            _logics.Add(_videoLogic.GetADType(), _videoLogic);
            _logics.Add(_bannerLogic.GetADType(), _bannerLogic);
            
            RequestAD();
        }

        public override void ShowRewardedVideo(Action<bool> callback)
        {
            _videoLogic.Show(callback);
        }

        public override void ShowBanner()
        {
            _bannerLogic.Show(null);
        }

        public override void HideBanner()
        {
            _bannerLogic.Hide();
        }

        public override void OnUpdate()
        {
            _videoLogic?.OnUpdate();
            _bannerLogic?.OnUpdate();
        }
        
        internal void RequestAD(string type = "null")
        {
            T2FToolKit.Ad.StartCoroutine(RequestADIEnumerator(type));
        }
        
        private IEnumerator RequestADIEnumerator(string adType)
        {
            if (_isLoading)
            {
                yield return _loadWait;
            }

            
            if (int.TryParse(adType, out  var type))
            {
                if ( _logics.TryGetValue(type, out var logic) && logic.IsCanShow())
                {
                    yield break;
                }
            }
            
            _isLoading = true;
            
            var url =  T2FToolKit.Ad.GetEndPoint("/app/loadAd");
            // 创建请求体
            var requestBody = new ADRequestBody
            {
                //设备唯一ID
                deviceId = DeviceIDUtil.DeviceID,
                //广告类型。0-banner；1-激励视频
                type = adType
            };
            
            //to json
            var json = JsonUtility.ToJson(requestBody);
            
            //加密
            var encryptedData = AesUtil.EncryptString(T2FToolKit.Ad.EncryptionKey ,json);
            var bodyRaw = new UTF8Encoding().GetBytes(encryptedData);

            AdResponseList adResponseList = null;
            
            //请求广告
            yield return T2FToolKit.Ad.SendPostRequest(url, T2FToolKit.Ad.JsonHeaders, bodyRaw,
                (result, downloadHandler) =>
                {
                    if (result)
                    {
                        // 成功响应，处理数据
                        var responseText = downloadHandler.text;
                        var data = AesUtil.DecryptString(T2FToolKit.Ad.EncryptionKey,responseText);
                        adResponseList = JsonUtility.FromJson<AdResponseList>(data);
                    }
                    else
                    {
                        //缓存起来 等待一分钟后请求
                        
                    }
                 
                });
            
            if (adResponseList != null)
            {
                for (var i = 0; i < adResponseList.Ads.Count; i++)
                { 
                    var adResponse = adResponseList.Ads[i];
                   yield return CacheSystem.DownloadAndCacheAD(adResponse, adResponse.type == 1, null);
                }
                
                foreach (var adResponse in adResponseList.Ads)
                {
                    if ( _logics.TryGetValue(adResponse.type, out var logic))
                    {
                        logic.FillResponse(adResponse);
                    }
                }
            }
            
            _isLoading = false;
        }
    }
}