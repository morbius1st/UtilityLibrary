﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using SharedCode.SampleData;
using UtilityLibrary;
using WpfProject02.Annotations;
using static SharedCode.TreeClasses.Selection;
using static SharedCode.TreeClasses.Selection.SelectedState;
using static SharedCode.TreeClasses.Selection.SelectMode;

// Solution:     WpfProject02
// Project:       WpfProject02
// File:             TreeNode.cs
// Created:      2023-10-07 (10:34 AM)


namespace SharedCode.TreeClasses
{
	[WpfProject02.Annotations.NotNull]
	public class TreeNode<TNd, TLd> : 
		ITreeNode,
		INotifyPropertyChanged,
		IComparer<TreeNode<TNd, TLd>>, 
		ICloneable,  
		IEnumerable<TreeNode<TNd, TLd>>

		where TNd : class
		where TLd : class
	{
		private string? nodeKey;

		private Tree<TNd, TLd>? tree;
		private TreeNode<TNd, TLd>? parentNodeEx;

		private TNd nodeData = null;

		private TreeNode<TNd, TLd>? tn;


		private bool? isChecked = false;
		private bool? priorIsChecked = false;
		private bool isExpanded;
		private bool isChosen;
		private bool isTriState = false;

		private int nodesChecked;
		private int nodesMixed;

		private TreeLeaf<TNd, TLd>? foundLeaf;
		private TreeNode<TNd, TLd>? foundNode;
		private bool isEnabled = true;
		private SelectedState state = DESELECTED;

		// private static TreeClass<TNd, TLd>? tree;

		#region private fields

		#endregion

		#region ctor

		// ReSharper disable once UnusedMember.Global
		public TreeNode()
		{
			
		}

		public TreeNode(
			string nodeKey,
			Tree<TNd, TLd>? tree,
			TreeNode<TNd, TLd>? parentNodeEx,
			TNd nodeData,
			Selection.SelectedState isSelected = DESELECTED)
		{
			this.nodeKey = nodeKey;
			this.nodeData = nodeData;
			this.parentNodeEx = parentNodeEx;
			this.isChecked = Tree<TNd, TLd>.boolList[(int) isSelected];
			this.tree = tree;
			// 
		}

	#endregion

	#region public properties
		
		// serializable properties
		public ObservableDictionary<string, TreeNode<TNd, TLd>>? 
			Nodes { get; set; }

		public ObservableDictionary<string, TreeLeaf<TNd, TLd>>? 
			Leaves { get; set; }

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
		// public ITreeNode<TNd, TLd> IParentNode => (ITreeNode<TNd, TLd>) ParentNode;
		public ITreeNode IParentNode => (ITreeNode) ParentNode;

		public Tree<TNd, TLd> Tree
		{
			get => tree;
			set => tree = value;
		}
		// public ITree<TNd, TLd> ITree => (ITree<TNd, TLd>) Tree;
		public ITree ITree => (ITree) Tree;

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

		//
		
		// selection 

		// when the check box "ischecked" is checked
		public bool? IsChecked
		{
			get => Tree<TNd, TLd>.boolList[(int) state];
			set
			{
				state = Tree<TNd, TLd>.StateFromBool(value);

				tree.NodeSelector.SelectDeselect(this);
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

		public bool IsEnabled
		{
			get => isEnabled; 
			set
			{
				if (isEnabled == value) return;
				isEnabled = value;
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

		public SelectedState State
		{
			get => state; 
			set
			{
				if (state == value) return;
				state = value;

				if (state!= UNSET)
				{
					isChecked = Tree<TNd, TLd>.boolList[(int) state];
					OnPropertyChanged(nameof(IsChecked));
				}

				OnPropertyChanged();
			}
		}

		/*

		public SelectedState IsSelectedState
		{
			get => Tree<TNd, TLd>.StateFromBool(isChecked);

			private set
			{
				isChecked = Tree<TNd, TLd>.boolList[(int) value];
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsChecked));
			}
		}

		public SelectedState PriorIsSelectedState
		{
			get => Tree<TNd, TLd>.StateFromBool(priorIsChecked);

			private set
			{
				priorIsChecked = Tree<TNd, TLd>.boolList[(int) value];
				OnPropertyChanged();
				OnPropertyChanged(nameof(PriorIsChecked));
			}
		}

		*/

		#endregion

		#region public methods

		// selection

		public void Select()
		{
			State = SELECTED;
		}

		public void DeSelect()
		{
			State = DESELECTED;
		}

		public void MixedSelectNode()
		{
			isChecked = Tree<TNd, TLd>.boolList[(int) MIXED];
			OnPropertyChanged(nameof(IsChecked));
		}
		
		/*
		public void CheckedChangedFromChild(SelectedState newChildSelectedState) //  , CheckedState oldChildCheckedState )
		{
			// Examples.M.WriteLine($" 500 1A ckd chg| who| {nodeKey}| current| {IsCheckedState}| new| {newChildCheckedState}"); //   old| {oldChildCheckedState}");
			// Debug.WriteLine($" 500 1A ckd chg| who| {nodeKey}| current| {IsCheckedState}| new| {newChildCheckedState}"); //  old| {oldChildCheckedState}");

			if (newChildSelectedState.Equals(IsSelectedState) || ParentNode == null) return; // we are done

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
				IsSelectedState = SELECTED;
				tree.AddNodeToSelected(this);
				IsTriState = false;
			}
			else if (noneCheckedOrMixed)
			{
				// Debug.WriteLine($" 500 ", "none checked| set to UNCHECKED");
				IsSelectedState = UNSELECTED;
				tree.RemoveNodeFromSelected(this);
				IsTriState = false;
			}
			else
			{
				// Debug.WriteLine($" 500 ", "some checked| set to MIXED");
				IsTriState = true;
				IsSelectedState = MIXED;
			}

			ParentNode.CheckedChangedFromChild(IsSelectedState);
		}

		public void CheckedChangeFromParent(SelectedState newParentSelectedState)
		{
			// Examples.M.WriteLine($" 501 1B ckd chg| who| {nodeKey}| current| {IsCheckedState}| new| {newParentCheckedState}"); //   old| {oldChildCheckedState}");
			// Debug.WriteLine($" 501 1B ckd chg| who| {nodeKey}| current| {IsCheckedState}| new| {newParentCheckedState}" ); // old| {oldChildCheckedState}");


			if (Nodes != null)
			{
				foreach (var kvp in Nodes)
				{
					// Debug.WriteLine($"\n 500 ", $"start updating| {kvp.Value.NodeKey}");
					kvp.Value.CheckedChangeFromParent(newParentSelectedState);
					// Debug.WriteLine($" 500 ", $"finished updating| {kvp.Value.NodeKey}");
				}
			}
			// else
			// {
			// 	Debug.WriteLine($" 500 ", "nodes is null");
			// }

			switch (newParentSelectedState)
			{
			case SELECTED:
				{
					// Debug.WriteLine($" 500 ", "parent checked, save to prior, set checked");
					IsTriState = false;
					PriorIsSelectedState = IsSelectedState;
					IsSelectedState = SELECTED;
					tree.AddNodeToSelected(this);
					break;
				}
			case UNSELECTED:
				{
					// Debug.WriteLine($" 500 ", "parent unchecked, set to unchecked");
					IsTriState = false;
					IsSelectedState = UNSELECTED;
					tree.RemoveNodeFromSelected(this);
					break;
				}
			default:
				{
					// Debug.WriteLine($" 500 ", $"parent other ({newParentCheckedState})");

					if (PriorIsSelectedState == MIXED) IsTriState = true;

					IsSelectedState = PriorIsSelectedState;
					PriorIsSelectedState = UNSELECTED;

					// Debug.WriteLine($" 500 ", $"set to {IsCheckedState}");

					break;
				}
			}

		}
		*/

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
				nodesChecked += kvp.Value.isChecked == Tree<TNd, TLd>.boolList[(int) SELECTED] ? 1 : 0;
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

		/*
		ui []
		+-> bool? Ischecked 
			set state
				[no onproperty]
		+<		[ no set ischecked]
		|
		+-> Tree been selected(node)
		+-<
		+-> set selection lists
		+-<
		+-> node set state
			> node.SetState()
				SetState()
					> state = value (adjus for tristate
					> onprop(ischecked)
					> onprop (state)



		*/

		public void SelectMatchingNodeTree() { }

		public void SelectMatchingNodeBranch() { }


		public void DeSelectMatchingNodeTree() { }

		public void DeSelectMatchingNodeBranch()
		{
			Predicate<string> toupper =  s => s.Equals(s.ToUpper());

			Predicate<string> a = toupper;
		}

	#endregion

	#region system overrides
		
		/*
		public IEnumerable<ITreeNode<TNd, TLd>> EnumNodes()
		{
			// ITreeNode<TNd, TLd> treeNode = startNode == null ? Tree.RootNode : startNode;

			if (HasNodes)
			{
				foreach (ITreeNode<TNd, TLd> tn in this)
				{
					yield return tn;
				}
			}
		}

		public IEnumerable<ITreeLeaf<TNd, TLd>> EnumLeaves()
		{
			// ITreeNode<TNd, TLd> treeNode = startNode == null ? Tree.RootNode : startNode;

			if (HasLeaves)
			{
				foreach (KeyValuePair<string, TreeLeaf<TNd, TLd>> kvp in Leaves)
				{
					yield return (ITreeLeaf<TNd, TLd>) kvp.Value;
				}
			}
		}
		*/


		public IEnumerable<ITreeNode> EnumNodes()
		{
			// ITreeNode<TNd, TLd> treeNode = startNode == null ? Tree.RootNode : startNode;

			if (HasNodes)
			{
				foreach (ITreeNode tn in this)
				{
					yield return tn;
				}
			}
		}

		public IEnumerable<ITreeLeaf> EnumLeaves()
		{
			// ITreeNode<TNd, TLd> treeNode = startNode == null ? Tree.RootNode : startNode;

			if (HasLeaves)
			{
				foreach (KeyValuePair<string, TreeLeaf<TNd, TLd>> kvp in Leaves)
				{
					yield return (ITreeLeaf) kvp.Value;
				}
			}
		}


		IEnumerator IEnumerable.GetEnumerator()
		{
			return new TreeNodeEnumerator<TNd, TLd>(this);
		}

		public IEnumerator<TreeNode<TNd, TLd>> GetEnumerator()
		{
			return new TreeNodeEnumerator<TNd, TLd>(this);
		}

		public IEnumerable<TreeLeaf<TNd, TLd>> GetLeaves()
		{
			if (HasLeaves)
			{
				foreach (KeyValuePair<string, TreeLeaf<TNd, TLd>> kvp in Leaves)
				{
					yield return kvp.Value;
				}
			}
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