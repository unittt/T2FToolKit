using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
// ReSharper disable ConvertToUsingDeclaration

namespace T2FToolKit
{
    internal static class WebRequestUtil
    {
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="self"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static Coroutine SendGetRequest(this MonoBehaviour self,string url,Dictionary<string,string> headers ,Action<bool,DownloadHandler> callBack)
        {
            return self.StartCoroutine(SendGetRequestCoroutine(url, headers, callBack));
        }

        private static IEnumerator SendGetRequestCoroutine(string url, Dictionary<string,string> headers ,Action<bool,DownloadHandler> callBack)
        {
            using (var request = UnityWebRequest.Get(url))
            {
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.SetRequestHeader(header.Key, header.Value);
                    }
                }
                
                yield return  request.SendWebRequest();
                callBack?.Invoke(request.result == UnityWebRequest.Result.Success, request.downloadHandler);
            }
        }
        
        
        /// <summary>
        /// Post 请求
        /// </summary>
        /// <param name="self"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <param name="bodyRaw"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static Coroutine SendPostRequest(this MonoBehaviour self,string url, Dictionary<string,string> headers,byte[] bodyRaw,Action<bool,DownloadHandler> callBack)
        {
            return self.StartCoroutine(SendPostRequestCoroutine(url, headers,bodyRaw, callBack));
        }
        
        private static IEnumerator SendPostRequestCoroutine(string url, Dictionary<string,string> headers,byte[] bodyRaw,Action<bool,DownloadHandler> callBack)
        {
            using (var request = new UnityWebRequest(url, "POST"))
            {
                
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.SetRequestHeader(header.Key, header.Value);
                    }
                }
                
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                yield return request.SendWebRequest();
                
                callBack?.Invoke(request.result == UnityWebRequest.Result.Success, request.downloadHandler);
            }
        }
    }
}