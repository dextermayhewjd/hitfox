using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;
     [SerializeField] int maxUsernameLength = 16;
    void Start()
    {
        // Set the character limit for the TMP_InputField.
        usernameInput.characterLimit = maxUsernameLength;

        // Register the validation callback.
        usernameInput.onValidateInput += ValidateInput;
        if(PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
        }
        else
        {
            usernameInput.text = "Player " + Random.Range(0,10000).ToString("0000");
            OnUsernameInputValueChanged();
        }
    }

    public void OnUsernameInputValueChanged()
    {
        PhotonNetwork.NickName = usernameInput.text;
        PlayerPrefs.SetString("username",usernameInput.text);
    }
    private char ValidateInput(string text, int charIndex, char addedChar)
    {
        // Additional validations can be added here, for example:
        // Restrict specific characters or enforce the use of only alphanumeric characters.

        return addedChar; // If all validations pass, return the input character.
    }
}
