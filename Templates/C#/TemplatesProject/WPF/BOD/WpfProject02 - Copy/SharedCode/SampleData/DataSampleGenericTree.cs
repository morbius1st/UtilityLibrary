#region + Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfProject02.Annotations;

#endregion

// user name: jeffs
// created:   6/7/2023 11:59:33 PM

namespace SharedCode.SampleData
{
	public class TreeClassG :
		TreeClassG<TreeNodeDataG, TreeLeafDictG<TreeLeafG>, TreeNodeG, TreeLeafG> { }

	public class TreeNodeG :
		TreeNodeG<TreeNodeDataG, TreeLeafDictG<TreeLeafG>, TreeLeafG>
	{
		public TreeNodeG() { }

		public TreeNodeG(string nodeName,
			TreeNodeDataG nodeData,
			ITreeNode<TreeNodeDataG, TreeLeafDictG<TreeLeafG>, TreeLeafG> parent = null)
			: base(nodeName, nodeData, parent) { }
	}

	public class TreeLeafCollectionG : TreeLeafDictG<TreeLeafG> { }


	/*
	 * the above are the practical classes
	 * the below are the concept classes
	 */


	// H.1
	public class TreeClassG<TNd, TLc, TN, TL> : INotifyPropertyChanged,
		IEnumerable<KeyValuePair<string, TN>>
		where TNd : class, new()                          // tree node data
		where TLc : class, ITreeLeafCollection<TL>, new() // tree leaf collection
		where TN : class, ITreeNode<TNd, TLc, TL>, new()  // a tree node
		where TL : class, ITreeLeaf, new()                // a tree leaf
	{
		// the actual tree class

		public TNd TreeNodeData { get; private set; }


		private Dictionary<string, TN> nodes =
			new Dictionary<string, TN>();

		public IEnumerator<KeyValuePair<string, TN>> GetEnumerator()
		{
			return nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}


		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


		public override string ToString()
		{
			return $"{nameof(TreeClassG<TNd, TLc, TN, TL>)}| ({nodes?.Count ?? -1})";
		}
	}


	// H.2 - tree node data
	public class TreeNodeDataG : INotifyPropertyChanged
	{
		// represents EXTRA information saved with each node
		// private string name - this is a part of node and is not Extra information
		private string value;

		public TreeNodeDataG() { }

