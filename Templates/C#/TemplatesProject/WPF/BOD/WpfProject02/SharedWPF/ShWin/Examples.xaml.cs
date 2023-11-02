using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SharedCode.SampleData;
using SharedCode.ShDebugAssist;
using SharedCode.TreeClasses;
using SharedWPF.ShWin;
using ShCode.SampleData;
using WpfProject02.Annotations;
using static SharedCode.TreeClasses.Selection;
using static SharedCode.TreeClasses.Selection.SelectMode;
using static SharedCode.TreeClasses.Selection.SelectFirstClass;
using System.Xml.Linq;
using System.Collections.ObjectModel;

/*
 notes:
for TREE class
1. actual tree is in the class DataSample Tree
2. here has a referece to the tree "ObservTree"
3. tree changes need to be followed by an OpPropertyChanged)
4. this is just a front end for the data model - avoid modifications here
5. DataSampleTree is just the data setup and intermediary for the tree - tree preforms the work
6. global class "ShDebugMessages" ("M") allows all classes to send messages to the windows

*/

namespace SharedWPF.ShWin
{
	/// <summary>
	/// Interaction logic for Examples.xaml
	/// </summary>
	public partial class Examples : Window, INotifyPropertyChanged, IWinMsg
	{
		private TreeClassSampleData tcsd;
		private SampleData sd;
		private SampleDataShow show;
		private ShDebugMessages shDbgMsg;

		private DataSampleTree dst;

		private Item aitem;

		private List<Item> blist;

		private List<List<Item>> elist;

		private TreeItem ftreeItem;

		private ItemAndListOfItems clistOfItems;

		private ClassOfItemAndfListOfItems dclassListOfItems;

		private TreeOfListOfTreeItems gtreeListOfTreeItems;

		private string codeMapText;

		private string statusBoxText;

		private string messageBoxText;
		private string testString = "test";

		private bool switch1 = false;

		// private SelectMode selectMode = SelectMode.INDIVIDUAL;
		//
		// private SelectMode selectedMode;
		// private KeyValuePair<string, SelectMode> selectedMode;


		public static  ShDebugMessages M;
		private object selItem;
		private bool? isSelected = false;


	#region message boxes

		public string MessageBoxText
		{
			get => messageBoxText;
			set
			{
				if (value == messageBoxText) return;
				messageBoxText = value;
				OnPropertyChanged();
			}
		}

		public string StatusBoxText
		{
			get => statusBoxText;
			set
			{
				if (value == statusBoxText) return;
				statusBoxText = value;
				OnPropertyChanged();
			}
		}

		public string CodeMapText
		{
			get => codeMapText;
			set
			{
				if (value == codeMapText) return;
				codeMapText = value;
				OnPropertyChanged();
			}
		}

	#endregion

	#region public properties

		// data structure types
		// A. simple item   ("Item")
		// B. simple flat list   ("ListOfItems")
		// C. simple item with a simple flat list  ("ItemAndListOfItems")
		// D. class with a simple flat list  ("ClassOfItemAndfListOfItems")
		// E. two-level flat list (item with a list)  ("ListOfItemsWithListOfItems")
		// F. multi-level list (item with a list which contains itself)  ("TreeOfListOfItems")
		// G. class of multi-level list (item with a list which contains itself)  ("TreeOfListOfItems")
		/*
		 * A.
		 * class Item { public name; public value}
		 * public "item"
		 *
		 * B.
		 * public List<Item>  "ListOfItems"
		 *
		 * * class ItemAndList {public Item; List<Item>;}
		 *
		 * C.
		 * public ItemAndList "ItemAndListOfItems"
		 *
		 * D.
		 * class ClassOfItemAndList {public classlistname; public ItemAndList}
		 * public ClassOfItemAndList  "ClassOfItemAndListOfItems"
		 *
		 * E.
		 * public List<ItemAndList>  "ListOfItemAndListOfItems"
		 *
		 * * class TreeItem {public item; List<TreeItem>;}
		 *
		 * F.
		 * public TreeItem  "TreeOfListOfItems"
		 *
		 * G.
		 * class ClassOfTreeItem {public ClassName; public TreeItem;}
		 * public ClassOfTreeItem "ClassOfTreeItem"
		 *
		 */

