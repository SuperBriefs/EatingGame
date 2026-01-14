using UnityEngine;

/// <summary>
/// 输入控制模块
/// </summary>
public class InputMgr : BaseManager<InputMgr>
{
    private bool isStart = false;
    /// <summary>
    /// 构造函数中 添加Update监听
    /// </summary>
    public InputMgr()
    {
        MonoMgr.GetInstance().AddUpdateListener(MyUpdate);
    }

    /// <summary>
    /// 是否开启或关闭 我的输入检测
    /// </summary>
    public void StartOrEndCheck(bool isOpen)
    {
        isStart = isOpen;
    }

    /// <summary>
    /// 用来检测按键抬起按下 分发事件的
    /// </summary>
    /// <param name="key"></param>
    private void CheckKeyCode(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            //事件中心模块 分发按下抬起事件
            EventCenter.GetInstance().EventTrigger("某键按下", key);
        }
        if (Input.GetKeyUp(key))
        {
            //事件中心模块 分发按下抬起事件
            EventCenter.GetInstance().EventTrigger("某键抬起", key);
        }
    }

    private void MyUpdate()
    {
        //没有开启输入检测 就不去检测 直接return
        if(!isStart) 
            return;
        CheckKeyCode(KeyCode.W);
        CheckKeyCode(KeyCode.A);
        CheckKeyCode(KeyCode.S);
        CheckKeyCode(KeyCode.D);
    }
}
