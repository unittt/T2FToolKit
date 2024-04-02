using System;
using System.IO;
using UnityEngine;

namespace T2FToolKit
{
    internal class BannerLogic :ADEntityLogic
    {
        private DefaultADBannerComponent _adBannerComponent;

        public override void OnInit()
        {
            _adBannerComponent = T2FToolKit.Ad.GetComponentInChildren<DefaultADBannerComponent>();
            _adBannerComponent.ClickBtn.onClick.AddListener(OpenADURL);
        }

        protected override GameObject GetView()
        {
            return _adBannerComponent.View;
        }

        public override int GetADType()
        {
            return 0;
        }

        protected override void OnPostRequestSuccess(AdResponse adResponse)
        {
            Show(null);
        }
        
        public override bool IsCanShow()
        {
            if (Response == null)
            {
                return false;
            }
            
            return true;
        }
        
        protected override void OnShow()
        {
            if (Response == null)
            {
                return;
            }
            
            var filePath = Response.GetUrl;

            if (!File.Exists(filePath))
            {
                return;
            }
            try
            {
                // 读取.bytes文件的字节数据
                var bytes = File.ReadAllBytes(filePath);
                _adBannerComponent.View.SetActive(true);
                _adBannerComponent.GifPlay.Stop();
                if (filePath.Contains("gif"))
                {
                    _adBannerComponent.GifPlay.Play(bytes);
                }
                else
                {
                    var tex = new Texture2D(2, 2);
                    tex.LoadImage(bytes);
                    _adBannerComponent.RawImage.texture = tex;
                    //播放图片
                    // StartCoroutine(WebRequestUtil.DownTexture(filePath, (texture) =>
                    // {
                    //     if (texture != null)
                    //     {
                    //         _view.SetActive(true);
                    //         _rawImage.texture = texture;
                    //     }
                    // }));
                }

            }
            catch (Exception)
            {
                _adBannerComponent.View.SetActive(false);
            }
        }

        public void Hide()
        {
            
        }
    }
}