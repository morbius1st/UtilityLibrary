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
using System.Xml.Linq;

#endregion

// username: jeffs
// created:  10/21/2023 8:43:53 PM

namespace SharedCode.TreeClasses
{
	/// <summary>
	/// can select multiple Te at a time<br/>
	/// that is, can continue to select notes as<br/>
	/// often as needed.  selecting a selected node<br/>
	/// deselects that one node
	/// Te is ITreeNode or ITreeLeaf
	/// </summary>
	public class TreeSelectorMultiple : ATreeSelector
	{
	#region private fields

	#endregion

	#region ctor

		public TreeSelectorMultiple(SelectedListMultiple selected) : base(selected)
		{
			OnPropertyChanged(Name);

			M.WriteLine("\nMultiple Selection: can select random nodes / selecting a branch node");
			M.WriteLine("does NOT selects/deselects the branch\n");
		}

	#endregion

	#region public properties

		public override string Name => "Multiple Selector";

		// fixed settings for this selector
		public override SelectMode SelectionMode => MULTISELECTNODE;
		public override SelectFirstClass SelectionFirstClass => NODE_MULTI;
		public override SelectSecondClass SelectionSecondClass => NODES_ONLY;
		public override SelectTreeAllowed CanSelectTree => YES;

	#endregion

	#region private properties

	#endregion

	#region public methods

		protected override bool select(ITreeNode? node)
		{
			if (!Selected!.Select(node)) return false;

			node.Select();
			RaiseNodeSelectedEvent(node);
			return true;
		}

		protected override bool deselect(ITreeNode? node)
		{
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

		protected override bool treeSelect()
		{
			if (!Tree!.IRootNode.HasNodes) return false;

			treeSelect(Tree!.IRootNode);

			updateProperties();

			return true;
		}

		// select all nodes from the bottom up
		private void treeSelect(ITreeNode? node)
		{
			if (node == null || !node.HasNodes) return;

			foreach (ITreeNode n in node.EnumNodes())
			{
				treeSelect(n);

				/*
				// must use property and not field
				Selected!.Select(n);

				// do not use node.Select()
				n.Select();
				*/

				// select(n);

				// will this work?  don't want to issue an
				// event for every selection
				n.Select();

			}
		}


	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(TreeSelectorMultiple)}";
		}

	#endregion
	}
}