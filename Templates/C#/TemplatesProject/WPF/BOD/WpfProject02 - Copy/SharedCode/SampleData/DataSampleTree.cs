﻿// Solution:     WpfProject02
// Project:       WpfProject02
// File:             DataSampleTree.cs
// Created:      2023-06-14 (8:14 PM)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using SharedCode.ShDebugAssist;
using SharedWPF.ShWin;
using static SharedCode.SampleData.Selection;
using static SharedCode.SampleData.Selection.SelectMode;
using static SharedCode.SampleData.Selection.SelectClass;

namespace SharedCode.SampleData;

public class DataSampleTree : INotifyPropertyChanged
{
	private const string TREETITLE = "TREE CLASS";

#region private fields

	private Tree tree;

	private TreeNode treeNode;

	private int nodeIdx = 0;

	private List<string> data;

	private Examples e;

	private  ShDebugMessages M;

	// private bool treeCreated;

	// private KeyValuePair<string, SelectMode> selectedMode;

	private KeyValuePair<SelectMode, SelModeData> selectedMode;


	// private List<TreeLeaf> searchLeaf= new List<TreeLeaf>();
	private int leafSearchCounter = 0;
	private int leafSearchCount = 0;
	private string selectedNodeKey;
	private string deSelectedNodeKey;
	private const string leafSearchString = "Search Leaf";

#endregion

#region ctor

	public DataSampleTree()
	{
		init();
	}

#endregion

#region public properties

	public Tree Tree => tree;

	public Examples E
	{
		get => e;
		set { e = value; }
	}





	public KeyValuePair<SelectMode, SelModeData> SelectedMode
	{
		get => selectedMode;
		set
		{
			selectedMode = value;
			OnPropertyChanged();
		}
	}

	public Dictionary<SelectMode, SelModeData> SelectionModes => Selection.SelModes;

	public bool TreeCreated
	{
		get => Tree != null;
	}

	public bool CanMultiSelect
	{
		get
		{
			bool result = ((Tree?.SelectClass ?? (int) SelectClass.NODE_ONLY)) >= (int) SelectClass.NODE_EXTENDED;
			return result;
		}
	}

	public string SelectModeName
	{
		get { return Selection.SelModes[Tree?.SelectMode ?? SelectMode.INDIVIDUAL].Description; }
	}

	public string SelectedClass 
	{ get
		{
			if (tree == null) return "not selected";
			return $"{Tree.SelectClass.ToString()}";
		}
	}

	public string TreeTitle => $"{TREETITLE} ({Tree?.SelectMode ?? SelectMode.INDIVIDUAL})";

	public string SelectedNodeKey
	{
		get => selectedNodeKey;
		private set
		{
			selectedNodeKey = value;
			OnPropertyChanged();


		}
	}

	public string DeSelectedNodeKey
	{
		get => deSelectedNodeKey;
		private set
		{
			deSelectedNodeKey = value;
			OnPropertyChanged();
		}
	}

	public string SelNodeKey
	{
		get
		{
			if (Tree?.SelectedNodes == null ||
				Tree.SelectedNodes.Count == 0
				) return "not selected";

			return Tree.SelectedNodes[0].NodeKey;
		}
	}

	public string PriorSelNodeKey
	{
		get
		{
			if (Tree?.PriorSelectedNodes == null ||
				Tree.PriorSelectedNodes.Count == 0
				) return "not selected";

			return Tree.PriorSelectedNodes[0].NodeKey;
		}
	}


#endregion

#region private properties

#endregion

#region public methods

	// selection

	public void SelectTree()
	{
		M.WriteLineCodeMap("Enter Method");

		M.WriteLineMargin($"select tree");
	}

	public void DeSelectTree()
	{
		M.WriteLineCodeMap("Enter Method");

		M.WriteLineMargin($"deselect tree");
	}


	// common

	public void AddNodesAndLeaves()
	{
		M.WriteLineMargin($"adding nodes and leaves");

		int idx = 0;

		nodeIdx = 0;

		TreeNode? priorNode;

		TreeNode? node = null;
		TreeLeaf? leaf;

		List<string> nodePath;

		KeyValuePair<string, string[]>[][] nodesAndLeaves = AddTestNodes2();

		foreach (KeyValuePair<string, string[]>[] kvps in nodesAndLeaves)
		{
			priorNode = tree.RootNode;

			if (kvps == null) break;


			// Debug.WriteLine("\n\nadding a branch");
			//
			// Debug.Write($"node idx| {idx++}| \t"); 
			//
			// showNodePath(kvps);
			// Debug.Write("\n"); 


			nodePath = new List<string>(5);

			for (int i = 0; i < kvps.Length; i++)
			{
				// key = node (add if needed)
				// value = leaves (add if exists)

				KeyValuePair<string, string[]> kvp = kvps[i];

				// Debug.WriteLine($"\tprior node   | {priorNode.NodeKey}| count| {priorNode.Nodes?.Count.ToString() ?? "is null" }");

				// bool result = tree.GetMatchingNodes(kvp.Key, priorNode) > 0;

				// TreeNode t;
				bool result = tree.ContainsNode(kvp.Key, out  priorNode, priorNode);
				// bool result3 = tree.ContainsNode(kvp.Key, out  t, null, 1, true);

				// Debug.WriteLine($"\ttry add node | {kvp.Key}| already contains?| n/a vs {result} vs n/a");

				// not found?
				if (!result)
				{
					// yep - not found
					// node = new TreeNode(kvp.Key, null);
					node = makeTempTreeNode(kvp.Key);
					node.IsExpanded = true;

					// Debug.WriteLine($"\tadding node  | {node.NodeKey}");
					//
					//
					// Debug.Write("\t\t");
					// showNodePath(node.NodeKey, nodePath);
					//
					// Debug.WriteLine($"\tnode count| {node.Nodes?.Count.ToString() ?? "is null"}");
					//
					// showNodePath(node.NodeKey, nodePath);


					// AddNode sets currentNode
					if (!tree.AddNode(node, nodePath))
					{
						// Debug.WriteLine($"\tadd FAILED");

						M.WriteLineMargin($"*** Error *** | failed to add node| {node.NodeKey} (breaking)");
						break;
					}

					priorNode = tree.CurrentNode;
				}

				// Debug.WriteLine($"\tproceeding");
				// priorNode = tree.CurrentNode;
				// priorNode = t;


				// Debug.WriteLine($"\tsetting prior node to| {priorNode?.NodeKey ?? "is null"}");

				nodePath.Add(kvp.Key);

				if (kvp.Value != null)
				{
					foreach (string leafName in kvp.Value)
					{
						leaf = new TreeLeaf(leafName, null, node);

						Tree.AddLeaf(leaf);

						if (leafSearchCounter++ == 7)
						{
							leaf = new TreeLeaf(leafSearchString, null, node);
							Tree.AddLeaf(leaf);
							leafSearchCounter = 0;
							leafSearchCount++;
						}
					}
				}
			}
			// Debug.WriteLine($"\tnext loop");
		}

		// M.WriteLineMargin($"leaf search duplicate leaves| {leafSearchCount.ToString()}");

		M.WriteLineMargin($"DONE");

		M.WriteLineStatus("nodes and leaves added");
	}

