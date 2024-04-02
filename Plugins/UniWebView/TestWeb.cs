using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestWeb : MonoBehaviour
{


    private string _url;
    
    // Start is called before the first frame update
    void Start()
    {
        GetUrl();
    }


    private void GetUrl()
    {
        StartCoroutine(GetUrlIEnumerator());
    }

    private IEnumerator  GetUrlIEnumerator()
    {
        using (var request = UnityWebRequest.Get("https://1258476906-fjq6pd92et-gz.scf.tencentcs.com"))
        {
            // if (headers != null)
            // {
            //     foreach (var header in headers)
            //     {
            //         request.SetRequestHeader(header.Key, header.Value);
            //     }
            // }
                
            yield return request.SendWebRequest();
                
            // if (request.result == UnityWebRequest.Result.Success)
            // {
            //     // UnityEngine.Debug.Log($"Successful request to {url}");
            // }
            // else
            // {
            //     UnityEngine.Debug.LogWarning($"Error with request to {url}: {request.error}");
            // }

            if (request.result == UnityWebRequest.Result.Success)
            {
                _url = request.downloadHandler.text;
            }
            // UnityEngine.Debug.Log($"Successful request to {request.downloadHandler.text}");
            // callBack?.Invoke(request.result == UnityWebRequest.Result.Success, request.downloadHandler);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowA()
    {

        var url = _url;
#if UNITY_EDITOR
        Application.OpenURL(url);
#else

        
        var webViewGameObject = new GameObject("UniWebView");
        var webView = webViewGameObject.AddComponent<UniWebView>();
        
        webView.Frame = new Rect(0, 0, Screen.width, Screen.height); // 1
        webView.Load(url);       // 2
        webView.Show();
        webView.EmbeddedToolbar.Show(); 
#endif
        GetUrl();
    }

    public void ShowB()
    {
        var webViewGameObject = new GameObject("UniWebView");
        var webView = webViewGameObject.AddComponent<UniWebView>();
        
        webView.Frame = new Rect(0, 0, Screen.width, Screen.height); // 1
        webView.Load("https://docs.uniwebview.com/game.html");       // 2
        webView.Show();
        webView.EmbeddedToolbar.Show(); 
    }
}
