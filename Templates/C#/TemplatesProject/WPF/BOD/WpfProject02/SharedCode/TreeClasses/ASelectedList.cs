// Solution:     WpfProject02
// Project:       WpfProject02
// File:             SelectionList.cs
// Created:      2023-10-17 (9:20 PM)

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace SharedCode.TreeClasses
{
	public abstract class ASelectedList : ISelectedList

	{
		protected ObservableCollection<ITreeNode> selected;
		protected ObservableCollection<ITreeNode> priorSelected;

		public ASelectedList( /*Selection.SelectMode mode */)
		{
			ClearSelectedList();
			ClearPriorSelectedList();

			OnPropertyChanged(Name);

			updateSelectedProperties();
		}

		public abstract string Name { get; }

		public ObservableCollection<ITreeNode> Selected
		{
			get => selected;
			set
			{
				selected = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<ITreeNode> PriorSelected
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


		public ITreeNode? CurrentSelected => Selected != null && Selected.Count > 0 ? Selected?[0] : null;
		public ITreeNode? CurrentPriorSelected => PriorSelected != null && PriorSelected.Count > 0 ? PriorSelected?[0] : null;


		public abstract bool Select(ITreeNode element);

		public abstract bool Deselect(ITreeNode element);


		public void Clear()
		{
			ClearSelectedList();
			ClearPriorSelectedList();
		}

		protected void ClearSelectedList()
		{
			Selected = new ObservableCollection<ITreeNode>();

			// RaiseSelectedClearedEvent();
			updateSelectedProperties();
		}

		protected void ClearPriorSelectedList()
		{
			PriorSelected = new ObservableCollection<ITreeNode>();
			updatePriorSelectedProperties();
		}

		/// <summary>
		/// move all of the selected items to
		///	the prior list and make a new, blank list
		///	when the selection process automatically 
		///	selects items, use this to reset the lists
		/// upon a selection
		/// </summary>
		public void MoveSelectedListToPrior()
		{
			if (selected.Count < 1) return;

			priorSelected = new ObservableCollection<ITreeNode>(selected);

			selected = new ObservableCollection<ITreeNode>();
		}

		/// <summary>
		/// take the top item in the selected list
		/// and move to the prior list
		/// </summary>
		public void MoveSelectedToPrior()
		{
			if (selected.Count < 1) return;

			ITreeNode node = selected[0];

			priorSelected.Add(node);

			selected.Remove(node);

		}

		/// <summary>
		/// adds an element to the selected list as
		/// long as it is not already in the list
		/// </summary>
		protected bool addElementToSelected(ITreeNode element)
		{
			if (element == null) return false;
			if (selected.Contains(element)) return false;

			selected.Add(element);

			// RaiseElementAddedToSelectedEvent(element);
			// updateSelectedProperties();

			return true;
		}

		/// <summary>
		/// adds an element to the prior selected list as
		/// long as it is not already in the list
		/// </summary>
		protected bool addElementToPriorSelected(ITreeNode element)
		{
			if (element == null) return false;
			if (priorSelected.Contains(element)) return false;

			priorSelected.Add(element);

			// RaiseElementAddedToPriorSelectedEvent(element);
			// updatePriorSelectedProperties();

			return true;
		}

		/// <summary>
		/// removes an element to the selected list as
		/// long as it is in the list
		/// </summary>
		protected void removeElementFromSelected(ITreeNode element)
		{
			if (element == null) return;
			if (!selected.Contains(element)) return;

			selected.Remove(element);

			// RaiseElementRemovedFromSelectedEvent(element);
			// updateSelectedProperties();
		}

		/// <summary>
		/// removes an element to the prior selected list as
		/// long as it is in the list
		/// </summary>
		protected void removeElementFromPriorSelected(ITreeNode element)
		{
			if (element == null) return;
			if (!priorSelected.Contains(element)) return;

			priorSelected.Remove(element);

			// RaiseElementRemovedFromPriorSelectedEvent(element);
			// updatePriorSelectedProperties();
		}

		protected void updateSelectedProperties()
		{
			OnPropertyChanged(nameof(Selected));
		
			OnPropertyChanged(nameof(SelectedCount));
			OnPropertyChanged(nameof(PriorSelectedCount));
		
			OnPropertyChanged(nameof(CurrentSelected));
			OnPropertyChanged(nameof(CurrentPriorSelected));
		}

		protected void updatePriorSelectedProperties()
		{
			if (priorSelected != null) OnPropertyChanged(nameof(PriorSelected));
			OnPropertyChanged(nameof(PriorSelectedCount));
			if (CurrentPriorSelected != null) OnPropertyChanged(nameof(CurrentPriorSelected));
		}


		public IEnumerable<ITreeNode> EnumSelected()
		{
			if (selected != null && selected.Count > 0)
			{
				foreach (ITreeNode e in selected)
				{
					yield return e;
				}
			}
		}

		public IEnumerable<ITreeNode> EnumPriorSelected()
		{
			if (priorSelected != null && priorSelected.Count > 0)
			{
				foreach (ITreeNode e in priorSelected)
				{
					yield return e;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<ITreeNode> GetEnumerator()
		{
			yield break;
		}


		// public delegate void SelectedClearedEventHandler(object sender);
		//
		// public event ISelectedList.SelectedClearedEventHandler SelectedCleared;
		//
		// protected virtual void RaiseSelectedClearedEvent()
		// {
		// 	SelectedCleared?.Invoke(this);
		// }
		//
		//
		// public delegate void PriorSelectedClearedEventHandler(object sender);
		//
		// public event ISelectedList.PriorSelectedClearedEventHandler PriorSelectedCleared;
		//
		// protected virtual void RaisePriorSelectedClearedEvent()
		// {
		// 	PriorSelectedCleared?.Invoke(this);
		// }
		//
		//
		// public delegate void ElementAddedToSelectedEventHandler(object sender, ITreeNode element);
		//
		// public event ISelectedList.ElementAddedToSelectedEventHandler ElementAddedToSelected;
		//
		// [DebuggerStepThrough]
		// protected void RaiseElementAddedToSelectedEvent(ITreeNode element)
		// {
		// 	updateSelectedProperties();
		// 	ElementAddedToSelected?.Invoke(this, element);
		// }
		//
		//
		// public delegate void ElementAddedToPriorSelectedEventHandler(object sender, ITreeNode element);
		//
		// public event ISelectedList.ElementAddedToPriorSelectedEventHandler ElementAddedToPriorSelected;
		//
		// [DebuggerStepThrough]
		// protected void RaiseElementAddedToPriorSelectedEvent(ITreeNode element)
		// {
		// 	updatePriorSelectedProperties();
		// 	ElementAddedToPriorSelected?.Invoke(this, element);
		// }
		//
		//
		// public delegate void ElementRemovedFromSelectedEventHandler(object sender, ITreeNode element);
		//
		// public event ISelectedList.ElementRemovedFromSelectedEventHandler ElementRemovedFromSelected;
		//
		// [DebuggerStepThrough]
		// protected void RaiseElementRemovedFromSelectedEvent(ITreeNode element)
		// {
		// 	updateSelectedProperties();
		// 	ElementRemovedFromSelected?.Invoke(this, element);
		// }
		//
		//
		// public delegate void ElementRemovedFromPriorSelectedEventHandler(object sender, ITreeNode element);
		//
		// public event ISelectedList.ElementRemovedFromPriorSelectedEventHandler? ElementRemovedFromPriorSelected;
		//
		// [DebuggerStepThrough]
		// protected void RaiseElementRemovedFromPriorSelectedEvent(ITreeNode element)
		// {
		// 	updatePriorSelectedProperties();
		// 	ElementRemovedFromPriorSelected?.Invoke(this, element);
		// }

		public event PropertyChangedEventHandler PropertyChanged;

		[DebuggerStepThrough]
		protected void OnPropertyChanged([CallerMemberName] string memberName = "" )
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	}


}