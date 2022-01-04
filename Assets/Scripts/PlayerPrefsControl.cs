using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsControl : MonoBehaviour
{
    public bool SaveEmail;
    public bool SavePassword;

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


        if (PlayerPrefs.GetInt("SavedEmailBool", 1) == 1)
        {
            SaveEmail = true;
        }
        if (PlayerPrefs.GetInt("SavedPasswordBool", 1) == 1)
        {
            SavePassword = true;
        }
    }


    public void UseLoginInformation()
    {
        if (SaveEmail)
        {
            FirebaseManager.Instance.emailLoginField.text = PlayerPrefs.GetString("SavedEmail", "");
        }
        if (SavePassword)
        {
            FirebaseManager.Instance.passwordLoginField.text = PlayerPrefs.GetString("SavedPassword", ""); // ########### THIS IS PROBABLY UNSAFE, BUT FINE FOR AN INTERNAL PRACTICE APP ONLY.
        }
    }

    public void SaveLoginInformation()
    {
        if (SaveEmail)
        {
            PlayerPrefs.SetString("SavedEmail", FirebaseManager.Instance.emailLoginField.text);
        }
        else
        {
            PlayerPrefs.SetString("SavedEmail", "");
        }

        if (SavePassword)
        {
            PlayerPrefs.SetString("SavedPassword", FirebaseManager.Instance.passwordLoginField.text); // ########### THIS IS PROBABLY UNSAFE, BUT FINE FOR AN INTERNAL PRACTICE APP ONLY.
        }
        else
        {
            PlayerPrefs.SetString("SavedPassword", "");
        }
    }
}
