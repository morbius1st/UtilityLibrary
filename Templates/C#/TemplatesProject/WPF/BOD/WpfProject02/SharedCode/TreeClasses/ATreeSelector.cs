// Solution:     WpfProject02
// Project:       WpfProject02
// File:             ATreeSelector.cs
// Created:      2023-10-21 (10:13 AM)

using SharedCode.ShDebugAssist;
using SharedWPF.ShWin;

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SharedCode.TreeClasses;

public struct STreeSelection
{
	public STreeSelection(ITree tree)
	{
		this.Tree = tree;

		this.Mode = Selection.SelectMode.INDIVIDUAL;
		this.SelectionFirstClass = Selection.SelectFirstClass.NODE_ONLY;
		this.SelectionSecondClass = Selection.SelectSecondClass.NODES_ONLY;
		this.CanSelectTree = Selection.SelectTreeAllowed.NO;
	}

	public ITree Tree { get; set; }
	public Selection.SelectMode Mode { get; set; }
	public Selection.SelectTreeAllowed CanSelectTree { get; set; }
	public Selection.SelectSecondClass SelectionSecondClass { get; set; }
	public Selection.SelectFirstClass SelectionFirstClass { get; set; }
}

public abstract class ATreeSelector : INotifyPropertyChanged
{
	private  ShDebugMessages M = Examples.M;

	protected ASelectedList? selected;

	// private STreeSelection setg;

	public ATreeSelector(ASelectedList selected)
	{
		Selected = selected;
	}

	public ITree? Tree { get; set; }

	public ASelectedList? Selected
	{
		get => selected;
		private set
		{
			selected = value;
			OnPropertyChanged();
		}
	}

	public abstract string Name { get; }

	public abstract Selection.SelectMode SelectionMode { get; }
	public abstract Selection.SelectFirstClass SelectionFirstClass { get; }
	public abstract Selection.SelectSecondClass SelectionSecondClass { get; }
	public abstract Selection.SelectTreeAllowed CanSelectTree { get; }

	public bool IsTriState => SelectionFirstClass == Selection.SelectFirstClass.TRI_STATE;
	public bool MaySelectTree => CanSelectTree == Selection.SelectTreeAllowed.YES;
	public bool WillSelectLeaves => SelectionSecondClass == Selection.SelectSecondClass.NODES_AND_LEAVES;

	public int SelectedCount => Selected!.SelectedCount;
	public int PriorSelectedCount => Selected!.PriorSelectedCount;

	public ITreeNode? CurrentSelected => Selected!.CurrentSelected;
	public ITreeNode? CurrentPriorSelected => Selected!.CurrentPriorSelected;

	public bool HasSelection => Selected != null && SelectedCount > 0;
	public bool HasPriorSelection => Selected != null && PriorSelectedCount > 0;

	public void Clear()
	{
		selected!.Clear();

		OnPropertyChanged(nameof(Selected));

		updateProperties();

		
	}

	protected void updateProperties()
	{
		OnPropertyChanged(nameof(Selected));

		OnPropertyChanged(nameof(CurrentSelected));
		OnPropertyChanged(nameof(SelectedCount));

		OnPropertyChanged(nameof(CurrentPriorSelected));
		OnPropertyChanged(nameof(PriorSelectedCount));

		OnPropertyChanged(nameof(HasSelection));
		OnPropertyChanged(nameof(HasPriorSelection));

		OnPropertyChanged(nameof(SelectedCount));
	}


	protected abstract bool select(ITreeNode? node);
	protected abstract bool selectEx(ITreeNode? node);
	protected abstract bool deselect(ITreeNode? node);
	protected abstract bool deselectEx(ITreeNode? node);
	protected abstract bool mixed(ITreeNode? node);
	protected abstract bool treeSelect();


	public bool TreeSelect()
	{
		if (Tree.IRootNode == null || !Tree.IRootNode.HasNodes) return false;
		if (!treeSelect()) return false;

		updateProperties();
		RaiseTreeSelectedEvent(Tree);

		return true;
	}

	public bool TreeDeSelect() // doubles as "deselect all"
	{
		if (Tree.IRootNode == null || !Tree.IRootNode.HasNodes) return false;
		if (!HasSelection) return false;

		// cannot modify the selected list while using same to enumerate
		// need to create a independant list of deselected nodes
		// and them remove them from the selected list

		List<ITreeNode> nodesToDeselect = new List<ITreeNode>();

		foreach (ITreeNode node in selected.EnumSelected())
		{
			
			nodesToDeselect.Add(node);
			node.Deselect();
		}

		foreach (ITreeNode node in nodesToDeselect)
		{
			selected.Deselect(node);
		}

		updateProperties();

		RaiseTreeDeselectedEvent(Tree);

		return true;
	}