	public void TestCountNodesAndLeaves()
	{
		// counting
		// nodes / tree   (root)
		// nodes / branch (root) / (current) / (selected) <-tree level / -> node level random
		// nodes / node   (root) / (current) / (selected)  <-tree level / -> node level random


		AddNodesAndLeaves();
		testCountNodes();
	}

	public void TreeClear()
	{
		tree.Clear();
		UpdateTreeProps();
	}

	// node

	public bool TestAddNode()
	{
		string name = "Proton";

		bool result = testAddNode(new List<string>() { "Alpha", "Beta", "Delta" }, name);

		return result;
	}

	public int TestEnumerator1()
	{
		M.WriteLineCodeMap("Enter Method");

		return ShowTreeNodes();
	}

	// search tree for node of a specific name
	// tell if found and list its nodes
	public int TestEnumerator2()
	{
		M.WriteLineCodeMap("Enter Method");

		M.Write($"looking for| {data[10]}");

		bool result = tree.ContainsNode(data[10]); // lambda

		if (!result)
		{
			M.WriteLine(" | not found");
			return 0;
		}

		M.WriteLine($"  | found");

		int count = 0;

		return ShowNodesAndMaybeLeaves(false);
	}

	public void TestFindNodesAlt()
	{
		bool result;
		TreeNode node;

		M.WriteLineCodeMap("Enter Method");

		M.WriteLine($"looking for| {data[10]} | in the whole tree| ");

		// search from root node since startAtCurrNode will be false
		result = tree.ContainsNode(data[10]); // lambda

		if (!result)
		{
			M.WriteLine("** FAILED **");
			return;
		}
		else
		{
			M.WriteLine($"\t** WORKED ** | nodeData/Value {tree.CurrentNode.NodeData.Value}");
		}

		// save the current node;
		node = tree.CurrentNode;


		M.WriteLine($"looking for| {data[21]} | starting from CurrentNode | ");

		// currentnode is (data[10] // Lambda
		// find node starting from "currentNode"
		// result = tree.ContainsNode(data[21], true); // Omicron
		result = tree.ContainsNode(data[21]); // Omicron

		if (!result)
		{
			M.WriteLine("** FAILED **");
			return;
		}
		else
		{
			M.WriteLine($"\t** WORKED ** | nodeData/Value {tree.CurrentNode.NodeData.Value}");
		}


		M.WriteLine($"looking for| {data[22]} | starting from \"node\" via tree| ");

		result = tree.GetMatchingNodes(data[22], node) > 0; // Upsilon

		if (!result)
		{
			M.WriteLine("** FAILED **");
			return;
		}
		else
		{
			M.WriteLine($"\t** WORKED ** | nodeData/Value {tree.CurrentNode.NodeData.Value}");
		}


		M.WriteLine($"looking for| {data[22]} | starting from \"node\" via \"node\"| ");

		result = node.ContainsNode(data[22], out node); // Upsilon

		if (!result)
		{
			M.WriteLine("** FAILED **");
			return;
		}
		else
		{
			M.WriteLine($"\t** WORKED ** | nodeData/Value {node.NodeData.Value}");

			M.WriteLine("\nnode path");
			M.WriteLine(TreeClassEx.GetNodePath(node));
		}


		M.WriteLine($"search for node| {data[18]} when duplicates");

		result = tree.ContainsNode(data[18], out node); // Upsilon

		if (!result)
		{
			M.WriteLine("** FAILED **");
			return;
		}
		else
		{
			M.WriteLine($"\t** WORKED ** | nodeData/Value {node.NodeData.Value}");

			M.WriteLine("\nnode path");
			M.WriteLine(TreeClassEx.GetNodePath(node));
		}
	}

