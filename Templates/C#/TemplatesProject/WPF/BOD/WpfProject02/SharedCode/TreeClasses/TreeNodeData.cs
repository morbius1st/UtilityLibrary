// Solution:     WpfProject02
// Project:       WpfProject02
// File:             TreeNodeData.cs
// Created:      2023-10-05 (7:32 PM)

using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfProject02.Annotations;

namespace SharedCode.TreeClasses;

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