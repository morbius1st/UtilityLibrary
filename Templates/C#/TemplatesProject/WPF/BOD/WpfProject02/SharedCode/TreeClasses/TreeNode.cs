using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SharedCode.SampleData;
using UtilityLibrary;
using WpfProject02.Annotations;
using static SharedCode.SampleData.Selection;
using static SharedCode.SampleData.Selection.CheckedState;
using static SharedCode.SampleData.Selection.SelectMode;

// Solution:     WpfProject02
// Project:       WpfProject02
// File:             TreeNode.cs
// Created:      2023-10-07 (10:34 AM)


namespace SharedCode.TreeClasses
{
	[WpfProject02.Annotations.NotNull]
	public class TreeNode<TNd, TLd> : ITreeNodeEx<TNd, TLd>, INotifyPropertyChanged,
		IComparer<TreeNode<TNd, TLd>>, ICloneable,  IEnumerable<TreeNode<TNd, TLd>>
		where TNd : class
		where TLd : class
	{
		private string? nodeKey;

		private Tree<TNd, TLd>? tree;
		private TreeNode<TNd, TLd>? parentNodeEx;


		private bool? isChecked = false;

		private bool? priorIsChecked = false;

		private TNd nodeData = null;

		private TreeNode<TNd, TLd>? tn;
		private bool isExpanded;
		private bool isChosen;

		private bool isTriState = false;

		private int nodesChecked;
		private int nodesMixed;

		private TreeLeaf<TNd, TLd>? foundLeaf;
		private TreeNode<TNd, TLd>? foundNode;

		// private static TreeClass<TNd, TLd>? tree;

	#region private fields

	#endregion

	#region ctor

		// ReSharper disable once UnusedMember.Global
		public TreeNode() { }

		public TreeNode(
			string nodeKey,
			Tree<TNd, TLd>? tree,
			TreeNode<TNd, TLd>? parentNodeEx,
			TNd nodeData,
			Selection.CheckedState isChecked = UNCHECKED)
		{
			this.nodeKey = nodeKey;
			this.nodeData = nodeData;
			this.parentNodeEx = parentNodeEx;
			this.isChecked = Tree<TNd, TLd>.boolList[(int) isChecked];
			this.tree = tree;
		}

	#endregion

	#region public properties

		// serializable properties
		public ObservableDictionary<string, TreeNode<TNd, TLd>>? Nodes { get; set; }

		public ObservableDictionary<string, TreeLeaf<TNd, TLd>>? Leaves { get; set; }

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

		public TreeNode<TNd, TLd> ParentNodeEx
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

		public Tree<TNd, TLd> Tree
		{
			get => tree;
			set => tree = value;
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

		public TreeNode<TNd, TLd> FoundNode
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
		public TreeNode<TNd, TLd> ParentNode
		{
			get => parentNodeEx;
			private set
			{
				if (Equals(value, parentNodeEx)) return;
				parentNodeEx = (TreeNode<TNd, TLd>) value;
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

		public Selection.SelectMode SelectMode  => tree?.SelectMode ?? INDIVIDUAL;

		// when the check box "ischecked" is checked
		public bool? IsChecked
		{
			get => isChecked;
			set
			{
				// Debug.WriteLine($"*** who| {nodeKey}| value| {(value.HasValue ? value : "null")} | checkedstate| {temp} *** \n");

				isChecked = value;

				OnPropertyChanged();
				OnPropertyChanged(nameof(IsCheckedState));

				if (tree.IsTriState)
				{
					tree.UpdateSelectionTriState(this);
				}
				else
				{
					priorIsChecked = isChecked;
					OnPropertyChanged(nameof(PriorIsChecked));
					OnPropertyChanged(nameof(PriorIsCheckedState));

					CheckedState temp = Tree<TNd, TLd>.StateFromBool(value);

					if (temp == CHECKED )
					{
						tree.UpdateSelected(this);
					}
					else if (temp ==  UNCHECKED )
					{
						tree.UpdateDeSelected(this);
					}
					else
					{
						tree.UpdateSelectionTriState(this);
					}
				}

			}
		}

		public bool? PriorIsChecked
		{
			get => priorIsChecked;
			set
			{
				priorIsChecked = value;

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

		public bool IsTriState
		{
			get => isTriState;
			set
			{
				isTriState = value;
				OnPropertyChanged();
			}
		}

		public CheckedState IsCheckedState
		{
			get => Tree<TNd, TLd>.StateFromBool(isChecked);

			private set
			{
				isChecked = Tree<TNd, TLd>.boolList[(int) value];
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsChecked));
			}
		}

		public CheckedState PriorIsCheckedState
		{
			get => Tree<TNd, TLd>.StateFromBool(priorIsChecked);

			private set
			{
				priorIsChecked = Tree<TNd, TLd>.boolList[(int) value];
				OnPropertyChanged();
				OnPropertyChanged(nameof(PriorIsChecked));
			}
		}

	#endregion

	#region public methods

		// selection

		public void SelectNode()
		{
			isChecked = Tree<TNd, TLd>.boolList[(int) CHECKED];
			OnPropertyChanged(nameof(IsChecked));
		}

		public void DeSelectNode()
		{
			isChecked = Tree<TNd, TLd>.boolList[(int) UNCHECKED];
			OnPropertyChanged(nameof(IsChecked));
		}

		public void MixedSelectNode()
		{
			isChecked = Tree<TNd, TLd>.boolList[(int) MIXED];
			OnPropertyChanged(nameof(IsChecked));
		}

		public void CheckedChangedFromChild(CheckedState newChildCheckedState  /*, CheckedState oldChildCheckedState  */)
		{
			// Examples.M.WriteLine($" 500 1A ckd chg| who| {nodeKey}| current| {IsCheckedState}| new| {newChildCheckedState}"  /* old| {oldChildCheckedState}"*/);
			// Debug.WriteLine($" 500 1A ckd chg| who| {nodeKey}| current| {IsCheckedState}| new| {newChildCheckedState}"  /*old| {oldChildCheckedState}"*/);

			if (newChildCheckedState.Equals(IsCheckedState) || ParentNode == null) return; // we are done

			int numNodes = Nodes?.Count ?? 0;

			NodeCountSelection();

			// Debug.WriteLine($"node count    | {numNodes}");
			// Debug.WriteLine($"ckd node count| {nodesChecked}");
			// Debug.WriteLine($"mix node count| {nodesMixed}");
			// Debug.WriteLine($"prior checked | {priorIsChecked}");

			bool allChecked = (numNodes - nodesChecked) == 0;
			bool noneCheckedOrMixed = nodesChecked == 0 && nodesMixed == 0;

			// updating parent
			// child can send any of the three states

			// if sent checked, test all nodes, 
			// if all nodes == checked, set to checked (set to two state)
			// if not all nodes == checked, set to mixed (set to three state)
			// update parent

			// if sent unchecked, test all nodes
			// if not all nodes == checked, set to mixed (set to three state)
			// if no nodes == checked, set to unchecked (set to two state)
			// update parent



			if (allChecked)
			{
				// Debug.WriteLine($" 500 ", "all checked| set to CHECKED");
				IsCheckedState = CHECKED;
				tree.AddNodeToSelected(this);
				IsTriState = false;
			}
			else if (noneCheckedOrMixed)
			{
				// Debug.WriteLine($" 500 ", "none checked| set to UNCHECKED");
				IsCheckedState = UNCHECKED;
				tree.RemoveNodeFromSelected(this);
				IsTriState = false;
			}
			else
			{
				// Debug.WriteLine($" 500 ", "some checked| set to MIXED");
				IsTriState = true;
				IsCheckedState = MIXED;
			}

			ParentNode.CheckedChangedFromChild(IsCheckedState);
		}

		public void CheckedChangeFromParent(CheckedState newParentCheckedState)
		{
			// Examples.M.WriteLine($" 501 1B ckd chg| who| {nodeKey}| current| {IsCheckedState}| new| {newParentCheckedState}"  /* old| {oldChildCheckedState}"*/);
			// Debug.WriteLine($" 501 1B ckd chg| who| {nodeKey}| current| {IsCheckedState}| new| {newParentCheckedState}"  /*old| {oldChildCheckedState}"*/);


			if (Nodes != null)
			{
				foreach (var kvp in Nodes)
				{
					// Debug.WriteLine($"\n 500 ", $"start updating| {kvp.Value.NodeKey}");
					kvp.Value.CheckedChangeFromParent(newParentCheckedState);
					// Debug.WriteLine($" 500 ", $"finished updating| {kvp.Value.NodeKey}");
				}
			}
			// else
			// {
			// 	Debug.WriteLine($" 500 ", "nodes is null");
			// }

			switch (newParentCheckedState)
			{
			case CHECKED:
				{
					// Debug.WriteLine($" 500 ", "parent checked, save to prior, set checked");
					IsTriState = false;
					PriorIsCheckedState = IsCheckedState;
					IsCheckedState = CHECKED;
					tree.AddNodeToSelected(this);
					break;
				}
			case UNCHECKED:
				{
					// Debug.WriteLine($" 500 ", "parent unchecked, set to unchecked");
					IsTriState = false;
					IsCheckedState = UNCHECKED;
					tree.RemoveNodeFromSelected(this);
					break;
				}
			default:
				{
					// Debug.WriteLine($" 500 ", $"parent other ({newParentCheckedState})");

					if (PriorIsCheckedState == MIXED) IsTriState = true;

					IsCheckedState = PriorIsCheckedState;
					PriorIsCheckedState = UNCHECKED;

					// Debug.WriteLine($" 500 ", $"set to {IsCheckedState}");

					break;
				}
			}

		}

		// general

		public void NotifyNodesUpdated()
		{
			OnPropertyChanged(nameof(Nodes));
		}

		public void InitNodes()
		{
			if (Nodes == null)
				Nodes
					= new ObservableDictionary<string, TreeNode<TNd, TLd>>();
		}

		public void InitLeaves()
		{
			if (Leaves == null) Leaves = new ObservableDictionary<string, TreeLeaf<TNd, TLd>>();
		}

		public void NodeCountSelection()
		{
			nodesChecked = 0;
			nodesMixed = 0;

			if ((Nodes?.Count ?? 0) == 0) return;

			foreach (KeyValuePair<string, TreeNode<TNd, TLd>> kvp in Nodes)
			{
				nodesChecked += kvp.Value.isChecked == Tree<TNd, TLd>.boolList[(int) CHECKED] ? 1 : 0;
				nodesMixed += kvp.Value.isChecked == Tree<TNd, TLd>.boolList[(int) MIXED] ? 1 : 0;
			}
		}

		public int NodeCountBranch()
		{
			if ((Nodes?.Count ?? 0) == 0) return 0;

			int result = Nodes.Count;

			foreach (KeyValuePair<string, TreeNode<TNd, TLd>> kvp in Nodes)
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

		public void AddNode(string key, TreeNode<TNd, TLd> node)
		{
			Nodes.Add(key, node);

			OnPropertyChanged(nameof(Nodes));
		}

		public bool TryAddNode(string key, TreeNode<TNd, TLd> node)
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

		public bool ContainsNode(string findKey, out TreeNode<TNd, TLd> node)
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

		public void AddLeaf(string key, TreeLeaf<TNd, TLd> leaf)
		{
			Leaves.Add(key, leaf);

			OnPropertyChanged(nameof(Leaves));
		}

		public bool TryAddLeaf(string key, TreeLeaf<TNd, TLd> leaf)
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


		public void SelectMatchingNodeTree() { }

		public void SelectMatchingNodeBranch() { }


		public void DeSelectMatchingNodeTree() { }

		public void DeSelectMatchingNodeBranch() { }

	#endregion

	#region system overrides

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new TreeNodeEnumerator<TNd, TLd>(this);
		}

		public IEnumerator<TreeNode<TNd, TLd>> GetEnumerator()
		{
			return new TreeNodeEnumerator<TNd, TLd>(this);
		}

		public int Compare(TreeNode<TNd, TLd>? x, TreeNode<TNd, TLd>? y)
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
			return $"{nameof(TreeNode<TNd, TLd>)} | name| {nodeKey} | parent| {parentNodeEx?.NodeKey ?? "no parent"}";
		}

	#endregion
	}
}