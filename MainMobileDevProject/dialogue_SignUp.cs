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

namespace MainMobileDevProject
{
    //custom eventargs class
    public class OnSignUpEventArgs : EventArgs
    {
        //because these are private we will have to use the firstname, email properties otuside of this class, 
        //rather than nFirstName or nEmail
        private string mUsername;
        private string mEmail;
        private string mPassword;
        private Button mBtnPickImg;
        private ImageView mImgUpload;        


        //get and set just act like the properties of an object***
        //get implies read-only, set implies write only for the object
        //don't have to call explciity, can just call like a variable and c# will do the rest, 
        //we just use firstname as in (I think!...) --> get var name = firstname or firstname = John Doe

        public string Username
        {
            get { return mUsername; }
            //c# knows that value means set the value to whatever value we give when we initialise firstname
            set { mUsername = value; } 
        }

        public string Email
        {
            get { return mEmail; }
            set { mEmail = value; }
        }

        public string Password
        {
            get { return mPassword; }
            set { mPassword = value; }
        }

        public Button BtnImg
        {
            get { return mBtnPickImg; }
            //c# knows that value means set the value to whatever value we give when we initialise firstname
            set { mBtnPickImg = value; }
        }

        public ImageView ImgUpload
        {
            get { return mImgUpload; }
            //c# knows that value means set the value to whatever value we give when we initialise firstname
            set { mImgUpload = value; }
        }

        //calling base method of parent class
        public OnSignUpEventArgs(string username, string email, string password, ImageView image) : base()
        {
            //can use private variables here of course, or just our properties
            //wrench symbol signifies property
            //for next line, FirstName property takes the value specified in the constructor, and 'sets' the nFirstName value
            Username = username;
            Email = email;
            Password = password;
            ImgUpload = image;
            
        }

        public OnSignUpEventArgs(string username, string email, string password) : base()
        {            
            Username = username;
            Email = email;
            Password = password;            
        }
    }



    //first thing, must specify this inheritance in order to behave as a dialogue
    public class dialogue_SignUp : DialogFragment
    {
        private EditText mUsername;
        private EditText mTxtEmail;
        private EditText mTxtPassword;
        private Button mBtnSignUp;

        private Button mBtnPickPhoto;
        private ImageView mImgUploadPhoto;

        private TextView mTxtImgPath;
        

        //this event is going to take the event we just created. if not specified, it'll take the regular eventargs class
        public event EventHandler<OnSignUpEventArgs> mOnSignUpComplete;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            //create view the dialogue will hold
            //next we're taking a layout and placing it on top of our current view
            //attach it to our initial container, and false because we don't want to attach it to the root
            var view = inflater.Inflate(Resource.Layout.dialog_sign_up, container, false);

            mUsername = view.FindViewById<EditText>(Resource.Id.txtRegUsername);
            mTxtEmail = view.FindViewById<EditText>(Resource.Id.txtRegEmail);
            mTxtPassword = view.FindViewById<EditText>(Resource.Id.txtRegPassword);
            mBtnSignUp = view.FindViewById<Button>(Resource.Id.btnDialogEmail);

            mTxtImgPath = view.FindViewById<Button>(Resource.Id.txtImgPath);            

            mBtnPickPhoto = view.FindViewById<Button>(Resource.Id.btnOpenGallery);
            mImgUploadPhoto = view.FindViewById<ImageView>(Resource.Id.imgUploadPhoto);

            mBtnSignUp.Click += mBtnSignUp_Click;
            
            return view;
        }
        

        void mBtnSignUp_Click(object sender, EventArgs e)
        {
            //creating custom event arguments, see class at top
            //user has clciked sign up button
            //onclick we want to broadcast the event. 1 is invoke(takes a sender, who's broadcasting this)
            //can also write as the following, it'll know what we want based on ctor
            //invoke lets us know it's an event                        

            //mOnSignUpComplete.Invoke(this, new OnSignUpEventArgs(mTxtFirstName.Text, mTxtEmail.Text, mTxtPassword.Text, mImgUploadPhoto));
            mOnSignUpComplete.Invoke(this, new OnSignUpEventArgs(mUsername.Text, mTxtEmail.Text, mTxtPassword.Text));
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
        
        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);            
        }        
    }
}