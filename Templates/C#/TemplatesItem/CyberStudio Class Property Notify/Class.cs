#region using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

// username: $username$
// created:  $time$

namespace $rootnamespace$
{

	public class $safeitemname$ : INotifyPropertyChanged
	{
	#region private fields
    
        

	#endregion

	#region ctor

		public $safeitemname$() {}

	#endregion

	#region public properties
    
        

	#endregion

	#region private properties
    
        

	#endregion

	#region public methods
    
        

	#endregion

	#region private methods
    
        

	#endregion

	#region event consuming
    
        

	#endregion

	#region event publishing

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		[DebuggerStepThrough]
		private void OnPropertyChange([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof($safeitemname$)}";
		}

	#endregion

	}
}
