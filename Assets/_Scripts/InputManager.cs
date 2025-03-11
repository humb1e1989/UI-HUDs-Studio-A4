using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public UnityEvent<Vector2> OnMove = new();
    public UnityEvent OnJump = new();
    public UnityEvent OnDash = new();
    public UnityEvent OnSettingsMenu = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            OnSettingsMenu?.Invoke();
        }

        // 如果设置菜单激活，不处理其他输入
        if (GameManager.Instance.IsSettingsMenuActive) return;

        // 移动输入
        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) input.y += 1;
        if (Input.GetKey(KeyCode.S)) input.y -= 1;
        if (Input.GetKey(KeyCode.A)) input.x -= 1;
        if (Input.GetKey(KeyCode.D)) input.x += 1;

        if (input != Vector2.zero)
        {
            OnMove?.Invoke(input.normalized);
        }

        // 跳跃和冲刺输入
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJump?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnDash?.Invoke();
        }
    }
}