	public void TestFindNodes()
	{
		bool result;
		TreeNode node;
		string bogusNode = "Bogus Node Name";

		M.WriteLineCodeMap("Enter Method");

		M.WriteLine($"search for nodes");


		M.WriteLine($"search A| for node| {bogusNode} | find first (nth = 1) (no such) ** should fail** ");
		result = tree.ContainsNode(bogusNode, out node); // Rho
		showFindNodeResults(result, node);

		M.WriteLine($"search B| for node|{data[19]} | find first (and only) (nth = 1) (this is the last node( (ex dups)");
		result = tree.ContainsNode(data[19], out node); // Rho
		showFindNodeResults(result, node);


		M.WriteLine($"search Z| for node| {data[10]} | find first (and maybe only) (nth = 1)");
		result = tree.ContainsNode(data[10], out node); // Rho
		showFindNodeResults(result, node);

		// node now points to a valid node


		M.WriteLine($"search for node when duplicates exist");

		M.WriteLine($"search 0| for node| {data[18]} | find first (nth = 1)| but start from {node.NodeKey}");
		result = tree.ContainsNode(data[18], out node, node, 1); // Rho
		showFindNodeResults(result, node);


		M.WriteLine($"search 1| for node| {data[18]} | find first (nth = 1)");
		result = tree.ContainsNode(data[18], out node, null, 1); // Rho
		showFindNodeResults(result, node);

		M.WriteLine($"search 2| for node| {data[18]} | find second (nth = 2)");
		result = tree.ContainsNode(data[18], out node, null, 2); // Rho
		showFindNodeResults(result, node);

		M.WriteLine($"search 3| for node| {data[18]} | find eight (nth = 8) (fail is correct) ");
		result = tree.ContainsNode(data[18], out node, null, 8); // Rho
		showFindNodeResults(result, node);

		M.WriteLine($"search 4| for node| {data[18]} | find last (nth = 0)");
		result = tree.ContainsNode(data[18], out node, null, 0); // Rho
		showFindNodeResults(result, node);

		M.WriteLine($"search 5| for node| {data[18]} | find next (nth = -1)");

		result = true;

		while (result)
		{
			result = tree.ContainsNode(data[18], out node, null, -1, false ); // Rho
			showFindNodeResults(result, node);
		}

		M.WriteLine($"search 6| for node| {data[18]} | find next (second time to verify reset) (nth = -1)");

		result = true;

		while (result)
		{
			result = tree.ContainsNode(data[18], out node, null, -1, false); // Rho
			showFindNodeResults(result, node);
		}

		M.WriteLine($"search 7| node with reset - first two matches");

		M.WriteLine($"search 7| for node| {data[18]} | find next (first) (nth = -1)");
		result = tree.ContainsNode(data[18], out node, null, -1); // Rho
		showFindNodeResults(result, node);

		M.WriteLine($"search 7| for node| {data[18]} | find next (second) (nth = -1)");
		result = tree.ContainsNode(data[18], out node, null, -1); // Rho
		showFindNodeResults(result, node);


		M.WriteLine($"search 7| for node| {data[19]} | find next (first) (nth = -1)");
		result = tree.ContainsNode(data[19], out node, null, -1); // Rho
		showFindNodeResults(result, node);
	}

	public int TestCountNodeMatches()
	{
		M.WriteLineCodeMap("Enter Method");

		// count (and find) nodes == rho
		M.WriteLineMargin($"search for node with name| {data[18]}");

		int result = tree.GetMatchingNodes(data[18]);

		if (result > 0)
		{
			M.WriteLineMargin($"*** WORKED *** | {result}");
		}
		else
		{
			M.WriteLineMargin($"*** FAILED *** | {result}");
		}

		return result;
	}

	public void RenameNode()
	{
		bool result;

		M.WriteLineCodeMap("Enter Method");

		M.WriteLine($"rename with key name| {data[11]}");
		M.WriteLine($"to this key name    | {data[24]}");

		result = tree.ContainsNode(data[11]);

		if (!result)
		{
			M.WriteLine($"*** FAILED ***");
			return;
		}

		result = tree.RenameNode(tree.CurrentNode, data[24]);

		if (!result)
		{
			M.WriteLine($"*** FAILED ***");
			return;
		}

		M.WriteLine($"*** WORKED ***");
	}

	public void TestMoveNode()
	{
		bool result;
		TreeNode sourceNode;
		TreeNode destinationNode;

		M.WriteLineCodeMap("Enter Method");

		// move Nu
		M.WriteLine($"move node with key name| {data[11]}");
		// phi
		M.Write($"to node with key| {data[17]}| ");

		result = tree.ContainsNode(data[11]);
		if (!result)
		{
			M.WriteLine($"\t** FAILED ** ");
			return;
		}

		sourceNode = tree.CurrentNode;

		result = tree.ContainsNode(data[17]);
		if (!result)
		{
			M.WriteLine($"\t** FAILED ** ");
			return;
		}

		destinationNode = tree.CurrentNode;

		M.WriteLine($"move node| {sourceNode.NodeKey} to {destinationNode.NodeKey}");

		result = tree.MoveNode(sourceNode, destinationNode);

		if (result)
		{
			M.WriteLine($"\t** WORKED ** - node moved");
			M.WriteLine($"** verify **| {destinationNode.Nodes.ContainsKey(sourceNode.NodeKey)}");
		}
		else
		{
			M.WriteLine($"\t** FAILED ** ");
		}
	}

	public void TestDeleteNode()
	{
		bool result;
		TreeNode node;


		M.WriteLineCodeMap("Enter Method");

		M.WriteLineMargin($"delete node with key name| {data[11]} | method 1");

		// find parent node / set currentNode
		result = tree.ContainsNode(data[11]);
		if (!result)
		{
			M.WriteLineMargin($"\t** FAILED ** ");
			return;
		}

		result = tree.DeleteNode(tree.FoundNode);

		if (result)
		{
			M.WriteLineMargin($"\t** WORKED 1 ** | {tree.FoundNode.NodeKey} deleted");
		}
		else
		{
			M.WriteLineMargin($"\t** FAILED ** ");
		}


		M.WriteLineMargin($"delete node with key name| {data[23]} | method 2");

		List<string> nodePath = new List<string>
		{
			data[9], data[10], data[23]
		};

		result = tree.DeleteNode(nodePath, out node);

		if (result)
		{
			M.WriteLineMargin($"\t** WORKED 2 ** | {node.NodeKey} deleted");
		}
		else
		{
			M.WriteLineMargin($"\t** FAILED ** ");
		}
	}


