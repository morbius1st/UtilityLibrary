#region + Using Directives

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;


#endregion

// user name: jeffs
// created:   5/9/2020 10:23:38 PM


namespace SharedWPF.ShSupport
{
	public class CustomProperties : DependencyObject
	{
	#region GenericBoolOne

		public static readonly DependencyProperty GenericBoolOneProperty = DependencyProperty.RegisterAttached(
			"GenericBoolOne", typeof(bool), typeof(CustomProperties),
			new PropertyMetadata(false));

		public static void SetGenericBoolOne(UIElement e, bool value)
		{
			e.SetValue(GenericBoolOneProperty, value);
		}

		public static bool GetGenericBoolOne(UIElement e)
		{
			return (bool) e.GetValue(GenericBoolOneProperty);
		}

	#endregion

	#region GenericBoolTwo

		public static readonly DependencyProperty GenericBoolTwoProperty = DependencyProperty.RegisterAttached(
			"GenericBoolTwo", typeof(bool), typeof(CustomProperties),
			new PropertyMetadata(false));

		public static void SetGenericBoolTwo(UIElement e, bool value)
		{
			e.SetValue(GenericBoolTwoProperty, value);
		}

		public static bool GetGenericBoolTwo(UIElement e)
		{
			return (bool) e.GetValue(GenericBoolTwoProperty);
		}

	#endregion

	#region GenericStringOne

		public static readonly DependencyProperty GenericStringOneProperty = DependencyProperty.RegisterAttached(
			"GenericStringOne", typeof(string), typeof(CustomProperties), 
			new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.Inherits));

		public static void SetGenericStringOne(UIElement e, string value)
		{
			e.SetValue(GenericStringOneProperty, value);
		}

		public static string GetGenericStringOne(UIElement e)
		{
			return (string) e.GetValue(GenericStringOneProperty);
		}

	#endregion

		// #region GenericStringTwo
		//
		// 	public static readonly DependencyProperty GenericStringTwoProperty = DependencyProperty.RegisterAttached(
		// 		"GenericStringTwo", typeof(string), typeof(CustomProperties), new PropertyMetadata(""));
		//
		// 	public static void SetGenericStringTwo(UIElement e, string value)
		// 	{
		// 		e.SetValue(GenericStringTwoProperty, value);
		// 	}
		//
		// 	public static string GetGenericStringTwo(UIElement e)
		// 	{
		// 		return (string) e.GetValue(GenericStringTwoProperty);
		// 	}
		//
		// #endregion
		//
		// #region GenericStringThree
		//
		// 	public static readonly DependencyProperty GenericStringThreeProperty = DependencyProperty.RegisterAttached(
		// 		"GenericStringThree", typeof(string), typeof(CustomProperties), new PropertyMetadata(""));
		//
		// 	public static void SetGenericStringThree(UIElement e, string value)
		// 	{
		// 		e.SetValue(GenericStringThreeProperty, value);
		// 	}
		//
		// 	public static string GetGenericStringThree(UIElement e)
		// 	{
		// 		return (string) e.GetValue(GenericStringThreeProperty);
		// 	}
		//
		// #endregion

	#region GenericIntOne

