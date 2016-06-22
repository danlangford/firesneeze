using Facebook.Unity;
using GooglePlayGames;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

public class PlayFabLoginCalls
{
    [CompilerGenerated]
    private static Action<bool> <>f__am$cache13;
    [CompilerGenerated]
    private static PlayFabClientAPI.GetAccountInfoCallback <>f__am$cache14;
    private static string android_id = string.Empty;
    private static bool android_success = false;
    public static string Email;
    private static string ios_id = string.Empty;
    public static UserAccountInfo LoggedInUserInfo;
    public static LoginPathways LoginMethodUsed;
    public static readonly string OverrideEmail = string.Empty;
    public static readonly string OverridePassword = string.Empty;
    public static readonly string OverrideUsername = string.Empty;
    public static string Password;
    private static string playfab_id = string.Empty;
    private static string token;
    public static string Username;

    public static  event FailedLoginHandler OnLoginFail;

    public static  event SuccessfulLoginHandler OnLoginSuccess;

    public static  event CallbackSuccess OnPlayfabCallbackSuccess;

    public static  event PlayFabErrorHandler OnPlayFabError;

    public static  event StartSpinner StartSpinnerRequest;

    public static void AddUserNameAndPassword(string user, string pass, string email)
    {
        Debug.Log("AddUserNameAndPassword(user : " + user + ", pass : " + pass + ", email : " + email + ")");
        RequestSpinner();
        AddUsernamePasswordRequest request = new AddUsernamePasswordRequest {
            Email = email,
            Password = pass,
            Username = user
        };
        PlayFabClientAPI.AddUsernamePassword(request, new PlayFabClientAPI.AddUsernamePasswordCallback(PlayFabLoginCalls.OnAddUserNameAndPasswordSuccess), new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
    }

    private static void CallFBLogin()
    {
        Debug.Log("CallFBLogin()");
        List<string> permissions = new List<string> { 
            "public_profile",
            "email",
            "user_friends"
        };
        FB.LogInWithReadPermissions(permissions, new FacebookDelegate<ILoginResult>(PlayFabLoginCalls.FBLoginCallback));
    }

    private static void CallFBLogout()
    {
        FB.LogOut();
    }

    public static bool CanUseService(LoginPathways path)
    {
        switch (path)
        {
            case LoginPathways.deviceId:
                return CheckForSupportedMobilePlatform();

            case LoginPathways.pf_username:
                return true;

            case LoginPathways.facebook:
                return CheckForSupportedMobilePlatform();

            case LoginPathways.gameCenter:
                if ((Application.platform != RuntimePlatform.OSXPlayer) && (Application.platform != RuntimePlatform.IPhonePlayer))
                {
                    break;
                }
                return true;

            case LoginPathways.pf_email:
                return true;

            case LoginPathways.steam:
                if (((Application.platform != RuntimePlatform.LinuxPlayer) && (Application.platform != RuntimePlatform.OSXPlayer)) && (Application.platform != RuntimePlatform.WindowsPlayer))
                {
                    break;
                }
                return true;
        }
        return false;
    }

    public static bool CheckForSupportedMobilePlatform()
    {
        if ((Application.platform != RuntimePlatform.Android) && (Application.platform != RuntimePlatform.IPhonePlayer))
        {
            return false;
        }
        return true;
    }

    public static void FBLoginCallback(IResult result)
    {
        if (result.Error != null)
        {
            if (OnPlayFabError != null)
            {
                OnPlayFabError("Facebook Error: " + result.Error, PlayFabAPIMethods.Generic);
            }
        }
        else if (!FB.IsLoggedIn)
        {
            if (OnPlayFabError != null)
            {
                OnPlayFabError("Facebook Error: Login cancelled by Player", PlayFabAPIMethods.Generic);
            }
        }
        else
        {
            LoginWithFacebook(AccessToken.CurrentAccessToken.TokenString);
        }
    }

    public static void GetAccountInfo(PlayFabClientAPI.GetAccountInfoCallback callback = null)
    {
        GetAccountInfoRequest request = new GetAccountInfoRequest();
        if (callback != null)
        {
            PlayFabClientAPI.GetAccountInfo(request, callback, new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
        }
        else
        {
            PlayFabClientAPI.GetAccountInfo(request, new PlayFabClientAPI.GetAccountInfoCallback(PlayFabLoginCalls.OnGetAccountInfoSuccess), new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
        }
    }

    private static bool GetDeviceId()
    {
        if (CheckForSupportedMobilePlatform())
        {
            AndroidJavaClass class2 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject obj3 = class2.GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getContentResolver", new object[0]);
            AndroidJavaClass class3 = new AndroidJavaClass("android.provider.Settings$Secure");
            object[] args = new object[] { obj3, "android_id" };
            android_id = class3.CallStatic<string>("getString", args);
            return true;
        }
        if (OnPlayFabError != null)
        {
            OnPlayFabError("Must be using android or ios platforms to use deveice id.", PlayFabAPIMethods.Generic);
        }
        return false;
    }

    public static void LinkDeviceId()
    {
        Debug.Log("LinkDeviceId()");
        if (GetDeviceId())
        {
            RequestSpinner();
            if (!string.IsNullOrEmpty(android_id))
            {
                Debug.Log("Linking Android");
                LinkAndroidDeviceIDRequest request = new LinkAndroidDeviceIDRequest {
                    AndroidDeviceId = android_id
                };
                PlayFabClientAPI.LinkAndroidDeviceID(request, new PlayFabClientAPI.LinkAndroidDeviceIDCallback(PlayFabLoginCalls.OnLinkAndroidDeviceIdSuccess), new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
            }
            else if (!string.IsNullOrEmpty(ios_id))
            {
                Debug.Log("Linking iOS");
                LinkIOSDeviceIDRequest request2 = new LinkIOSDeviceIDRequest {
                    DeviceId = ios_id
                };
                PlayFabClientAPI.LinkIOSDeviceID(request2, new PlayFabClientAPI.LinkIOSDeviceIDCallback(PlayFabLoginCalls.OnLinkIosDeviceIdSuccess), new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
            }
        }
    }

    public static void LinkFBAccount()
    {
        Debug.Log("LinkFBAccount()");
        if (!FB.IsLoggedIn)
        {
            Debug.Log("FB is not logged in, calling StartFacebookLogin() now.");
            StartFacebookLogin();
        }
        else
        {
            Debug.Log("FB is logged in, attempting to link now.");
            LinkFacebookAccountRequest request = new LinkFacebookAccountRequest {
                AccessToken = AccessToken.CurrentAccessToken.TokenString
            };
            PlayFabClientAPI.LinkFacebookAccount(request, new PlayFabClientAPI.LinkFacebookAccountCallback(PlayFabLoginCalls.OnLinkFbIdSuccess), new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
        }
    }

    public static void LinkGameCenterAccount()
    {
        Debug.Log("Linking Game Center Account..");
    }

    public static void LoginWithDeviceId(bool createAccount)
    {
        if (GetDeviceId())
        {
            LoginMethodUsed = LoginPathways.deviceId;
            if (!string.IsNullOrEmpty(android_id))
            {
                Debug.Log("Using Android Device ID: " + android_id);
                LoginWithAndroidDeviceIDRequest request = new LoginWithAndroidDeviceIDRequest {
                    AndroidDeviceId = android_id,
                    TitleId = PlayFabSettings.TitleId,
                    CreateAccount = new bool?(createAccount)
                };
                PlayFabClientAPI.LoginWithAndroidDeviceID(request, new PlayFabClientAPI.LoginWithAndroidDeviceIDCallback(PlayFabLoginCalls.OnDeviceIdLoginSucceed), new ErrorCallback(PlayFabLoginCalls.OnDeviceIdLoginFail), null);
            }
            else if (!string.IsNullOrEmpty(ios_id))
            {
                Debug.Log("Using IOS Device ID: " + ios_id);
                LoginWithIOSDeviceIDRequest request2 = new LoginWithIOSDeviceIDRequest {
                    DeviceId = ios_id,
                    TitleId = PlayFabSettings.TitleId,
                    CreateAccount = new bool?(createAccount)
                };
                PlayFabClientAPI.LoginWithIOSDeviceID(request2, new PlayFabClientAPI.LoginWithIOSDeviceIDCallback(PlayFabLoginCalls.OnDeviceIdLoginSucceed), new ErrorCallback(PlayFabLoginCalls.OnDeviceIdLoginFail), null);
            }
        }
    }

    public static void LoginWithDeviceIdAndLinkFBAccount()
    {
        if (GetDeviceId())
        {
            LoginMethodUsed = LoginPathways.deviceId;
            if (!string.IsNullOrEmpty(android_id))
            {
                Debug.Log("Using Android Device ID: " + android_id);
                LoginWithAndroidDeviceIDRequest request = new LoginWithAndroidDeviceIDRequest {
                    AndroidDeviceId = android_id,
                    TitleId = PlayFabSettings.TitleId,
                    CreateAccount = true
                };
                PlayFabClientAPI.LoginWithAndroidDeviceID(request, new PlayFabClientAPI.LoginWithAndroidDeviceIDCallback(PlayFabLoginCalls.OnDeviceIdLoginSucceedLinkFB), new ErrorCallback(PlayFabLoginCalls.OnDeviceIdLoginFail), null);
            }
            else if (!string.IsNullOrEmpty(ios_id))
            {
                Debug.Log("Using IOS Device ID: " + ios_id);
                LoginWithIOSDeviceIDRequest request2 = new LoginWithIOSDeviceIDRequest {
                    DeviceId = ios_id,
                    TitleId = PlayFabSettings.TitleId,
                    CreateAccount = true
                };
                PlayFabClientAPI.LoginWithIOSDeviceID(request2, new PlayFabClientAPI.LoginWithIOSDeviceIDCallback(PlayFabLoginCalls.OnDeviceIdLoginSucceedLinkFB), new ErrorCallback(PlayFabLoginCalls.OnDeviceIdLoginFail), null);
            }
        }
    }

    public static void LoginWithEmail(string user, string password)
    {
        if ((user.Length > 0) && (password.Length > 0))
        {
            LoginMethodUsed = LoginPathways.pf_email;
            LoginWithEmailAddressRequest request = new LoginWithEmailAddressRequest {
                Email = user,
                Password = password,
                TitleId = PlayFabSettings.TitleId
            };
            PlayFabClientAPI.LoginWithEmailAddress(request, new PlayFabClientAPI.LoginWithEmailAddressCallback(PlayFabLoginCalls.OnLoginResult), new ErrorCallback(PlayFabLoginCalls.OnLoginError), null);
        }
        else if (OnPlayFabError != null)
        {
            OnLoginFail("Username or Password is invalid. Check credentials and try again");
        }
    }

    private static void LoginWithFacebook(string token)
    {
        Debug.Log("LoginWithFacebook(" + token + ")");
        LoginMethodUsed = LoginPathways.facebook;
        LoginWithFacebookRequest request = new LoginWithFacebookRequest {
            AccessToken = token,
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = false
        };
        PlayFabClientAPI.LoginWithFacebook(request, new PlayFabClientAPI.LoginWithFacebookCallback(PlayFabLoginCalls.OnLoginResult), new ErrorCallback(PlayFabLoginCalls.OnLoginError), null);
    }

    private static void LoginWithGameCenter(string token, bool createAccount)
    {
        PlayFabSettings.LoginMethod = string.Empty;
        LoginMethodUsed = LoginPathways.gameCenter;
        LoginWithGameCenterRequest request = new LoginWithGameCenterRequest {
            PlayerId = token,
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = new bool?(createAccount)
        };
        PlayFabClientAPI.LoginWithGameCenter(request, new PlayFabClientAPI.LoginWithFacebookCallback(PlayFabLoginCalls.OnGameCenterLogin), new ErrorCallback(PlayFabLoginCalls.OnLoginError), null);
    }

    public static void LoginWithGoogle(string token, bool createAccount)
    {
        Debug.Log("LoginWithGoogle(" + token + ")");
        LoginMethodUsed = LoginPathways.googlePlus;
        LoginWithGoogleAccountRequest request = new LoginWithGoogleAccountRequest {
            AccessToken = token,
            CreateAccount = new bool?(createAccount)
        };
        PlayFabClientAPI.LoginWithGoogleAccount(request, new PlayFabClientAPI.LoginWithGoogleAccountCallback(PlayFabLoginCalls.OnLoginResult), new ErrorCallback(PlayFabLoginCalls.OnLoginError), null);
    }

    public static void LoginWithGooglePlayGames(string token, bool createAccount)
    {
        PlayFabSettings.LoginMethod = string.Empty;
        LoginMethodUsed = LoginPathways.googlePlayGames;
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest {
            CustomId = token,
            TitleId = PlayFabSettings.TitleId,
            CreateAccount = new bool?(createAccount)
        };
        PlayFabClientAPI.LoginWithCustomID(request, new PlayFabClientAPI.LoginWithCustomIDCallback(PlayFabLoginCalls.OnGooglePlayGamesLogin), new ErrorCallback(PlayFabLoginCalls.OnLoginError), null);
    }

    public static void LoginWithUsername(string user, string password)
    {
        if ((user.Length > 0) && (password.Length > 0))
        {
            LoginMethodUsed = LoginPathways.pf_username;
            LoginWithPlayFabRequest request = new LoginWithPlayFabRequest {
                Username = user,
                Password = password,
                TitleId = PlayFabSettings.TitleId
            };
            PlayFabClientAPI.LoginWithPlayFab(request, new PlayFabClientAPI.LoginWithPlayFabCallback(PlayFabLoginCalls.OnLoginResult), new ErrorCallback(PlayFabLoginCalls.OnLoginError), null);
        }
        else if (OnPlayFabError != null)
        {
            GuiPanelLogin.SetMessage("User Name and Password cannot be blank.");
            GuiPanelCreateAccount.SetMessage("User Name and Password cannot be blank.");
            OnLoginFail("User Name and Password cannot be blank.");
        }
    }

    public static void Logout()
    {
        Debug.Log("Logout()");
        if (OnLoginFail != null)
        {
            OnLoginFail("Logout");
        }
        android_id = string.Empty;
        ios_id = string.Empty;
        PlayFabClientAPI.Logout();
        Game.Network.CurrentUser.Reset();
        PlayerPrefs.SetInt("PlayFabFacebook", 0);
        PlayerPrefs.SetInt("PlayFabManual", 0);
        PlayerPrefs.SetString("PlayFabManualEmail", string.Empty);
        PlayerPrefs.SetString("PlayFabManualPassword", string.Empty);
        PlayerPrefs.Save();
        for (int i = 0; i < LicenseTable.Count; i++)
        {
            LicenseManager.RevokeLicense(LicenseTable.Key(i));
        }
        Collection.Clear();
        if (FB.IsInitialized && FB.IsLoggedIn)
        {
            CallFBLogout();
        }
    }

    private static void OnAddDataError(PlayFabError error)
    {
        Game.Network.UpdateNetworkConnection(error.ErrorMessage);
        Debug.Log(string.Concat(new object[] { "Add data error: ", error.Error, " ", error.ErrorMessage }));
    }

    private static void OnAddDataSuccess(UpdateUserDataResult result)
    {
    }

    private static void OnAddUserNameAndPasswordSuccess(AddUsernamePasswordResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("OnAddUserNameAndPasswordSuccess(result)");
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(string.Empty, PlayFabAPIMethods.AddUsernamePassword);
        }
    }

    private static void OnDeviceIdLoginFail(PlayFabError error)
    {
        Game.Network.UpdateNetworkConnection(error.ErrorMessage);
    }

    private static void OnDeviceIdLoginSucceed(PlayFab.ClientModels.LoginResult result)
    {
        Game.Network.UpdateNetworkConnection();
        if ((((((LoggedInUserInfo == null) || (LoggedInUserInfo.PrivateInfo == null)) || ((LoggedInUserInfo.PrivateInfo.Email == null) || (LoggedInUserInfo.PrivateInfo.Email.Length <= 0))) && (LoggedInUserInfo != null)) && ((LoggedInUserInfo.FacebookInfo == null) || (((LoggedInUserInfo.FacebookInfo.FacebookId == null) || (LoggedInUserInfo.FacebookInfo.FacebookId.Length <= 0)) && ((LoggedInUserInfo.FacebookInfo.FullName == null) || (LoggedInUserInfo.FacebookInfo.FullName.Length <= 0))))) && (((LoggedInUserInfo.PrivateInfo == null) || (LoggedInUserInfo.PrivateInfo.Email == null)) || (LoggedInUserInfo.PrivateInfo.Email.Length <= 0)))
        {
        }
    }

    private static void OnDeviceIdLoginSucceedLinkFB(PlayFab.ClientModels.LoginResult result)
    {
        Game.Network.UpdateNetworkConnection();
        if (<>f__am$cache14 == null)
        {
            <>f__am$cache14 = delegate (GetAccountInfoResult info) {
                LoggedInUserInfo = info.AccountInfo;
                LinkFBAccount();
            };
        }
        GetAccountInfo(<>f__am$cache14);
    }

    private static void OnGameCenterLogin(PlayFab.ClientModels.LoginResult result)
    {
        PlayFabSettings.LoginMethod = "Game Center";
        GuiWindow current = GuiWindow.Current;
        if ((current != null) && (current is GuiWindowMainMenu))
        {
            GuiWindowMainMenu menu = current as GuiWindowMainMenu;
            menu.accountLabel.Text = PlayFabSettings.LoginMethod;
        }
        OnLoginResult(result);
    }

    private static void OnGetAccountInfoSuccess(GetAccountInfoResult result)
    {
        Game.Network.UpdateNetworkConnection();
        LoggedInUserInfo = result.AccountInfo;
        GuiWindow current = GuiWindow.Current;
        if ((current != null) && (current is GuiWindowMainMenu))
        {
            GuiWindowMainMenu menu = current as GuiWindowMainMenu;
            if (LoggedInUserInfo != null)
            {
                menu.accountLabel.Text = "Tap to Connect";
                if ((LoggedInUserInfo.PlayFabId != null) && (LoggedInUserInfo.PlayFabId.Length > 0))
                {
                    menu.accountLabel.Text = "PlayFab - " + LoggedInUserInfo.PlayFabId;
                }
                if ((LoggedInUserInfo.Username != null) && (LoggedInUserInfo.Username.Length > 0))
                {
                    menu.accountLabel.Text = "PlayFab - " + LoggedInUserInfo.Username;
                }
                if (((LoggedInUserInfo.PrivateInfo != null) && (LoggedInUserInfo.PrivateInfo.Email != null)) && (LoggedInUserInfo.PrivateInfo.Email.Length > 0))
                {
                    menu.accountLabel.Text = LoggedInUserInfo.PrivateInfo.Email;
                }
                if (((LoggedInUserInfo.FacebookInfo != null) && (LoggedInUserInfo.FacebookInfo.FullName != null)) && (LoggedInUserInfo.FacebookInfo.FullName.Length > 0))
                {
                    menu.accountLabel.Text = "Facebook - " + LoggedInUserInfo.FacebookInfo.FullName;
                }
                if (LoginMethodUsed == LoginPathways.gameCenter)
                {
                    menu.accountLabel.Text = "Logged in to GameCenter";
                }
                if (LoginMethodUsed == LoginPathways.googlePlayGames)
                {
                    menu.accountLabel.Text = "Logged in to GooglePlayGames";
                }
            }
        }
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(string.Empty, PlayFabAPIMethods.GetAccountInfo);
        }
    }

    private static void OnGooglePlayGamesLogin(PlayFab.ClientModels.LoginResult result)
    {
        LoginMethodUsed = LoginPathways.googlePlayGames;
        PlayFabSettings.LoginMethod = "Google Play Games";
        GuiWindow current = GuiWindow.Current;
        if ((current != null) && (current is GuiWindowMainMenu))
        {
            GuiWindowMainMenu menu = current as GuiWindowMainMenu;
            menu.accountLabel.Text = PlayFabSettings.LoginMethod;
        }
        OnLoginResult(result);
    }

    public static void OnHideUnity(bool isGameShown)
    {
        Debug.Log("OnHideUnity(" + isGameShown + ")");
        Time.timeScale = isGameShown ? ((float) 1) : ((float) 0);
    }

    public static void OnInitComplete()
    {
        Debug.Log("OnInitComplete()");
        if (!FB.IsLoggedIn)
        {
            CallFBLogin();
        }
        else
        {
            LoginWithFacebook(AccessToken.CurrentAccessToken.TokenString);
        }
    }

    private static void OnLinkAndroidDeviceIdSuccess(LinkAndroidDeviceIDResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("Linking Android Success!");
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(string.Empty, PlayFabAPIMethods.LinkAndroidDeviceID);
        }
        GetAccountInfo(null);
    }

    private static void OnLinkFbIdSuccess(LinkFacebookAccountResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("Successfully linked Facebook: " + result.ToString());
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(string.Empty, PlayFabAPIMethods.LinkFacebookId);
        }
        GetAccountInfo(null);
        GuiWindowMainMenu.ShowFacebookButton(false);
        UI.Busy = false;
    }

    private static void OnLinkGameCenterIdSuccess(LinkGameCenterAccountResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("Successfully linked GameCenter: " + result.ToString());
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(string.Empty, PlayFabAPIMethods.LinkGameCenterId);
        }
        GetAccountInfo(null);
    }

    private static void OnLinkGoogleIdSuccess(LinkGoogleAccountResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("Successfully linked Google: " + result.ToString());
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(string.Empty, PlayFabAPIMethods.LinkGameCenterId);
        }
        GetAccountInfo(null);
    }