	// leaf

	public int TestCountLeafMatches()
	{
		M.WriteLineMargin($"search for leaf with name| {leafSearchString}");

		int result = 0;

		result = tree.CountMatchingLeaves(leafSearchString, null);

		if (result > 0)
		{
			M.WriteLineMargin($"*** WORKED *** | {result}");
		}
		else
		{
			M.WriteLineMargin($"*** FAILED *** | {result}");
		}

		return result;
	}

	public void TestCountLeafMatches2()
	{
		M.WriteLineCodeMap("Enter Method");

		int result = 0;

		TestCountLeafMatches();

		string find = "bogus leaf key";

		M.WriteLineMargin($"search for leaf with name| {find}");

		result = tree.CountMatchingLeaves(find, null);

		if (result > 0)
		{
			M.WriteLineMargin($"*** FAILED *** | {result}");
		}
		else
		{
			M.WriteLineMargin($"*** WORKED *** | {result}");
		}


		TreeNode[] tn = new TreeNode[3];

		if (!tree.ContainsNode(data[0])) return;

		tn[0] = tree.CurrentNode;

		if (!tree.ContainsNode(data[9])) return;

		tn[1] = tree.CurrentNode;


		if (!tree.ContainsNode(data[16])) return;

		tn[2] = tree.CurrentNode;

		M.WriteLineMargin("got notes to start with");

		testCountLeafMatches(tn[0]);
		testCountLeafMatches(tn[1]);
		testCountLeafMatches(tn[2]);
	}

	public void TestDeleteLeaf()
	{
		bool result;
		TreeNode node;
		TreeLeaf found;

		M.WriteLineCodeMap("Entering Method");

		M.WriteLine($"delete leaf");
		M.WriteLine($"search for 2nd leaf with name| {leafSearchString}");

		result =  tree.ContainsLeaf(leafSearchString, out found, null, 2);

		if (!result)
		{
			M.WriteLine($"*** FAILED *** leaf not found");
			return;
		}

		M.Write($"*** FOUND ***");

		node = (TreeNode) found.ParentNode;

		M.WriteLine($"node path | {ShowNodeParents(node)}");

		ShowNodeLeaves(node);

		result = tree.DeleteLeaf(node, found);

		if (!result)
		{
			M.WriteLine($"*** FAILED *** leaf not deleted");
			return;
		}

		M.Write($"*** WORKED ***");

		ShowNodeLeaves(node);
	}

	public void TestFindLeaf()
	{
		bool result;
		TreeLeaf found;

		M.WriteLineCodeMap("Enter Method");

		M.WriteLine($"search for leaf with name| {leafSearchString}");


		M.WriteLine($"search type 0| next occurrence");

		result = true;
		int count = 0;

		while (result)
		{
			result = tree.ContainsLeaf(leafSearchString, out found, null, -1);

			if (result)
			{
				M.Write($"*** FOUND *** # {count++}| ");
				M.WriteLine($"key found| {found.LeafKey}| in node| {found.ParentNode.NodeKey}");
			}
			else
			{
				M.WriteLine($"*** FAILED ***");
			}
		}

		M.WriteLine($"search type 1| first occurrence");

		result = tree.ContainsLeaf(leafSearchString, out found);

		if (result)
		{
			M.WriteLine($"*** FOUND ***");
			M.WriteLine($"key found| {found.LeafKey}| in node| {found.ParentNode.NodeKey}");
		}
		else
		{
			M.WriteLine($"*** FAILED ***");
		}


		M.WriteLine($"search type 2| second occurrence");

		result = tree.ContainsLeaf(leafSearchString, out found, null, 2);

		if (result)
		{
			M.WriteLine($"*** FOUND ***");
			M.WriteLine($"key found| {found.LeafKey}| in node| {found.ParentNode.NodeKey}");
		}
		else
		{
			M.WriteLine($"*** FAILED ***");
		}


		M.WriteLine($"search type 3| 8th occurrence (does not exist)");

		result = tree.ContainsLeaf(leafSearchString, out found, null, 8);

		if (result)
		{
			M.WriteLine($"*** FOUND (which is a fail) ***");
			M.WriteLine($"key found| {found.LeafKey}| in node| {found.ParentNode.NodeKey}");
		}
		else
		{
			M.WriteLine($"*** FAILED (which is good) ***");
		}


		M.WriteLine($"search type 4| last occurrence");

		result = tree.ContainsLeaf(leafSearchString, out found, null, 0);

		if (result)
		{
			M.WriteLine($"*** FOUND ***");
			M.WriteLine($"key found| {found.LeafKey}| in node| {found.ParentNode.NodeKey}");
		}
		else
		{
			M.WriteLine($"*** FAILED ***");
		}


		M.WriteLine($"search type 5| next occurrence");

		result = true;
		count = 0;

		while (result)
		{
			result = tree.ContainsLeaf(leafSearchString, out found, null, -1);

			if (result)
			{
				M.Write($"*** FOUND *** # {count++}| ");
				M.WriteLine($"key found| {found.LeafKey}| in node| {found.ParentNode.NodeKey}");
			}
			else
			{
				M.WriteLine($"*** FAILED ***");
			}
		}

		M.WriteLine($"search type 6| next occurrence 2nd round");

		result = true;
		count = 0;

		while (result)
		{
			result = tree.ContainsLeaf(leafSearchString, out found, null, -1);

			if (result)
			{
				M.Write($"*** FOUND *** # {count++}| ");
				M.WriteLine($"key found| {found.LeafKey}| in node| {found.ParentNode.NodeKey}");
			}
			else
			{
				M.WriteLine($"*** FAILED ***");
			}
		}


		M.WriteLine($"search type 6| next occurrence with reset");

		result = true;
		count = 0;

		M.WriteLine("get the first two matches");

		result = tree.ContainsLeaf(leafSearchString, out found, null, -1);

		showFindResults(found, result, $"*** FOUND *** # {count++}| ");


		result = tree.ContainsLeaf(leafSearchString, out found, null, -1);

		showFindResults(found, result, $"*** FOUND *** # {count++}| ");


		M.WriteLine("reset| get the first two matches");

		result = tree.ContainsLeaf(null, out found, null, -1);


		result = tree.ContainsLeaf(leafSearchString, out found, null, -1);

		showFindResults(found, result, $"*** FOUND *** # {count++}| ");


		result = tree.ContainsLeaf(leafSearchString, out found, null, -1);

		showFindResults(found, result, $"*** FOUND *** # {count++}| ");
	}

