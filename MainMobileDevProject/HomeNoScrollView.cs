using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Content.PM;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Android.Graphics;
using MySql.Data.MySqlClient;
using System.Data;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android.Util;
using static Android.Widget.AdapterView;
using Android.Support.V7.App;

namespace MainMobileDevProject
{
    [Activity(Label = "Sliding Tab Layout", MainLauncher = true, Icon = "@drawable/xs")]
    //[Activity(Label = "Sliding Tab Layout", Icon = "@drawable/xs")]

    public class HomeNoScrollView: AppCompatActivity
    {
        private ListView mListView;
        private BaseAdapter<Contact> mAdapter;
        private List<Contact> mContacts;
        private ImageView mSelectedPic;        
        private Button mBtnAddPics, button;
        public List<byte[]> imagesByteList = new List<byte[]>();
        public List<string> imagesTagsList = new List<string>();
        Bitmap myBitmap;


        private ImageView _imageView;

        public static class App
        {
            public static File _file;
            public static File _dir;
            public static Bitmap bitmap;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.homeNoScrollView);

            string text = Intent.GetStringExtra("MyData") ?? "Data not available";
            System.Diagnostics.Debug.Write("DATA WE PASSED HERE IS : " + text);

            mListView = FindViewById<ListView>(Resource.Id.listView);
            mContacts = new List<Contact>();
            mBtnAddPics = FindViewById<Button>(Resource.Id.btnAdd);
            
            
            //ItemClickEventArgs

            Action<ImageView> action = PicSelected;

            //mAdapter = new ContactListAdapter(this, Resource.Layout.pager_item, mContacts, action);
            mAdapter = new ContactListAdapter(this, Resource.Layout.row_contact, mContacts);

            

            //mListView.chil


            mListView.ItemClick += lv_ItemClick;

            void lv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
            {
                System.Diagnostics.Debug.Write("LISTVIEW CLICKED AT  : " + e.Position);
            }


                mListView.Adapter = mAdapter;
                //mListView.NotifyDataSetChanged();
                
            //mListView.ItemClick += MListView_ItemClick;


            mListView.ItemSelected += (sender, e) =>
            {
                System.Diagnostics.Debug.Write("LISTVIEW CLICKED AT  : " + e.Position);
                //ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, items);
                //lv1.Adapter = adapter;
                string str = mListView.GetItemAtPosition(e.Position).ToString();
                System.Diagnostics.Debug.Write("LISTVIEW CLICKED AT  : " + str);
                //tv1.Text = str;
            };

            mListView.ItemClick += (sender, e) =>
            {
                System.Diagnostics.Debug.Write("LISTVIEW CLICKED AT  : " + e.Position);
                //ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, items);
                //lv1.Adapter = adapter;
                string str = mListView.GetItemAtPosition(e.Position).ToString();
                System.Diagnostics.Debug.Write("LISTVIEW CLICKED AT  : " + str);
                //tv1.Text = str;
            };

            mAdapter.NotifyDataSetChanged();



            Button button = FindViewById<Button>(Resource.Id.myButton);
            _imageView = FindViewById<ImageView>(Resource.Id.imageView1);
            button.Click += (object sender, EventArgs e) =>
            {
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                dialogue_AddCameraPhoto addCameraPhoto = new dialogue_AddCameraPhoto();
                //dialogue_Login signInDialog = new dialogue_Login();

                addCameraPhoto.Show(transaction, "dialog fragment");

                //addCameraPhoto.mAddCameraPhotoComplete += AddCameraPhoto_mAddCameraPhotoComplete;
                addCameraPhoto.mAddCameraPhotoComplete += UploadCameraPhoto;

                


                //StartActivity(new Intent(this, typeof(dialogue_AddCameraPhoto)));
            };
                


            mBtnAddPics.Click += MBtnAddPics_Click;


            //mBtnSaveImgInfo.Click += UploadCameraPhoto;



        }

