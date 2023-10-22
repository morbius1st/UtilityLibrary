// Solution:     WpfProject02
// Project:       WpfProject02
// File:             NodeEnableDisable.cs
// Created:      2023-10-21 (9:18 AM)

using System;

namespace SharedCode.TreeClasses;

public class NodeEnableDisable
{
	public NodeEnableDisable(ITree tree, Predicate<ITreeNode> predicate)
	{
		Tree = tree;
		this.Predicate = predicate;
	}

	public ITree Tree { get; set; }

	public bool Apply()
	{
		try
		{
			foreach (ITreeNode node in Tree.EnumAllNodes())
			{
				if (Predicate(node))
				{
					node.IsEnabled = true;
				}
				else
				{
					node.IsEnabled = false;
				}
			}
		}
		catch
		{
			return false;
		}

		return true;
	}

	public Predicate<ITreeNode> Predicate { get; set; }
}