#region using

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using static SharedCode.TreeClasses.Selection;
using static SharedCode.TreeClasses.Selection.SelectMode;
using static SharedCode.TreeClasses.Selection.SelectFirstClass;
using static SharedCode.TreeClasses.Selection.SelectSecondClass;
using static SharedCode.TreeClasses.Selection.SelectTreeAllowed;

#endregion

// username: jeffs
// created:  10/14/2023 5:02:16 PM

namespace SharedCode.TreeClasses
{
	public class TreeSelectorIndividual<Te> : ATreeSelector<Te>
		where Te : class, ITreeElement
	{
	#region private fields

	#endregion

	#region ctor

		public TreeSelectorIndividual(SelectedList<Te> selected) : base(selected) { }

	#endregion

	#region public properties

		// fixe4d settings for this selector
		public override SelectMode SelectionMode => INDIVIDUAL;
		public override SelectFirstClass SelectionFirstClass => NODE_ONLY;
		public override SelectSecondClass SelectionSecondClass => NODES_ONLY;
		public override SelectTreeAllowed CanSelectTree => NO;

	#endregion

	#region private properties

	#endregion

	#region public methods

		public override bool SelectDeselect(Te element)
		{
			if (element== null || 
				element.State == Selection.SelectedState.UNSET) return false;

			if (element.State == Selection.SelectedState.SELECTED)
			{
				select(element);
			} 
			else if (element.State == Selection.SelectedState.DESELECTED)
			{
				deSelect(element);
			}

			updateProperties();

			return true;
		}

		private bool select(Te element)
		{
			if (!Selected!.Select(element, true)) return false;

			if (Selected.PriorSelectedCount > 0)
			{
				Selected.CurrentPriorSelected?.DeSelect();
			}

			element.Select();

			return true;
		}

		private bool deSelect(Te element)
		{
			if (!Selected!.DeSelect(element)) return false;

			element.DeSelect();

			return true;
		}

		public override bool TreeSelect()
		{
			throw new NotImplementedException();
		}

		public override bool TreeDeSelect()
		{
			throw new NotImplementedException();
		}

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
			return $"this is {nameof( TreeSelectorIndividual<Te>) }";
		}

	#endregion
	}

}