#region + Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SharedCode.ShDebugAssist;
using SharedWPF.ShWin;
#endregion

// user name: jeffs
// created:   4/21/2023 6:27:48 PM

namespace ShCode.SampleData
{
	// simple flat item with Name, Description, & Value
	// setting options
	public class DictDataItem : INotifyPropertyChanged
	{
		private string name;


		public string Name
		{
			get => name;
			set
			{
				name = value;
				OnPropertyChanged();
			}
		}

		public string Description { get; set; }
		public int Value { get; set; }
		

		public DictDataItem(string name, string description, int value)
		{
			Name = name;
			Description = description;
			Value = value;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

		public override string ToString()
		{
			return $"name| {Name}";
		}
	}

	// just the collection definition
	// simple flat dictionary of items
	public static class DictionaryDataSample
	{
		public static string SampleName { get; set; } = "SampleName";
		public static string SampleDescription { get; set; } = "SampleDescription";
		public static int SampleValue { get; set; } = 100;
		public static Dictionary<string, DictDataItem> SampleDictionary { get; set; }

		static DictionaryDataSample()
		{
			InitDict();
		}

		public static void InitDict()
		{
			SampleDictionary = new Dictionary<string, DictDataItem>();

			DictDataItem di;

			di = new DictDataItem("setting type 1", "desc1", 101);
			SampleDictionary.Add(di.Name, di);
			di = new DictDataItem("setting type 2", "desc2", 102);
			SampleDictionary.Add(di.Name, di);
			di = new DictDataItem("setting type 3", "desc3", 103);
			SampleDictionary.Add(di.Name, di);
			di = new DictDataItem("setting type 4", "desc4", 104);
			SampleDictionary.Add(di.Name, di);
			di = new DictDataItem("setting type 5", "desc5", 105);
			SampleDictionary.Add(di.Name, di);
			di = new DictDataItem("setting type 6", "desc6", 106);
			SampleDictionary.Add(di.Name, di);
		}

		public static string ToString()
		{
			return $"name| {SampleName}";
		}
	}

	// simple class to show the dictionary provided
	public class AssistDict
	{
		private ShDebugMessages Dm;

		public AssistDict(IWinMsg m)
		{
			Dm = new ShDebugMessages(m);
			
		}

		public void ShowDict1(Dictionary<string, DictDataItem> d1)
		{
			foreach (KeyValuePair<string, DictDataItem> kvp in d1)
			{
				Dm.WriteLine($"key| {kvp.Key} | into| {kvp.Value.Name}  {kvp.Value.Description}");
			}
		}
	}

}
