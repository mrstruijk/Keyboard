using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class KeyStrokeManager : MonoBehaviour
{
    [SerializeField] [Range(2, 20)] private int m_requiredKeyCodeLength = 4;
    [SerializeField] private AllowedCharacterType m_allowedCharacterType = AllowedCharacterType.Digits;
    [SerializeField] private TextMeshProUGUI m_displayPhrase;
    [SerializeField] private Button[] m_keyCaps;
    [SerializeField] private Button m_ok;
    [SerializeField] private Button m_backSpace;
    [SerializeField] private List<AudioClip> m_audioClips;
    [SerializeField] private AudioClip m_failedAudio;

    public UnityEvent<string> KeyCodeEntered;

    private string[] _characters;
    private string _keyCode = "";
    private const string ClearedPhrase = "== Cleared ==";


    private void Awake()
    {
        _characters = new string[m_keyCaps.Length];
        m_displayPhrase.text = _keyCode; // Display keycode initially as empty
    }


    private void OnEnable()
    {
        GetButtonContent();
        AddButtonsClickListener();
    }


    private void GetButtonContent()
    {
        for (var i = 0; i < m_keyCaps.Length; i++)
        {
            _characters[i] = m_keyCaps[i].GetComponentInChildren<TextMeshProUGUI>().text;
        }
    }


    private void AddButtonsClickListener()
    {
        foreach (var keyCap in m_keyCaps)
        {
            keyCap.onClick.AddListener(() => OnKeyPressed(keyCap));
        }

        m_backSpace.onClick.AddListener(RemoveCharacter);
        m_backSpace.onClick.AddListener(() => PlayAudio(m_backSpace.transform.position));
        m_ok.onClick.AddListener(() => PlayAudio(m_ok.transform.position));
    }


    private void OnKeyPressed(Button keyCap)
    {
        var character = keyCap.GetComponentInChildren<TextMeshProUGUI>().text;

        if (IsCharacterValid(character))
        {
            AddCharacter(character);
            PlayAudio(keyCap.transform.position);
        }
        else
        {
            PlayFailedAudio(keyCap.transform.position);
        }
    }


    private void PlayAudio(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(m_audioClips[Random.Range(0, m_audioClips.Count)], position);
    }


    private void PlayFailedAudio(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(m_failedAudio, position);
    }


    private void RemoveCharacter()
    {
        m_backSpace.interactable = true;
        m_ok.interactable = false;

        // Remove the last character
        _keyCode = _keyCode.Substring(0, _keyCode.Length - 1);
        m_displayPhrase.text = _keyCode;

        if (_keyCode.Length == 0)
        {
            m_backSpace.interactable = false;
        }
    }


    private void AddCharacter(string character)
    {
        if (_keyCode.Length < m_requiredKeyCodeLength)
        {
            _keyCode += character; // Add character to keycode string
            m_displayPhrase.text = _keyCode;
            m_backSpace.interactable = true;
        }
        else
        {
            m_displayPhrase.text = ClearedPhrase;
            _keyCode = ""; // Reset keycode
            m_backSpace.interactable = false;
            m_ok.interactable = false;

            return;
        }

        m_ok.interactable = _keyCode.Length == m_requiredKeyCodeLength;
    }


    private bool IsCharacterValid(string character)
    {
        var isValid = false;

        switch (m_allowedCharacterType)
        {
            case AllowedCharacterType.Letters:
                isValid = char.IsLetter(character[0]);

                break;

            case AllowedCharacterType.Digits:
                isValid = char.IsDigit(character[0]);

                break;

            case AllowedCharacterType.LettersAndDigits:
                isValid = char.IsLetterOrDigit(character[0]);

                break;
        }

        return isValid;
    }


    public void EnterKeyCode()
    {
        if (_keyCode.Length == m_requiredKeyCodeLength)
        {
            KeyCodeEntered?.Invoke(_keyCode);
            DisableAllButtons();
            UnsubscribeButtons();
        }
        else
        {
            Debug.LogWarning($"Keycode is not of required length {m_requiredKeyCodeLength}");
        }
    }


    private void DisableAllButtons()
    {
        foreach (var keyCap in m_keyCaps)
        {
            keyCap.interactable = false;
        }

        m_ok.interactable = false;
        m_backSpace.interactable = false;
    }


    private void UnsubscribeButtons()
    {
        foreach (var keyCap in m_keyCaps)
        {
            keyCap.onClick.RemoveListener(() => OnKeyPressed(keyCap));
        }

        m_backSpace.onClick.RemoveListener(RemoveCharacter);
        m_backSpace.onClick.RemoveListener(() => PlayAudio(m_backSpace.transform.position));
        m_ok.onClick.RemoveListener(() => PlayAudio(m_ok.transform.position));
    }


    [ContextMenu(nameof(ResetTextFieldToName))]
    private void ResetTextFieldToName()
    {
        foreach (var keyCap in m_keyCaps)
        {
            keyCap.GetComponentInChildren<TextMeshProUGUI>().text = keyCap.name;
        }
    }
}


public enum AllowedCharacterType
{
    Letters,
    Digits,
    LettersAndDigits
}