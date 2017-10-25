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
using Android.Support.Design.Widget;
using Android.Graphics;

namespace POCDriverApp.Utility
{
    class CustomisedSnackbar
    {
        private static readonly Color red = Color.ParseColor("#C62828");
        private static readonly Color green = Color.ParseColor("#2E7D32");
        private static readonly Color blue = Color.ParseColor("#1565C0");
        private static readonly Color orange = Color.ParseColor("#F57C00");

        private static View getSnackBarLayout(Snackbar snackbar)
        {
            if (snackbar != null)
            {
                return snackbar.View;
            }
            return null;
        }

        private static Snackbar colorSnackBar(Snackbar snackbar, Color color)
        {
            View snackBarView = getSnackBarLayout(snackbar);
            if (snackBarView != null)
            {
                snackBarView.SetBackgroundColor(color);
            }

            return snackbar;
        }

        public static Snackbar info(Snackbar snackbar)
        {
            return colorSnackBar(snackbar, blue);
        }

        public static Snackbar warning(Snackbar snackbar)
        {
            return colorSnackBar(snackbar, orange);
        }

        public static Snackbar alert(Snackbar snackbar)
        {
            return colorSnackBar(snackbar, red);
        }

        public static Snackbar confirm(Snackbar snackbar)
        {
            return colorSnackBar(snackbar, green);
        }
    }
}