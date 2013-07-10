using System;
using MonoTouch.UIKit;
using System.Linq;
using MonoTouch.CoreImage;
using System.Drawing;
using System.Collections.Generic;

namespace MonkeyBox
{
	public class PlaygroundViewController : UIViewController
	{
		PlayGroundView PlayGroundView;

		public PlaygroundViewController ()
		{
		}
		public override void ViewWillAppear (bool animated)
		{
			DropboxDatabase.Shared.MonkeysUpdated += HandleMonkeysUpdated;
			PlayGroundView.UpdateMonkeys (DropboxDatabase.Shared.Monkeys);
			base.ViewWillAppear (animated);
		}
		public override void ViewDidDisappear (bool animated)
		{
			DropboxDatabase.Shared.MonkeysUpdated -= HandleMonkeysUpdated;
			base.ViewDidDisappear (animated);
		}

		void HandleMonkeysUpdated (object sender, EventArgs e)
		{
			PlayGroundView.UpdateMonkeys (DropboxDatabase.Shared.Monkeys);
		}

		public override void LoadView ()
		{
			View = PlayGroundView = new PlayGroundView ();
		}

		public void UpdateMonkey()
		{

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

			this.BackgroundColor = UIColor.DarkGray;
		}
		Dictionary<Monkey, MonkeyView> MonkeyDictionary = new Dictionary<Monkey, MonkeyView> ();
		public void UpdateMonkeys(Monkey[] monkeys)
		{
			UIView.BeginAnimations ("monkeys");
			for(int i = 0; i < monkeys.Length; i ++){
				Monkey monkey = monkeys[i];
				MonkeyView view;
				MonkeyDictionary.TryGetValue(monkey,out view);
				if (view == null){
					view = new MonkeyView (monkey);
					MonkeyDictionary.Add(monkey,view);
				}
				view.Update (monkey, this.Bounds);
				this.InsertSubview(view,i);
			}
			UIView.CommitAnimations ();
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

