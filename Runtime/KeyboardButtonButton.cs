using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class KeyboardButtonButton : MonoBehaviour
{
    [SerializeField] private Button _button;


    private void OnValidate()
    {
        if (_button != null)
        {
            return;
        }

        _button = GetComponent<Button>();
    }


    [ContextMenu(nameof(Click))]
    public void Click()
    {
        _button.onClick.Invoke();
    }
}