#region using

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
using SharedWPF.ShWin;
using UtilityLibrary;
using WpfProject02.Annotations;
using static SharedCode.SampleData.Tree;
using static SharedCode.SampleData.Selection;
using static SharedCode.SampleData.Selection.SelectMode;
using static SharedCode.SampleData.Selection.SelectClass;
using static SharedCode.SampleData.Selection.CheckedState;

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


namespace SharedCode.SampleData
{
#region treeclass system

#region shortcuts

	public class Tree : TreeClass<TreeNodeData, TreeLeafData>
	{
		public Tree(string treeName, SelectMode mode, TreeNodeData nodeData = null) :
			base(treeName, mode, nodeData) { }
	}

	public class TreeNode : TreeClassNode<TreeNodeData, TreeLeafData>, IEnumerable<TreeNode>
	{
		public TreeNode( string nodeKey,
			TreeClassNode<TreeNodeData, TreeLeafData>? parentNode,
			Tree? tree, TreeNodeData? nodeData) :
			base(nodeKey, tree, (TreeNode) parentNode, nodeData) { }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new TreeNodeEnumerator(this);
		}

		public IEnumerator<TreeNode> GetEnumerator()
		{
			return new TreeNodeEnumerator(this);
		}
	}

	public class TreeLeaf : TreeClassLeaf<TreeLeafData>
	{
		public TreeLeaf(string leafKey, TreeLeafData leafData,
			ITreeNodeEx parentNode = null) : base(leafKey, leafData, parentNode) { }
	}

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
	

	TreeNode:









									   
	*/

