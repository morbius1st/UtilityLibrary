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
	/// <summary>
	/// can select only one Te at a time<br/>
	/// that is, selecting a node deselects the prior node<br/>
	/// Te is ITreeNode or ITreeLeaf
	/// </summary>
	public class TreeSelectorIndividual : ATreeSelector
	{
	#region private fields

	#endregion

	#region ctor

		public TreeSelectorIndividual(SelectedListIndividual selected) : base(selected)
		{
			OnPropertyChanged(Name);

			M.WriteLine("\nIndividual Selection: can select random nodes / selecting one node");
			M.WriteLine("will deselect the prior node\n");
		}

	#endregion

	#region public properties

		public override string Name => "Individual Selector";

		// fixed settings for this selector
		public override SelectMode SelectionMode => INDIVIDUAL;
		public override SelectFirstClass SelectionFirstClass => NODE_ONLY;
		public override SelectSecondClass SelectionSecondClass => NODES_ONLY;
		public override SelectTreeAllowed CanSelectTree => NO;

	#endregion

	#region private properties

	#endregion

	#region public methods

		// select the node
		protected override bool select(ITreeNode node)
		{
			// M.WriteLine("@ select");
			// M.Write($"in selected| {selected.Selected.Contains(node)} | ");
			// M.WriteLine($"in prior| {selected.PriorSelected.Contains(node)}");

			if (SelectedCount > 0)
			{
				if (!deselect(CurrentSelected)) return false;
			}

			if (!selected.Select(node)) return false;

			node.Select();

			RaiseNodeSelectedEvent(node);

			return true;
		}

		// deselect the node
		protected override bool deselect(ITreeNode node)
		{
			// M.WriteLine("@ deselect");
			// M.Write($"in selected| {selected.Selected.Contains(node)} | ");
			// M.WriteLine($"in prior| {selected.PriorSelected.Contains(node)}");

			if (!Selected!.Deselect(node)) return false;

			node.Deselect();

			RaiseNodeDeselectedEvent(node);

			return true;
		}
		
		// only applies when tri state
		protected override bool mixed(ITreeNode? node)
		{
			throw new NotImplementedException();
		}

		// only applies when also selecting leaves
		protected override bool selectEx(ITreeNode? node)
		{
			throw new NotImplementedException();
		}

		// only applies when also deselecting leaves
		protected override bool deselectEx(ITreeNode? node)
		{
			throw new NotImplementedException();
		}

		// only applies when tree selection is allowed
		protected override bool treeSelect()
		{
			throw new NotImplementedException();
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof( TreeSelectorIndividual) }";
		}

	#endregion
	}

}