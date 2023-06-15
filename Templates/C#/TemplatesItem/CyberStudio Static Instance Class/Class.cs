#region using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#endregion


// projname: $projectname$
// itemname: $safeitemname$
// username: $username$
// created:  $time$

namespace $rootnamespace$
{
	public class $safeitemname$
	{
	#region private fields

		private static readonly Lazy<$safeitemname$> instance =
		new Lazy<$safeitemname$>(()=> new $safeitemname$());

	#endregion

	#region ctor

		private $safeitemname$() { }

	#endregion

	#region public properties

		public static $safeitemname$ Instance => instance.Value;

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
    
        

	#endregion

	#region system overrides

	public override string ToString()
	{
		return $"this is {nameof($safeitemname$)}";
	}

	#endregion

	}
}