		public static readonly DependencyProperty GenericIntOneProperty = DependencyProperty.RegisterAttached(
			"GenericIntOne", typeof(int), typeof(CustomProperties),
			new FrameworkPropertyMetadata(-1, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetGenericIntOne(UIElement e, int value)
		{
			e.SetValue(GenericIntOneProperty, value);
		}

		public static int GetGenericIntOne(UIElement e)
		{
			return (int) e.GetValue(GenericIntOneProperty);
		}

	#endregion

	#region GenericDoubleOne

		public static readonly DependencyProperty GenericDoubleOneProperty = DependencyProperty.RegisterAttached(
			"GenericDoubleOne", typeof(double), typeof(CustomProperties),
			new FrameworkPropertyMetadata(-1.0,
				FrameworkPropertyMetadataOptions.AffectsMeasure |
				FrameworkPropertyMetadataOptions.Inherits |
				FrameworkPropertyMetadataOptions.AffectsParentMeasure |
				FrameworkPropertyMetadataOptions.AffectsRender));

		public static void SetGenericDoubleOne(UIElement e, double value)
		{
			e.SetValue(GenericDoubleOneProperty, value);
		}

		public static double GetGenericDoubleOne(UIElement e)
		{
			return (double) e.GetValue(GenericDoubleOneProperty);
		}

	#endregion

	#region GenericObjectOne

		public static readonly DependencyProperty GenericObjectOneProperty = DependencyProperty.RegisterAttached(
			"GenericObjectOne", typeof(object), typeof(CustomProperties), new FrameworkPropertyMetadata(new object(), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetGenericObjectOne(UIElement e, object value)
		{
			e.SetValue(GenericObjectOneProperty, value);
		}

		[DebuggerStepThrough]
		public static object GetGenericObjectOne(UIElement e)
		{
			return (object) e.GetValue(GenericObjectOneProperty);
		}

	#endregion

	#region GenericPopupOne

		public static readonly DependencyProperty GenericPopupOneProperty = DependencyProperty.RegisterAttached(
			"GenericPopupOne", typeof(Popup), typeof(CustomProperties), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetGenericPopupOne(UIElement e, Popup value)
		{
			e.SetValue(GenericPopupOneProperty, value);
		}

		[DebuggerStepThrough]
		public static Popup GetGenericPopupOne(UIElement e)
		{
			return (Popup) e.GetValue(GenericPopupOneProperty);
		}

	#endregion

	#region GenericContainerObjOne

		public static readonly DependencyProperty GenericContainerObjOneProperty = DependencyProperty.RegisterAttached(
			"GenericContainerObjOne", typeof(object), typeof(CustomProperties), new PropertyMetadata(new object()));

		public static void SetGenericContainerObjOne(UIElement e, object value)
		{
			e.SetValue(GenericContainerObjOneProperty, value);
		}

		public static object GetGenericContainerObjOne(UIElement e)
		{
			return (object) e.GetValue(GenericContainerObjOneProperty);
		}

	#endregion

	#region GenericPathGeometry

		public static readonly DependencyProperty GenericPathGeometryProperty = DependencyProperty.RegisterAttached(
			"GenericPathGeometry", typeof(PathGeometry), typeof(CustomProperties),
			new FrameworkPropertyMetadata(new PathGeometry(),
				FrameworkPropertyMetadataOptions.Inherits));

		public static void SetGenericPathGeometry(UIElement e, PathGeometry value)
		{
			e.SetValue(GenericPathGeometryProperty, value);
		}

		public static PathGeometry GetGenericPathGeometry(UIElement e)
		{
			return (PathGeometry) e.GetValue(GenericPathGeometryProperty);
		}

	#endregion

	#region GenericGeometry

		public static readonly DependencyProperty GenericGeometryProperty = DependencyProperty.RegisterAttached(
			"GenericGeometry", typeof(Geometry), typeof(CustomProperties),
			new FrameworkPropertyMetadata(null,
				FrameworkPropertyMetadataOptions.Inherits));

		public static void SetGenericGeometry(UIElement e, Geometry value)
		{
			e.SetValue(GenericGeometryProperty, value);
		}

		public static Geometry GetGenericGeometry(UIElement e)
		{
			return (Geometry) e.GetValue(GenericGeometryProperty);
		}

	#endregion

		
	#region GenericContentControl

		public static readonly DependencyProperty GenericContentControlProperty = DependencyProperty.RegisterAttached(
			"GenericContentControl", typeof(ContentControl), typeof(CustomProperties),
			new FrameworkPropertyMetadata(null,
				FrameworkPropertyMetadataOptions.Inherits));

		public static void SetGenericContentControl(UIElement e, ContentControl value)
		{
			e.SetValue(GenericContentControlProperty, value);
		}

		public static ContentControl GetGenericContentControl(UIElement e)
		{
			return (ContentControl) e.GetValue(GenericContentControlProperty);
		}

	#endregion

		
	#region GenericCanvas

		public static readonly DependencyProperty GenericCanvasProperty = DependencyProperty.RegisterAttached(
			"GenericCanvas", typeof(Canvas), typeof(CustomProperties),
			new FrameworkPropertyMetadata(null,
				FrameworkPropertyMetadataOptions.Inherits));

		public static void SetGenericCanvas(UIElement e, Canvas value)
		{
			e.SetValue(GenericCanvasProperty, value);
		}

		public static Canvas GetGenericCanvas(UIElement e)
		{
			return (Canvas) e.GetValue(GenericCanvasProperty);
		}

	#endregion

		
	#region GenericUiElement
	
		public static readonly DependencyProperty GenericUIElementProperty = DependencyProperty.RegisterAttached(
			"GenericUIElement", typeof(UIElement), typeof(CustomProperties),
			new FrameworkPropertyMetadata(null,
				FrameworkPropertyMetadataOptions.Inherits));
	
		public static void SetGenericUIElement(UIElement e, object value)
		{
			e.SetValue(GenericUIElementProperty, value);
		}
	
		public static UIElement GetGenericUIElement(UIElement e)
		{
			return (UIElement) e.GetValue(GenericUIElementProperty);
		}
	
	#endregion







		// #region DropDownWidthAdjustment
		//
		// 	public static readonly DependencyProperty DropDownWidthAdjustmentProperty = DependencyProperty.RegisterAttached(
		// 		"DropDownWidthAdjustment", typeof(double), typeof(CustomProperties), new PropertyMetadata(0.0));
		//
		// 	public static void SetDropDownWidthAdjustment(UIElement e, double value)
		// 	{
		// 		e.SetValue(DropDownWidthAdjustmentProperty, value);
		// 	}
		//
		// 	public static double GetDropDownWidthAdjustment(UIElement e)
		// 	{
		// 		return (double) e.GetValue(DropDownWidthAdjustmentProperty);
		// 	}
		//
		// #endregion
		//
		// #region DropDownMinWidth
		//
		// 	public static readonly DependencyProperty DropDownMinWidthProperty = DependencyProperty.RegisterAttached(
		// 		"DropDownMinWidth", typeof(double), typeof(CustomProperties), new PropertyMetadata(100.0));
		//
		// 	public static void SetDropDownMinWidth(UIElement e, double value)
		// 	{
		// 		e.SetValue(DropDownMinWidthProperty, value);
		// 	}
		//
		// 	public static double GetDropDownMinWidth(UIElement e)
		// 	{
		// 		return (double) e.GetValue(DropDownMinWidthProperty);
		// 	}
		//
		// #endregion
		//
		// #region MouseOverBrush
		//
		// 	public static readonly DependencyProperty MouseOverBgBrushProperty = DependencyProperty.RegisterAttached(
		// 		"MouseOverBrush", typeof(SolidColorBrush), typeof(CustomProperties), new PropertyMetadata(default(SolidColorBrush)));
		//
		// 	public static void SetMouseOverBgBrush(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(MouseOverBgBrushProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetMouseOverBgBrush(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(MouseOverBgBrushProperty);
		// 	}
		//
		// #endregion
		// 	
		// #region DropDownBrush
		//
		// 	public static readonly DependencyProperty  DropDownBrushProperty = DependencyProperty.RegisterAttached(
		// 		"DropDownBrush", typeof(SolidColorBrush), typeof(CustomProperties), new PropertyMetadata(default(SolidColorBrush)));
		//
		// 	public static void SetDropDownBrushh(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(DropDownBrushProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetDropDownBrush(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(DropDownBrushProperty);
		// 	}
		//
		// #endregion
		//
		// #region NotEnabledBrush
		//
		// 	public static readonly DependencyProperty NotEnabledBrushProperty = DependencyProperty.RegisterAttached(
		// 		"NotEnabledBrush", typeof(SolidColorBrush), typeof(CustomProperties), new PropertyMetadata(default(SolidColorBrush)));
		//
		// 	public static void SetNotEnabledBrush(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(NotEnabledBrushProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetNotEnabledBrush(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(NotEnabledBrushProperty);
		// 	}
		//
		// #endregion
		//
	}
}