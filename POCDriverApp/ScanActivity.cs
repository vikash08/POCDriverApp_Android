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
using Android.Graphics;
using POCDriverApp.Utility;

namespace POCDriverApp
{
    public class ScanActivity : Fragment
    {
        // EditText containing the "New ToDo" text
        private EditText textNewToDo;
        private Button scanItem;
        private LinearLayout ll;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
         var  view = inflater.Inflate(Resource.Layout.Activity_PickUp, null);

            MobileBarcodeScanner.Initialize(this.Activity.Application);

            textNewToDo = view.FindViewById<EditText>(Resource.Id.textNewToDo);
			scanItem = view.FindViewById<Button>(Resource.Id.buttonscanItem);
            ll = view.FindViewById<LinearLayout>(Resource.Id.llactivity_pickup);

			scanItem.Click += (sender, args) =>
			{
                ScanItem();
			};

			return view;
        }

        [Java.Interop.Export()]
        public async void ScanItem()
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();
            bool isValid = false;

            if (result != null)
            {
              isValid = validateGoods(result.ToString());
                textNewToDo.Text = result.Text;

            }
            if (isValid)

            {
                ShowToast("Item Validated Succesfully", true,true);
            }
            else
            {
                ShowToast("Scanning Abort ! Item Not Valid", false,true);
            }
           // ShowToast("Item Validated Succesfully");
            // Console.WriteLine("Scanned Barcode: " + result.Text);

        }

        public void ShowToast(string text,bool isvalid , bool IsLengthShort = false)
        {
            Handler mainHandler = new Handler(Looper.MainLooper);
            Java.Lang.Runnable runnableToast = new Java.Lang.Runnable(() =>
            {
                //var duration = IsLengthShort ? ToastLength.Short : ToastLength.Long;

                //Toast.MakeText(this.Context, text, duration).Show();
                try
                { 
                Color col = Color.White;
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

        public bool validateGoods(string Item)
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

                    if (result.Contains("true"))
                    {
                        return true;
                    }
                    return false;
                }

            }
            catch (Exception e)
            {
                return false;
            }
        }

    }
}