using System;
using Android.Content;
using Java.Util;

namespace POCDriverApp
{
    public class UserSessionManager
    {
        ISharedPreferences pref;
        ISharedPreferencesEditor editor;
        Context _context;
        int PRIVATE_MODE = 0;

        private static String PREFER_NAME = "ScanningAppPref";
        private static String IS_USER_LOGIN = "IsUserLoggedIn";

        public static String TAG_pwd = "pwd";
        public static String TAG_uuid = "uuid";
        public static String TAG_mail = "mail";
        public static String TAG_name = "name";

        public UserSessionManager(Context context)
        {
            this._context = context;
            pref = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(_context);
            editor = pref.Edit();
        }

        public void createUserLoginSession(String uId, String name, String mail, String uPwd)
        {
            editor.PutBoolean(IS_USER_LOGIN, true);

            editor.PutString(TAG_uuid, uId);
            editor.PutString(TAG_name, name);
            editor.PutString(TAG_mail, mail);
            editor.PutString(TAG_pwd, uPwd);

            editor.Commit();
        }

        public HashMap getUserDetails()
        {
            var user = new HashMap();

            user.Put(TAG_uuid, pref.GetString(TAG_uuid, null));
            user.Put(TAG_pwd, pref.GetString(TAG_pwd, null));
            user.Put(TAG_mail, pref.GetString(TAG_mail, null));
            user.Put(TAG_name, pref.GetString(TAG_name, null));

            return user;
        }

        public void logoutUser()
        {
            //perform signout operation

            editor.Clear();
            editor.Commit();
        }

        public Boolean isUserLoggedIn()
        {
            return pref.GetBoolean(IS_USER_LOGIN, false);
        }
    }
}
