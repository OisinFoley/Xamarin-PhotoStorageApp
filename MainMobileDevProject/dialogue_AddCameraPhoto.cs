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
using Java.IO;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android.Util;
using Android.Provider;
using Android.Content.PM;

namespace MainMobileDevProject
{
    //custom eventargs class
    public class OnNewPhotoEventArgs : EventArgs
    {
        //because these are private we will have to use the firstname, email properties otuside of this class, 
        //rather than nFirstName or nEmail
        private string mTags;        
        private Bitmap mCameraPhoto;
                
        public string Tags
        {
            get { return mTags; }
            //c# knows that value means set the value to whatever value we give when we initialise firstname
            set { mTags = value; } 
        }

        public Bitmap CameraPhoto
        {
            get { return mCameraPhoto; }
            //c# knows that value means set the value to whatever value we give when we initialise firstname
            set { mCameraPhoto = value; }
        }



        //calling base method of parent class
        public OnNewPhotoEventArgs(string tags, Bitmap bm) : base()
        {
            Tags = tags;
            CameraPhoto = bm;
        }
        
    }

    public static class App
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }


    public class dialogue_AddCameraPhoto : DialogFragment
    {
        private EditText mTags;
        private ImageView mImgView;
        private Bitmap mCameraPhoto;
        private Button mBtnConfirmCameraPhoto;

        

        //this event is going to take the event we just created. if not specified, it'll take the regular eventargs class
        public event EventHandler<OnNewPhotoEventArgs> mAddCameraPhotoComplete;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            //create view the dialogue will hold            
            //attach it to our initial container, and false because we don't want to attach it to the root
            var view = inflater.Inflate(Resource.Layout.dialog_add_cameraPhoto, container, false);

            mTags = view.FindViewById<EditText>(Resource.Id.tbxTags);
            mImgView = view.FindViewById<ImageView>(Resource.Id.imageView1);
            //mCameraPhoto
            mBtnConfirmCameraPhoto = view.FindViewById<Button>(Resource.Id.btnDialogConfirmPhoto);

            mBtnConfirmCameraPhoto.Click += mBtnConfirmCameraPhoto_Click;
            
            
                CreateDirectoryForPictures();
                mImgView.Click += TakeAPicture;
            

            return view;

            


        }
        

        void mBtnConfirmCameraPhoto_Click(object sender, EventArgs e)
        {            
            mAddCameraPhotoComplete.Invoke(this, new OnNewPhotoEventArgs (mTags.Text,mCameraPhoto));            
            this.Dismiss(); //will slide out and go away                
        }



        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            //our dialog will no longer have blank title displayed at top            
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle);
            base.OnActivityCreated(savedInstanceState);
            //name comes from our styles.xml,, sets animation
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; 
        }
        
        //public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        //{
        //    base.OnActivityResult(requestCode, resultCode, data);            
        //}

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            // Make it available in the gallery

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);

            Uri contentUri = Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            //SendBroadcast(mediaScanIntent);

            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume to much memory
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = mImgView.Height;
            App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
            //mCameraPhoto = App._file.Path.
            //  BitmapFactory.DecodeFile(fileName, options); ;
            mCameraPhoto = BitmapFactory.DecodeFile(App._file.Path);
            System.Diagnostics.Debug.Write("MCAMERAPHOT IS ::: " + mCameraPhoto);


            if (App.bitmap != null)
            {
                mImgView.SetImageBitmap(App.bitmap);
                App.bitmap = null;
            }

            // Dispose of the Java side bitmap.
            GC.Collect();
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = new File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "Base64Attempt");
            if (!App._dir.Exists())
            {
                App._dir.Mkdirs();
            }
        }

        //private bool IsThereAnAppToTakePictures()
        //{
        //    Intent intent = new Intent(MediaStore.ActionImageCapture);
        //    //IList<ResolveInfo> availableActivities =
        //        //PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
        //    //return availableActivities != null && availableActivities.Count > 0;
        //}

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }
    }
    public static class BitmapHelpers
    {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);
            //mCameraPhoto
            //Bitmap storedBitmap = BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // in order to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            //return storedBitmap;
            return resizedBitmap;
        }
    }
}