    private static void OnLinkIosDeviceIdSuccess(LinkIOSDeviceIDResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("Linking iOS Success!");
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(string.Empty, PlayFabAPIMethods.LinkIOSDeviceID);
        }
        GetAccountInfo(null);
    }

    private static void OnLoginError(PlayFabError error)
    {
        Game.Network.UpdateNetworkConnection(error.ErrorMessage);
        string message = string.Empty;
        if ((error.Error == PlayFabErrorCode.InvalidParams) && error.ErrorDetails.ContainsKey("Password"))
        {
            GuiPanelLogin.SetMessage("Invalid Password");
            GuiPanelCreateAccount.SetMessage("Invalid Password");
            message = "Invalid Password";
        }
        else if (((error.Error == PlayFabErrorCode.InvalidParams) && error.ErrorDetails.ContainsKey("Username")) || (error.Error == PlayFabErrorCode.InvalidUsername))
        {
            GuiPanelLogin.SetMessage("Invalid Username");
            GuiPanelCreateAccount.SetMessage("Invalid Username");
            message = "Invalid Username";
        }
        else if (error.Error == PlayFabErrorCode.AccountNotFound)
        {
            GuiPanelLogin.SetMessage("Account Not Found, you must have a linked PlayFab account. Start by registering a new account or using your device id");
            GuiPanelCreateAccount.SetMessage("Account Not Found, you must have a linked PlayFab account. Start by registering a new account or using your device id");
            message = "Account Not Found, you must have a linked PlayFab account. Start by registering a new account or using your device id";
            switch (error.Origin)
            {
                case PlayFabError.OriginType.Facebook:
                    LinkFBAccount();
                    break;

                case PlayFabError.OriginType.GameCenter:
                    LinkGameCenterAccount();
                    break;
            }
        }
        else if (error.Error == PlayFabErrorCode.AccountBanned)
        {
            GuiPanelLogin.SetMessage("Account Banned");
            GuiPanelCreateAccount.SetMessage("Account Banned");
            message = "Account Banned";
        }
        else if (error.Error == PlayFabErrorCode.InvalidUsernameOrPassword)
        {
            GuiPanelLogin.SetMessage("Invalid Username or Password");
            GuiPanelCreateAccount.SetMessage("Invalid Username or Password");
            message = "Invalid Username or Password";
        }
        else
        {
            GuiPanelLogin.SetMessage($"Error {error.HttpCode}: {error.ErrorMessage}");
            GuiPanelCreateAccount.SetMessage($"Error {error.HttpCode}: {error.ErrorMessage}");
            message = $"Error {error.HttpCode}: {error.ErrorMessage}";
        }
        if (OnLoginFail != null)
        {
            OnLoginFail(message);
        }
        ErrorStatus = "OnLoginError(" + message + ")";
        Debug.Log(string.Concat(new object[] { "OnLoginError[", (int) error.Error, "](", message, ")" }));
        android_id = string.Empty;
        ios_id = string.Empty;
    }

    private static void OnLoginResult(PlayFab.ClientModels.LoginResult result)
    {
        Game.Network.UpdateNetworkConnection();
        GuiWindow current = GuiWindow.Current;
        if (current != null)
        {
            GuiWindowMainMenu menu = current as GuiWindowMainMenu;
            if (menu != null)
            {
                if ((result.Origin == PlayFab.ClientModels.LoginResult.OriginType.PlayFab) || (result.Origin == PlayFab.ClientModels.LoginResult.OriginType.Default))
                {
                    menu.accountLabel.Text = "PlayFab";
                }
                if (result.Origin == PlayFab.ClientModels.LoginResult.OriginType.Facebook)
                {
                    menu.accountLabel.Text = "Facebook";
                }
                menu.loginButton.Show(false);
                if ((menu.loginPanel != null) && menu.loginPanel.Visible)
                {
                    menu.loginPanel.Show(false);
                }
                if ((menu.createAccountPanel != null) && menu.createAccountPanel.Visible)
                {
                    menu.createAccountPanel.Show(false);
                }
                if ((menu.notLoggedInPanel != null) && menu.notLoggedInPanel.Visible)
                {
                    menu.notLoggedInPanel.Show(false);
                }
                if ((menu.statusPanel != null) && !menu.statusPanel.Visible)
                {
                    menu.statusPanel.Show(true);
                }
                menu.outOfDatePanel.Show(Game.Network.OutOfDate);
            }
        }
        GuiPanelLogin.SetMessage("Login successful!");
        GuiPanelCreateAccount.SetMessage("Login successful!");
        if (OnLoginSuccess != null)
        {
            android_success = true;
            OnLoginSuccess($"{result.SessionTicket}");
        }
        GetAccountInfo(null);
        Game.Network.OnLogin();
        switch (result.Origin)
        {
            case PlayFab.ClientModels.LoginResult.OriginType.Android:
                android_success = true;
                break;
        }
        UI.Busy = false;
    }

    private static void OnPhotonAuthenticationSuccess(GetPhotonAuthenticationTokenResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("Photon connected successfully!");
    }

    private static void OnPlayFabCallbackError(PlayFabError error)
    {
        Game.Network.UpdateNetworkConnection(error.ErrorMessage);
        string errorMessage = error.ErrorMessage;
        Debug.Log("OnPlayFabCallbackError(" + errorMessage + ")");
        if (OnPlayFabError != null)
        {
            OnPlayFabError(errorMessage, PlayFabAPIMethods.Generic);
        }
        UI.Busy = false;
    }

    private static void OnRegisterError(PlayFabError error)
    {
        Game.Network.UpdateNetworkConnection(error.ErrorMessage);
        Debug.Log("OnRegisterError: " + error.ErrorMessage);
        string errorMessage = error.ErrorMessage;
        if (errorMessage == "EmailAddressNotAvailable")
        {
            if ((OverrideEmail.Length > 0) && (OverridePassword.Length > 0))
            {
                LoginWithEmail(OverrideEmail, OverridePassword);
            }
            else
            {
                LoginWithEmail(Email, Password);
            }
        }
        else
        {
            Debug.Log("OnPlayFabCallbackError(" + errorMessage + ")");
        }
        if ((errorMessage.ToLower().Contains("resolve host") || errorMessage.ToLower().Contains("failed to connect")) || errorMessage.ToLower().Contains("503"))
        {
            GuiPanelLogin.SetMessage("Error: Unable to connect, please make sure you have a network connection!");
            GuiPanelCreateAccount.SetMessage("Error: Unable to connect, please make sure you have a network connection!");
        }
        else
        {
            GuiPanelLogin.SetMessage("OnRegisterError: " + errorMessage);
            GuiPanelCreateAccount.SetMessage("OnRegisterErorr: " + errorMessage);
        }
        if (OnPlayFabError != null)
        {
            OnPlayFabError(errorMessage, PlayFabAPIMethods.Generic);
        }
    }

    private static void OnRegisterResult(RegisterPlayFabUserResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("Registration Successful :)");
        if (OnPlayfabCallbackSuccess != null)
        {
            GuiPanelLogin.SetMessage("Registration Successful!");
            GuiPanelCreateAccount.SetMessage("Registration Successful!");
            OnPlayfabCallbackSuccess("Registration Successful!", PlayFabAPIMethods.RegisterPlayFabUser);
        }
        if ((OverrideUsername.Length > 0) && (OverridePassword.Length > 0))
        {
            LoginWithUsername(OverrideUsername, OverridePassword);
        }
        else
        {
            LoginWithUsername(Username, Password);
        }
    }

    private static void OnSendAccountRecoveryEmailSuccess(SendAccountRecoveryEmailResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("OnSendAccountRecoveryEmailSuccess(result)");
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(string.Empty, PlayFabAPIMethods.SendAccountRecoveryEmail);
        }
    }

    public static void OnTestDeviceIdHasAccountError(PlayFabError error)
    {
        Game.Network.UpdateNetworkConnection(error.ErrorMessage);
        Debug.Log("No account matches this device id.");
        android_id = string.Empty;
        ios_id = string.Empty;
        if ((OnPlayFabError != null) && (error.HttpCode == 0x3e9))
        {
            OnPlayFabError("No account matches this device id.", PlayFabAPIMethods.LoginWithDeviceId);
        }
    }

    private static void OnUnlinkAndroidDeviceIdSuccess(UnlinkAndroidDeviceIDResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("Unlink Android Success!");
        android_id = string.Empty;
        ios_id = string.Empty;
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(string.Empty, PlayFabAPIMethods.UnlinkAndroidDeviceID);
        }
        GetAccountInfo(null);
    }

    private static void OnUnlinkFbIdSuccess(UnlinkFacebookAccountResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("OnUnlinkFbIdSuccess(result)");
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(string.Empty, PlayFabAPIMethods.UnlinkFacebookId);
        }
        GetAccountInfo(null);
    }

    private static void OnUnlinkGameCenterIdSuccess(UnlinkGameCenterAccountResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("OnUnlinkGameCenterIdSuccess(result)");
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(string.Empty, PlayFabAPIMethods.UnlinkGameCenterId);
        }
        GetAccountInfo(null);
    }

    private static void OnUnlinkGoogleIdSuccess(UnlinkGoogleAccountResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("OnUnlinkGoogleIdSuccess(result)");
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(string.Empty, PlayFabAPIMethods.UnlinkGameCenterId);
        }
        GetAccountInfo(null);
    }

    private static void OnUnlinkIosDeviceIdSuccess(UnlinkIOSDeviceIDResult result)
    {
        Game.Network.UpdateNetworkConnection();
        Debug.Log("Unlink iOS Success!");
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(string.Empty, PlayFabAPIMethods.UnlinkIOSDeviceID);
        }
        GetAccountInfo(null);
    }

    private static void OnUpdateDisplayName(UpdateUserTitleDisplayNameResult result)
    {
    }

    private static void OnUpdateDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Game.Network.UpdateNetworkConnection();
        LoggedInUserInfo.TitleInfo.DisplayName = result.DisplayName;
        if (OnPlayfabCallbackSuccess != null)
        {
            OnPlayfabCallbackSuccess(result.DisplayName, PlayFabAPIMethods.UpdateDisplayName);
        }
    }

    public static void PersistentGameCenterLogin()
    {
        LoginWithGameCenter(PlayerPrefs.GetString("PlayFabGameCenterID"), true);
    }

    public static void PersistentGPGLogin()
    {
        LoginWithGooglePlayGames(PlayerPrefs.GetString("PlayFabGPGID"), true);
    }

    public static void RegisterNewPlayfabAccount(string user, string pass1, string pass2, string email)
    {
        if (((user.Length == 0) || (pass1.Length == 0)) || ((pass2.Length == 0) || (email.Length == 0)))
        {
            GuiPanelLogin.SetMessage("All fields are required.");
            GuiPanelCreateAccount.SetMessage("All fields are required.");
            if (OnPlayFabError != null)
            {
                OnPlayFabError("All fields are required.", PlayFabAPIMethods.RegisterPlayFabUser);
            }
        }
        else if (!ValidatePassword(pass1, pass2))
        {
            GuiPanelLogin.SetMessage("Passwords must match and be longer than 5 characters.");
            GuiPanelCreateAccount.SetMessage("Passwords must match and be longer than 5 characters.");
            if (OnPlayFabError != null)
            {
                OnPlayFabError("Passwords must match and be longer than 5 characters.", PlayFabAPIMethods.RegisterPlayFabUser);
            }
        }
        else
        {
            Username = user;
            Email = email;
            Password = pass1;
            RegisterPlayFabUserRequest request = new RegisterPlayFabUserRequest {
                TitleId = PlayFabSettings.TitleId,
                Username = user,
                Email = email,
                Password = pass1
            };
            PlayFabClientAPI.RegisterPlayFabUser(request, new PlayFabClientAPI.RegisterPlayFabUserCallback(PlayFabLoginCalls.OnRegisterResult), new ErrorCallback(PlayFabLoginCalls.OnRegisterError), null);
        }
    }

    public static void RequestSpinner()
    {
        if (StartSpinnerRequest != null)
        {
            StartSpinnerRequest();
        }
    }

    public static void SendAccountRecoveryEmail(string email)
    {
        Debug.Log("SendAccountRecoveryEmail(" + email + ")");
        RequestSpinner();
        SendAccountRecoveryEmailRequest request = new SendAccountRecoveryEmailRequest {
            Email = email,
            TitleId = PlayFabSettings.TitleId
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, new PlayFabClientAPI.SendAccountRecoveryEmailCallback(PlayFabLoginCalls.OnSendAccountRecoveryEmailSuccess), new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
    }

    public static void SetPersistentGameCenterLogin(string id)
    {
        PlayerPrefs.SetInt("PlayFabGameCenter", 1);
        PlayerPrefs.SetString("PlayFabGameCenterID", id);
    }

    public static void SetPersistentGPGLogin(string id)
    {
        PlayerPrefs.SetInt("PlayFabGPG", 1);
        PlayerPrefs.SetString("PlayFabGPGID", id);
    }

    public static void StartDebugLogin()
    {
        Username = new Regex("[^a-zA-Z0-9]").Replace(Environment.MachineName, string.Empty);
        if (Username.Length > 20)
        {
            Username = Username.Substring(0, 20);
        }
        Password = string.Empty;
        Email = string.Empty;
        char[] chArray = Username.ToCharArray();
        for (int i = chArray.Length - 1; i >= 0; i--)
        {
            Password = Password + chArray[i];
        }
        for (int j = 0; j < chArray.Length; j++)
        {
            if (!char.IsLetter(chArray[j]))
            {
                break;
            }
            Email = Email + chArray[j];
        }
        Email = Email + "@Obsidian.net";
        Debug.Log("Username: " + Username + ", Password: " + Password + ", Email: " + Email);
        if (((OverrideUsername.Length > 0) && (OverridePassword.Length > 0)) && (OverrideEmail.Length > 0))
        {
            RegisterNewPlayfabAccount(OverrideUsername, OverridePassword, OverridePassword, OverrideEmail);
        }
        else
        {
            RegisterNewPlayfabAccount(Username, Password, Password, Email);
        }
    }

    public static void StartFacebookLogin()
    {
        FB.Init(new InitDelegate(PlayFabLoginCalls.OnInitComplete), new HideUnityDelegate(PlayFabLoginCalls.OnHideUnity), null);
    }

    public static void StartGameCenterLogin()
    {
        if (!Social.localUser.authenticated)
        {
            if (<>f__am$cache13 == null)
            {
                <>f__am$cache13 = delegate (bool success) {
                    if (success)
                    {
                        SetPersistentGameCenterLogin(Social.localUser.id);
                        LoginWithGameCenter(Social.localUser.id, true);
                    }
                    else
                    {
                        Debug.Log("Authentication failed");
                    }
                };
            }
            Social.localUser.Authenticate(<>f__am$cache13);
        }
        else
        {
            LoginWithGameCenter(Social.localUser.id, true);
        }
    }

    public static void StartGooglePlayGamesLogin()
    {
        <StartGooglePlayGamesLogin>c__AnonStorey123 storey = new <StartGooglePlayGamesLogin>c__AnonStorey123 {
            gpgp = PlayGamesPlatform.Activate()
        };
        if (storey.gpgp != null)
        {
            if (!storey.gpgp.IsAuthenticated())
            {
                storey.gpgp.Authenticate(new Action<bool>(storey.<>m__144));
            }
            else
            {
                LoginWithGooglePlayGames(storey.gpgp.GetUserId(), true);
            }
        }
    }

    public static void TestDeviceIdHasAccount()
    {
        if (GetDeviceId())
        {
            LoginMethodUsed = LoginPathways.deviceId;
            if (!string.IsNullOrEmpty(android_id))
            {
                Debug.Log("Testing Android Device ID: " + android_id);
                LoginWithAndroidDeviceIDRequest request = new LoginWithAndroidDeviceIDRequest {
                    AndroidDeviceId = android_id,
                    TitleId = PlayFabSettings.TitleId,
                    CreateAccount = false
                };
                PlayFabClientAPI.LoginWithAndroidDeviceID(request, new PlayFabClientAPI.LoginWithAndroidDeviceIDCallback(PlayFabLoginCalls.OnLoginResult), new ErrorCallback(PlayFabLoginCalls.OnTestDeviceIdHasAccountError), null);
            }
            else if (!string.IsNullOrEmpty(ios_id))
            {
                Debug.Log("Testing IOS Device ID: " + ios_id);
                LoginWithIOSDeviceIDRequest request2 = new LoginWithIOSDeviceIDRequest {
                    DeviceId = ios_id,
                    TitleId = PlayFabSettings.TitleId,
                    CreateAccount = false
                };
                PlayFabClientAPI.LoginWithIOSDeviceID(request2, new PlayFabClientAPI.LoginWithIOSDeviceIDCallback(PlayFabLoginCalls.OnLoginResult), new ErrorCallback(PlayFabLoginCalls.OnTestDeviceIdHasAccountError), null);
            }
        }
    }

    public static void UnlinkDeviceId()
    {
        Debug.Log("UnlinkDeviceId");
        if (GetDeviceId())
        {
            RequestSpinner();
            if (!string.IsNullOrEmpty(android_id))
            {
                Debug.Log("Unlinking Android");
                UnlinkAndroidDeviceIDRequest request = new UnlinkAndroidDeviceIDRequest();
                PlayFabClientAPI.UnlinkAndroidDeviceID(request, new PlayFabClientAPI.UnlinkAndroidDeviceIDCallback(PlayFabLoginCalls.OnUnlinkAndroidDeviceIdSuccess), new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
            }
            else if (!string.IsNullOrEmpty(ios_id))
            {
                Debug.Log("Unlinking iOS");
                UnlinkIOSDeviceIDRequest request2 = new UnlinkIOSDeviceIDRequest();
                PlayFabClientAPI.UnlinkIOSDeviceID(request2, new PlayFabClientAPI.UnlinkIOSDeviceIDCallback(PlayFabLoginCalls.OnUnlinkIosDeviceIdSuccess), new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
            }
        }
    }

    public static void UnlinkFBAccount()
    {
        Debug.Log("Unlinking FB Account..");
        UnlinkFacebookAccountRequest request = new UnlinkFacebookAccountRequest();
        PlayFabClientAPI.UnlinkFacebookAccount(request, new PlayFabClientAPI.UnlinkFacebookAccountCallback(PlayFabLoginCalls.OnUnlinkFbIdSuccess), new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
    }

    public static void UnlinkGameCenterAccount()
    {
        Debug.Log("UnlinkGameCenterAccount()");
        UnlinkGameCenterAccountRequest request = new UnlinkGameCenterAccountRequest();
        PlayFabClientAPI.UnlinkGameCenterAccount(request, new PlayFabClientAPI.UnlinkGameCenterAccountCallback(PlayFabLoginCalls.OnUnlinkGameCenterIdSuccess), new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
    }

    public static void UnlinkGoogleAccount()
    {
        Debug.Log("Unlinking Google Account..");
        UnlinkGoogleAccountRequest request = new UnlinkGoogleAccountRequest();
        PlayFabClientAPI.UnlinkGoogleAccount(request, new PlayFabClientAPI.UnlinkGoogleAccountCallback(PlayFabLoginCalls.OnUnlinkGoogleIdSuccess), new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
    }

    public static void UpdateDisplayName(string displayName)
    {
        Debug.Log("UpdateDisplayName(displayName : " + displayName + ")");
        if (displayName == LoggedInUserInfo.TitleInfo.DisplayName)
        {
            Debug.Log("Name remains the same, no updates needed.");
        }
        else if ((displayName.Length > 2) && (displayName.Length < 0x15))
        {
            RequestSpinner();
            UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest {
                DisplayName = displayName
            };
            PlayFabClientAPI.UpdateUserTitleDisplayName(request, new PlayFabClientAPI.UpdateUserTitleDisplayNameCallback(PlayFabLoginCalls.OnUpdateDisplayNameSuccess), new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
        }
        else if (OnPlayFabError != null)
        {
            OnPlayFabError("Display name must be between 3 and 20 characters", PlayFabAPIMethods.UpdateDisplayName);
        }
    }

    public static void UpdateUserDisplayName(string displayName)
    {
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest {
            DisplayName = displayName
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, new PlayFabClientAPI.UpdateUserTitleDisplayNameCallback(PlayFabLoginCalls.OnUpdateDisplayName), new ErrorCallback(PlayFabLoginCalls.OnPlayFabCallbackError), null);
    }

    public static bool ValidatePassword(string p1, string p2) => 
        ((p1 == p2) && (p1.Length > 5));

    public static string AndroidID =>
        android_id;

    public static string ErrorStatus
    {
        [CompilerGenerated]
        get => 
            <ErrorStatus>k__BackingField;
        [CompilerGenerated]
        set
        {
            <ErrorStatus>k__BackingField = value;
        }
    }

    public static string IOSID =>
        ios_id;

    public static string PlayFabID =>
        playfab_id;

    public static string Token
    {
        get => 
            token;
        set
        {
            if ((token == null) || !token.Equals(value))
            {
                token = value;
            }
        }
    }

    [CompilerGenerated]
    private sealed class <StartGooglePlayGamesLogin>c__AnonStorey123
    {
        internal PlayGamesPlatform gpgp;

        internal void <>m__144(bool success)
        {
            if (!success)
            {
                Debug.Log("Google Play Games FAILED TO AUTHENTICATE");
            }
            if (success)
            {
                PlayFabLoginCalls.SetPersistentGPGLogin(this.gpgp.GetUserId());
                PlayFabLoginCalls.LoginWithGooglePlayGames(this.gpgp.GetUserId(), true);
            }
        }
    }

    public delegate void CallbackSuccess(string message, PlayFabAPIMethods method);

    public delegate void FailedLoginHandler(string message);

    public enum LoginPathways
    {
        deviceId = 1,
        facebook = 3,
        gameCenter = 4,
        googlePlayGames = 8,
        googlePlus = 7,
        pf_email = 5,
        pf_username = 2,
        steam = 6
    }

    public delegate void PlayFabErrorHandler(string message, PlayFabAPIMethods method);

    public delegate void StartSpinner();

    public delegate void SuccessfulLoginHandler(string path);
}

