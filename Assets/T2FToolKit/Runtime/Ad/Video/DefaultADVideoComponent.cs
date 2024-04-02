using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace T2FToolKit
{
    public sealed class DefaultADVideoComponent : MonoBehaviour
    {
        public RectTransform CanvasRectTransform;
        public GameObject View;
        public RawImage RawImage;
        public VideoPlayer VideoPlayer;
        public Button ClickBtn;
        public Button CloseBtn;
        public RawImage AppIcon;
        public Text AppName;
        public Text AppDes;
        public Button AppEnterBtn;
        public Text TimeTxt;
        public Text Countdown;
        public Text ObtainedTxt;
        public Text EnterTxt;
        public RectTransform Topc;
        
        
        #region 二次确认面板
        public GameObject ConfirmPanel;
        public Text ConfirmTxt1;
        public Text ConfirmTxt2;
        public Button CancelBtn;
        public Text CancelTxt;
        public Button ContinueBtn;
        public Text ContinueTxt;
        #endregion
    }
}