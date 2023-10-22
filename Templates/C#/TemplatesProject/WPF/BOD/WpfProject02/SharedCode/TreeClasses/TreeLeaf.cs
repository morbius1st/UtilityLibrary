// Solution:     WpfProject02
// Project:       WpfProject02
// File:             TreeLeaf.cs
// Created:      2023-10-07 (10:33 AM)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SharedCode.SampleData;
using WpfProject02.Annotations;

using static SharedCode.TreeClasses.Selection;

namespace SharedCode.TreeClasses;

public class TreeLeaf<TNd, TLd> : INotifyPropertyChanged, 
	IComparer<TreeLeaf<TNd, TLd>>, 
	ICloneable, 
	ITreeLeaf
	// ITreeLeaf<TNd, TLd>,
	where TNd : class
	where TLd : class
{
#region private fields

	private string leafKey;
	private TreeNode<TNd, TLd> parentNode;
	private TLd leafData;
	private bool? isChecked;
	private bool isChosen;

	private SelectedState state;

#endregion

#region ctor

	public TreeLeaf() { }

	public TreeLeaf(string leafKey, TLd leafData,
		TreeNode<TNd, TLd> parentNode = null)
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

	public TreeNode<TNd, TLd> ParentNode
	{
		get => parentNode;
		private set
		{
			if (Equals(value, parentNode)) return;
			parentNode = value;
			OnPropertyChanged();
		}
	}

	// public ITreeNode<TNd, TLd> HostNode => (ITreeNode<TNd, TLd>) ParentNode;
	public ITreeNode HostNode => (ITreeNode) ParentNode;

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

	public bool? IsChecked
	{
		get => isChecked;
		set
		{
			isChecked = value;
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

	public SelectedState State
	{
		get => state; 
		set
		{
			if (state == value) return;
			state = value;

			if (state!= SelectedState.UNSET)
			{
				isChecked = Tree<TNd, TLd>.boolList[(int) state];
				OnPropertyChanged(nameof(IsChecked));
			}

			OnPropertyChanged();
		}
	}


#endregion

#region public methods

	public bool MoveParent(TreeNode<TNd, TLd> node)
	{
		if (node == null || !node.ContainsLeaf(leafKey)) return false;

		ParentNode = node;

		return true;
	}

	public void Select()
	{
		isChecked = true;
		OnPropertyChanged(nameof(IsChecked));
	}

	public void DeSelect()
	{
		isChecked = false;
		OnPropertyChanged(nameof(IsChecked));
	}

#endregion

#region system overrides

	public int Compare(TreeLeaf<TNd, TLd>? x, TreeLeaf<TNd, TLd>? y)
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
		return $"{nameof(TreeLeaf<TNd, TLd>)} | name| {LeafKey} | parent| {parentNode?.NodeKey ?? "is null"}";
	}

#endregion
}