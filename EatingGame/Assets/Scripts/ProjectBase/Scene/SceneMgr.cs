using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// </summary>
public class SceneMgr : BaseManager<SceneMgr>
{
    /// <summary>
    /// 切换场景 同步
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name, UnityAction fun)
    {
        //场景同步加载
        SceneManager.LoadScene(name);
        //加载完成过后 才会去执行fun
        fun.Invoke();
    }

    /// <summary>
    /// 提供给外部的 异步场景切换的接口
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fun"></param>
    public void LoadSceneAsync(string name, UnityAction fun)
    {
        MonoMgr.GetInstance().StartCoroutinee(ReallyLoadSceneAsync(name, fun));
    }

    /// <summary>
    /// 切换场景 异步
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fun"></param>
    /// <returns></returns>
    private IEnumerator ReallyLoadSceneAsync(string name, UnityAction fun)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        //可以得到场景加载的一个进度
        while (!ao.isDone)
        {
            //事件中心 向外分发 进度  外面想用就用
            EventCenter.GetInstance().EventTrigger("进度条更新", ao.progress);
            //这里去更新进度条
            yield return ao.progress;
        }        
        //加载完成过后 才会去执行fun
        fun.Invoke();
    }
}
