using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using SharedWPF.ShWin;

namespace WpfProject02.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, IWinMsg,INotifyPropertyChanged
	{

		
	#region private fields

		private string messageBoxText;
		private string statusBoxText;
		private string codeMapText;

	#endregion
		
	#region ctor

		public MainWindow()
		{
			InitializeComponent();
		}

	#endregion

	#region public properties

		public string MessageBoxText
		{
			get => messageBoxText;
			set
			{
				messageBoxText = value;
				OnPropertyChanged();
			}
		}

		public string StatusBoxText
		{
			get => statusBoxText;
			set
			{ 
				
				statusBoxText = value;
				OnPropertyChanged();
			}
		}

		public string CodeMapText
		{
			get => codeMapText;
			set
			{ 
				codeMapText = value;
				OnPropertyChanged();
			}
		}

	#endregion

	#region private properties

	#endregion

	#region public methods

	#endregion

	#region private methods

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(MainWindow)}";
		}

	#endregion

	#region event publishing

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}


		private void BtnExit_OnClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void BtnExamples_OnClick(object sender, RoutedEventArgs e)
		{

			SharedWPF.ShWin.Examples x = new Examples();
			x.Show();
		}

	#endregion

	#region event consuming

	#endregion
	}
}
