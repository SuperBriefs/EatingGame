using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitchButton : MonoBehaviour
{
    // 方法1：通过按钮组件直接绑定
    void Start()
    {
        // 获取按钮组件
        Button button = GetComponent<Button>();

        if (button != null)
        {
            // 添加点击事件监听
            button.onClick.AddListener(LoadChoiceScene);
        }
        else
        {
            Debug.LogError("此脚本需要附加到带有Button组件的GameObject上！");
        }
    }

    // 切换到choice场景的方法
    public void LoadChoiceScene()
    {
        // 加载名为"choice"的场景
        SceneManager.LoadScene("choice");
    }

    // 可选：在Inspector中指定场景名称
    [SerializeField] private string sceneName = "choice";

    public void LoadSceneByName()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    // 可选：异步加载场景（推荐用于较大场景）
    public void LoadChoiceSceneAsync()
    {
        StartCoroutine(LoadSceneAsync("choice"));
    }

    private System.Collections.IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            // 可以在这里更新加载进度条
            // float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            yield return null;
        }
    }
}