		public Examples()
		{
			InitializeComponent();

			init();
		}

		public string Title { get; set; } = "Examples";

		// T Observable Tree
		public Tree<TreeNodeData, TreeLeafData> ObservTree => dst?.Tree;

		public TreeNode<TreeNodeData, TreeLeafData> ObservTreeNode => dst?.Tree?.RootNode;

		public ObservableCollection<ITreeNode> SelectedList => ObservTree?.Selector?.Selected?.Selected ?? null;

		public string TestString
		{
			get => testString;
			set
			{
				if (value == testString) return;
				testString = value;
				OnPropertyChanged();
			}
		}

		// A
		public Item Aitem
		{
			get => aitem;
			set
			{
				if (Equals(value, aitem)) return;
				aitem = value;
				OnPropertyChanged();
			}
		}

		// B
		public List<Item>  Blist
		{
			get => blist;
			set
			{
				if (Equals(value, blist)) return;
				blist = value;
				OnPropertyChanged();
			}
		}

		// C
		public ItemAndListOfItems ClistOfItems
		{
			get => clistOfItems;
			set
			{
				if (Equals(value, clistOfItems)) return;
				clistOfItems = value;
				OnPropertyChanged();
			}
		}

		// D
		public ClassOfItemAndfListOfItems DclassListOfItems
		{
			get => dclassListOfItems;
			set
			{
				if (Equals(value, dclassListOfItems)) return;
				dclassListOfItems = value;
				OnPropertyChanged();
			}
		}

		// E
		public List<List<Item>> Elist
		{
			get => elist;
			set
			{
				if (Equals(value, elist)) return;
				elist = value;
				OnPropertyChanged();
			}
		}

		// F
		public TreeItem FtreeItem
		{
			get => ftreeItem;
			set
			{
				if (Equals(value, ftreeItem)) return;
				ftreeItem = value;
				OnPropertyChanged();
			}
		}

		// G
		public TreeOfListOfTreeItems GtreeListOfTreeItems
		{
			get => gtreeListOfTreeItems;
			set
			{
				if (Equals(value, gtreeListOfTreeItems)) return;
				gtreeListOfTreeItems = value;
				OnPropertyChanged();
			}
		}


		public DataSampleTree Data => dst;


		public bool? IsSelected
		{
			get => isSelected;
			set
			{
				isSelected = value;
				if (M != null)
				{
					M.WriteLine($"Example.IsSelected| value| {(value.HasValue ? value : "null")}");
				}

				OnPropertyChanged();
			}
		}

	#endregion

	#region methods

		private void init()
		{
			shDbgMsg  = new ShDebugMessages(this);
			M = new ShDebugMessages(this);
			show = new SampleDataShow(shDbgMsg);

			tcsd = new TreeClassSampleData();
			sd = new SampleData();

			dst = new DataSampleTree();
			dst.E = this;

			OnPropertyChanged(nameof(Data));

			Aitem = sd.MakeItem("A");

			Blist = sd.MakeItemList("B");

			ClistOfItems = sd.MakeItemAndListOfItems("C");

			DclassListOfItems = sd.MakeClassOfItemAndfListOfItems("D");

			Elist = sd.MakeListOfItemList("E");

			FtreeItem = sd.MakeTreeItem2("F");

			GtreeListOfTreeItems = sd.MakeTreeOfListOfTreeItems("G");
		}

		public void UpdateObservTree()
		{
			OnPropertyChanged(nameof(ObservTree));
			OnPropertyChanged(nameof(SelectedList));
		}

	#endregion

	#region events

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

	#endregion

	#region buttons

