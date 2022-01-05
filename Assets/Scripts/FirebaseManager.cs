using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class FirebaseManager : MonoBehaviour
{
    // Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;    
    public FirebaseUser User;
    public DatabaseReference DBreference;

    // Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;
    public Toggle saveLoginEmail;
    public Toggle saveLoginPassword;

    // Register variables
    [Header("Register")]
    public TMP_InputField fullNameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField birthdayRegisterField;
    public TMP_InputField streetnameRegisterField;
    public TMP_InputField cityRegisterField;
    public TMP_InputField zipcodeRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

/*
    // User Data variables
    [Header("UserData")]
    public TMP_InputField usernameField;
    public TMP_InputField highscoreField;
    public TMP_InputField totalScoreField;
    public TMP_InputField totalDeathsField;
    public GameObject scoreElement;
    public Transform scoreboardContent;
    private int curLoadedDBHighscore = 0;
    private int curLoadedDBTScore;
    private int curLoadedDBTDeaths;
*/

    private static FirebaseManager instance;
    public static FirebaseManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        // Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void Start()
    {
        PlayerPrefsControl.Instance.UseLoginInformation();
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        // Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    /// <summary>
    /// Clears the fields on the login UI.
    /// </summary>
    public void ClearLoginFields()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }

    /// <summary>
    /// Clears the text fields on the Register UI.
    /// </summary>
    public void ClearRegisterFields()
    {
        fullNameRegisterField.text = "";
        emailRegisterField.text = "";
        birthdayRegisterField.text = "";
        streetnameRegisterField.text = "";
        cityRegisterField.text = "";
        zipcodeRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    // Function for login button
    public void LoginButton()
    {
        SaveLoginToggleState();
        PlayerPrefsControl.Instance.SaveLoginInformation();
        // Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }

    // Function for the register button
    public void RegisterButton()
    {
        // Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, fullNameRegisterField.text, birthdayRegisterField.text, streetnameRegisterField.text, cityRegisterField.text, zipcodeRegisterField.text));
    }

    // Function for the sign out button
    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.instance.LoginScreen();
        ClearRegisterFields();
        ClearLoginFields();
    }

/*
    // Function for button that saves manually edited DB stats. Should be used for testing purposes
    public void SaveDataButton()
    {
        StartCoroutine(UpdateUsernameAuth(usernameField.text));
        StartCoroutine(UpdateUsernameDatabase(usernameField.text));

        StartCoroutine(UpdateHighscore(int.Parse(highscoreField.text)));
        StartCoroutine(UpdateTotalScore(int.Parse(totalScoreField.text)));
        StartCoroutine(UpdateTotalDeaths(int.Parse(totalDeathsField.text)));
    }
*/

    // Function for the play button
    public void PlayButton()
    {
        SceneManager.LoadScene("EndlessRunner");
    }

/*
    // Function for the scoreboard button
    public void ScoreboardButton()
    {
        StartCoroutine(LoadScoreboardData());
    }
*/

    public void SaveLoginToggleState()
    {
        if (saveLoginEmail.isOn)
        {
            PlayerPrefs.SetInt("SavedEmailBool", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SavedEmailBool", 0);
        }

        if (saveLoginPassword.isOn)
        {
            PlayerPrefs.SetInt("SavedPasswordBool", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SavedPasswordBool", 0);
        }
    }

    /// <summary>
    /// Sends the input information to Firebase to attempt a login.
    /// </summary>
    /// <param name="_email"></param>
    /// <param name="_password"></param>
    /// <returns></returns>
    private IEnumerator Login(string _email, string _password)
    {
        // Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        // Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            // If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            // User is now logged in
            // Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged in";
            // PlayerPrefs.SetString("SavedEmail", emailLoginField.text);
            // PlayerPrefs.SetString("SavedPassword", passwordLoginField.text); // ########### THIS IS PROBABLY UNSAFE, BUT FINE FOR AN INTERNAL PRACTICE APP ONLY.
            // SaveLoginToggleState();
            // StartCoroutine(LoadUserData());

            yield return new WaitForSeconds(2);

            // usernameField.text = User.DisplayName;
            // UIManager.instance.UserDataScreen();
            confirmLoginText.text = "";
            ClearLoginFields();
            ClearRegisterFields();
            UIManager.instance.CloseStartScreen();
        }
    }

    /// <summary>
    /// Sends the input information to Firebase for to attempt to register.
    /// </summary>
    /// <param name="_email"></param>
    /// <param name="_password"></param>
    /// <param name="_username"></param>
    /// <returns></returns>
    private IEnumerator Register(string _email, string _password, string _fullName, string _birthday, string _streetname, string _city, string _zipcode)
    {
        if (_fullName == "")
        {
            // If the fullname field is blank show a warning
            warningRegisterText.text = "Missing full name";
        }
        else if (_birthday == "")
        {
            // If the birthday field is blank show a warning
            warningRegisterText.text = "Missing birthday";
        }
        else if (_streetname == "")
        {
            // If the streetname field is blank show a warning
            warningRegisterText.text = "Missing street name";
        }
        else if (_city == "")
        {
            // If the city field is blank show a warning
            warningRegisterText.text = "Missing city";
        }
        else if (_zipcode == "")
        {
            // If the zipcode field is blank show a warning
            warningRegisterText.text = "Missing zipcode";
        }
        else if(passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            // If the password does not match show a warning
            warningRegisterText.text = "Password does not match!";
        }
        else 
        {
            // Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            // Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                // If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email already in use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                // User has now been created
                // Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    // Create a user profile and set the username
                    UserProfile profile = new UserProfile{DisplayName = _fullName};

                    // Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    // Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        // If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Full name set failed!";
                    }
                    else
                    {
                        // Set the initial database values
                        StartCoroutine(UpdateUsernameDatabase(_fullName));
                        StartCoroutine(SaveFullNameDatabase(_fullName));
                        StartCoroutine(SaveEmailDatabase(_email));
                        StartCoroutine(SaveBirthdayDatabase(_birthday));
                        StartCoroutine(SaveStreetnameDatabase(_streetname));
                        StartCoroutine(SaveCityDatabase(_city));
                        StartCoroutine(SaveZipcodeDatabase(_zipcode));
                        StartCoroutine(SaveRegisterDateDatabase());

                        // Username is now set
                        // Now return to login screen
                        UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                        ClearLoginFields();
                        ClearRegisterFields();
                    }
                }
            }
        }
    }

    private IEnumerator UpdateUsernameAuth(string _username)
    {
        // Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        // Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        // Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            // Auth username is now updated
        }
    }

    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("username").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database username is now updated
        }
    }

    // ###################################################### CURRENTLY WORKING ON THIS

    private IEnumerator SaveFullNameDatabase(string _fullName)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("fullname").SetValueAsync(_fullName);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database fullname is now registered.
        }
    }

    private IEnumerator SaveEmailDatabase(string _email)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("email").SetValueAsync(_email);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database email is now registered.
        }
    }

    private IEnumerator SaveBirthdayDatabase(string _birthday)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("birthday").SetValueAsync(_birthday);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database birthday is now registered.
        }
    }

    private IEnumerator SaveStreetnameDatabase(string _streetname)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("streetname").SetValueAsync(_streetname);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database streetname is now registered.
        }
    }

    private IEnumerator SaveCityDatabase(string _city)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("city").SetValueAsync(_city);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database city is now registered.
        }
    }

    private IEnumerator SaveZipcodeDatabase(string _zipcode)
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("zipcode").SetValueAsync(_zipcode);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database zipcode is now registered.
        }
    }

    private IEnumerator SaveRegisterDateDatabase()
    {
        // Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("registerdate").SetValueAsync(DateTime.Now.ToString("dd/MM/yyyy"));

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Database register date is now registered.
        }
    }

