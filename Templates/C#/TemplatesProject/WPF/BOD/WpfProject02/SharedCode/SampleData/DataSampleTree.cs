#region using

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SharedCode.SampleData;
using UtilityLibrary;
using WpfProject02.Annotations;

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
 * methods / properties needed
 * * for tree
 * added	MaxDepth [Max node depth]
 * added	isTriState
 * added	isSelected
 * added	isExpanded
 * done		Clear (new)
 * added	Count (nodes - whole tree)
 * added	Count (leaves - whole tree)
 * none		select
 * done		enumerate tree
 * MAYBE:
 * Keys
 * Values
 * all nodes must be unique
 * 
 * 
 * * for nodes
 * done		AddNode (in current node / via node key path)
 * none		DeleteNode (in current node / via node key path)
 * none		ContainsNode (via key)
 * none		MoveNode
 * done		Count (branch)
 * none		indexer
 * none		isSelected
 * none		isExpanded
 * none		isTriState
 * none		selection communication
 * 
 * * for leafs
 * done		AddLeaf (in current node / via node key path)
 * none		DeleteLeaf (in current node / via node key path)
 * none		ContainsLeaf (via Leaf)
 * none		MoveLeaf (from node to node)
 * none		Count (node)
 * none		enumerate (node / whole tree)
 * none		?? indexer ?? (I think not - does not make sense)
 * none		isSelected
 * none		isExpanded
 * selection communication
 * MAYBE
 * Keys
 * Values
 * 
 */


namespace SharedCode.SampleData
{
#region user side

	public class Tree : TreeClass<TreeNodeData, TreeLeafData>
	{
		public Tree(string treeName, TreeNodeData nodeData = null) : base(treeName, nodeData) { }
	}

	public class TreeNode : TreeClassNode<TreeNodeData, TreeLeafData>
	{
		public TreeNode(string nodeName, TreeNodeData nodeData, TreeClassNode<TreeNodeData, TreeLeafData> parentNode = null) : base(nodeName, nodeData, parentNode) { }
	}

	public class TreeLeaf : TreeClassLeaf<TreeLeafData>
	{
		public TreeLeaf(string leafName, TreeLeafData leafData, ITreeNodeBase parentNodeBase = null) : base(leafName, leafData, parentNodeBase) { }
	}

	public class TreeEnum : TreeClassEnumerator<TreeNodeData, TreeLeafData>
	{
		public TreeEnum(TreeClass<TreeNodeData, TreeLeafData> tree) : base(tree) { }
	}

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