	public void TestMoveLeaf()
	{
		bool result;
		TreeNode sourceNode;
		TreeNode destinationNode;

		M.WriteLineCodeMap("Enter Method");

		M.WriteLineMargin($"move leaf");

		M.WriteLineMargin($"finding leaf| {leafSearchString} ");

		result = true;
		TreeLeaf found;

		result = tree.ContainsLeaf(leafSearchString, out found, null, -1);

		if (!result)
		{
			M.WriteLineMargin($"*** FAIL *** | not found");
			return;
		}

		sourceNode = (TreeNode) found.ParentNode;

		// source node: Nu
		M.WriteLineMargin($"from node| {sourceNode.NodeKey}");
		// destination node: phi
		M.WriteLineMargin($"to node| {data[17]}");
		M.WriteMargin($"finding node| ");

		// sets foundnode
		result = tree.ContainsNode(data[17]);

		if (result)
		{
			M.WriteLineMargin($"*** WORKED| found ***");
		}
		else
		{
			M.WriteLineMargin($"*** FAIL *** | not found");
			return;
		}

		destinationNode = tree.FoundNode;
		;

		result = tree.MoveLeaf(sourceNode, destinationNode, leafSearchString);

		M.WriteMargin($"move the leaf between the nodes| ");

		if (result)
		{
			M.WriteLineMargin($"*** WORKED ***");
		}
		else
		{
			M.WriteLineMargin($"*** FAIL ***");
		}

		// backcheck

		// save the "moved" leaf
		found = tree.FoundLeaf;

		result = destinationNode.ContainsLeaf(leafSearchString);

		M.WriteMargin($"search for the leaf in the destination node| ");

		if (result)
		{
			M.WriteLineMargin($"*** WORKED ***");
		}
		else
		{
			M.WriteLineMargin($"*** FAIL ***");
		}

		// test the leaf moved versus a leaf found
		result = tree.FoundLeaf.LeafKey.Equals(found.LeafKey);

		M.WriteMargin($"compare leaves - must match / work| ");

		if (result)
		{
			M.WriteLineMargin($"*** WORKED ***");
		}
		else
		{
			M.WriteLineMargin($"*** FAIL ***");
		}
	}


	// show methods

	public int ShowNodesAndMaybeLeaves(bool showLeaves = true)
	{
		int result = 0;


		M.WriteLineMargin("Show Nodes and Leaves");

		foreach (TreeNode tcn in tree)
			// foreach (TreeClassNode<TreeNodeData, TreeLeafData> tcn in tree)
		{
			result++;
			showNodesAndMaybeLeaves(tcn, showLeaves);
		}

		M.WriteLineMargin("** done **");

		return result;
	}

	public int ShowNodesAndMaybeLeaves(TreeNode start, bool showLeaves = true)
	{
		int result = 0;

		M.WriteLineMargin($"Show Nodes and Leaves| start at| {start.NodeKey}");

		foreach (TreeNode tcn in start)
			// foreach (TreeClassNode<TreeNodeData, TreeLeafData> tcn in tree)
		{
			result++;
			showNodesAndMaybeLeaves(tcn, showLeaves);
		}

		M.WriteLineMargin("** done **");

		return result;
	}

	public void ShowAllLeaves()
	{
		foreach (TreeLeaf leaf in tree.GetLeaves())
		{
			M.WriteLine("Leaf|", $"parent| {leaf.ParentNode.NodeKey} | leaf| {leaf.LeafKey}");
		}
	}


	// test all

