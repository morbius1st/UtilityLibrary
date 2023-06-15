#region + Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using SharedCode.ShDebugAssist;
using SharedWPF.ShWin;
using WpfProject02.Annotations;

#endregion

// user name: jeffs
// created:   6/5/2023 12:09:52 PM

namespace SharedCode.SampleData
{
	// data structure types
	// A. simple item   ("Item")
	// B. simple flat list   ("ListOfItems")  (list<A>)
	// C. simple item with a simple flat list  ("ItemAndListOfItems")  A + List<A>
	// D. class with a simple flat list  ("ClassOfItemAndfListOfItems")  class {C)
	// E. two-level flat list (item with a list)  ("ListOfItemsWithListOfItems")  List<B>
	// F. Simple TreeItem ("TreeItem") == multi-level list (item with a list which contains itself)  ("TreeOfListOfItems")  T + List<T>
	// G. class of multi-level list (TreeItem with a list which contains itself)  ("TreeOfListOfItems")  class {F}
	// H. class of Tree (multi-level) in which the tree info and tree data are separate objects and data is generic
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
	 * class ClassOfItemAndList {public ClassOfItemAndList; public ItemAndList}
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
	 * class ClassOfTreeItem {public ClassOfTreeItem; public TreeItem;}
	 *
	 * H.
	 * class TreeClass {public TreeClass<T>; public TreeInfi; public T TreeData}
	 *
	 */
	//


	// A
	public class Item : INotifyPropertyChanged
	{
		private string itemName;
		private string value;

		public Item(string itemName, string value)
		{
			this.itemName = itemName;
			this.value = value;
		}

		public string ItemName
		{
			get => itemName;
			set
			{
				itemName = value;
				OnPropertyChanged();
			}
		}

		public string Value
		{
			get => value;
			set
			{
				this.value = value;
				OnPropertyChanged();
			}
		}