	public class TreeClass<TNd, TLd> : INotifyPropertyChanged, IEnumerable<TreeClassNode<TNd, TLd>>
		//, IEnumerable<KeyValuePair<String, TreeClassNode<TNd, TLd>>>
		where TNd : class
		where TLd : class
	{
	#region private fields

		private TreeClassNode<TNd, TLd> rootNode;

		// represents the primary selected node - since
		// multiple nodes / leaves can be selected
		private TreeClassNode<TNd, TLd> selectedNode;

		// represents the node currently being used for node operations
		// there must always be a current node
		private TreeClassNode<TNd, TLd> currentNode;
		private bool isTriState;
		private bool isSelected;

	#endregion

	#region ctor

		public TreeClass() { }

		public TreeClass(string treeName, TNd nodeData = null)
		{
			TreeName = treeName;
			NodeData = nodeData;

			initRootNode();

			currentNode = rootNode;
		}

	#endregion

	#region public properties

		public string TreeName { get; set; }

		public TNd NodeData { get; set; }

		public TreeClassNode<TNd, TLd> RootNode => rootNode;

		public TreeClassNode<TNd, TLd> SelectedNode
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

		public int CountRootNodes => rootNode?.NodeCount ?? 0;
		public int CountCurrentNodes => currentNode?.NodeCount ?? 0;
		public int CountSelectedNodes => selectedNode?.NodeCount ?? 0;

	#endregion

	#region private properties

	#endregion

	#region public methods

	#region tree methods

		public bool ContainsNode(string searchNode)
		{
			currentNode = null;
			 
			foreach (
				KeyValuePair<string, TreeClassNode<TNd, TLd>> kvp in 
					rootNode.Nodes)
			{
				if (kvp.Key.Equals(searchNode))
				{
					currentNode = kvp.Value;
					return true;
				}
			}

			return false;
		}

		public void Clear()
		{
			TreeName = String.Empty;
			NodeData = null;

			rootNode.Nodes.Clear();

			initRootNode();

			GC.Collect();
		}

		public int NodeCountTree()
		{
			return rootNode.NodeCountBranch();
		}

		public int LeafCountTree ()
		{
			return 0;
		}

	#endregion


	#region node methods

		/// <summary>
		/// Add a node (if possible) to the current node<br/>
		/// Returns: true->worked / false->failed (exists)
		/// </summary>
		/// <param name="node">A TreeNode</param>
		/// <returns>true->worked / false-failed (exists)</returns>
		public bool AddNode(TreeClassNode<TNd, TLd> node)
		{
			if (currentNode == null) return false;

			currentNode.InitNodes();

			bool result = currentNode.Nodes.TryAdd(node.NodeName, node);

			if (result) node.ParentNode = currentNode;

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
		public void AddNode(TreeClassNode<TNd, TLd> newNode, string[] nodePath, int idx = 0)
		{
			if (idx == nodePath.Length || nodePath[idx] == null)
			{
				// got to the bottom of the list
				AddNode(newNode);
				return;
			}

			if (idx == 0)
			{
				currentNode = rootNode;
			}

			// node to be used for the next loop
			TreeClassNode<TNd, TLd> node;

			// if the node is found - use it
			bool result = currentNode.Nodes.TryGetValue(nodePath[idx], out node);

			if (!result)
			{
				// does not exist - add a basic node
				// then use this node
				node = new TreeClassNode<TNd, TLd>(nodePath[idx], null);
				node.InitNodes();
				AddNode(node);
			}

			currentNode = node;

			AddNode(newNode, nodePath, idx + 1);
		}

	#endregion

	#region leaf methods

		/// <summary>
		/// Add a leaf to the current node
		/// </summary>
		/// <param name="leaf"></param>
		/// <returns></returns>
		public bool AddLeaf(TreeClassLeaf<TLd> leaf)
		{
			if (currentNode == null) return false;

			currentNode.InitLeaves();

			return currentNode.Leaves.TryAdd(leaf.LeafName, leaf);
		}


	#endregion

	#endregion

	#region private methods

		private void initRootNode()
		{
			rootNode = new TreeClassNode<TNd, TLd>("ROOT", null);
			rootNode.ParentNode = null;
			rootNode.InitNodes();
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
			if (tree.CountRootNodes == 0)
			{
				currNodes = null;
			}
			else
			{
				currNodes = new List<TreeClassNode<TNd, TLd>>();
				currNodes.Add(tree.RootNode.Nodes[0].Value);

				ies = new List<IEnumerator<KeyValuePair<string, TreeClassNode<TNd, TLd>>>>();
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
		public int NodeCount { get; }
		public bool HasLeaves { get; }
		public int LeafCount { get; }
	}

	public class TreeClassNode<TNd, TLd> : ITreeNodeBase, INotifyPropertyChanged, IComparer<TreeClassNode<TNd, TLd>>, ICloneable
		where TNd : class
		where TLd : class
	{
		private string nodeName;

		private TreeClassNode<TNd, TLd> parentNode;
		private bool? isSelected;

		private TNd nodeData = null;

	#region private fields

	#endregion

	#region ctor

		// ReSharper disable once UnusedMember.Global
		public TreeClassNode() { }

		public TreeClassNode(string nodeName,
			TNd nodeData,
			TreeClassNode<TNd, TLd> parentNode = null,
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
		public ObservableDictionary<string, TreeClassNode<TNd, TLd>> Nodes { get; private set; }

		[AllowNull]
		public ObservableDictionary<string, TreeClassLeaf<TLd>> Leaves { get; private set; }

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

		public TreeClassNode<TNd, TLd> ParentNode
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
				parentNode = (TreeClassNode<TNd, TLd>) value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ParentNode));
			}
		}

		public bool HasNodes => NodeCount > 0;
		public int NodeCount => Nodes?.Count ?? 0;

		public bool HasLeaves => LeafCount > 0;
		public int LeafCount => Leaves?.Count ?? 0;

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
			if (Nodes == null) Nodes = new ObservableDictionary<string, TreeClassNode<TNd, TLd>>();
		}

