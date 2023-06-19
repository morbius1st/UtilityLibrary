#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SharedCode.SampleData;
using SharedWPF.ShWin;
using UtilityLibrary;
using WpfProject02.Annotations;
using static SharedCode.SampleData.Tree;

#endregion

// username: jeffs
// created:  6/10/2023 8:53:05 AM


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
 * added	MaxDepth [Max node depth]
 * added	isTriState
 * added	isSelected
 * added	isExpanded
 * done		Clear (new)
 * done		Count (nodes - whole tree)
 * added	Count (leaves - whole tree)
 * none		select
 * done		enumerate tree
 * MAYBE:
 * Keys
 * Values
 * SETTINGS
 * all nodes must be unique | per tree / per branch
 *		> error options: exception / ignore, return false
 * 
 * * node operations / at the tree level
 * done		AddNode
 * none		DeleteNode 
 * none		ContainsNode (via key)
 * none		MoveNode (between branches)
 * none		MakeCurrent
 * done		Count (branch)
 * none		selection communication
 * none		expansion communication
 *
 * * node operations / at the node level
 * none		AddNode
 * none		DeleteNode
 * none		ContainsNode (via key)
 * none		MoveNode (within the same parent node)
 * none		MakeCurrent
 * none		Select
 * none		indexer
 * none		count (nodes)
 * none		isSelected
 * none		isExpanded
 * none		isTriState
 * none		selection communication
 * none		expansion communication
 * none		AddLeaf
 * none		DeleteLeaf
 * none		ContainsLear
 * none		CountLeaves (in this node)
 * none		CountLeaves (this node and below)
 * none		MoveLeaf (within node)
 *
 * 
 * * leaf operations / at the tree level
 * done		AddLeaf (in current node / via node key path)
 * none		DeleteLeaf (in current node / via node key path)
 * none		ContainsLeaf (via Leaf)
 * none		MoveLeaf (from node to node)
 * none		isSelected
 * none		isExpanded
 * selection communication
 * MAYBE
 * Keys
 * Values
 *
 *
 * * leaf operations / at the leaf level
 *
 */


namespace SharedCode.SampleData
{
#region user side

	public class TreeNodeData : INotifyPropertyChanged
	{
		// represents EXTRA information saved with each node
		// private string name - this is a part of node and is not Extra information
		private string value;

		public TreeNodeData(string value)
		{
			this.value = value;
		}


