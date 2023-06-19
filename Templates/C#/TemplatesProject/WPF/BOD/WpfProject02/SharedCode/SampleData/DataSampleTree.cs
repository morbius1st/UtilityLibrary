// Solution:     WpfProject02
// Project:       WpfProject02
// File:             DataSampleTree.cs
// Created:      2023-06-14 (8:14 PM)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using SharedCode.ShDebugAssist;

using SharedWPF.ShWin;

namespace SharedCode.SampleData;

public class DataSampleTree
{
#region private fields

	private Tree tree;

	private TreeNode treeNode;

	private int nodeIdx = -1;

	private List<string> data;

	private  ShDebugMessages M;

#endregion

#region ctor

	public DataSampleTree()
	{
		init();
	}

#endregion

#region public properties

	public Tree Tree => tree;

#endregion

#region private properties

#endregion

#region public methods

	public void TestAddNode()
	{
		TestAddNode(new List<string>() { "Alpha", "Beta", "Delta" });
	}

	public void TestAddNode(List<string> nodePath)
	{
		treeNode = makeTempTreeNode();

		tree.AddNode(treeNode, nodePath);
	}

	public int TestEnumerator()
	{
		AddNodesAndLeaves();

		return TestShowTreeNodes();
	}

	public int TestShowTreeNodesAndLeaves()
	{
		int result = 0;


		Examples.M.WriteLine("\nShow Nodes and Leaves\n");

		foreach (TreeNode tcn in tree)
			// foreach (TreeClassNode<TreeNodeData, TreeLeafData> tcn in tree)
		{
			result++;

			Examples.M.WriteLine(ShowNodeParents(tcn));

			// Debug.WriteLine();

			ShowNodeLeaves(tcn);
		}

		Examples.M.WriteLine("\n** done **\n\n");

		return result;
	}

	public int TestShowTreeNodes()
	{
		int result = 0;

		foreach (TreeNode tcn in tree)
		{
			result++;

			Debug.WriteLine(ShowNodeParents(tcn));
		}

		return result;
	}
	//
	// public void AddTestNodes()
	// {
	// 	TestAddNode(new List<string>() { "Alpha", "Beta", "Delta" });	// 1-1-1			// 0
	// 	TestAddNode(new List<string>() { "Alpha", "Beta", "Gamma" });	// 1-1-2			// 1
	//
	// 	TestAddNode(new List<string>(){ "Alpha", "Zeta", "Eta" });			// 1-2-1		// 2
	// 	TestAddNode(new List<string>() { "Alpha", "Zeta", "Theta", "Iota" });	// 1-2-2-1	// 3
	// 	TestAddNode(new List<string>() { "Alpha", "Zeta", "Theta", "Kappa" });	// 1-2-2-2	// 4
	// 	TestAddNode(new List<string>() { "Sigma", "Lambda", "Nu" });		// 2-1-1		// 5
	// 	TestAddNode(new List<string>() { "Sigma", "Lambda", "Nu", "Xi" });	// 2-1-1-1		// 6
	// 	TestAddNode(new List<string>() { "Sigma", "Lambda", "Nu", "Pi" });	// 2-1-1-2		// 7
	// 	TestAddNode(new List<string>() { "Sigma", "Omega", "Mu" });	// 2-2-1				// 8
	// 	TestAddNode(new List<string>() { "Tau", "Phi", "Rho" });	// 3-1-1				// 9
	// 	TestAddNode(new List<string>() { "Tau", "Phi", "Chi" });	// 3-1-2				// 10
	// }

	public void AddNodesAndLeaves()
	{
		TreeNode node;
		TreeLeaf leaf;

		List<string> nodePath;

		KeyValuePair<string, string[]>[][] nodesAndLeaves = AddTestNodes2();

		foreach (KeyValuePair<string, string[]>[] kvps in nodesAndLeaves)
		{
			// showNodePath(kvps);

			if (kvps == null) break;

			nodePath = new List<string>(5);

			for (int i = 0; i < kvps.Length; i++)
			{
				// key = node (add if needed)
				// value = leaves (add if exists)

				KeyValuePair<string, string[]> kvp = kvps[i];

				bool result = tree.ContainsNode(kvp.Key);

				if (!result)
				{
					node = new TreeNode(kvp.Key, null);

					// showNodePath(node.NodeName, nodePath);

					tree.AddNode(node, nodePath);
				}

				nodePath.Add(kvp.Key);

				if (kvp.Value != null)
				{
					foreach (string leafName in kvp.Value)
					{
						leaf = new TreeLeaf(leafName, null);
						Tree.AddLeaf(leaf);
					}
				}
			}
		}
	}

