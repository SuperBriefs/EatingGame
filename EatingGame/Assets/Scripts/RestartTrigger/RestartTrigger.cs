using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartTrigger : MonoBehaviour
{
    [Header("触发配置")]
    [SerializeField] private LayerMask playerLayer; //玩家图层
    [SerializeField] private string restartSceneName; //要重启的场景名称
    [SerializeField] private float restartDelay = 0f; //重启延迟

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Invoke(nameof(RestartLevel), restartDelay);
        }
    }

    //重启关卡核心逻辑
    private void RestartLevel()
    {
        if (string.IsNullOrEmpty(restartSceneName))
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
        else
        {
            //重启指定名称的场景
            SceneManager.LoadScene(restartSceneName);
        }
    }
}