		public TreeNodeDataG(string value)
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
			return $"{nameof(TreeNodeDataG)}| ({value})";
		}
	}


	public interface ITreeNode<TNd, TLc, TL> : ITreeNodeRoot,
		IEnumerable<KeyValuePair<string, ITreeNode<TNd, TLc, TL>>>
		where TLc : class, ITreeLeafCollection<TL>, new()
		where TL : class, ITreeLeaf, new()
	{
		public TNd NodeData { get; }

		public TLc Leaves { get; }

		public ITreeNode<TNd, TLc, TL> ParentNode { get; }

		// public bool AddNode(string[] nodePath, int idx, ITreeNode<TNd, TLc, TL> node);
		// public bool RemoveNode(string[] nodePath, int idx);
		// public bool ContainsNode(string[] nodePath, int idx, out ITreeNode<TNd, TLc, TL> node);
		//
		// public bool AddLeaf(TL leaf);
		// public bool RemoveLeaf(string leafName);
		// public bool ContainsLeaf(string leafName, out TL leaf);
	}

	public interface ITreeNodeRoot
	{
		public string NodeName { get; }
		public ITreeNodeRoot ParentNodeRoot { get; }
		public bool? IsSelected { get; }
		public bool HasNodes { get; }
		public int NodeCount { get; }
		public bool HasLeaves { get; }
		public int LeafCount { get; }
	}

	public class TreeNodeG<TNd, TLc, TL> : INotifyPropertyChanged,
		ITreeNode<TNd, TLc, TL>
		where TLc : class, ITreeLeafCollection<TL>, new()
		where TL : class, ITreeLeaf, new()
	{
		// private Dictionary<string, ITreeNode<TNd, TLc, TL>> nodes =
		// 	new Dictionary<string, ITreeNode<TNd, TLc, TL>>();


		private Dictionary<string, ITreeNode<TNd, TLc, TL>> nodes =
			new Dictionary<string, ITreeNode<TNd, TLc, TL>>();

		private TLc leaves;

		private string nodeName;
		private ITreeNode<TNd, TLc, TL> parentNode;
		private bool? isSelected;
		private TNd nodeData;
		private ITreeNode<TNd, TLc, TL> parentNode1;

		public TreeNodeG() { }

		public TreeNodeG(string nodeName, TNd nodeData, ITreeNode<TNd, TLc, TL> parent = null)
		{
			this.nodeData = nodeData;
			this.nodeName = nodeName;

			isSelected = false;
			ParentNodeRoot = parent;
		}

		public TNd NodeData
		{
			get => nodeData;
			private set
			{
				if (EqualityComparer<TNd>.Default.Equals(value, nodeData)) return;
				nodeData = value;
				OnPropertyChanged();
			}
		}


	#region Nodes

		// done
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

		// done
		public ITreeNode<TNd, TLc, TL> ParentNode
		{
			get => parentNode;
			private set
			{
				if (Equals(value, parentNode)) return;
				parentNode = value;
				OnPropertyChanged();
			}
		}

		// done
		public ITreeNodeRoot ParentNodeRoot
		{
			get => parentNode;
			private set
			{
				if (Equals(value, parentNode)) return;
				parentNode = (ITreeNode<TNd, TLc, TL>) value;
				OnPropertyChanged();
			}
		}

		// // need
		// public bool AddNode(
		// 	string[] nodePath, int idx, ITreeNode<TNd, TLc, TL> node)
		// {
		// 	if (node == null) return false;
		//
		// 	init();
		//
		// 	return false;
		// }
		//
		// // need
		// public bool RemoveNode(string[] nodePath, int idx)
		// {
		// 	return false;
		// }
		//
		// // started
		// public bool ContainsNode(string[] nodePath,
		// 	int idx, out ITreeNode<TNd, TLc, TL> node)
		// {
		// 	node = null;
		//
		// 	if (idx == nodePath.Length) return false;
		//
		// 	ITreeNode<TNd, TLc, TL> tn;
		//
		// 	bool result = nodes.TryGetValue(nodePath[idx], out tn);
		//
		// 	if (!result) return false;
		//
		// 	if (idx == nodePath.Length - 1) return true;
		//
		// 	return tn.ContainsNode(nodePath, idx + 1, out tn);
		//
		// }

		// done
		public int NodeCount => nodes != null ? nodes.Count : 0;

		// done
		public bool HasNodes => nodes != null && nodes.Count > 0;

		// done
		public bool? IsSelected
		{
			get => isSelected;
			private set
			{
				if (value == isSelected) return;
				isSelected = value;
				OnPropertyChanged();
			}
		}

	#endregion

	#region Leaves

		public TLc Leaves { get; }
		//
		// // done
		// public bool AddLeaf(TL leaf)
		// {
		// 	return leaves.Add(leaf);
		// }
		//
		// // done
		// public bool RemoveLeaf(string leafName)
		// {
		// 	return leaves.Remove(leafName);
		// }
		//
		// // done
		// public bool ContainsLeaf(string leafName, out TL leaf)
		// {
		// 	leaf = null;
		// 	return leaves.Contains(leafName, out leaf);
		// }

		// done
		public int LeafCount =>  leaves != null ? leaves.Count : 0;

		// done
		public bool HasLeaves => leaves != null && leaves.Count > 0;

	#endregion

		private void init()
		{
			if (nodes != null) return;

			nodes = new Dictionary<string, ITreeNode<TNd, TLc, TL>>();
		}


		public IEnumerator<KeyValuePair<string, ITreeNode<TNd, TLc, TL>>> GetEnumerator()
		{
			return nodes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}


		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public override string ToString()
		{
			return $"{nameof(TreeNodeG<TNd, TLc, TL>)}| ({nodeName}) ({nodes?.Count ?? -1}) ({leaves?.Count ?? -1})";
		}
	}


#region TreeLeaf

	public interface ITreeLeaf
	{
		public string LeafName { get; }
		public ITreeNodeRoot ParentNodeRoot { get; }
		public bool? IsSelected { get; }
	}

	// H.5 - tree leaf data
	public class TreeLeafG : INotifyPropertyChanged, ITreeLeaf
	{
		private string leafName;
		private string value;
		private bool? isSelected;

	#region	ctor

		public TreeLeafG() { }

		public TreeLeafG(
			string leafName,
			string value,
			ITreeNodeRoot parentNodeRoot = null
			)
		{
			this.leafName = leafName;
			this.value = value;

			isSelected = false;
			ParentNodeRoot = parentNodeRoot;
		}

	#endregion

		// required properties
		public string LeafName
		{
			get => leafName;
			set
			{
				if (value == leafName) return;
				leafName = value;
				OnPropertyChanged();
			}
		}

		public ITreeNodeRoot  ParentNodeRoot { get; private set; }

		// done
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

		// optional properties
		// done
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
			return $"{nameof(TreeLeafG)}| ({leafName})";
		}

	}

#endregion