		public void InitLeaves()
		{
			if (Leaves == null) Leaves = new ObservableDictionary<string, TreeClassLeaf<TLd>>();
		}

		public int NodeCountBranch()
		{
			if ((Nodes?.Count ?? 0) == 0) return 0;

			int result = Nodes.Count;

			foreach (KeyValuePair<string, TreeClassNode<TNd, TLd>> kvp in Nodes)
			{
				result += kvp.Value.NodeCountBranch();
			}

			return result;
		}

	#endregion


	#region system overrides

		public int Compare(TreeClassNode<TNd, TLd>? x, TreeClassNode<TNd, TLd>? y)
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

	public class TreeClassLeaf<TLd> : INotifyPropertyChanged, IComparer<TreeClassLeaf<TLd>>, ICloneable
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

		public int Compare(TreeClassLeaf<TLd>? x, TreeClassLeaf<TLd>? y)
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

#endregion

	public class DataSampleTree
	{
	#region private fields

		private Tree tree;

		private TreeNode treeNode;

		private int nodeIdx = -1;

	#endregion

	#region ctor

		public DataSampleTree()
		{
			tree = new Tree("This is a Tri-State Tree");
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
			TestAddNode(new [] { "Alpha", "Beta", "Delta" });
		}

		public void TestAddNode(string[] nodePath)
		{
			treeNode = makeTempTreeNode();

			tree.AddNode(treeNode, nodePath);
		}

		public void TestAddNode(KeyValuePair<string, string[]>[] nodesAndLeaves) { }

		public int TestEnumerator()
		{
			AddTestNodes();

			// TreeEnum te = new TreeEnum(tree);
			//
			// while (te.MoveNext())
			// {
			// 	Debug.WriteLine($"Current| {te.Current.NodeName}");
			// }

			// int result = 0;
			//
			// foreach (TreeClassNode<TreeNodeData, TreeLeafData> tcn in tree)
			// {
			// 	result++;
			//
			// 	Debug.WriteLine($"Current| {tcn.NodeName}");
			// }

			return TestShowTreeNodes();
		}

		
		public int TestShowTreeNodesAndLeaves()
		{
			int result = 0;

			foreach (TreeClassNode<TreeNodeData, TreeLeafData> tcn in tree)
			// foreach (TreeClassNode<TreeNodeData, TreeLeafData> tcn in tree)
			{
				result++;

				Debug.WriteLine(ShowNodeParents(tcn));

				ShowNodeLeaves(tcn);
			}

			return result;
		}

