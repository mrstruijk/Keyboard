using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class KeyStrokeManager : MonoBehaviour
{
    [SerializeField] [Range(2, 10)] private int m_requiredKeyCodeLength = 4;
    [SerializeField] private Button[] m_keyCaps;
    [SerializeField] private Button m_backSpace;
    [SerializeField] private TextMeshProUGUI m_displayPhrase;
    [SerializeField] private Button m_ok;
    [SerializeField] private List<AudioClip> m_clips;
    
    public UnityEvent<string> KeyCodeEntered;

    private string[] _characters;

    private int _participantNumber = 0;
    private const string ClearedPhrase = "== Cleared ==";


    private void Awake()
    {
        _characters = new string[m_keyCaps.Length];
        m_displayPhrase.text = _participantNumber.ToString();
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
            keyCap.onClick.AddListener(() => AddCharacter(keyCap.GetComponentInChildren<TextMeshProUGUI>().text));
            keyCap.onClick.AddListener(() => PlayAudio(keyCap.transform.position));
        }

        m_backSpace.onClick.AddListener(RemoveCharacter);
        m_backSpace.onClick.AddListener(() => PlayAudio(m_backSpace.transform.position));
        m_ok.onClick.AddListener(() => PlayAudio(m_ok.transform.position));
    }


    private void PlayAudio(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(m_clips[Random.Range(0, m_clips.Count)], position);
    }


    private void RemoveCharacter()
    {
        m_backSpace.interactable = true;
        m_ok.interactable = false;

        // Remove the last digit by integer division
        _participantNumber /= 10;
        m_displayPhrase.text = _participantNumber.ToString();

        if (_participantNumber == 0)
        {
            m_backSpace.interactable = false;
        }
    }


    private void AddCharacter(string character)
    {
        int digit;

        // Check if the character is a valid digit
        if (int.TryParse(character, out digit))
        {
            // If the current participant number is less than the required length
            if (_participantNumber.ToString().Length < m_requiredKeyCodeLength)
            {
                _participantNumber = _participantNumber * 10 + digit; // Add the digit to the number
                m_displayPhrase.text = _participantNumber.ToString();
                m_backSpace.interactable = true; // Backspace is always interactable if there's at least one digit
            }
            else
            {
                // If the number is too long, show the ClearedPhrase and disable buttons
                m_displayPhrase.text = ClearedPhrase;
                _participantNumber = 0; // Reset the participant number
                m_backSpace.interactable = false;
                m_ok.interactable = false;

                return; // Exit early
            }
        }

        // Set OK button interactable only if the length matches the required length
        if (_participantNumber.ToString().Length == m_requiredKeyCodeLength)
        {
            m_ok.interactable = true;
        }
        else
        {
            m_ok.interactable = false;
        }
    }


    public void EnterKeyCode()
    {
        if (_participantNumber.ToString().Length == m_requiredKeyCodeLength)
        {
            KeyCodeEntered?.Invoke(_participantNumber.ToString());

            DisableAllButtons();
            UnsubscribeButtons();
        }
        else
        {
            Debug.LogWarning($"Participant number is not of required length {m_requiredKeyCodeLength}");
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
            keyCap.onClick.RemoveListener(() => AddCharacter(keyCap.GetComponentInChildren<TextMeshProUGUI>().text));
            keyCap.onClick.RemoveListener(() => PlayAudio(keyCap.transform.position));
        }

        m_backSpace.onClick.RemoveListener(RemoveCharacter);
        m_backSpace.onClick.RemoveListener(() => PlayAudio(m_backSpace.transform.position));
        m_ok.onClick.RemoveListener(() => PlayAudio(m_ok.transform.position));
    }


    /// <summary>
    ///     Needs to be done if there's spaces in the TextField component.
    /// </summary>
    [ContextMenu(nameof(ResetTextFieldToName))]
    private void ResetTextFieldToName()
    {
        foreach (var keyCap in m_keyCaps)
        {
            keyCap.GetComponentInChildren<TextMeshProUGUI>().text = keyCap.name;
        }
    }
}