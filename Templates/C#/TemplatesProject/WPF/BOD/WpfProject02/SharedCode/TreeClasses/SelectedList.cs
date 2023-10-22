// Solution:     WpfProject02
// Project:       WpfProject02
// File:             SelectionList.cs
// Created:      2023-10-17 (9:20 PM)

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace SharedCode.TreeClasses;

public class SelectedList<Te> : ISelectedList<Te>
	where Te : class, ITreeElement
{
	private List<Te> selected;
	private List<Te> priorSelected;

	public SelectedList(/*Selection.SelectMode mode */)
	{
		ClearSelectedList();
		ClearPriorSelectedList();

		updateSelectedProperties();
	}

	// public Selection.SelectMode SelectionMode { get; }

	private List<Te> Selected
	{
		get => selected;
		set
		{
			selected = value;
			OnPropertyChanged();
		}
	}

	private List<Te> PriorSelected
	{
		get => priorSelected;
		set
		{
			priorSelected = value;
			OnPropertyChanged();
		}
	}

	public int SelectedCount => Selected.Count;
	public int PriorSelectedCount => PriorSelected.Count;


	public Te? CurrentSelected => Selected != null && Selected.Count > 0 ? Selected?[0] : null;
	public Te? CurrentPriorSelected => PriorSelected != null && PriorSelected.Count > 0 ? PriorSelected?[0] : null;


	public bool Select(Te element, bool resetPriorSelected = false)
	{
		if (element == null) return false;

		removeElementFromPriorSelected(element);

		if (resetPriorSelected) MoveSelectedToPrior();

		return addElementToSelected(element);
	}

	public bool DeSelect(Te element)
	{
		if (element == null) return false;

		removeElementFromSelected(element);

		return addElementToPriorSelected(element);
	}

	public void MoveSelectedToPrior()
	{
		if (selected.Count < 1) return;

		priorSelected = new List<Te>();

		foreach (Te e in selected)
		{
			addElementToPriorSelected(e);
		}

		selected = new List<Te>();
	}

	public void Clear()
	{
		ClearSelectedList();
		ClearPriorSelectedList();

		updateSelectedProperties();
	}


	private void ClearSelectedList()
	{
		selected = new List<Te>();

		RaiseSelectedClearedEvent();
	}

	private void ClearPriorSelectedList()
	{
		priorSelected = new List<Te>();

		RaisePriorSelectedClearedEvent();
	}

	private bool addElementToSelected(Te element)
	{
		if (element == null) return false;
		if (selected.Contains(element)) return false;

		selected.Add(element);

		RaiseElementAddedToSelectedEvent(element);

		return true;
	}

	private bool addElementToPriorSelected(Te element)
	{
		if (element == null) return false;
		if (priorSelected.Contains(element)) return false;

		priorSelected.Add(element);

		RaiseElementAddedToPriorSelectedEvent(element);

		return true;
	}

	private void removeElementFromSelected(Te element)
	{
		if (element == null) return;
		if (!selected.Contains(element)) return;

		selected.Remove(element);

		RaiseElementRemovedFromSelectedEvent(element);
	}

	private void removeElementFromPriorSelected(Te element)
	{
		if (element == null) return;
		if (!priorSelected.Contains(element)) return;

		priorSelected.Remove(element);

		RaiseElementRemovedFromPriorSelectedEvent(element);
	}


	private void updateSelectedProperties()
	{
		OnPropertyChanged(nameof(Selected));

		OnPropertyChanged(nameof(SelectedCount));
		OnPropertyChanged(nameof(PriorSelectedCount));

		OnPropertyChanged(nameof(CurrentSelected));
		OnPropertyChanged(nameof(CurrentPriorSelected));
	}


	private void updatePriorSelectedProperties()
	{
		OnPropertyChanged(nameof(PriorSelected));
		OnPropertyChanged(nameof(PriorSelectedCount));
		OnPropertyChanged(nameof(CurrentPriorSelected));
	}


	public IEnumerable<Te> EnumSelected()
	{
		if (selected != null && selected.Count > 0)
		{
			foreach (Te e in selected)
			{
				yield return e;
			}
		}
	}

	public IEnumerable<Te> EnumPriorSelected()
	{
		if (priorSelected != null && priorSelected.Count > 0)
		{
			foreach (Te e in priorSelected)
			{
				yield return e;
			}
		}
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public IEnumerator<Te> GetEnumerator()
	{
		yield break;
	}


	public delegate void SelectedClearedEventHandler(object sender);

	public event ISelectedList<Te>.SelectedClearedEventHandler SelectedCleared;

	protected virtual void RaiseSelectedClearedEvent()
	{
		SelectedCleared?.Invoke(this);
	}


	public delegate void PriorSelectedClearedEventHandler(object sender);

	public event ISelectedList<Te>.PriorSelectedClearedEventHandler PriorSelectedCleared;

	protected virtual void RaisePriorSelectedClearedEvent()
	{
		PriorSelectedCleared?.Invoke(this);
	}


	public delegate void ElementAddedToSelectedEventHandler(object sender, Te element);

	public event ISelectedList<Te>.ElementAddedToSelectedEventHandler ElementAddedToSelected;

	[DebuggerStepThrough]
	private void RaiseElementAddedToSelectedEvent(Te element)
	{
		updateSelectedProperties();
		ElementAddedToSelected?.Invoke(this, element);
	}


	public delegate void ElementAddedToPriorSelectedEventHandler(object sender, Te element);

	public event ISelectedList<Te>.ElementAddedToPriorSelectedEventHandler ElementAddedToPriorSelected;

	[DebuggerStepThrough]
	private void RaiseElementAddedToPriorSelectedEvent(Te element)
	{
		updatePriorSelectedProperties();
		ElementAddedToPriorSelected?.Invoke(this, element);
	}


	public delegate void ElementRemovedFromSelectedEventHandler(object sender, Te element);

	public event ISelectedList<Te>.ElementRemovedFromSelectedEventHandler ElementRemovedFromSelected;

	[DebuggerStepThrough]
	private void RaiseElementRemovedFromSelectedEvent(Te element)
	{
		updateSelectedProperties();
		ElementRemovedFromSelected?.Invoke(this, element);
	}


	public delegate void ElementRemovedFromPriorSelectedEventHandler(object sender, Te element);

	public event ISelectedList<Te>.ElementRemovedFromPriorSelectedEventHandler? ElementRemovedFromPriorSelected;

	[DebuggerStepThrough]
	private void RaiseElementRemovedFromPriorSelectedEvent(Te element)
	{
		updatePriorSelectedProperties();
		ElementRemovedFromPriorSelected?.Invoke(this, element);
	}

	public event PropertyChangedEventHandler PropertyChanged;

	[DebuggerStepThrough]
	private void OnPropertyChanged([CallerMemberName] string memberName = "" )
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
	}
}