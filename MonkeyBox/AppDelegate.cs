using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
//using DropBoxSync.iOS;

namespace MonkeyBox
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		const string DropboxSyncKey = "zva4sa0z5xbz3cu";
		const string DropboxSyncSecret = "tdehe8w2hx5phvt";
		// class-level declarations
		UIWindow window;
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
//			var manager = new DBAccountManager (DropboxSyncKey, DropboxSyncSecret);
//			DBAccountManager.SharedManager = manager;
//
//			var account = manager.LinkedAccount;
//			if (account != null) {
//				var filesystem = new DBFilesystem (account);
//				DBFilesystem.SharedFilesystem = filesystem;
//			}	
			// create a new window instance based on the screen size
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			window.RootViewController = new PlaygroundViewController ();

			// make the window visible
			window.MakeKeyAndVisible ();

			
			return true;
		}
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

