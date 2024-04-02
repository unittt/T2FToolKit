using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace T2FToolKit
{
    internal static class CacheSystem
    {
        private static AdResponseData Data;
        private const string CACHE_DATA_KEY = "adcachedata";

        /// <summary>
        /// 单线下载中
        /// </summary>
        private static bool _isLoading;

        /// <summary>
        /// 单线下载等待
        /// </summary>
        private static readonly WaitUntil _loadWait;

        static CacheSystem()
        {
            Data = LoadData();

            if (Data.Responses.Count > 0)
            {
                //清理缓存
                foreach (var response in Data.Responses)
                {
                    if (File.Exists(response.localUrl))
                    {
                        File.Delete(response.localUrl);
                    }

                    if (File.Exists(response.localIconUrl))
                    {
                        File.Delete(response.localIconUrl);
                    }
                }

                Data.Responses.Clear();
                SaveData();
            }

            _loadWait = new WaitUntil(() => !_isLoading);
        }

        private static AdResponseData LoadData()
        {
            try
            {
                var json = PlayerPrefs.GetString(CACHE_DATA_KEY, "");
                return !string.IsNullOrEmpty(json) ? JsonUtility.FromJson<AdResponseData>(json) : new AdResponseData();
            }
            catch (Exception e)
            {
                return new AdResponseData();
            }
        }


        public static void SaveData()
        {
            if (Data == null) return;
            var json = JsonUtility.ToJson(Data);
            PlayerPrefs.SetString(CACHE_DATA_KEY, json);
        }

        /// <summary>
        /// 下载并且缓存广告
        /// </summary>
        /// <param name="adResponse"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static Coroutine DownloadAndCacheAD(AdResponse adResponse, bool IsloadIcon, Action<AdResponse> callBack)
        {
            return T2FToolKit.Ad.StartCoroutine(DownloadAndCacheADInternal(adResponse, IsloadIcon, callBack));
        }

        /// <summary>
        /// 下载并且缓存广告
        /// </summary>
        /// <param name="adResponse"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        private static IEnumerator DownloadAndCacheADInternal(AdResponse newResponse, bool IsloadIcon,
            Action<AdResponse> callBack)
        {
            if (newResponse is null)
            {
                yield break;
            }

            //单线加载，如果其他地方在加载资源，则等待
            if (_isLoading)
            {
                yield return _loadWait;
            }

            _isLoading = true;

            var result = newResponse;
            foreach (var response in Data.Responses)
            {
                //判断是否存在缓存
                if (result.url == response.url && File.Exists(response.localUrl))
                {
                    result.localUrl = response.localUrl;

                    if (IsloadIcon && File.Exists(response.localIconUrl))
                    {
                        result.localIconUrl = response.localIconUrl;
                    }

                    break;
                }
            }

            if (string.IsNullOrEmpty(result.localUrl))
            {
                string adUrl = result.url;
                using (var www = UnityWebRequest.Get(adUrl))
                {
                    //获得文件的名称
                    var fileName = Guid.NewGuid() + Path.GetFileName(new Uri(adUrl).LocalPath);
                    var path = Path.Combine(Application.persistentDataPath, fileName);
                    www.downloadHandler = new DownloadHandlerFile(path);
                    // 下载视频
                    yield return www.SendWebRequest();

                    if (www.result == UnityWebRequest.Result.Success)
                    {
                        //保存到本地
                        //设置adResponse 本地url
                        result.localUrl = path;
                        //加入到data列表中
                        Data.Responses.Add(result);
                        //保存data
                        SaveData();
                    }
                }
            }


            if (IsloadIcon)
            {
                var iconUrl = result.detail.adIcon;
                //保存icon
                if (string.IsNullOrEmpty(result.localIconUrl) && !string.IsNullOrEmpty(iconUrl))
                {
                    using (var www = UnityWebRequest.Get(iconUrl))
                    {
                        //获得文件的名称
                        var fileName = Guid.NewGuid() + Path.GetFileName(new Uri(iconUrl).LocalPath);
                        var path = Path.Combine(Application.persistentDataPath, fileName);
                        www.downloadHandler = new DownloadHandlerFile(path);
                        // 下载icon
                        yield return www.SendWebRequest();

                        if (www.result == UnityWebRequest.Result.Success)
                        {
                            //保存到本地
                            //设置adResponse 本地url
                            result.localIconUrl = path;
                            //加入到data列表中
                            if (!Data.Responses.Contains(result))
                            {
                                Data.Responses.Add(result);
                            }

                            //保存data
                            SaveData();
                        }
                    }
                }
            }

            callBack?.Invoke(result);
            _isLoading = false;
        }
    }

    [System.Serializable]
    public class AdResponseData
    {
        public List<AdResponse> Responses = new();
    }
}