	public void TestAll()
	{
		int a;
		bool result;
		string name = "Proton";

		M.WriteLine("\nTest All\n");
		M.Write("TestAll| CLEAR tree | ->");
		Tree.Clear();
		M.WriteLine("DONE");

		M.WriteLine("TestAll| LOAD tree | ->");
		M.MarginUp();
		AddNodesAndLeaves();
		M.MarginDn();
		M.WriteLine("DONE");

		e.UpdateObservTree();


		M.WriteLine("TestAll| tree STATUS | ->");
		M.MarginUp();
		treeStatus();
		M.MarginDn();
		M.WriteLine("DONE");

		M.WriteLine("TestAll| NODE COUNTS | ->");
		M.MarginUp();
		testCountNodes();
		M.MarginDn();
		M.WriteLine("DONE");


		M.WriteLine("TestAll| NODE MATCH COUNT | ->");
		M.MarginUp();
		a = TestCountNodeMatches();
		M.WriteLine($"nodes found| {a}");
		M.MarginDn();
		M.WriteLine("DONE");

		M.WriteLine("TestAll| LEAF COUNTS | ->");
		M.MarginUp();
		testCountLeaves();
		M.MarginDn();
		M.WriteLine("DONE");

		M.WriteLine("TestAll| LEAF MATCH COUNT | ->");
		M.MarginUp();
		a = TestCountLeafMatches();
		M.WriteLine($"leaves found| {a}");
		M.MarginDn();
		M.WriteLine("DONE");

		M.WriteLine("TestAll| enumerate tree nodes | all| ->");
		M.MarginUp();
		a = ShowNodesAndMaybeLeaves(false);
		M.WriteLine($"nodes found| {a}");
		M.MarginDn();
		M.WriteLine("DONE");

		M.WriteLine("TestAll| enumerate tree nodes | node down|->");
		M.MarginUp();
		tree.ContainsNode(data[0]);
		a = ShowNodesAndMaybeLeaves(tree.FoundNode,	false);
		M.WriteLine($"nodes found| {a}");
		M.MarginDn();
		M.WriteLine("DONE");


		M.WriteLine("TestAll| NODE OPERATIONS | ->");
		M.WriteLine("TestAll| ADD node | ->");

		M.MarginUp();
		result = testAddNode(new List<string>() { "Alpha", "Beta", "Delta" }, name);
		M.WriteLine($"TestAll| ADD node | \"Alpha\">\"Beta\">\"Delta\"> ** {name} ** ->|");

		if (result)
		{
			M.WriteLineMargin($"\t** WORKED** | {name} added");
		}
		else
		{
			M.WriteLineMargin($"\t** FAILED ** ");
		}

		TestDeleteNode();

		a = ShowNodesAndMaybeLeaves(false);
		M.WriteLine($"nodes found| {a}");
		M.MarginDn();
		M.WriteLine("DONE");

		e.UpdateObservTree();
	}

	private void treeStatus()
	{
		if (tree == null || tree.RootNode == null)
		{
			M.WriteLine($"TreeStat| tree null");
			return;
		}

		M.WriteLineMargin($"TreeStat| Name", $"{tree.TreeName}");
		M.WriteLineMargin($"TreeStat| current node| key", $"{tree.CurrentNode.NodeKey}");

		M.WriteLineMargin($"TreeStat| tree selection mode", $"{tree.SelectMode}");
		M.WriteLineMargin($"TreeStat| require unique keys?", $"{tree.RequireUniqueKeys}");
		// M.WriteLineMargin($"TreeStat| max depth", $"{tree.MaxDepth}");

		M.WriteLine($"TreeStat| NODE specific | ");

		nodeStatus(tree.RootNode);
	}

	private void nodeStatus(TreeNode tn)
	{
		M.WriteLineMargin($"TreeStat| node key", $"{tn.NodeKey}");
		M.WriteLineMargin($"TreeStat| is node null", $"{tn.IsNodesNull}");
		M.WriteLineMargin($"TreeStat| is chosen", $"{tn.IsChosen}");
		M.WriteLineMargin($"TreeStat| is expanded", $"{tn.IsExpanded}");
		M.WriteLineMargin($"TreeStat| is leaves null", $"{tn.IsLeavesNull}");
		M.WriteLineMargin($"TreeStat| is selected", $"{tn.IsChecked}");
		M.WriteLineMargin($"TreeStat| has nodes", $"{tn.HasNodes}");
		M.WriteLineMargin($"TreeStat| has leaves", $"{tn.HasLeaves}");
		M.WriteLineMargin($"TreeStat| parent node| key", $"{tn.ParentNode?.NodeKey ?? "null parent"}");
		M.WriteLineMargin($"TreeStat| parent node| tree nm", $"{tn.ParentNode?.Tree.TreeName ?? "null parent"}");
		M.WriteLineMargin($"TreeStat| parent node ex| key", $"{tn.ParentNodeEx?.NodeKey ?? "null parent"}");
		M.WriteLineMargin($"TreeStat| node data| value", $"{tn.NodeData?.Value ?? "null node data"}");
	}

#endregion

#region private methods

	private void UpdateTreeProps()
	{
		OnPropertyChanged(nameof(Tree));
		OnPropertyChanged(nameof(TreeCreated));
		OnPropertyChanged(nameof(TreeTitle));
		OnPropertyChanged(nameof(SelectModeName));
		OnPropertyChanged(nameof(CanMultiSelect));
		OnPropertyChanged(nameof(SelectedClass));
	}


	public void CreateTree()
	{
		tree = new Tree("Test Tree", selectedMode.Key, new TreeNodeData("Test Node Data"));



		UpdateTreeProps();

		tree.NodeSelected += Tree_NodeSelected;
		tree.NodeDeSelected += TreeOnNodeDeSelected;
	}


	private void init()
	{
		M = Examples.M;

		data = new List<string>()
		{
			"Alpha",  // 0  (top level)
			"Beta",   // 1
			"Delta",  // 2
			"Gamma",  // 3
			"Zeta",   // 4
			"Eta",    // 5
			"Theta",  // 6
			"Iota",   // 7
			"Kappa",  // 8
			"Sigma",  // 9  (top level)
			"Lambda", // 10
			"Nu",     // 11
			"Xi",     // 12
			"Pi",     // 13
			"Omega",  // 14
			"Mu",     // 15
			"Tau",    // 16 (top level)
			"Phi",    // 17
			"Rho",    // 18
			"Chi",    // 19

			"Epsilon", // 20
			"Omicron", // 21
			"Upsilon", // 22
			"Psi",     // 23

			"Quark" // 24
		};
	}

