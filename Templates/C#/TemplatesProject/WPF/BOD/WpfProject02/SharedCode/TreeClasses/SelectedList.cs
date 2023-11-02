// Solution:     WpfProject02
// Project:       WpfProject02
// File:             SelectedList.cs
// Created:      2023-10-23 (11:19 PM)

namespace SharedCode.TreeClasses;


/// <summary>
/// select items using a tri-state system. that is
/// the node can be selected, deselected, or mixed
/// mixed means that some, but not all, of its children
/// are selected.  the tri state system also
/// cycles selected state of children and parents
/// selection cycle is
/// not selected -> selected -> mixed -> not selected
/// </summary>
public class SelectedListTriState : ASelectedList
{
	public override bool Select(ITreeNode? element)
	{
		if (element == null
			|| element.IamRoot) return false;

		removeElementFromPriorSelected(element);

		if (selected.Count == 0) ClearPriorSelectedList();

		return addElementToSelected(element);
	}

	public override bool Deselect(ITreeNode element)
	{
		if (element == null) return false;

		removeElementFromSelected(element);

		return addElementToPriorSelected(element);
	}

	public override string Name => "tri-state select list";

}


/// <summary>
/// can select multiple items.  however, selecting
/// a node in a branch, selects the branch from that point
/// down.  can only deselect an item by deselecting
/// a node.  however, deselecting a branch node
/// deselects the branch from that point down.
/// when the selected list returns to no items, the
/// next selection clears the prior selection list
/// </summary>
public class SelectedListExtended : ASelectedList
{
	public override bool Select(ITreeNode element)
	{
		if (element == null) return false;

		removeElementFromPriorSelected(element);
			
		if (selected.Count == 0) ClearPriorSelectedList();

		return addElementToSelected(element);
	}

	public override bool Deselect(ITreeNode element)
	{
		if (element == null) return false;

		removeElementFromSelected(element);

		return addElementToPriorSelected(element);
	}

	// protected override void updateSelectedProperties() { }

	public override string Name => "name";

}

/// <summary>
/// select multiple items - selecting an unselected
/// item leaves all other items selected.  only deselecting
/// an item can deselect that item.  when the selected
/// list returns to no items, the next selection clears
/// the prior selection list
/// </summary>
public class SelectedListMultiple : ASelectedList
{
	public override bool Select(ITreeNode element)
	{
		if (element == null) return false;

		removeElementFromPriorSelected(element);

		if (selected.Count == 0) ClearPriorSelectedList();

		// ClearPriorSelectedList(true);

		return addElementToSelected(element);
	}

	public override bool Deselect(ITreeNode element)
	{
		if (element == null) return false;

		removeElementFromSelected(element);

		return addElementToPriorSelected(element);
	}

	// protected override void updateSelectedProperties() { }

	public override string Name => "name";
}

/// <summary>
/// select individual items - only one at a time
/// selecting an item, deselects the current selection (if any)
/// and move this to the prior list
/// </summary>
public class SelectedListIndividual : ASelectedList
{
	public override bool Select(ITreeNode element)
	{
		if (element == null) return false;

		removeElementFromPriorSelected(element);

		MoveSelectedToPrior();

		return addElementToSelected(element);
	}

	public override bool Deselect(ITreeNode element)
	{
		if (element == null) return false;

		removeElementFromSelected(element);

		return addElementToPriorSelected(element);
	}

	// protected override void updateSelectedProperties() { }

	public override string Name => "name";
}