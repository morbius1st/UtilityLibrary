#region + Using Directives
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#endregion


// itemname: $$safeitemname$$
// username: $username$
// created:  $time$


namespace $rootnamespace$
{
	public class $safeitemname$
	{
		private $safeitemname$() { }

		private static readonly Lazy<$safeitemname$> instance = 
			new Lazy<$safeitemname$>(()=> new $safeitemname$());

		public static $safeitemname$ Instance => instance.Value;

	}
}