	// public void AddTestNodes()
	// {
	// 	00 TestAddNode(new List<string>() { "Alpha", "Beta", "Delta" });					// 1-1-1		// 0
	// 	01 TestAddNode(new List<string>() { "Alpha", "Beta", "Gamma" });					// 1-1-2		// 1
	// 	02 TestAddNode(new List<string>() { "Alpha", "Zeta", "Eta" });						// 1-2-1		// 2
	// 	03 TestAddNode(new List<string>() { "Alpha", "Zeta", "Theta", "Iota" });			// 1-2-2-1		// 3
	// 	04 TestAddNode(new List<string>() { "Alpha", "Zeta", "Theta", "Kappa" });			// 1-2-2-2		// 4
	// 	05 TestAddNode(new List<string>() { "Sigma", "Lambda", "Nu" });						// 2-1-1		// 5
	// 	06 TestAddNode(new List<string>() { "Sigma", "Lambda", "Nu", "Xi" });				// 2-1-1-1		// 6
	// 	07 TestAddNode(new List<string>() { "Sigma", "Lambda", "Nu", "Pi" });				// 2-1-1-2		// 7
	// 	08 TestAddNode(new List<string>() { "Sigma", "Lambda", "Epsilon", "Omicron" });		// 2-1-2-1		// 7
	// 	09 TestAddNode(new List<string>() { "Sigma", "Lambda", "Epsilon", "Upsilon" });		// 2-1-2-2		// 7
	// 	10 TestAddNode(new List<string>() { "Sigma", "Lambda", "Psi"});						// 2-1-3		// 7
	// 	11 TestAddNode(new List<string>() { "Sigma", "Omega", "Mu" });						// 2-2-1		// 8
	// 	12 TestAddNode(new List<string>() { "Tau", "Phi", "Rho" });							// 3-1-1		// 9
	// 	13 TestAddNode(new List<string>() { "Tau", "Phi", "Chi" });							// 3-1-2		// 10
	// }


	private KeyValuePair<string, string[]>[][] AddTestNodes2()
	{
		int nodeCount = 25;
		int leafCount = -1;
		int branchCount = 20;

		KeyValuePair<string, string[]>[][] nodesAndLeaves;


		KeyValuePair<string, string[]>[] kvp =
			new KeyValuePair<string, string[]>[nodeCount];

		for (int i = 0; i < nodeCount; i++)
		{
			kvp[i] = new KeyValuePair<string, string[]>(data[i], null);
		}

		nodesAndLeaves = new KeyValuePair<string, string[]>[branchCount][];

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
			kvp[10],
			kvp[20],
			new KeyValuePair<string, string[]>(
				data[21], new []
				{
					$"{data[21]} | Leaf 2-1-2-1-{++leafCount:D3}"
				}
				),
		};

		nodesAndLeaves[9] = new []
		{
			kvp[9],
			kvp[10],
			kvp[20],
			new KeyValuePair<string, string[]>(
				data[22], new []
				{
					$"{data[22]} | Leaf 2-1-2-2-{++leafCount:D3}"
				}
				),
		};

		nodesAndLeaves[10] = new []
		{
			kvp[9],
			kvp[10],
			new KeyValuePair<string, string[]>(
				data[23], new []
				{
					$"{data[23]} | Leaf 2-1-3-{++leafCount:D3}",
					$"{data[23]} | Leaf 2-1-3-{++leafCount:D3}",
					$"{data[23]} | Leaf 2-1-3-{++leafCount:D3}"
				}
				),
		};

		nodesAndLeaves[11] = new []
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

		nodesAndLeaves[12] = new []
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

		nodesAndLeaves[13] = new []
		{
			kvp[16], // tau
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

		// duplicate node name 1

		// add rho more than once
		// original location is tau / phi / rho
		nodesAndLeaves[14] = new []
		{
			kvp[9],  // sigma
			kvp[10], // lambda
			kvp[18]  // rho
		};

		nodesAndLeaves[15] = new []
		{
			kvp[9],  // sigma
			kvp[10], // lambda
			kvp[23], // psi
			kvp[18]  // rho
		};

		nodesAndLeaves[16] = new []
		{
			kvp[0], // alpha
			kvp[1], // beta
			kvp[18] // rho
		};

		nodesAndLeaves[17] = new []
		{
			kvp[0], // alpha
			kvp[4], // zeta
			kvp[6], // theta
			kvp[18] // rho
		};


		return nodesAndLeaves;
	}


	private TreeNode makeTempTreeNode(string? name = null)
	{
		if (name == null)
		{
			name = $"Tree Node {nodeIdx:D5}";
		}

		return new TreeNode(name, null, tree,
			new TreeNodeData($"Node Data {nodeIdx++:D5}"));
	}

	private bool testAddNode(List<string> nodePath, string name)
	{
		treeNode = makeTempTreeNode(name);

		return tree.AddNode(treeNode, nodePath);
	}

