using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V4.App;
using Android.Support.V4.Widget;
using Android.Graphics;

namespace POCDriverApp
{
    public class MainActivity : Fragment
    {
        List<string> items;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.DragDrop_Activity, null);

            var list = view.FindViewById<DraggableListView>(Resource.Id.listView1);

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
            list.Adapter = new DraggableListAdapter((Android.App.Activity) this.Activity, items);

            return view;

        }
    }

    public class DraggableListAdapter : BaseAdapter, IDraggableListAdapter
    {
        public List<string> Items { get; set; }

        public int mMobileCellPosition { get; set; }

        Android.App.Activity context;

        public DraggableListAdapter(Android.App.Activity context, List<string> items) : base()
        {
            Items = items;
            this.context = context;
            mMobileCellPosition = int.MinValue;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return Items[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View cell = convertView;
            if (cell == null)
            {
                cell = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, parent, false);
                cell.SetMinimumHeight(150);
                cell.SetBackgroundColor(Color.WhiteSmoke);

            }

            var text = cell.FindViewById<TextView>(Android.Resource.Id.Text1);
            if (text != null)
            {
                text.Text = position.ToString() + "  " + GetItem(position);
            }

            cell.Visibility = mMobileCellPosition == position ? ViewStates.Invisible : ViewStates.Visible;
            cell.TranslationY = 0;

            return cell;
        }

        public override int Count
        {
            get
            {
                return Items.Count;
            }
        }

        public void SwapItems(int indexOne, int indexTwo)
        {
            var oldValue = Items[indexOne];
            Items[indexOne] = Items[indexTwo];
            Items[indexTwo] = oldValue;
            mMobileCellPosition = indexTwo;
            NotifyDataSetChanged();
        }
    }
}


