#define OFFLINE_SYNC_ENABLED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ZXing.Mobile;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.App;
using System.Net.Http;
using Newtonsoft.Json;
using Android.Support.Design.Widget;

using Android.Support.V4.Widget;

using POCDriverApp.Utility;
using POCDriverApp.DAL;
using System.Threading;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Xamarin.Controls;
using Android.Graphics;
using Java.IO;

namespace POCDriverApp
{
    public class ScanActivity : Fragment
    {
        // EditText containing the "New ToDo" text
        private EditText textNewToDo;
        private Button scanItem;
        private LinearLayout ll;
        private SignaturePadView signaturePad;

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

        Bitmap image;
        static ScanActivity instance = new ScanActivity();

        // Return the current activity instance.
        public static ScanActivity CurrentActivity
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

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
         var  view = inflater.Inflate(Resource.Layout.Activity_PickUp, null);


            CurrentPlatform.Init();

            // Create the client instance, using the mobile app backend URL.
            client = new MobileServiceClient(applicationURL);

            intialisetable();

            MobileBarcodeScanner.Initialize(this.Activity.Application);

            textNewToDo = view.FindViewById<EditText>(Resource.Id.textNewToDo);
			scanItem = view.FindViewById<Button>(Resource.Id.buttonscanItem);
            ll = view.FindViewById<LinearLayout>(Resource.Id.llactivity_pickup);
            ll = view.FindViewById<LinearLayout>(Resource.Id.llactivity_pickup);

            signaturePad = view.FindViewById<SignaturePadView>(Resource.Id.signaturePadView1);
            //var signature = new SignaturePadView(this.Context)
            //{
            //    StrokeWidth = 3f,
            //    StrokeColor = Color.White,
            //    BackgroundColor = Color.Black
            //};

            //Bitmap image = signature.GetImage();


            scanItem.Click += (sender, args) =>
			{
                ScanItem();
			};

           
            return view;
        }

        public async void intialisetable()
        {


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
            // AddItem(consignmentitemnumber, eventcode);

            await SyncAsync();

        }

        [Java.Interop.Export()]
        public async void ScanItem()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();
            string isValid = "false";

            if (result != null)
            {
              isValid = validateGoods(result.ToString());

                textNewToDo.Text = result.Text;

            }
            if (isValid.Contains("true"))