		private void BtnDebug_OnClick(object sender, RoutedEventArgs e)
		{
			Tree<TreeNodeData, TreeLeafData> t = dst.Tree;

			TreeNode<TreeNodeData, TreeLeafData> rn1 = dst.Tree.RootNode;
			
			// ITreeNode<TreeNodeData, TreeLeafData> it1 = dst.Tree.RootNode;
			// ITreeNode<TreeNodeData, TreeLeafData> it2 =
			// 	(ITreeNode<TreeNodeData, TreeLeafData>) dst.Tree.IRootNode;
			//
			// ITreeNode<TreeNodeData, TreeLeafData> it3 = null;
			//
			//
			// ITreeNode2 rn2 = dst.Tree.RootNode;
			// ITreeNode2 tn2 = null;


			// string sel = (t?.SelectedNodes?.Count ?? 0) > 0 ? t.SelectedNodes[0].NodeKey : "none";
			// string psel = (t?.PriorSelectedNodes?.Count ?? 0) > 0 ? t.PriorSelectedNodes[0].NodeKey : "none";


			// foreach (ITreeNode<TreeNodeData, TreeLeafData> node in it1.EnumNodes())
			// {
			// 	Debug.WriteLine($"{node.NodeKey}");
			//
			// 	if (node.HasLeaves)
			// 	{
			// 		it3 = node;
			// 		break;
			// 	}
			// }

			// foreach (ITreeLeaf<TreeNodeData, TreeLeafData> leaf in it3.EnumLeaves())
			// {
			// 	Debug.WriteLine($"{leaf.LeafKey}");
			// }


			// foreach (ITreeNode2 node in rn2.EnumNodes2())
			// {
			// 	Debug.WriteLine($"{node.NodeKey}");
			//
			// 	if (node.HasLeaves)
			// 	{
			// 		tn2 = node;
			// 		break;
			// 	}
			// }
			//
			// foreach (ITreeLeaf2 leaf in tn2.EnumLeaves2())
			// {
			// 	Debug.WriteLine($"{leaf.LeafKey}");
			// 	Debug.WriteLine($"{((TreeLeaf<TreeNodeData, TreeLeafData>) leaf).LeafKey}");
			// }

			// foreach (ITreeNode<TreeNodeData, TreeLeafData> node in dst.Tree.EnumAllNodes())
			// {
			// 	Debug.WriteLine($"{node.NodeKey}");
			// }
			//
			// foreach (ITreeLeaf<TreeNodeData, TreeLeafData> leaf in dst.Tree.EnumAllLeaves())
			// {
			// 	Debug.WriteLine($"{leaf.LeafKey}");
			//
			// }

			int s = t.Selector.SelectedCount;
			int p = t.Selector.PriorSelectedCount;

			ATreeSelector selector = t.Selector;

			int a = 1 + 1;

			// Debug.WriteLine($"selected| {sel}");
			// Debug.WriteLine($"prior selected| {psel}");
		}


		private void BtnExit_OnClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void BtnTestAll_OnClick(object sender, RoutedEventArgs e)
		{
			dst.TestAll();
		}

		private void BtnShowItem_OnClick(object sender, RoutedEventArgs e)
		{
			show.ShowA(Aitem);
		}

		private void BtnShowTreeNodes_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			int a = dst.ShowNodesAndMaybeLeaves(false);

			M.WriteLine($"count| {a}");
		}

		private void BtnShowTreeLeaves_OnClick(object sender, RoutedEventArgs e)
		{
			dst.ShowAllLeaves();
		}

		private void BtnShowTreeAndLeaves_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			int a = dst.ShowNodesAndMaybeLeaves();

