using System.Threading;
using System.Windows;
using UtilityLibrary;
using static UtilityLibrary.Balloon;

namespace WpfUtilityLibrary
{
	/// <summary>
	/// Interaction logic for WpfUtilLibMainWin.xaml
	/// </summary>
	public partial class WpfUtilLibMainWin : Window
	{
		public WpfUtilLibMainWin()
		{
			InitializeComponent();

			this.Top = 700;
			this.Left = 1800;
		}

		private void btnBalloonBL_Click(object sender, RoutedEventArgs e)
		{

//			Balloon b = new Balloon(this, btnBalloonBL, "I've been pressed\nSecond line of text");
//
			// default orientation is
			// bottom right
			//			b.CornerRadius = 10.0;
			//			b.Show();
			//
			//			// sleep for (1/2) seconds
			//			Thread.Sleep(500);
			//
			//			b = new Balloon(this, btnBalloonBL, "I've been pressed\nSecond line of text");
			//			b.CornerRadius = 15.0;
			//			b.Orientation = BalloonOrientation.BottomLeft;
			//			b.PauseTime = 2000;
			//			b.ShowDialog();
			//
			//			// sleep for (1/2) seconds
			//			Thread.Sleep(500);
			//
			//			b = new Balloon(this, btnBalloonBL, "I've been pressed\nSecond line of text");
			//			b.CornerRadius = 20.0;
			//			b.Orientation = BalloonOrientation.TopRight;
			//			b.PauseTime = 3000;
			//			b.ShowDialog();
			//
			//			// sleep for (1/2) seconds
			//			Thread.Sleep(500);
			//
			//			b = new Balloon(this, btnBalloonBL, "I've been pressed\nSecond line of text");
			//			b.CornerRadius = 25.0;
			//			b.Orientation = BalloonOrientation.TopLeft;
			//			b.DurationFadeIn = 200;
			//			b.PauseTime = 3000;
			//			b.FadeOutDuration = 200;
			//			b.ShowDialog();


			Balloon bBL = new Balloon(this, btnBalloonBL, "Bottom Left\nSecond line of text");
			bBL.Orientation = BalloonOrientation.BOTTOM_LEFT;
			bBL.X = -20;
			bBL.Show();
		}

		private void btnBalloonBR_Click(object sender, RoutedEventArgs e)
		{			
			Balloon bBR = new Balloon(this, btnBalloonBR, "Bottom Right\nSecond line of text");
			bBR.Orientation = BalloonOrientation.BOTTOM_RIGHT;
			bBR.X = 20;
			bBR.Show();

		}

		private void btnBalloonTL_Click(object sender, RoutedEventArgs e)
		{
			Balloon bTL = new Balloon(this, btnBalloonTL, "Top Left\nSecond line of text");
			bTL.Orientation = BalloonOrientation.TOP_LEFT;
			bTL.X = -20;
			bTL.Show();
		}


		private void btnBalloonTR_Click(object sender, RoutedEventArgs e)
		{
			Balloon bTR = new Balloon(this, btnBalloonTR, "Top Right\nSecond line of text");
			bTR.Orientation = BalloonOrientation.TOP_RIGHT;
			bTR.X = 20;
			bTR.Show();
		}
	}
}
