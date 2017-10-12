/*
 * To add Offline Sync Support:
 *  1) Add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore (and dependencies) to all client projects
 *  2) Uncomment the #define OFFLINE_SYNC_ENABLED
 *
 * For more information, see: http://go.microsoft.com/fwlink/?LinkId=717898
 */
#define OFFLINE_SYNC_ENABLED

using System;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Widget;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using POCDriverApp;
// VIKASH for adding Mobile Center
using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;

using V7Toolbar = Android.Support.V7.Widget.Toolbar;


// Vikash For Notification  (to check presence of google play service)
//using Android.Gms.Common;

//Vikash notification(messageing and observe trasaction from FMS)
//using Firebase.Messaging;
//using Firebase.Iid;
using Android.Util;
using Gcm.Client;
using Android.Content;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
#endif

namespace POCDriverApp
{
    [Activity(MainLauncher = false,
               Icon = "@drawable/ic_launcher", Label = "@string/app_name",
               Theme = "@style/Theme.DesignDemo")]
    public class ToDoActivity : BaseActivity
    {
        // Client reference.
        private MobileServiceClient client;
        TextView msgText;
        const string TAG = "NotificationActivity";
#if OFFLINE_SYNC_ENABLED
        private IMobileServiceSyncTable<ToDoItem> todoTable;

        const string localDbFilename = "localstore.db";
#else
        private IMobileServiceTable<ToDoItem> todoTable;
#endif

        // Adapter to map the items list to the view
        private ToDoItemAdapter adapter;

        // EditText containing the "New ToDo" text
        private EditText textNewToDo;

		// URL of the mobile app backend.
        const string applicationURL = @"https://pocdriverapp.azurewebsites.net";


        // Define a authenticated user.
        private MobileServiceUser user;

        // Vikash Notification
        // Create a new instance field for this activity.
        static ToDoActivity instance = new ToDoActivity();

        // Return the current activity instance.
        public static ToDoActivity CurrentActivity
        {
            get
            {
                return instance;
            }
        }
        // Return the Mobile Services client.
        public MobileServiceClient CurrentClient
        {
            get
            {
                return client;
            }
        }




        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //  Log.Debug(TAG, "google app id: " + Resource.String.google_app_id);


            //Task.Run(() => {
            //    var instanceid = FirebaseInstanceId.Instance;
            //    instanceid.DeleteInstanceId();
            //    Log.Debug("TAG", "{0} {1}", instanceid.Token, instanceid.GetToken(this.GetString(Resource.String.gcm_defaultSenderId), Firebase.Messaging.FirebaseMessaging.InstanceIdScope));
            //});


            // Vikash Sending analytics data to mobile center
            //MobileCenter.Start("8a1fe3f0-3e94-447a-87d3-58554bdf1477",
            //       typeof(Analytics), typeof(Crashes));

            // Set our view from the "main" layout resource
            //SetContentView(Resource.Layout.Activity_To_Do);

            var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);
           // SetSupportActionBar(toolbar);
            //FrameLayout contentFrameLayout = (FrameLayout)FindViewById(Resource.Layout.); //Remember this is the FrameLayout area within your activity_main.xml
            LayoutInflater inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
            inflater.Inflate(Resource.Layout.Activity_To_Do, toolbar);


            CurrentPlatform.Init();

            // Create the client instance, using the mobile app backend URL.
            client = new MobileServiceClient(applicationURL);

            // Vikash Notification 
            // Set the current instance of TodoActivity.
            instance = this;

            // Make sure the GCM client is set up correctly.
            GcmClient.CheckDevice(this);
            GcmClient.CheckManifest(this);

            // Register the app for push notifications.
            GcmClient.Register(this, ToDoBroadcastReceiver.senderIDs);
            // Vikash Notification end

#if OFFLINE_SYNC_ENABLED
            await InitLocalStoreAsync();

            // Get the sync table instance to use to store TodoItem rows.
            todoTable = client.GetSyncTable<ToDoItem>();
#else
            todoTable = client.GetTable<ToDoItem>();
#endif
        
            textNewToDo = FindViewById<EditText>(Resource.Id.textNewToDo);

            // Create an adapter to bind the items with the view
            adapter = new ToDoItemAdapter(this, Resource.Layout.Row_List_To_Do);
            var listViewToDo = FindViewById<ListView>(Resource.Id.listViewToDo);
            listViewToDo.Adapter = adapter;

            // Load the items from the mobile app backend.
            OnRefreshItemsSelected();
            msgText = FindViewById<TextView>(Resource.Id.msgText);

            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.GetString(key);
                    Log.Debug(TAG, "Key: {0} Value: {1}", key, value);
                }
            }

          //  IsPlayServicesAvailable();
        }

        //public bool IsPlayServicesAvailable()
        //{
        //    int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
        //    if (resultCode != ConnectionResult.Success)
        //    {
        //        if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
        //            msgText.Text = GoogleApiAvailability.Instance.GetErrorString(resultCode);
        //        else
        //        {
        //            msgText.Text = "This device is not supported";
        //            Finish();
        //        }
        //        return false;
        //    }
        //    else
        //    {
        //        msgText.Text = "Google Play Services is Available.";
        //        return true;
        //    }
        //}


