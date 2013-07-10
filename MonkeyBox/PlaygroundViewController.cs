using System;
using MonoTouch.UIKit;
using System.Linq;
using MonoTouch.CoreImage;
using System.Drawing;

namespace MonkeyBox
{
	public class PlaygroundViewController : UIViewController
	{
		PlayGroundView PlayGroundView;

		public PlaygroundViewController ()
		{
		}

		public override void LoadView ()
		{
			View = PlayGroundView = new PlayGroundView ();
		}
	}

	public class PlayGroundView : UIView
	{
		UIPinchGestureRecognizer pinchGesture;

		public PlayGroundView ()
		{
			pinchGesture = new UIPinchGestureRecognizer (Scale);
			this.AddGestureRecognizer (pinchGesture);

			var rotationGesture = new UIRotationGestureRecognizer (Rotate);
			this.AddGestureRecognizer (rotationGesture);

			var panGesture = new UIPanGestureRecognizer (Move);
			this.AddGestureRecognizer (panGesture);

			this.AddSubview (new MonkeyView ("Fred"));
			this.AddSubview (new MonkeyView ("George"));
			this.AddSubview (new MonkeyView ("Hootie"));
			this.AddSubview (new MonkeyView ("Julian"));
			this.AddSubview (new MonkeyView ("Nim"));
			this.AddSubview (new MonkeyView ("Pepe"));
			this.BackgroundColor = UIColor.DarkGray;
		}

		MonkeyView currentMonkey;

		public MonkeyView CurrentMonkey {
			get {
				if (currentMonkey == null)
					currentMonkey = Subviews.FirstOrDefault () as MonkeyView;
				return currentMonkey;
			}
			set {
				if (currentMonkey == value)
					return;
				currentMonkey = value;
				this.BringSubviewToFront (currentMonkey);
			}
		}

		float lastScale = 1f;

		void Scale (UIPinchGestureRecognizer gesture)
		{
			try {
				if (CurrentMonkey == null)
					return;
				if (gesture.State == UIGestureRecognizerState.Began)
					lastScale = 1f;
				var scale = 1f - (lastScale - gesture.Scale);

				var transform = CurrentMonkey.Transform;
				transform.Scale (scale, scale);
				CurrentMonkey.Transform = transform;

				Console.WriteLine (transform);
				lastScale = gesture.Scale;
				Console.WriteLine (scale);
			} catch (Exception ex) {
				Console.WriteLine (ex);
			}

		}

		float lastRotation = 0f;

		void Rotate (UIRotationGestureRecognizer gesture)
		{
			if (CurrentMonkey == null)
				return;
			if (gesture.State == UIGestureRecognizerState.Ended) {
				lastRotation = 0;
				return;
			}
			var rotation = 0 - (lastRotation - gesture.Rotation);
			var transform = CurrentMonkey.Transform;
			transform.Rotate (rotation);
			CurrentMonkey.Transform = transform;

			lastRotation = gesture.Rotation;
		}

		PointF initialPoint;

		void Move (UIPanGestureRecognizer gesture)
		{
			if (CurrentMonkey == null)
				return;
			var point = gesture.TranslationInView (this);

			if (gesture.State == UIGestureRecognizerState.Began)
				initialPoint = CurrentMonkey.Center;

			point.X += initialPoint.X;
			point.Y += initialPoint.Y;

			CurrentMonkey.Center = point;
		}
	}
}

