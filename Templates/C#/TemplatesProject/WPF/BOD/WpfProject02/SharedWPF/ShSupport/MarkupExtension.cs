#region + Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

#endregion

// user name: jeffs
// created:   10/7/2023 7:13:33 AM

namespace SharedWPF.ShSupport
{
	[MarkupExtensionReturnType(typeof(Type))]
	public class GenericType : MarkupExtension
	{
		public GenericType() { }

		public GenericType(Type baseType, params Type[] innerTypes)
		{
			BaseType = baseType;
			InnerTypes = innerTypes;
		}

		public Type BaseType { get; set; }

		public Type[] InnerTypes { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			Type result = BaseType.MakeGenericType(InnerTypes);
			return result;
		}
	}
}
