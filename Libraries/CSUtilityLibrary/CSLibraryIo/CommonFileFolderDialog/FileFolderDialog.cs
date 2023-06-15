using System.Collections.Generic;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace CSLibraryIo.CommonFileFolderDialog
{
	public class FileAndFolderDialog
	{

	#region private fields

		private CommonOpenFileDialog cfd;

		private bool selectingFolder = false;

		private bool multiselect = false;

		private FileAndFolderDialogSettings settings;
		private string title = "Select an item";
		private string startingFolder;

	#endregion

	#region ctor

		public FileAndFolderDialog()
		{
			ResetDialog();
		}

	#endregion


	#region public properties

		/// <summary>
		/// the title for the dialog box
		/// </summary>
		public string Title
		{
			get => title ?? cfd.Title;
			set => title = value;
		}

		/// <summary>
		/// the folder to start in<br/>
		/// if null, 
		/// </summary>
		public string StartingFolder
		{
			get => startingFolder ?? cfd.InitialDirectory;
			set => startingFolder = value;
		}

		/// <summary>
		/// access to the dialog to allow access to other properties,<br/>
		/// methods, or events
		/// </summary>
		public CommonOpenFileDialog Dialog => cfd;

		/// <summary>
		/// group of common settings
		/// </summary>
		public FileAndFolderDialogSettings Settings
		{
			get => settings;
			set => settings = value;
		}

		/// <summary>
		/// Access to the filters collection
		/// </summary>
		public FileDialogFilterCollection Filters => settings.Filters;

		/// <summary>
		/// the result of the selection operation
		/// </summary>
		public CommonFileDialogResult Result { get; private set; }

		/// <summary>
		/// determines if a selection was made
		/// </summary>
		public bool HasSelection { get; private set; }

	#endregion

	#region public methods

		// public int CalculateSum(int a, int b)
		// {
		// 	return a + b;
		// }
		//

		public string GetFile(string title = null, string startingFolder = null,
			FileAndFolderDialogSettings settings = null)
		{
			if (title != null) this.title = title;
			if (startingFolder != null) this.startingFolder = startingFolder;
			if (settings != null) this.settings = settings;

			// these are the pre-set values
			// selectingFolder = false;
			// multiselect = false;

			// preset in settings
			// showPlacesList = true;
			// allowNonFileSysItems = false;
			// allowPropertyEditing = false;

			if (!SelectFileOrFolder()) return null;

			return cfd.FileName;
		}

		public IEnumerable<string> GetFiles(string title = null, string startingFolder = null,
			FileAndFolderDialogSettings settings = null)
		{
			if (title != null) this.title = title;
			if (startingFolder != null) this.startingFolder = startingFolder;
			if (settings != null) this.settings = settings;

			multiselect = true;

			// these are the pre-set values
			// selectingFolder = false;

			// pre-set in settings
			// showPlacesList = true;
			// allowNonFileSysItems = false;
			// allowPropertyEditing = false;


			if (!SelectFileOrFolder()) return null;

			return cfd.FileNames;
		}

		public string GetFolder(string title = null, string startingFolder = null,
			FileAndFolderDialogSettings settings = null)
		{
			if (title != null) this.title = title;
			if (startingFolder != null) this.startingFolder = startingFolder;
			if (settings != null) this.settings = settings;

			selectingFolder = true;

			// these are the pre-set values
			// multiselect = false;

			// pre-set in settings
			// showPlacesList = true;
			// allowNonFileSysItems = false;
			// allowPropertyEditing = false;

			if (!SelectFileOrFolder()) return null;

			return cfd.FileName;
		}

		public IEnumerable<string> GetFolders(string title = null, string startingFolder = null,
			FileAndFolderDialogSettings settings = null)
		{
			if (title != null) this.title = title;
			if (startingFolder != null) this.startingFolder = startingFolder;
			if (settings != null) this.settings = settings;

			selectingFolder = true;
			multiselect = true;

			//pre-set in settings
			// showPlacesList = true;
			// allowNonFileSysItems = false;
			// allowPropertyEditing = false;

			if (!SelectFileOrFolder()) return null;

			return cfd.FileNames;
		}

	#endregion


		private bool SelectFileOrFolder()
		{
			HasSelection = false;

			try
			{
				using (cfd)
				{
					cfd.Title = Title;

					if (!selectingFolder && settings.Filters != null)
					{
						foreach (CommonFileDialogFilter filter in settings.Filters)
						{
							cfd.Filters.Add(filter);
						}
					}

					cfd.InitialDirectory = StartingFolder;

					cfd.IsFolderPicker = selectingFolder;
					cfd.Multiselect = multiselect;
					cfd.ShowPlacesList = settings.ShowPlacesList;
					cfd.AllowNonFileSystemItems = settings.AllowNonFileSysItems;
					cfd.AllowPropertyEditing = settings.AllowPropertyEditing;
					cfd.AddToMostRecentlyUsedList = settings.AddToMostRecentlyUsedList;

					Result = cfd.ShowDialog();

					if (Result != CommonFileDialogResult.Ok) return false;

					HasSelection = true;
				}
			}
			catch
			{
				return false;
			}

			return true;
		}

	#region private fields

		private void ResetDialog()
		{
			cfd = new CommonOpenFileDialog();

			settings = new FileAndFolderDialogSettings();

			Result = CommonFileDialogResult.None;

			HasSelection = false;
		}

	#endregion

	#region Overrides

		public override string ToString()
		{
			return "this is CSLibraryIo.CommonFileFolderDialog.FileAndFolderDialog";
		}

	#endregion

		public class FileAndFolderDialogSettings
		{
		#region private fields

			private FileDialogFilterCollection filters = null;

		#endregion

		#region public fields

			/// <summary>
			/// <br/>
			/// Determines if the places list is displayed
			/// </summary>
			/// <remarks>
			/// Default is True
			/// </remarks>
			public bool ShowPlacesList = true;

			/// <summary>
			/// <br/>
			/// Determines if user can select non-filesystem items (e.g. libraries)
			/// </summary>
			/// <remarks>
			/// Default is False
			/// </remarks>
			public bool AllowNonFileSysItems = false;


			/// <summary>
			/// <br/>
			/// Determines if user can edit properties
			/// </summary>
			/// <remarks>
			/// Default is False
			/// </remarks>
			public bool AllowPropertyEditing = false;

			/// <summary>
			/// <br/>
			/// Controls whether to show or hide the list of places from which the user has recently opened or saved items
			/// </summary>
			/// <remarks>
			/// Default is False
			/// </remarks>
			public bool AddToMostRecentlyUsedList = false;

			public FileDialogFilterCollection Filters
			{
				get
				{
					if (filters == null)
					{
						filters = new FileDialogFilterCollection();
					}

					return filters;
				}


			}

		#endregion

		#region properties

			public bool HasFilters => Filters != null && Filters.Count > 0;

		#endregion
		}

		
	}

	public class FileDialogFilterCollection : List<CommonFileDialogFilter> { }

}