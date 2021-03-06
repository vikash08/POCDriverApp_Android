﻿using System;
using Android.App;
using Android.Views;
using Android.OS;
using Android.Content;
using V7Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Java.Util;
using Android.Widget;

namespace POCDriverApp
{
    [Activity(Icon = "@drawable/ic_launcher", Label = "@string/app_name",
               Theme = "@style/Theme.DesignDemo")]
    public class BaseActivity : AppCompatActivity
    {
        DrawerLayout drawerLayout;
        NavigationView navigationView;
        UserSessionManager sessionManager;

        //[Java.Interop.Export()]
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.NavigationLayoutaxml);
            var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);

            sessionManager = new UserSessionManager(Application.Context);
            HashMap user = sessionManager.getUserDetails();
            var uuid = user.Get(UserSessionManager.TAG_uuid);
            var email = user.Get(UserSessionManager.TAG_mail);
            var name = user.Get(UserSessionManager.TAG_name);

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            var toggle = new ActionBarDrawerToggle(
                    this, drawerLayout, toolbar, Resource.String.drawer_open,
                                                      Resource.String.drawer_close);
            toggle.SyncState();

            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;

            View hView = navigationView.InflateHeaderView(Resource.Layout.Header);

            TextView header = hView.FindViewById<TextView>(Resource.Id.headerText);
            header.Text = name + " (" + uuid + ")";

            home_Activity home = new home_Activity();

            SupportActionBar.Title = "Home";

            SupportFragmentManager.BeginTransaction()
            .Replace(Resource.Id.fragment_container, home)
            .Commit();

            navigationView.Menu.GetItem(0).SetChecked(true);
        }

        public void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            e.MenuItem.SetChecked(true);

            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.nav_Home):
                    home_Activity home = new home_Activity();

                    SupportActionBar.Title = "Home";

                    SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, home)
                    .Commit();

                    break;

                case (Resource.Id.nav_OperList):
                    MainActivity opList = new MainActivity();

                    SupportActionBar.Title = "Operation List";

                    SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, opList)
                    .Commit();

                    break;

                case (Resource.Id.nav_pickUp):
                    var activitypickup = new Intent(this, typeof(ToDoActivity));
                    activitypickup.PutExtra("MyData", "Data from Activity1");
                    StartActivity(activitypickup);
                    break;

                case (Resource.Id.nav_delivery):
                    ScanActivity scan1 = new ScanActivity();

                    SupportActionBar.Title = "Delivery";

                    SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, scan1)
                    .Commit();

                    break;

                case (Resource.Id.nav_loading):
                    ScanActivity scan2 = new ScanActivity();

                    SupportActionBar.Title = "Loading";

                    SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, scan2)
                    .Commit();

                    break;

                case (Resource.Id.nav_Logout):
                    sessionManager.logoutUser();
                    var login = new Intent(this, typeof(Login));
                    StartActivity(login);
                    Finish();

                    break;
            }

            e.MenuItem.SetChecked(true);
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

        public override void OnBackPressed()
        {
            if (drawerLayout.IsDrawerOpen(navigationView))
                drawerLayout.CloseDrawers();
            else
                base.OnBackPressed();
        }

        public void updateDrawer(int pos, String title)
        {
            SupportActionBar.Title = title;
            navigationView.Menu.GetItem(pos).SetChecked(true);
        }
    }
}