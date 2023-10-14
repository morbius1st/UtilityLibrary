﻿#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Xml.Linq;
using SharedCode.SampleData;
using SharedCode.TreeClasses;
using SharedWPF.ShWin;
using UtilityLibrary;
using WpfProject02.Annotations;
// using static SharedCode.SampleData.Tree;
using static SharedCode.SampleData.Selection;
using static SharedCode.SampleData.Selection.SelectMode;
using static SharedCode.SampleData.Selection.SelectFirstClass;
using static SharedCode.SampleData.Selection.SelectSecondClass;
using static SharedCode.SampleData.Selection.SelectTreeAllowed;

#endregion

// username: jeffs
// created: 6/10/2023 8:53:05 AM


/*
 * this is a (can) be a tri state tree (set IsTriState to True)
 *
 * (added = property / method provided but not integrated throughout)
 * (done = Added & integrated / complete)
 * (none = nothing)
 *
 * adding a node options
 * a) provide the node and the node path
 *		* adds all "missing" nodes found in the path
 *		* unambiguous - exact location provided
 *		* tree is the starting point
 * b) "find" the node and add the node to the found node
 *		* if duplicate node names are permitted, the node
 *				could be added to the wrong node
 *		* tree is the starting point
 * c) loop through the tree to locate the correct node
 *		to which to add the node (similar to b - but manual
 *		and allows the possibility of dealing with a duplicate
 *		* "found" node is the starting point (i.e., node added to the
 *				found node and not to the tree
 *
 * adding a leaf
 * same as options a & c above
 *
 * methods / properties needed
 * * tree operations
 *		ADDED	MaxDepth [Max node depth]
 *		ADDED	isTriState
 *		ADDED	isSelected
 *		ADDED	isExpanded
 *			done		Clear (new)
 *			none		select
 *			done		enumerate tree
 *			done		enumerate branch
 *		MAYBE:
 *		Keys
 *		Values
 *	SETTINGS
 *		all nodes must be unique | per tree / per branch
 *		> error options: exception / ignore, return false
 * 
 *			done		AddNode
 *			done		DeleteNode
 *			done		RenameNode
 *			done		ContainsNode (via key)
 *			done		MoveNode (between branches)
 *			done		MakeCurrent (use property)
 *			done		MakeSelected (use property)
 *			done		Count (nodes - [root])
 *			done		Count (nodes - whole tree)
 *			done		Count (nodes - [current])
 *			done		Count (nodes - branch [current])
 *			done		Count (nodes - [selected])
 *			done		Count (nodes - branch [selected])
 *			done		CountMatchingNodes (tree)
 *			done		CountMatchingNodes (this node and below)
 *
 * NONE		selection communication
 * NONE		expansion communication
 *
 *			done		ContainsNode (via key)
 *			done		ContainsKey
 *			done		Add
 *			done		TryAddNode
 *			done		DeleteNode
 *			done		MakeCurrent (use property)
 *			done		parent - make selected (use property)
 *			done		parent - make current (use property)
 *			done		Select
 *			n/a			indexer
 *			done		count (nodes)
 *			done		isSelected
 *			done		isChoosen
 *			done		isExpanded
 * NONE		isTriState
 * NONE		selection communication
 * NONE		expansion communication
 *
 *			done		AddLeaf
 *			done		TryDeleteLeaf
 *			done		ContainsLeaf
 *			n/a			CountLeaves (in this node) 
 *			done		CountLeaves (this node and below)
 *			done		CountMatchingLeaves (tree)
 *			done		CountMatchingLeaves (this node and below) 
 *			n/a			MoveLeaf (within node)
 *			done		MoveLeaf (within between nodes)
 *
 * 
 *			done		AddLeaf (in current node / via node key path) 
 *			done		DeleteLeaf (in current node / via node key path) 
 *			done		ContainsLeaf (via Leaf)
 *			done		MoveLeaf (from node to node)
 *			done		Count (leaves - whole tree)
 * NONE		isSelected
 * NONE		isExpanded
 *
 *		selection communication
 *		MAYBE
 *		Keys
 *		Values
 *
 * *******************************
 *
 * to do
 *
 * TREE class
 * Tree has nothing - all handeled via
 * TREENODE
 *Starting from RootNode
 * With Exceptions
 *
 * implement 2-state selection protocol
 *
 * methods on Tree - but these are just
 * shortcuts to the RootNode Method
 * SelectNodeTree
 * DeSelectNodeTree
 * DeSelectNodeTree
 * DeSelectLeafTree
 *
 *
 *
 *			SelectNodeTree
 *			SelectNodeBranch
 *			SelectNodeAllBranches
 *			SelectMatchingNodeTree
 *			SelectMatchingNodeBranch
 *			SelectNode
 *			DeSelectNodeTree
 *			DeSelectNodeBranch
 * 			DeSelectNodeAllBranches
 *			DeSelectMatchingNodeTree
 *			DeSelectMatchingNodeBranch
 *			DeSelectNode
 *
 *			DeSelectLeafTree
 *			SelectLeafBranch
 * 			SelectLeafAllBranches
 *			SelectMatchingLeafTree
 *			SelectMatchingLeafBranch
 *			SelectLeaf
 *			DeSelectLeafTree
 *			DeSelectLeafBranch
 * 			SelectLeafAllBranches
 *			DeSelectMatchingLeafTree
 *			DeSelectMatchingLeafBranch
 *			DeSelectLeaf
 *
 */

namespace SharedCode.TreeClasses
{
	public interface ITreeSelection
	{
		bool SelectNode();
	}

