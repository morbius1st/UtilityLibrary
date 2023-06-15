using System;
using System.Runtime.Serialization;
using UtilityLibrary;

namespace SettingsManager
{
	public partial class Heading
	{
		public static string SuiteName = "SettingsManager";
		
		[DataMember(Order = 5)] public string Notes              = "created by v7.2";
		[DataMember(Order = 4)] public string DataClassVersion;
		[DataMember(Order = 6)] public string Description;
	}
}