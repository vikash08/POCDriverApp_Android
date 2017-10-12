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
using ZXing.Mobile;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
namespace POCDriverApp
{
    [Activity(MainLauncher = false,
                Icon = "@drawable/ic_launcher", Label = "@string/app_name",
                Theme = "@style/Theme.DesignDemo")]
    public class ScanActivity : BaseActivity
    {

        // EditText containing the "New ToDo" text
        private EditText textNewToDo;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

           // SetContentView(Resource.Layout.Activity_PickUp);
            var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);
            // SetSupportActionBar(toolbar);
            //FrameLayout contentFrameLayout = (FrameLayout)FindViewById(Resource.Layout.); //Remember this is the FrameLayout area within your activity_main.xml
            LayoutInflater inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
            inflater.Inflate(Resource.Layout.Activity_PickUp, toolbar);

            // Create your application here
            MobileBarcodeScanner.Initialize(Application);

           
        }

        [Java.Interop.Export()]
        public async void ScanItem(View view)
        {

            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();

            if (result != null)
            { 
                textNewToDo = FindViewById<EditText>(Resource.Id.textNewToDo);
                textNewToDo.Text = result.Text;
            }
            // Console.WriteLine("Scanned Barcode: " + result.Text);

        }

    }
}