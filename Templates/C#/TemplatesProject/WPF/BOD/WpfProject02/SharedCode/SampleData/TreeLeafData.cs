// Solution:     WpfProject02
// Project:       WpfProject02
// File:             TreeLeafData.cs
// Created:      2023-10-05 (7:32 PM)

using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfProject02.Annotations;

namespace SharedCode.SampleData;

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