#region	TreeLeafCollection

	public interface ITreeLeafCollection<TL> :
		IEnumerable<KeyValuePair<string, TL>>
		where TL : class, ITreeLeaf, new()
	{
		// public bool Add(TL leaf);
		// public bool Remove(string leafName);
		// public bool Contains(string leafName, out TL leaf);
		public int Count { get; }
		public bool HasLeaves { get; }
	}

	// done
	// H.6 - tree leaf collection
	public class TreeLeafDictG<TL> : INotifyPropertyChanged, ITreeLeafCollection<TL>
		where TL : class, ITreeLeaf, new()
	{
		// manages the collection of tree leaves - done as a  separate class to 
		// allow this to be done in different ways e.g. as a List<> or Dictionary<>

		private Dictionary<string, TL> leaves;

		private bool initialized;

	#region public methods

		// // done
		// public bool Add(TL leaf)
		// {
		// 	init();
		//
		// 	return leaf.LeafName == null ? false : leaves.TryAdd(leaf.LeafName, leaf);
		// }
		//
		// // done
		// public bool Remove(string leafName)
		// {
		// 	if (!HasLeaves || leafName == null) return false;
		//
		// 	return leaves.Remove(leafName);
		// }
		//
		// // done
		// public bool Contains(string leafName, out TL leaf)
		// {
		// 	leaf = null;
		//
		// 	return
		// 		leafName == null ? false : leaves.TryGetValue(leafName, out leaf);
		// }

		// done
		public int Count => leaves != null ? leaves.Count : 0;

		// done
		public bool HasLeaves => leaves != null && leaves.Count > 0;

		public IEnumerator<KeyValuePair<string, TL>> GetEnumerator()
		{
			return leaves.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

	#endregion

		private void init()
		{
			if (leaves != null) return;

			leaves = new Dictionary<string, TL>();
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public override string ToString()
		{
			return $"{nameof(TreeLeafDictG<TL>)}| ({leaves?.Count ?? -1})";
		}
	}

#endregion


	// public interface ITreeNode2<TNc, TLc, TN, TL> : ITreeNodeRoot
	// 	where TNc : class, ITreeNodeCollection2<TNc, TLc, TN, TL>, new()
	// 	where TLc : class, ITreeLeafCollection<TL>, new()
	// 	where TN : class, ITreeNode2<TNc, TLc, TN, TL>, new()
	// 	where TL : class, ITreeLeaf, new() { }
	//
	//
	//
	// // H.3 - tree node info
	// public class TreeNode2<TNd, TNc, TLc, TN, TL> : INotifyPropertyChanged, ITreeNode2<TNc, TLc, TN, TL>
	// 	where TNd : class, new() // tree node data
	// 	where TNc : class, ITreeNodeCollection2<TNc, TLc, TN, TL>, new()
	// 	where TLc : class, ITreeLeafCollection<TL>, new()
	// 	where TN : class, ITreeNode2<TNc, TLc, TN, TL>, new()
	// 	where TL : class, ITreeLeaf, new() 
	// {
	// 	private static int tempNodeIdx = 0;
	// 	private ITreeNodeRoot parent;
	//
	//
	// 	// used to create unique anonymous tree nodes
	// 	public static int TempNodeIdx => tempNodeIdx++;
	//
	// 	// the information per node
	// 	public ITreeNodeRoot ParentNode
	// 	{
	// 		get => parent;
	// 		private set
	// 		{
	// 			if (Equals(value, parent)) return;
	// 			parent = value;
	// 			OnPropertyChanged();
	// 		}
	// 	}
	//
	// 	public string NodeName { get; private set; }
	// 	public bool? IsDefined { get; private set; }
	// 	public bool? IsSelected { get; set; }
	//
	// 	public TNd TreeNodeData { get; private set; }
	//
	// 	public Dictionary<string, TreeNode2<TNd, TNc, TLc, TN, TL>> Children { get; private set; }
	//
	// 	public TLc Leaves { get; private set; }
	//
	// 	public TreeNode2()
	// 	{
	// 		NodeName = $"TempTreeNode {TempNodeIdx:D7}";
	// 		IsDefined = false;
	// 		TreeNodeData = new TNd();
	// 		Children = null;
	// 		Leaves = null;
	// 	}
	//
	// 	public TreeNode2(string treeNodeName)
	// 	{
	// 		NodeName = treeNodeName;
	// 		IsDefined = null;
	// 		TreeNodeData = new TNd();
	// 		Children = null;
	// 		Leaves = null;
	// 	}
	//
	// 	public TreeNode2(string treeNodeName, TNd nodeData)
	// 	{
	// 		NodeName = treeNodeName;
	// 		IsDefined = true;
	// 		TreeNodeData = nodeData;
	// 		Children = null;
	// 		Leaves = null;
	// 	}
	//
	// 	public event PropertyChangedEventHandler? PropertyChanged;
	//
	// 	[NotifyPropertyChangedInvocator]
	// 	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	// 	{
	// 		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	// 	}
	// }
	//
	//
	// public interface ITreeNodeCollection2<TNc, TLc, TN, TL> : IEnumerable<KeyValuePair<string, TN>>
	// 	where TN : class, ITreeNode2<TNc, TLc, TN, TL>, new() // tree node
	// 	where TNc : class, ITreeNodeCollection2<TNc, TLc, TN, TL>, new()
	// 	where TLc : class, ITreeLeafCollection<TL>, new()
	// 	where TL : class, ITreeLeaf, new()
	// {
	// 	public void Add(TN node);
	// 	public void Find(string nodeName);
	// 	public void Remove(string nodeName);
	// }
	//
	// // H.4 - tree node collection
	// public class TreeNodeCollection2<TNc, TLc, TL, TN> : INotifyPropertyChanged, ITreeNodeCollection2<TNc, TLc, TN, TL>
	// 	where TN : class, ITreeNode2<TNc, TLc, TN, TL>, new() // tree node
	// 	where TNc : class, ITreeNodeCollection2<TNc, TLc, TN, TL>, new()
	// 	where TLc : class, ITreeLeafCollection<TL>, new()
	// 	where TL : class, ITreeLeaf, new()
	// {
	// 	public TreeNodeCollection2()
	// 	{
	// 		nodes = new Dictionary<string, TN>();
	// 	}
	//
	// 	private Dictionary<string, TN> nodes;
	//
	// 	public void Add(TN node) { }
	// 	public void Find(string nodeName) { }
	// 	public void Remove(string nodeName) { }
	//
	//
	// 	IEnumerator IEnumerable.GetEnumerator()
	// 	{
	// 		return GetEnumerator();
	// 	}
	//
	// 	public IEnumerator<KeyValuePair<string, TN>> GetEnumerator()
	// 	{
	// 		return nodes.GetEnumerator();
	// 	}
	//
	//
	// 	public event PropertyChangedEventHandler? PropertyChanged;
	//
	// 	[NotifyPropertyChangedInvocator]
	// 	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	// 	{
	// 		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	// 	}
	// }

	public class TreeClassSampleData

	{
		private TreeClassG
			<
			TreeNodeDataG,
			TreeLeafDictG<TreeLeafG>,
			TreeNodeG<
			TreeNodeDataG,
			TreeLeafDictG<TreeLeafG>,
			TreeLeafG
			>,
			TreeLeafG
			> treeClassG;

		private TreeClassG treeClass;


		public TreeClassSampleData()
		{
			treeClassG = new TreeClassG<TreeNodeDataG,
				TreeLeafDictG<TreeLeafG>,
				TreeNodeG<TreeNodeDataG, TreeLeafDictG<TreeLeafG>, TreeLeafG>, TreeLeafG>();

			treeClass = new TreeClassG();
		}

		public void Test()
		{
			// long way

			TreeLeafDictG<TreeLeafG> tld = new TreeLeafDictG<TreeLeafG>();

			TreeNodeG<TreeNodeDataG, TreeLeafDictG<TreeLeafG>, TreeLeafG> treeNode1 =
				new TreeNodeG<TreeNodeDataG, TreeLeafDictG<TreeLeafG>, TreeLeafG>(
					"Node 001", new TreeNodeDataG("TNd Value 001"));

			TreeNodeG<TreeNodeDataG, TreeLeafDictG<TreeLeafG>, TreeLeafG> treeNode2 =
				new TreeNodeG<TreeNodeDataG, TreeLeafDictG<TreeLeafG>, TreeLeafG>(
					"Node 002", new TreeNodeDataG("TNd Value 002"));

			// treeNode1.AddNode(treeNode2);


			// short way

			TreeLeafCollectionG tlc = new TreeLeafCollectionG();

			TreeLeafG tl = new TreeLeafG("Leaf Name 01", "TL Value 01");

			// tlc.Add(tl);

			TreeNodeG Tn = new TreeNodeG("Node 003", new TreeNodeDataG("TNd Value 003"));



		}


		public bool AddNode()
		{
			return false;
		}

		public bool RemoveNode()
		{
			return false;
		}

		public bool ContainsNode()
		{
			return false;
		}

		public bool FindNode()
		{
			return false;
		}


		public bool AddLeaf()
		{
			return false;
		}

		public bool RemoveLeaf()
		{
			return false;
		}

		public bool ContainsLear()
		{
			return false;
		}

		public bool FindLear()
		{
			return false;
		}
	}
}