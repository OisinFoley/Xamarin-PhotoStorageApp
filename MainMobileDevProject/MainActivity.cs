using System;
using System.Diagnostics;
using Android.App;
using Android.Widget;
using Android.OS;
using System.Threading;
using Android.Views;
using System.Net;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;
using Android.Content;
using System.IO;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using System.Linq;
using System.Text;
using Android.Content.PM;
//using Android.Net;
using Xamarin.Facebook;
using Xamarin.Facebook.Login.Widget;
using Xamarin.Facebook.Login;
using Newtonsoft.Json;


namespace MainMobileDevProject
{
    //[Activity(Label = "3Trá", MainLauncher = true, Icon = "@drawable/xs")]
    [Activity(Label = "3Trá", Icon = "@drawable/xs")]
    //public class MainActivity : Activity
    public class MainActivity : Activity, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback

    {
        private Button mBtnSignIn, mBtnSignup, mBtnOpenGallery, mBtnConverttoBase, mBtnConverttoImg;
        private ProgressBar mProgressBar;
        private TextView mTxtBase64String, mTxtTestCross, mTxtImgChoiceInfo, mTest;

        private ImageView mImgUploadedPhoto;
        private Android.Net.Uri mMyUri;
        private List<string> mImgBaseStrings;
        private TableLayout mTblImgStrings;

        private LoginButton mBtnFbLogin;
        private ICallbackManager mCallBackManager;
        private MyProfileTracker mProfileTracker;
        private ProfilePictureView mProfilePic;

        private Button mBtnGetEmail;

        protected override void OnCreate(Bundle bundle)
        {
            //NOTE*** you have decodebitmapfromstream and decoderesource and decodebytearray
                //def don't need them all

            base.OnCreate(bundle);

            //problem getting view to load with fb login button
            //make fresh activity and try copying xml and code there.
            //async it's another activity we shouldnt get the bother from fb login through a dialog'

            FacebookSdk.SdkInitialize(this.ApplicationContext);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            mBtnSignIn = FindViewById<Button>(Resource.Id.btnSignIn);
            mTxtImgChoiceInfo = FindViewById<TextView>(Resource.Id.txtOr);
            mBtnSignup = FindViewById<Button>(Resource.Id.btnSignUp);
            mProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            mTxtBase64String = FindViewById<TextView>(Resource.Id.txtViewImgBase64);
            mImgUploadedPhoto = FindViewById<ImageView>(Resource.Id.imgUploadedPhoto);
            mTxtTestCross = FindViewById<TextView>(Resource.Id.txtImgPath);
            mBtnOpenGallery = FindViewById<Button>(Resource.Id.btnOpenGallery);
            mBtnConverttoBase = FindViewById<Button>(Resource.Id.btnConverttoBase64);
            mBtnConverttoImg = FindViewById<Button>(Resource.Id.btnConverttoImg);
            mTblImgStrings = FindViewById<TableLayout>(Resource.Id.tblBaseStrings);

            mBtnGetEmail = FindViewById<Button>(Resource.Id.btnGetEmail);

            //string[] mImgBaseStrings = new string[] { };
            //List<string> mImgBaseStrings = new List<string>();
            
            mProfileTracker = new MyProfileTracker();
            mProfileTracker.mOnProfileChanged += mProfileTracker_mOnProfileChanged;
            mProfileTracker.StartTracking();

            mProfilePic = FindViewById<ProfilePictureView>(Resource.Id.profilePicMain);

            mBtnFbLogin = FindViewById<LoginButton>(Resource.Id.btnFbLoginMain);            
            mTest = FindViewById<TextView>(Resource.Id.txtTest);

            mBtnFbLogin.SetReadPermissions(new List<string> { "public_profile", "user_friends", "email" });

            mCallBackManager = CallbackManagerFactory.Create();

            mBtnFbLogin.RegisterCallback(mCallBackManager, this);

            mBtnGetEmail.Click += (o, e) =>
            {
                System.Diagnostics.Debug.Write("welllllllllllllllll");
                GraphRequest request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);

                Bundle parameters = new Bundle();
                parameters.PutString("fields", "id,name,age_range,email");
                request.Parameters = parameters;
                request.ExecuteAsync();
            };


