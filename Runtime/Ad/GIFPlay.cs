using Gif2Textures;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GIFPlay : MonoBehaviour {

    /// <summary>
    /// 播放状态
    /// </summary>
    public enum PLAYSTATE
    {
        ONCE = 0,
        LOOP
    }

    [Tooltip("目标对象_UI")] public RawImage m_RawImage;
    [Tooltip("目标对象_模型")] public Renderer m_renderer;
    // [Tooltip("自动播放")] public bool m_bAutoPlay = true;
    [Tooltip("播放方式")] public PLAYSTATE m_state = PLAYSTATE.LOOP;
    [Tooltip("帧数")][Range(1, 30)] public int m_frameCount = 21;
    // [Tooltip("Resources下的路径")] public string m_GifFileName = @"GIFImg\0.gif";//把gif图片后缀改成bytes
    [Tooltip("是否缓存贴图")] public bool m_cacheTextures = true;//如果是则第一次会卡顿一下 把所有图片加载到内存 否则每张都需要重新加载，之前的将会释放
    //---------------------公有变量 -- 非序列号
    [System.NonSerialized] public int FrameIndex = 0;//当前帧
    [System.NonSerialized] public int FrameCount = 0;//总帧数
    //---------------------私有变量
    private bool m_bIsPlayer = false;//是否正在播放
    private float m_frameSecond = 0.045f;//帧秒
    GifFrames m_GifFrames = null;//GIF加载器

    // 初始运算
    // void Start () {
    //     //加载资源
    //     TextAsset ta = Resources.Load(m_GifFileName) as TextAsset;
    //     if (ta == null)
    //     {
    //         Debug.LogWarning("不能打开gif文件 \"" + m_GifFileName + "\" 来自: " + gameObject.name);
    //         return;
    //     }
    //     //获取
    //     MemoryStream ms = new MemoryStream(ta.bytes);
    //     m_GifFrames = new GifFrames();
    //     if (!m_GifFrames.Load(ms, m_cacheTextures))
    //         m_GifFrames = null;
    //
    //     //总帧数
    //     FrameCount = m_GifFrames.GetFrameCount();
    //     //获取每一张图片的持续时间
    //     m_frameSecond = 1.0f / m_frameCount;
    //     //如果自动开始 则开始播放
    //     if (m_bAutoPlay) Play();
	// }

    // void Update()
    // {
    //     if(Input.GetMouseButtonDown(0)) {
    //         Stop();
    //         Play();
    //     }
    //     if (Input.GetMouseButtonDown(1)) {
    //         Stop();
    //     }
    // }


    public void Play(byte[] bytes)
    {
        Stop();
        
        MemoryStream ms = new MemoryStream(bytes);
        m_GifFrames = new GifFrames();
        if (!m_GifFrames.Load(ms, m_cacheTextures))
            m_GifFrames = null;
        
        //总帧数
        FrameCount = m_GifFrames.GetFrameCount();
        //获取每一张图片的持续时间
        m_frameSecond = 1.0f / m_frameCount;
        
        Play();
    }
    
    /// <summary>
    /// 开始播放序列帧
    /// </summary>
    public void Play()
    {
        m_bIsPlayer = true;//记录是否在播放
        StartCoroutine(PlayUpdate());//开始更新
    }

    /// <summary>
    /// 暂停播放
    /// </summary>
    public void Pause()
    {
        m_bIsPlayer = false;//记录是否在播放
        StopAllCoroutines();//停止协程
    }

    /// <summary>
    /// 停止播放
    /// </summary>
    public void Stop()
    {
        m_bIsPlayer = false;//记录是否在播放
        FrameIndex = 0;//返回为第一帧
        m_GifFrames?.Restart();
        StopAllCoroutines();//停止协程
    }

    /// <summary>
    /// 是否正在播放
    /// </summary>
    public bool IsPlayer()
    {
        return m_bIsPlayer;
    }

    /// <summary>
    /// 切换图片
    /// </summary>
    void ChangeImage()
    {
        Texture2D texTemp;
        float delay;
        m_GifFrames.GetNextFrame(out texTemp, out delay);//从gif管理器获取纹理
        //RawImage 或者 模型贴图
        if (m_RawImage)
        {
            m_RawImage.texture = texTemp;
        }
        else
        {
            m_renderer.material.mainTexture = texTemp;
        }
        
    }

    /// <summary>
    /// 使用协程更新
    /// </summary>
    IEnumerator PlayUpdate()
    {
        float oldTime = Time.time;
        //死循环运算
        while (true)
        {
            #region 替换为下一张
            ++FrameIndex;//索引+ addCount  相当于+1  如果是pingpong 则可能addCount == -1
            //如果索引超出范围
            if (FrameIndex >= FrameCount || FrameIndex < 0)
            {
                //不同状态有不同的处理方式
                if (m_state == PLAYSTATE.ONCE)
                {
                    Stop();
                    break;
                }
                else if (m_state == PLAYSTATE.LOOP)
                {
                    FrameIndex = 0;//如果是循环则从0开始
                }
            }
            ChangeImage();//替换图片
            #endregion
            // yield return new WaitForSeconds(m_frameSecond);//暂停协程,开始其他运算
            yield return new WaitForSeconds(0.5f);//暂停协程,开始其他运算
        }
        yield return null;
    }

}

