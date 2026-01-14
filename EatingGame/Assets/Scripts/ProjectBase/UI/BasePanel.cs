using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 面板基类
/// 找到所有自己面板下的控件对象
/// 提供 显示 或 隐藏的行为
/// </summary>
public class BasePanel : MonoBehaviour
{
    //专门用于控制面板透明度的组件
    private CanvasGroup canvasGroup;
    //淡入淡出的速度
    private float alphaSpeed = 5;

    //当前是隐藏还是显示
    public bool isShow = false;

    //当隐藏完毕后 想要做的事情
    private UnityAction hideCallBack = null;

    //通过里氏替换原则 来存储所有控件
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();
    
    protected virtual void Awake()
    {
        //一开始就获取面板上挂载的CanvasGroup组件
        canvasGroup = GetComponent<CanvasGroup>();
        //如果忘记添加这样一个脚本
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        FindChildrenControl<Button>();
        FindChildrenControl<Image>();
        FindChildrenControl<Text>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<InputField>();
    }

    /// <summary>
    /// 显示自己
    /// </summary>
    public virtual void ShowMe()
    {
        canvasGroup.alpha = 0;
        isShow = true;
    }

    /// <summary>
    /// 隐藏自己
    /// </summary>
    public virtual void HideMe(UnityAction callBack = null)
    {
        canvasGroup.alpha = 1;
        isShow = false;

        hideCallBack = callBack;
    }

    /// <summary>
    /// 得到对应名字的对应控件脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    protected T GetControl<T>(string controlName) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(controlName))
        {
            for(int i = 0; i < controlDic[controlName].Count; i++)
            {
                if (controlDic[controlName][i] is T)
                {
                    return controlDic[controlName][i] as T;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// 找到子对象的对应控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        for (int i = 0; i < controls.Length; i++)
        {
            string objName = controls[i].gameObject.name;
            if (controlDic.ContainsKey(objName))
            {
                controlDic[objName].Add(controls[i]);
            }
            else
            {
                controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
            }

            //如果是按钮控件
            if (controls[i] is Button)
            {
                (controls[i] as Button).onClick.AddListener(() =>
                {
                    OnClick(objName);
                });
            }
            //如果是单选框 或 多选框
            else if(controls[i] is Toggle)
            {
                (controls[i] as Toggle).onValueChanged.AddListener((isOpen) =>
                {
                    OnValueChangedToggle(objName, isOpen);
                });
            }
            //如果是输入框
            else if(controls[i] is InputField)
            {
                (controls[i] as InputField).onValueChanged.AddListener((text) =>
                {
                    OnValueChangedInput(objName, text);
                });
                (controls[i] as InputField).onSubmit.AddListener((text) =>
                {
                    OnSubmitInput(objName, text);
                });
                (controls[i] as InputField).onEndEdit.AddListener((text) =>
                {
                    OnEndEditInput(objName, text);
                });
            }
            //如果是滑动条
            else if(controls[i] is Slider)
            {
                (controls[i] as Slider).onValueChanged.AddListener((value) =>
                {
                    OnValueChangedSlider(objName, value);
                });
            }
            //如果是滚动视图
            else if(controls[i] is ScrollRect)
            {
                (controls[i] as ScrollRect).onValueChanged.AddListener((value) =>
                {
                    OnValueChangedScrollRect(objName, value);
                });
            } 
        }
    }

    /// <summary>
    /// 点击按钮触发事件
    /// </summary>
    /// <param name="btnName"></param>
    protected virtual void OnClick(string btnName) { }

    /// <summary>
    /// 点击单选框或多选框触发事件
    /// </summary>
    /// <param name="btnName"></param>
    /// <param name="value"></param>
    protected virtual void OnValueChangedToggle(string btnName, bool value) { }

    /// <summary>
    /// 输入框响应事件
    /// </summary>
    /// <param name="btnName"></param>
    /// <param name="text"></param>
    protected virtual void OnValueChangedInput(string btnName, string text) { }
    protected virtual void OnSubmitInput(string btnName, string text) { }
    protected virtual void OnEndEditInput(string btnName, string text) { }

    /// <summary>
    /// 拖动条响应事件
    /// </summary>
    /// <param name="btnName"></param>
    /// <param name="text"></param>
    protected virtual void OnValueChangedSlider(string btnName, float value) { }

    /// <summary>
    /// 滚动视图响应事件
    /// </summary>
    /// <param name="btnName"></param>
    /// <param name="value"></param>
    protected virtual void OnValueChangedScrollRect(string btnName, Vector2 value) { }

    /// <summary>
    /// 在Update里进行面板的显隐
    /// </summary>
    protected virtual void Update()
    {
        //当处于显示状态时 如果透明度不为1 就会不停加到1 加到1过后 就停止变化了
        //淡入
        if (isShow && canvasGroup.alpha != 1)
        {
            canvasGroup.alpha += alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha >= 1)
            {
                canvasGroup.alpha = 1;
            }
        }
        //淡出
        else if (!isShow && canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= alphaSpeed * Time.deltaTime;
            if (canvasGroup.alpha <= 0)
            {
                canvasGroup.alpha = 0;
                //让面板 透明度淡出完成之后 再去执行的一些逻辑
                hideCallBack?.Invoke();
            }
        }
    }
}
