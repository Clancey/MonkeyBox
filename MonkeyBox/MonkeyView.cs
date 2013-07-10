using System;
using MonoTouch.UIKit;

namespace MonkeyBox
{
	public class MonkeyView : UIControl
	{
		UIImageView image;
		public MonkeyView (string monkeyName)
		{
			image = new UIImageView (UIImage.FromBundle (monkeyName));
			this.AddSubview (image);
			this.Frame = image.Frame;
		}

		public PlayGroundView CurrentPlayground {get;set;}

		public override void MovedToSuperview ()
		{
			base.MovedToSuperview ();
			if (this.Superview is PlayGroundView)
				CurrentPlayground = (PlayGroundView)this.Superview;
		}
		public override void TouchesBegan (MonoTouch.Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			CurrentPlayground.CurrentMonkey = this;
		}
	}
}