            {
                ShowToast("Item Validated Succesfully and Loaded", true,true);
                Thread.Sleep(2000);
                //SendEventstoServer(result.Text, "3");
                AddItem(result.Text, "I");
            }
            else if(isValid.Contains("false"))
            {
                ShowToast("Scanning Aborted ! Item Not Valid", false,true);
            }
            else
            {
                ShowToast("Offline!! Item Loaded ", false, false);
                AddItem(result.Text, "3");
            }
            // ShowToast("Item Validated Succesfully");
            // Console.WriteLine("Scanned Barcode: " + result.Text);
            
        }

        //private void SendEventstoServer(string item, string eventcode)
        //{

        //    GoodseventsToServer ges = new GoodseventsToServer();
        //    ges.onScan(item, eventcode);

        //}

        public void ShowToast(string text,bool isvalid , bool IsLengthShort = false)
        {
            bool isnotOffline = IsLengthShort;

            //if (!IsLengthShort)
            //{

            //}

            IsLengthShort = true;
            Handler mainHandler = new Handler(Looper.MainLooper);
            Java.Lang.Runnable runnableToast = new Java.Lang.Runnable(() =>
            {
                //var duration = IsLengthShort ? ToastLength.Short : ToastLength.Long;

                //Toast.MakeText(this.Context, text, duration).Show();
                try
                { 
                Color col = Color.White;
                    if(!isnotOffline)
                    {

                        Snackbar snackbar = Snackbar.Make(ll, text, Snackbar.LengthIndefinite).SetAction("Dismiss", action => { }).SetActionTextColor(col);
                        CustomisedSnackbar.warning(snackbar).Show();
                        return;
                    }

                if(isvalid)
                {
                    // col = Color.Green ;
                    Snackbar snackbar = Snackbar.Make(ll, text, Snackbar.LengthIndefinite).SetAction("Dismiss", action => { }).SetActionTextColor(col);
                    CustomisedSnackbar.confirm(snackbar).Show();
                }
                else
                {
                    // col = Color.Red;
                    Snackbar snackbar = Snackbar.Make(ll, text, Snackbar.LengthIndefinite).SetAction("Dismiss", action => { }).SetActionTextColor(col);
                    CustomisedSnackbar.alert(snackbar).Show();

                }
                }
                catch(Exception e)
                {
                    return;
                }

                //Snackbar.Make(ll, text, Snackbar.LengthIndefinite)
                //        .SetAction("Dismiss", action => { }).SetActionTextColor(col)
                //        .Show();

                // var snack = Snackbar.Make(view1, text, Snackbar.LengthIndefinite).Show();
                //.SetAction("Undo", (view) => { /*Undo message sending here.*/ }).Show();


            });

            mainHandler.Post(runnableToast);
        }


        // Call Validate goods API Call

        public string validateGoods(string Item)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //client.BaseAddress = new
                    //    Uri("http://driverapptester.azurewebsites.net/api/ValidateGoods");

                    string queryString = "http://driverapptester.azurewebsites.net/api/ValidateGoods?Item="+Item.Trim()+"&ZUMO-API-VERSION=2.0.0";
                    var result = client.GetStringAsync(queryString).Result;
                    // var json = client.GetStringAsync("/").Result;
                    //var results = JsonConvert.DeserializeObject<List<Contact>>(result);

                    if (result == null)
                        return "offline";
                    if (result.Contains("true"))
                    {
                        openAlert(result);

                        return "true";
                    }
                    return "false";
                }

            }
            catch (Exception e)
            {
                return "offline";
            }
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
                    await todoTable.PullAsync("allGoodsEvent", todoTable.CreateQuery()); // query ID is used for incremental sync
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


        // Called when the refresh menu option is selected.
        private async void OnRefreshItemsSelected()
        {
#if OFFLINE_SYNC_ENABLED
            // Get changes from the mobile app backend.
            await SyncAsync(pullData: true);

         
#endif
            // refresh view using local store.
           // await RefreshItemsFromTableAsync();
        }

        ////Refresh the list with the items in the local store.
        //private async Task RefreshItemsFromTableAsync()
        //{
        //    try
        //    {
        //        // Get the items that weren't marked as completed and add them in the adapter
        //        //  var list = await todoTable.Where(item => item.Complete == false).ToListAsync();

        //        //adapter.Clear();

        //        //foreach (Goodsevent current in list)
        //        //    adapter.Add(current);
        //    }
        //    catch (Exception e)
        //    {
        //        // CreateAndShowDialog(e, "Error");
        //    }
        //    finally
        //    {

        //    }
        //}
        [Java.Interop.Export()]
        private void openAlert(string result)
        {
            //set alert for executing the task
            AlertDialog.Builder alert = new AlertDialog.Builder(this.Context);
            alert.SetTitle("Validation response From Econnect");
            alert.SetMessage(result.Trim());
            alert.SetPositiveButton("OK", (senderAlert, args) => {
               // Toast.MakeText(this.Context, "Deleted!", ToastLength.Short).Show();
            });

            //alert.SetNegativeButton("Cancel", (senderAlert, args) => {
            //    Toast.MakeText(this.Context, "Cancelled!", ToastLength.Short).Show();
            //});

            AlertDialog dialog = alert.Create();
            dialog.Show();
        }


       
        private static byte[] GetBytesFromImage(Bitmap bitmap)
        {

            System.IO.MemoryStream byteArrayOutputStream = new System.IO.MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
            byte[] byteArray = byteArrayOutputStream.ToArray();
            return byteArray;
        }

        [Java.Interop.Export()]
        public async void AddItem(string consignmentitemnumber, string eventcode)
        {

            //  Log.Debug(TAG, "InstanceID token: " + FirebaseInstanceId.Instance.Token);

            if (client == null || string.IsNullOrWhiteSpace(consignmentitemnumber))
            {
                return;
            }

            Bitmap file = signaturePad.GetImage();
            string bitmapString = string.Empty;
            if (file!=null)
            { 
            byte[][] pic = new byte[1][];
            pic[0] = GetBytesFromImage(file);
             bitmapString = Convert.ToBase64String(pic[0], Base64FormattingOptions.InsertLineBreaks);
            }

            // Create a new item
            var item = new Goodsevent
            {
                Consignmentitemnumber = consignmentitemnumber,
                Eventcode = eventcode,
                Picture = bitmapString
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


        private void CreateAndShowDialog(Exception exception, String title)
        {
            CreateAndShowDialog(exception.Message, title);
        }

        private void CreateAndShowDialog(string message, string title)
        {
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this.Context);

            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }

    }
}