	// checkbox-tri-state / inverted
	// deselected -> mixed -> selected -> deselected
	public bool SelectDeselect(ITreeNode? node, bool? value)
	{
		M.Write($"apply {(value.HasValue ? value : "null")} to {node.NodeKey} | ");

		// Debug.WriteLine($"\nstart| applying {(value.HasValue ? value : "null")} to {node.NodeKey}");

		if (node == null) return false;

		if (value.HasValue && value==true)
		{
			// get here when unchecked is selected
			if (!select(node)) return false;
			// update properties at the end
			
		}
		else if (value.HasValue && value==false)
		{
			// get here when checked is selected
			if (!deselect(node)) return false;
			// update properties at the end
			
		}
		else
		{
			// mixed is considered as selected
			// get here when a mixed selected
			if (!mixed(node)) return false;
		}

		// Debug.WriteLine("update properties");

		updateProperties();

		return true;
	}






	public event PropertyChangedEventHandler? PropertyChanged;

	[DebuggerStepThrough]
	protected void OnPropertyChanged([CallerMemberName] string memberName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
	}

	public IEnumerable<ITreeNode> EnumSelected() => selected!.EnumSelected();
	public IEnumerable<ITreeNode> EnumPriorSelected() => selected!.EnumPriorSelected();

	/* events:
	** node selected
	** node deselected
	* leaf selected
	* leaf deselected
	** tree selected
	** tree deselected

	*/


	public delegate void NodeSelectedEventHandler(object sender, ITreeNode node);

	public event ATreeSelector.NodeSelectedEventHandler NodeSelected;

	protected virtual void RaiseNodeSelectedEvent(ITreeNode node)
	{
		NodeSelected?.Invoke(this, node);
	}


	public delegate void NodeDeselectedEventHandler(object sender, ITreeNode node);

	public event ATreeSelector.NodeDeselectedEventHandler NodeDeselected;

	protected virtual void RaiseNodeDeselectedEvent(ITreeNode node)
	{
		NodeDeselected?.Invoke(this, node);
	}

	public delegate void LeafSelectedEventHandler(object sender, ITreeLeaf node);

	public event ATreeSelector.LeafSelectedEventHandler LeafSelected;

	protected virtual void RaiseLeafSelectedEvent(ITreeLeaf node)
	{
		LeafSelected?.Invoke(this, node);
	}


	public delegate void LeafDeselectedEventHandler(object sender, ITreeLeaf node);

	public event ATreeSelector.LeafDeselectedEventHandler LeafDeselected;

	protected virtual void RaiseLeafDeselectedEvent(ITreeLeaf node)
	{
		LeafDeselected?.Invoke(this, node);
	}

	public delegate void TreeSelectedEventHandler(object sender, ITree tree);

	public event ATreeSelector.TreeSelectedEventHandler TreeSelected;

	protected virtual void RaiseTreeSelectedEvent(ITree Tree)
	{
		TreeSelected?.Invoke(this, Tree);
	}


	public delegate void TreeDeselectedEventHandler(object sender, ITree Tree);

	public event ATreeSelector.TreeDeselectedEventHandler TreeDeselected;

	protected virtual void RaiseTreeDeselectedEvent(ITree Tree)
	{
		TreeDeselected?.Invoke(this, Tree);
	}


	public delegate void SelectionClearedEventHandler(object sender);

	public event ATreeSelector.SelectionClearedEventHandler SelectionCleared;

	protected virtual void RaiseSelectionClearedEvent()
	{
		SelectionCleared?.Invoke(this);
	}







	// public event ISelectedList.SelectedClearedEventHandler SelectedCleared
	// {
	// 	add => Selected!.SelectedCleared += value;
	// 	remove => Selected!.SelectedCleared -= value;
	// }
	//
	// public event ISelectedList.PriorSelectedClearedEventHandler PriorSelectedCleared
	// {
	// 	add => Selected!.PriorSelectedCleared += value;
	// 	remove => Selected!.PriorSelectedCleared -= value;
	// }
	//
	// public event ISelectedList.ElementAddedToSelectedEventHandler ElementAddedToSelected
	// {
	// 	add => Selected!.ElementAddedToSelected += value;
	// 	remove => Selected!.ElementAddedToSelected -= value;
	// }
	//
	// public event ISelectedList.ElementAddedToPriorSelectedEventHandler ElementAddedToPriorSelected
	// {
	// 	add => Selected!.ElementAddedToPriorSelected += value;
	// 	remove => Selected!.ElementAddedToPriorSelected -= value;
	// }
	//
	// public event ISelectedList.ElementRemovedFromSelectedEventHandler ElementRemovedFromSelected
	// {
	// 	add => Selected!.ElementRemovedFromSelected += value;
	// 	remove => Selected!.ElementRemovedFromSelected -= value;
	// }
	//
	// public event ISelectedList.ElementRemovedFromPriorSelectedEventHandler ElementRemovedFromPriorSelected
	// {
	// 	add => Selected!.ElementRemovedFromPriorSelected += value;
	// 	remove => Selected!.ElementRemovedFromPriorSelected -= value;
	// }

}