		public override string ToString()
		{
			return $"{nameof(Item)} | {itemName}";
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	// F
	public class TreeItem : INotifyPropertyChanged
	{
		private Item titem;
		private List<TreeItem> treeItems;

		public TreeItem(string itemName, string value)
		{
			Titem = new Item(itemName, value);

			TreeItems = new List<TreeItem>();
		}

		public Item Titem
		{
			get => titem;
			set
			{
				if (Equals(value, titem)) return;
				titem = value;
				OnPropertyChanged();
			}
		}

		public List<TreeItem> TreeItems
		{
			get => treeItems;
			set
			{
				if (Equals(value, treeItems)) return;
				treeItems = value;
				OnPropertyChanged();
			}
		}

		public override string ToString()
		{
			return $"{nameof(TreeItem)} | {titem.ItemName} ({treeItems.Count})";
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	// C
	public class ItemAndListOfItems : INotifyPropertyChanged
	{
		private Item citem;
		private List<Item> clist;

		public ItemAndListOfItems(string itemName, string value)
		{
			Citem = new Item(itemName, value);

			clist = new List<Item>();
		}

		public Item Citem
		{
			get => citem;
			set
			{
				if (Equals(value, citem)) return;
				citem = value;
				OnPropertyChanged();
			}
		}

		public List<Item> Clist
		{
			get => clist;
			set
			{
				if (Equals(value, clist)) return;
				clist = value;
				OnPropertyChanged();
			}
		}

		public override string ToString()
		{
			return $"{nameof(ItemAndListOfItems)} ({citem.ItemName} ({clist.Count})";
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	// D
	public class ClassOfItemAndfListOfItems : INotifyPropertyChanged
	{
		// C
		private ItemAndListOfItems dlist;

		public ClassOfItemAndfListOfItems(string itemName, string value)
		{
			ItemAndListOfItems items = new ItemAndListOfItems(itemName, value);

			dlist = items;
		}

		public ItemAndListOfItems Dlist
		{
			get => dlist;
			set
			{
				if (Equals(value, dlist)) return;
				dlist = value;
				OnPropertyChanged();
			}
		}

		public override string ToString()
		{
			return $"{nameof(ClassOfItemAndfListOfItems)} {dlist.ToString()}";
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	// G
	public class TreeOfListOfTreeItems : INotifyPropertyChanged
	{
		private TreeItem gtree;

		public TreeOfListOfTreeItems(string itemName, string value)
		{
			this.gtree = new TreeItem(itemName, value);
		}

		public TreeItem Gtree
		{
			get => gtree;
			set
			{
				if (Equals(value, gtree)) return;
				gtree = value;
				OnPropertyChanged();
			}
		}

		public override string ToString()
		{
			return $"{nameof(TreeOfListOfTreeItems)} {gtree.ToString()}";
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	// initializers
	public class SampleData
	{
		// A
		public Item MakeItem(string preface)
		{
			return new Item($"{preface} item", $"item {preface}");
		}

		// B (or any List<item>
		public List<Item> MakeItemList(string type)
		{
			List<Item> list = new List<Item>();

			string idx;

			for (int i = 0; i < 3; i++)
			{
				idx = $"{type}{i}-";

				list.Add(MakeItem(idx));
			}

			return list;
		}

		// C or any ItemAndListOfItems
		public ItemAndListOfItems MakeItemAndListOfItems(string type)
		{
			ItemAndListOfItems item  = new ItemAndListOfItems(null, null);

			item.Citem = MakeItem("root");

			item.Clist = MakeItemList(type);

			return item;
		}

		// D
		public ClassOfItemAndfListOfItems MakeClassOfItemAndfListOfItems(string type)
		{
			ClassOfItemAndfListOfItems dList = new ClassOfItemAndfListOfItems(null, null);

			dList.Dlist = MakeItemAndListOfItems(type);

			return dList;
		}

		// E
		public  List<List<Item>> MakeListOfItemList(string type)
		{
			List<List<Item>> LList = new List<List<Item>>();
			List<Item> list = new List<Item>();
			Item item;
			string idx;

			for (int i = 0; i < 3; i++)
			{
				list = MakeItemList($"{type}{i}.");

				LList.Add(list);
			}

			return LList;
		}

		// F.1
		private TreeItem MakeTreeItem1(string preface)
		{
			TreeItem tItem = new TreeItem($"{preface} item", $"item {preface}");
			tItem.TreeItems = new List<TreeItem>();

			return tItem;
		}

		// F.2
		public TreeItem MakeTreeItem2(string type)
		{
			TreeItem tItem = MakeTreeItem1($"{type}");

			string idx;

			for (int i = 0; i < 3; i++)
			{
				idx = $"{type}-{i}";

				tItem.TreeItems.Add(MakeTreeItem1(idx));
			}

			return tItem;
		}

		// G.1
		public TreeOfListOfTreeItems MakeTreeOfListOfTreeItems(string type)
		{
			TreeOfListOfTreeItems tItem = new TreeOfListOfTreeItems(null, null);

			tItem.Gtree.Titem = MakeItem($"{type}-root");
			tItem.Gtree = MakeTree2(3, 3, 0, "G");

			return tItem;
		}

		// G.2
		private TreeItem MakeTree1(int qty, int depth, int actDepth, string type)
		{
			TreeItem ti = MakeTreeItem1(type);

			string subType = $"{type}-{actDepth + 1}";

			for (int i = 0; i < qty; i++)
			{
				if (depth > 0)
				{
					ti.TreeItems.Add(MakeTree1(qty, --depth, actDepth + 1, $"{subType}-{i + 1}"));
				}
				else
				{
					ti.TreeItems.Add(MakeTreeItem2($"{type}.{i + 1}"));
				}
			}

			return ti;
		}


		// G.3
		private TreeItem MakeTree2(int qty, int depth, int actDepth, string type)
		{
			TreeItem ti = MakeTreeItem1(type);

			string idx;

			for (int i = 0; i < qty; i++)
			{
				idx = $"{type}-{actDepth + 1}.{i + 1}";

				if (depth > 1)
				{
					ti.TreeItems.Add(MakeTree2(qty, depth - 1, actDepth + 1, idx));
				}
				else
				{
					ti.TreeItems.Add(MakeTreeItem1(idx));
				}
			}

			return ti;
		}

	}

	public class SampleDataForTreeItems
	{
		// need TreeData Info:  Name & Value

		// need to identify where in the tree the data belongs
		// the idea is that the initialization routine must create the
		// needed tree info items in order to fill the tree
		// per the location specified

		// need  TreeData Info: Location: 
		// parent node name / child node name / child node name / etc.
		// leaf position 0 through ??

		// treeInfo object
		// max parents == 1 = max one parent / 0 = unlimited
		// max levels deep == 1 max one level deep / 0 = unlimited
		// max nodes per level == 1 = max one leaf per node / 0 = unlimited
		// max leaves total == min 10 (max 10 leaves total) / 0 = unlimited

		public struct TreeDataData
		{
			public string LeafName { get; }
			public string LeafValue { get; }
			public int LeafPosition { get; }
			public string[] NodePath { get; }

			public TreeDataData(  string name, string value, int pos, string[] nodePath)
			{
				LeafName = name ?? "UDF";
				LeafValue = value ?? "UDF";
				LeafPosition = pos;
				NodePath = nodePath;
			}
		}

		// below are node names
		public const string P1 = "Parent1";
		public const string P2 = "Parent2";
		public const string P3 = "Parent3";


		public const string P1C1 = "P1-Child1";
		public const string P1C2 = "P1-Child2";

		public const string P2C1 = "P2-Child1";
		public const string P2C2 = "P2-Child2";
		public const string P2C3 = "P2-Child3";

		public const string P3C1 = "P1-Child1";
		public const string P3C2 = "P1-Child2";


		public const string P1C1SC1 = "P1C1-Sub-Child1";

		public const string P2C1SC1 = "P2C1-Sub-Child1";
		public const string P2C1SC2 = "P2C1-Sub-Child2";

		public const string P2C3SC1 = "P2C3-Sub-Child1";

		public const string P3C2SC1 = "P3C2-Sub-Child1";

		public TreeDataData[] Tdd { get; }

		public SampleDataForTreeItems()
		{
			Tdd = new TreeDataData[12];

			int idx = -1;


			Tdd[++idx] = new TreeDataData($"leaf {idx}", $"value {idx}", 1,
				new string[] { P2 } );

			Tdd[++idx] = new TreeDataData($"leaf {idx}", $"value {idx}", 0,
				new string[] { P3, P3C1 } );

			Tdd[++idx] = new TreeDataData($"leaf {idx}", $"value {idx}", 0,
				new string[] { P2, P2C1, P2C1SC1 } );

			Tdd[++idx] = new TreeDataData($"leaf {idx}", $"value {idx}", 1,
				new string[] { P2, P2C2 } );

			Tdd[++idx] = new TreeDataData($"leaf {idx}", $"value {idx}", 0,
				new string[] { P2, P2C1, P2C1SC1 } );

			Tdd[++idx] = new TreeDataData($"leaf {idx}", $"value {idx}", 2,
				new string[] { P1, P1C1, P1C1SC1 } );

			Tdd[++idx] = new TreeDataData($"leaf {idx}", $"value {idx}", 0,
				new string[] { P1, P1C2 } );

			Tdd[++idx] = new TreeDataData($"leaf {idx}", $"value {idx}", 0,
				new string[] { P2, P2C3, P2C3SC1 } );

			Tdd[++idx] = new TreeDataData($"leaf {idx}", $"value {idx}", 2,
				new string[] { P2, P2C1, P2C1SC2 } );

			Tdd[++idx] = new TreeDataData($"leaf {idx}", $"value {idx}", 0,
				new string[] { P3, P3C2, P3C2SC1 } );

			Tdd[++idx] = new TreeDataData($"leaf {idx}", $"value {idx}", 0,
				new string[] { P1 } );
		}
	}


	// show routines
	public class SampleDataShow
	{
		private ShDebugMessages m;

		public SampleDataShow(ShDebugMessages m)
		{
			this.m = m;
		}


		public void ShowA(Item item)
		{
			m.WriteLine("** Showing **", "Item");
			m.ShowRuler(4);
			m.MarginUp();
			m.WriteLineAligned("ItemName", item.ItemName);
			m.WriteLineAligned("Value", item.Value);
			m.MarginDn();
			m.WriteLine("** done **\n");
		}
	}
}