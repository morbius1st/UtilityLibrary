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
using System.Xml.Linq;
using SharedCode.ShDebugAssist;
using SharedWPF.ShWin;

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
	public class TreeSelectorTriState : ATreeSelector
	{
	#region private fields

		// private  ShDebugMessages M = Examples.M;

	#endregion

	#region ctor

		public TreeSelectorTriState(SelectedListTriState selected) : base(selected)
		{
			selList = selected;

			OnPropertyChanged(nameof(Name));
			// OnPropertyChanged(nameof(Selected));
			// OnPropertyChanged(nameof(selList));

			M.WriteLine("\nTriState Selection: Can select multiple nodes / Selecting a branch node");
			M.WriteLine("selects/deselects the branch / remembers last selection states\n");
		}

	#endregion

	#region public properties

		public override string Name => "TriState Selector";

		// fixed settings for this selector
		public override SelectMode SelectionMode => TRISTATE;
		public override SelectFirstClass SelectionFirstClass => TRI_STATE;
		public override SelectSecondClass SelectionSecondClass => NODES_ONLY;
		public override SelectTreeAllowed CanSelectTree => YES;

	#endregion

	#region public methods

		private SelectedListTriState selList { get; }

		// private void updateProps()
		// {
		// 	OnPropertyChanged(nameof(Selected));
		// 	OnPropertyChanged(nameof(selList));
		// 	selList.updateProps();
		// }


		// a note went
		// from unchecked to checked or
		// from mixed to checked
		protected override bool select(ITreeNode? node)
		{
			// M.Write("I/2&4 or II/5 or III/8");

			if (!Selected!.Select(node)) return false;

			if (node!.PriorStateHasValue())
			{
				// M.WriteLine("(CASE III: column 8)");  // (E) tested

				// added to select list in main method

				node.RestoreState();
				node.UnsetPriorState();

				setAllChildrenToPriorState(node);
			}
			else
			{
// select
				// added to select list in main method
				node.Select();

				// if (!
				Selected!.Select(node);
				// 	)
				// {
				// 	Debug.WriteLine($"selecting {node.NodeKey} returned false");
				// }

				if (node.HasNodes)
				{
					// M.WriteLine("(CASE I: column 2 & 4)");  // (A) tested

					selectChildren(node, false);
				}
				// else
				// {
				// 	M.WriteLine("(CASE II: column 5)");  // (H) tested
				// }
			}

			applySelectToParents(node);

			return true;
		}


		// deselect a node
		// node went from checked to unchecked
		protected override bool deselect(ITreeNode? node)
		{
			// M.Write("I/3 or I/5 or III/7 or IV/7");

			if (!Selected!.Deselect(node)) return false;

			bool hasChildren = false;
			bool clearPriorChecked = false;
// deselect
			// removed to select list in main method
			node!.Deselect();

			// if (!
			Selected!.Deselect(node);
			// 	)
			// {
			// 	Debug.WriteLine($"deselecting {node.NodeKey} returned false");
			// }

			if (node.HasNodes)
			{
				deselectChildren(node);
				hasChildren = true;
			}

			if (node.PriorStateHasValue())
			{
				if (node!.IParentNode!.PriorStateHasValue())
				{
					// M.WriteLine("(CASE IV: column 7)");  // (G) tested
					setAllChildrenToUnset(node);

					node.UnsetPriorState();

					clearPriorChecked = true;
				}
				// else
				// {
				// 	M.WriteLine("(CASE III: column 7)");  //(C) tested
				// }
			}
			// else
			// {
			// 	if (hasChildren)
			// 	{
			// 		M.WriteLine("(CASE I: column 3)");  // (K) tested
			// 	}
			// 	else
			// 	{
			// 		M.WriteLine("(CASE I: column 5)");  // (I) tested
			// 	}
			//
			// }

			applyDeselectToParents(node, clearPriorChecked);

			return true;
		}

		// only applies when tri state
		protected override bool mixed(ITreeNode? node)
		{
			// M.Write("III/6");

			// if (!Selected!.Select(node)) return false;
			Selected!.Select(node);

			// M.WriteLine("(CASE III: column 6)");  // (F) tested

			node!.SavePriorState();
// select
			// not added to select list in main method
			// and do not add here
			node.Select();

			selectChildren(node, true);

			return true;
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

	#region private methods

		private void setAllChildrenToPriorState(ITreeNode? node)
		{
			if (!node!.HasNodes) return;

			foreach (ITreeNode childNode in node.EnumNodes())
			{
				setAllChildrenToPriorState(childNode);

				if (childNode.PriorState == SelectedState.SELECTED
					|| childNode.PriorState == SelectedState.MIXED
					)
				{
					Selected!.Select(childNode);
				}
				else 
				{
					Selected!.Deselect(childNode);
				}

				childNode.RestoreState();
				childNode.UnsetPriorState();
			}
		}

		private void setAllChildrenToUnset(ITreeNode? node)
		{
			if (!node!.HasNodes) return;

			foreach (ITreeNode childNode in node.EnumNodes())
			{
				setAllChildrenToUnset(childNode);

				childNode.UnsetPriorState();
			}
		}

		private void selectChildren(ITreeNode? node, bool saveState)
		{
			// M.WriteLine($"\tprocess| {node.NodeKey}");

			if (!node!.HasNodes) return;

			foreach (ITreeNode childNode in node.EnumNodes())
			{
				selectChildren(childNode, saveState);

				if (saveState) childNode.SavePriorState();
// select
				Selected!.Select(childNode);
				childNode.Select();
			}
		}

		private void deselectChildren(ITreeNode? node)
		{
			if (!node!.HasNodes) return;

			foreach (ITreeNode childNode in node.EnumNodes())
			{
				deselectChildren(childNode);
// deselect
				Selected!.Deselect(childNode);
				childNode.Deselect();
			}
		}

		private void setChildrenToUnset(ITreeNode? node)
		{
			if (!node!.HasNodes) return;

			foreach (ITreeNode childNode in node.EnumNodes())
			{
				childNode.UnsetPriorState();
			}
		}


		private void applySelectToParents(ITreeNode? node)
		{
			// that is, when reached the root node
			if (node!.IParentNode == null) return;

			// set parent to selected or mixed depending on
			// whether all parent's nodes are selected
			if (node.IParentNode.allChildrenNodesSelected())
			{
				if (node.IParentNode.State == SelectedState.SELECTED) return;
// select
				Selected!.Select(node.IParentNode);
				node.IParentNode.Select();
			}
			else
			{
				if (node.IParentNode.State == SelectedState.MIXED) return;
// mixed
				Selected!.Select(node.IParentNode);
				// do not set as selected
				node.IParentNode.SetMixed();
			}

			// continue up the tree
			applySelectToParents(node.IParentNode);
		}

		private void applyDeselectToParents(ITreeNode? node, bool clearPriorState)
		{
			if (node!.IParentNode == null) return;

			// M.WriteLine($"desel parent| {node.IParentNode.NodeKey}");


			if (node.IParentNode.allChildrenNodesDeselected())
			{
				// all current node's parent's nodes are deselected
				// I have already been deselected

				// M.WriteLine("\tall childs desel");

				if (node.IParentNode.State == SelectedState.DESELECTED) return;

				
// deselect
				Selected!.Deselect(node.IParentNode);

				// M.WriteLine("\tparent| desel");

				node.IParentNode.Deselect();
			}
			else
			{
				// not all current node's parent's nodes are deselected
				// i have already been deselected
				// then need set parent to mixed.

				// M.WriteLine("\tsome childs sel");

				if (node.IParentNode.State == SelectedState.MIXED) return;

				
// mixed
				Selected!.Deselect(node.IParentNode);

				// M.WriteLine("\tparent| mixed");
				node.IParentNode.SetMixed();
			}

			if (clearPriorState)
			{
				// M.WriteLine("\tclear prior state");
				node.IParentNode.UnsetPriorState();

				setChildrenToUnset(node.IParentNode);
			}

			applyDeselectToParents(node.IParentNode, clearPriorState);
		}

		/*

		private void applyStateChangeToParents(ITreeNode? currentNode, SelectedState newState)
		{
			switch (newState)
			{
			case SelectedState.SELECTED:
				currentNode.Select();
				applySelectedToParents(currentNode.IParentNode);
				break;
			case SelectedState.DESELECTED:
				currentNode.Deselect();
				applyDeselectToParents(currentNode.IParentNode);
				break;
			case SelectedState.MIXED:
				Debug.WriteLine("tri-state: got mixed state (from child)");
				break;
			}
		}

		private void applyStateChangeToChildren(ITreeNode currentNode, SelectedState newState)
		{
			switch (newState)
			{
			case SelectedState.SELECTED:
				// currentNode.Select();
				applySelectedToChildren(currentNode);
				break;
			case SelectedState.DESELECTED:
				// currentNode.Deselect();
				applyDeselectedToChildren(currentNode);
				break;
			case SelectedState.MIXED:
				Debug.WriteLine("tri-state: got mixed state (from child)");
				break;
			}

		}

		when is deselected then is selected:
		select all children.  set state based on selection state of its nodes
		  if all nodes selected -> selected, if one node is not selected -> mixed
		update all parents.  set state based on selection state of its nodes
		  if all nodes selected -> selected, if one node is not selected -> mixed

		when is selected then is deselected
		deselect all children.  set state based on selection state of its nodes
		  if all nodes deselected -> deselected, if one node is not selected -> mixed
		update all parents.  set state based on selection state of its nodes
		  if all nodes deselected -> deselected, if one node is not selected -> mixed
		*/


		/*

		// that is, apply the state change to all parents
		// until can no longer will apply
		private void applyStateChangeToParents(ITreeNode? childNode, SelectedState newState)
		{
			Debug.WriteLine($"\tapply| {newState.ToString()} to parent of {childNode.NodeKey}| parent null? {childNode.IParentNode == null}");

			if (childNode?.IParentNode == null) return;

			Debug.WriteLine($"\tapplying | {newState.ToString()} to {childNode.IParentNode.NodeKey}");

			bool result;

			switch (newState)
			{
			case SelectedState.SELECTED:
				result =childNode.IParentNode.Select();
				if (!result) return;
				break;
			case SelectedState.DESELECTED:
				result = childNode.IParentNode.Deselect();
				if (!result) return;
				break;
			case SelectedState.MIXED:
				Debug.WriteLine("tri-state: got mixed state (from child)");
				break;
			}

			Debug.WriteLine($"goto fromchild");
			applyStateChangeToParents(childNode.IParentNode, newState);
			Debug.WriteLine($"return fromchild");
		}

		// that is, apply the state change to all children
		// until can no longer apply
		private void applyStateChangeToChildren(ITreeNode node, SelectedState newState)
		{
			Debug.WriteLine($"\tapply| {newState.ToString()}  to children of {node.NodeKey}| has nodes| {node.HasNodes}");

			if (!(node?.HasNodes ?? false)) return;

			bool result;

			foreach (ITreeNode childNode in node.EnumNodes())
			{
				Debug.WriteLine($"\tapplying| {newState.ToString()} to {childNode.NodeKey}");

				// when appling to children, process children first
				// then update settings

				Debug.WriteLine($"goto fromParent");

				applyStateChangeToChildren(childNode, newState);

				Debug.WriteLine($"return fromParent");

				
				switch (newState)
				{
				case SelectedState.SELECTED:
					result=childNode.Select();
					if (!result) return;
					break;
				case SelectedState.DESELECTED:
					result=childNode.Deselect();
					if (!result) return;
					break;
				case SelectedState.MIXED:
					Debug.WriteLine("tri-state: got mixed state (from parent)");
					break;
				}

			}
		}

		*/

	#endregion

	#region system overrides

		public override string ToString()
		{
			return $"this is {nameof(TreeSelectorTriState) }";
		}

	#endregion
	}
}