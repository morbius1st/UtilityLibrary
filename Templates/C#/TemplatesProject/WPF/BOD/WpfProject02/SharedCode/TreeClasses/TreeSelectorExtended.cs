#region using

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static SharedCode.TreeClasses.Selection;
using static SharedCode.TreeClasses.Selection.SelectMode;
using static SharedCode.TreeClasses.Selection.SelectFirstClass;
using static SharedCode.TreeClasses.Selection.SelectSecondClass;
using static SharedCode.TreeClasses.Selection.SelectTreeAllowed;

#endregion

// username: jeffs
// created:  10/21/2023 9:40:53 PM

namespace SharedCode.TreeClasses
{
	/// <summary>
	/// can select multiple Te at a time 
	/// that is, can continue to select notes as 
	/// often as needed.  however, if a branch is 
	/// selected, the whole branch at and below 
	/// the selected node is selected 
	/// selecting a selected node 
	/// deselects that one node
	/// Te is ITreeNode or ITreeLeaf
	/// </summary>
	public class TreeSelectorExtended : ATreeSelector
	{
	#region private fields

	#endregion

	#region ctor

		public TreeSelectorExtended(SelectedListExtended selected) : base(selected)
		{
			OnPropertyChanged(Name);

			M.WriteLine("\nExtended Selection: can select nodes / selecting a branch node");
			M.WriteLine("selects/deselects the branch\n");
		}

	#endregion

	#region public properties

		public override string Name => "Extended Selector";

		// fixed settings for this selector
		public override SelectMode SelectionMode => EXTENDED;
		public override SelectFirstClass SelectionFirstClass => NODE_EXTENDED;
		public override SelectSecondClass SelectionSecondClass => NODES_ONLY;
		public override SelectTreeAllowed CanSelectTree => YES;

	#endregion

	#region private properties

	#endregion

	#region public methods

		// select the provided node and any associated
		// noted (extended)
		protected override bool select(ITreeNode? node)
		{
			if (!Selected!.Select(node)) return false;

			selectChildNodes(node);

			node.Select();
			RaiseNodeSelectedEvent(node);

			return true;
		}

		private void selectChildNodes(ITreeNode? node)
		{
			if (node == null || !node.HasNodes) return;

			foreach (ITreeNode n in node.EnumNodes())
			{
				selectChildNodes(n);

				if (!Selected.Select(n)) continue;

				n.Select();
				RaiseNodeSelectedEvent(n);
			}
		}


		// deselect the provided node and any associated
		// noted (extended)
		protected override bool deselect(ITreeNode node)
		{
			if (!Selected!.Deselect(node)) return false;

			deselectChildNodes(node);

			node.Deselect();
			RaiseNodeSelectedEvent(node);

			return true;
		}

		private void deselectChildNodes(ITreeNode node)
		{
			if (node == null || !node.HasNodes) return;

			foreach (ITreeNode n in node.EnumNodes())
			{
				deselectChildNodes(n);

				if (!Selected.Deselect(n)) continue;

				n.Deselect();
				RaiseNodeSelectedEvent(n);
			}
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

		// select the whole tree??
		protected override bool treeSelect()
		{
			throw new NotImplementedException();
		}

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(TreeSelectorExtended)}";
		}

	#endregion
	}
}