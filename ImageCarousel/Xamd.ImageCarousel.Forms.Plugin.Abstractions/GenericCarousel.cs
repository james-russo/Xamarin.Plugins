using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace Xamd.ImageCarousel.Forms.Plugin.Abstractions
{
	public class GenericCarousel : AbsoluteLayout
	{
		public static readonly BindableProperty ViewsProperty = BindableProperty.Create<GenericCarousel, ObservableCollection<View>> (p => p.ContentViews, default(ObservableCollection<View>));

		public ObservableCollection<View> ContentViews {
			get { return (ObservableCollection<View>)GetValue (ViewsProperty); }
			set { SetValue (ViewsProperty, value); }
		}

		public View CurrentView { get; private set; }

		private Timer timer;

		private double TimeOutDuration { get; set; }


		public GenericCarousel ()
		{
			ContentViews = new ObservableCollection<View> ();
		}

		protected override void LayoutChildren (double x, double y, double width, double height)
		{
			base.LayoutChildren (x, y, width, height);

			//fix layout issues caused by base behavior, make sure these things are in the right place before swiping begins
			var point = Point.Zero;

			foreach (View image in ContentViews) {
				image.Layout (new Rectangle (point, image.Bounds.Size));
				point = new Point (point.X + image.Width + this.Bounds.Width, 0);

			}
		}

		protected override void OnPropertyChanged (string propertyName = null)
		{
			base.OnPropertyChanged (propertyName);

			//if the Images property has changed, clear our ImageList of images and add all the new images as children
			if (propertyName == ViewsProperty.PropertyName && ContentViews != null) {
				ContentViews.CollectionChanged += Images_CollectionChanged;

				addImagesAsChildren ();
			}
		}

		protected override void OnChildAdded (Element child)
		{
			base.OnChildAdded (child);

			//each time a child Image is added, add it to the ImageList
			if (child is View) {
				//set a CurrentImage if we don't already have one
				if (CurrentView == null) {
					CurrentView = child as View;
				}
			}
		}

		public void OnSwipeLeft ()
		{
			var imageNumber = ContentViews.IndexOf (CurrentView);
			var nextNumber = imageNumber == ContentViews.Count - 1 ? 0 : imageNumber + 1;
			var nextImage = ContentViews [nextNumber];

			//make sure this image is in position to be animated in
			nextImage.Layout (new Rectangle (new Point (CurrentView.Width, 0), CurrentView.Bounds.Size));

			var current = CurrentView;

			current.LayoutTo (new Rectangle (-(this.Bounds.Width + this.Width + CurrentView.Width), 0, CurrentView.Width, CurrentView.Height));
			CurrentView = nextImage;
			nextImage.LayoutTo (new Rectangle (0, 0, CurrentView.Width, CurrentView.Height));
		}

		public void OnSwipeRight ()
		{
			var imageNumber = ContentViews.IndexOf (CurrentView);
			var nextNumber = imageNumber == 0 ? ContentViews.Count - 1 : imageNumber - 1;
			var nextImage = ContentViews [nextNumber];

			//make sure this image is in position to be animated in
			nextImage.Layout (new Rectangle (new Point (-CurrentView.Width, 0), CurrentView.Bounds.Size));

			var current = CurrentView;

			current.LayoutTo (new Rectangle ((this.Bounds.Width + this.Width + CurrentView.Width), 0, CurrentView.Width, CurrentView.Height));
			CurrentView = nextImage;
			nextImage.LayoutTo (new Rectangle (0, 0, CurrentView.Width, CurrentView.Height));
		}

		void Images_CollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			foreach (var item in e.NewItems) {
				var image = item as View;
				if (image != null) {
					addImageAsChild (image);
				}
			}
		}



		void addImagesAsChildren ()
		{
			foreach (var image in ContentViews) {
				addImageAsChild (image);
			}
		}

		public void StartTimer(int secondsDelay)
		{
			TimeOutDuration = TimeSpan.FromSeconds(secondsDelay).TotalMilliseconds;

			if (timer != null)
			{
				timer.Dispose();
				timer = null;
			}

			if (TimeOutDuration > 0 && timer == null)
			{
				timer = new Timer(OnTimerTick, null, (int)TimeOutDuration, (int)TimeOutDuration);
			}
		}

		void OnTimerTick(object state)
		{			
			Xamarin.Forms.Device.BeginInvokeOnMainThread (() => OnSwipeLeft ());
		}

		void addImageAsChild (View image)
		{
			var point = Point.Zero;
			this.Children.Add (image, new Rectangle (point, new Size (1, 1)), AbsoluteLayoutFlags.SizeProportional);
			point = new Point (point.X + image.Width, 0);
		}
	}
}

