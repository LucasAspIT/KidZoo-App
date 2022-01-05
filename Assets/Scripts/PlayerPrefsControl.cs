using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsControl : MonoBehaviour
{
    private static PlayerPrefsControl instance;

    public static PlayerPrefsControl Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }


    public void UseLoginInformation()
    {
        if (PlayerPrefs.GetInt("SavedEmailBool", 1) == 1)
        {
            FirebaseManager.Instance.emailLoginField.text = PlayerPrefs.GetString("SavedEmail", "");
            FirebaseManager.Instance.saveLoginEmail.isOn = true;
        }
        if (PlayerPrefs.GetInt("SavedPasswordBool", 1) == 1)
        {
            FirebaseManager.Instance.passwordLoginField.text = PlayerPrefs.GetString("SavedPassword", ""); // ########### THIS IS PROBABLY UNSAFE, BUT FINE FOR AN INTERNAL PRACTICE APP ONLY.
            FirebaseManager.Instance.saveLoginPassword.isOn = true;
        }
    }

    public void SaveLoginInformation()
    {
        if (PlayerPrefs.GetInt("SavedEmailBool", 1) == 1)
        {
            PlayerPrefs.SetString("SavedEmail", FirebaseManager.Instance.emailLoginField.text);
        }
        else
        {
            PlayerPrefs.SetString("SavedEmail", "");
        }

        if (PlayerPrefs.GetInt("SavedPasswordBool", 1) == 1)
        {
            PlayerPrefs.SetString("SavedPassword", FirebaseManager.Instance.passwordLoginField.text); // ########### THIS IS PROBABLY UNSAFE, BUT FINE FOR AN INTERNAL PRACTICE APP ONLY.
        }
        else
        {
            PlayerPrefs.SetString("SavedPassword", "");
        }
    }
}