#if OFFLINE_SYNC_ENABLED
        private async Task InitLocalStoreAsync()
        {
            var store = new MobileServiceSQLiteStore(localDbFilename);
            store.DefineTable<ToDoItem>();

            // Uses the default conflict handler, which fails on conflict
            // To use a different conflict handler, pass a parameter to InitializeAsync.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=521416
            await client.SyncContext.InitializeAsync(store);
        }

        private async Task SyncAsync(bool pullData = false)
        {
            try {
                await client.SyncContext.PushAsync();

                if (pullData) {
                    await todoTable.PullAsync("allTodoItems", todoTable.CreateQuery()); // query ID is used for incremental sync
                }
            }
            catch (Java.Net.MalformedURLException) {
                CreateAndShowDialog(new Exception("There was an error creating the Mobile Service. Verify the URL"), "Error");
            }
            catch (Exception e) {
                CreateAndShowDialog(e, "Error");
            }
        }
#endif

        //Initializes the activity menu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.activity_main, menu);
            return false;
        }

        //Select an option from the menu
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.menu_refresh) {
                item.SetEnabled(false);

                OnRefreshItemsSelected();

                item.SetEnabled(true);
            }
            return true;
        }

        // Called when the refresh menu option is selected.
        private async void OnRefreshItemsSelected()
        {
#if OFFLINE_SYNC_ENABLED
			// Get changes from the mobile app backend.
            await SyncAsync(pullData: true);
#endif
			// refresh view using local store.
            await RefreshItemsFromTableAsync();
        }

        //Refresh the list with the items in the local store.
        private async Task RefreshItemsFromTableAsync()
        {
            try {
                // Get the items that weren't marked as completed and add them in the adapter
                var list = await todoTable.Where(item => item.Complete == false).ToListAsync();

                adapter.Clear();

                foreach (ToDoItem current in list)
                    adapter.Add(current);

            }
            catch (Exception e) {
                CreateAndShowDialog(e, "Error");
            }
        }

        public async Task CheckItem(ToDoItem item)
        {
            if (client == null) {
                return;
            }

            // Set the item as completed and update it in the table
            item.Complete = true;
            try {
				// Update the new item in the local store.
                await todoTable.UpdateAsync(item);
#if OFFLINE_SYNC_ENABLED
                // Send changes to the mobile app backend.
				await SyncAsync();
#endif

                if (item.Complete)
                    adapter.Remove(item);

            }
            catch (Exception e) {
                CreateAndShowDialog(e, "Error");
            }
        }

        [Java.Interop.Export()]
        public  void PickUpItem(View view)
        {

            var activity2 = new Intent(this, typeof(ScanActivity));
            activity2.PutExtra("MyData", "Data from Activity1");
            StartActivity(activity2);

        }
        [Java.Interop.Export()]
        public async void AddItem(View view)
        {

          //  Log.Debug(TAG, "InstanceID token: " + FirebaseInstanceId.Instance.Token);

            if (client == null || string.IsNullOrWhiteSpace(textNewToDo.Text)) {
                return;
            }

            // Create a new item
            var item = new ToDoItem {
                Text = textNewToDo.Text,
                Complete = false
            };

            try {
				// Insert the new item into the local store.
                await todoTable.InsertAsync(item);
#if OFFLINE_SYNC_ENABLED
                // Send changes to the mobile app backend.
				await SyncAsync();
#endif

                if (!item.Complete) {
                    adapter.Add(item);
                }
            }
            catch (Exception e) {
                CreateAndShowDialog(e, "Error");
            }

            textNewToDo.Text = "";
        }



        //private async Task<bool> Authenticate()
        //{
        //    var success = false;
        //    try
        //    {
        //        // Sign in with Facebook login using a server-managed flow.
        //        user = await client.LoginAsync(this,MobileServiceAuthenticationProvider.Facebook);
        //        CreateAndShowDialog(string.Format("you are now logged in - {0}",
        //            user.UserId), "Logged in!");

        //        success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        CreateAndShowDialog(ex, "Authentication failed");
        //    }
        //    return success;
        //}

        //[Java.Interop.Export()]
        //public async void LoginUser(View view)
        //{
        //    // Load data only after authentication succeeds.
        //    if (await Authenticate())
        //    {
        //        //Hide the button after authentication succeeds.
        //        FindViewById<Button>(Resource.Id.buttonLoginUser).Visibility = ViewStates.Gone;

        //        // Load the data.
        //        OnRefreshItemsSelected();
        //    }
        //}

        private void CreateAndShowDialog(Exception exception, String title)
        {
            CreateAndShowDialog(exception.Message, title);
        }

        private void CreateAndShowDialog(string message, string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }
    }
}


