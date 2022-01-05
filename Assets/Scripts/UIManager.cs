using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject startScreen;

    [SerializeField]
    private GameObject userDashboard;

    [SerializeField]
    private GameObject loginUI;

    [SerializeField]
    private GameObject registerUI;

    public TMP_Text namedWelcomeMsg;


    public static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            return instance;
        }
    }

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

    /// <summary>
    /// Turn off all UI related to the login, register, user data and scoreboard.
    /// </summary>
    public void ClearScreen()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
    }

    /// <summary>
    /// Opens the login screen.
    /// </summary>
    public void LoginScreen()
    {
        ClearScreen();
        loginUI.SetActive(true);
    }

    /// <summary>
    /// Opens the register screen.
    /// </summary>
    public void RegisterScreen()
    {
        ClearScreen();
        registerUI.SetActive(true);
    }

    /// <summary>
    /// Enables the start screen.
    /// </summary>
    public void EnableStartScreen()
    {
        ClearScreen();
        startScreen.SetActive(true);
    }

    /// <summary>
    /// Disables the start screen.
    /// </summary>
    public void DisableStartScreen()
    {
        ClearScreen();
        startScreen.SetActive(false);
    }

    /// <summary>
    /// Enable the user dashboard.
    /// </summary>
    public void EnableUserDashboard()
    {
        ClearScreen();
        userDashboard.SetActive(true);
    }

    /// <summary>
    /// Disables the user dashboard.
    /// </summary>
    public void DisableUserDashboard()
    {
        ClearScreen();
        userDashboard.SetActive(true);
    }
}
