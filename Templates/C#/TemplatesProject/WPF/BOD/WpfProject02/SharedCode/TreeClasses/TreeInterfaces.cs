// Solution:     WpfProject02
// Project:       WpfProject02
// File:             TreeInterfaces.cs
// Created:      2023-10-15 (12:01 PM)

using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using WpfProject02.Annotations;

using static SharedCode.TreeClasses.Selection;

namespace SharedCode.TreeClasses;

public interface ITreeElement
{
	public bool? IsChecked { get;}

	public bool Select();
	public bool Deselect();

	public Selection.SelectedState State { get; set; }
}


public interface ITreeLeaf : ITreeElement
{
	public string LeafKey { get; }
	public ITreeNode  HostNode { get; }

	public bool Select();
	public bool Deselect();
}


[NotNull]
public interface ITreeNode : IEnumerable, ITreeElement

{
	public string NodeKey { get; }
	public ITree ITree { get; }
	public ITreeNode? IParentNode { get; }

	public bool IamRoot {get; }

	public bool IsTriState { get; }
	public bool IsChosen { get; set; }
	public bool IsExpanded { get; set; }
	public bool IsEnabled { get; set; }
	public bool HasNodes { get; }
	public bool HasLeaves { get; }

	public int CountNodes {get; }

	public SelectedState State { get; }
	public SelectedState PriorState { get; }

	public new bool Select();
	public new bool Deselect();
	public new void SetMixed();

	public void NodeCountSelection();
	public bool allChildrenNodesSelected();
	public bool allChildrenNodesDeselected();
	// public void SavePriorChecked();
	// public void RestorerPriorChecked();
	// public void ClearPriorChecked();



	public void SavePriorState();
	public void RestoreState();
	public void UnsetPriorState();
	public bool PriorStateHasValue();

	public IEnumerable<ITreeNode>
		EnumNodes();

	public IEnumerable<ITreeLeaf>
		EnumLeaves();
}

public interface ITree : IEnumerable

{
	public string TreeName { get; }
	public ITreeNode IRootNode { get; }

	public bool IsTriState { get; }

	public IEnumerable<ITreeNode> 
		EnumAllNodes();
	
	public IEnumerable<ITreeLeaf> 
		EnumAllLeaves();
}
