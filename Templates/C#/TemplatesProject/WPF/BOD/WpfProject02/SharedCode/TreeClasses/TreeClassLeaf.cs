// Solution:     WpfProject02
// Project:       WpfProject02
// File:             TreeClassLeaf.cs
// Created:      2023-10-07 (10:33 AM)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SharedCode.SampleData;
using WpfProject02.Annotations;

namespace SharedCode.TreeClasses;

public class TreeClassLeaf<TNd, TLd> : INotifyPropertyChanged, 
	IComparer<TreeClassLeaf<TNd, TLd>>, ICloneable
	where TNd : class
	where TLd : class
{
#region private fields

	private string leafKey;
	private TreeClassNode<TNd, TLd> parentNode;
	private TLd leafData;
	private bool isSelected;
	private bool isChosen;

#endregion

#region ctor

	public TreeClassLeaf() { }

	public TreeClassLeaf(string leafKey, TLd leafData,
		TreeClassNode<TNd, TLd> parentNode = null)
	{
		LeafKey = leafKey;
		LeafData = leafData;
		ParentNode = parentNode;
	}

#endregion

#region public properties

	public string LeafKey
	{
		get => leafKey;
		private set
		{
			if (value == leafKey) return;
			leafKey = value;
			OnPropertyChanged();
		}
	}

	public TreeClassNode<TNd, TLd> ParentNode
	{
		get => parentNode;
		private set
		{
			if (Equals(value, parentNode)) return;
			parentNode = value;
			OnPropertyChanged();
		}
	}

	public TLd LeafData
	{
		get => leafData;
		set
		{
			if (EqualityComparer<TLd>.Default.Equals(value, leafData)) return;
			leafData = value;
			OnPropertyChanged();
		}
	}

	public bool IsSelected
	{
		get => isSelected;
		set
		{
			isSelected = value;
			OnPropertyChanged();
		}
	}

	public bool IsChosen
	{
		get => isChosen;
		set
		{
			isChosen = value;
			OnPropertyChanged();
		}
	}

#endregion

#region public methods

	public bool MoveParent(TreeClassNode<TNd, TLd> node)
	{
		if (node == null || !node.ContainsLeaf(leafKey)) return false;

		ParentNode = node;

		return true;
	}

	public void SelectLeaf()
	{
		isSelected = true;
		OnPropertyChanged(nameof(IsSelected));
	}

	public void DeSelectLeaf()
	{
		isSelected = false;
		OnPropertyChanged(nameof(IsSelected));
	}

#endregion

#region system overrides

	public int Compare(TreeClassLeaf<TNd, TLd>? x, TreeClassLeaf<TNd, TLd>? y)
	{
		return 0;
	}

	public object Clone()
	{
		return null!;
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	[NotifyPropertyChangedInvocator]
	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public override string ToString()
	{
		return $"{nameof(TreeClassLeaf<TNd, TLd>)} | name| {LeafKey} | parent| {parentNode?.NodeKey ?? "is null"}";
	}

#endregion
}