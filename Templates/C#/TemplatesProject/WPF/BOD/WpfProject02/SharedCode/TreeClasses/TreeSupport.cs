// Solution:     WpfProject02
// Project:       WpfProject02
// File:             TreeSupport.cs
// Created:      2023-10-14 (5:05 PM)

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SharedCode.TreeClasses;

public static class TreeSupport<TNd, TLd>
	where TNd : class
	where TLd : class
{
	public static void showNodePath(string location, string nodeName, List<string> nodePath)
	{
		Debug.Write($"node path {location}| ");

		foreach (string s in nodePath)
		{
			Debug.Write($"> {s} ");
		}

		Debug.Write($" | ({nodeName})");

		Debug.Write("\n");
	}

	// support methods
	public static string GetNodePath(TreeNode<TNd, TLd> node)
	{
		StringBuilder sb = new StringBuilder();
		TreeNode<TNd, TLd> parent = node.ParentNodeEx;

		List<TreeNode<TNd, TLd>> parentNodes = GetParentNodes(node);

		for (var i = parentNodes.Count - 1; i >= 0; i--)
		{
			sb.Append(parentNodes[i].NodeKey).Append(">");
		}

		sb.Append(node.NodeKey);

		return sb.ToString();
	}

	public static List<TreeNode<TNd, TLd>> GetParentNodes(TreeNode<TNd, TLd> node)
	{
		List<TreeNode<TNd, TLd>> parents = new List<TreeNode<TNd, TLd>>();

		TreeNode<TNd, TLd> parent = node.ParentNodeEx;

		while (parent != null)
		{
			parents.Add(parent);

			parent = parent.ParentNodeEx;
		}

		return parents;
	}
}