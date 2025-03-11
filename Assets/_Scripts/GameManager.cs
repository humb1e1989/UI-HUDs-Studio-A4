using UnityEngine;

public class GameManager : SingletonMonoBehavior<GameManager>
{
    [SerializeField] private int score;
    [SerializeField] private CoinCounterUI coinCounter;

    // 保留原有的其他变量
    private bool isSettingsMenuActive;

    // 创建一个只读属性，用于检查设置菜单状态
    public bool IsSettingsMenuActive => isSettingsMenuActive;

    protected override void Awake()
    {
        base.Awake();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 如果有任何事件监听器设置，确保在这里添加
    }

    public void IncreaseScore()
    {
        score++;
        coinCounter.UpdateScore(score);
    }

    // 这里添加了设置菜单相关函数，我们将在后面的步骤中使用
    private void ToggleSettingsMenu()
    {
        if (isSettingsMenuActive) DisableSettingsMenu();
        else EnableSettingsMenu();
    }

    private void EnableSettingsMenu()
    {
        Time.timeScale = 0f;
        // settingsMenu.SetActive(true); - 我们将在后面实现这部分
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isSettingsMenuActive = true;
    }

    private void DisableSettingsMenu()
    {
        Time.timeScale = 1f;
        // settingsMenu.SetActive(false); - 我们将在后面实现这部分
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isSettingsMenuActive = false;
    }

    // 保留其他原有的方法
}