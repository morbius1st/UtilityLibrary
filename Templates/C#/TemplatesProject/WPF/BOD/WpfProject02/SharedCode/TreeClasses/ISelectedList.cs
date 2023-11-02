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

public interface ISelectedList : INotifyPropertyChanged, IEnumerable<ITreeNode>
{
	// public Selection.SelectMode SelectionMode { get; }

	public ITreeNode? CurrentSelected { get; }
	public ITreeNode? CurrentPriorSelected { get; }

	public int SelectedCount { get; }
	public int PriorSelectedCount { get; }

	public IEnumerable<ITreeNode> EnumSelected();
	public IEnumerable<ITreeNode> EnumPriorSelected();


	public bool Select(ITreeNode node);
	public bool Deselect(ITreeNode node);


	public void MoveSelectedListToPrior();

	public void Clear();

}