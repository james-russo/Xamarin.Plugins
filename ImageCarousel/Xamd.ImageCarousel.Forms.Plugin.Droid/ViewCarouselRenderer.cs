﻿using System;
using Xamarin.Forms.Platform.Android;
using Android.Views;
using Xamd.ImageCarousel.Forms.Plugin.Droid;
using Xamarin.Forms;

[assembly: ExportRenderer (typeof(Xamd.ImageCarousel.Forms.Plugin.Abstractions.GenericCarousel), typeof(ViewCarouselRenderer))]
namespace Xamd.ImageCarousel.Forms.Plugin.Droid
{
	public class ViewCarouselRenderer : VisualElementRenderer<Xamd.ImageCarousel.Forms.Plugin.Abstractions.GenericCarousel>
	{
		readonly GestureListener listener;
		readonly GestureDetector detector; 
		public ViewCarouselRenderer ()
		{
			listener = new GestureListener ();
			detector = new GestureDetector (listener);

			listener.Swipe += Listener_Swipe;
		}

				

			/// <summary>
			/// Used for registration with dependency service
			/// </summary>
			public static void Init ()
			{
				var temp = DateTime.Now;
			}

			protected override void Dispose (bool disposing)
			{
				if (disposing) {
					listener.Swipe -= Listener_Swipe;
				}

				base.Dispose (disposing);
			}

			void Listener_Swipe (object sender, GestureListener.SwipeType e)
			{
				switch (e) {
				case GestureListener.SwipeType.Left:
					Element.OnSwipeLeft ();
					break;
				case GestureListener.SwipeType.Right:
					Element.OnSwipeRight ();
					break;
				}
			}

		protected override void OnElementChanged (ElementChangedEventArgs<Xamd.ImageCarousel.Forms.Plugin.Abstractions.GenericCarousel> e)
			{
				base.OnElementChanged (e);

				if (e.OldElement != null) {
					GenericMotion -= HandleGenericMotion;
					Touch -= HandleTouch;
				}

				if (e.NewElement != null) {
					this.GenericMotion += HandleGenericMotion;
					this.Touch += HandleTouch;
				}
			}

			void HandleTouch (object sender, TouchEventArgs e)
			{
				detector.OnTouchEvent (e.Event);
			}

			void HandleGenericMotion (object sender, GenericMotionEventArgs e)
			{
				detector.OnTouchEvent (e.Event);
			}

			class GestureListener : GestureDetector.SimpleOnGestureListener
			{
				public enum SwipeType
				{
					Left,
					Right
				}

				const int SWIPE_THRESHOLD = 100;
				const int SWIPE_VELOCITY_THRESHOLD = 100;

				public event EventHandler<SwipeType> Swipe;

				public override bool OnFling (MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
				{					
					bool result = false;

					try {
						float diffY = e2.GetY () - e1.GetY ();
						float diffX = e2.GetX () - e1.GetX ();

						if (Math.Abs (diffX) > Math.Abs (diffY)) {
							if (Math.Abs (diffX) > SWIPE_THRESHOLD && Math.Abs (velocityX) > SWIPE_VELOCITY_THRESHOLD) {
								if (diffX > 0) {
									if (Swipe != null) {
										Swipe (this, SwipeType.Right);
									}
								} else {
									if (Swipe != null) {
										Swipe (this, SwipeType.Left);
									}								
								}
							}

							result = true;
						}

						result = true;
					} catch (Exception ex) {
						Console.WriteLine (ex.Message);
					}

					return result;
				}
			}
		}
}