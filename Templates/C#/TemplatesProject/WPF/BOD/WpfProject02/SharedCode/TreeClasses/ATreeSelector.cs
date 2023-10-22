// Solution:     WpfProject02
// Project:       WpfProject02
// File:             ATreeSelector.cs
// Created:      2023-10-21 (10:13 AM)

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

public abstract class ATreeSelector<Te> : INotifyPropertyChanged
	where Te : class, ITreeElement
{
	private SelectedList<Te>? selected;

	// private STreeSelection setg;

	public ATreeSelector(SelectedList<Te> selected)
	{
		Selected = selected;
	}

	public ITree Tree { get; set; }

	protected SelectedList<Te>? Selected
	{
		get => selected;
		private set
		{
			selected = value;
			OnPropertyChanged();
		}
	}

	public abstract Selection.SelectMode SelectionMode { get; }
	public abstract Selection.SelectFirstClass SelectionFirstClass { get; }
	public abstract Selection.SelectSecondClass SelectionSecondClass { get; }
	public abstract Selection.SelectTreeAllowed CanSelectTree { get; }

	public bool IsTriState => SelectionFirstClass == Selection.SelectFirstClass.TRI_STATE;
	public bool MaySelectTree => CanSelectTree == Selection.SelectTreeAllowed.YES;
	public bool WillSelectLeaves => SelectionSecondClass == Selection.SelectSecondClass.NODES_AND_LEAVES;

	public int SelectedCount => Selected!.SelectedCount;
	public int PriorSelectedCount => Selected!.PriorSelectedCount;

	public Te? CurrentSelected => Selected!.CurrentSelected;
	public Te? CurrentPriorSelected => Selected!.CurrentPriorSelected;

	public bool HasSelection => Selected != null && SelectedCount > 0;
	public bool HasPriorSelection => Selected != null && PriorSelectedCount > 0;

	public abstract bool SelectDeselect(Te element);
	public abstract bool TreeSelect();
	public abstract bool TreeDeSelect();

	public void Clear()
	{
		selected.Clear();

		OnPropertyChanged(nameof(Selected));

		updateProperties();

		
	}

	protected void updateProperties()
	{
		OnPropertyChanged(nameof(CurrentSelected));
		OnPropertyChanged(nameof(SelectedCount));

		OnPropertyChanged(nameof(CurrentPriorSelected));
		OnPropertyChanged(nameof(PriorSelectedCount));

		OnPropertyChanged(nameof(HasSelection));
		OnPropertyChanged(nameof(HasPriorSelection));

		OnPropertyChanged(nameof(SelectedCount));
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	[DebuggerStepThrough]
	private void OnPropertyChanged([CallerMemberName] string memberName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
	}

	public IEnumerable<Te> EnumSelected() => selected!.EnumSelected();
	public IEnumerable<Te> EnumPriorSelected() => selected!.EnumPriorSelected();


	public event ISelectedList<Te>.SelectedClearedEventHandler SelectedCleared
	{
		add => Selected!.SelectedCleared += value;
		remove => Selected!.SelectedCleared -= value;
	}

	public event ISelectedList<Te>.PriorSelectedClearedEventHandler PriorSelectedCleared
	{
		add => Selected!.PriorSelectedCleared += value;
		remove => Selected!.PriorSelectedCleared -= value;
	}

	public event ISelectedList<Te>.ElementAddedToSelectedEventHandler ElementAddedToSelected
	{
		add => Selected!.ElementAddedToSelected += value;
		remove => Selected!.ElementAddedToSelected -= value;
	}

	public event ISelectedList<Te>.ElementAddedToPriorSelectedEventHandler ElementAddedToPriorSelected
	{
		add => Selected!.ElementAddedToPriorSelected += value;
		remove => Selected!.ElementAddedToPriorSelected -= value;
	}

	public event ISelectedList<Te>.ElementRemovedFromSelectedEventHandler ElementRemovedFromSelected
	{
		add => Selected!.ElementRemovedFromSelected += value;
		remove => Selected!.ElementRemovedFromSelected -= value;
	}

	public event ISelectedList<Te>.ElementRemovedFromPriorSelectedEventHandler ElementRemovedFromPriorSelected
	{
		add => Selected!.ElementRemovedFromPriorSelected += value;
		remove => Selected!.ElementRemovedFromPriorSelected -= value;
	}
}