		public string Value
		{
			get => value;
			set
			{
				if (value == this.value) return;
				this.value = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public override string ToString()
		{
			return $"{nameof(TreeNodeData)}| ({value})";
		}
	}

	public class TreeLeafData : INotifyPropertyChanged
	{
		private string value1;
		private string value2;

		public string Value1
		{
			get => value1;
			set
			{
				if (value == value1) return;
				value1 = value;
				OnPropertyChanged();
			}
		}

		public string Value2
		{
			get => value2;
			set
			{
				if (value == value2) return;
				value2 = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	#region system overrides

		public override string ToString()
		{
			return $"{nameof(TreeLeafData)} | v1| {value1} | v2| {value2}";
		}

	#endregion
	}

#endregion

#region treeclass system

	public class Tree : TreeClass<TreeNodeData, TreeLeafData>
	{
		public Tree(string treeName, TreeNodeData nodeData = null) : 
			base(treeName, nodeData) { }
	}

	public class TreeNode : TreeClassNode<TreeNodeData, TreeLeafData>
	{
		public TreeNode(string nodeName, TreeNodeData nodeData, TreeClassNode<TreeNodeData, TreeLeafData> parentNode = null) : base(nodeName, nodeData, (TreeNode) parentNode) { }
	}

	public class TreeLeaf : TreeClassLeaf<TreeLeafData>
	{
		public TreeLeaf(string leafName, TreeLeafData leafData, ITreeNodeBase parentNodeBase = null) : base(leafName, leafData, parentNodeBase) { }
	}

	public class TreeClass<TNd, TLd> : INotifyPropertyChanged, IEnumerable<TreeNode>
		//, IEnumerable<KeyValuePair<String, TreeClassNode<TNd, TLd>>>
		where TNd : class
		where TLd : class
	{
	#region private fields

		private TreeNode? rootNode;

		// represents the primary selected node - since
		// multiple nodes / leaves can be selected
		private TreeNode? selectedNode;

		// represents the node currently being used for node operations
		// there must always be a current node
		private TreeNode? currentNode;
		private bool isTriState;
		private bool isSelected;

	#endregion

	#region ctor

		public TreeClass() { }

		public TreeClass(string treeName, TNd nodeData = null)
		{
			TreeName = treeName;
			NodeData = nodeData;

			makeRootNode();

			// currentNode = rootNode;
		}

	#endregion

	#region public properties

		public string TreeName { get; set; }

		public TNd NodeData { get; set; }

		public TreeNode RootNode => rootNode;

		public TreeNode SelectedNode
		{
			get => selectedNode;
			set
			{
				if (Equals(value, selectedNode)) return;
				selectedNode = value;
				currentNode = value;

				selectedNode.IsSelected = true;

				OnPropertyChanged();
			}
		}

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


		public int MaxDepth { get; private set; }

		public bool HasLimitedDepth => MaxDepth > 0;

		public bool IsSelected
		{
			get => isSelected;
			set
			{
				if (value == isSelected) return;
				isSelected = value;
				OnPropertyChanged();
			}
		}

		public bool IsTriState
		{
			get => isTriState;
			private set
			{
				if (value == isTriState) return;
				isTriState = value;
				OnPropertyChanged();
			}
		}

		public int CountNodesRoot => rootNode?.CountNodes ?? 0;
		public int CountNodesCurrent => currentNode?.CountNodes ?? 0;
		public int CountNodesSelected => selectedNode?.CountNodes ?? 0;

		public int CountNodesTree => rootNode?.NodeCountBranch() ?? 0;
		public int CountNodesCurrentEx => currentNode?.NodeCountBranch() ?? 0;
		public int CountNodesSelectedEx => selectedNode?.NodeCountBranch() ?? 0;

	#endregion

	#region private properties

	#endregion

	#region public methods

	#region tree methods

		public bool ContainsNode(string findNode, bool currNode = true)
		{
			// currentNode = null;
			//
			// foreach (
			// 	KeyValuePair<string, TreeNode> kvp in
			// 	rootNode.Nodes)
			// {
			// 	if (kvp.Key.Equals(findNode))
			// 	{
			// 		currentNode = kvp.Value;
			// 		return true;
			// 	}
			// }

			if (currNode)
			{
				return containNode(findNode);
			}


			return containsNodeTree(findNode);
		}

		public void Clear()
		{
			if (rootNode == null)
			{
				makeRootNode();
				return;
			}
			
			rootNode.Nodes.Clear();

			if (rootNode.HasLeaves) rootNode.Leaves.Clear();

			// TreeName = String.Empty;
			// NodeData = null;

			// initRootNode();

			GC.Collect();
		}

	#endregion


	#region node methods

		/// <summary>
		/// Add a node (if possible) to the current node<br/>
		/// Returns: true->worked / false->failed (exists)
		/// </summary>
		/// <param name="node">A TreeNode</param>
		/// <returns>true->worked / false-failed (exists)</returns>
		public bool AddNode(TreeNode node)
		{
			if (currentNode == null) return false;

			currentNode.InitNodes();

			bool result = currentNode.Nodes.TryAdd(node.NodeName, node);

			if (result)
			{
				node.ParentNode = currentNode;
				currentNode = node;
			}

			return result;
		}

		/// <summary>
		/// Add a node (if possible) based on the node path<br/>
		/// node path starts from a selected node down
		/// Returns: true->worked / false->failed (exists)
		/// </summary>
		/// <param name="nodePath"></param>
		/// <param name="node"></param>
		/// <returns></returns>
		public void AddNode(TreeNode newNode, List<string> nodePath, int idx = 0)
		{
			if (idx == 0)
			{
				currentNode = rootNode;
				// showNodePath("@B1", newNode.NodeName, nodePath);
			}

			// Debug.WriteLine($"add node| idx| {idx} | path count| {nodePath.Count} | current node | {currentNode?.NodeName ?? "null"}");

			if (idx == nodePath.Count || nodePath[idx] == null)
			{
				// Debug.WriteLine($"\t@bottom of the list| add node| {newNode.NodeName}");

				// got to the bottom of the list
				AddNode(newNode);
				return;
			}

			// node to be used for the next loop
			TreeNode node;

			// if the node is found - use it
			bool result = currentNode.Nodes.TryGetValue(nodePath[idx], out node);

			if (!result)
			{
				// does not exist - add a basic node
				// then use this node
				node = new TreeNode(nodePath[idx], null);
				node.InitNodes();
				result = AddNode(node);

				if (!result) return;
			}

			currentNode = node;

			AddNode(newNode, nodePath, idx + 1);
		}

		private void showNodePath(string location, string nodeName, List<string> nodePath)
		{
			Debug.Write($"node path {location}| ");

			foreach (string s in nodePath)
			{
				Debug.Write($"> {s} ");
			}

			Debug.Write($" | ({nodeName})");

			Debug.Write("\n");
		}

	#endregion

	#region leaf methods

		/// <summary>
		/// Add a leaf to the current node
		/// </summary>
		/// <param name="leaf"></param>
		/// <returns></returns>
		public bool AddLeaf(TreeLeaf leaf)
		{
			if (currentNode == null) return false;

			currentNode.InitLeaves();

			return currentNode.Leaves.TryAdd(leaf.LeafName, leaf);
		}

	#endregion

	#endregion

	#region private methods

		private void makeRootNode()
		{
			rootNode = new TreeNode("ROOT", null);
			rootNode.ParentNode = null;
			rootNode.InitNodes();

			currentNode = rootNode;
		}

		private bool containNode(string findNode)
		{
			currentNode = null;

			foreach (
				KeyValuePair<string, TreeNode> kvp in
				rootNode.Nodes)
			{
				if (kvp.Key.Equals(findNode))
				{
					currentNode = kvp.Value;
					return true;
				}
			}

			return false;
		}

		private bool containsNodeTree(string findNode)
		{
			currentNode = rootNode;

			IEnumerator<TreeNode> ie = GetEnumerator();

			while (ie.MoveNext())
			{
				if (ie.Current.NodeName.Equals(findNode))
				{
					currentNode = ie.Current;
					return true;
				}
			}

			return false;
		}

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

	#endregion

	#region system overrides

		// public IEnumerator<KeyValuePair<string, TreeClassNode<TNd, TLd>>> GetEnumerator()
		// {
		// 	return rootNode.Nodes.GetEnumerator();
		// }
		//
		// IEnumerator IEnumerable.GetEnumerator()
		// {
		// 	return GetEnumerator();
		// }

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
			if (tree.CountNodesRoot == 0)
			{
				currNodes = null;
			}
			else
			{
				currNodes = new List<TreeNode>();
				currNodes.Add(tree.RootNode.Nodes[0].Value);

				ies = new List<IEnumerator<KeyValuePair<string, TreeNode>>>();
				ies.Add(tree.RootNode.Nodes?.GetEnumerator() ?? null);
			}
		}

		object IEnumerator.Current => Current;

		public void Dispose() { }
	}

	public interface ITreeNodeBase
	{
		public string NodeName { get; }
		public ITreeNodeBase ParentNodeBase { get; }
		public bool? IsSelected { get; }
		public bool HasNodes { get; }
		public int CountNodes { get; }
		public bool HasLeaves { get; }
		public int CountLeaves { get; }
	}

	public class TreeClassNode<TNd, TLd> : ITreeNodeBase, INotifyPropertyChanged,
		IComparer<TreeNode>, ICloneable
		where TNd : class
		where TLd : class
	{
		private string nodeName;

		private TreeNode parentNode;
		private bool? isSelected;

		private TNd nodeData = null;

	#region private fields

	#endregion

	#region ctor

		// ReSharper disable once UnusedMember.Global
		public TreeClassNode() { }

		public TreeClassNode(string nodeName,
			TNd nodeData,
			TreeNode parentNode = null,
			bool isSelected = false)
		{
			this.nodeName = nodeName;
			this.nodeData = nodeData;
			this.parentNode = parentNode;
			this.isSelected = isSelected;
		}

	#endregion

	#region public properties

		// serializable properties
		[AllowNull]
		public ObservableDictionary<string, TreeNode> Nodes { get; private set; }

		[AllowNull]
		public ObservableDictionary<string, TreeLeaf> Leaves { get; private set; }

		public string NodeName
		{
			get => nodeName;
			private set
			{
				if (value == nodeName) return;
				nodeName = value;
				OnPropertyChanged();
			}
		}

		public TreeNode ParentNode
		{
			get => parentNode;
			set
			{
				if (Equals(value, parentNode)) return;
				parentNode = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ParentNodeBase));
			}
		}

		public bool? IsSelected
		{
			get => isSelected;
			set
			{
				if (value == isSelected) return;
				isSelected = value;
				OnPropertyChanged();
			}
		}


		// not serializable properties
		public ITreeNodeBase ParentNodeBase
		{
			get => parentNode;
			private set
			{
				if (Equals(value, parentNode)) return;
				parentNode = (TreeNode) value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ParentNode));
			}
		}

