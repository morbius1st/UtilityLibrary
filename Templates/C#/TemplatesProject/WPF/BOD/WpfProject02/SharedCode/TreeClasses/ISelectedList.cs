// Solution:     WpfProject02
// Project:       WpfProject02
// File:             ISelectedList.cs
// Created:      2023-10-21 (9:17 AM)

using System.Collections.Generic;
using System.ComponentModel;

namespace SharedCode.TreeClasses;

/*
 selection notes
** use selection state
when selected (SELECTED) - remove from prior list and add to selected list
when deselected (DESELECTED) - remove from selection list and add to prior list
when unset (MIXED) - remove from both lists?
*/

public interface ISelectedList<Te> : INotifyPropertyChanged, IEnumerable<Te>
	where Te : class, ITreeElement
{
	// public Selection.SelectMode SelectionMode { get; }

	public Te? CurrentSelected { get; }
	public Te? CurrentPriorSelected { get; }

	public int SelectedCount { get; }
	public int PriorSelectedCount { get; }

	public IEnumerable<Te> EnumSelected();
	public IEnumerable<Te> EnumPriorSelected();

	public bool Select(Te element, bool resetPriorSelected);
	public bool DeSelect(Te element);

	public void MoveSelectedToPrior();


	public void Clear();


	public delegate void SelectedClearedEventHandler(object sender);

	public event SelectedClearedEventHandler SelectedCleared;

	public delegate void PriorSelectedClearedEventHandler(object sender);

	public event PriorSelectedClearedEventHandler PriorSelectedCleared;



	public delegate void ElementAddedToSelectedEventHandler(object sender, Te e);

	public event ElementAddedToSelectedEventHandler ElementAddedToSelected;


	public delegate void ElementAddedToPriorSelectedEventHandler(object sender, Te e);

	public event ElementAddedToPriorSelectedEventHandler ElementAddedToPriorSelected;

	public delegate void ElementRemovedFromSelectedEventHandler(object sender, Te element);

	public event ElementRemovedFromSelectedEventHandler ElementRemovedFromSelected;

	public delegate void ElementRemovedFromPriorSelectedEventHandler(object sender, Te e);

	public event ElementRemovedFromPriorSelectedEventHandler ElementRemovedFromPriorSelected;

	public event PropertyChangedEventHandler PropertyChanged;
}