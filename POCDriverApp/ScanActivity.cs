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
namespace POCDriverApp
{
    public class ScanActivity : Fragment
    {
        // EditText containing the "New ToDo" text
        private EditText textNewToDo;
        private Button scanItem;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.Activity_PickUp, null);

            MobileBarcodeScanner.Initialize(this.Activity.Application);

            textNewToDo = view.FindViewById<EditText>(Resource.Id.textNewToDo);
			scanItem = view.FindViewById<Button>(Resource.Id.buttonscanItem);

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

            if (result != null)
            {
                textNewToDo.Text = result.Text;
            }
            // Console.WriteLine("Scanned Barcode: " + result.Text);
        }
    }
}