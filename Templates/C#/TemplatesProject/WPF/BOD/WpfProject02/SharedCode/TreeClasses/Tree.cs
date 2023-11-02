#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using SharedWPF.ShWin;
using UtilityLibrary;
using WpfProject02.Annotations;

using static SharedCode.TreeClasses.Selection;

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
	public class Tree<TNd, TLd> :
			ITree,
			INotifyPropertyChanged,
			IEnumerable<TreeNode<TNd, TLd>>
		// ITree<TNd, TLd>,
		where TNd : class
		where TLd : class
	{
	#region private fields

		private TreeRootNode<TNd, TLd>? rootNode;

		// represents the last selected (or deselected) node - since
		// multiple nodes / leaves can be selected
		// private TreeNode<TNd, TLd>? selectedNode;

		// represents the node currently being used for node operations
		// there must always be a current node
		// can and does change during most node operations 
		// can only be stable if no other node operations occur
		private TreeNode<TNd, TLd>? currentNode;

		// represents a temporary node that remains stable during
		// multi-step operations
		// important that this only get used for multi-step
		// but single subject operations
		private TreeNode<TNd, TLd>? tempNode;

		// represents a node found during search operations
		// such as ContainsNode
		private TreeNode<TNd, TLd>? foundNode;

		private TreeLeaf<TNd, TLd>? foundLeaf;


		// private bool isTriState;
		private bool requireUniqueKeys;

		private string? leafToFind = null;
		private List<TreeLeaf<TNd, TLd>>? foundLeaves = null;
		private int foundLeafIdx = -1;

		private string? nodeToFind = null;
		private List<TreeNode<TNd, TLd>>? foundNodes = null;
		private int foundNodeIdx = -1;

		private NodeEnableDisable? nodeEnableDisable = null;

		/*
		// selection

		private SelectMode selectMode = INDIVIDUAL;


		// private SelectFirstClass selectFirstClass = NODE_ONLY;
		private List<TreeNode<TNd, TLd>> selectedNodes;
		private List<TreeNode<TNd, TLd>> priorSelectedNodes;

		private List<TreeLeaf<TNd, TLd>> selectedLeaves;

		private TreeSelectionIndividual selectionIndividualNodes;

		private static SelectedList<ITreeNode> selectedNodes2;
		private static SelectedList<ITreeLeaf> selectedLeaves2;

		private static ISelectedList<ITreeNode> iSelectedNodes;
		private static ISelectedList<ITreeLeaf> iSelectedLeaves;

		private ITreeSelection iTreeSelect;

		*/


		private ATreeSelector? selector = null;

		// .................................check state		  mixed >	checked >	unchecked >	mixed
		//														0			1			2			3 (0)
		public static readonly bool?[] boolList = new bool?[] { null,		true,		false,		null };
		

		#endregion

		#region ctor

		public Tree()
		{
			rootNode = null;
		}

		public Tree(string treeName,
			ATreeSelector selector,
			TNd nodeData = null)
		{
			TreeName = treeName;

			Selector = selector;

			makeRootNode(nodeData);
		}

	#endregion

	#region public properties

		// basic data

		public string TreeName { get; set; }

		public TreeNode<TNd, TLd>? RootNode => rootNode;

		public ITreeNode  IRootNode => (ITreeNode) rootNode!;

		public TreeNode<TNd, TLd> CurrentNode
		{
			get => currentNode;
			set
			{
				if (Equals(value, currentNode)) return;
				currentNode = value;
				OnPropertyChanged();
			}
		}

		public TreeNode<TNd, TLd> FoundNode
		{
			get => foundNode;
			set
			{
				if (Equals(value, foundNode)) return;
				foundNode = value;
				OnPropertyChanged();
			}
		}

		public TreeLeaf<TNd, TLd> FoundLeaf
		{
			get => foundLeaf;
			set
			{
				if (Equals(foundLeaf, value)) return;
				foundLeaf = value;
				OnPropertyChanged();
			}
		}

		public List<TreeNode<TNd, TLd>> FoundNodes => foundNodes;
		public List<TreeLeaf<TNd, TLd>> FoundLeaves => foundLeaves;

		public ATreeSelector Selector
		{
			get => selector!;
			private set
			{
				if (value == null)
				{
					throw new NullReferenceException();
				}

				selector = value;
				selector.Tree = this;
				OnPropertyChanged();
			}
		}

		// selection

		#region settings

		// public bool CanSelectTree =>	true;	//SelectTreeAllowed == SelectTreeAllowed.YES;
		// public bool SelectLeaves =>		true;	//SelectSecondClass == NODES_AND_LEAVES;

		/*
		// selection

		// one time settings / set upon creation / property change not needed (i think)

		public SelectMode SelectMode
		{
			get => selectMode;
			private set { selectMode = value; }
		}

		public SelectFirstClass SelectFirstClass => SelModes[selectMode].ClassFirst;
		public SelectSecondClass SelectSecondClass => SelModes[selectMode].ClassSecond;
		public SelectTreeAllowed SelectTreeAllowed => SelModes[selectMode].SelectTreeAllowed;

		public  SelectedList<ITreeNode> SelectedNodes2 => selectedNodes2;
		public  SelectedList<ITreeLeaf> SelectedLeaf2 => selectedLeaves2;
		
		*/


		public bool IsTriState => selector!.IsTriState;
		// enable-disable

		public NodeEnableDisable NodeEnableDisable
		{
			get => nodeEnableDisable;
			set
			{
				nodeEnableDisable = value;
				OnPropertyChanged();
			}
		}

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

		public static SelectedState StateFromBool(bool? test)
		{
			return (SelectedState) IndexFromBool(test);
		}

		public static SelectedState StateFromIndex(int idx)
		{
			return (SelectedState) idx;
		}

	#endregion


	#region tree methods

		// clear the whole node (and leaves) tree
		/// <summary>
		/// clear & reset the tree & associated values & settings
		/// </summary>
		public void Clear()
		{
			rootNode = null;
			makeRootNode(null);

			OnPropertyChanged(nameof(RootNode));

			GC.Collect();

			CurrentNode = null;
			FoundNode = null;
			RequireUniqueKeys = false;
			TreeName = null;

			if (selector!=null) selector.Clear();
		}

	#endregion


	#region node methods

		// add methods

		/// <summary>
		/// Add a node (if possible) to the current node or, if provided,<br/>
		/// to the node provided. respects RequireUniqueKeys<br/>
		/// Returns: true->worked / false->failed
		/// </summary>
		public bool AddNode(TreeNode<TNd, TLd> node, TreeNode<TNd, TLd>? addNode = null)
		{
			TreeNode<TNd, TLd>? an;

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
		public bool AddNode(TreeNode<TNd, TLd> newNode, List<string> nodePath)
		{
			bool result;
			TreeNode<TNd, TLd>? node;

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
			TreeNode<TNd, TLd> t;
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
			out TreeNode<TNd, TLd> node,
			TreeNode<TNd, TLd>? startNode = null,
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
		public bool DeleteNode(TreeNode<TNd, TLd>? node)
		{
			// if (node == null) return false;
			//
			// TreeNode<TNd, TLd> parentNode = node.ParentNodeEx;
			//
			// if (parentNode == null) return false; 
			//
			// if (node?.ParentNode == null) return false;
			//
			// return ((TreeNode<TNd, TLd>) node.ParentNode).DeleteNode(node.NodeKey);
			return ((TreeNode<TNd, TLd>) node?.ParentNode)?.DeleteNode(node.NodeKey) ?? false;
		}

		/// <summary>
		/// Delete a node (if possible) based on a node path. the last node<br/>
		/// in the path is the node to delete. a copy of the deleted node is<br/>
		/// returned in the out parameter. Note that all nodes connected to<br/>
		/// deleted node also get deleted.<br/>
		/// Returns: true -> worked / false -> failed
		/// </summary>
		public bool DeleteNode(List<string> nodePath, out TreeNode<TNd, TLd>? node)
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
		public bool MoveNode(TreeNode<TNd, TLd>? sourceNode, TreeNode<TNd, TLd>? destinationNode)
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

			TreeNode<TNd, TLd>? parentNode = (TreeNode<TNd, TLd>) sourceNode.ParentNode;

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
		public bool RenameNode(TreeNode<TNd, TLd> node, string newName)
		{
			if (node == null || newName.IsVoid()) return false;

			TreeNode<TNd, TLd>? parentNode = node.ParentNodeEx;

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
			TreeNode<TNd, TLd>? startNode = null,
			int nth = 1,
			bool firstMatch = false)
		{
			foundNodes = new List<TreeNode<TNd, TLd>>();

			if (findKey.IsVoid()) return -1;

			startNode ??= rootNode;

			foreach (TreeNode<TNd, TLd> node in startNode)
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
			out TreeNode<TNd, TLd> node, int nth)
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
		public int CountMatchingNodes(string nodeKey, TreeNode<TNd, TLd>? startPoint)
		{
			if (nodeKey == null) return -1;

			TreeNode<TNd, TLd> node;

			bool result = ContainsNode(nodeKey, out node, startPoint, 1) ;

			if (!result) return 0;

			return foundNodes.Count;
		}

		// enumerate

		/// <summary>
		/// enumerate through the all of the nodes for every node starting
		/// from startNode (if provided) or RootNode if not provided
		/// </summary>
		public IEnumerable<TreeNode<TNd, TLd>> GetNodes(TreeNode<TNd, TLd> startNode = null)
		{
			TreeNode<TNd, TLd> treeNode = startNode == null ? RootNode : startNode;

			foreach (TreeNode<TNd, TLd> tn in treeNode)
			{
				yield return tn;
			}
		}


		// public bool ContainsNode(string findKey,
		// 	TreeNode<TNd, TLd>? startNode = null,
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
		public bool AddLeaf(TreeLeaf<TNd, TLd> leaf, TreeNode<TNd, TLd> node = null)
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
		/// <summary>
		/// Delete a leaf from the current node
		/// </summary>
		public bool DeleteLeaf(TreeNode<TNd, TLd>? node, TreeLeaf<TNd, TLd>? leaf)
		{
			if (leaf == null || leaf.LeafKey.IsVoid()) return false;

			return node?.TryDeleteLeaf(leaf.LeafKey) ?? false;
		}

		// move
		/// <summary>
		/// move a leaf from one node to another node
		/// </summary>
		public bool MoveLeaf(TreeNode<TNd, TLd>? source, TreeNode<TNd, TLd>? destination, string leafKey)
		{
			bool result;
			TreeLeaf<TNd, TLd> leaf = null;

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

		/// <summary>
		/// rename a leaf
		/// </summary>
		public bool RenameLeaf(TreeNode<TNd, TLd> node, string oldKey, string newKey)
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
			out TreeLeaf<TNd, TLd> leaf,
			TreeNode<TNd, TLd>? startNode = null,
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
			TreeNode<TNd, TLd>? startNode = null,
			int nth = 1,
			bool firstMatch = false)
		{
			foundLeaves = new List<TreeLeaf<TNd, TLd>>();

			if (findKey.IsVoid()) return -1;

			startNode ??= rootNode;

			foreach (TreeLeaf<TNd, TLd> l in this.GetLeaves(startNode))
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
			out TreeLeaf<TNd, TLd> leaf, int nth)
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
		public int CountMatchingLeaves(string leafKey, TreeNode<TNd, TLd>? startPoint)
		{
			if (leafKey.IsVoid()) return -1;

			TreeLeaf<TNd, TLd> leaf;

			bool result = ContainsLeaf(leafKey, out leaf, startPoint, 1);

			if (!result) return 0;

			return foundLeaves.Count;
		}

		// enumerate

		/// <summary>
		/// enumerate through the all of the leaves for every node starting
		/// from startNode (if provided) or RootNode if not provided
		/// </summary>
		public IEnumerable<TreeLeaf<TNd, TLd>> GetLeaves(TreeNode<TNd, TLd> startNode = null)
		{
			TreeNode<TNd, TLd> treeNode = startNode == null ? RootNode : startNode;

			foreach (TreeNode<TNd, TLd> tn in treeNode)
			{
				if (!tn.HasLeaves) continue;

				foreach (KeyValuePair<string, TreeLeaf<TNd, TLd>> kvp in tn.Leaves)
				{
					yield return kvp.Value;
				}
			}
		}

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
				new TreeRootNode<TNd, TLd>("ROOT", this, null, nodeData);
			rootNode.InitNodes();

			currentNode = rootNode;
		}

		private bool OkToAddNode(TreeNode<TNd, TLd> node)
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

			IEnumerator<TreeNode<TNd, TLd>> ie = GetEnumerator();

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
		private bool getNodeByPath(List<string>? nodePath, out TreeNode<TNd, TLd>? node)
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

	#endregion

	#region system overrides

		public IEnumerable<ITreeNode> EnumAllNodes()
		{
			if (rootNode.HasNodes)
			{
				foreach (TreeNode<TNd, TLd> node in rootNode)
				{
					yield return (ITreeNode) node;
				}
			}
		}

		public IEnumerable<ITreeLeaf> EnumAllLeaves()
		{
			if (rootNode.HasNodes)
			{
				foreach (TreeNode<TNd, TLd> node in rootNode)
				{
					foreach (TreeLeaf<TNd, TLd> leaf in node.GetLeaves())
					{
						yield return (ITreeLeaf) leaf;
					}
				}
			}
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			return new TreeEnumerator<TNd, TLd>(this);
		}

		public IEnumerator<TreeNode<TNd, TLd>> GetEnumerator()
		{
			return new TreeEnumerator<TNd, TLd>(this);
		}

		public override string ToString()
		{
			return $"{nameof(Tree<TNd, TLd>)} | name| {TreeName}";
		}

	#endregion
	}


	/*

	// node mixed 

	public delegate void NodeMixedEventHandler(object sender, TreeNode<TNd, TLd> node);

	public event NodeMixedEventHandler NodeMixed;

	[DebuggerStepThrough]
	protected virtual void RaiseNodeMixedEvent(TreeNode<TNd, TLd> node)
	{
		NodeMixed?.Invoke(this, node);
	}

	// leaf selected

	public delegate void LeafSelectedEventHandler(object sender, TreeLeaf<TNd, TLd> leaf);

	public event LeafSelectedEventHandler LeafSelected;

	[DebuggerStepThrough]
	protected virtual void RaiseLeafSelectedEvent(TreeLeaf<TNd, TLd> leaf)
	{
		LeafSelected?.Invoke(this, leaf);
	}

	// leaf deselected

	public delegate void LeafDeSelectedEventHandler(object sender, TreeLeaf<TNd, TLd> leaf);

	public event LeafDeSelectedEventHandler LeafDeSelected;

	[DebuggerStepThrough]
	protected virtual void RaiseLeafDeSelectedEvent(TreeLeaf<TNd, TLd> leaf)
	{
		LeafDeSelected?.Invoke(this, leaf);
	}

	*/


	/*

public TreeSelectionIndividual SelectionIndividualNodes
{
	get => selectionIndividualNodes;
	set
	{
		selectionIndividualNodes = value;
		OnPropertyChanged();
	}
}

public bool ApplyEnableDisable()
{
	if (NodeEnableDisable == null) return false;

	return NodeEnableDisable.Apply();
}


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

public void UpdateSelected(TreeNode<TNd, TLd>? node)
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

	// RaiseNodeSelectedEvent(node);
	OnPropUpdateSelectedNodes();
}

public void UpdateDeSelected(TreeNode<TNd, TLd> node)
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

	// RaiseNodeDeSelectedEvent(node);
	OnPropUpdateSelectedNodes();
}

public void UpdateSelectionTriState(TreeNode<TNd, TLd>? node)
{
	// Examples.M.WriteLine("\n+501 00 doing tri-state");
	// Debug.WriteLine($"\n+501 00 doing tri-state| {node}");

	if (node?.ParentNode != null) node.ParentNode.CheckedChangedFromChild(node.IsSelectedState);  // , node.PriorIsCheckedState  );

	if (node.Nodes != null)
	{
		foreach (KeyValuePair<string, TreeNode<TNd, TLd>> kvp in node.Nodes)
		{
			// Debug.WriteLine($"+500     update children| name| {kvp.Value.NodeKey}");
			kvp.Value.CheckedChangeFromParent(node.IsSelectedState);
		}
	}

	if (node.IsSelectedState == SelectedState.SELECTED)
	{
		AddNodeToSelected(node);
		// RaiseNodeSelectedEvent(node);
	}
	else if (node.IsSelectedState == SelectedState.UNSELECTED)
	{
		RemoveNodeFromSelected(node);
		// RaiseNodeDeSelectedEvent(node);
	}
	else
	{
		RaiseNodeMixedEvent(node);
	}
}

private void updateSelectedIndividual(TreeNode<TNd, TLd> node)
{
	// only one node selected at a time
	// will always add a node

	priorSelectedNodes = selectedNodes;
	// selectedNodes = new List<TreeNode<TNd, TLd>> { node };

	if (priorSelectedNodes.Count > 0)
	{
		priorSelectedNodes[0].DeSelectNode();
		// RaiseNodeDeSelectedEvent(priorSelectedNodes[0]);
	}

	selectedNodes.Add(node);
}

private bool updateDeselectedIndividual(TreeNode<TNd, TLd> node)
{
	selectedNodes.Remove(node);

	// RaiseNodeDeSelectedEvent(node);

	return true;
}

private bool updateSelectedExtended(TreeNode<TNd, TLd> node)
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

private bool updateDeselectedExtended(TreeNode<TNd, TLd> node)
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

private bool selectBranch(TreeNode<TNd, TLd> node)
{
	// case 1 - nothing selected before
	// Examples.M.WriteLine(" 100 1A extended select| case 1| nothing selected before");

	if (!AddNodeToSelected(node)) return false;

	// RaiseNodeSelectedEvent(node);

	// node.SelectNode();
	SelectNodeAllBranches(node);

	return true;
}

private bool deselectBranch(TreeNode<TNd, TLd> node)
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

	List<TreeNode<TNd, TLd>> removed = new() { node };

	removed.AddRange(DeSelectNodeAllBranches(node));

	removeListFromSelected(removed);

	return true;
}


public bool AddNodeToSelected(TreeNode<TNd, TLd> node)
{
	if (node == null) return false;

	RemoveNodeFromPriorSelected(node);

	if (selectedNodes.Contains(node)) return false;

	selectedNodes.Add(node);
	OnPropertyChanged(nameof(SelectedNodes));

	// RaiseNodeSelectedEvent(node);

	if (SelectLeaves) SelectAllLeavesInAllNodes(node);

	return true;
}

public bool RemoveNodeFromSelected(TreeNode<TNd, TLd> node)
{
	if (!selectedNodes.Contains(node)) return false;

	AddNodeToPriorSelected(node);

	selectedNodes.Remove(node);
	OnPropertyChanged(nameof(SelectedNodes));

	// RaiseNodeDeSelectedEvent(node);

	if (SelectLeaves) DeSelectAllLeavesInAllNodes(node);

	return true;
}


// leaf

public bool SelectAllLeavesInAllNodes(TreeNode<TNd, TLd> node)
{
	if (node == null) return false;

	bool result = true;

	foreach (TreeNode<TNd, TLd> n in node)
	{
		result = result && selectAllLeavesInNode(n);
	}

	return result;
}

public bool DeSelectAllLeavesInAllNodes(TreeNode<TNd, TLd> node)
{
	if (node == null) return false;

	bool result = true;

	foreach (TreeNode<TNd, TLd> n in node)
	{
		result = result && deSelectAllLeavesInNode(n);
	}

	return result;
}


private bool selectAllLeavesInNode(TreeNode<TNd, TLd> node)
{
	if (node == null) return false;

	bool result = true;

	foreach (KeyValuePair<string, TreeLeaf<TNd, TLd>> kvp in node.Leaves)
	{
		kvp.Value.SelectLeaf();
		result = result && AddLeafToSelected(kvp.Value);
	}

	return result;
}

private bool deSelectAllLeavesInNode(TreeNode<TNd, TLd> node)
{
	if (node == null) return false;

	bool result = true;

	foreach (KeyValuePair<string, TreeLeaf<TNd, TLd>> kvp in node.Leaves)
	{
		kvp.Value.SelectLeaf();
		result = result && RemoveLeafFromSelected(kvp.Value);
	}

	return result;
}


public bool AddLeafToSelected(TreeLeaf<TNd, TLd> leaf)
{
	if (leaf == null || selectedLeaves.Contains(leaf)) return false;

	// not using prior selected

	selectedLeaves.Add(leaf);
	OnPropertyChanged(nameof(SelectedLeaves));

	RaiseLeafSelectedEvent(leaf);

	return true;
}

public bool RemoveLeafFromSelected(TreeLeaf<TNd, TLd> leaf)
{
	if (leaf == null || !selectedLeaves.Contains(leaf)) return false;

	selectedLeaves.Remove(leaf);
	OnPropertyChanged(nameof(SelectedLeaves));

	RaiseLeafDeSelectedEvent(leaf);

	return true;
}


// private

private bool AddNodeToPriorSelected(TreeNode<TNd, TLd> node)
{
	if (priorSelectedNodes.Contains(node)) return false;
	priorSelectedNodes.Add(node);
	OnPropertyChanged(nameof(PriorSelectedNodes));
	return true;
}

private bool RemoveNodeFromPriorSelected(TreeNode<TNd, TLd> node)
{
	if (!priorSelectedNodes.Contains(node)) return false;
	priorSelectedNodes.Remove(node);
	OnPropUpdateSelectedNodes();
	return true;
}


private void SelectNodeAllBranches(TreeNode<TNd, TLd> node)
{
	if (node.Nodes == null || node.Nodes.Count == 0) return;

	foreach (KeyValuePair<string, TreeNode<TNd, TLd>> kvp in node.Nodes)
	{
		kvp.Value.SelectNode();
		AddNodeToSelected(kvp.Value);
		SelectNodeAllBranches(kvp.Value);
	}
}

private List<TreeNode<TNd, TLd>> DeSelectNodeAllBranches(TreeNode<TNd, TLd> node)
{
	// need to be careful here - cannot modify the selectednode list while deselecting
	// as this will cause conflict issues
	List<TreeNode<TNd, TLd>> removed = new List<TreeNode<TNd, TLd>>();

	if (node.Nodes == null || node.Nodes.Count == 0) return removed;

	// this will deselect all nodes and add them to the
	// prior selected list.
	foreach (KeyValuePair<string, TreeNode<TNd, TLd>> kvp in node.Nodes)
	{
		removed.AddRange(DeSelectNodeAllBranches(kvp.Value));

		kvp.Value.DeSelectNode();
		// AddNodeToPriorSelected(kvp.Value);

		removed.Add(kvp.Value);
	}

	return removed;
}

private void removeListFromSelected(List<TreeNode<TNd, TLd>> removed)
{
	if (removed == null || removed.Count < 1) return;

	foreach (TreeNode<TNd, TLd> node in removed)
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
	priorSelectedNodes = new List<TreeNode<TNd, TLd>>();
	selectedNodes = new List<TreeNode<TNd, TLd>>();

	OnPropUpdateSelectedNodes();
}


// checked state

// private void SetSelectFirstClass()
// {
// 	SelectFirstClass = (SelectFirstClass) ((int) selectMode / 100 * 100);
// }

*/
}