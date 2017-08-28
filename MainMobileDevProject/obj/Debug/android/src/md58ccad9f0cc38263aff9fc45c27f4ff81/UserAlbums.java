package md58ccad9f0cc38263aff9fc45c27f4ff81;


public class UserAlbums
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("MainMobileDevProject.UserAlbums, MainMobileDevProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", UserAlbums.class, __md_methods);
	}


	public UserAlbums () throws java.lang.Throwable
	{
		super ();
		if (getClass () == UserAlbums.class)
			mono.android.TypeManager.Activate ("MainMobileDevProject.UserAlbums, MainMobileDevProject, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