		public bool HasNodes => CountNodes > 0;
		public int CountNodes => Nodes?.Count ?? 0;

		public bool HasLeaves => CountLeaves > 0;
		public int CountLeaves => Leaves?.Count ?? 0;

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

	#endregion

	#region public methods

		public void InitNodes()
		{
			if (Nodes == null) Nodes = new ObservableDictionary<string, TreeNode>();
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
			return $"{nameof(TreeClassNode<TNd, TLd>)} | name| {nodeName} | parent| {parentNode.NodeName}";
		}

	#endregion
	}

	public class TreeClassLeaf<TLd> : INotifyPropertyChanged, IComparer<TreeLeaf>, ICloneable
		where TLd : class
	{
		private string leafName;
		private ITreeNodeBase parentNodeBase;
		private TLd leafData;

	#region private fields

	#endregion

	#region ctor

		public TreeClassLeaf() { }

		public TreeClassLeaf(string leafName, TLd leafData,
			ITreeNodeBase parentNodeBase = null)
		{
			LeafName = leafName;
			LeafData = leafData;
			ParentNodeBase = parentNodeBase;
		}

	#endregion

	#region public properties

		public string LeafName
		{
			get => leafName;
			private set
			{
				if (value == leafName) return;
				leafName = value;
				OnPropertyChanged();
			}
		}

		public ITreeNodeBase ParentNodeBase
		{
			get => parentNodeBase;
			private set
			{
				if (Equals(value, parentNodeBase)) return;
				parentNodeBase = value;
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
			return $"{nameof(TreeClassLeaf<TLd>)} | name| {LeafName} | parent| {parentNodeBase.NodeName}";
		}

	#endregion
	}

	// public class Tree : TreeClass<TreeNodeData, TreeLeafData>
	// {
	// 	public Tree(string treeName, TreeNodeData nodeData = null) : base(treeName, nodeData) { }
	// }

#endregion
}