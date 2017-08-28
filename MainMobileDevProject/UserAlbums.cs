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
using MySql.Data.MySqlClient;
using System.Data;

namespace MainMobileDevProject
{
    [Activity(Label = "User Albums List", Icon = "@drawable/xs")]
    public class UserAlbums : Activity
    {
        private BaseAdapter<Contact> mAdapter;
        private List<Contact> mContacts;
        private ListView mAlbumList;
        private ImageView mSelectedPic;
        private Button mBtnAddAlbum;
        public List<string> AlbumNameList = new List<string>();
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.userAlbums);

            string text = Intent.GetStringExtra("UsersID") ?? "No user available";
            System.Diagnostics.Debug.Write("EXTRA INTENT DATA IS  : " + Intent.GetStringExtra("UsersID"));

            mAlbumList = FindViewById<ListView>(Resource.Id.listAlbums);
            mContacts = new List<Contact>();

            Action<ImageView> action = PicSelected;
            //try to make another ctor later which doesnt need an actionimage button a the last param
            mAdapter = new ContactListAdapter(this, Resource.Layout.row_contact, mContacts, action);

            mBtnAddAlbum = FindViewById<Button>(Resource.Id.btnAddAlbum);
           

            //problem here...
            mAlbumList.Adapter = mAdapter;

            PopulateAlbumFeed();
            //mContacts.Add(new Contact() { Name = "OISIN", Number = "FOLEY" });       
            mContacts.Add(new Contact() { Name = "OISIN" });

            mAdapter.NotifyDataSetChanged();

            mBtnAddAlbum.Click += (object sender, EventArgs e) =>
            {
                //pull up dialog
                //this is used to pull up the dialog from the activity
                FragmentTransaction transaction = FragmentManager.BeginTransaction();
                dialogue_AddAlbum addAlbumDialog = new dialogue_AddAlbum();
                addAlbumDialog.Show(transaction, "dialog fragment");
                addAlbumDialog.mAddAlbumComplete += AddAlbumToDb;
                //dialogue_SignUp signUpDialog = new dialogue_SignUp();
                //signUpDialog.Show(transaction, "dialog fragment");

                ////signUpDialog.mOnSignUpComplete += signUpDialog_mOnSignUpComplete;
                //signUpDialog.mOnSignUpComplete += SignUpDialog_mOnSignUpComplete;
            };

            //setup some sort of click listener
            /*
             * .click+=(object sender, EventAgs e)=>
             * {
             *      intent i = new Intent(typeof(HomeNoScrollView));
             *      i.PutStringExtra("UserID",Intent.GetStringExtra("UsersID"));
             *      i.PutStringExtra("AlbumID", somehow grab e.albumid from listener );
             *      StartActivity(i);
             * }
             * 
             */

        }

        private void AddAlbumToDb(object sender, OnNewAlbumEventArgs e)
        {
            //wanted to do ternary if like in Javascript, but no luck here
            //e.Title.Length > 0 ? System.Diagnostics.Debug.Write("YOUR ENTERED ALBU M TITLE IS: " + e.Title): System.Diagnostics.Debug.Write("YOUR ENTERED ALBU M TITLE IS: ");
            //e.Title.Length > 0 ? System.Diagnostics.Debug.Write("YOUR ENTERED ALBU M TITLE IS: " + e.Title):);
            if (e.Title.Length > 0)
            {
                //if given empty album title we don't do anything
                MySqlConnection con = new MySqlConnection("Server=db4free.net;Port=3307;database=ofsligodb;User Id=ofoley1;Password=pinecone;charset=utf8");
                
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                        System.Diagnostics.Debug.WriteLine("CONNECTION OPEN*****: ");                        

                        MySqlCommand Readcmd = new MySqlCommand("INSERT INTO tblUserAlbum(UserID,Title) " +
                            "VALUES(@uid,@title)");
                        
                        Readcmd.Parameters.AddWithValue("@uid", Intent.GetStringExtra("UsersID"));
                        Readcmd.Parameters.AddWithValue("@title", e.Title.First().ToString().ToUpper() + e.Title.Substring(1));

                        Readcmd.Connection = con;
                        Readcmd.ExecuteNonQuery();

                        System.Diagnostics.Debug.WriteLine("INSERT SHOULD HAVE HAPPENED*****: ");
                        
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
                }

            }


        }

        private void PicSelected(ImageView selectedPic)
        {
            mSelectedPic = selectedPic;
            Intent intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            this.StartActivityForResult(Intent.CreateChooser(intent, "Selecte a Photo"), 0);
        }

        public void PopulateAlbumFeed()
        {
            MySqlConnection con = new MySqlConnection("Server=db4free.net;Port=3307;database=ofsligodb;User Id=ofoley1;Password=pinecone;charset=utf8");
            //List<string> AlbumList;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string query = "SELECT * FROM tblUserAlbum HAVING UserID =  '" + Intent.GetStringExtra("UsersID") +"'";
                //Intent.GetStringExtra("UsersID")
                
                MySqlCommand cmd = new MySqlCommand(query, con);

                System.Diagnostics.Debug.Write("TRYING TO READ");
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    AlbumNameList.Add(dataReader.GetString(2));
                    System.Diagnostics.Debug.Write("image still as base64 string is : " + dataReader.GetString(1));
                    //System.Diagnostics.Debug.Write("image converted from base64 found is : " + Convert.FromBase64String(dataReader.GetString(1)));
                }

                //foreach (string title in AlbumNameList)
                //{
                //    mContacts.Add(new Contact() { Name = title });
                //}

                for(int i =0; i<AlbumNameList.Count;i++)
                {
                    mContacts.Add(new Contact() { Name = i.ToString(), Number = AlbumNameList[i] });
                }

                //mContacts.Add(new Contact() { Name = "OISIN", Number = "FOLEY", Image = imageslist[0] });
                //System.Diagnostics.Debug.Write("imageslist lentth is: " + imageslist.Count);
                mAdapter.NotifyDataSetChanged();

                //System.Diagnostics.Debug.Write("image ID found is : " + imageslist[0]);
                //System.Diagnostics.Debug.Write("image ID found is : " + imageslist[1]);
            }


            catch (MySqlException ex)
            {
                //mTxtBase64String.Text = ex.ToString();
            }
            finally
            {
                con.Close();
                System.Diagnostics.Debug.Write("CONN CLOSED");
                System.Diagnostics.Debug.Write("Album ID count found is : " + AlbumNameList.Count);
            }
        }

    }
}