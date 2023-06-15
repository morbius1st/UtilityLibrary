using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms;
using Point = System.Windows.Point;
using System.Windows.Media;

using Color = System.Windows.Media.Color;


namespace UtilityLibrary
{
	/// <summary>
	/// Interaction logic for Balloon.xaml
	/// </summary>
	public partial class Balloon : Window, INotifyPropertyChanged
	{
		#region + Preface

		public static double BALLOON_MARGIN_B_L = 16.0;
		public static double ARROW_HEIGHT = 20.0;

		public enum BalloonOrientation
		{
			BOTTOM_LEFT = -1,
			BOTTOM_RIGHT = -2,
			TOP_RIGHT = 2,
			TOP_LEFT = 1
		}

		private const int MAX_TIME = 10000;
		private const double CORNER_RADIUS_MINIMUM = 8.0;
		private const double LEFT_OR_RIGHT_MARGIN = 5.0;
		private const double ARROW_MARGIN_BASE = 1.0;

		private Window             win;
		private FrameworkElement   fe;
		private ResourceDictionary r;

		private readonly Color COLOR_TIP = Color.FromRgb(0x67, 0xAE, 0xFA);
		private readonly Color COLOR_TOP = Color.FromRgb(0xDA, 0xEB, 0xFF);
		private readonly Color COLOR_STOP1 = Color.FromRgb(0xBA, 0xDF, 0xFF);
		private readonly Color COLOR_STOP2 = Color.FromRgb(0x97, 0xCF, 0xFF);
		private readonly Color COLOR_BOTTOM = Color.FromRgb(0x2A, 0x82, 0xE1);
		private readonly Color COLOR_TEXT = Colors.Black;

		private readonly double[] _cornerRadii = new [] { 10.0, 14.0, 20.0 , 24.0};

		private Thickness arrowMargin;

		private CornerRadius _crx = new CornerRadius(10);

		private BalloonOrientation _orientation;

		private double _screenXScaleFactor;
		private double _screenYScaleFactor;

		private Vector _userAdjustment = new Vector();

		private LinearGradientBrush _pointerGradientAbove;
		private LinearGradientBrush _pointerGradientBelow;
		private LinearGradientBrush _mainGradient;
		private SolidColorBrush _textColor;

		private int _fadeOutDuration = 500;
		private int _fadeInDuration  = 500;
		private int _pauseTime       = 1000;

		private bool showArrow = true;

		// keep at least 5 pixles between the screen edge and the balloon

		private Vector _internalAdjustment;

		#endregion

		#region + Constructor

		public Balloon(Window win, FrameworkElement fe, string message)
		{
			InitializeComponent();

			this.win = win;
			this.fe  = fe;

			r = this.Resources;

			SetText = message;

			// this is the default orientation
			Orientation = BalloonOrientation.BOTTOM_RIGHT;

			DefaultColors();
			GetScreenScaleFactors();
		}

		#endregion

		#region + Properties

		public double X
		{
			get => _userAdjustment.X;
			set => _userAdjustment.X = value;
		}

		public double Y
		{
			get => _userAdjustment.Y;
			set => _userAdjustment.Y = value;
		}

		public LinearGradientBrush PointerGradientAbove
		{
			get => _pointerGradientAbove;
			set
			{
				_pointerGradientAbove = value;
				OnPropertyChange();
			}
		}

		public LinearGradientBrush PointerGradientBelow
		{
			get => _pointerGradientBelow;
			set
			{
				_pointerGradientBelow = value;
				OnPropertyChange();
			}
		}
		public LinearGradientBrush MainGradient
		{
			get => _mainGradient;
			set
			{
				_mainGradient = value;
				OnPropertyChange();
			}
		}

		public SolidColorBrush TextColor
		{
			get => _textColor;
			set
			{
				_textColor = value;
				OnPropertyChange();
			}
		}

