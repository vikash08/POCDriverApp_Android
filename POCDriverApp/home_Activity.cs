using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.App;
using Android.Support.Design.Widget;

namespace POCDriverApp
{
    public class home_Activity : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.home_layout, null);

            var img = view.FindViewById<ImageButton>(Resource.Id.imageButton);
            var img2 = view.FindViewById<ImageButton>(Resource.Id.imageButton2);
            var img3 = view.FindViewById<ImageButton>(Resource.Id.imageButton3);
            var img4 = view.FindViewById<ImageButton>(Resource.Id.imageButton4);

            img2.Click += delegate
            {
                var activitypickup = new Intent(this.Activity, typeof(ToDoActivity));
                activitypickup.PutExtra("MyData", "Data from Activity1");
                StartActivity(activitypickup);
            };

            img.Click += delegate
            {
				MainActivity opList = new MainActivity();

				this.Activity.SupportFragmentManager.BeginTransaction()
				.Replace(Resource.Id.fragment_container, opList)
				.Commit();

				((BaseActivity) this.Activity).updateDrawer(1, "Operation List");
			};

			img3.Click += delegate
			{
				ScanActivity scan2 = new ScanActivity();

				this.Activity.SupportFragmentManager.BeginTransaction()
				.Replace(Resource.Id.fragment_container, scan2)
				.Commit();

				((BaseActivity)this.Activity).updateDrawer(4, "Loading");
			};

			img4.Click += delegate
			{
				ScanActivity scan2 = new ScanActivity();

				this.Activity.SupportFragmentManager.BeginTransaction()
				.Replace(Resource.Id.fragment_container, scan2)
				.Commit();

				((BaseActivity)this.Activity).updateDrawer(3, "Delivery");
			};

			return view;
        }
    }
}