            mBtnSignIn.Click += (object sender, EventArgs e) =>
            {
                //pull up dialog
                
                //finish matching up login dialog, some of the code here is to do with signup as its copied from elsewhere

                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                dialogue_Login signInDialog = new dialogue_Login();
                
                signInDialog.Show(transaction, "dialog fragment");

                signInDialog.mOnLoginComplete += SignInDialog_mOnSignInComplete;
                System.Diagnostics.Debug.Write("BTN SIGNIN CLICKED");
                //Went with a fully-fledged function(name on line above) as it'd be quite a bit to write inside of a lambda expression
                //signInDialog.mOnLoginComplete += (object theSender, OnLoginEventArgs e) =>
                //{

                //}
            };
            
            mBtnSignup.Click += (object sender, EventArgs e) =>
            {
                //pull up dialog
                //this is used to pull up the dialog from the activity
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                dialogue_SignUp signUpDialog = new dialogue_SignUp();
                signUpDialog.Show(transaction, "dialog fragment");

                //signUpDialog.mOnSignUpComplete += signUpDialog_mOnSignUpComplete;
                signUpDialog.mOnSignUpComplete += SignUpDialog_mOnSignUpComplete;
            };

            mBtnOpenGallery.Click += delegate {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(
                    Intent.CreateChooser(imageIntent, "Select photo"), 0);
            };

            mBtnConverttoBase.Click += MBtnConverttoBase_Click;

            mBtnConverttoImg.Click += MBtnConverttoImg_Click;

            mImgUploadedPhoto.Click+= (object sender, EventArgs e) =>
            {
                Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.mail4_small);
                
                //Convert to byte array
                MemoryStream memStream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, memStream);
                byte[] byteArray = memStream.ToArray();

                var intent = new Intent(this, typeof(FullscreenImage));                
                intent.SetType("image/*");
                intent.SetAction(Intent.ActionGetContent);                
                intent.PutExtra("MyImg", byteArray);
                