	public void TestCountNodesAndLeaves()
	{
		AddNodesAndLeaves();


		if (tree.ContainsNode(data[9]))
		{
			tree.SelectedNode = tree.CurrentNode;
		}

		tree.ContainsNode(data[16]);


		M.Write("\nNode Counts\n");

		M.WriteLine("Count Nodes   | Root", tree.CountNodesRoot.ToString());
		M.WriteLine("Count Nodes   | Tree", tree.CountNodesTree.ToString());

		M.WriteLine("Current Node  | Name", tree.CurrentNode?.NodeName ?? "null");
		M.WriteLine("Count Nodes   | Current", tree.CountNodesRoot.ToString());
		M.WriteLine("Count Nodes ex| Current", tree.CountNodesCurrentEx.ToString());



		M.WriteLine("Selected Node | Name", tree.SelectedNode?.NodeName ?? "null");
		M.WriteLine("Count Nodes   | Current", tree.CountNodesSelected.ToString());
		M.WriteLine("Count Nodes ex| Current", tree.CountNodesSelectedEx.ToString());

	}

	public KeyValuePair<string, string[]>[][] AddTestNodes2()
	{
		int nodeCount = 19;
		int leafCount = -1;


		// List<string> data = new List<string>()
		// {
		// 	"Alpha",	// 0  (top level)
		// 	"Beta",		// 1
		// 	"Delta",	// 2
		// 	"Gamma",	// 3
		// 	"Zeta",		// 4
		// 	"Eta",		// 5
		// 	"Theta",	// 6
		// 	"Iota",		// 7
		// 	"Kappa",	// 8
		// 	"Sigma",	// 9  (top level)
		// 	"Lambda",	// 10
		// 	"Nu",		// 11
		// 	"Xi",		// 12
		// 	"Pi",		// 13
		// 	"Omega",	// 14
		// 	"Mu",		// 15
		// 	"Tau",		// 16 (top level)
		// 	"Phi",		// 17
		// 	"Rho",		// 18
		// 	"Chi",		// 19
		// };

		KeyValuePair<string, string[]>[][] nodesAndLeaves;


		KeyValuePair<string, string[]>[] kvp = 
			new KeyValuePair<string, string[]>[nodeCount];

		for (int i = 0; i < nodeCount; i++)
		{
			kvp[i] = new KeyValuePair<string, string[]>(data[i], null);
		}

		nodesAndLeaves = new KeyValuePair<string, string[]>[11][];

		nodesAndLeaves[0] = new []
		{
			new KeyValuePair<string, string[]>(
				data[0],
				new []
				{
					$"{data[0]} | Leaf 1-{++leafCount:D3}"
				}
				),
			new KeyValuePair<string, string[]>(
				data[1],
				new []
				{
					$"{data[1]} | Leaf 1-1-{++leafCount:D3}",
					$"{data[1]} | Leaf 1-1-{++leafCount:D3}",
					$"{data[1]} | Leaf 1-1-{++leafCount:D3}"
				}),
			new KeyValuePair<string, string[]>(
				data[2],
				new []
				{
					$"{data[2]} | Leaf 1-1-1-{++leafCount:D3}"
				}
				)
		};


		nodesAndLeaves[1] = new []
		{
			kvp[0],
			kvp[1],
			new KeyValuePair<string, string[]>(
				data[3],
				new []
				{
					$"{data[3]} | Leaf 1-1-2-{++leafCount:D3}",
					$"{data[3]} | Leaf 1-1-2-{++leafCount:D3}"
				}
				)
		};

		nodesAndLeaves[2] = new []
		{
			kvp[0],
			kvp[4],
			new KeyValuePair<string, string[]>(
				data[5],
				new []
				{
					$"{data[5]} | Leaf 1-2-1-{++leafCount:D3}",
					$"{data[5]} | Leaf 1-2-1-{++leafCount:D3}",
					$"{data[5]} | Leaf 1-2-1-{++leafCount:D3}",
					$"{data[5]} | Leaf 1-2-1-{++leafCount:D3}"
				}
				)
		};

		nodesAndLeaves[3] = new []
		{
			kvp[0],
			kvp[4],
			new KeyValuePair<string, string[]>(
				data[6],
				new []
				{
					$"{data[6]} | Leaf 1-2-2-{++leafCount:D3}"
				}
				),
			new KeyValuePair<string, string[]>(
				data[7], 
				new []
				{
					$"{data[7]} | Leaf 1-2-2-1-{++leafCount:D3}",
					$"{data[7]} | Leaf 1-2-2-1-{++leafCount:D3}"
				}
				),
		};

		nodesAndLeaves[4] = new []
		{
			kvp[0],
			kvp[4],
			kvp[6],
			new KeyValuePair<string, string[]>(
				data[8], new []
				{
					$"{data[8]} | Leaf 1-2-2-2-{++leafCount:D3}",
					$"{data[8]} | Leaf 1-2-2-2-{++leafCount:D3}"
				}
				),
		};

		nodesAndLeaves[5] = new []
		{
			new KeyValuePair<string, string[]>(
				data[9], new []
				{
					$"{data[9]} | Leaf 2-{++leafCount:D3}",
				}
				),
			new KeyValuePair<string, string[]>(
				data[10], new []
				{
					$"{data[10]} | Leaf 2-1-{++leafCount:D3}",
					$"{data[10]} | Leaf 2-1-{++leafCount:D3}",
					$"{data[10]} | Leaf 2-1-{++leafCount:D3}",
					$"{data[10]} | Leaf 2-1-{++leafCount:D3}",
				}
				),
			new KeyValuePair<string, string[]>(
				data[11], new []
				{
					$"{data[11]} | Leaf 2-1-1-{++leafCount:D3}"
				}
				),
		};


		nodesAndLeaves[6] = new []
		{
			kvp[9],
			kvp[10],
			kvp[11],
			new KeyValuePair<string, string[]>(
				data[12], new []
				{
					$"{data[12]} | Leaf 2-1-1-1-{++leafCount:D3}"
				}
				),
		};

		nodesAndLeaves[7] = new []
		{
			kvp[9],
			kvp[10],
			kvp[11],
			new KeyValuePair<string, string[]>(
				data[13], new []
				{
					$"{data[13]} | Leaf 2-1-1-2-{++leafCount:D3}",
					$"{data[13]} | Leaf 2-1-1-2-{++leafCount:D3}"
				}
				),
		};

		nodesAndLeaves[8] = new []
		{
			kvp[9],
			kvp[14],
			new KeyValuePair<string, string[]>(
				data[15], new []
				{
					$"{data[15]} | Leaf 2-2-1-{++leafCount:D3}",
					$"{data[15]} | Leaf 2-2-1-{++leafCount:D3}"
				}
				),
		};

		nodesAndLeaves[9] = new []
		{
			kvp[16],
			new KeyValuePair<string, string[]>(
				data[17], new []
				{
					$"{data[17]} | Leaf 3-1-{++leafCount:D3}"
				}
				),
			new KeyValuePair<string, string[]>(
				data[18], new []
				{
					$"{data[18]} | Leaf 3-1-1-{++leafCount:D3}",
					$"{data[18]} | Leaf 3-1-1-{++leafCount:D3}",
					$"{data[18]} | Leaf 3-1-1-{++leafCount:D3}",
					$"{data[18]} | Leaf 3-1-1-{++leafCount:D3}",
					$"{data[18]} | Leaf 3-1-1-{++leafCount:D3}"
				}
				),
		};

		nodesAndLeaves[10] = new []
		{
			kvp[16],
			kvp[17],
			new KeyValuePair<string, string[]>(
				data[19], new []
				{
					$"{data[19]} | Leaf 3-1-2-{++leafCount:D3}"
				}
				),
		};

		return nodesAndLeaves;
	}

#endregion

#region private methods