		public BalloonOrientation Orientation
		{
			get => _orientation;

			set
			{
				if (_orientation != value)
				{
					_orientation = value;
					AdjustPointerForOrientation();
				}
			}
		}

		public double CornerRadius { get; set; } = CORNER_RADIUS_MINIMUM;

		public  CornerRadius CornerRad
		{
			get => _crx;
			set
			{
				_crx = value;
				OnPropertyChange();

				AdjustArrowMargin();
			}
		}


		public bool ShowArrow
		{
			get => showArrow;
			set
			{
				showArrow = value;

				AdjustPointerForOrientation();
			}
		}

		public Thickness ArrowMargin
		{
			get => arrowMargin;
			set
			{
				arrowMargin = value;
				OnPropertyChange();
			}
		}

		public int FadeInDuration
		{
			get => _fadeInDuration;
			set
			{
				if (value > 0 && value < MAX_TIME)
				{
					FiDuration = new Duration(GetTimeSpan(value));
				}
			}
		}

		public Duration FiDuration
		{
			get => new Duration(GetTimeSpan(FadeInDuration));
			set
			{
				int millisecs = GetMilliSecs(value.TimeSpan);

				if (millisecs > 0 && millisecs < MAX_TIME)
				{
					_fadeInDuration = millisecs;
					OnPropertyChange();
				}
			}
		}

		public int PauseTime
		{
			get => _pauseTime;
			set
			{
				if (value > 0 && value < MAX_TIME)
				{
					PauseTimeSpan = GetTimeSpan(value);
				}
			}
		}

		public TimeSpan PauseTimeSpan
		{
			get => GetTimeSpan(PauseTime);
			set
			{
				int millisecs = GetMilliSecs(value);
				if (millisecs > 0 && millisecs < MAX_TIME)
				{
					_pauseTime = millisecs;
					BeginTime = GetTimeSpan(PauseTime + FadeInDuration);
				}
			}
		}

		// the begin time for the fade out animation
		public TimeSpan BeginTime
		{
			get => GetTimeSpan(PauseTime + FadeInDuration);
			set
			{
				int millisecs = GetMilliSecs(value);

				if (millisecs > 0 && millisecs < MAX_TIME)
				{
					_pauseTime = millisecs - FadeInDuration;
					OnPropertyChange();
				}
			}
		}

		public int FadeOutDuration
		{
			get => _fadeOutDuration;
			set
			{
				if (value > 0 && value < MAX_TIME)
				{
					FoDuration = new Duration(GetTimeSpan(value));
				}
			}
		}

		public Duration FoDuration
		{
			get => new Duration(GetTimeSpan(FadeOutDuration));
			set
			{
				int millisecs = GetMilliSecs(value.TimeSpan);

				if (millisecs > 0 && millisecs < MAX_TIME)
				{
					_fadeOutDuration = millisecs;
					OnPropertyChange();
				}
			}
		}

		#region + Private


		private string SetText
		{
			set
			{
				string msg = value ?? "null message";

				textBlock.Text = msg;

				int returnCount = Math.Min(msg.Length - msg.Replace("\n", "").Length, 2);

				CornerRadius = _cornerRadii[Math.Min(returnCount, _cornerRadii.Length - 1)];
			}
		}

		#endregion

		#endregion

		#region + Methods

		#region + Private

