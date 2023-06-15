#region + Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#endregion

// user name: jeffs
// created:   4/21/2023 6:42:49 PM

namespace ShCode.SampleData
{
	// simple flat item which holds a second flat data item
	public class ItemSample : INotifyPropertyChanged
	{
		private DictDataItem dictData;
		public string Namex { get; set; }
		public string Desc { get; set; }

		public DictDataItem DictData
		{
			get => dictData;
			set
			{
				dictData = value;
				OnPropertyChanged();
			}
		}

		public ItemSample(string namex, string desc, DictDataItem dictData)
		{
			Namex = namex;
			Desc = desc;
			DictData = dictData;

		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

		public override string ToString()
		{
			return $"name| {Namex ?? "null"} | dictdata.name| {dictData?.Name ?? "null"}";
		}
	}

	public class SettingSample : INotifyPropertyChanged
	{
		public string Version { get; set; } = "1.0.0.0";

		public SettingSampleData SetgSampleData {get; set; }

		public SettingSampleTree SetgSampleTree { get; set; }

		public SettingSample()
		{
			SetgSampleData = new SettingSampleData();

			OnPropertyChanged(nameof(SetgSampleData));

			DictDataItem di = DictionaryDataSample.SampleDictionary.ElementAt(0).Value;

			SetgSampleTree = new SettingSampleTree("Root", new ItemSample("root item", "root item", di));

			addSampleTree(SetgSampleTree);

			OnPropertyChanged(nameof(SetgSampleTree));

		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}


		private int[] treeSampleCounts = new [] {2,2,3 };


		private void addSampleTree(SettingSampleTree tree)
		{
			tree.TreeItems = new List<SettingSampleTree>();

			for (int idx = 0; idx < treeSampleCounts.Length; idx++)
			{
				string pf = ((char) (65 + idx)).ToString();

				SettingSampleTree tr = addSampleTreeItem(idx, pf);

				tr.TreeItems = new List<SettingSampleTree>();

				for (int i = 0; i < treeSampleCounts[idx]; i++)
				{
					SettingSampleTree tr2 = addSampleTreeItem((idx+1) * 10 + i, pf);
					tr2.TreeItems = new List<SettingSampleTree>();

					for (int j = 0; j < 2; j++)
					{
						SettingSampleTree tr3 = addSampleTreeItem((idx+1) * 100 + j, pf);
						tr2.TreeItems.Add(tr3);
					}

					tr2.NotifyTreeItemsUpdate();

					tr.TreeItems.Add(tr2);
				}

				tr.NotifyTreeItemsUpdate();

				tree.TreeItems.Add(tr);
			}

			tree.NotifyTreeItemsUpdate();
		}

		private SettingSampleTree addSampleTreeItem(int idx, string preface)
		{
			string id = $"{preface}{idx}";
			string name = $"Tree Name {id}";
			DictDataItem di = DictionaryDataSample.SampleDictionary.ElementAt(0).Value;

			ItemSample ix = new ItemSample($"Tree Item Name {id}", $"Tree Item Desc {id}", di);

			return new SettingSampleTree(name, ix);
		}



	}

	// class to hold and populate a flat list of ItemSamples
	public class SettingSampleData : INotifyPropertyChanged
	{
		public List<ItemSample> ItemSampleList {get; set; }

		public SettingSampleData()
		{
			ItemSampleList = new List<ItemSample>();

			ItemSample ixs;

			ixs = new ItemSample("is name 1", "is desc 1", DictionaryDataSample.SampleDictionary["setting type 1"]);
			ItemSampleList.Add(ixs);

			ixs = new ItemSample("is name 2", "is desc 2", DictionaryDataSample.SampleDictionary["setting type 2"]);
			ItemSampleList.Add(ixs);

			ixs = new ItemSample("is name 3", "is desc 3", DictionaryDataSample.SampleDictionary["setting type 3"]);
			ItemSampleList.Add(ixs);

			ixs = new ItemSample("is name 4", "is desc 4", DictionaryDataSample.SampleDictionary["setting type 4"]);
			ItemSampleList.Add(ixs);

			OnPropertyChanged(nameof(ItemSampleList));

		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}
	}

	// multi-level item that holds a a flat ItemSample 
	// and holds a list of itself
	public class SettingSampleTree : INotifyPropertyChanged
	{
		private ItemSample sampleItem;
		private List<SettingSampleTree> treeItems;
		public string Sname { get; set; }

		public ItemSample SampleItem
		{
			get => sampleItem;
			set
			{
				sampleItem = value;
				OnPropertyChanged();
			}
		}

		public List<SettingSampleTree> TreeItems
		{
			get => treeItems;
			set
			{
				treeItems = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

		public SettingSampleTree(string sname, ItemSample sampleItem)
		{
			Sname = sname;
			SampleItem = sampleItem;
		}

		public void NotifyTreeItemsUpdate()
		{
			OnPropertyChanged(nameof(TreeItems));
		}

		public override string ToString()
		{
			return $"name| {Sname} | count| {TreeItems?.Count ?? 0}";
		}
	}



}
