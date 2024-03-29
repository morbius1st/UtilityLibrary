﻿#region using

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
	/// TriState (inverted) Selection: Can select multiple nodes | Selecting a branch node
	/// selects/deselects the branch | remembers last selection states.
	/// By inverted, the checkbox used inverts the check-mark sequence to be:<br/>
	/// unchecked -> mixed -> checked -> unchecked<br/>
	/// must use the checkbox: CsCheckBoxTriState
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

		private SelectedListTriState selList { get; }
	#endregion

	#region public methods

		// VALUE
		protected override bool? AdjustValue( ITreeNode? node, bool? newValue)
		{
			// if null, return false
			if (!newValue.HasValue)
			{
				return false;
			}

			// if false
			if (newValue == false)
			{
				return null;
			}

			// only choice left
			return true;
		}


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
			if (!Selected!.Select(node)) return false;

			bool clearPriorChecked = false;

			node.Select();

			if (node!.PriorStateHasValue() && node!.IParentNode!.PriorStateHasValue())
			{
				// [A4]
				M.WriteLine("[A4])");

				clearPriorChecked = true;

				selectChildren(node, null);

				node.UnsetPriorState();
			}
			else
			{
				// either [A1] or [A2] or [A3]
				M.WriteLine("[A1] or [A2] or [A3])");

				if (node.HasNodes)
				{
					// [A2] or [A3] (sometimes)
					selectChildren(node, false);
				}
			}

			applySelectToParents(node, clearPriorChecked);

			return true;
		}

		// deselect a node
		// node went from checked to unchecked
		protected override bool deselect(ITreeNode? node)
		{
			if (!Selected!.Deselect(node)) return false;

			// bool hasChildren = false;
			bool clearPriorChecked = false;

			if (node.PriorStateHasValue() && !node.IParentNode!.PriorStateHasValue())
			{
				// [B3]
				M.WriteLine("[B3])");

				// node will always x -> m
				node.RestoreState();
				node.UnsetPriorState();

				// special case, undo the deselect of the node
				Selected!.Select(node);

				setAllChildrenToPriorState(node);
			}
			else
			{
				// [B1] or [B2] & [B4] never occurs
				M.WriteLine("[B1] or [B2])");

				node!.Deselect();

				if (node.HasNodes)
				{
					deselectAllChildren(node, false);
				}
			}

			applyDeselectToParents(node, clearPriorChecked);

			return true;
		}

		// only applies when tri state
		protected override bool mixed(ITreeNode? node)
		{
			M.WriteLine("[C]"); 

			Selected!.Deselect(node);
			
			node.SavePriorState();
			node.Deselect();

			deselectAllChildren(node, true);

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

		// CHILDREN

		/// <summary>
		/// set this node and all children nodes to their
		/// prior state - selecting / unselecting as applies
		/// unset the prior state
		/// </summary>
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

		/// <summary>
		/// remove the prior state from nodes children.
		/// does not change the top level node supplied
		/// </summary>
		private void setAllChildrenToUnset(ITreeNode? node)
		{
			if (!node!.HasNodes) return;

			foreach (ITreeNode childNode in node.EnumNodes())
			{
				setAllChildrenToUnset(childNode);

				childNode.UnsetPriorState();
			}
		}

		/// <summary>
		/// select all children nodes.  
		/// does not change the top level node supplied.
		/// if saveState is true set priorState.
		/// if saveState is null clear priorState.
		/// </summary>
		private void selectChildren(ITreeNode? node, bool? adjustState)
		{
			// adjust state:
			// null: do nothing
			// true: save state
			// false: clear prior state

			// M.WriteLine($"\tprocess| {node.NodeKey}");

			if (!node!.HasNodes) return;

			foreach (ITreeNode childNode in node.EnumNodes())
			{
				selectChildren(childNode, adjustState);

				if (adjustState != false)
				{
					if (adjustState == true)
					{
						childNode.SavePriorState();
					}
					else
					{
						// set to null
						childNode.UnsetPriorState();
					}
				}

// select
				Selected!.Select(childNode);
				childNode.Select();
			}
		}

		/// <summary>
		/// deselect all children.  do not change priorState.
		/// does not change the top level node supplied
		/// </summary>
		private void deselectAllChildren(ITreeNode? node, bool saveState)
		{
			if (!node!.HasNodes) return;

			foreach (ITreeNode childNode in node.EnumNodes())
			{
				deselectAllChildren(childNode, saveState);
// deselect
				if (saveState)
				{
					childNode.SavePriorState();
				}

				Selected!.Deselect(childNode);
				childNode.Deselect();
			}
		}

		/// <summary>
		/// unset priorState for all children.
		/// does not change the top level node supplied.
		/// </summary>
		/// <param name="node"></param>
		private void setChildrenToUnset(ITreeNode? node)
		{
			if (!node!.HasNodes) return;

			foreach (ITreeNode childNode in node.EnumNodes())
			{
				childNode.UnsetPriorState();
			}
		}

		// PARENTS

		/// <summary>
		/// apply select all parents setting the parent to
		/// checked or mixed as applies
		/// </summary>
		/// <param name="node"></param>
		private void applySelectToParents(ITreeNode? node, bool clearPriorState)
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

			if (clearPriorState)
			{
				node.IParentNode.UnsetPriorState();

				setChildrenToUnset(node.IParentNode);
			}

			// continue up the tree
			applySelectToParents(node.IParentNode, clearPriorState);
		}

		/// <summary>
		/// apply deselect to app parents.
		/// set parent to unchecked or mixed as applies.
		/// if clearPriorState, remove prior state.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="clearPriorState"></param>
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
				// Selected!.Deselect(node.IParentNode);
				Selected!.Select(node.IParentNode);

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
			return $"this is {nameof(TreeSelectorTriStateInverted) }";
		}

	#endregion
	}
}