		private void DefaultColors()
		{
			LinearGradientBrush lgb = new LinearGradientBrush();
			lgb.StartPoint = new Point(0.5, 0.0);
			lgb.EndPoint = new Point(0.5, 0.9);
			lgb.GradientStops.Add(new GradientStop(COLOR_TIP, 0.0));
			lgb.GradientStops.Add(new GradientStop(COLOR_TOP, 1.0));

			PointerGradientAbove = lgb;
			
			lgb = new LinearGradientBrush();
			lgb.StartPoint = new Point(0.5, 0.9);
			lgb.EndPoint = new Point(0.5, -0.15);
			lgb.GradientStops.Add(new GradientStop(COLOR_TIP, 0.0));
			lgb.GradientStops.Add(new GradientStop(COLOR_BOTTOM, 1.0));

			PointerGradientBelow = lgb;

			lgb = new LinearGradientBrush();
			lgb.StartPoint = new Point(0.5, 0.0);
			lgb.EndPoint = new Point(0.5, 1.0);
			lgb.GradientStops.Add(new GradientStop(COLOR_TOP   , 0.0));
			lgb.GradientStops.Add(new GradientStop(COLOR_STOP1 , 0.3));
			lgb.GradientStops.Add(new GradientStop(COLOR_STOP2 , 0.7));
			lgb.GradientStops.Add(new GradientStop(COLOR_BOTTOM, 1.0));

			MainGradient = lgb;

			TextColor = new SolidColorBrush(COLOR_TEXT);
		}

		// calculate the number of milli seconds
		// using only the seconds and milliseconds
		// values from the timespan
		private int GetMilliSecs(TimeSpan timeSpan)
		{
			return timeSpan.Seconds * 1000 + timeSpan.Milliseconds;
		}

		private TimeSpan GetTimeSpan(int millisecs)
		{
			return new TimeSpan(0, 0, 0, 0, millisecs);
		}

		private CornerRadius GetCornerRadius()
		{
			// set the corner radius but validate its size first
			// first - force at least the minimum
			double rad = Math.Max(CornerRadius, CORNER_RADIUS_MINIMUM);

			// make sure that the radius fits the control size - not too large
			// validate versus the smallest of actual height versus the actual width
			// however, don't let it get too small either
			rad = Math.Max(Math.Min(rad, Math.Min(border.ActualWidth / 2, border.ActualHeight / 2)), CORNER_RADIUS_MINIMUM);

			return new CornerRadius(rad);
		}

		private void AdjustPointerForOrientation()
		{
			// must adjust:
			// which pointer is visible / collapsed

			// make sure all are collapsed to start
			pointerForBottomRight.Visibility = Visibility.Collapsed;
			pointerForBottomLeft.Visibility = Visibility.Collapsed;
			pointerForTopRight.Visibility = Visibility.Collapsed;
			pointerForTopLeft.Visibility = Visibility.Collapsed;

			if (!ShowArrow) return;

			// the default is all pointers off
			// turn on the correct pointer
			switch (_orientation)
			{
			case BalloonOrientation.BOTTOM_RIGHT:
				{
					// bottom right uses the TL pointer
					pointerForBottomRight.Visibility = Visibility.Visible;
					break;
				}
			case BalloonOrientation.BOTTOM_LEFT:
				{
					// bottom left uses the TR pointer
					pointerForBottomLeft.Visibility = Visibility.Visible;
					break;
				}
			case BalloonOrientation.TOP_LEFT:
				{
					// top left uses the BR pointer
					pointerForTopLeft.Visibility = Visibility.Visible;
					break;
				}
			case BalloonOrientation.TOP_RIGHT:
				{
					// top right uses the BL pointer
					pointerForTopRight.Visibility = Visibility.Visible;
					break;
				}
			}
		}

		private void AdjustArrowMargin()
		{
			// if (!ShowArrow) return;

			switch (_orientation)
			{
			case BalloonOrientation.BOTTOM_RIGHT:
				{
					// bottom right uses the TL pointer
					ArrowMargin = new Thickness(ARROW_MARGIN_BASE + _crx.TopLeft, 0, 0, 0);
					break;
				}
			case BalloonOrientation.BOTTOM_LEFT:
				{
					// bottom left uses the TR pointer
					ArrowMargin = new Thickness(0, 0, ARROW_MARGIN_BASE + _crx.TopRight, 0);
					break;
				}
			case BalloonOrientation.TOP_RIGHT:
				{
					// top right uses the BL pointer
					ArrowMargin = new Thickness(ARROW_MARGIN_BASE + _crx.BottomLeft, 0, 0, 0);
					break;
				}
			case BalloonOrientation.TOP_LEFT:
				{
					// top left uses the BR pointer
					ArrowMargin = new Thickness(0, 0, (ARROW_MARGIN_BASE + _crx.BottomRight), 0);
					break;
				}
			}
		}

