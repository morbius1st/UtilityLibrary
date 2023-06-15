using System;
using System.Runtime.Serialization;
using UtilityLibrary;

namespace SettingsManager
{
	// [DataContract(Namespace =  N_SPACE)]
	public partial class Heading
	{
		// public static string ClassVersionName = nameof(DataClassVersion);
		// public static string SystemVersionName = nameof(SystemVersion);
  //       
  //
		// public const string N_SPACE = "";
  //
		// public Heading(string dataClassVersion) => DataClassVersion = dataClassVersion;
  //
		// [DataMember(Order = 1)] public string SaveDateTime       = DateTime.Now.ToString("yyyy-MM-dd - HH:mm zzz");
		// [DataMember(Order = 2)] public string AssemblyVersion    = CsUtilities.AssemblyVersion;
		// [DataMember(Order = 3)] public string SystemVersion      = "7.0";

		public static string SuiteName = "SettingsManager";
		
		[DataMember(Order = 5)] public string Notes              = "created by v7.0";
		[DataMember(Order = 4)] public string DataClassVersion;
		[DataMember(Order = 6)] public string Description;
	}
}