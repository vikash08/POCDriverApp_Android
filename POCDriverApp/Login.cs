using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Graphics;
using Android.Content;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;


namespace POCDriverApp
{
    [Activity(MainLauncher = true,
               Icon = "@drawable/ic_launcher", Label = "@string/app_name",
               Theme = "@style/Theme.DesignDemo")]
    public class Login : AppCompatActivity
    {
        EditText LoginUsername, LoginPassword;
        String Login_Username, Login_Password;
        UserSessionManager session;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.LoginLayout);

            LoginUsername = FindViewById<EditText>(Resource.Id.input_email_login);
            LoginPassword = FindViewById<EditText>(Resource.Id.input_password_login);

            var LoginButton = FindViewById<Button>(Resource.Id.loginButton);

            session = new UserSessionManager(Application.Context);

            if (session.isUserLoggedIn())
            {
				var activitypickup = new Intent(this, typeof(BaseActivity));
				StartActivity(activitypickup);
                Finish();
			}

            LoginButton.Click += delegate
            {
                Login_Username = LoginUsername.Text.ToString();
                Login_Password = LoginPassword.Text.ToString();

                if (validate())
                {
                    processSignIn();
                }
            };
        }

        private Boolean validate()
        {
            var valid = true;

            if (String.IsNullOrEmpty(Login_Username))
            {
                LoginUsername.Error = "Enter a valid email address";
                valid = false;
            }
            else
            {
                LoginUsername.Error = null;
            }

            if (String.IsNullOrEmpty(Login_Password) || Login_Password.Length < 6)
            {
                LoginPassword.Error = "Password should be at least 6 characters";
                valid = false;
            }
            else
            {
                LoginPassword.Error = null;
            }

            return valid;
        }

        private void processSignIn()
        {
            session.createUserLoginSession(Login_Username, "Bikramjit S", "bikram@tcs.com", Login_Password);

			var activitypickup = new Intent(this, typeof(BaseActivity));
			StartActivity(activitypickup);
			Finish();
        }
    }
}