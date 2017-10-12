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
using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;

namespace POCDriverApp
{

    [Activity(MainLauncher = false,
               Icon = "@drawable/ic_launcher", Label = "@string/app_name",
               Theme = "@style/Theme.DesignDemo")]
    public class home_Activity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // SetContentView(Resource.Layout.home_layout);

            var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);

            //NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            //navigationView.get().getItem(0).setChecked(true);

            //FrameLayout contentFrameLayout = (FrameLayout)FindViewById(Resource.Layout.); //Remember this is the FrameLayout area within your activity_main.xml
            LayoutInflater inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
            inflater.Inflate(Resource.Layout.home_layout, toolbar);
        

        // Create your application here
    }

     }
}