		private Rectangle GetScaledScreenSize()
		{
			Rectangle result = new Rectangle();

			Point p1 = fe.TranslatePoint(new Point(0, 0), win);

			Point p2 =  win.PointToScreen(new Point(0, 0));

			Screen screen = Screen.FromPoint(new System.Drawing.Point(
				(int) (p1.X + p2.X + fe.ActualWidth / 2), 
				(int) (p1.Y + p2.Y + fe.ActualHeight / 2)));

			result = screen.Bounds.Scale(_screenXScaleFactor, _screenXScaleFactor);
	
			return result;
		}

		private Point CalcWinPosition()
		{
			// get info for the screen that the main window
			// is currently positioned
			Rectangle screen = GetScaledScreenSize();

			Point winCorner = CalcControlTlCorner();

			Point winPosition = Point.Add(winCorner, CalcOrientationAdjust());

			winPosition = Point.Add(winPosition, _userAdjustment);
			
			// determine if the balloon is off the screen.
			// adjust and re-calculate
			if (IsWinOffScreen(winPosition, screen))
			{
				winPosition = Point.Add(winCorner, CalcOrientationAdjust());

				winPosition = Point.Add(winPosition, _userAdjustment);

				winPosition = Point.Add(winPosition, _internalAdjustment);
			}

			return winPosition;
		}

		private void GetScreenScaleFactors()
		{
			// get the screen scaling factors
			PresentationSource src = PresentationSource.FromVisual(fe);
			Matrix mx  = src.CompositionTarget.TransformFromDevice;

			_screenXScaleFactor = mx.M11;
			_screenYScaleFactor = mx.M22;
		}

		private Point CalcControlTlCorner()
		{
			// p is the control's TL corner relative to the 
			// window's client area
			Point p = fe.TranslatePoint(new Point(0, 0), win);

			// p2 is the TL parent window corner in screen coordinates (un-scaled)
			Point p2 = win.PointToScreen(new Point(0, 0));

			// calculate the scaled screen coordinate for the parent's
			// TL corner
			double winX = p2.X * _screenXScaleFactor;
			double WinY = p2.Y * _screenYScaleFactor;

			return new Point(winX + p.X, WinY + p.Y);
		}

		private Vector CalcOrientationAdjust()
		{
			Vector vector = new Vector(0.0, 0.0);
			// this method only works after the loaded event is called
			// as the actual height width are not valid otherwise

			if (_orientation == BalloonOrientation.BOTTOM_RIGHT ||
				_orientation == BalloonOrientation.BOTTOM_LEFT)
			{
				vector.Y = fe.ActualHeight;
			}
			else
			{
				// vector.Y = -1.0 * (this.ActualHeight - BALLOON_MARGIN_B_L + 
				// 	(ShowArrow ? ARROW_HEIGHT : 0));
				vector.Y = -1.0 * (this.ActualHeight);
			}

			if (_orientation == BalloonOrientation.BOTTOM_LEFT ||
				_orientation == BalloonOrientation.TOP_LEFT)
			{
				vector.X = fe.ActualWidth - this.ActualWidth + BALLOON_MARGIN_B_L;
			}

			return vector;
		}

