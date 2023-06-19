using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using SharedWPF.ShWin;
using ShCode.SampleData;
using WpfProject02.Annotations;

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

		public static  ShDebugMessages M;


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


		public event PropertyChangedEventHandler? PropertyChanged;

		private void init()
		{
			shDbgMsg  = new ShDebugMessages(this);
			M = new ShDebugMessages(this);
			show = new SampleDataShow(shDbgMsg);

			tcsd = new TreeClassSampleData();
			dst = new DataSampleTree();
			sd = new SampleData();

			Aitem = sd.MakeItem("A");

			Blist = sd.MakeItemList("B");

			ClistOfItems = sd.MakeItemAndListOfItems("C");

			DclassListOfItems = sd.MakeClassOfItemAndfListOfItems("D");

			Elist = sd.MakeListOfItemList("E");

			FtreeItem = sd.MakeTreeItem2("F");

			GtreeListOfTreeItems = sd.MakeTreeOfListOfTreeItems("G");

			// tcsd.Test();
		}


		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void BtnExit_OnClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void BtnDebug_OnClick(object sender, RoutedEventArgs e)
		{
			Tree t = dst.Tree;

			int a = 1 + 1;
		}

		private void BtnShowItem_OnClick(object sender, RoutedEventArgs e)
		{
			show.ShowA(Aitem);
		}

		private void BtnShowTreeAndLeaves_OnClick(object sender, RoutedEventArgs e)
		{
			M.WriteLineCodeMap("Enter Method");

			int a = dst.TestShowTreeNodesAndLeaves();

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

			dst.AddNodesAndLeaves();

			M.WriteLineStatus("nodes and leaves added");

			// dst.TestAddNodes2();
			// a = dst.TestShowTreeNodes();
			// or
			// a = dst.TestShowTreeNodesAndLeaves();
			// Debug.WriteLine($"count from show tree| {a}");
		}


		private void BtnTestA_OnClick(object sender, RoutedEventArgs e)
		{
			// TreeOfListOfTreeItems x = GtreeListOfTreeItems;

			Tree t = new Tree("Test tree 2", new TreeNodeData("Test node data 2"));

			dst.Tree.Clear();

			int a = 1 + 1;


			a = dst.TestEnumerator();
			Debug.WriteLine($"count from enumerator| {a}");


			// dst.TestAddNode();
			// or
			// dst.TestAddNodesAndLeaves();
			// a = dst.Tree.NodeCountTree();
			// Debug.WriteLine($"count from counter| {a}");

		}
	}
}