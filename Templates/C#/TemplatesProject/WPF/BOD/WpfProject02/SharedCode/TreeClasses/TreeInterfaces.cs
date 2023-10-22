// Solution:     WpfProject02
// Project:       WpfProject02
// File:             TreeInterfaces.cs
// Created:      2023-10-15 (12:01 PM)

using System.Collections;
using System.Collections.Generic;

namespace SharedCode.TreeClasses;

public interface ITreeElement
{
	public bool? IsChecked { get;}

	public void Select();
	public void DeSelect();

	public Selection.SelectedState State { get; set; }
}


public interface ITreeLeaf : ITreeElement
{
	public string LeafKey { get; }
	public ITreeNode  HostNode { get; }

	public void Select();
	public void DeSelect();
}

public interface ITreeNode : IEnumerable, ITreeElement

{
	public string NodeKey { get; }
	public ITree ITree { get; }
	public ITreeNode IParentNode { get; }

	public bool? PriorIsChecked { get; set; }
	public bool IsTriState { get; }
	public bool IsChosen { get; set; }
	public bool IsExpanded { get; set; }
	public bool IsEnabled { get; set; }
	public bool HasNodes { get; }
	public bool HasLeaves { get; }

	public void Select();
	public void DeSelect();

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
