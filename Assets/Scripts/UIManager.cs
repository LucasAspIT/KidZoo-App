using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField]
    private GameObject startScreen;

    // Screen object variables
    public GameObject loginUI;
    public GameObject registerUI;
    // public GameObject userDataUI;
    // public GameObject scoreboardUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    // Functions to change the login screen UI

    /// <summary>
    /// Turn off all UI related to the login, register, user data and scoreboard.
    /// </summary>
    public void ClearScreen()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        // userDataUI.SetActive(false);
        // scoreboardUI.SetActive(false);
    }

    public void LoginScreen() // Back button
    {
        ClearScreen();
        loginUI.SetActive(true);
    }

    public void RegisterScreen() // Register button
    {
        ClearScreen();
        registerUI.SetActive(true);
    }

    public void CloseStartScreen()
    {
        ClearScreen();
        startScreen.SetActive(false);
    }

/*
    public void UserDataScreen() // Logged in
    {
        ClearScreen();
        userDataUI.SetActive(true);
    }

    public void ScoreboardScreen() // Scoreboard button
    {
        ClearScreen();
        scoreboardUI.SetActive(true);
    }
*/

}
