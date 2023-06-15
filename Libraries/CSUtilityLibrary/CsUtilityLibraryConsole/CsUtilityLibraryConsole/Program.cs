using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSLibraryIo.CommonFileFolderDialog;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace CsUtilityLibraryConsole
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			FileAndFolderDialog fd = new FileAndFolderDialog();

			Console.WriteLine("\ngetting a file");

			FileAndFolderDialog.FileAndFolderDialogSettings fdSetg = new FileAndFolderDialog.FileAndFolderDialogSettings();

			fdSetg.Filters.Add(new CommonFileDialogFilter("A File", "*.xlsx"));

			string file = fd.GetFile("This is a title", null, fdSetg);

			Console.WriteLine("\ngot a file?| " + fd.HasSelection);
			Console.WriteLine("file got   | " + file ?? "none");

			Console.WriteLine("\nwaiting    | ");
			Console.ReadKey();

			fd = new FileAndFolderDialog();
			fd.Title = "this is a new title";
			fd.Filters.Add(new CommonFileDialogFilter("test", @"*.*"));
			fd.Settings.AddToMostRecentlyUsedList = true;

			file = fd.GetFile();

			Console.WriteLine("\ngot a file?| " + fd.HasSelection);
			Console.WriteLine("file got   | " + file ?? "none");

			Console.WriteLine("\nwaiting    | ");
			Console.ReadKey();

			return;

			TaskDialog td = new TaskDialog();

			td.Caption = "This is an example TaskDialog";
			td.InstructionText = "This is the main instruction";
			td.Text = "This is the TaskDialog's Text";
			td.Icon = TaskDialogStandardIcon.Information;

			TaskDialogResult result = td.Show();


			Console.WriteLine("\nwaiting    | ");
			Console.ReadKey();

		}
	}
}
