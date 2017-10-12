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
    public class BaseActivity : AppCompatActivity
    {
        DrawerLayout drawerLayout;
        NavigationView navigationView;
        //[Java.Interop.Export()]
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.NavigationLayoutaxml);
            var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetHomeButtonEnabled(true);
            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_launcher);

            //this.ActionBar.SetDisplayHomeAsUpEnabled(true);
            //this.ActionBar.SetHomeButtonEnabled(true);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;
            //navigationView.NavigationItemSelected += (sender, e) =>
            //{
            //    e.MenuItem.SetChecked(true);
            //    //react to click here and swap fragments or navigate


            //    drawerLayout.CloseDrawers();
            //};

        }



        public void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            e.MenuItem.SetChecked(true);
           
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.nav_Home):
                    // React on 'Home' selection
                    // ...
                    var activity2 = new Intent(this, typeof(home_Activity));
                    activity2.PutExtra("MyData", "Data from Activity1");
                    StartActivity(activity2);
                    break;
                case (Resource.Id.nav_delivery):
                    // React on 'Home' selection
                    // ...
                    var activitydelivery = new Intent(this, typeof(ScanActivity));
                    activitydelivery.PutExtra("MyData", "Data from Activity1");
                    StartActivity(activitydelivery);
                    break;
                case (Resource.Id.nav_OperList):
                    // React on 'Home' selection
                    // ...
                    var activityOL = new Intent(this, typeof(MainActivity));
                    activityOL.PutExtra("MyData", "Data from Activity1");
                    StartActivity(activityOL);
                    break;
                case (Resource.Id.nav_pickUp):
                    // React on 'Home' selection
                    // ...
                    var activitypickup = new Intent(this, typeof(ToDoActivity));
                    activitypickup.PutExtra("MyData", "Data from Activity1");
                    StartActivity(activitypickup);
                    break;
                case (Resource.Id.nav_loading):
                    // React on 'Home' selection
                    // ...
                    var activityLoading = new Intent(this, typeof(ScanActivity));
                    activityLoading.PutExtra("MyData", "Data from Activity1");
                    StartActivity(activityLoading);
                    break;
            }
            e.MenuItem.SetChecked(true);
            // Close drawer
            drawerLayout.CloseDrawers();
        }

        //[Java.Interop.Export()]
        //public EventHandler<NavigationView.NavigationItemSelectedEventArgs> NavigationView_NavigationItemSelected(object sender, object e)
        //{
        //    switch (e.MenuItem.ItemId)
        //    {
        //        case (Resource.Id.nav_Home):
        //            // React on 'Home' selection
        //            // ...
        //            var activity2 = new Intent(this, typeof(home_Activity));
        //            activity2.PutExtra("MyData", "Data from Activity1");
        //            StartActivity(activity2);
        //            break;
        //    }

            //    // Close drawer
            //    drawerLayout.CloseDrawers();
            //    return null;
            //}

            //[Java.Interop.Export()]
            //public void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
            //{
            //    switch (e.MenuItem.ItemId)
            //    {
            //        case (Resource.Id.nav_Home):
            //            // React on 'Home' selection
            //            // ...
            //            var activity2 = new Intent(this, typeof(home_Activity));
            //            activity2.PutExtra("MyData", "Data from Activity1");
            //            StartActivity(activity2);
            //            break;
            //    }

            //    // Close drawer
            //    drawerLayout.CloseDrawers();
            //}

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.MenuInflater.Inflate(Resource.Menu.menu, menu);

            //var searchItem = menu.FindItem(Resource.Id.action_search);

            //searchView = searchItem.ActionProvider.JavaCast<Android.Widget.SearchView>();

            //searchView.QueryTextSubmit += (sender, args) =>
            //{
            //    Toast.MakeText(this, "You searched: " + args.Query, ToastLength.Short).Show();

            //};

           return false;
            //return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {


            //switch (id)
            //{
            //    case R.id.nav_Home:
            //        //Do some thing here
            //        // add navigation drawer item onclick method here
            //        var activity2 = new Intent(this, typeof(home_Activity));
            //        activity2.PutExtra("MyData", "Data from Activity1");
            //        StartActivity(activity2);
            //        break;
            //    case R.id.nav_OperList:
            //        //Do some thing here
            //        // add navigation drawer item onclick method here
            //        break;
            //    case R.id.nav_pickUp:
            //        //Do some thing here
            //        // add navigation drawer item onclick method here
            //        break;
            //    case R.id.nav_delivery:
            //        //Do some thing here
            //        // add navigation drawer item onclick method here
            //        break;
            //    case R.id.nav_loading:
            //        //Do some thing here
            //        // add navigation drawer item onclick method here
            //        break;
            //}

            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}