using System;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Auth;
using firebasesample.iOS.Services.FirebaseAuth;
using firebasesample.Services.FirebaseAuth;
using Foundation;
using UIKit;
using Xamarin.Auth;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseAuthService))]
namespace firebasesample.iOS.Services.FirebaseAuth
{
    public class FirebaseAuthService : IFirebaseAuthService
    {
        public static String KEY_AUTH = "auth";
        public static OAuth2Authenticator XAuth;
        private static bool hasLoginResult = false;
        private static bool loginResult = false;
        private static bool signUpResult = false;
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken token;
        Task t;

        public IntPtr Handle => throw new NotImplementedException();

        public string getAuthKey()
        {
            return KEY_AUTH;
        }

        public bool IsUserSigned()
        {
            var user = Auth.DefaultInstance.CurrentUser;
            return user != null;
        }

        public async Task<bool> Logout()
        {
            NSError error;
            var signedOut = Auth.DefaultInstance.SignOut(out error);

            if (!signedOut)
            {              
                return false;
            }

            return true;
        }

        //LOGIN USER/PASS
        public async Task<bool> SignIn(string email, string password)
        {            
            Auth.DefaultInstance.SignIn(email, password, HandleAuthResultLoginHandler);
            token = tokenSource.Token;
            t = Task.Factory.StartNew(async () =>
            {
                await Task.Delay(4000);
            }, token).Unwrap();
            await t;
            
            
            return loginResult;
        }

        private void HandleAuthResultLoginHandler(User user, Foundation.NSError error)
        {
            if (error != null)
            {
                loginResult = false;
                hasLoginResult = true;
            }
            else
            {
                loginResult = true;
                hasLoginResult = true;
            }
            tokenSource.Cancel();
        }                      

        //COMPROBAR TOKEN LOGIN GOOGLE
        public async Task<bool> SignInWithGoogle(string tokenId)
        {
            String[] tokens = tokenId.Split(new string[] { "###" }, StringSplitOptions.None);
            var credential = GoogleAuthProvider.GetCredential(tokens[0], tokens[1]);
            Auth.DefaultInstance.SignIn(credential, HandleAuthResultHandlerGoogleSignin);
            token = tokenSource.Token;
            t = Task.Factory.StartNew(async () =>
            {
                await Task.Delay(4000);
            }, token).Unwrap();
            await t;

            return loginResult;
        }


        private void HandleAuthResultHandlerGoogleSignin(User user, NSError error)
        {
            if (error != null)
            {
                loginResult = false;
                hasLoginResult = true;
            }
            else
            {
                loginResult = true;
                hasLoginResult = true;
            }

            tokenSource.Cancel();
        }

        // LOGIN GOOLE
        public void SignInWithGoogle()
        {
            XAuth = new OAuth2Authenticator(
                clientId: "719081152163-odlthbmghah3m60ji4bt5dg3o1ghg089.apps.googleusercontent.com",
                clientSecret: "",
                scope: "profile",
                authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/v2/auth"),
                redirectUrl: new Uri("com.googleusercontent.apps.719081152163-odlthbmghah3m60ji4bt5dg3o1ghg089:/oauth2redirect"),
                accessTokenUrl: new Uri("https://www.googleapis.com/oauth2/v4/token"),
                isUsingNativeUI: true);
            
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            XAuth.Completed += OnAuthenticationCompleted;

            XAuth.Error += OnAuthenticationFailed;

            var viewController = XAuth.GetUI();
            vc.PresentViewController(viewController, true, null);
        }

        private void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            vc.DismissViewController(true, null);

            if (e.IsAuthenticated)
            {
                var access_token = e.Account.Properties["access_token"].ToString();
                var id_token = e.Account.Properties["id_token"].ToString();

                MessagingCenter.Send(FirebaseAuthService.KEY_AUTH, FirebaseAuthService.KEY_AUTH, id_token + "###" + access_token);
            }
            else
            {
                //Error
            }
        }

        private void OnAuthenticationFailed(object sender, AuthenticatorErrorEventArgs e)
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            vc.DismissViewController(true, null);
        }

        // REGISTRO USUARIO
        public async Task<bool> SignUp(string email, string password)
        {
            Auth.DefaultInstance.CreateUser(email, password,HandleAuthResultHandlerSignUp);
            token = tokenSource.Token;
            t = Task.Factory.StartNew(async () =>
            {
                await Task.Delay(4000);
            }, token).Unwrap();
            await t;
            return signUpResult;
        }

        private void HandleAuthResultHandlerSignUp(User user, Foundation.NSError error)
        {
            if (error != null)
            {
                signUpResult = false;
                hasLoginResult = true;
            }
            else
            {
                signUpResult = true;
                hasLoginResult = true;
            }
            tokenSource.Cancel();
        }
             
        public string GetUserId()
        {
            var user = Auth.DefaultInstance.CurrentUser;
            return user.Uid;
        }
    }
}
