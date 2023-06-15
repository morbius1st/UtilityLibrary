#region using
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

#endregion

// projname: $projectname$
// itemname: $safeitemname$
// username: $username$
// created:  $time$

namespace $safeprojectname$.Windows
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
	#region private fields

	#endregion

	#region ctor

		public MainWindow()
		{
			InitializeComponent();
		}

	#endregion

	#region public properties
    
    

	#endregion

	#region private properties
    
    

	#endregion

	#region public methods
    
    

	#endregion

	#region private methods
    
    

	#endregion

	#region event processing

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChange([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#endregion


	}
}
