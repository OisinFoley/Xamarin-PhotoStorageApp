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
using Android.Graphics;

namespace MainMobileDevProject
{
    [Activity(Label = "FullscreenImage", Theme = "@style/Theme.Splash")]
    public class FullscreenImage: Activity
    {
        private ImageView iv;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.fullscreen);

            iv = FindViewById<ImageView>(Resource.Id.imgFullscreen);


            //try to pass activ1's resource drawable int value, then pass it over here, then try to grab it back out, and
            //set our image in this activity(activity2) to the value of what we passed from before.
            //that will then mimic, the whole allowing image fullscreen in our main app


            string text = Intent.GetStringExtra("MyData") ?? "Data not available";

            byte[] byteArray = Intent.GetByteArrayExtra("MyImg"); //getIntent().getByteArrayExtra("image");
            Bitmap bmp = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length);

            iv.SetImageBitmap(bmp);

            iv.Click += (object sender, EventArgs e) =>
            {
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            };

        }
    }
}