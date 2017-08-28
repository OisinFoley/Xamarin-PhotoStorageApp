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
using Android.Content.PM;
using Xamarin.Facebook;
using Xamarin.Facebook.Login.Widget;
using Xamarin.Facebook.Login;
using Newtonsoft.Json;

namespace MainMobileDevProject
{
    //custom eventargs class
    public class OnLoginEventArgs : EventArgs
    {
        //because these are private we will have to use the firstname, email properties otuside of this class, 
        //rather than nFirstName or nEmail
        private string mUserIdentifer, mPassword;        
        private Button mBtnPickImg;

        public string UserIdentifier
        {
            get { return mUserIdentifer; }
            //c# knows that value means set the value to whatever value we give when we initialise firstname
            set { mUserIdentifer = value; }
        }                

        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }

        public Button BtnImg
        {
            get { return mBtnPickImg; }            
            set { mBtnPickImg = value; }
        }
        
        //calling base method of parent class
        public OnLoginEventArgs(string userIdentifier, string password) : base()
        {            
            UserIdentifier = userIdentifier;            
            Password = password;                        
        }

        public OnLoginEventArgs(string userIdentifier) : base()
        {
            UserIdentifier = userIdentifier;
            Password = "";
        }

        public OnLoginEventArgs() : base()
        {
            UserIdentifier = "";
            Password = "";
        }
    }



    //first thing, must specify this inheritance in order to behave as a dialogue    
    public class dialogue_Login : DialogFragment
    {
        private EditText mTxtUserIdentifier;        
        private EditText mTxtPassword;        
        private Button mBtnLoginStandard;        
        
        //this event is going to take the event we just created.
        public event EventHandler<OnLoginEventArgs> mOnLoginComplete;
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            
            base.OnCreateView(inflater, container, savedInstanceState);

            //create view the dialogue will hold
            //next we're taking a layout and placing it on top of our current view
            //attach it to our initial container, and false because we don't want to attach it to the root
            var view = inflater.Inflate(Resource.Layout.dialog_login, container, false);

            mTxtUserIdentifier = view.FindViewById<EditText>(Resource.Id.tbxUserIdentifer);            
            mTxtPassword = view.FindViewById<EditText>(Resource.Id.tbxLoginPassword);            
            mBtnLoginStandard = view.FindViewById<Button>(Resource.Id.btnDialogLogin);
            
            //if (AccessToken.CurrentAccessToken != null)
            //{
            //    //The user is logged in through Facebook
            //    faceBookButton.Text = "Logged Out";                
           //}                        

            mBtnLoginStandard.Click += MBtnLoginStandard_Click;           
                    
            return view;
        }

        private void MBtnFbLogin_Click(object sender, EventArgs e)
        {
            //Boolean fbFound = isAppInstalled(this, "com.facebook.katana");
            Boolean fbFound = isAppInstalled(Android.App.Application.Context, "com.facebook.katana");
            System.Diagnostics.Debug.Write("PACKAGE FOUND EQUALS is : " + fbFound);

            if (fbFound)
            {
                //DoFacebookLogin();
                //we can sign out but the sign in operation does not complete, not getting error which is a positive

                /*
                GraphRequest request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);

                Bundle parameters = new Bundle();
                parameters.PutString("fields", "id,name,age_range,email");
                request.Parameters = parameters;
                request.ExecuteAsync();
                */
                
            }
            else
            {
                String appPackageName = "com.facebook.katana";

                Intent i = new Intent(Intent.ActionView);
                //i.SetData(Uri.parse("https://play.google.com/store/apps/details?id=my"+appPackageName));
                i.SetData(Android.Net.Uri.Parse("https://play.google.com/store/apps/details?id=com.facebook.katana"));
                StartActivity(i);
            }

            //close to sorting these errors, if can't do, then uncomment and work in other functionality
            //var string = Android.App.Application.Context


        }


        public static Boolean isAppInstalled(Context context, String packageName)
        {
            try
            {
                //context.getPackageManager().getApplicationInfo(packageName, 0);
                context.PackageManager.GetApplicationInfo(packageName, 0);
                return true;
            }
            catch (PackageManager.NameNotFoundException e)
            {
                return false;
            }
        }        

        private void MBtnLoginStandard_Click(object sender, EventArgs e)
        {
            mOnLoginComplete.Invoke(this, new OnLoginEventArgs(mTxtUserIdentifier.Text, mTxtPassword.Text));
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
        
        //was protected earlier
        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            //could be an error if called as part of another activityresult
           // mCallBackManager.OnActivityResult(requestCode, (int)resultCode, data);
        }        
    }
}