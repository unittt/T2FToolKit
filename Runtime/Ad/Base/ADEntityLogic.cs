using System;
using UnityEngine;

namespace T2FToolKit
{
    internal abstract class ADEntityLogic 
    {
        protected AdResponse Response;
        private bool _isShowing;
        private Action<bool> _callBack;
        
        protected abstract GameObject GetView();

        public virtual void OnInit(){}

        public virtual void OnUpdate() {}

        public abstract int GetADType();
        
        public void Show(Action<bool> callBack)
        {
            if (!IsCanShow())
            {
                return;
            }

            try
            {
                _isShowing = true;
                _callBack = callBack;
                T2FToolKit.Ad.ADShow(Response.adId);
            
                SetViewActive(true);
                OnShow();
            }
            catch (Exception e)
            {
                Release();
            }
        }
        
        protected abstract void OnShow();
     

        public virtual bool IsCanShow()
        {
            if (_isShowing)
            {
                return false;
            }
            
            if (Response == null)
            {
                return false;
            }
            
            return true;
        }

        protected void OpenADURL()
        {
            if (Response == null) return;
            Application.OpenURL(Response.detail.adUrl);
            T2FToolKit.Ad.ADClick(Response.adId);
        }

        protected virtual void OnADEnd(bool result)
        {
            //打点
            T2FToolKit.Ad.ADEnd(Response.adId);
            _callBack?.Invoke(result);
            Release();
        }

        public virtual void SetViewActive(bool active)
        {
            GetView().SetActive(active);
        }

        protected virtual void Release()
        {
            _isShowing = false;
            //清理
            Response = null;
            //触发回调
            _callBack = null;
            SetViewActive(false);
        }

        /// <summary>
        /// 填充广告请求
        /// </summary>
        /// <param name="adResponse"></param>
        public void FillResponse(AdResponse adResponse)
        {
            if (adResponse is null || string.IsNullOrEmpty(adResponse.GetUrl))
            {
                return;
            }
            Response = adResponse;
            // Debug.Log($"ADType[{GetADType()}] 加载成功{Response.adId}  URL：{adResponse.url}  LocalURL:{Response.localUrl}");
            OnPostRequestSuccess(Response);
        }
        
        /// <summary>
        /// 当广告请求完成
        /// </summary>
        /// <param name="adResponse"></param>
        protected virtual void OnPostRequestSuccess(AdResponse adResponse)
        {
        }

        protected void RequestAD()
        {
            var helper =T2FToolKit.Ad.Helper as DefaultADHelper;
            helper.RequestAD(GetADType().ToString());
        }
    }
    
       
    [Serializable]
    public class ADRequestBody
    {
        public string deviceId;
        public string type;
    }
}