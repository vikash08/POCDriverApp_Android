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
using Android.Graphics;

namespace POCDriverApp
{
    [Activity(MainLauncher = false,
               Icon = "@drawable/ic_launcher", Label = "@string/app_name",
               Theme = "@style/Theme.DesignDemo")]
    public class MainActivity : BaseActivity
    {
		List<string> items;


		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			// Set our view from the "main" layout resource
			//SetContentView (Resource.Layout.DragDrop_Activity);

            var toolbar = FindViewById<V7Toolbar>(Resource.Id.toolbar);
            // SetSupportActionBar(toolbar);
            //FrameLayout contentFrameLayout = (FrameLayout)FindViewById(Resource.Layout.); //Remember this is the FrameLayout area within your activity_main.xml
            LayoutInflater inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
            inflater.Inflate(Resource.Layout.DragDrop_Activity, toolbar);

            var list = FindViewById<DraggableListView> (Resource.Id.listView1);


			items = new List<string> {
				"Pick Up At PGO",
				"Loading At PGO",
				"Delivery At Oslo Terminal",
				"Pick Up At National Theatre",
				"Delivery At Storo ",
				"Unloading At Bergen",
				"Loading At Bergen",
				"Delivery in Bergen",
                "Pick Up At Tromso",
                "Loading At Tromso",
                "Delivery At Oslo Central",
                "Pick Up At Stovner",
                "Delivery At Okern ",
                "Unloading At Sandvika",
                "Loading At Sandvika",
                "Delivery in Lorenskog",
            };
			list.Adapter = new DraggableListAdapter (this, items);
		}
	}

	public class DraggableListAdapter : BaseAdapter, IDraggableListAdapter
	{
		public List<string> Items { get; set; }


		public int mMobileCellPosition { get; set; }

		Activity context;

		public DraggableListAdapter (Activity context, List<string> items) : base ()
		{
			Items = items;
			this.context = context;
			mMobileCellPosition = int.MinValue;
		}

		public override Java.Lang.Object GetItem (int position)
		{
			return Items [position];
		}

		public override long GetItemId (int position)
		{
			return position;
		}

		public override View GetView (int position, View convertView, ViewGroup parent)
		{
			View cell = convertView;
			if (cell == null) {
				cell = context.LayoutInflater.Inflate (Android.Resource.Layout.SimpleListItem1, parent, false);
				cell.SetMinimumHeight (150);
				cell.SetBackgroundColor (Color.WhiteSmoke);
                
			}

			var text = cell.FindViewById<TextView> (Android.Resource.Id.Text1);
			if (text != null) {
				text.Text = position.ToString () + "  "+ GetItem(position);
			}

			cell.Visibility = mMobileCellPosition == position ? ViewStates.Invisible : ViewStates.Visible;
			cell.TranslationY = 0;

			return cell;
		}

		public override int Count {
			get {
				return Items.Count;
			}
		}

		public void SwapItems (int indexOne, int indexTwo)
		{
			var oldValue = Items [indexOne];
			Items [indexOne] = Items [indexTwo];
			Items [indexTwo] = oldValue;
			mMobileCellPosition = indexTwo;
			NotifyDataSetChanged ();
		}
			
	}
}


