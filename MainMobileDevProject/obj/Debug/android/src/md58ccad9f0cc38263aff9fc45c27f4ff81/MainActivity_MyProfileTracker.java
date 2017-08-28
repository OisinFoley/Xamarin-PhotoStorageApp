package md58ccad9f0cc38263aff9fc45c27f4ff81;


public class MainActivity_MyProfileTracker
	extends com.facebook.ProfileTracker
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCurrentProfileChanged:(Lcom/facebook/Profile;Lcom/facebook/Profile;)V:GetOnCurrentProfileChanged_Lcom_facebook_Profile_Lcom_facebook_Profile_Handler\n" +
			"";
		mono.android.Runtime.register ("MainMobileDevProject.MainActivity+MyProfileTracker, MainMobileDevProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MainActivity_MyProfileTracker.class, __md_methods);
	}


	public MainActivity_MyProfileTracker () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MainActivity_MyProfileTracker.class)
			mono.android.TypeManager.Activate ("MainMobileDevProject.MainActivity+MyProfileTracker, MainMobileDevProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCurrentProfileChanged (com.facebook.Profile p0, com.facebook.Profile p1)
	{
		n_onCurrentProfileChanged (p0, p1);
	}

	private native void n_onCurrentProfileChanged (com.facebook.Profile p0, com.facebook.Profile p1);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
