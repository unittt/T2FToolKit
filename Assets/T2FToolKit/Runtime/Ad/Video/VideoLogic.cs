using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace T2FToolKit
{
    internal class VideoLogic : ADEntityLogic
    {
        private RenderTexture _texture;
        private Vector2Int _canvasSizeDelta;
        private bool _result;
        private bool _isPlaying;
        private float _duration;
        
        private DefaultADVideoComponent _videoComponent;
        
        
        #region 词条
        private LanguageEntry _rewardAvailable;
        private LanguageEntry _rewardObtained;
        private LanguageEntry _enter;

        private LanguageEntry _confirmTxtEntry1;
        private LanguageEntry _confirmTxtEntry2;
        private LanguageEntry _confirmTxtEntry3;
        private LanguageEntry _confirmTxtEntry4;
        #endregion
        
        
        public override void OnInit()
        {
            _videoComponent = T2FToolKit.Ad.GetComponentInChildren<DefaultADVideoComponent>();
            
            _canvasSizeDelta = new Vector2Int((int)_videoComponent.CanvasRectTransform.sizeDelta.x, (int)_videoComponent.CanvasRectTransform.sizeDelta.y);
            _videoComponent.ClickBtn.onClick.AddListener(OpenADURL);
            _videoComponent.AppEnterBtn.onClick.AddListener(OpenADURL);
            _videoComponent.CloseBtn.onClick.AddListener(OnCloseVideo);
            _videoComponent.CancelBtn.onClick.AddListener(OnClickCancel);
            _videoComponent.ContinueBtn.onClick.AddListener(OnClickContinue);


            #region 词条
            _rewardAvailable = new LanguageEntry("秒后可获得奖励", "sec til reward avail", "seg p/ recomp. disp", "seg para recomp. disp");
            _rewardObtained = new LanguageEntry("已获得奖励", "Reward obtained", "Recompensa obtida", "Recompensa obtenida");
            _enter = new LanguageEntry("进入游戏", "Enter the game", "Entrar no jogo", "Entrar al juego");
            
            _confirmTxtEntry1 = new LanguageEntry("暂未获得奖励", "No rewards obtained yet", "Ainda não foram obtidas recompensas", "Aún no se han obtenido recompensas");
            _confirmTxtEntry2 = new LanguageEntry("是否继续?", "Continue?", "Continuar?", "¿Continuar?");
            _confirmTxtEntry3 = new LanguageEntry("放弃", "Give up", "Desistir", "Rendirse");
            _confirmTxtEntry4 = new LanguageEntry("继续", "Continue", "Continuar", "Continuar");
            #endregion
        }
        
        protected override GameObject GetView()
        {
            return _videoComponent.View;
        }

        public override int GetADType()
        {
            return 1;
        }

        protected override void OnShow()
        {
            _result = false;
            _isPlaying = false;
            
            CreateTexture();
            
            var url = Response.GetUrl;

            var videoPlayer = _videoComponent.VideoPlayer;
            
            videoPlayer.url = url;
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare();
            videoPlayer.Play();
            
            ShowAppInfo();
            
            //隐藏取消界面
            _videoComponent.ConfirmPanel.SetActive(false);
            SetConfirmEntry();
            
            //计时显示关闭
            _videoComponent.TimeTxt.gameObject.SetActive(false);
            _videoComponent.ObtainedTxt.gameObject.SetActive(false);
            
            _videoComponent.Countdown.text = _rewardAvailable.GetValue();
            _videoComponent.ObtainedTxt.text = _rewardObtained.GetValue();
            
            AudioListener.volume = 0;
        }
        
        private void SetConfirmEntry()
        {
            _videoComponent.ConfirmTxt1.text = _confirmTxtEntry1.GetValue();
            _videoComponent.ConfirmTxt2.text = _confirmTxtEntry2.GetValue();
            _videoComponent.CancelTxt.text = _confirmTxtEntry3.GetValue();
            _videoComponent.ContinueTxt.text = _confirmTxtEntry4.GetValue();
        }
        
        private void OnVideoPrepared(VideoPlayer source)
        {
            _duration = (int)source.length; // 获取视频时长
            _videoComponent.TimeTxt.gameObject.SetActive(true);
            _isPlaying = true;
            
            T2FToolKit.Ad.StartCoroutine(RefreshTopWidth1());
        }
        
        private IEnumerator RefreshTopWidth1()
        {
            yield return new WaitForEndOfFrame();
            var width = _videoComponent.Countdown.preferredWidth;
            width += 140;
            _videoComponent.Topc.sizeDelta = new Vector2(width, 50);
        }
        
        private IEnumerator RefreshTopWidth2()
        {
            yield return new WaitForEndOfFrame();
            var width = _videoComponent.ObtainedTxt.preferredWidth;
            width += 140;
            _videoComponent.Topc.sizeDelta = new Vector2(width, 50);
        }
        
        public override void OnUpdate()
        {
            if (_isPlaying &&!_result&&  _duration > 0)
            {
                _duration -= Time.deltaTime;
                var intValue = (int)_duration;
                _videoComponent.TimeTxt.text = intValue.ToString();

                if (intValue <= 0)
                {
                    _videoComponent.TimeTxt.gameObject.SetActive(false);
                    _videoComponent.ObtainedTxt.gameObject.SetActive(true);
                    T2FToolKit.Ad.StartCoroutine(RefreshTopWidth2());
                    _result = true;
                }
            }
        }
        
        private void ShowAppInfo()
        {
            try
            {
                
                if (File.Exists(Response.localIconUrl))
                {
                    var bytes = File.ReadAllBytes(Response.localIconUrl);
                    var tex = new Texture2D(2, 2);
                    tex.LoadImage(bytes);
                    _videoComponent.AppIcon.texture = tex;
                    _videoComponent.AppIcon.SetNativeSize();
                }

                _videoComponent.AppName.text = Response.detail.adName;
                _videoComponent.AppDes.text = Response.detail.adDesc;
                _videoComponent.EnterTxt.text = _enter.GetValue();

            }
            catch (Exception e)
            {
                
            }
        }
        
        
        private void CreateTexture()
        {
            _texture = RenderTexture.GetTemporary(_canvasSizeDelta.x, _canvasSizeDelta.y, 0);
            _texture.autoGenerateMips = false;
            _videoComponent.VideoPlayer.targetTexture = _texture;
            _videoComponent.RawImage.texture = _texture;
        }
        
        private void ReleaseTemporary()
        {
            var videoPlayer = _videoComponent.VideoPlayer;
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.Stop();
            videoPlayer.clip = null;
            videoPlayer.url = "";
            
            videoPlayer.targetTexture = null;
            _videoComponent.RawImage.texture = null;
            
            _texture?.Release();
            RenderTexture.ReleaseTemporary(_texture);
            _texture = null;
            
            AudioListener.volume = 1;
        }
        
        private void OnCloseVideo()
        {
            if (_result)
            {
                OnADEnd(_result);
            }
            else
            {
                //暂停视频
                _videoComponent.VideoPlayer.Pause();
                //当没有看完广告时
                _isPlaying = false;
                _videoComponent.ConfirmPanel.SetActive(true);   
            }
        }
        
        private void OnClickCancel()
        {
            //取消关闭视频
            OnADEnd(false);
        }
        
        private void OnClickContinue()
        {
            _videoComponent.ConfirmPanel.SetActive(false);
            //继续播放
            _isPlaying = true;
            _videoComponent.VideoPlayer.Play();
        }
        
        protected override void Release()
        {
            base.Release();
            ReleaseTemporary();
            RequestAD();
            //
            _isPlaying = false;
            _result = false;
            _duration = 0;
        }
    }
}