using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UI层级枚举
/// </summary>
public enum E_UI_Layer
{
    Bot,
    Mid,
    Top,
    System
}

/// <summary>
/// UI管理器
/// 管理所有显示的面板
/// 提供给外部 显示和隐藏等等接口
/// </summary>
public class UIManager : BaseManager<UIManager>
{
    public Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    private Transform bot;
    private Transform mid;
    private Transform top;
    private Transform system;

    //记录外面UI的Canvas父对象 方便以后外部可能会使用它
    public RectTransform canvas;

    public UIManager()
    {
        //创建Canvas 让其过场景时 不被移除
        GameObject obj = ResourcesMgr.GetInstance().Load<GameObject>("UI/Canvas");
        canvas = obj.transform as RectTransform;
        GameObject.DontDestroyOnLoad(obj);

        //找到各层
        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        system = canvas.Find("System");
    }

    /// <summary>
    /// 通过层级枚举 得到对应的父对象
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public Transform GetLayerFather(E_UI_Layer layer)
    {
        switch(layer)
        {
            case E_UI_Layer.Bot:
                return bot;
            case E_UI_Layer.Mid:
                return mid;
            case E_UI_Layer.Top:
                return top;
            case E_UI_Layer.System:
                return system;
        }
        return null;
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板上对应的脚本</typeparam>
    /// <param name="panelName">面板名</param>
    /// <param name="layer">面板在Canvas的哪一层</param>
    /// <param name="callBack">创建完面板后执行的回调函数</param>
    public void ShowPanel<T>(string panelName, E_UI_Layer layer = E_UI_Layer.Mid, UnityAction<T> callBack = null) where T : BasePanel
    {
        if (panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].ShowMe();
            //处理面板创建完成后的逻辑
            if (callBack != null)
            {
                callBack(panelDic[panelName] as T);
            }
            //避免面板重复加载 如果面板存在 调用回调函数后 直接return
            return;
        }

        ResourcesMgr.GetInstance().LoadAsync<GameObject>("UI/" + panelName, (obj) =>
        {
            //把它作为 Canvas的子对象
            //并且 要设置它的相对位置
            //找到父对象 确定显示在哪一层
            Transform father = bot;
            switch (layer)
            {
                case E_UI_Layer.Mid:
                    father = mid;
                    break;
                case E_UI_Layer.Top:
                    father = top;
                    break;
                case E_UI_Layer.System:
                    father = system;
                    break;
            }

            //设置父对象 设置相对位置和大小
            obj.transform.SetParent(father);

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;

            //得到预设体身上的面板脚本
            T panel = obj.GetComponent<T>();
            //处理面板创建完成后的逻辑
            if(callBack != null)
            {
                callBack(panel);
            }

            panel.ShowMe();

            //把面板存起来
            panelDic.Add(panelName, panel);
        });
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="panelName">面板名</param>
    public void HidePanel(string panelName)
    {
        if(panelDic.ContainsKey(panelName))
        {
            panelDic[panelName].HideMe(() =>
            {
                GameObject.Destroy(panelDic[panelName].gameObject);
                panelDic.Remove(panelName);
            });
        }
    }

    /// <summary>
    /// 得到某一个已经显示的面板 方便外部调用
    /// </summary>
    public T GetPanel<T>(string panelName) where T : BasePanel
    {
        if(panelDic.ContainsKey(panelName))
        {
            return panelDic[panelName] as T;
        }
        return null;
    }

    /// <summary>
    /// 给控件添加自定义事件监听
    /// </summary>
    /// <param name="control">控件对象</param>
    /// <param name="type">事件类型</param>
    /// <param name="callBack">事件的响应函数</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if(trigger == null)
        {
            trigger = control.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        trigger.triggers.Add(entry);
    }
}
