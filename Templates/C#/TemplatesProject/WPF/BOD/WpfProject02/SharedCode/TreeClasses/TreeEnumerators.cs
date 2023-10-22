// Solution:     WpfProject02
// Project:       WpfProject02
// File:             TreeEnumerators.cs
// Created:      2023-10-15 (12:07 PM)

using System.Collections;
using System.Collections.Generic;

namespace SharedCode.TreeClasses;

public class TreeNodeEnumerator<TNd, TLd> : IEnumerator<TreeNode<TNd, TLd>>
	where TNd : class
	where TLd : class
{
	private readonly TreeNode<TNd, TLd> node;
	private List<TreeNode<TNd, TLd>>? currNodes;

	private List<IEnumerator<KeyValuePair<string, TreeNode<TNd, TLd>>>> ies;

	public TreeNodeEnumerator() { }

	public TreeNodeEnumerator(TreeNode<TNd, TLd> node)
	{
		this.node = node;

		Reset();
	}


	public TreeNode<TNd, TLd> Current
	{
		get
		{
			if (currNodes == null) return null;

			return currNodes[^1];
		}
	}


	public List<TreeNode<TNd, TLd>> CurrentPath
	{
		get
		{
			if (currNodes == null) return null;

			return currNodes;
		}
	}


	public bool MoveNext()
	{
		if (ies == null || ies.Count == 0) return false;

		bool result = ies[^1].MoveNext();

		if (result)
		{
			TreeNode<TNd, TLd> node = ies[^1].Current.Value;

			currNodes.Add(node);

			if (node.HasNodes)
			{
				ies.Add(node.Nodes.GetEnumerator());
			}

			return true;
		}
		else
		{
			ies.RemoveAt(ies.Count - 1);

			if (ies.Count == 0) return false;
		}

		return MoveNext();
	}

	public void Reset()
	{
		// if (node.CountNodes == 0)
		// {
		// 	currNodes = null;
		// }
		// else
		// {
		currNodes = new List<TreeNode<TNd, TLd>>();
		ies = new List<IEnumerator<KeyValuePair<string, TreeNode<TNd, TLd>>>>();

		if (!node.IsNodesNull &&
			node.CountNodes != 0)
		{
			currNodes.Add(node.Nodes[0].Value);

			if (!node.IsNodesNull) ies.Add(node.Nodes.GetEnumerator());
		}

		// }
	}

	object IEnumerator.Current => Current;

	public void Dispose() { }
}

public class TreeEnumerator<TNd, TLd> : IEnumerator<TreeNode<TNd, TLd>>
	where TNd : class
	where TLd : class
{
	private readonly Tree<TNd, TLd> tree;
	private List<TreeNode<TNd, TLd>>? currNodes;

	private List<IEnumerator<KeyValuePair<string, TreeNode<TNd, TLd>>>> ies;

	public TreeEnumerator() { }

	public TreeEnumerator(Tree<TNd, TLd> tree)
	{
		this.tree = tree;

		Reset();
	}


	public TreeNode<TNd, TLd> Current
	{
		get
		{
			if (currNodes == null) return null;

			return currNodes[^1];
		}
	}


	public List<TreeNode<TNd, TLd>> CurrentPath
	{
		get
		{
			if (currNodes == null) return null;

			return currNodes;
		}
	}


	public bool MoveNext()
	{
		if (ies == null || ies.Count < 1) { return false; }

		bool result = ies[^1].MoveNext();

		if (result)
		{
			TreeNode<TNd, TLd> node = ies[^1].Current.Value;

			currNodes.Add(node);

			if (node.HasNodes)
			{
				ies.Add(node.Nodes.GetEnumerator());
			}

			return true;
		}
		else
		{
			ies.RemoveAt(ies.Count - 1);

			if (ies.Count == 0) return false;
		}

		return MoveNext();
	}

	public void Reset()
	{
		currNodes = new List<TreeNode<TNd, TLd>>();

		ies = new List<IEnumerator<KeyValuePair<string, TreeNode<TNd, TLd>>>>();

		if (!tree.RootNode.IsNodesNull &&
			tree.CountNodesRoot != 0)
		{
			currNodes.Add(tree.RootNode.Nodes[0].Value);

			ies.Add(tree.RootNode.Nodes.GetEnumerator());
		}
	}

	object IEnumerator.Current => Current;

	public void Dispose() { }
}