#region Tree Classes

	public class TreeClass<TNd, TLd> : INotifyPropertyChanged,
		IEnumerable<TreeNode>
		where TNd : class
		where TLd : class
	{
	#region private fields

		private TreeNode rootNode;

		// represents the last selected (or deselected) node - since
		// multiple nodes / leaves can be selected
		// private TreeNode? selectedNode;

		// represents the node currently being used for node operations
		// there must always be a current node
		// can and does change during most node operations 
		// can only be stable if no other node operations occur
		private TreeNode? currentNode;

		// represents a temporary node that remains stable during
		// multi-step operations
		// important that this only get used for multi-step
		// but single subject operations
		private TreeNode? tempNode;

		// represents a node found during search operations
		// such as ContainsNode
		private TreeNode? foundNode;

		private TreeLeaf? foundLeaf;


		// private bool isTriState;
		private bool requireUniqueKeys;

		private string leafToFind;
		private List<TreeLeaf> foundLeaves;
		private int foundLeafIdx = -1;

		private string nodeToFind;
		private List<TreeNode> foundNodes;
		private int foundNodeIdx = -1;

		// selection

		private SelectMode selectMode = INDIVIDUAL;
		private int selectClass = (int) NODE_ONLY;
		private List<TreeNode> selectedNodes;
		private List<TreeNode> priorSelectedNodes;

		// .................................check state		  mixed >	checked >	unchecked >	mixed
		//														0			1			2			3 (0)
		public static readonly bool?[] boolList = new bool?[] { null,		true,		false,		null };

	#endregion

	#region ctor

		public TreeClass() { }

		public TreeClass(string treeName, SelectMode mode, TNd nodeData = null)
		{
			TreeName = treeName;
			NodeData = nodeData;
			SelectMode = mode;

			makeRootNode();

			selectedNodes = new List<TreeNode>();
			priorSelectedNodes = new List<TreeNode>();
		}

	#endregion

	#region public properties

		// basic data

		public string TreeName { get; set; }

		// public TNd NodeData { get; set; }

		public TreeNode RootNode => rootNode;

		public TreeNode CurrentNode
		{
			get => currentNode;
			set
			{
				if (Equals(value, currentNode)) return;
				currentNode = value;
				OnPropertyChanged();
			}
		}

		public TreeNode FoundNode
		{
			get => foundNode;
			set
			{
				if (Equals(value, foundNode)) return;
				foundNode = value;
				OnPropertyChanged();
			}
		}

		public TreeLeaf FoundLeaf
		{
			get => foundLeaf;
			set
			{
				if (Equals(foundLeaf, value)) return;
				foundLeaf = value;
				OnPropertyChanged();
			}
		}

		public List<TreeNode> FoundNodes => foundNodes;
		public List<TreeLeaf> FoundLeaves => foundLeaves;

		// selection
		// most selection login is in the node class

		public SelectMode SelectMode
		{
			get => selectMode;
			private set
			{
				selectMode = value;
				SetSelectClass();
			}
		}

		public List<TreeNode> SelectedNodes
		{
			get => selectedNodes;
			private set
			{
				selectedNodes = value;
				OnPropertyChanged();
			}
		}

		public List<TreeNode> PriorSelectedNodes
		{
			get => priorSelectedNodes;
			private set
			{
				priorSelectedNodes = value;
				OnPropertyChanged();
			}
		}

		public int SelectClass
		{
			get => selectClass;
			private set
			{
				selectClass = value;
				OnPropertyChanged();
			}
		}

		// public bool MultiSelectionAllowed
		// {
		// 	get => multiSelectionAllowed;
		// 	private set
		// 	{
		// 		multiSelectionAllowed = value;
		// 	}
		// }

		// unique

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

		// public bool IsTriState
		// {
		// 	get => isTriState;
		// 	private set
		// 	{
		// 		if (value == isTriState) return;
		// 		isTriState = value;
		// 		OnPropertyChanged();
		// 	}
		// }

		// public int CountLeavesSelectedEx => selectedNode?.LeafCountBranch() ?? 0;
		// public int CountNodesSelectedEx => selectedNode?.NodeCountBranch() ?? 0;
		// counts - attached directly to the node
		// public int CountNodesSelected => selectedNode?.CountNodes ?? 0;
		// public int CountLeavesSelected => selectedNode?.CountLeaves ?? 0;
		// extended - from the node down

	#endregion

	#region private properties

	#endregion

	#region public methods

	#region selection methods

		private void OnPropUpdateSelectedNodes()
		{
			OnPropertyChanged(nameof(SelectedNodes));
			OnPropertyChanged(nameof(PriorSelectedNodes));
		}

		private void SetSelectClass()
		{
			int selClass = (int) selectMode;

			if ( selClass < Selection.SelectClass.NODE_EXTENDED.AsInt())
			{
				SelectClass = NODE_ONLY.AsInt();
			}
			else if  (selClass < Selection.SelectClass.NODE_MULTI.AsInt())
			{
				SelectClass = NODE_EXTENDED.AsInt();
			}
			else if ( selClass < Selection.SelectClass.LEAF_MULTI.AsInt())
			{
				SelectClass = NODE_MULTI.AsInt();
			}
			else if ( selClass < Selection.SelectClass.TRI_STATE.AsInt())
			{
				SelectClass = LEAF_MULTI.AsInt();
			}
			else
			{
				SelectClass = TRI_STATE.AsInt();
			}
		}

		public void UpdateSelected(TreeNode node)
		{
			switch (SelectClass)
			{
			case (int) NODE_ONLY:
				{
					Examples.M.WriteLine("10 0A doing indiv select");
					updateSelectedIndividual(node);
					break;
				}
			case (int) NODE_EXTENDED:
				{
					Examples.M.WriteLine(" 100 0A doing extended select");
					if (!updateSelectedExtended(node)) return;
					break;
				}
			case (int) NODE_MULTI:
				{
					Examples.M.WriteLine(" 200 0A doing multi-node select");
					selectedNodes.Add(node);
					break;
				}
			case (int) LEAF_MULTI:
				{
					break;
				}
			case (int) TRI_STATE:
				{
					break;
				}
			}

			RaiseNodeSelectedEvent(node);
			OnPropUpdateSelectedNodes();
		}

		public void UpdateDeSelected(TreeNode node)
		{
			switch (SelectClass)
			{
			case (int) NODE_ONLY:
				{
					Examples.M.WriteLine("-10 0A doing indiv deselect");
					// may not remove return here to prevent 
					// event / update
					if (!UpdateDeselectedIndividual(node)) return;
					break;
				}
			case (int) NODE_EXTENDED:
				{
					Examples.M.WriteLine("-100 0A doing extended deselect");
					// may not remove return here to prevent 
					// event / update
					if (!UpdateDeselectedExtended(node)) return;
					break;
				}
			case (int) NODE_MULTI:
				{
					Examples.M.WriteLine("-200 0A doing multi-node deselect");
					selectedNodes.Remove(node);
					priorSelectedNodes.Clear();
					priorSelectedNodes.Add(node);
					break;
				}
			case (int) LEAF_MULTI:
				{
					break;
				}
			case (int) TRI_STATE:
				{
					break;
				}
			}

			RaiseNodeDeSelectedEvent(node);
			OnPropUpdateSelectedNodes();
		}

		public void UpdateMixedSelected (TreeNode node)
		{

		}

		private void updateSelectedIndividual(TreeNode node)
		{
			// only one node selected at a time
			// will always add a node
			if (selectedNodes.Count == 0)
			{
				selectedNodes.Add(node);
			}
			else
			{
				priorSelectedNodes = selectedNodes;
				priorSelectedNodes[0].DeSelectNode();
				selectedNodes = new List<TreeNode> { node };

				RaiseNodeDeSelectedEvent(priorSelectedNodes[0]);
			}
		}

		private bool UpdateDeselectedIndividual(TreeNode node)
		{
			selectedNodes.Remove(node);

			return true;
		}
		
		private bool updateSelectedExtended(TreeNode node)
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
				Examples.M.WriteLine(" 100 3A extended select| case 3| node in a different branch selected");
				Examples.M.WriteLine(" 100 4A extended select| case 4| or node closer to root in the tree selected");

				// procedure:
				// deselect branch
				// select branch

				deselectBranch(selectedNodes[0]);
				selectBranch(node);
			}

			return true;
		}


		private bool UpdateDeselectedExtended(TreeNode node)
		{
			// case 2
			// remove branch - one of the selected nodes
			// was deselected
			Examples.M.WriteLine("-100 0A extended select| deselect");

			bool result = deselectBranch(node);

			return result;
		}

		private bool selectBranch(TreeNode node)
		{
			// case 1 - nothing selected before
			Examples.M.WriteLine(" 100 1A extended select| case 1| nothing selected before");

			if (!AddNodeToSelected(node)) return false;

			RaiseNodeSelectedEvent(node);

			// node.SelectNode();
			node.SelectNodeAllBranches();

			return true;
		}

		private bool deselectBranch(TreeNode node)
		{
			// provided node must be in the selected node list
			if (!selectedNodes.Contains(node)) return false;

			// case 2
			// one of the selected nodes was deselected
			// deselect branch
			Examples.M.WriteLine("-100 2A extended select| case 2| deselect child node");

			// node provided may not be the top node
			// need to get to the top node and deselect from there;

			// note that the selected node list cannot be modified
			// while deselecting
			selectedNodes[0].DeSelectNode();
			RaiseNodeDeSelectedEvent(selectedNodes[0]);

			selectedNodes[0].DeSelectNodeAllBranches();

			// once all deselected, update lists
			PriorSelectedNodes = selectedNodes;
			SelectedNodes = new List<TreeNode>();

			return true;
		}


		public bool AddNodeToSelected(TreeNode node)
		{
			if (selectedNodes.Contains(node)) return false;
			selectedNodes.Add(node);

			return true;
		}

		public bool RemoveFromSelected(TreeNode node)
		{
			if (!selectedNodes.Contains(node)) return false;

			selectedNodes.Remove(node);

			return true;
		}

		public void ResetSelected()
		{
			priorSelectedNodes = new List<TreeNode>();
			selectedNodes = new List<TreeNode>();

			OnPropUpdateSelectedNodes();
		}

		// select all nodes from the root node down
		public void SelectNodeTree()
		{
			if (rootNode?.Nodes == null) return;

			rootNode.SelectNodeAllBranches();
		}

		// deselect all nodes from the root node down
		public void DeSelectNodeTree()
		{
			if (rootNode?.Nodes == null) return;

			rootNode.DeSelectNodeAllBranches();
		}


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

	#region node methods

		// clear the whole node (and leaves) tree
		public void Clear()
		{
			if (rootNode == null)
			{
				makeRootNode();
				return;
			}

			rootNode.Nodes.Clear();

			if (rootNode.HasLeaves) rootNode.ClearLeaves();

			OnPropertyChanged(nameof(RootNode));
			
			GC.Collect();
		}

		// add methods

		/// <summary>
		/// Add a node (if possible) to the current node or, if provided,<br/>
		/// to the node provided. respects RequireUniqueKeys<br/>
		/// Returns: true->worked / false->failed
		/// </summary>
		public bool AddNode(TreeNode node, TreeNode? addNode = null)
		{
			TreeNode? an;

			an = addNode ?? currentNode;

			if (an == null ||
				!OkToAddNode(node)) return false;

			an.InitNodes();

			// bool result = an.Nodes.TryAdd(node.NodeKey, node);
			bool result = an.TryAddNode(node.NodeKey, node);

			if (result)
			{
				node.ParentNodeEx = an;
				node.Tree = this as Tree;
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
		public bool AddNode(TreeNode newNode, List<string> nodePath)
		{
			bool result;
			TreeNode? node;

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
			TreeNode t;
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
			out TreeNode node,
			TreeNode? startNode = null,
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
		public bool DeleteNode(TreeNode? node)
		{
			// if (node == null) return false;
			//
			// TreeNode parentNode = node.ParentNodeEx;
			//
			// if (parentNode == null) return false; 
			//
			// if (node?.ParentNode == null) return false;
			//
			// return ((TreeNode) node.ParentNode).DeleteNode(node.NodeKey);
			return ((TreeNode) node?.ParentNode)?.DeleteNode(node.NodeKey) ?? false;
		}

		/// <summary>
		/// Delete a node (if possible) based on a node path. the last node<br/>
		/// in the path is the node to delete. a copy of the deleted node is<br/>
		/// returned in the out parameter. Note that all nodes connected to<br/>
		/// deleted node also get deleted.<br/>
		/// Returns: true -> worked / false -> failed
		/// </summary>
		public bool DeleteNode(List<string> nodePath, out TreeNode? node)
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
		public bool MoveNode(TreeNode? sourceNode, TreeNode? destinationNode)
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

			TreeNode? parentNode = (TreeNode) sourceNode.ParentNode;

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
		public bool RenameNode(TreeNode node, string newName)
		{
			if (node == null || newName.IsVoid()) return false;

			TreeNode? parentNode = node.ParentNodeEx;

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
			TreeNode? startNode = null,
			int nth = 1,
			bool firstMatch = false)
		{
			foundNodes = new List<TreeNode>();

			if (findKey.IsVoid()) return -1;

			startNode ??= rootNode;

			foreach (TreeNode node in startNode)
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
			out TreeNode node, int nth)
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
		public int CountMatchingNodes(string nodeKey, TreeNode? startPoint)
		{
			if (nodeKey == null) return -1;

			TreeNode node;

			bool result = ContainsNode(nodeKey, out node, startPoint, 1) ;

			if (!result) return 0;

			return foundNodes.Count;
		}

		// enumerate

		/// <summary>
		/// enumerate through the all of the nodes for every node starting
		/// from startNode (if provided) or RootNode if not provided
		/// </summary>
		public IEnumerable<TreeNode> GetNodes(TreeNode startNode = null)
		{
			TreeNode treeNode = startNode == null ? RootNode : startNode;

			foreach (TreeNode tn in treeNode)
			{
				yield return tn;
			}
		}


		// public bool ContainsNode(string findKey,
		// 	TreeNode? startNode = null,
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
		public bool AddLeaf(TreeLeaf leaf, TreeNode node = null)
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

		public bool DeleteLeaf(TreeNode? node, TreeLeaf? leaf)
		{
			if (leaf == null || leaf.LeafKey.IsVoid()) return false;

			return node?.TryDeleteLeaf(leaf.LeafKey) ?? false;
		}

		// move
		public bool MoveLeaf(TreeNode? source, TreeNode? destination, string leafKey)
		{
			bool result;
			TreeLeaf leaf = null;

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

		public bool RenameLeaf(TreeNode node, string oldKey, string newKey)
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
			out TreeLeaf leaf,
			TreeNode? startNode = null,
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
			TreeNode? startNode = null,
			int nth = 1,
			bool firstMatch = false)
		{
			foundLeaves = new List<TreeLeaf>();

			if (findKey.IsVoid()) return -1;

			startNode ??= rootNode;

			foreach (TreeLeaf l in this.GetLeaves(startNode))
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
			out TreeLeaf leaf, int nth)
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
		public int CountMatchingLeaves(string leafKey, TreeNode? startPoint)
		{
			if (leafKey.IsVoid()) return -1;

			TreeLeaf leaf;

			bool result = ContainsLeaf(leafKey, out leaf, startPoint, 1);

			if (!result) return 0;

			return foundLeaves.Count;
		}

		// enumerate

		/// <summary>
		/// enumerate through the all of the leaves for every node starting
		/// from startNode (if provided) or RootNode if not provided
		/// </summary>
		public IEnumerable<TreeLeaf> GetLeaves(TreeNode startNode = null)
		{
			TreeNode treeNode = startNode == null ? RootNode : startNode;

			foreach (TreeNode tn in treeNode)
			{
				if (!tn.HasLeaves) continue;

				foreach (KeyValuePair<string, TreeLeaf> kvp in tn.Leaves)
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
					TreeNode? startPoint, out TreeLeaf leaf, int nth = 1)
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

					foundLeaves = new List<TreeLeaf>();

					foreach (TreeLeaf l in this.GetLeaves(startPoint))
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

		private void makeRootNode(TNd nodeData)
		{
			rootNode = new TreeNode("ROOT", null, this as Tree, (TreeNodeData) nodeData);
			rootNode.InitNodes();

			currentNode = rootNode;
			// selectedNode = rootNode;
		}

		private bool OkToAddNode(TreeNode node)
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

			IEnumerator<TreeNode> ie = GetEnumerator();

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
		private bool getNodeByPath(List<string>? nodePath, out TreeNode? node)
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

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		// node selected
		public delegate void NodeSelectedEventHandler(object sender, TreeNode node);

		public event Tree.NodeSelectedEventHandler NodeSelected;

		protected virtual void RaiseNodeSelectedEvent(TreeNode node)
		{
			NodeSelected?.Invoke(this, node);
		}

		// node deselected

		public delegate void NodeDeSelectedEventHandler(object sender, TreeNode node);

		public event Tree.NodeDeSelectedEventHandler NodeDeSelected;

		protected virtual void RaiseNodeDeSelectedEvent(TreeNode node)
		{
			NodeDeSelected?.Invoke(this, node);
		}

	#endregion

	#region system overrides

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new TreeClassEnumerator<TNd, TLd>(this);
		}

		public IEnumerator<TreeNode> GetEnumerator()
		{
			return new TreeClassEnumerator<TNd, TLd>(this);
		}

		public override string ToString()
		{
			return $"{nameof(TreeClass<TNd, TLd>)} | name| {TreeName}";
		}

	#endregion
	}

	[WpfProject02.Annotations.NotNull]
	public class TreeClassNode<TNd, TLd> : ITreeNodeEx, INotifyPropertyChanged,
		IComparer<TreeNode>, ICloneable
		where TNd : class
		where TLd : class
	{
		private string? nodeKey;

		private Tree? tree;
		private TreeNode? parentNodeEx;


		private CheckedState isChecked;
		private CheckedState priorIsChecked; // needed for tri-state

		private TNd nodeData = null;

		private TreeNode? tn;
		private bool isExpanded;
		private bool isChosen;

		private TreeLeaf? foundLeaf;
		private TreeNode? foundNode;

		// private static TreeClass<TNd, TLd>? tree;

	#region private fields

	#endregion

	#region ctor

		// ReSharper disable once UnusedMember.Global
		public TreeClassNode() { }

		public TreeClassNode(string nodeKey,
			Tree? tree,
			TreeNode? parentNodeEx,
			TNd nodeData,
			CheckedState isChecked = UNCHECKED)
		{
			this.nodeKey = nodeKey;
			this.nodeData = nodeData;
			this.parentNodeEx = parentNodeEx;
			this.isChecked = isChecked;
			this.tree = tree;
		}

	#endregion

	#region public properties

		// serializable properties
		public ObservableDictionary<string, TreeNode>? Nodes { get; set; }

		public ObservableDictionary<string, TreeLeaf>? Leaves { get; set; }

		public string NodeKey
		{
			get => nodeKey;
			set
			{
				if (value == nodeKey) return;
				nodeKey = value;
				OnPropertyChanged();
			}
		}

		public TreeNode ParentNodeEx
		{
			get => parentNodeEx;
			set
			{
				if (Equals(value, parentNodeEx)) return;
				parentNodeEx = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ParentNode));
			}
		}

		public Tree Tree
		{
			get => tree;
			set => tree = value;
		}

		public TreeLeaf FoundLeaf
		{
			get => foundLeaf;
			set
			{
				if (Equals(foundLeaf, value)) return;
				foundLeaf = value;
				OnPropertyChanged();
			}
		}

		public TreeNode FoundNode
		{
			get => foundNode;
			set
			{
				if (Equals(foundNode, value)) return;
				foundNode = value;
				OnPropertyChanged();
			}
		}

		// not serializable properties
		public ITreeNode ParentNode
		{
			get => parentNodeEx;
			private set
			{
				if (Equals(value, parentNodeEx)) return;
				parentNodeEx = (TreeNode) value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ParentNodeEx));
			}
		}

		public bool HasNodes => CountNodes > 0;
		public int CountNodes => Nodes?.Count ?? 0;
		public int CountNodesEx => NodeCountBranch();

		public bool HasLeaves => CountLeaves > 0;
		public int CountLeaves => Leaves?.Count ?? 0;
		public int CountLeavesEx => LeafCountBranch();

		public TNd NodeData
		{
			get => nodeData;
			set
			{
				if (EqualityComparer<TNd>.Default.Equals(value, nodeData)) return;
				nodeData = value;
				OnPropertyChanged();
			}
		}

		public bool IsNodesNull => Nodes == null;
		public bool IsLeavesNull => Leaves == null;

		// selection 

		public SelectMode SelectMode  => tree?.SelectMode ?? INDIVIDUAL;

		// when the check box "ischecked" is checked
		public bool? IsChecked
		{
			get => Tree.boolList[(int) isChecked];
			set
			{
				isChecked = Tree.StateFromBool(value);
				priorIsChecked = isChecked;
				OnPropertyChanged(nameof(PriorIsChecked));

				if (isChecked == CHECKED)
				{
					tree.UpdateSelected((TreeNode)(ITreeNode)this);
				}
				else if (isChecked ==  UNCHECKED)
				{
					tree.UpdateDeSelected((TreeNode)(ITreeNode)this);
				}
				else
				{
					tree.UpdateMixedSelected((TreeNode)(ITreeNode)this);
				}

				OnPropertyChanged();
			}
		}

		public bool? PriorIsChecked
		{
			get => Tree.boolList[(int) priorIsChecked];
			set
			{
				priorIsChecked = Tree.StateFromBool(value);

				OnPropertyChanged();
			}
		}


		public bool IsChosen
		{
			get => isChosen;
			set
			{
				if (value == isChosen) return;
				isChosen = value;
				OnPropertyChanged();
			}
		}

		public bool IsExpanded
		{
			get => isExpanded;
			set
			{
				if (value == isExpanded) return;
				isExpanded = value;
				OnPropertyChanged();
			}
		}

	#endregion

	#region public methods

		public void SelectNode()
		{
			isChecked = CHECKED;
			OnPropertyChanged(nameof(IsChecked));
		}

		public void DeSelectNode()
		{
			isChecked = UNCHECKED;
			OnPropertyChanged(nameof(IsChecked));
		}

		public void MixedSelectNode()
		{
			isChecked = MIXED;
			OnPropertyChanged(nameof(IsChecked));
		}


		public void NotifyNodesUpdated()
		{
			OnPropertyChanged(nameof(Nodes));
		}

		public void InitNodes()
		{
			if (Nodes == null)
				Nodes
					= new ObservableDictionary<string, TreeNode>();
		}

		public void InitLeaves()
		{
			if (Leaves == null) Leaves = new ObservableDictionary<string, TreeLeaf>();
		}

		public int NodeCountBranch()
		{
			if ((Nodes?.Count ?? 0) == 0) return 0;

			int result = Nodes.Count;

			foreach (KeyValuePair<string, TreeNode> kvp in Nodes)
			{
				result += kvp.Value.NodeCountBranch();
			}

			return result;
		}

		public int LeafCountBranch()
		{
			if ((Nodes?.Count ?? 0) == 0) return 0;
			if ((Leaves?.Count ?? 0) == 0) return 0;

			int result = Leaves.Count;

			foreach (var kvp in Nodes)
			{
				result += kvp.Value.LeafCountBranch();
			}

			return result;
		}

		// node methods

		public void AddNode(string key, TreeNode node)
		{
			Nodes.Add(key, node);

			OnPropertyChanged(nameof(Nodes));
		}

		public bool TryAddNode(string key, TreeNode node)
		{
			if (!Nodes.TryAdd(key, node)) return false;

			OnPropertyChanged(nameof(Nodes));

			return true;
		}

		public bool ContainsNode(string findKey)
		{
			if (findKey.IsVoid()) return false;

			bool result = Nodes?.ContainsKey(findKey) ?? false;

			FoundNode = Nodes.Found.Value;

			return result;
		}

		public bool ContainsNode(string findKey, out TreeNode node)
		{
			node = null;

			if (findKey.IsVoid()) return false;

			bool result = ContainsNode(findKey);

			node = foundNode;

			return result;
		}

		public bool DeleteNode(string nodeKey)
		{
			bool result = Nodes?.Remove(nodeKey) ?? false;

			if (!result) return false;

			OnPropertyChanged(nameof(Nodes));

			return true;
		}

		public bool TryDeleteNode(string nodeKey)
		{
			bool result = ContainsNode(nodeKey);

			if (!result) return false;

			return Nodes.Remove(nodeKey);
		}

		public bool ReplaceKeyNode(string oldkey, string newKey)
		{
			if (oldkey.IsVoid() || newKey.IsVoid()) return false;

			if (!Nodes.ReplaceKey(oldkey, newKey)) return false;

			OnPropertyChanged(nameof(Nodes));

			return true;
		}

		// leaf methods

		public void ClearNodes()
		{
			Nodes.Clear();
			OnPropertyChanged(nameof(Nodes));
		}

		public void ClearLeaves()
		{
			Leaves.Clear();
			OnPropertyChanged(nameof(Leaves));
		}

		public void AddLeaf(string key, TreeLeaf leaf)
		{
			Leaves.Add(key, leaf);

			OnPropertyChanged(nameof(Leaves));
		}

		public bool TryAddLeaf(string key, TreeLeaf leaf)
		{
			if (!Leaves.TryAdd(key, leaf)) return false;

			OnPropertyChanged(nameof(Leaves));

			return true;
		}

		public bool ContainsLeaf(string leafKey)
		{
			if (leafKey.IsVoid() || Leaves == null || Leaves.Count == 0) return false;

			bool result = Leaves.TryGetValue(leafKey, out foundLeaf);

			OnPropertyChanged(nameof(FoundLeaf));

			return result;
		}

		public bool DeleteLeaf(string leafKey)
		{
			bool result = Leaves?.Remove(leafKey) ?? false;

			if (!result) return false;

			OnPropertyChanged(nameof(Leaves));

			return true;
		}

		public bool TryDeleteLeaf(string leafKey)
		{
			bool result = ContainsLeaf(leafKey);

			if (!result) return false;

			return Leaves.Remove(leafKey);
		}

		public bool ReplaceKeyLeaf(string oldkey, string newkey)
		{
			if (oldkey.IsVoid() || newkey.IsVoid()) return false;

			if (!Leaves.ReplaceKey(oldkey, newkey)) return false;

			OnPropertyChanged(nameof(Leaves));

			return true;
		}

	#endregion

	#region selection

		// procedure:
		// loop through all owned nodes and select them
		// then pass down to select all
		public void SelectNodeAllBranches()
		{
			if (Nodes == null || Nodes.Count == 0) return;

			foreach (KeyValuePair<string, TreeNode> kvp in Nodes)
			{
				kvp.Value.SelectNode();
				Tree.AddNodeToSelected(kvp.Value);
				kvp.Value.SelectNodeAllBranches();
			}
		}


		public void SelectMatchingNodeTree() { }

		public void SelectMatchingNodeBranch() { }

		public void DeSelectNodeAllBranches()
		{
			if (Nodes == null || Nodes.Count == 0) return;

			foreach (KeyValuePair<string, TreeNode> kvp in Nodes)
			{
				kvp.Value.DeSelectNode();
				kvp.Value.DeSelectNodeAllBranches();
			}
		}

		public void DeSelectMatchingNodeTree() { }

		public void DeSelectMatchingNodeBranch() { }

	#endregion

	#region system overrides

		public int Compare(TreeNode? x, TreeNode? y)
		{
			return 0;
		}

		public object Clone()
		{
			return null;
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public override string ToString()
		{
			return $"{nameof(TreeClassNode<TNd, TLd>)} | name| {nodeKey} | parent| {parentNodeEx?.NodeKey ?? "no parent"}";
		}

	#endregion
	}

	public class TreeClassLeaf<TLd> : INotifyPropertyChanged, IComparer<TreeLeaf>, ICloneable
		where TLd : class
	{
	#region private fields

		private string leafKey;
		private ITreeNode parentNode;
		private TLd leafData;
		private bool? isSelected;
		private bool isChosen;

	#endregion

	#region ctor

		public TreeClassLeaf() { }

		public TreeClassLeaf(string leafKey, TLd leafData,
			ITreeNode parentNode = null)
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

		public ITreeNode ParentNode
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

		public bool? IsSelected
		{
			get => isSelected;
			private set
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

		public bool MoveParent(TreeNode node)
		{
			if (node == null || !node.ContainsLeaf(leafKey)) return false;

			ParentNode = node;

			return true;
		}

	#endregion

	#region system overrides

		public int Compare(TreeLeaf? x, TreeLeaf? y)
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
			return $"{nameof(TreeClassLeaf<TLd>)} | name| {LeafKey} | parent| {parentNode?.NodeKey ?? "is null"}";
		}

	#endregion
	}

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

		public enum SelectClass
		{
			NODE_ONLY			= 10,
			NODE_EXTENDED		= 100,
			NODE_MULTI			= 200,
			LEAF_MULTI			= 300,
			TRI_STATE			= 400,
			END					= 500
		}

		// order matters - 
		public enum SelectMode
		{
			INDIVIDUAL			= 11, // node only, one at a time
			INDIVIDUALPLUS		= 12, // node only, one per time, + select all leaves

			EXTENDED			= 101, // select node + all child nodes
			EXTENDEDPLUS		= 102, // select node + all child nodes + select all leaves

			MULTISELECTNODE		= 201, // node only, many individual
			MULTISELECTNODEPLUS	= 202, // node only, many individual + select all leaves

			MULTISELECTLEAF		= 301, // leaves only, many individual, cannot select nodes

			TRISTATE			= 401, // tri state select none, node, node + children
			TRISTATEPLUS		= 402, // tri state select none, node, node + children
		}

		public struct SelModeData
		{
			public string Description { get; }

			public SelModeData(string description)
			{
				Description = description;
			}
		}


		public static Dictionary<SelectMode, SelModeData>
			SelModes = new()
			{
				{ INDIVIDUAL			, new SelModeData("Individual"         ) },
				{ INDIVIDUALPLUS     , new SelModeData("IndividualPlus"     ) },
				{ EXTENDED           , new SelModeData("Extended"           ) },
				{ EXTENDEDPLUS       , new SelModeData("ExtendedPlus"       ) },
				{ TRISTATE           , new SelModeData("TriState"           ) },
				{ TRISTATEPLUS       , new SelModeData("TriStatePlus"       ) },
				{ MULTISELECTNODE    , new SelModeData("MultiSelectNode"    ) },
				{ MULTISELECTNODEPLUS, new SelModeData("MultiSelectNodePlus") },
				{ MULTISELECTLEAF    , new SelModeData("MutliSelectLeaf"    ) },
			};
	}

	public static class TreeClassEx
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
		public static string GetNodePath(TreeNode node)
		{
			StringBuilder sb = new StringBuilder();
			TreeNode parent = node.ParentNodeEx;

			List<TreeNode> parentNodes = GetParentNodes(node);

			for (var i = parentNodes.Count - 1; i >= 0; i--)
			{
				sb.Append(parentNodes[i].NodeKey).Append(">");
			}

			sb.Append(node.NodeKey);

			return sb.ToString();
		}

		public static List<TreeNode> GetParentNodes(TreeNode node)
		{
			List<TreeNode> parents = new List<TreeNode>();

			TreeNode parent = node.ParentNodeEx;

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

	public interface ITreeNode
	{
		public string NodeKey { get; }
		public ITreeNode ParentNode { get; }
		public Tree Tree { get; }
	}

	public interface ITreeNodeEx : ITreeNode
	{
		public bool? IsChecked { get; }
		public bool HasNodes { get; }
		public int CountNodes { get; }
		public bool HasLeaves { get; }
		public int CountLeaves { get; }
	}

#endregion

#region enumerators

	public class TreeClassEnumerator<TNd, TLd> : IEnumerator<TreeNode>
		where TNd : class
		where TLd : class
	{
		private readonly TreeClass<TNd, TLd> tree;
		private List<TreeNode>? currNodes;

		private List<IEnumerator<KeyValuePair<string, TreeNode>>> ies;

		public TreeClassEnumerator() { }

		public TreeClassEnumerator(TreeClass<TNd, TLd> tree)
		{
			this.tree = tree;

			Reset();
		}


		public TreeNode Current
		{
			get
			{
				if (currNodes == null) return null;

				return currNodes[^1];
			}
		}


		public List<TreeNode> CurrentPath
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
				TreeNode node = ies[^1].Current.Value;

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
			currNodes = new List<TreeNode>();

			ies = new List<IEnumerator<KeyValuePair<string, TreeNode>>>();

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

	public class TreeNodeEnumerator : IEnumerator<TreeNode>
	{
		private readonly TreeNode node;
		private List<TreeNode>? currNodes;

		private List<IEnumerator<KeyValuePair<string, TreeNode>>> ies;

		public TreeNodeEnumerator() { }

		public TreeNodeEnumerator(TreeNode node)
		{
			this.node = node;

			Reset();
		}


		public TreeNode Current
		{
			get
			{
				if (currNodes == null) return null;

				return currNodes[^1];
			}
		}


		public List<TreeNode> CurrentPath
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
				TreeNode node = ies[^1].Current.Value;

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
			currNodes = new List<TreeNode>();
			ies = new List<IEnumerator<KeyValuePair<string, TreeNode>>>();

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