		private void ShowNodeLeaves(TreeClassNode<TreeNodeData, TreeLeafData> node)
		{
			if (node == null || !node.HasLeaves) return;

			foreach (KeyValuePair<string, TreeClassLeaf<TreeLeafData>> kvp 
					in node.Leaves)
			{
				Debug.WriteLine($"\t{kvp.Value.LeafName}");
			}
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

		private string ShowNodeParents(TreeClassNode<TreeNodeData, TreeLeafData> tcn)
		{
			List<string> parents = new List<string>();

			TreeClassNode<TreeNodeData, TreeLeafData> node = tcn;
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


		public void AddTestNodes()
		{
			TestAddNode(new [] { "Alpha", "Beta", "Delta" });			// 1-1-1
			TestAddNode(new [] { "Alpha", "Beta", "Gamma" });			// 1-1-2

			TestAddNode(new [] { "Alpha", "Zeta", "Eta" });				// 1-2-1
			TestAddNode(new [] { "Alpha", "Zeta", "Theta", "iota" });	// 1-2-2-1
			TestAddNode(new [] { "Alpha", "Zeta", "Theta", "kappa" });

			TestAddNode(new [] { "Sigma", "Lambda", "Nu" });
			TestAddNode(new [] { "Sigma", "Lambda", "Nu", "Xi" });
			TestAddNode(new [] { "Sigma", "Lambda", "Nu", "P1" });

			TestAddNode(new [] { "Sigma", "Omega", "Mu" });

			TestAddNode(new [] { "Tau", "Phi", "Rho" });
			TestAddNode(new [] { "Tau", "Phi", "Chi" });
		}

		public void TestAddNodes2()
		{
			TreeNode node;
			TreeLeaf leaf;

			string[] nodePath;

			KeyValuePair<string, string[]>[][] nodesAndLeaves = AddTestNodes2();

			foreach (KeyValuePair<string, string[]>[] kvps in nodesAndLeaves)
			{
				if (kvps == null) break;

				nodePath = new string[5];

				for (int i = 0; i < kvps.Length; i++)
				{
					// key = node (add if needed)
					// value = leaves (add if exists)

					KeyValuePair<string, string[]> kvp = kvps[i];

					bool result = tree.ContainsNode(kvp.Key);

					if (!result)
					{
						node = new TreeNode(kvp.Key, null);

						tree.AddNode(node, nodePath);
					}

					nodePath[i] = kvp.Key;

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

		public KeyValuePair<string, string[]>[][] AddTestNodes2()
		{
			int leafCount = -1;


			KeyValuePair<string, string[]>[][] nodesAndLeaves;

			nodesAndLeaves = new KeyValuePair<string, string[]>[10][];
			
			nodesAndLeaves[0] = new []
			{
				new KeyValuePair<string, string[]>(
					"Alpha", null),
				new KeyValuePair<string, string[]>(
					"Beta",
					new []
					{
						$"Leaf 1-1-{++leafCount:D3}",
						$"Leaf 1-1-{++leafCount:D3}",
						$"Leaf 1-1-{++leafCount:D3}"
					}),
				new KeyValuePair<string, string[]>(
					"Delta",
					new []
					{
						$"Leaf 1-1-1-{++leafCount:D3}"
					}
					)
			};

			
			nodesAndLeaves[1] = new []
			{
				new KeyValuePair<string, string[]>(
					"Alpha", null),
				new KeyValuePair<string, string[]>(
					"Beta", null),
				new KeyValuePair<string, string[]>(
					"Gamma",
					new []
					{
						$"Leaf 1-1-2-{++leafCount:D3}",
						$"Leaf 1-1-2-{++leafCount:D3}"
					}
					)
			};
			
			nodesAndLeaves[2] = new []
			{
				new KeyValuePair<string, string[]>(
					"Alpha", null),
				new KeyValuePair<string, string[]>(
					"Zeta", null),
				new KeyValuePair<string, string[]>(
					"Eta",
					new []
					{
						$"Leaf 1-2-1-{++leafCount:D3}",
						$"Leaf 1-2-1-{++leafCount:D3}",
						$"Leaf 1-2-1-{++leafCount:D3}",
						$"Leaf 1-2-1-{++leafCount:D3}"
					}
					)
			};
			
			nodesAndLeaves[3] = new []
			{
				new KeyValuePair<string, string[]>(
					"Alpha", null),
				new KeyValuePair<string, string[]>(
					"Zeta", null),
				new KeyValuePair<string, string[]>(
					"Theta",
					new []
					{
						$"Leaf 1-2-2-{++leafCount:D3}"
					}
					),
				new KeyValuePair<string, string[]>(
					"Iota", null)
			};

			return nodesAndLeaves;
		}

	#endregion

	#region private methods

		private TreeNode makeTempTreeNode()
		{
			return new TreeNode($"Tree Node {++nodeIdx:D5}", new TreeNodeData($"Node Data {nodeIdx:D5}"));
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
}