	public class TreeClass<TNd, TLd> : INotifyPropertyChanged,
		IEnumerable<TreeClassNode<TNd, TLd>>
		where TNd : class
		where TLd : class
	{
	#region private fields

		private TreeClassNode<TNd, TLd> rootNode;

		// represents the last selected (or deselected) node - since
		// multiple nodes / leaves can be selected
		// private TreeClassNode<TNd, TLd>? selectedNode;

		// represents the node currently being used for node operations
		// there must always be a current node
		// can and does change during most node operations 
		// can only be stable if no other node operations occur
		private TreeClassNode<TNd, TLd>? currentNode;

		// represents a temporary node that remains stable during
		// multi-step operations
		// important that this only get used for multi-step
		// but single subject operations
		private TreeClassNode<TNd, TLd>? tempNode;

		// represents a node found during search operations
		// such as ContainsNode
		private TreeClassNode<TNd, TLd>? foundNode;

		private TreeClassLeaf<TNd, TLd>? foundLeaf;


		// private bool isTriState;
		private bool requireUniqueKeys;

		private string leafToFind;
		private List<TreeClassLeaf<TNd, TLd>> foundLeaves;
		private int foundLeafIdx = -1;

		private string nodeToFind;
		private List<TreeClassNode<TNd, TLd>> foundNodes;
		private int foundNodeIdx = -1;

		// selection

		private SelectMode selectMode = INDIVIDUAL;
		// private SelectFirstClass selectFirstClass = NODE_ONLY;
		private List<TreeClassNode<TNd, TLd>> selectedNodes;
		private List<TreeClassNode<TNd, TLd>> priorSelectedNodes;

		private List<TreeClassLeaf<TNd, TLd>> selectedLeaves;

		// .................................check state		  mixed >	checked >	unchecked >	mixed
		//														0			1			2			3 (0)
		public static readonly bool?[] boolList = new bool?[] { null,		true,		false,		null };

	#endregion

	#region ctor

		public TreeClass() { }

		public TreeClass(string treeName, SelectMode mode, TNd nodeData = null)
		{
			TreeName = treeName;
			// NodeData = nodeData;
			SelectMode = mode;

			makeRootNode(nodeData);

			selectedNodes = new List<TreeClassNode<TNd, TLd>>();
			priorSelectedNodes = new List<TreeClassNode<TNd, TLd>>();
		}

	#endregion

	#region public properties

		// basic data

		public string TreeName { get; set; }

		// public TNd NodeData { get; set; }

		public TreeClassNode<TNd, TLd> RootNode => rootNode;

		public TreeClassNode<TNd, TLd> CurrentNode
		{
			get => currentNode;
			set
			{
				if (Equals(value, currentNode)) return;
				currentNode = value;
				OnPropertyChanged();
			}
		}

		public TreeClassNode<TNd, TLd> FoundNode
		{
			get => foundNode;
			set
			{
				if (Equals(value, foundNode)) return;
				foundNode = value;
				OnPropertyChanged();
			}
		}

		public TreeClassLeaf<TNd, TLd> FoundLeaf
		{
			get => foundLeaf;
			set
			{
				if (Equals(foundLeaf, value)) return;
				foundLeaf = value;
				OnPropertyChanged();
			}
		}

		public List<TreeClassNode<TNd, TLd>> FoundNodes => foundNodes;
		public List<TreeClassLeaf<TNd, TLd>> FoundLeaves => foundLeaves;

		// selection
		// most selection login is in the node class

		public List<TreeClassNode<TNd, TLd>> SelectedNodes
		{
			get => selectedNodes;
			private set
			{
				selectedNodes = value;
				OnPropertyChanged();
			}
		}

		public List<TreeClassNode<TNd, TLd>> PriorSelectedNodes
		{
			get => priorSelectedNodes;
			private set
			{
				priorSelectedNodes = value;
				OnPropertyChanged();
			}
		}

		public List<TreeClassLeaf<TNd, TLd>> SelectedLeaves
		{
			get => selectedLeaves;
			set
			{
				selectedLeaves = value;
				OnPropertyChanged();
			}
		}

	#region settings


		// selection

		// one time settings / set upon creation / property change not needed (i think)

		public SelectMode SelectMode
		{
			get => selectMode;
			private set
			{
				selectMode = value;
			}
		}

		public SelectFirstClass SelectFirstClass => SelModes[selectMode].ClassFirst;
		public SelectSecondClass SelectSecondClass => SelModes[selectMode].ClassSecond;
		public SelectTreeAllowed SelectTreeAllowed => SelModes[selectMode].SelectTreeAllowed;


		public bool IsTriState => SelectFirstClass == TRI_STATE;
		public bool CanSelectTree => SelectTreeAllowed == SelectTreeAllowed.YES;
		public bool SelectLeaves => SelectSecondClass == NODES_AND_LEAVES;


		// unique keys

		/// <summary>
		/// require all keys to be unique even if in different branches<br/>
		/// or in different nodes
		/// </summary>
		public bool RequireUniqueKeys
		{
			get => requireUniqueKeys;
			set
			{
				if (value == requireUniqueKeys) return;
				requireUniqueKeys = value;
				OnPropertyChanged();
			}
		}
		

	#endregion

	#region counts

		// counts - attached directly to the node

		// nodes

		public int CountNodesRoot => rootNode?.CountNodes ?? 0;
		public int CountNodesCurrent => currentNode?.CountNodes ?? 0;

		public int CountNodesTree => rootNode?.NodeCountBranch() ?? 0;

		// extended - from the node down
		public int CountNodesCurrentEx => currentNode?.NodeCountBranch() ?? 0;

		// leaves

		public int CountLeavesRoot => rootNode?.CountLeaves ?? 0;
		public int CountLeavesCurrent => currentNode?.CountLeaves ?? 0;

		public int CountLeavesTree => rootNode?.LeafCountBranch() ?? 0;

		// extended - from the node down
		public int CountLeavesCurrentEx => currentNode?.LeafCountBranch() ?? 0;
		

	#endregion


	#endregion

	#region private properties

	#endregion

	#region public methods

	#region selection methods

		// select all nodes from the root node down
		public void SelectNodeTree()
		{
			if (rootNode?.Nodes == null) return;

			if (SelectFirstClass == NODE_MULTI)
			{
				SelectNodeAllBranches(rootNode);
			}
			else if (SelectFirstClass == TRI_STATE)
			{
				SelectNodeAllBranches(rootNode);
			}
		}

		// deselect all nodes from the root node down
		public void DeSelectNodeTree()
		{
			if (rootNode?.Nodes == null) return;

			if (CanSelectTree)
			{
				DeSelectNodeAllBranches(rootNode);
			}

			// if (SelectFirstClass == NODE_MULTI)
			// {
			// 	DeSelectNodeAllBranches(rootNode);
			// }
			// else if (SelectFirstClass == TRI_STATE)
			// {
			// 	DeSelectNodeAllBranches(rootNode);
			// }
		}

		// node 

		public void UpdateSelected(TreeClassNode<TNd, TLd>? node)
		{
			switch (SelectFirstClass)
			{
			case NODE_ONLY:
				{
					// Examples.M.WriteLine("+100 0A doing indiv select");
					updateSelectedIndividual(node);
					break;
				}
			case NODE_EXTENDED:
				{
					// Examples.M.WriteLine("+200 0A doing extended select");
					if (!updateSelectedExtended(node)) return;
					break;
				}
			case NODE_MULTI:
				{
					// Examples.M.WriteLine("+300 0A doing multi-node select");
					priorSelectedNodes.Clear();
					AddNodeToSelected(node);

					break;
				}
			case NODE_MULTI_EX:
				{
					selectBranch(node);
					break;
				}
			case LEAF_MULTI:
				{
					break;
				}
			}

			RaiseNodeSelectedEvent(node);
			OnPropUpdateSelectedNodes();
		}

		public void UpdateDeSelected(TreeClassNode<TNd, TLd> node)
		{
			switch (SelectFirstClass)
			{
			case NODE_ONLY:
				{
					// Examples.M.WriteLine("-10 0A doing indiv deselect");
					// may not remove return here to prevent 
					// event / update
					if (!updateDeselectedIndividual(node)) return;
					break;
				}
			case NODE_EXTENDED:
				{
					// Examples.M.WriteLine("-100 0A doing extended deselect");
					// may not remove return here to prevent 
					// event / update
					if (!updateDeselectedExtended(node)) return;
					break;
				}
			case NODE_MULTI:
				{
					// Examples.M.WriteLine("-200 0A doing multi-node deselect");
					selectedNodes.Remove(node);
					// 
					priorSelectedNodes.Add(node);
					break;
				}
			case NODE_MULTI_EX:
				{
					deselectBranch(node);
					break;
				}

			case LEAF_MULTI:
				{
					break;
				}
			}

			RaiseNodeDeSelectedEvent(node);
			OnPropUpdateSelectedNodes();
		}

		public void UpdateSelectionTriState(TreeClassNode<TNd, TLd>? node)
		{
			// Examples.M.WriteLine("\n+501 00 doing tri-state");
			// Debug.WriteLine($"\n+501 00 doing tri-state| {node}");

			if (node?.ParentNode != null) node.ParentNode.CheckedChangedFromChild(node.IsCheckedState /*, node.PriorIsCheckedState */ );

			if (node.Nodes != null)
			{
				foreach (KeyValuePair<string, TreeClassNode<TNd, TLd>> kvp in node.Nodes)
				{
					// Debug.WriteLine($"+500     update children| name| {kvp.Value.NodeKey}");
					kvp.Value.CheckedChangeFromParent(node.IsCheckedState);
				}
			}

			if (node.IsCheckedState == CheckedState.CHECKED)
			{
				AddNodeToSelected(node);
				RaiseNodeSelectedEvent(node);
			}
			else if (node.IsCheckedState == CheckedState.UNCHECKED)
			{
				RemoveNodeFromSelected(node);
				RaiseNodeDeSelectedEvent(node);
			}
			else
			{
				RaiseNodeMixedEvent(node);
			}
		}

		private void updateSelectedIndividual(TreeClassNode<TNd, TLd> node)
		{
			// only one node selected at a time
			// will always add a node
			// if (selectedNodes.Count == 0)
			// {
			// 	selectedNodes.Add(node);
			// }
			// else 
			// {
				priorSelectedNodes = selectedNodes;
				// selectedNodes = new List<TreeClassNode<TNd, TLd>> { node };

				priorSelectedNodes[0].DeSelectNode();

				RaiseNodeDeSelectedEvent(priorSelectedNodes[0]);
			// }

			selectedNodes.Add(node);
		}

		private bool updateDeselectedIndividual(TreeClassNode<TNd, TLd> node)
		{
			selectedNodes.Remove(node);

			RaiseNodeDeSelectedEvent(node);

			return true;
		}

		private bool updateSelectedExtended(TreeClassNode<TNd, TLd> node)
		{
			// two cases:
			// new - nothing selected before
			// selected node in a different branch

			bool result;

			// gatekeeper added the original node
			if (selectedNodes.Count < 1)
			{
				// case 1 - nothing selected before
				return selectBranch(node);
			}
			else
			{
				// case 3 - selected node from a different branch
				// case 4 - selected node in same branch but closer to root selected
				// Examples.M.WriteLine(" 100 3A extended select| case 3| node in a different branch selected");
				// Examples.M.WriteLine(" 100 4A extended select| case 4| or node closer to root in the tree selected");

				// procedure:
				// deselect branch
				// select branch

				deselectBranch(selectedNodes[0]);
				selectBranch(node);
			}

			return true;
		}

		private bool updateDeselectedExtended(TreeClassNode<TNd, TLd> node)
		{
			// case 2
			// one of the selected nodes was deselected
			// deselect whole branch 
			// 
			// Examples.M.WriteLine("-100 0A extended select| deselect");

			bool result = deselectBranch(selectedNodes[0]);
			// if (!result) return false;

			// node.SelectNode();
			// AddNodeToSelected(node);

			// result = selectBranch(node);

			return result;
		}

		private bool selectBranch(TreeClassNode<TNd, TLd> node)
		{
			// case 1 - nothing selected before
			// Examples.M.WriteLine(" 100 1A extended select| case 1| nothing selected before");

			if (!AddNodeToSelected(node)) return false;

			// RaiseNodeSelectedEvent(node);

			// node.SelectNode();
			SelectNodeAllBranches(node);

			return true;
		}

		private bool deselectBranch(TreeClassNode<TNd, TLd> node)
		{
			// generic method - start at node provided and work 
			// down from there

			// selected and priorselected could have members
			// that remain so they cannot be wiped clean

			// provided node must be in the selected node list
			if (!selectedNodes.Contains(node)) return false;

			priorSelectedNodes.Clear();

			// note that the selected node list cannot be modified
			// while deselecting
			node.DeSelectNode();

			List<TreeClassNode<TNd, TLd>> removed = new() { node };

			removed.AddRange(DeSelectNodeAllBranches(node));

			removeListFromSelected(removed);

			return true;
		}


		public bool AddNodeToSelected(TreeClassNode<TNd, TLd> node)
		{
			if (node == null) return false;

			RemoveNodeFromPriorSelected(node);

			if (selectedNodes.Contains(node)) return false;

			selectedNodes.Add(node);
			OnPropertyChanged(nameof(SelectedNodes));

			RaiseNodeSelectedEvent(node);

			if (SelectLeaves) SelectAllLeavesInAllNodes(node);

			return true;
		}

		public bool RemoveNodeFromSelected(TreeClassNode<TNd, TLd> node)
		{
			if (!selectedNodes.Contains(node)) return false;

			AddNodeToPriorSelected(node);

			selectedNodes.Remove(node);
			OnPropertyChanged(nameof(SelectedNodes));

			RaiseNodeDeSelectedEvent(node);

			if (SelectLeaves) DeSelectAllLeavesInAllNodes(node);

			return true;
		}


		// leaf

		public bool SelectAllLeavesInAllNodes(TreeClassNode<TNd, TLd> node)
		{
			if (node == null) return false;

			bool result = true;

			foreach (TreeClassNode<TNd, TLd> n in node)
			{
				result = result && selectAllLeavesInNode(n);
			}

			return result;
		}

		public bool DeSelectAllLeavesInAllNodes(TreeClassNode<TNd, TLd> node)
		{
			if (node == null) return false;

			bool result = true;

			foreach (TreeClassNode<TNd, TLd> n in node)
			{
				result = result && deSelectAllLeavesInNode(n);
			}

			return result;
		}


		private bool selectAllLeavesInNode(TreeClassNode<TNd, TLd> node)
		{
			if (node == null) return false;

			bool result = true;

			foreach (KeyValuePair<string, TreeClassLeaf<TNd, TLd>> kvp in node.Leaves)
			{
				kvp.Value.SelectLeaf();
				result = result && AddLeafToSelected(kvp.Value);
			}

			return result;
		}

		private bool deSelectAllLeavesInNode(TreeClassNode<TNd, TLd> node)
		{
			if (node == null) return false;

			bool result = true;

			foreach (KeyValuePair<string, TreeClassLeaf<TNd, TLd>> kvp in node.Leaves)
			{
				kvp.Value.SelectLeaf();
				result = result && RemoveLeafFromSelected(kvp.Value);
			}

			return result;
		}


		public bool AddLeafToSelected(TreeClassLeaf<TNd, TLd> leaf)
		{
			if (leaf == null || selectedLeaves.Contains(leaf)) return false;

			// not using prior selected

			selectedLeaves.Add(leaf);
			OnPropertyChanged(nameof(SelectedLeaves));

			RaiseLeafSelectedEvent(leaf);

			return true;
		}

		public bool RemoveLeafFromSelected(TreeClassLeaf<TNd, TLd> leaf)
		{
			if (leaf == null || !selectedLeaves.Contains(leaf)) return false;

			selectedLeaves.Remove(leaf);
			OnPropertyChanged(nameof(SelectedLeaves));

			RaiseLeafDeSelectedEvent(leaf);

			return true;
		}


		// private

		private bool AddNodeToPriorSelected(TreeClassNode<TNd, TLd> node)
		{
			if (priorSelectedNodes.Contains(node)) return false;
			priorSelectedNodes.Add(node);
			OnPropertyChanged(nameof(PriorSelectedNodes));
			return true;
		}

		private bool RemoveNodeFromPriorSelected(TreeClassNode<TNd, TLd> node)
		{
			if (!priorSelectedNodes.Contains(node)) return false;
			priorSelectedNodes.Remove(node);
			OnPropUpdateSelectedNodes();
			return true;
		}


		private void SelectNodeAllBranches(TreeClassNode<TNd, TLd> node)
		{
			if (node.Nodes == null || node.Nodes.Count == 0) return;

			foreach (KeyValuePair<string, TreeClassNode<TNd, TLd>> kvp in node.Nodes)
			{
				kvp.Value.SelectNode();
				AddNodeToSelected(kvp.Value);
				SelectNodeAllBranches(kvp.Value);
			}
		}

		private List<TreeClassNode<TNd, TLd>> DeSelectNodeAllBranches(TreeClassNode<TNd, TLd> node)
		{
			// need to be careful here - cannot modify the selectednode list while deselecting
			// as this will cause conflict issues
			List<TreeClassNode<TNd, TLd>> removed = new List<TreeClassNode<TNd, TLd>>();

			if (node.Nodes == null || node.Nodes.Count == 0) return removed;

			// this will deselect all nodes and add them to the
			// prior selected list.
			foreach (KeyValuePair<string, TreeClassNode<TNd, TLd>> kvp in node.Nodes)
			{
				removed.AddRange(DeSelectNodeAllBranches(kvp.Value));

				kvp.Value.DeSelectNode();
				// AddNodeToPriorSelected(kvp.Value);

				removed.Add(kvp.Value);
			}

			return removed;
		}

		private void removeListFromSelected(List<TreeClassNode<TNd, TLd>> removed)
		{
			if (removed == null || removed.Count < 1) return;

			foreach (TreeClassNode<TNd, TLd> node in removed)
			{
				RemoveNodeFromSelected(node);
			}
		}



		private void OnPropUpdateSelectedNodes()
		{
			OnPropertyChanged(nameof(SelectedNodes));
			OnPropertyChanged(nameof(PriorSelectedNodes));
		}


		public void ResetSelected()
		{
			priorSelectedNodes = new List<TreeClassNode<TNd, TLd>>();
			selectedNodes = new List<TreeClassNode<TNd, TLd>>();

			OnPropUpdateSelectedNodes();
		}


		// checked state

		// private void SetSelectFirstClass()
		// {
		// 	SelectFirstClass = (SelectFirstClass) ((int) selectMode / 100 * 100);
		// }

		public static int IndexFromBool(bool? test)
		{
			if (!test.HasValue)
			{
				// null
				return (int) NULL;
			}
			else if (test.Value)
			{
				// true
				return (int) TRUE;
			}

			return (int) FALSE;
		}

		public static CheckedState StateFromBool(bool? test)
		{
			return (CheckedState) IndexFromBool(test);
		}

		public static CheckedState StateFromIndex(int idx)
		{
			return (CheckedState) idx;
		}



	#endregion


	#region tree methods

		// clear the whole node (and leaves) tree
		public void Clear()
		{
			rootNode = null;
			makeRootNode(null);

			OnPropertyChanged(nameof(RootNode));

			GC.Collect();

			CurrentNode = null;
			FoundNode = null;
			RequireUniqueKeys = false;
			SelectMode = SelectMode.UNDEFINED; // and SelectClass
			TreeName = null;

			SelectedNodes = new List<TreeClassNode<TNd, TLd>>();
			PriorSelectedNodes = new List<TreeClassNode<TNd, TLd>>();

			// if (rootNode == null)
			// {
			// 	makeRootNode();
			// 	return;
			// }
			//
			// rootNode.Nodes.Clear();
			//
			// if (rootNode.HasLeaves) rootNode.ClearLeaves();
			//
			// OnPropertyChanged(nameof(RootNode));
		}

	#endregion


	#region node methods

		// add methods

		/// <summary>
		/// Add a node (if possible) to the current node or, if provided,<br/>
		/// to the node provided. respects RequireUniqueKeys<br/>
		/// Returns: true->worked / false->failed
		/// </summary>
		public bool AddNode(TreeClassNode<TNd, TLd> node, TreeClassNode<TNd, TLd>? addNode = null)
		{
			TreeClassNode<TNd, TLd>? an;

			an = addNode ?? currentNode;

			if (an == null ||
				!OkToAddNode(node)) return false;

			an.InitNodes();

			// bool result = an.Nodes.TryAdd(node.NodeKey, node);
			bool result = an.TryAddNode(node.NodeKey, node);

			if (result)
			{
				node.ParentNodeEx = an;
				node.Tree = this;
				an = node;
			}

			CurrentNode = an;

			return result;
		}

		/// <summary>
		/// add a node (if possible) to the node at the end of the node path<br/>
		/// node path is absolute (starts at rootNode<br/>
		/// return true -> worked; false -> failed
		/// </summary>
		public bool AddNode(TreeClassNode<TNd, TLd> newNode, List<string> nodePath)
		{
			bool result;
			TreeClassNode<TNd, TLd>? node;

			if (!getNodeByPath(nodePath, out node)) return false;

			return AddNode(newNode, node);
		}

		// contains methods

		/// <summary>
		/// Search for a node with the key provided: <c>findKey</c><br/>
		///	Start from <c>RootNode</c><br/>
		/// This is just a shortcut for the main <c>ContainsNode</c><br/>
		/// return true else return false<br/>
		/// Sets FoundNode to the node found, if any
		/// </summary>
		public bool ContainsNode(string nodeKey)
		{
			FoundNode = null;
			TreeClassNode<TNd, TLd> t;
			bool result = ContainsNode(nodeKey, out t);
			return result;
		}

		// nth | == 1 means, find first, 2 means find second, etc.
		// == 0 means find last
		// == -1 means loop through all of the found nodes
		/// <summary>
		/// Search for a node with the key provided: <c>findKey</c>. Start from
		/// <c>startPoint</c> if provided, at <c>rootNode</c> if not.
		/// return the node found or null when there are duplicate node names
		/// allowed, nth = 1 means, return the first match, 2 means the second, etc.
		/// nth = 0, returns the last match. this routine always finds all of the
		/// matches (except when <c>firstMatch</c> is false) and puts the list
		/// into <c>FoundNodes</c>.
		/// Setting nth as -1, the routine will return each match in the series until it
		/// return null as an indication that there are no more matches. Setting
		/// nth as -1 also sets firstMatch as false.
		/// when firstMatch is true, this stops looking and returns the match
		/// immediately
		/// Sets FoundNode to the node found, if any
		/// </summary>
		public bool ContainsNode(string findKey,
			out TreeClassNode<TNd, TLd> node,
			TreeClassNode<TNd, TLd>? startNode = null,
			int nth = 1,
			bool firstMatch = true)
		{
			int nodeCount;
			node = null;
			foundNode = null;

			if (nth < -1 || findKey.IsVoid()) return false;

			startNode ??= rootNode;

			if (nth == -1)
			{
				nth =
					nextFoundNode(findKey, out node, nth);

				FoundNode = node;

				if (nth == -1) return false;
				if (nth == 0) return true;
			}

			nodeCount = GetMatchingNodes(findKey, startNode, nth, firstMatch);

			nth = nth == 0 ? nodeCount : nth;

			if (nodeCount < nth) return false;

			node = foundNodes[nth - 1];
			FoundNode = node;

			return true;
		}

		// delete methods 

		/// <summary>
		/// Delete the node provided. Note that all nodes connected to the<br/>
		/// deleted node also get deleted.<br/>
		/// Returns: true -> worked / false -> failed
		/// </summary>
		public bool DeleteNode(TreeClassNode<TNd, TLd>? node)
		{
			// if (node == null) return false;
			//
			// TreeClassNode<TNd, TLd> parentNode = node.ParentNodeEx;
			//
			// if (parentNode == null) return false; 
			//
			// if (node?.ParentNode == null) return false;
			//
			// return ((TreeClassNode<TNd, TLd>) node.ParentNode).DeleteNode(node.NodeKey);
			return ((TreeClassNode<TNd, TLd>) node?.ParentNode)?.DeleteNode(node.NodeKey) ?? false;
		}

		/// <summary>
		/// Delete a node (if possible) based on a node path. the last node<br/>
		/// in the path is the node to delete. a copy of the deleted node is<br/>
		/// returned in the out parameter. Note that all nodes connected to<br/>
		/// deleted node also get deleted.<br/>
		/// Returns: true -> worked / false -> failed
		/// </summary>
		public bool DeleteNode(List<string> nodePath, out TreeClassNode<TNd, TLd>? node)
		{
			node = null;

			if (nodePath == null || nodePath.Count == 0) return false;

			if (!getNodeByPath(nodePath, out node)) return false;

			return node.ParentNodeEx?.DeleteNode(node.NodeKey) ?? false;
		}

		// move methods

		/// <summary>
		/// Move a node (if possible) from one node to another node<br/>
		/// Returns: true -> worked / false -> failed
		/// </summary>
		public bool MoveNode(TreeClassNode<TNd, TLd>? sourceNode, TreeClassNode<TNd, TLd>? destinationNode)
		{
			// steps:
			// check with destination node that a node with the
			// same key does not already exist
			// get parent node for the source node
			// delete the source node
			// add source to the destination

			if (sourceNode == null || destinationNode == null) return false;

			if (destinationNode.ContainsNode(sourceNode.NodeKey)) return false;

			bool result;

			TreeClassNode<TNd, TLd>? parentNode = (TreeClassNode<TNd, TLd>) sourceNode.ParentNode;

			if (parentNode == null) return false;

			parentNode.DeleteNode(sourceNode.NodeKey);

			sourceNode.ParentNodeEx = destinationNode;

			destinationNode.AddNode(sourceNode.NodeKey, sourceNode);

			return true;
		}

		// rename method

		/// <summary>
		/// Changes the nodeKey
		/// </summary>
		public bool RenameNode(TreeClassNode<TNd, TLd> node, string newName)
		{
			if (node == null || newName.IsVoid()) return false;

			TreeClassNode<TNd, TLd>? parentNode = node.ParentNodeEx;

			if (parentNode == null) return false;

			if (!parentNode.ReplaceKeyNode(node.NodeKey, newName)) return false;

			node.NodeKey = newName;

			return true;
		}

		// matching

		/// <summary>
		/// get the list of list of nodes that match <c>findKey</c>
		/// </summary>
		public int GetMatchingNodes(string findKey,
			TreeClassNode<TNd, TLd>? startNode = null,
			int nth = 1,
			bool firstMatch = false)
		{
			foundNodes = new List<TreeClassNode<TNd, TLd>>();

			if (findKey.IsVoid()) return -1;

			startNode ??= rootNode;

			foreach (TreeClassNode<TNd, TLd> node in startNode)
			{
				if (node.NodeKey.Equals(findKey))
				{
					foundNodes.Add(node);

					if (firstMatch && foundNodes.Count == nth) break;
				}
			}

			return foundNodes.Count;
		}

		/// <summary>
		/// return the index for the next matching node after a list of matching
		/// nodes has been found and saved
		/// </summary>
		private int nextFoundNode(string nodeKey,
			out TreeClassNode<TNd, TLd> node, int nth)
		{
			node = null;
			FoundNode = null;

			// change node key? reset if yes
			if (nodeToFind == null || !nodeToFind.Equals(nodeKey))
			{
				foundNodeIdx = -1;
			}

			nodeToFind = nodeKey;


			if (foundNodeIdx < 0)
			{
				foundNodeIdx = 0;
				return 1; // nth becomes this
			}

			// foundidx is >= 0

			foundNodeIdx++;

			if (foundNodeIdx >= foundNodes.Count)
			{
				foundNodeIdx = -1;
				return -1; // nth becomes this - and this means fail
			}

			// got good info - set the node and return 0 - which means, done
			node = foundNodes[foundNodeIdx];
			FoundNode = node;
			return 0;
		}

		/// <summary>
		/// Count the number of nodes, in the whole tree, that match "nodeKey"<br/>
		/// start at 'startPoint' if provided - start at rootNode if not<br/>
		/// return -1 if nodekey is null or empty;<br/>
		/// 0 if none found; else the number found<br/>
		/// </summary>
		public int CountMatchingNodes(string nodeKey, TreeClassNode<TNd, TLd>? startPoint)
		{
			if (nodeKey == null) return -1;

			TreeClassNode<TNd, TLd> node;

			bool result = ContainsNode(nodeKey, out node, startPoint, 1) ;

			if (!result) return 0;

			return foundNodes.Count;
		}

		// enumerate

		/// <summary>
		/// enumerate through the all of the nodes for every node starting
		/// from startNode (if provided) or RootNode if not provided
		/// </summary>
		public IEnumerable<TreeClassNode<TNd, TLd>> GetNodes(TreeClassNode<TNd, TLd> startNode = null)
		{
			TreeClassNode<TNd, TLd> treeNode = startNode == null ? RootNode : startNode;

			foreach (TreeClassNode<TNd, TLd> tn in treeNode)
			{
				yield return tn;
			}
		}


		// public bool ContainsNode(string findKey,
		// 	TreeClassNode<TNd, TLd>? startNode = null,
		// 	int nth = 1,
		// 	bool firstMatch = true)
		// {
		// 	int nodeCount;
		// 	currentNode = null;
		//
		// 	if (nth < -1 || findKey.IsVoid()) return false;
		//
		// 	startNode ??= rootNode;
		//
		// 	if (nth == -1) 
		// 	{
		// 		nth =
		// 			nextFoundNode(findKey, out currentNode, nth);
		//
		// 		if (nth == -1) return false;
		// 		if (nth == 0) return true;
		// 	}
		//
		// 	nodeCount = GetMatchingNodes(findKey, startNode, nth, firstMatch);
		//
		// 	nth = nth == 0 ? nodeCount : nth;
		//
		// 	if (nodeCount < nth) return false;
		//
		// 	currentNode = foundNodes[nth - 1];
		//
		// 	return true;
		// }


		// process loop
		// return info
		// if (foundIdx >= 0)
		//		if (foundidx >= list.count) -> return node == null & -1 - fail
		//		if (foundidx < list.count) -> return a node & 0 - good return / done
		//	else
		//		if (foundidx < 0) -> return node == null & 1 - good / continue
		// or
		// return 1, proceed normally
		// return -1, fail, return false
		// return 0, got next node, set and return true;

		// delete methods

	#endregion

	#region leaf methods

		// add methods

		/// <summary>
		/// Add a leaf to the current node
		/// </summary>
		/// <param name="leaf"></param>
		/// <returns></returns>
		public bool AddLeaf(TreeClassLeaf<TNd, TLd> leaf, TreeClassNode<TNd, TLd> node = null)
		{
			if (node != null)
			{
				tempNode = node;
			}
			else
			{
				tempNode = currentNode;
			}

			if (tempNode == null) return false;

			tempNode.InitLeaves();

			return tempNode?.Leaves?.TryAdd(leaf.LeafKey, leaf) ?? false;
		}

		// delete

		public bool DeleteLeaf(TreeClassNode<TNd, TLd>? node, TreeClassLeaf<TNd, TLd>? leaf)
		{
			if (leaf == null || leaf.LeafKey.IsVoid()) return false;

			return node?.TryDeleteLeaf(leaf.LeafKey) ?? false;
		}

		// move
		public bool MoveLeaf(TreeClassNode<TNd, TLd>? source, TreeClassNode<TNd, TLd>? destination, string leafKey)
		{
			bool result;
			TreeClassLeaf<TNd, TLd> leaf = null;

			if (source == null || destination == null) return false;

			result = destination.ContainsLeaf(leafKey);
			// if found, duplicate - about
			if (result) return false;

			// sets foundleaf
			result = source.ContainsLeaf(leafKey);
			// if not found - abort
			if (!result) return false;

			leaf = source.FoundLeaf;

			// setup - make the move
			// add to destination
			// error - abort
			// remove from source
			// error - remove from destination
			// abort

			result = AddLeaf(leaf, destination);

			if (!result) return false;

			result = DeleteLeaf(source, leaf);

			if (!result)
			{
				DeleteLeaf(destination, leaf);
				return false;
			}

			leaf.MoveParent(destination);

			foundLeaf = leaf;

			OnPropertyChanged(nameof(RootNode));

			return true;
		}

		// rename

		public bool RenameLeaf(TreeClassNode<TNd, TLd> node, string oldKey, string newKey)
		{
			if (node == null) return false;

			return !node.ReplaceKeyLeaf(oldKey, newKey);
		}

		// contains

		/// <summary>
		/// Search for a leaf with the key provided: <c>findKey</c>. Start from
		/// <c>startPoint</c> if provided, at <c>rootNode</c> if not.
		/// return the leaf found or null when there are duplicate leaves names
		/// allowed, nth = 1 means, return the first match, 2 means the second, etc.
		/// nth = 0, returns the last match. this routine always finds all of the
		/// matches (except when <c>firstMatch</c> is false) and puts the list
		/// into <c>FoundLeaves</c>.
		/// Setting nth as -1, the routine will return each match in the series until it
		/// return null as an indication that there are no more matches. Setting
		/// nth as -1 also sets firstMatch as false. 
		/// When firstMatch is true, this stops looking and returns the match
		/// immediately. 
		/// </summary>
		public bool ContainsLeaf(string findKey,
			out TreeClassLeaf<TNd, TLd> leaf,
			TreeClassNode<TNd, TLd>? startNode = null,
			int nth = 1,
			bool firstMatch = true)
		{
			int leafCount;
			leaf = null;

			if (nth < -1 || findKey.IsVoid()) return false;

			startNode ??= rootNode;

			if (nth == -1)
			{
				firstMatch = false;

				nth =
					nextFoundLeaf(findKey, out leaf, nth);

				if (nth == -1) return false;
				if (nth == 0) return true;
			}

			leafCount = GetMatchingLeaves(findKey, startNode, nth, firstMatch);

			nth = nth == 0 ? leafCount : nth;

			if (leafCount < nth) return false;

			leaf = foundLeaves[nth - 1];

			return true;
		}

		// matching

		/// <summary>
		/// find all of the leaves that match <c>findKey</c>
		/// </summary>
		public int GetMatchingLeaves(string findKey,
			TreeClassNode<TNd, TLd>? startNode = null,
			int nth = 1,
			bool firstMatch = false)
		{
			foundLeaves = new List<TreeClassLeaf<TNd, TLd>>();

			if (findKey.IsVoid()) return -1;

			startNode ??= rootNode;

			foreach (TreeClassLeaf<TNd, TLd> l in this.GetLeaves(startNode))
			{
				if (l.LeafKey.Equals(findKey))
				{
					foundLeaves.Add(l);

					if (firstMatch && foundLeaves.Count == nth) break;
				}
			}

			return foundLeaves.Count;
		}

		private int nextFoundLeaf(string findKey,
			out TreeClassLeaf<TNd, TLd> leaf, int nth)
		{
			leaf = null;

			if (leafToFind == null || !leafToFind.Equals(findKey))
			{
				foundLeafIdx = -1;
			}

			leafToFind = findKey;

			if (foundLeafIdx < 0)
			{
				foundLeafIdx = 0;
				return 1;
			}

			// foundidx >= 0

			foundLeafIdx++;

			if (foundLeafIdx >= foundLeaves.Count)
			{
				foundLeafIdx = -1;
				return -1;
			}

			leaf = foundLeaves[foundLeafIdx];
			return 0;
		}

		/// <summary>
		/// Count the number of leaves, in the whole tree, that match "leafKey"<br/>
		/// start at 'startPoint' if provided - start at rootNode if not<br/>
		/// return -1 if leafkey is null or empty; 0 if none found; else the number found<br/>
		/// </summary>
		public int CountMatchingLeaves(string leafKey, TreeClassNode<TNd, TLd>? startPoint)
		{
			if (leafKey.IsVoid()) return -1;

			TreeClassLeaf<TNd, TLd> leaf;

			bool result = ContainsLeaf(leafKey, out leaf, startPoint, 1);

			if (!result) return 0;

			return foundLeaves.Count;
		}

		// enumerate

		/// <summary>
		/// enumerate through the all of the leaves for every node starting
		/// from startNode (if provided) or RootNode if not provided
		/// </summary>
		public IEnumerable<TreeClassLeaf<TNd, TLd>> GetLeaves(TreeClassNode<TNd, TLd> startNode = null)
		{
			TreeClassNode<TNd, TLd> treeNode = startNode == null ? RootNode : startNode;

			foreach (TreeClassNode<TNd, TLd> tn in treeNode)
			{
				if (!tn.HasLeaves) continue;

				foreach (KeyValuePair<string, TreeClassLeaf<TNd, TLd>> kvp in tn.Leaves)
				{
					yield return kvp.Value;
				}
			}
		}


		/*

				/// <summary>
				/// Search for a leaf with the provided name (if possible)<br/>
				/// start at 'startPoint' if provided - start at rootNode if not<br/>
				/// Finds the first occurrence of a leaf with the name provided <br/>
				/// note: since there could be duplicate names, the leaf returned<br/>
				/// may not be the correct leaf. provide an integer (nth) to<br/>
				/// find the nth occurrence of the leafKey<br/>
				/// nth is 1 based - set nth to 1 to find the first occurrence<br/>
				/// Set nth to 0 to find the last matching leaf<br/>
				/// Set nth to -1 to loop through all of the duplicate leaves
				/// </summary>
				public bool ContainsLeaf(string leafKey, 
					TreeClassNode<TNd, TLd>? startPoint, out TreeClassLeaf<TNd, TLd> leaf, int nth = 1)
				{
					Examples.M.WriteLineStatus("Enter Method");

					leaf = null;

					if (nth < -1 || leafKey.IsVoid()) return false;

					startPoint ??= rootNode;

					// if nth != -1; ignore / proceed / do nothing - 

					// false if no more elements in list: return (leaf = null) & -1
					// true if got next item; return leaf & 1
					// if (idx <0), first time through, set values; return (leaf = null) & null

					if (nth == -1)
					{
						if (leafToFind == null ||
							!leafToFind.Equals(leafKey))
						{
							foundLeafIdx = -1;
						}

						leafToFind = leafKey;

						if (foundLeafIdx >= 0)
						{
							foundLeafIdx++;

							if (foundLeafIdx >= (foundLeaves.Count))
							{
								foundLeafIdx = -1;
								return false;
							}

							leaf = foundLeaves[foundLeafIdx];

							return true;
						}
						else
						{
							foundLeafIdx = 0;
							nth = 1;
						}
					}

					foundLeaves = new List<TreeClassLeaf<TNd, TLd>>();

					foreach (TreeClassLeaf<TNd, TLd> l in this.GetLeaves(startPoint))
					{
						if (l.LeafKey.Equals(leafKey))
						{
							foundLeaves.Add(l);
						}
					}

					// nth could == 0, 1, 2, etc; but not -1
					nth = nth == 0 ? foundLeaves.Count : nth;

					if (foundLeaves.Count < nth) return false;

					leaf = foundLeaves[nth - 1];

					return true;
				}

		*/

	#endregion

	#endregion

	#region private methods

		// make a temporary nodeKey based on the 
		// nodes current key
		private string? getTempNodeKey(string currKey)
		{
			string tempKeySuffix = "_temp";
			string? tempKey = null;
			bool result;
			int maxLoops = 99;

			for (int i = 0; i < maxLoops; i++)
			{
				tempKey = $"{currKey}{tempKeySuffix}{i:D2}";
				result = containsNodeTree(tempKey);
				if (!result)
				{
					maxLoops = -1;
					break;
				}
			}

			return maxLoops == -1 ? tempKey : null;
		}

		private void makeRootNode(TNd? nodeData)
		{
			rootNode =
				new TreeClassNode<TNd, TLd>("ROOT", this, null, nodeData);
			rootNode.InitNodes();

			currentNode = rootNode;
			// selectedNode = rootNode;
		}

		private bool OkToAddNode(TreeClassNode<TNd, TLd> node)
		{
			if (node == null || node.NodeKey.IsVoid()) return false;

			if (requireUniqueKeys)
			{
				return !containsNodeTree(node.NodeKey);
			}

			return true;
		}

		// todo - fix to not use "name" and use key
		// search for a node with the name provided 
		// start at the root node - set CurrentNode to
		// the node if found and return true
		// else return false
		private bool containsNodeTree(string findKey)
		{
			Examples.M.WriteLineStatus("Enter Method");
			currentNode = rootNode;

			IEnumerator<TreeClassNode<TNd, TLd>> ie = GetEnumerator();

			while (ie?.MoveNext() ?? false)
			{
				if (ie.Current.NodeKey.Equals(findKey))
				{
					currentNode = ie.Current;
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Get a node, with the key name provided, at the path given - if it exists<br/>
		/// return false if not found; set currNode to null; set out node to null<br/>
		/// return true if found; set currNode and out node to the node found
		/// </summary>
		private bool getNodeByPath(List<string>? nodePath, out TreeClassNode<TNd, TLd>? node)
		{
			node = null;
			bool result;

			if (nodePath != null)
			{
				node = rootNode;

				foreach (string key in nodePath)
				{
					result = node.Nodes?.TryGetValue(key, out node) ?? false;

					if (!result)
					{
						node = null;
						break;
					}
				}
			}

			if (node == null)
			{
				currentNode = null;
				return false;
			}

			currentNode = node;
			return true;
		}

		// private string formatSelNodePathTemp()
		// {
		// 	StringBuilder sb = new StringBuilder();
		//
		// 	foreach (string s in selectedNodePathTemp)
		// 	{
		// 		sb.Insert(0, $">{s}");
		// 	}
		//
		// 	return sb.ToString();
		// }

	#endregion

	#region event consuming

	#endregion

	#region event publishing

		public event PropertyChangedEventHandler? PropertyChanged;

		[DebuggerStepThrough]
		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


		// node selected
		public delegate void NodeSelectedEventHandler(object sender, TreeClassNode<TNd, TLd> node);

		public event NodeSelectedEventHandler NodeSelected;

		[DebuggerStepThrough]
		protected virtual void RaiseNodeSelectedEvent(TreeClassNode<TNd, TLd> node)
		{
			NodeSelected?.Invoke(this, node);
		}


		// node deselected

		public delegate void NodeDeSelectedEventHandler(object sender, TreeClassNode<TNd, TLd> node);

		public event NodeDeSelectedEventHandler NodeDeSelected;

		[DebuggerStepThrough]
		protected virtual void RaiseNodeDeSelectedEvent(TreeClassNode<TNd, TLd> node)
		{
			NodeDeSelected?.Invoke(this, node);
		}


		// node mixed 

		public delegate void NodeMixedEventHandler(object sender, TreeClassNode<TNd, TLd> node);

		public event NodeMixedEventHandler NodeMixed;

		[DebuggerStepThrough]
		protected virtual void RaiseNodeMixedEvent(TreeClassNode<TNd, TLd> node)
		{
			NodeMixed?.Invoke(this, node);
		}

		// leaf selected

		public delegate void LeafSelectedEventHandler(object sender, TreeClassLeaf<TNd, TLd> leaf);

		public event LeafSelectedEventHandler LeafSelected;

		[DebuggerStepThrough]
		protected virtual void RaiseLeafSelectedEvent(TreeClassLeaf<TNd, TLd> leaf)
		{
			LeafSelected?.Invoke(this, leaf);
		}

		// leaf deselected

		public delegate void LeafDeSelectedEventHandler(object sender, TreeClassLeaf<TNd, TLd> leaf);

		public event LeafDeSelectedEventHandler LeafDeSelected;

		[DebuggerStepThrough]
		protected virtual void RaiseLeafDeSelectedEvent(TreeClassLeaf<TNd, TLd> leaf)
		{
			LeafDeSelected?.Invoke(this, leaf);
		}

	#endregion

	#region system overrides

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new TreeClassEnumerator<TNd, TLd>(this);
		}

		public IEnumerator<TreeClassNode<TNd, TLd>> GetEnumerator()
		{
			return new TreeClassEnumerator<TNd, TLd>(this);
		}

		public override string ToString()
		{
			return $"{nameof(TreeClass<TNd, TLd>)} | name| {TreeName}";
		}

	#endregion
	}
}

namespace SharedCode.SampleData
{
#region treeclass system

#region shortcuts

#endregion

	/*
	 Tree Class:
	// methods
	// tree							   
	Clear							   
	GetEnumerator					   
	ToString						   
	TreeClass						   
									   
									   
	// node							   
	AddNode							   
	ContainsNode					   
	DeleteNode						   
	GetMatchingNodes				   
	MoveNode						   
	RenameNode						   
									   
									   
	// leaf							   
	AddLeaf							   
	ContainsLeaf					   
	DeleteLeaf						   
	GetLeaves						   
	GetMatchingLeaves				   
	MatchingLeafCount				   
	MoveLeaf						   
									   
	 properties						   
	TreeName						   
									   
	CountLeavesCurrent				   
	CountLeavesCurrentEx			   
	CountLeavesRoot					   
	CountLeavesSelected				   
	CountLeavesSelectedEx			   
	CountLeavesTree					   
	CountNodesCurrent				   
	CountNodesCurrentEx				   
	CountNodesRoot					   
	CountNodesSelected				   
	CountNodesSelectedEx			   
	CountNodesTree					   
									   
	RootNode						   
	NodeData						   
									   
	CurrentNode						   
	SelectedNode					   
									   
	FoundNode						   
	FoundLeaf						   
	FoundLeaves						   
									   
	SelectedNodePath				   
									   
	IsSelected						   
	IsTriState						   
	MaxDepth						   
	RequireUniqueKeys				   
									   
									   
	events							   
	PropertyChanged	
	

	TreeClassNode<TNd, TLd>:









									   
	*/

#region Tree Classes

#endregion

#region extender / add features

	public class Selection
	{
		public enum CheckedState
		{
			UNSET		= -1,
			MIXED		= 0,
			CHECKED		= 1,
			UNCHECKED	= 2
		}

		public const int NULL  = 0;
		public const int TRUE  = 1;
		public const int FALSE = 2;

		public enum SelectFirstClass
		{
			UNSET				= 0,
			NODE_ONLY			= 100,
			NODE_EXTENDED		= 200,
			NODE_MULTI			= 300,
			NODE_MULTI_EX		= 400,
			LEAF_MULTI			= 500,
			TRI_STATE			= 600,
			END					= 700
		}

		public enum SelectSecondClass
		{
			UNSET = 0,
			NODES_ONLY,
			NODES_AND_LEAVES,
			LEAVES_ONLY
		}

		public enum SelectTreeAllowed
		{
			UNSET,
			YES,
			NO
		}

		// order matters - 
		public enum SelectMode
		{
			UNDEFINED			= 0,

			INDIVIDUAL			= NODE_ONLY + 1, // node only, one at a time
			INDIVIDUALPLUS		= NODE_ONLY + 2, // node only, one per time, + select all leaves

			EXTENDED			= NODE_EXTENDED + 1, // select node + all child nodes
			EXTENDEDPLUS		= NODE_EXTENDED + 2, // select node + all child nodes + select all leaves

			MULTISELECTNODE		= NODE_MULTI + 1, // node only, many individual
			MULTISELECTNODEPLUS	= NODE_MULTI + 2, // node only, many individual + select all leaves

			MULTISELECTNODEEX	= NODE_MULTI_EX + 1,   // node only, many individual - ex, selecting a branch, selects the whole branch
			MULTISELECTNODEEXPLUS = NODE_MULTI_EX + 2, // node only, many individual - ex, selecting a branch, selects the whole branch + select leaves


			MULTISELECTLEAF		= LEAF_MULTI + 1, // leaves only, many individual, cannot select nodes

			TRISTATE			= TRI_STATE + 1, // tri state select none, node, node + children
			TRISTATEPLUS		= TRI_STATE + 2, // tri state select none, node, node + children
		}

		public struct SelModeData
		{
			public string Description { get; }
			public SelectFirstClass ClassFirst { get; }
			public SelectSecondClass ClassSecond { get; }
			public SelectTreeAllowed SelectTreeAllowed { get; }

			public SelModeData(string description, 
				SelectFirstClass selFirstClass, 
				SelectSecondClass selSecondClass,
				SelectTreeAllowed selTree
				)
			{
				Description = description;
				ClassFirst = selFirstClass;
				ClassSecond = selSecondClass;
				SelectTreeAllowed = selTree;
			}
		}

		public static Dictionary<SelectMode, SelModeData>
			SelModes = new()
			{
				{ UNDEFINED			     , new SelModeData("Undefined"                  , SelectFirstClass.UNSET, SelectSecondClass.UNSET, SelectTreeAllowed.UNSET) },
				{ INDIVIDUAL		     , new SelModeData("Individual"                 , NODE_ONLY    , NODES_ONLY      , NO) },
				{ INDIVIDUALPLUS         , new SelModeData("IndividualPlus"             , NODE_ONLY    , NODES_AND_LEAVES, NO) },
				{ EXTENDED               , new SelModeData("Extended"                   , NODE_EXTENDED, NODES_ONLY      , YES ) },
				{ EXTENDEDPLUS           , new SelModeData("ExtendedPlus"               , NODE_EXTENDED, NODES_AND_LEAVES, NO) },
				{ TRISTATE               , new SelModeData("TriState"                   , TRI_STATE    , NODES_ONLY      , YES) },
				{ TRISTATEPLUS           , new SelModeData("TriStatePlus"               , TRI_STATE    , NODES_AND_LEAVES, NO) },
				{ MULTISELECTNODE        , new SelModeData("MultiSelectNode"            , NODE_MULTI   , NODES_ONLY      , YES) },
				{ MULTISELECTNODEPLUS    , new SelModeData("MultiSelectNodePlus"        , NODE_MULTI   , NODES_AND_LEAVES, YES) },
				{ MULTISELECTNODEEX      , new SelModeData("MultiSelectNodeExtended"    , NODE_MULTI_EX, NODES_ONLY      , YES) },
				{ MULTISELECTNODEEXPLUS  , new SelModeData("MultiSelectNodeExtendedPlus", NODE_MULTI_EX, NODES_AND_LEAVES, NO) },
				{ MULTISELECTLEAF        , new SelModeData("MutliSelectLeaf"            , LEAF_MULTI   , LEAVES_ONLY     , NO) },
			};
	}

	public static class TreeClassEx<TNd, TLd>
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
		public static string GetNodePath(TreeClassNode<TNd, TLd> node)
		{
			StringBuilder sb = new StringBuilder();
			TreeClassNode<TNd, TLd> parent = node.ParentNodeEx;

			List<TreeClassNode<TNd, TLd>> parentNodes = GetParentNodes(node);

			for (var i = parentNodes.Count - 1; i >= 0; i--)
			{
				sb.Append(parentNodes[i].NodeKey).Append(">");
			}

			sb.Append(node.NodeKey);

			return sb.ToString();
		}

		public static List<TreeClassNode<TNd, TLd>> GetParentNodes(TreeClassNode<TNd, TLd> node)
		{
			List<TreeClassNode<TNd, TLd>> parents = new List<TreeClassNode<TNd, TLd>>();

			TreeClassNode<TNd, TLd> parent = node.ParentNodeEx;

			while (parent != null)
			{
				parents.Add(parent);

				parent = parent.ParentNodeEx;
			}

			return parents;
		}
	}

#endregion

#region interfaces

	public interface ITreeNode<TNd, TLd>
		where TNd : class
		where TLd : class
	{
		public string NodeKey { get; }
		public TreeClassNode<TNd, TLd> ParentNode { get; }
		public TreeClass<TNd, TLd> Tree { get; }
	}

	public interface ITreeNodeEx<TNd, TLd> : ITreeNode<TNd, TLd>
		where TNd : class
		where TLd : class
	{
		public bool? IsChecked { get; }
		public bool HasNodes { get; }
		public int CountNodes { get; }
		public bool HasLeaves { get; }
		public int CountLeaves { get; }
	}

#endregion

#region enumerators

	public class TreeClassEnumerator<TNd, TLd> : IEnumerator<TreeClassNode<TNd, TLd>>
		where TNd : class
		where TLd : class
	{
		private readonly TreeClass<TNd, TLd> tree;
		private List<TreeClassNode<TNd, TLd>>? currNodes;

		private List<IEnumerator<KeyValuePair<string, TreeClassNode<TNd, TLd>>>> ies;

		public TreeClassEnumerator() { }

		public TreeClassEnumerator(TreeClass<TNd, TLd> tree)
		{
			this.tree = tree;

			Reset();
		}


		public TreeClassNode<TNd, TLd> Current
		{
			get
			{
				if (currNodes == null) return null;

				return currNodes[^1];
			}
		}


		public List<TreeClassNode<TNd, TLd>> CurrentPath
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
				TreeClassNode<TNd, TLd> node = ies[^1].Current.Value;

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
			currNodes = new List<TreeClassNode<TNd, TLd>>();

			ies = new List<IEnumerator<KeyValuePair<string, TreeClassNode<TNd, TLd>>>>();

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

	public class TreeNodeEnumerator<TNd, TLd> : IEnumerator<TreeClassNode<TNd, TLd>>
		where TNd : class
		where TLd : class
	{
		private readonly TreeClassNode<TNd, TLd> node;
		private List<TreeClassNode<TNd, TLd>>? currNodes;

		private List<IEnumerator<KeyValuePair<string, TreeClassNode<TNd, TLd>>>> ies;

		public TreeNodeEnumerator() { }

		public TreeNodeEnumerator(TreeClassNode<TNd, TLd> node)
		{
			this.node = node;

			Reset();
		}


		public TreeClassNode<TNd, TLd> Current
		{
			get
			{
				if (currNodes == null) return null;

				return currNodes[^1];
			}
		}


		public List<TreeClassNode<TNd, TLd>> CurrentPath
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
				TreeClassNode<TNd, TLd> node = ies[^1].Current.Value;

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
			currNodes = new List<TreeClassNode<TNd, TLd>>();
			ies = new List<IEnumerator<KeyValuePair<string, TreeClassNode<TNd, TLd>>>>();

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

#endregion

#endregion
}