	private int ShowTreeNodes()
	{
		int result = 0;

		foreach (TreeNode tcn in tree)
		{
			result++;

			M.WriteLine(ShowNodeParents(tcn));
		}

		return result;
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
			M.WriteLineMargin($"\t{kvp.Value.LeafKey}");
			// Debug.WriteLine();
		}
	}

	private string ShowNodeParents(TreeNode tcn)
	{
		List<string> parents = new List<string>();

		TreeNode node = tcn;
		while (node.ParentNodeEx  != null)
		{
			parents.Add(node.NodeKey);

			node = node.ParentNodeEx;
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
		Debug.Write("node path original| ");

		foreach (KeyValuePair<string, string[]> kvp in kvps)
		{
			Debug.Write($" > {kvp.Key}");
		}

		Debug.Write("\n");
	}

	private void showNodePath(string nodeName, List<string> nodePath)
	{
		Debug.Write("path| ");

		foreach (string s in nodePath)
		{
			Debug.Write($"> {s} ");
		}

		Debug.Write($" | ({nodeName})");
	}

	private void testCountNodes()
	{
		// set selected node
		// tree.ContainsNode(data[9]);
		// tree.SelectedNode = tree.FoundNode;

		// set current node // lambda
		tree.ContainsNode(data[10]);
		tree.CurrentNode = tree.FoundNode;

		// set a node
		// phi
		tree.ContainsNode(data[17]);
		TreeNode? node = tree.FoundNode;

		M.WriteLineMargin("\nNode Counts");

		M.WriteLineMargin("Count Nodes   | Tree", tree.CountNodesTree.ToString());
		M.WriteLineMargin("Count Nodes   | Root", tree.CountNodesRoot.ToString());
		M.NewLine();
		M.WriteLineMargin("Current Node  | Name", tree.CurrentNode?.NodeKey ?? "null");
		M.WriteLineMargin("Count Nodes   | Current", tree.CountNodesCurrent.ToString());
		M.WriteLineMargin("Count Nodes ex| Current", tree.CountNodesCurrentEx.ToString());
		M.NewLine();
		// M.WriteLineMargin("Selected Node | Name", tree.SelectedNode?.NodeKey ?? "null");
		// M.WriteLineMargin("Count Nodes   | Selected", tree.CountNodesSelected.ToString());
		// M.WriteLineMargin("Count Nodes ex| Selected", tree.CountNodesSelectedEx.ToString());
		// M.NewLine();
		M.WriteLineMargin("found Node    | Name", node?.NodeKey ?? "null");
		M.WriteLineMargin("Count Nodes   | Found", node.CountNodes.ToString());
		M.WriteLineMargin("Count Nodes Ex| Found", node.CountNodesEx.ToString());
	}

	private void testCountLeaves()
	{
		// set selected node
		// tree.ContainsNode(data[9]);
		// tree.SelectedNode = tree.FoundNode;

		// set current node // lambda
		tree.ContainsNode(data[10]);
		tree.CurrentNode = tree.FoundNode;

		// set a node
		// phi
		tree.ContainsNode(data[17]);
		TreeNode? node = tree.FoundNode;

		M.WriteLineMargin("\nLeaf Counts");

		M.WriteLineMargin("Count Leaves   | Root", tree.CountLeavesRoot.ToString());
		M.WriteLineMargin("Count Leaves   | tree", tree.CountLeavesTree.ToString());
		M.NewLine();
		M.WriteLineMargin("Current Node   | Name", tree.CurrentNode?.NodeKey ?? "null");
		M.WriteLineMargin("Count Leaves   | Current", tree.CountLeavesCurrent.ToString());
		M.WriteLineMargin("Count Leaves ex| Current", tree.CountLeavesCurrentEx.ToString());
		M.NewLine();
		// M.WriteLineMargin("Selected Node  | Name", tree.SelectedNode?.NodeKey ?? "null");
		// M.WriteLineMargin("Count Leaves   | Selected", tree.CountLeavesSelected.ToString());
		// M.WriteLineMargin("Count Leaves ex| Selected", tree.CountLeavesSelectedEx.ToString());
		// M.NewLine();
		M.WriteLineMargin("found Node     | Name", node?.NodeKey ?? "null");
		M.WriteLineMargin("Count Leaves ex| Found", node.CountLeaves.ToString());
		M.WriteLineMargin("Count Leaves ex| Found", node.CountLeavesEx.ToString());
	}

	private void showFindNodeResults(bool result, TreeNode? node)
	{
		if (!result)
		{
			M.WriteLine("** FAILED **");
			return;
		}
		else
		{
			M.WriteLine($"\t** WORKED ** | node path| {TreeClassEx.GetNodePath(node)}");
		}
	}

	private void testCountLeafMatches(TreeNode node)
	{
		M.WriteLineMargin($"search for leaf with name| {leafSearchString}| starting at| {node.NodeKey}");

		int result = tree.CountMatchingLeaves(leafSearchString, node);

		if (result > 0)
		{
			M.WriteLineMargin($"*** WORKED *** | {result}");
		}
		else
		{
			M.WriteLineMargin($"*** FAILED *** | {result}");
		}
	}

	private void showFindResults(TreeLeaf found, bool result, string foundMsg)
	{
		if (result)
		{
			M.Write(foundMsg);
			M.WriteLine($"key found| {found.LeafKey}| in node| {found.ParentNode.NodeKey}");
		}
		else
		{
			M.WriteLine($"*** FAILED ***");
		}
	}

	private void showNodesAndMaybeLeaves(TreeNode tcn, bool showLeaves)
	{
		M.WriteLineMargin(ShowNodeParents(tcn));
		if (showLeaves) ShowNodeLeaves(tcn);
	}

#endregion

#region event consuming

	private void Tree_NodeSelected(object sender, TreeNode node)
	{
		selectedNodeKey = node.NodeKey;
		UpdatedNodeKeys();

	}

	private void TreeOnNodeDeSelected(object sender, TreeNode node)
	{
		deSelectedNodeKey = node.NodeKey;
		UpdatedNodeKeys();
	}

	private void UpdatedNodeKeys()
	{
		OnPropertyChanged(nameof(SelectedNodeKey));
		OnPropertyChanged(nameof(DeSelectedNodeKey));

		OnPropertyChanged(nameof(SelNodeKey));
		OnPropertyChanged(nameof(PriorSelNodeKey));
	}

#endregion

#region event publishing

	public event PropertyChangedEventHandler PropertyChanged;

	private void OnPropertyChanged([CallerMemberName] string memberName = "")
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
	}

#endregion

#region system overrides

	public override string ToString()
	{
		return $"this is {nameof(DataSampleTree)}";
	}

#endregion
}