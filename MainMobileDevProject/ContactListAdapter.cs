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
using MySql.Data.MySqlClient;
using System.Data;

namespace MainMobileDevProject
{
    class ContactListAdapter : BaseAdapter<Contact>
    {
        private Context mContext;
        private int mLayout;
        private List<Contact> mContacts;
        private Action<ImageView> mActionPicSelected;

        public ContactListAdapter(Context context, int layout, List<Contact> contacts, Action<ImageView> picSelected)
        {
            mContext = context;
            mLayout = layout;
            mContacts = contacts;
            mActionPicSelected = picSelected;
        }

        public ContactListAdapter(Context context, int layout, List<Contact> contacts)
        {
            mContext = context;
            mLayout = layout;
            mContacts = contacts;
         
        }

        public override Contact this[int position]
        {
            get { return mContacts[position]; }
        }

        public override int Count
        {
            get { return mContacts.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            //row.notif
            if (row == null)
            {
                row = LayoutInflater.From(mContext).Inflate(mLayout, parent, false);
            }

            row.FindViewById<TextView>(Resource.Id.txtName).Text = mContacts[position].Name;
            row.FindViewById<TextView>(Resource.Id.txtNumber).Text = mContacts[position].Number;

            ImageView pic = row.FindViewById<ImageView>(Resource.Id.imgPic);
            row.Clickable = true;

            if (mContacts[position].Image != null)
            {
                pic.SetImageBitmap(BitmapFactory.DecodeByteArray(mContacts[position].Image, 0, mContacts[position].Image.Length));
            }

            if (mContacts[position].Image == null)
            {
                //here we should replace mcontacts[position] with imageslist[position], then it'll at least pull
                //the 2 byte[] images we have
                //pic.SetImageBitmap(BitmapFactory.DecodeByteArray(mContacts[position].Image, 0, mContacts[position].Image.Length));
                //pic.SetImageBitmap(BitmapFactory.DecodeByteArray(mContacts[position].Image, 0, mContacts[position].Image.Length));
            }

            //var imageButton = row.FindViewById<ImageView>(Resource.Id.imgPic);
            //imageButton.Focusable = false;
            //imageButton.FocusableInTouchMode = false;
            //imageButton.Clickable = true;

            var removeButton = row.FindViewById<ImageButton>(Resource.Id.btnDeleteAlbum);
            removeButton.Focusable = false;
            removeButton.FocusableInTouchMode = false;
            removeButton.Clickable = true;

            //imageButton.Click += (sender, args) =>
            //{
            //    System.Diagnostics.Debug.Write("myBitmap row id :AAAAAAAAAAAAAAAAAAAA ");
            //    Console.WriteLine("ImageButton {0} clicked", position);
                
            //};

            removeButton.Click += (sender, args) =>
            {
                System.Diagnostics.Debug.Write("myBitmap row id :AAAAAAAAAAAAAAAAAAAA ");
                Console.WriteLine("RemoveButton {0} clicked", position);
                
            };



            //pic.Click -= pic_Click;
            pic.Click += (object sender, EventArgs e) =>
            {
                MySqlConnection con = new MySqlConnection("Server=db4free.net;Port=3307;database=ofsligodb;User Id=ofoley1;Password=pinecone;charset=utf8");

                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                        System.Diagnostics.Debug.WriteLine("CONNECTION OPEN*****: ");

                                                

                        MySqlCommand Readcmd = new MySqlCommand("DELETE FROM tblUserAlbumPhoto WHERE UserID = '" + 1 + "' AND " +
                            "AlbumID = '" + 1 + "' and ID = '" + position + "'");


                        Readcmd.Connection = con;
                        Readcmd.ExecuteNonQuery();

                        System.Diagnostics.Debug.WriteLine("DELETE SHOULD HAVE HAPPENED*****: ");


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
            };

            return row;
        }

        

        void pic_Click(object sender, EventArgs e)
        {
            //Picture has been clicked
           
            System.Diagnostics.Debug.Write("myBitmap row id :AAAAAAAAAAAAAAAAAAAA " );            

            

        }
    }
}