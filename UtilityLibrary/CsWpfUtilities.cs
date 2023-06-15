using System;
using System.Configuration;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace UtilityLibrary
{
	internal static class CsWpfUtilities
	{
		/// <summary>
		/// search for a framework fe
		/// </summary>
		/// <typeparam name="T">FrameworkElement</typeparam>
		/// <param name="fe">Starting point fe to start the search</param>
		/// <param name="sChildName">The string name of the fe to find</param>
		/// <returns></returns>
		public static T FindElementByNameContentControl<T>(FrameworkElement element, string sChildName) where T : FrameworkElement
		{
			T childElement = null;

			FrameworkElement fe = element;

			if (element is ContentControl)
			{
				object obj = (element as ContentControl).Content;
				if (obj is FrameworkElement)
				{
					fe = (FrameworkElement) obj;
				}
				else
				{
					if (VisualTreeHelper.GetChildrenCount(element) > 0)
					{
						obj = VisualTreeHelper.GetChild(element, 0);
						if (obj is FrameworkElement)
						{
							fe = (FrameworkElement) obj;
						}
					}
					else
					{
						fe = (FrameworkElement) ((DataTemplate) (element as ContentControl).ContentTemplate).LoadContent();
						fe = FindElementByNameContentControl<T>(fe, sChildName);
					}
				}
			}


			var nChildCount = VisualTreeHelper.GetChildrenCount(fe);

			for (int i = 0; i < nChildCount; i++)
			{
				FrameworkElement child = VisualTreeHelper.GetChild(fe, i) as FrameworkElement;

				if (child == null)
					continue;

				if (child is T && child.Name.Equals(sChildName))
				{
					childElement = (T)child;
					break;
				}

				childElement = FindElementByNameContentControl<T>(child, sChildName);

				if (childElement != null)
					break;
			}

			return childElement;
		}

		public static void ListLogicalTree(FrameworkElement element)
		{
			depth++;

			foreach (object child in LogicalTreeHelper.GetChildren(element))
			{
				Debug.Write($"{"  ".Repeat(depth)}found| {child.GetType().Name}");

				if (child is FrameworkElement)
				{
					Debug.WriteLine($" {((FrameworkElement) child).Name}");

					ListLogicalTree((FrameworkElement) child);
				}
				else
				{
					Debug.Write("\n");
				}
			}
			depth--;
		}

		public static void ListVisualTree(FrameworkElement element)
		{
			depth++;
			veItem++;

			int childCount = VisualTreeHelper.GetChildrenCount(element);

			// Debug.WriteLine($"{"  ".Repeat(depth)}element| name| {element.Name}| count| {childCount}");

			tellVe(element);

			for (int i = 0; i < childCount; i++)
			{
				object obj = VisualTreeHelper.GetChild(element, i) ;
				// Debug.WriteLine($"found| {obj.GetType()}");

				if (obj is FrameworkElement)
				{
					FrameworkElement child = (FrameworkElement) obj;

					ListVisualTree(child);
				}
			}

			depth--;

		}

		private static int loops = 0;
		private static int item = 0;
		private static int veItem = 0;
		private static int depth = -1;
		private static int depthMax = -2;
		private static int depthSave = 0;
		private static bool found;
		private static bool debug;


		[DebuggerStepThrough]
		private static void tellVe(FrameworkElement fe)
		{
			const int prefaceWidth = 15;
			const int nameWidth = -100;
			const int col1Width = -5;
			const int col2Width = -7;
			const int parentWidth = -30;

			int preLength;

			string preface;
			string divider;
			string num = $"{veItem,col2Width:####}";
			string type = null;
			string name = fe.Name.IsVoid() ? $"{fe.GetType().Name}{veItem:D4}" : fe.Name ;
			string parent = null;
			string parentType = null;
			string parent1 = $"{fe.Parent?.GetValue(FrameworkElement.NameProperty).ToString()}";
			string parent2= $"{VisualTreeHelper.GetParent(fe)?.GetValue(FrameworkElement.NameProperty).ToString()}";

			if (!fe.Name.IsVoid())
			{
				type = $"  ({fe.GetType().Name})";
			}
			else
			{
				type = "";
			}

			if (!parent1.IsVoid())
			{
				parentType = fe.Parent?.GetType().Name;
				parent = parent1;
			} 
			else if (!parent2.IsVoid())
			{
				parentType = VisualTreeHelper.GetParent(fe)?.GetType().Name;
				parent = parent2;
			}

			if (!parentType.IsVoid())
			{
				parent = $"{parent, parentWidth}[{parentType}]";
			}

			
			preLength = prefaceWidth - num.Length;
			divider = ".".Repeat(depth);

			name = $"{divider}{name}{type}";
			name = $" {name,nameWidth}";

			Debug.WriteLine($"{num}{name} {num} :: parent| {parent}");

		}

		[DebuggerStepThrough]
		private static void tellFe(object xe, string loc, string extra = null)
		{
			if (!debug) return;

			const int prefaceWidth = 15;
			const int nameWidth = -150;
			const int col1Width = -5;
			const int col2Width = -7;
			const int col3Width = -20;


			string preface = null;
			int prefaceLength;
			int nameLength;
			string lx;
			string name = null;
			string type = null;
			string nx = $"{item,4}";
			string parent = "";
			string parentType = null;
			string parent1 = "";
			string parent2 = "";
			string divider = null;

			if (depth < 0) depth = 0;

			if (depth > depthMax)
			{
				depthMax = depth;
			}

			Debug.Write("\n");

			if (xe is FrameworkElement)
			{
				FrameworkElement fe = xe as FrameworkElement;

				name = fe.Name.IsVoid() ? $"{fe.GetType().Name}{item:D3}" : fe.Name ;

				if (!fe.Name.IsVoid())
				{
					type = $"  ({fe.GetType().Name})";
				}
				else
				{
					type = "";
				}

				parent1 = fe.Parent?.GetValue(FrameworkElement.NameProperty).ToString();
				parent2 = $"{VisualTreeHelper.GetParent(fe)?.GetValue(FrameworkElement.NameProperty).ToString()}";

				if (parent1 != null)
				{
					parentType = fe.Parent?.GetType().Name;
					parent = parent1;
				} 
				else if (parent2 != null)
				{
					parentType = VisualTreeHelper.GetParent(fe)?.GetType().Name;
					parent = parent2;
				}

				if (parentType != null)
				{
					parent = $"{parent,-20}[{parentType}]";
				}

				lx = $"{loc,col1Width}";
				nx = $"{nx, col2Width}";
				preface = $"{lx} :: {nx}";
			}
			else
			{
				parent1 = "** FE **";
				name = $"{xe?.ToString() ?? "null"}";
				type = $"{xe?.GetType().Name ?? "null"}";


				lx = $"{loc,col1Width}";
				nx = $"{nx, col2Width}";
				preface = $"{lx} :: {nx}";
			}
			// Debug.WriteLine($"{preface}{title}| {name}| type| {type} {extra} | parent| {parent}");


			prefaceLength = prefaceWidth - preface.Length;
			preface = $"{preface}{" ".Repeat(prefaceLength)}";
			divider = ". ".Repeat(depth);

			name = $"{divider}{name}{type}";
			name = $"{name,nameWidth}";

			// loc  ::  number  . . . . . name  :: type:  (parent :: number)

			item++;

			Debug.Write($"{preface}{name} {nx} :: parent| {parent, -20}");
			if (extra != null)
			{
				Debug.Write(extra);
			}
		}

		[DebuggerStepThrough]
		private static bool checkChild(FrameworkElement childElement)
		{
			if (childElement != null)
			{
				if (!found)
				{
					found = true;
					depth++;
					tellFe(childElement, "cc1");
					depth--;

				}
				return true;
			}

			return false;
		}

		/// <summary>
		/// Find a WPF element given its name<br/>
		/// However, this is limited to the available visual tree<br/>
		/// That is, if the element is not displayed (e.g. on a hidden tab), it cannot be found
		/// </summary>
		/// <typeparam name="T">The Type of element being found (i.e. 'ListBox'</typeparam>
		/// <param name="element">The starting element of the search - use 'this' for the current window</param>
		/// <param name="sChildName">The string Name of the element to find (which means it must have a name)</param>
		/// <param name="dbug">A flag to show each element as it is found in the Debug window</param>
		/// <returns></returns>
		public static T FindElementByName<T>(FrameworkElement element, string sChildName, bool dbug) where T : FrameworkElement
		{
			debug = dbug;
			return FindElementByNameCc<T>(element, sChildName);
		}

		private static T FindElementByNameCc<T>(FrameworkElement element, string sChildName) where T : FrameworkElement
		{
			loops++;

			// depth++;
			// Debug.WriteLine($"{"\t".Repeat(depth)}0  begin| name| {element.Name} loop| {loops}");
			// Debug.Write("\n");

			// tellFe(element, "0", "begin", -1, $"loop| {loops}");

			T childElement = null;

			FrameworkElement te = element;

			if (te == null)
			{
				Debug.WriteLine($"is null");
			}
			else if (te is ContentControl)
			{
				depth++;

				childElement = findChildInCC<T>(te, sChildName);
				if (checkChild(childElement))
				{
					depth--;
					return childElement;
				}


				depth--;
			}
			else if (te is ItemsControl)
			{
				depth++;
				// tellFe(te, "B", "is itemscontrol");

				ItemsControl ic = te as ItemsControl;
				ItemCollection icol = ic.Items;
				int icount = icol.Count;

				// depth++;
				// tellFe(icol, "B1", "collection", $"| count| {icount}");


				foreach (object o in icol)
				{
					if (o is ContentControl)
					{
						childElement = FindElementByNameCc<T>((FrameworkElement) o, sChildName);
						if (checkChild(childElement))
						{
							// depth--;
							return childElement;
						}
					}
					else if (o is FrameworkElement)
					{
						depth++;
						tellFe((FrameworkElement)o, "B11");

						if ( ((FrameworkElement)o).Name.Equals(sChildName))
						{
							childElement = (T) (FrameworkElement)o;
							// depth--;
							return childElement;
						}

						childElement = FindChildElementByNameCc<T>((FrameworkElement) o, sChildName);
						if (checkChild(childElement))
						{
							// depth--;
							return childElement;
						}

						depth--;
					}
					else
					{
						depth++;
						tellFe(o, "B21");
						depth--;
					}
				}

				// depth--;
				depth--;
			}
			else if (te is Panel)
			{
				int count = VisualTreeHelper.GetChildrenCount(te);
				int count2 = ((Panel) te).Children.Count;

				depth++;
				tellFe((Panel)te, "C");

				if (count > 0)
				{
					Panel p = te as Panel;

					// depth++;
					// tellFe(p, "C1", "is panel with children", 310);

					foreach (UIElement pChild in p.Children)
					{
						if (pChild is FrameworkElement)
						{
							if ((pChild as FrameworkElement).Name.Equals(sChildName))
							{
								childElement = (T) (pChild as FrameworkElement);
								depth--;
								return childElement;
							}


							childElement = FindChildElementByNameCc<T>((FrameworkElement) pChild, sChildName);
							if (checkChild(childElement))
							{
								depth--;
								return childElement;
							}
						}



					}
					// depth--;
				}

				depth--;
			}
			// else if (te is ContentPresenter)
			// {
			// 	tellFe((ContentPresenter)te, "E", $"content source| {((ContentPresenter)te).ContentSource}");
			// }
			else if (te is FrameworkElement)
			{
				depth++;
				childElement = FindChildElementByNameCc<T>(te, sChildName);
				if (checkChild(childElement))
				{
					// depth--;
					return childElement;
				}

				depth--;
			}
			// else
			// {
			// 	Debug.WriteLine($"is none of the above| {te.Name }| type| {te.GetType()}| count| {VisualTreeHelper.GetChildrenCount(te)}");
			// }

			// depth--;

			return childElement;
		}

		private static T findChildInCC<T>(FrameworkElement fe, string sChildName) where T : FrameworkElement
		{
			T childElement = null;

			tellFe(fe, "A");

			object obj1 = (fe as ContentControl)?.Content;
			// object obj2 = (te as ContentControl).ContentTemplate;
			object obj3 = (fe as ContentControl)?.Template;

			if (obj3 != null)
			{
				// Debug.Write("A3 ");
				// depth++;
				// tellFe((ControlTemplate) obj3, "A3", "obj3 is controltemplate");

				DependencyObject obj31 = ((ControlTemplate)obj3).LoadContent();

				if (obj31 != null)
				{
					// Debug.Write("A31 ");

					// depth++;
					// tellFe(obj31, "A31", "unknown content", 130, $"dep obj type| {obj31.DependencyObjectType.Name}");


					if (obj31 is ContentControl)
					{
						// Debug.Write("A311 ");
						// depth++;
						// tellFe(obj31, "A311", "content control", 131);
						// depth--;

						childElement = findChildInCC<T>((ContentControl) obj31, sChildName);
						if (checkChild(childElement))
						{
							// depth--;
							return childElement;
						}
					}
					else
					{
						// Debug.Write("A312 ");
						// tellFe(obj31, "A312", "frameworkelement", 132);
						// depth--;

						
						childElement = FindElementByNameCc<T>((FrameworkElement) obj31, sChildName);
						if (checkChild(childElement))
						{
							
							return childElement;
						}
						
					}

					
				}

				FrameworkElementFactory ff = ((ControlTemplate)obj3).VisualTree;

				if (ff != null)
				{
					// Debug.Write("A32 ");
					depth++;
					tellFe(ff, "A32");
					depth--;
				}

				// depth--;
			}

			depthSave = depth;
			depth = depthMax;
			
			
			if (obj1 != null)
			{
				if (obj1 is ContentControl)
				{
					depth++;
					tellFe((ContentControl)obj1, "A11");
					childElement = findChildInCC<T>((ContentControl) obj1, sChildName);
					if (checkChild(childElement))
					{
						depth--;
						return childElement;
					}

					depth--;
				}
				else if (obj1 is FrameworkElement)
				{
					// depth++;
					// tellFe((FrameworkElement)obj1, "A12", 112);
					// depth--;

					// Debug.Write("A41 ");
					childElement = FindElementByNameCc<T>((FrameworkElement) obj1, sChildName);
					if (checkChild(childElement))
					{
						depth--;
						return childElement;
					}
				}
				else
				{
					depth++;
					tellFe(obj1, "A13");
					depth--;
				}
			}


			depth = depthSave;
			depthMax = 0;


			return (T) childElement;
		}

		private static T FindChildElementByNameCc<T>(FrameworkElement element, string sChildName) where T : FrameworkElement
		{
			if (element.Name.EndsWith(sChildName)) return (T) element;

			T childElement = null;

			var nChildCount = VisualTreeHelper.GetChildrenCount(element);

			if (nChildCount > 0)
			{
				depth++;
				tellFe(element, "Fx");


				for (int i = 0; i < nChildCount; i++)
				{
					FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

					if (child == null)
						continue;

					if (child is T && child.Name.Equals(sChildName))
					{
						childElement = (T)child;
						break;
					}

					childElement = FindElementByNameCc<T>(child, sChildName);

					if (childElement != null)
						break;
				}

				depth--;
			}
			else
			{
				depth++;
				tellFe(element, "Fx");
				depth--;
			}

			return childElement;
		}

		
		public static T FindElementByName<T>(FrameworkElement element, string sChildName) where T : FrameworkElement
		{
			T childElement = null;
			var nChildCount = VisualTreeHelper.GetChildrenCount(element);

			for (int i = 0; i < nChildCount; i++)
			{
				FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

				if (child == null)
					continue;

				if (child is T && child.Name.Equals(sChildName))
				{
					childElement = (T)child;
					break;
				}

				childElement = FindElementByName<T>(child, sChildName);

				if (childElement != null)
					break;
			}

			return childElement;
		}
		

	}
}