		private bool IsWinOffScreen(Point winPosition, Rectangle screen)
		{
			bool result = false;

			_internalAdjustment = new Vector(0.0, 0.0);

			// negative numbers mean it is past the screen edge;
			double top = winPosition.Y - screen.Top;
			double left = winPosition.X - screen.Left;
			double right = screen.Right - (winPosition.X + ActualWidth);
			double bottom = screen.Bottom - (winPosition.Y + ActualWidth);


			// the below assumes that the balloon cannot be 
			// above and below the screen edges and cannot be
			// past the left and right screen edges

			// is the window past the screens left edge
			if (top < 0 || bottom < 0)
			{
				// window is above screen's top edge
				Orientation = FlipOrientation(_orientation);
				this.UpdateLayout();
				result = true;
			} 

			if (left < 0)
			{
				// window is past screen's left edge
				// move the window to the right (make a positive amount)
				_internalAdjustment.X = -1 * left + LEFT_OR_RIGHT_MARGIN;
				result = true;
			} 
			else if (right < 0)
			{
				// window is past screen's right edge
				// move the window to the left (use a negative amount)
				_internalAdjustment.X = right - LEFT_OR_RIGHT_MARGIN;
				result = true;
			}

			return result;
		}

		private BalloonOrientation FlipOrientation(BalloonOrientation orientation)
		{
			return (BalloonOrientation) (-1 * (int) orientation);
		}

		#endregion


		#endregion

		#region + Events

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChange([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

		public void CloseWindow(object sender, EventArgs args)
		{
			Close();
		}

		private void Balloon_Loaded(object sender, RoutedEventArgs e)
		{
			Point TL = CalcWinPosition();

			Left = TL.X;
			Top = TL.Y;

			// update the properties with their
			// current values
			CornerRad = GetCornerRadius();
		}

		#endregion

	}

	static class EBalloonxtensions
	{
		public static Rectangle Scale(this Rectangle rc, double scaleFactorX, double scaleFactorY)
		{
			return new Rectangle(
			(int)(rc.Left * scaleFactorX),
			(int)(rc.Top * scaleFactorY),
			(int)(rc.Width * scaleFactorX),
			(int)(rc.Height * scaleFactorY));
		}

	}
}

namespace DesignTimeProperties
{
	public static class d
	{
		static bool? inDesignMode;

		/// <summary>
		/// Indicates whether or not the framework is in design-time mode. (Caliburn.Micro implementation)
		/// </summary>
		private static bool InDesignMode
		{
			get
			{
				if (inDesignMode == null)
				{
					var prop = DesignerProperties.IsInDesignModeProperty;
					inDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;

					if (!inDesignMode.GetValueOrDefault(false) && System.Diagnostics.Process.GetCurrentProcess()
							.ProcessName.StartsWith("devenv", System.StringComparison.Ordinal))
						inDesignMode = true;
				}

				return inDesignMode.GetValueOrDefault(false);
			}
		}

		public static DependencyProperty BackgroundProperty = DependencyProperty.RegisterAttached(
			"Background", typeof(System.Windows.Media.Brush), typeof(d),
			new PropertyMetadata(new PropertyChangedCallback(BackgroundChanged)));

		public static System.Windows.Media.Brush GetBackground(DependencyObject dependencyObject)
		{
			return (System.Windows.Media.Brush)dependencyObject.GetValue(BackgroundProperty);
		}
		public static void SetBackground(DependencyObject dependencyObject, System.Windows.Media.Brush value)
		{
			dependencyObject.SetValue(BackgroundProperty, value);
		}
		private static void BackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!InDesignMode)
				return;

			d.GetType().GetProperty("Background").SetValue(d, e.NewValue, null);
		}

		public static DependencyProperty CornerRadiusProperty = DependencyProperty.RegisterAttached(
			"CornerRadius", typeof(System.Windows.CornerRadius), typeof(d),
			new PropertyMetadata(new PropertyChangedCallback(CornerRadiusChanged)));

		public static System.Windows.CornerRadius GetCornerRadius(DependencyObject dependencyObject)
		{
			return (System.Windows.CornerRadius)dependencyObject.GetValue(CornerRadiusProperty);
		}
		public static void SetCornerRadius(DependencyObject dependencyObject, System.Windows.CornerRadius value)
		{
			dependencyObject.SetValue(CornerRadiusProperty, value);
		}
		private static void CornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (!InDesignMode)
				return;

			d.GetType().GetProperty("CornerRadius").SetValue(d, e.NewValue, null);
		}
	}
}
