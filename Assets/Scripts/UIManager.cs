using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject startScreen;

    [SerializeField]
    private GameObject loginUI;

    [SerializeField]
    private GameObject registerUI;


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
    /// Open the login screen.
    /// </summary>
    public void LoginScreen()
    {
        ClearScreen();
        loginUI.SetActive(true);
    }

    /// <summary>
    /// Open the register screen.
    /// </summary>
    public void RegisterScreen()
    {
        ClearScreen();
        registerUI.SetActive(true);
    }

    /// <summary>
    /// Close the start screen.
    /// </summary>
    public void CloseStartScreen()
    {
        ClearScreen();
        startScreen.SetActive(false);
    }
}
