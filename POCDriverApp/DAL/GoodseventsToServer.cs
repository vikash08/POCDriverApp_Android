
#define OFFLINE_SYNC_ENABLED

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;


#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
#endif

namespace POCDriverApp.DAL
{
    class GoodseventsToServer
    {
        // Client reference.
        private MobileServiceClient client;
        
#if OFFLINE_SYNC_ENABLED
        private IMobileServiceSyncTable<Goodsevent> todoTable;

        const string localDbFilename = "localstore.db";
#else
        private IMobileServiceTable<Goodsevent> todoTable;
#endif

        // Adapter to map the items list to the view
       // private GoodseventAdapter adapter;

        
        // URL of the mobile app backend.
        //const string applicationURL = @"https://pocdriverapp.azurewebsites.net";
        const string applicationURL = @"https://driverapptester.azurewebsites.net";

        // Define a authenticated user.
        //  private MobileServiceUser user;

        // Vikash Notification
        // Create a new instance field for this activity.
     

        
        // Return the Mobile Services client.
        public MobileServiceClient CurrentClient
        {
            get
            {
                return client;
            }
        }

        public  async void onScan(string consignmentitemnumber, string eventcode)
        {
            
            CurrentPlatform.Init();

            // Create the client instance, using the mobile app backend URL.
            client = new MobileServiceClient(applicationURL);

            
#if OFFLINE_SYNC_ENABLED
            await InitLocalStoreAsync();

            // Get the sync table instance to use to store Goodsevent rows.
            todoTable = client.GetSyncTable<Goodsevent>();
#else
            todoTable = client.GetTable<Goodsevent>();
#endif


            // Create an adapter to bind the items with the view
            //adapter = new GoodseventAdapter(this, Resource.Layout.Row_List_To_Do);

            // Load the items from the mobile app backend.
            //OnRefreshItemsSelected();

            //  IsPlayServicesAvailable();
            AddItem(consignmentitemnumber, eventcode);
        }

        

#if OFFLINE_SYNC_ENABLED
        private async Task InitLocalStoreAsync()
        {
            var store = new MobileServiceSQLiteStore(localDbFilename);
            store.DefineTable<Goodsevent>();

            // Uses the default conflict handler, which fails on conflict
            // To use a different conflict handler, pass a parameter to InitializeAsync.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=521416
            await client.SyncContext.InitializeAsync(store);
        }

        private async Task SyncAsync(bool pullData = false)
        {
            try
            {
                await client.SyncContext.PushAsync();

                if (pullData)
                {
                    await todoTable.PullAsync("allGoodsevents", todoTable.CreateQuery()); // query ID is used for incremental sync
                }
            }
            catch (Java.Net.MalformedURLException)
            {
               // CreateAndShowDialog(new Exception("There was an error creating the Mobile Service. Verify the URL"), "Error");
            }
            catch (Exception e)
            {
              //  CreateAndShowDialog(e, "Error");
            }
        }
#endif

        //Initializes the activity menu
        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.activity_main, menu);
        //    return false;
        //}

       

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
            try
            {
                // Get the items that weren't marked as completed and add them in the adapter
              //  var list = await todoTable.Where(item => item.Complete == false).ToListAsync();

                //adapter.Clear();

                //foreach (Goodsevent current in list)
                //    adapter.Add(current);
            }
            catch (Exception e)
            {
               // CreateAndShowDialog(e, "Error");
            }
            finally
            {
              
            }
        }

//        public async Task CheckItem(Goodsevent item)
//        {
//            if (client == null)
//            {
//                return;
//            }

//            // Set the item as completed and update it in the table
//            item.Complete = true;
//            try
//            {
//                // Update the new item in the local store.
//                await todoTable.UpdateAsync(item);
//#if OFFLINE_SYNC_ENABLED
//                // Send changes to the mobile app backend.
//                await SyncAsync();
//#endif

//                if (item.Complete)
//                    adapter.Remove(item);

//            }
//            catch (Exception e)
//            {
//               // CreateAndShowDialog(e, "Error");
//            }
//        }

       
        [Java.Interop.Export()]
        public async void AddItem(string consignmentitemnumber, string eventcode)
        {

            //  Log.Debug(TAG, "InstanceID token: " + FirebaseInstanceId.Instance.Token);

            if (client == null || string.IsNullOrWhiteSpace(consignmentitemnumber))
            {
                return;
            }

            // Create a new item
            var item = new Goodsevent
            {
                Consignmentitemnumber = consignmentitemnumber,
                Eventcode  = eventcode
            };

            try
            {
                // Insert the new item into the local store.
                await todoTable.InsertAsync(item);
#if OFFLINE_SYNC_ENABLED
                // Send changes to the mobile app backend.
                await SyncAsync();
#endif

                //if (!item.Complete)
                //{
                //    adapter.Add(item);
                //}
            }
            catch (Exception e)
            {
               // CreateAndShowDialog(e, "Error");
            }

           // textNewToDo.Text = "";
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

        //private void CreateAndShowDialog(Exception exception, String title)
        //{
        //    CreateAndShowDialog(exception.Message, title);
        //}

        //private void CreateAndShowDialog(string message, string title)
        //{
        //    Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);

        //    builder.SetMessage(message);
        //    builder.SetTitle(title);
        //    builder.Create().Show();
        //}
    }
}