/*
    private IEnumerator UpdateHighscore(int _highscore)
    {
        // Only update the database if the highscore is actually a highscore
        if (_highscore > curLoadedDBHighscore || curLoadedDBHighscore == 0)
        {
            // Set the currently logged in user highscore
            var DBTask = DBreference.Child("users").Child(User.UserId).Child("highscore").SetValueAsync(_highscore);

            yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            }
            else
            {
                // Highscore is now updated
                Debug.Log("Updated DB Highscore.");
            }
        }
    }

    private IEnumerator UpdateTotalScore(int _totalScore)
    {
        // Add the score gained in the game to the total score
        _totalScore += curLoadedDBTScore;

        // Set the currently logged in user totalScore
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("totalScore").SetValueAsync(_totalScore);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Total score is now updated
            Debug.Log("Updated DB TotalScore.");
            curLoadedDBTScore = _totalScore;
        }
    }

    private IEnumerator UpdateTotalDeaths(int _totalDeaths)
    {
        // Add the death to the total deaths count
        _totalDeaths += curLoadedDBTDeaths;

        // Set the currently logged in user totalDeaths
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("totalDeaths").SetValueAsync(_totalDeaths);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Total deaths are now updated
            Debug.Log("Updated DB TotalDeaths.");
            curLoadedDBTDeaths = _totalDeaths;
        }
    }



    /// <summary>
    /// Starts the coroutines that take are of updating the database.
    /// <para>Highscore, total score and death count is sent.</para>
    /// </summary>
    /// <param name="_highscore"></param>
    /// <param name="_totalScore"></param>
    public void UpdateDatabaseUponDeath(int _highscore, int _totalScore)
    {
        StartCoroutine(UpdateHighscore(_highscore));
        StartCoroutine(UpdateTotalScore(_totalScore));
        StartCoroutine(UpdateTotalDeaths(1));

        Debug.Log("UpdateDatabaseUponDeath() called.");
    }

    /// <summary>
    /// In case the local highscore is higher than the database highscore, this can be fired.
    /// </summary>
    public void UpdateDatabaseHighscoreOnLoad(int _highscore)
    {
        StartCoroutine(UpdateHighscore(_highscore));
    }

    /// <summary>
    /// Loads the currently logged in user's data from the database.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadUserData()
    {
        // Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            // No data exists yet
            highscoreField.text = "0";
            totalScoreField.text = "0";
            totalDeathsField.text = "0";

            SaveDataButton();
        }
        else
        {
            // Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            // Save the info for use in the scoreboard, and for later when updating the DB
            highscoreField.text = snapshot.Child("highscore").Value.ToString();
            curLoadedDBHighscore = int.Parse(highscoreField.text);
            totalScoreField.text = snapshot.Child("totalScore").Value.ToString();
            curLoadedDBTScore = int.Parse(totalScoreField.text);
            totalDeathsField.text = snapshot.Child("totalDeaths").Value.ToString();
            curLoadedDBTDeaths = int.Parse(totalDeathsField.text);


            // LoadedData.Instance.Highscore = curLoadedDBHighscore;
        }
    }

    /// <summary>
    /// Loads the scoreboard data from the DB and displays it on the scoreboard element.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadScoreboardData()
    {
        // Get all the users data ordered by highscore
        var DBTask = DBreference.Child("users").OrderByChild("highscore").GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            // Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;

            // Destroy any existing scoreboard elements
            foreach (Transform child in scoreboardContent.transform)
            {
                Destroy(child.gameObject);
            }

            // Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                int highscore = int.Parse(childSnapshot.Child("highscore").Value.ToString());
                int totalScore = int.Parse(childSnapshot.Child("totalScore").Value.ToString());
                int totalDeaths = int.Parse(childSnapshot.Child("totalDeaths").Value.ToString());

                // Instantiate new scoreboard elements
                // GameObject scoreboardElement = Instantiate(scoreElement, scoreboardContent);
                // scoreboardElement.GetComponent<ScoreElement>().NewScoreElement(username, highscore, totalScore, totalDeaths);
            }

            // Go to scoreboard screen
            // UIManager.instance.ScoreboardScreen();
        }
    }

    */
}