                StartActivity(intent);
            };
            
        }

        public void OnCompleted(Org.Json.JSONObject json, GraphResponse response)
        {
            string data = json.ToString();
            FacebookProfile result = JsonConvert.DeserializeObject<FacebookProfile>(data);

            System.Diagnostics.Debug.Write("THE DATA IS ******* : " + data);
            mTest.Text = data;

        }

        void mProfileTracker_mOnProfileChanged(object sender, OnProfileChangedEventArgs e)
        {
            if (e.mProfile != null)
            {
                try
                {
                    //mTxtFirstName.Text = e.mProfile.FirstName;
                    //mTxtLastName.Text = e.mProfile.LastName;
                    //mTxtName.Text = e.mProfile.Name;
                    //mTxtEmail.Text = e.mProfile.Email;
                    mProfilePic.ProfileId = e.mProfile.Id;
                    System.Diagnostics.Debug.Write("welllllllllllllllll" + e.mProfile.Class);
                    System.Diagnostics.Debug.Write("welllllllllllllllll" + e.mProfile.Handle);

                    System.Diagnostics.Debug.Write("welllllllllllllllll");
                }

                catch (Exception ex)
                {
                    //Handle error
                }
            }

            else
            {
                //the user must have logged out
                //mTxtFirstName.Text = "First Name";
                //mTxtLastName.Text = "Last Name";
                //mTxtName.Text = "Name";
                mProfilePic.ProfileId = null;
            }
        }



        public void OnSuccess(Java.Lang.Object result)
        {
            LoginResult loginResult = result as LoginResult;
            Console.WriteLine(AccessToken.CurrentAccessToken.UserId);
            System.Diagnostics.Debug.Write("MADE IT TO ONSUCCESS");
        }

        public void OnCancel()
        {
            //throw new NotImplementedException();
        }

        public void OnError(FacebookException error)
        {
            //throw new NotImplementedException();
        }

        protected override void OnDestroy()
        {
            mProfileTracker.StopTracking();
            base.OnDestroy();
        }

        public class MyProfileTracker : ProfileTracker
        {
            public event EventHandler<OnProfileChangedEventArgs> mOnProfileChanged;

            protected override void OnCurrentProfileChanged(Profile oldProfile, Profile newProfile)
            {
                if (mOnProfileChanged != null)
                {
                    mOnProfileChanged.Invoke(this, new OnProfileChangedEventArgs(newProfile));
                }
            }
        }

        public class OnProfileChangedEventArgs : EventArgs
        {
            public Profile mProfile;

            public OnProfileChangedEventArgs(Profile profile) { mProfile = profile; }
        }


        

        

        //was protected earlier
        /*
        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            //could be an error if called as part of another activityresult
            mCallBackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }
        */
        private void MBtnConverttoImg_Click(object sender, EventArgs e)
        {
            TableLayout stk = (TableLayout)FindViewById(Resource.Id.tblBaseStrings);
            TableRow tbrow0 = new TableRow(this);

            ImageView iv0 = new ImageView(this);
            //iv0.SetImageBitmap             

            /*
            TextView tv0 = new TextView(this);
            tv0.Text=" Sl.No ";
            tv0.SetTextColor(Color.GreenYellow);
            tbrow0.AddView(tv0);
            */

            /*
            TextView tv1 = new TextView(this);
            tv1.Text=" Product ";
            tv1.SetTextColor(Color.Turquoise);
            tbrow0.AddView(tv1);
            TextView tv2 = new TextView(this);
            tv2.Text= " Unit Price ";
            tv2.SetTextColor(Color.Magenta);
            tbrow0.AddView(tv2);
            TextView tv3 = new TextView(this);
            tv3.Text= " Stock Remaining ";
            tv3.SetTextColor(Color.Lime);
            tbrow0.AddView(tv3);
            stk.AddView(tbrow0);
            */
            /*
            for (int i = 0; i < 25; i++)
            {
                TableRow tbrow = new TableRow(this);                
                TextView t1v = new TextView(this);
                t1v.Text="" + i;
                t1v.SetTextColor(Color.Blue);
                //t1v.SetGravity(Gravity.CENTER);
                tbrow.AddView(t1v);
                TextView t2v = new TextView(this);
                t2v.Text="Product " + i;
                t2v.SetTextColor(Color.Black);
                //t2v.SetGravity(Gravity.CENTER);
                tbrow.AddView(t2v);
                TextView t3v = new TextView(this);
                t3v.Text="Rs." + i;
                t3v.SetTextColor(Color.Red);
                //t3v.SetGravity(Gravity.CENTER);
                tbrow.AddView(t3v);
                TextView t4v = new TextView(this);
                t4v.Text="well";
                t4v.SetTextColor(Color.Orange);
                //t4v.SetGravity(Gravity.CENTER);
                tbrow.AddView(t4v);
                TextView t5v = new TextView(this);
                t5v.Text = "wellddd";
                t5v.SetTextColor(Color.Maroon);
                //t4v.SetGravity(Gravity.CENTER);
                tbrow.AddView(t5v);
                stk.AddView(tbrow);
                System.Diagnostics.Debug.WriteLine("hey i made it here is *****: ");
                System.Diagnostics.Debug.WriteLine("tbrow child count is *****: " + tbrow.ChildCount);
            }
            */
        }

        private void MBtnConverttoBase_Click(object sender, EventArgs e)
        {
            //string s = null;            

            //if imgview is empty, then create the new list, each time we click this button we
                       
            /*
            List<string> mImgBaseStrings = new List<string>();
            int i = 0;
            while (i < 6)
            {
                mImgBaseStrings.Add("well" + i + " ");
                i++;
            }
                        
            foreach(string s in mImgBaseStrings)
            {
                mTxtImgChoiceInfo.Text += s + "_";
            }
            */
            //mTxtImgChoiceInfo.Text = mImgBaseStrings.ToString();
            

            

            //Bitmap bitmap = ((BitmapDrawable)image.getDrawable()).getBitmap();
            //Bitmap bitmap = ((BitmapDrawable)mImgUploadedPhoto.getDrawable()).getBitmap();
            //Bitmap bitmap1 = ((BitmapDrawable)((LayerDrawable)mImgUploadedPhoto.getDrawable()).getDra‌​wable(0)).getBitmap(‌​);

            //mImgUploadedPhoto.BuildDrawingCache();
            //Bitmap bitmap = mImgUploadedPhoto.GetDrawingCache(true);
            //MemoryStream stream = new MemoryStream();
            //bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);

            //at some stage we will have to have data.Data passed to this method call, for now we're 
            //going to store in a var for the sake of making progress

            //Android.Net.uri and system.uri

            //Uri myUri = new Uri(mTxtBase64String.Text);
            //mMyUri //this is an actual uri gathered straight from the activityresult down below
            //Android.Net.Uri myUri = Android.Net.Uri.Parse(mTxtBase64String.Text);
                //javaURI.toString());
            Stream stream = ContentResolver.OpenInputStream(mMyUri);
            //Stream stream = ContentResolver.OpenInputStream((Uri)mTxtBase64String.Text);
            //System.Diagnostics.Debug.WriteLine("base 64 is *****: " + data.Data);
            //mImgUploadedPhoto.SetImageBitmap(DecodeBitmapFromStream(myUri, 150, 150));
            mImgUploadedPhoto.SetImageBitmap(DecodeBitmapFromStream(mMyUri, 150, 150));

/*
            //something is up, the 2 methods below might not even be necessary,
            //not all of the code there is necessary, we just need to isolate the chosen image, then 
            //convert to base64 like with the resource. for now, using the resource for DB testing
*/

            //using the resource rather than chosen from gall should output the base64 successfully...
            //Bitmap bitmap = BitmapFactory.DecodeStream(stream);
            Bitmap bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.mail4_small);
            MemoryStream memStream = new MemoryStream();
            bitmap.Compress(Bitmap.CompressFormat.Webp, 100, memStream);

            byte[] picData = memStream.ToArray();

            string bal = Base64.EncodeToString(picData, Base64.Default);
            mTxtBase64String.Text = "**BASE64 is*** " + bal;
            System.Diagnostics.Debug.WriteLine("BASE64IS*****: " + bal);
            System.Diagnostics.Debug.WriteLine("isnullorempty*****: " + String.IsNullOrEmpty(bal));

            DecodeBaseString(bal);


            //System.Diagnostics.Debug.WriteLine("GOING TO INSERT METHOD*****: ");
            //not working right now, seems to be host issue
            InsertToDB(bal);

        }


        public void DecodeBaseString(string bs)
        {

            //byte[] data = Convert.FromBase64String(bs);
            byte[] bitmapData = Convert.FromBase64String(bs);

            //string decodedString = Encoding.UTF8.GetString(data);


            //Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(bal));
            //MemoryStream streamBitmap = new MemoryStream(bitmapData);

            Bitmap bmp = BitmapFactory.DecodeByteArray(bitmapData, 0, bitmapData.Length);
            //mImgUploadedPhoto.SetImageBitmap(bmp);

            CreateTable(bmp);
            
        }

        private void CreateTable(Bitmap bmp)
        {
            //tried dynamic var names but no luck
            /*
            Dictionary<int, string> dictionary =
            new Dictionary<int, string>();
            dictionary.Add(2, "cat");
            dictionary.Add(3, "ruff");
            
            if (dictionary.ContainsKey(3))
            {
                System.Diagnostics.Debug.WriteLine("DICTIONARYFOUNDIT*****: ");
            }

            int dictionary[0] = 33333;
            */

            TableLayout stk = (TableLayout)FindViewById(Resource.Id.tblBaseStrings);
            /*
            TableRow[] tblrow = new TableRow[] { };            
            for(int i = 0; i < 3; i++)
            {
                tblrow[i] = new TableRow(this);
                ImageView iv0 = new ImageView(this);
                iv0.SetImageBitmap(bmp);
                tblrow[i].AddView(iv0);
                stk.AddView(tblrow[i]);
            }
            */
            TableRow tblrow = new TableRow(this);
            ImageView iv0 = new ImageView(this);
            iv0.SetImageBitmap(bmp);
            tblrow.AddView(iv0);
            stk.AddView(tblrow);


        }

        private Bitmap DecodeBitmapFromStream(Android.Net.Uri data, int requestedWidth, int requestedHeight)
        {
            //Decode with InJustDecodeBounds = true to check dimensions
            Stream stream = ContentResolver.OpenInputStream(data);
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

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            //mTxtImgChoiceInfo.Text = (string)data.Data;
            mMyUri = data.Data;

            if (resultCode == Result.Ok)
            {
                //var view = inflater.Inflate(Resource.Layout.dialog_sign_up, container, false);
                //mTxtFirstName = view.FindViewById<EditText>(Resource.Id.txtRegUsername);

                var imageView =
                    FindViewById<ImageView>(Resource.Id.imgUploadedPhoto);
                imageView.SetImageURI(data.Data);
            }

            if(requestCode == 0)
            {
                System.Diagnostics.Debug.Write("REQUESCODE FROM PHOTO INTENT is : *****" + requestCode);
            }

            mCallBackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }

        private void SignInDialog_mOnSignInComplete(object sender, OnLoginEventArgs e)
        {                                    
            System.Diagnostics.Debug.Write("user udentifier is : " + e.UserIdentifier);
            System.Diagnostics.Debug.Write("pword is : " + e.Password);
            //ternary
            string userKey = e.UserIdentifier.Contains("@") ? userKey = "Email" : userKey = "Username";
            System.Diagnostics.Debug.Write("userKey is : *****" + userKey);

            System.Diagnostics.Debug.Write("IN SIGNINCOMPLETE");

            mProgressBar.Visibility = ViewStates.Visible;
            Thread thread = new Thread(ActLikeARequest);
            thread.Start();

            //CheckDatabase(e.UserIdentifier);
            MySqlConnection con = new MySqlConnection("Server=db4free.net;Port=3307;database=ofsligodb;User Id=ofoley1;Password=pinecone;charset=utf8");
            
            List<string> userlist = new List<string>();
            System.Diagnostics.Debug.Write("INITIALISED  USERLIST");
            try
            {
                System.Diagnostics.Debug.Write("INSIDE THE TRY");
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    System.Diagnostics.Debug.Write("CONN OPEN");
                    //if an email was entered in dialog fragment, we search for an email, otherwise byt username
                    string query = "SELECT * FROM tblUsers HAVING " + userKey + "='" + e.UserIdentifier + "'";
                    //string query = "SELECT * FROM tblTest";

                    MySqlCommand cmd = new MySqlCommand(query, con);
                    
                    System.Diagnostics.Debug.Write("TRYING TO READ");
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        //this works as hoped, but will need to rewrite as we won't get an accurate reading 
                        //for whether or not somebody was actually found. userlist will remain 0 even though we found a user
                        //the pword is the problem in this instance

                        //if (dataReader.GetString(3) != e.Password) break;
                        userlist.Add(dataReader.GetString(0) + " ");
                        //userlist.Add(dataReader.GetString(1) + " ");
                        //userlist.Add(dataReader.GetString(2) + " ");
                        //userlist.Add(dataReader.GetString(3) + " ");                        
                        //System.Diagnostics.Debug.Write("USER found is : " + dataReader["Username"]);
                    }
                    dataReader.Close();

                    System.Diagnostics.Debug.Write("user found and their id is : " + userlist[0]);
                    //System.Diagnostics.Debug.Write("user found and their name is : " + userlist[1]);
                    System.Diagnostics.Debug.Write("user list count is  : " + userlist.Count);

                    //if a user was found, go to home page ..if 0 then no user or pwor incorrect - *Need to handle pword scenario*
                    if (userlist.Count > 0)
                    {
                        //StartActivity(new Intent(this, typeof(Home)));
                        //StartActivity(new Intent(this, typeof(HomeNoScrollView)));

                        //var homeActivity = new Intent(this, typeof(HomeNoScrollView));
                        var userAlbumActivity = new Intent(this, typeof(UserAlbums));

                        userAlbumActivity.PutExtra("UsersID", userlist[0]);
                        StartActivity(userAlbumActivity);
                    }

                    if (userlist.Count() < 1)
                    {
                        //handle user not being found
                        System.Diagnostics.Debug.Write("USER DOES NOT EXIST *****: ");
                    }

                    

                    /*
                    //System.Diagnostics.Debug.Write("USER found is : " + userlist.Select(x => x).ToList()); 
                    StringBuilder builder = new StringBuilder();
                    foreach (string usr in userlist) // Loop through all strings
                    {
                        builder.Append(usr).Append("|"); // Append string to StringBuilder
                    }
                    string result = builder.ToString(); // Get string from StringBuilder                                        
                    */

                    
                    System.Diagnostics.Debug.Write("READER CLOSED *****: ");
                }

            }
            catch (MySqlException ex)
            {
                mTxtBase64String.Text = ex.ToString();
            }
            finally
            {
                con.Close();
            }
            System.Diagnostics.Debug.Write(userlist.Count);            

        }





        //static string CheckDatabase(string Uname)
        string CheckDatabase(string Uname)
        {            
            string UserLogStatus = "hi";
            
            mTxtTestCross.Text = "WELLL";

            MySqlConnection con = new MySqlConnection("Server=db4free.net;Port=3307;database=ofsligodb;User Id=ofoley1;Password=pinecone;charset=utf8");

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    System.Diagnostics.Debug.Write("CONN OPEN");
                    //MySqlCommand cmd = new MySqlCommand("SELECT * FROM tblTest WHERE Name = @Name");

                    //string query = "SELECT * FROM tblTest WHERE Name = " + Uname;
                    //string query = "SELECT * FROM tblTest HAVING Name = alfred";
                    //string query = "SELECT Name FROM tblTest HAVING Name = alfred";
                    //string query = "SELECT * FROM tblTest";
                    string query = "SELECT Name FROM tblTest HAVING Name = '" + Uname + "'";

                    MySqlCommand cmd = new MySqlCommand(query, con);

                    List<string> userlist = new List<string>();
                    
                    System.Diagnostics.Debug.Write("TRYING TO READ");
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        userlist.Add(dataReader.GetString(0) + " ");
                        userlist.Add(dataReader["Name"] + "");

                        System.Diagnostics.Debug.Write("USER found is : " + dataReader["Name"]);
                    }

                    if(userlist.Count() < 1)
                    {
                        //handle user not being found
                        System.Diagnostics.Debug.Write("YO that USER NOT found is : ");
                    }

                    

                    //Boolean fbFound = isAppInstalled(this, "com.facebook.katana");
                    //System.Diagnostics.Debug.Write("PACKAGE FOUND EQUALS is : " + fbFound);


                    //String appPackageName = "com.facebook.katana";

                    //Intent i = new Intent(Intent.ActionView);
                    ////i.SetData(Uri.parse("https://play.google.com/store/apps/details?id=my"+appPackageName));
                    //i.SetData(Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.facebook.katana"));
                    //StartActivity(i);




                    /*
                    //System.Diagnostics.Debug.Write("USER found is : " + userlist.Select(x => x).ToList()); 
                    StringBuilder builder = new StringBuilder();
                    foreach (string usr in userlist) // Loop through all strings
                    {
                        builder.Append(usr).Append("|"); // Append string to StringBuilder
                    }
                    string result = builder.ToString(); // Get string from StringBuilder                                        
                    */

                    dataReader.Close();
                    System.Diagnostics.Debug.Write("READER CLOSED");                    
                }                

            }
            catch (MySqlException ex)
            {
                mTxtBase64String.Text = ex.ToString();
            }
            finally
            {
                con.Close();
            }
            
            return UserLogStatus;
        }
        

        void SignUpDialog_mOnSignUpComplete(object sender, OnSignUpEventArgs e)
        {
            mBtnOpenGallery.Visibility = ViewStates.Visible;
            
            mProgressBar.Visibility = ViewStates.Visible;
            Thread thread = new Thread(ActLikeARequest);
            thread.Start();

            DoesUserAlreadyExist(e.Username, e.Email, e.Password);

        }

        private void ActLikeARequest()
        {
            Thread.Sleep(3000);

            //the above is obviously a different thread to our main thread.
            //trying to make ui changes on another thread will invariably cause a crash, hence next line

            //we want to run this when the action/wait/progression is over
            RunOnUiThread(() => { mProgressBar.Visibility = ViewStates.Invisible; }); //anonymous method at start
        }

        private void DoesUserAlreadyExist(string uname, string email, string password)
        {
            //USE AWS STRING LATER
            MySqlConnection con = new MySqlConnection("Server=db4free.net;Port=3307;database=ofsligodb;User Id=ofoley1;Password=pinecone;charset=utf8");            
            string checkUsersQuery = "SELECT * FROM tblUsers WHERE Username = '"+ uname + "' OR Email = '" + email +  "';";
            bool userExists = false;

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    System.Diagnostics.Debug.WriteLine("CONNECTION OPEN*****: ");
                    MySqlCommand cmd = new MySqlCommand(checkUsersQuery, con);
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    
                    while (dataReader.Read())
                    {
                        System.Diagnostics.Debug.WriteLine("READER OPEN*****: ");
                        if (dataReader["ID"] != DBNull.Value)
                        {
                            System.Diagnostics.Debug.WriteLine("USERNAME OR EMAIL ALREADY TAKEN*****: ");
                            userExists = true;
                        }
                        //has tried an else clause to try a direct insert from here, but datareader doesn't fire at all
                        //if no data, not even the debug message one line inside the 'while' statement
                    }
                    dataReader.Close();
                    System.Diagnostics.Debug.WriteLine("user exists bool value is*****: " + userExists);
                    System.Diagnostics.Debug.WriteLine("READER CLOSED*****: ");                    
                }
            }
            
            catch (MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR***** : " + ex.ToString());
                //mTxtImgChoiceInfo.Text = ex.ToString();
            }

            finally
            {
                //was something returned from the above dataReader[]
                if (userExists == false)
                {                    
                    System.Diagnostics.Debug.WriteLine("ATTEMPTING INSERT*****: ");
                    MySqlCommand InsertCmd = new MySqlCommand("INSERT INTO tblUsers(Username,Email,Password) VALUES(@uname,@email,@pword)");
                    InsertCmd.Parameters.AddWithValue("@uname", uname);
                    InsertCmd.Parameters.AddWithValue("@email", email);
                    InsertCmd.Parameters.AddWithValue("@pword", password);
                    InsertCmd.Connection = con;
                    InsertCmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine("INSERT SHOULD HAVE HAPPENED*****: ");                    
                }
                con.Close();
                System.Diagnostics.Debug.WriteLine("CONN CLOSED*****: ");
            }            
        }


        private void InsertToDB(string base64string)
        {
            System.Diagnostics.Debug.WriteLine("ENTERED INSERT METHOD*****: ");
            MySqlConnection con = new MySqlConnection("Server=db4free.net;Port=3307;database=ofsligodb;User Id=ofoley1;Password=pinecone;charset=utf8");

            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    System.Diagnostics.Debug.WriteLine("CONNECTION OPEN*****: ");
                    //When performing /GET
                    //string query = "SELECT * FROM tblTest";

                    MySqlCommand Readcmd = new MySqlCommand("INSERT INTO tblBlob(ImgAsString) VALUES(@baseString)");
                    //MySqlCommand Readcmd = new MySqlCommand("INSERT INTO tblTest(Name,ContactNumber) VALUES(@Name,@ContactNumber)");
                    //Readcmd.Parameters.AddWithValue("@Name", "george");
                    //Readcmd.Parameters.AddWithValue("@ContactNumber", 0851281285);


                    Readcmd.Parameters.AddWithValue("@baseString", base64string);                                      
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
                mTxtImgChoiceInfo.Text = ex.ToString();
            }
            finally
            {
                con.Close();
                System.Diagnostics.Debug.WriteLine("CONN CLOSED*****: ");
            }

        }
    }
}