	private void init()
	{
		tree = new Tree("Test Tree", new TreeNodeData("Test Node Data"));

		M = Examples.M;

		data = new List<string>()
		{
			"Alpha",  // 0
			"Beta",   // 1
			"Delta",  // 2
			"Gamma",  // 3
			"Zeta",   // 4
			"Eta",    // 5
			"Theta",  // 6
			"Iota",   // 7
			"Kappa",  // 8
			"Sigma",  // 9
			"Lambda", // 10
			"Nu",     // 11
			"Xi",     // 12
			"Pi",     // 13
			"Omega",  // 14
			"Mu",     // 15
			"Tau",    // 16
			"Phi",    // 17
			"Rho",    // 18
			"Chi",    // 19
		};
	}

	private TreeNode makeTempTreeNode()
	{
		return new TreeNode($"Tree Node {++nodeIdx:D5}", new TreeNodeData($"Node Data {nodeIdx:D5}"));
	}

	private void ShowNodeLeaves(TreeNode node)
	{
		if (node == null || !node.HasLeaves)
		{
			// Debug.WriteLine($"\t** no leaves **");
			return;
		}

		foreach (KeyValuePair<string, TreeLeaf> kvp
				in node.Leaves)
		{
			Examples.M.WriteLine($"\t{kvp.Value.LeafName}");
			// Debug.WriteLine();
		}
	}

	private string ShowNodeParents(TreeNode tcn)
	{
		List<string> parents = new List<string>();

		TreeNode node = tcn;
		while (node.ParentNode  != null)
		{
			parents.Add(node.NodeName);

			node = node.ParentNode;
		}

		if (parents.Count == 0) return String.Empty;

		StringBuilder sb = new StringBuilder();

		for (int i = parents.Count - 1; i >= 0; i--)
		{
			sb.Append(parents[i]);
			if (i != 0) sb.Append(" > ");
		}

		return sb.ToString();
	}

	private void showNodePath(KeyValuePair<string, string[]>[] kvps)
	{
		Debug.Write("\n\nnode path original| ");

		foreach (KeyValuePair<string, string[]> kvp in kvps)
		{
			Debug.Write($" > {kvp.Key}");
		}

		Debug.Write("\n");
	}

	private void showNodePath(string nodeName, List<string> nodePath)
	{
		Debug.Write("\nnode path @A1| ");
		foreach (string s in nodePath)
		{
			Debug.Write($"> {s} ");
		}

		Debug.Write($" | ({nodeName})");

		Debug.Write("\n");
	}

#endregion

#region event consuming

#endregion

#region event publishing

#endregion

#region system overrides

	public override string ToString()
	{
		return $"this is {nameof(DataSampleTree)}";
	}

#endregion
}