			M.WriteLine($"count| {a}");
		}

		private void BtnCountTreeAndLeaves_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			dst.TestCountNodesAndLeaves();

			M.WriteLineStatus("done");
		}

		private void BtnAddTreeAndLeaves_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			M.Write($"create tree first| ");

			dst.CreateTree();

			M.WriteLine($"*** done ***");

			dst.AddNodesAndLeaves();

			M.WriteLineStatus("nodes and leaves added");

			UpdateObservTree();
			// OnPropertyChanged(nameof(ObservTreeNode));
		}

		private void BtnEnumTreeAndLeaves_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			dst.AddNodesAndLeaves();

			int a ;

			M.WriteLine($"\nenumerator1");

			a = dst.TestEnumerator1();

			M.WriteLine($"count from enumerator1| {a}");

			M.WriteLine($"\nenumerator2");

			a = dst.TestEnumerator2();

			M.WriteLine($"count from enumerator2| {a}");
		}

		private void SetReqUnique_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			M.Write($"\nset require unique to true| ");

			dst.Tree.RequireUniqueKeys = true;

			M.WriteLine("*** done ***");
		}

		private void BtnRenameNode_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			M.WriteLine($"\nrename node| ");

			dst.RenameNode();

			M.WriteLine("*** done ***");
		}

		private void BtnAddNodePath_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			M.Write($"\nadd node path| ");

			bool result = dst.TestAddNode();

			string answer = result ? "*** WORKED ***" : "*** FAIL ***";

			M.WriteLine(answer);
		}

		private void BtnTestMoveNode_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			dst.TestMoveNode();

			// OnPropertyChanged(nameof(ObservTree));
			// OnPropertyChanged(nameof(ObservTree.RootNode));
			// OnPropertyChanged(nameof(ObservTree.RootNode.Nodes));
		}

		private void BtnTestDeleteNode_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			dst.TestDeleteNode();
		}

		private void BtnTestFindNode_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			dst.TestFindNodes();

			M.WriteLineStatus("done");
		}

		private void BtnTestDeleteLeaf_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			dst.TestDeleteLeaf();

			M.WriteLineStatus("done");
		}

		private void BtMoveLeaf_OnClick(object sender, RoutedEventArgs e)
		{
			dst.TestMoveLeaf();
		}

		private void BtnTestFindLeaf_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			dst.TestFindLeaf();

			M.WriteLineStatus("done");
		}

		private void BtnTestCountMatchingLeaves_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			dst.TestCountLeafMatches2();

			M.WriteLineStatus("done");
		}

		private void BtnClrMessages_OnClick(object sender, RoutedEventArgs e)
		{
			M.MsgClr(0);
		}

		private void InLineButton_OnClick(object sender, RoutedEventArgs e)
		{
			switch1 = !switch1;
			TestString = switch1 ? "is true" : "is false";
		}

		private void BtnTreeClear_OnClick(object sender, RoutedEventArgs e)
		{
			dst.TreeClear();
		}

		// selection 

		private void BtnSelTree_OnClick(object sender, RoutedEventArgs e)
		{
			dst.SelectTree();
		}

		private void BtnDeSelTree_OnClick(object sender, RoutedEventArgs e)
		{
			dst.DeSelectTree();
		}

		private void BtnShowSelDeSelLists_OnClick(object sender, RoutedEventArgs e)
		{
			dst.ShowSelDeselLists();
		}


		private void BtMNed_OnClick(object sender, RoutedEventArgs e)
		{
			dst.TestEnableDisable();
		}

		private void BtnEnum_OnClick(object sender, RoutedEventArgs e)
		{
			dst.testEnumerate();
		}

		private void BtnEnum3_OnClick(object sender, RoutedEventArgs e)
		{
			dst.TestEnumerator3();
		}

	#endregion


		private void BtnListTest_OnClick(object sender, RoutedEventArgs e)
		{
			if (dst.TestList == null)
			{
				dst.ListTest01();
			}

			dst.AddNodeToTestList01();

			OnPropertyChanged(nameof(Data));
		}
	}


	public class ListBoxContentTemplateSelector : DataTemplateSelector
	{
		public DataTemplate Empty { get; set; }
		public DataTemplate ListBox { get; set; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item == null) { return null; }


			return (bool) item ? ListBox : Empty;
		}
	}
}