        private void MListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            System.Diagnostics.Debug.Write("myBitmap row id : " + e.Id);
            System.Diagnostics.Debug.Write("myBitmap row parent is : " + e.Parent);
            System.Diagnostics.Debug.Write("myBitmap row position : " + e.Position);
            System.Diagnostics.Debug.Write("myBitmap row view : " + e.View);
        }



        //private void UploadCameraPhoto(object sender, EventArgs e)
        private void UploadCameraPhoto(object sender, OnNewPhotoEventArgs e)
        {
            //System.Diagnostics.Debug.Write("myBitmap is : " + myBitmap);
            
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();

            //myBitmap.Compress(Bitmap.CompressFormat.Webp, 100, memStream);
            e.CameraPhoto.Compress(Bitmap.CompressFormat.Webp, 100, memStream);


            byte[] picData = memStream.ToArray();
            string bal = Base64.EncodeToString(picData, Base64.Default);
            System.Diagnostics.Debug.WriteLine("BASE64IS*****: " + bal);

            MySqlConnection con = new MySqlConnection("Server=db4free.net;Port=3307;database=ofsligodb;User Id=ofoley1;Password=pinecone;charset=utf8");

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    System.Diagnostics.Debug.WriteLine("CONNECTION OPEN*****: ");
                    //When performing /GET
                    //string query = "SELECT * FROM tblTest";

                    MySqlCommand Readcmd = new MySqlCommand("INSERT INTO tblUserAlbumPhoto(UserID,AlbumID,Base64String,Tags) " +
                        "VALUES(@uid,@aid,@base64string,@tags)");
                    //MySqlCommand Readcmd = new MySqlCommand("INSERT INTO tblTest(Name,ContactNumber) VALUES(@Name,@ContactNumber)");
                    Readcmd.Parameters.AddWithValue("@uid", 1);
                    Readcmd.Parameters.AddWithValue("@aid", 1);
                    Readcmd.Parameters.AddWithValue("@base64string", bal);
                    Readcmd.Parameters.AddWithValue("@tags", e.Tags);                    


                    //Readcmd.Parameters.AddWithValue("@baseString", base64string);
                    //Readcmd.Parameters.AddWithValue("@baseString", "khfdkjndfkhbfjbfdkjjhfdfd");
                    Readcmd.Connection = con;
                    Readcmd.ExecuteNonQuery();

                    System.Diagnostics.Debug.WriteLine("INSERT SHOULD HAVE HAPPENED*****: ");

                    //MySqlCommand cmd = new MySqlCommand(query, con);
                    //MySqlDataReader dataReader = cmd.ExecuteReader();

                    //con.Close();                    

                }

            }

            catch (MySqlException ex)
            {
                //mTxtImgChoiceInfo.Text = ex.ToString();
            }
            finally
            {
                con.Close();
                System.Diagnostics.Debug.WriteLine("CONN CLOSED*****: ");
                mAdapter.NotifyDataSetChanged();
            }

        }

        private void MBtnAddPics_Click(object sender, EventArgs e)
        {
            MySqlConnection con = new MySqlConnection("Server=db4free.net;Port=3307;database=ofsligodb;User Id=ofoley1;Password=pinecone;charset=utf8");
            //List<string> imageslist = new List<string>();
            //List<byte[]> imageslist = new List<byte[]>();

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    System.Diagnostics.Debug.Write("CONN OPEN");
                    //string query = "SELECT * FROM tblUserAlbumPhoto HAVING UserID = '" + Intent.GetStringExtra("UsersID") + "' AND AlbumID = '" + Intent.GetStringExtra("UsersID") +"'";                    
                    string query = "SELECT * FROM tblUserAlbumPhoto HAVING UserID = '" + 1 + "' AND AlbumID = '" + 1 +"'";
                    MySqlCommand cmd = new MySqlCommand(query, con);

                    System.Diagnostics.Debug.Write("TRYING TO READ");
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {                        
                        imagesByteList.Add(Convert.FromBase64String(dataReader.GetString(4)));
                        imagesTagsList.Add(dataReader.GetString(5));
                        //System.Diagnostics.Debug.Write("image still as base64 string is : " + dataReader.GetString(1));
                        //System.Diagnostics.Debug.Write("image converted from base64 found is : " + Convert.FromBase64String(dataReader.GetString(1)));
                    }
                    System.Diagnostics.Debug.Write("image ID found is : " + imagesByteList[0]);
                    System.Diagnostics.Debug.Write("image ID found is : " + imagesByteList[1]);
                }

            }
            catch (MySqlException ex)
            {
                //mTxtBase64String.Text = ex.ToString();
            }
            finally
            {
                con.Close();
                System.Diagnostics.Debug.Write("CONN CLOSED");
                System.Diagnostics.Debug.Write("image ID count found is : " + imagesByteList.Count);
            }

            CountImagesList();
            if (imagesByteList.Count > 0) //just a test
            {
                CreateImgFromBytes(imagesByteList, imagesTagsList);
            }
        }

        private void CreateImgFromBytes(List<byte[]> imageslist, List<string> tags)
        {
            //foreach(byte[] ba in imageslist)
            for(int y=0;y<imagesByteList.Count;y++)
            {
                mContacts.Add(new Contact() { Name = tags[y], Image = imageslist[y] });
            }
            //mContacts.Add(new Contact() { Name = "OISIN", Number = "FOLEY", Image = imageslist[0] });
            System.Diagnostics.Debug.Write("imageslist lentth is: " + imageslist.Count);
            mAdapter.NotifyDataSetChanged();

            //Decode with InJustDecodeBounds = true to check dimensions
            //Stream stream = ContentResolver.OpenInputStream(data);
            //BitmapFactory.Options options = new BitmapFactory.Options();
            //options.InJustDecodeBounds = true;
            //BitmapFactory.DecodeStream(stream);

            ////Calculate InSamplesize
            //options.InSampleSize = CalculateInSampleSize(options, requestedWidth, requestedHeight);

            ////Decode bitmap with InSampleSize set
            //stream = ContentResolver.OpenInputStream(data); //Must read again
            //options.InJustDecodeBounds = false;
            //Bitmap bitmap = BitmapFactory.DecodeStream(stream, null, options);
            //return bitmap;
        }

        public void CountImagesList()
        {
            System.Diagnostics.Debug.Write("imageslist lentth is: " + imagesByteList.Count);
        }

        private void PicSelected(ImageView selectedPic)
        {
            mSelectedPic = selectedPic;
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            this.StartActivityForResult(Intent.CreateChooser(intent, "Selecte a Photo"), 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            //if (resultCode == Result.Ok)
            //{
            //    Stream stream = ContentResolver.OpenInputStream(data.Data);
            //    mSelectedPic.SetImageBitmap(DecodeBitmapFromStream(data.Data, 150, 150));
            //}

            Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);

            Uri contentUri = Uri.FromFile(App._file);
            mediaScanIntent.SetData(contentUri);
            SendBroadcast(mediaScanIntent);
            
            // Display in ImageView. We will resize the bitmap to fit the display.
            // Loading the full sized image will consume to much memory
            // and cause the application to crash.

            int height = Resources.DisplayMetrics.HeightPixels;
            int width = _imageView.Height;
            App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);
            System.Diagnostics.Debug.Write("APP . BITMAP IS :: " + App.bitmap);
            if (App.bitmap != null)
            {
                _imageView.SetImageBitmap(App.bitmap);
                myBitmap = App.bitmap;
                //App.bitmap = null;
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

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void TakeAPicture(object sender, EventArgs eventArgs)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }

        private Bitmap DecodeBitmapFromStream(Android.Net.Uri data, int requestedWidth, int requestedHeight)
        {
            //Decode with InJustDecodeBounds = true to check dimensions
            System.IO.Stream stream = ContentResolver.OpenInputStream(data);
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            BitmapFactory.DecodeStream(stream);

            //Calculate InSamplesize
            options.InSampleSize = CalculateInSampleSize(options, requestedWidth, requestedHeight);

            //Decode bitmap with InSampleSize set
            stream = ContentResolver.OpenInputStream(data); //Must read again
            options.InJustDecodeBounds = false;
            Bitmap bitmap = BitmapFactory.DecodeStream(stream, null, options);
            return bitmap;
        }

        private int CalculateInSampleSize(BitmapFactory.Options options, int requestedWidth, int requestedHeight)
        {
            //Raw height and widht of image
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > requestedHeight || width > requestedWidth)
            {
                //the image is bigger than we want it to be
                int halfHeight = height / 2;
                int halfWidth = width / 2;

                while ((halfHeight / inSampleSize) > requestedHeight && (halfWidth / inSampleSize) > requestedWidth)
                {
                    inSampleSize *= 2;
                }

            }

            return inSampleSize;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.actionbar_home, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.add:

                    CreateContactDialog dialog = new CreateContactDialog();
                    FragmentTransaction transaction = FragmentManager.BeginTransaction();

                    //Subscribe to event
                    dialog.OnCreateContact += dialog_OnCreateContact;
                    dialog.Show(transaction, "create contact");
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }

        }

        void dialog_OnCreateContact(object sender, CreateContactEventArgs e)
        {
            mContacts.Add(new Contact() { Name = e.Name, Number = e.Number });
            mAdapter.NotifyDataSetChanged();
        }
    }

    /*
    public static class BitmapHelpers
    {
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

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

            return resizedBitmap;
        }
    }
    */
}