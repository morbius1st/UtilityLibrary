#region using
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using SharedWPF.ShWin;

using UtilityLibrary;
#endregion

// username: jeffs
// created:  10/2/2022 10:05:58 AM


namespace SharedCode.ShDebugAssist
{
	public class ShDebugMessages
	{
	#region private fields


		private int marginSize = 0;
		private int marginSpaceSize = 2;
		private string intraMargin;
		private string location;

		#endregion

		#region ctor

			public ShDebugMessages(IWinMsg winMsgs)
			{
				Messages = winMsgs;

				IntraMarginClr();
			}

	#endregion

	#region public methods


		public IWinMsg Messages { get; set; }

		// public string MessageBoxText { get; set; }

		protected int ColumnWidth { get; set; } = 30;


		public void ShowRuler(int cols)
		{
			StringBuilder[] sb = new StringBuilder[2];
			sb[0] = new StringBuilder();
			sb[1] = new StringBuilder();

			for (int i = 0; i < cols; i++)
			{
				sb[0].Append($"{i}         ");
				sb[1].Append("0123456789");
			}

			WriteLine(sb[0].ToString());
			WriteLine(sb[1].ToString());
		}


		// write text & NO CRLF to
		// msgBox (==0) by default
		[DebuggerStepThrough]
		public void Write(string msg1, int msgBox=0)
		{
			writeMsg(msg1, msgBox);
		}

		// write text + CRLF to
		// msgBox (==0) by default
		[DebuggerStepThrough]
		public void WriteLine(string msg1, string msg2 = "", int msgBox=0)
		{
			writeMsg(msg1, msg2 + "\n", msgBox);
		}

		// adds a margin to the front of each line
		// margin is per 'marginSize'
		[DebuggerStepThrough]
		public void WriteLineMargin(string msg1, string msg2 = "", string spacer = " ", int msgBox=0)
		{
			writeMsg(msg1, msg2 + "\n",  spacer, msgBox);
		}

		// adds a margin to the front of each line
		// margin is per 'marginSize'
		[DebuggerStepThrough]
		public void WriteMargin(string msg1, string msg2 = "", string spacer = " ", int msgBox=0)
		{
			writeMsg(msg1, msg2,  spacer, msgBox);
		}




		
		[DebuggerStepThrough]
		public void WriteMsg(string msg1, string msg2 = "", int msgBox=0)
		{
			writeMsg(msg1, msg2, msgBox);
		}

		[DebuggerStepThrough]
		public void IntraMarginClr()
		{
			intraMargin = " ".Repeat(marginSpaceSize);
		}

		[DebuggerStepThrough]
		public void MsgClr(int msgBox=0)
		{
			clrMsg(msgBox);
		}

		[DebuggerStepThrough]
		public void MarginClr()
		{
			marginSize = 0;
		}

		[DebuggerStepThrough]
		public void MarginUp()
		{
			marginSize += marginSpaceSize;
		}

		[DebuggerStepThrough]
		public void MarginDn()
		{
			marginSize -= marginSpaceSize;

			if (marginSize < 0) marginSize = 0;
		}

		[DebuggerStepThrough]
		public void NewLine(int msgBox=0)
		{
			writeMsg("\n", msgBox);
		}

		[DebuggerStepThrough]
		public void WriteAligned(string msg1, string msg2 = "", string spacer = " ", int msgBox=0)
		{
			writeMsg(msg1, msg2, spacer, msgBox);
		}

		[DebuggerStepThrough]
		public void WriteLineAligned(string msg1, string msg2 = "", string spacer = " ", int msgBox=0)
		{
			writeMsg(msg1, msg2 + "\n", spacer, msgBox);
		}

		private string priorLocation = "<at begining>";

		[DebuggerStepThrough]
		public void WriteLineStatus(string msg1, [CallerFilePath] string membPath = "",
			[CallerMemberName] string membName = "")
		{
			string location = $"{Path.GetFileNameWithoutExtension(membPath),-25} :: {membName}";

			// string msg = $"> {location,-60} | {msg1, -60} | [ {priorLocation} ]";
			string msg = $"> {location,-60} | {msg1, -60}";

			priorLocation = location;

			writeMsg(msg + "\n", 1);
		}


		[DebuggerStepThrough]
		public void WriteLineCodeMap(string msg1 = "", [CallerMemberName] string membName = "")
		{
			string m1 = msg1.IsVoid() ? "" : $" | {msg1}";
			string msg = $"{membName,-40} {m1}";

			writeMsg(msg + "\n", 2);
		}

		[DebuggerStepThrough]
		public void WriteLineSpaced(string msg1, string msg2 = "", string spacer = " ", int msgBox=0)
		{
			writeMsg(msg1, msg2 + "\n", msgBox, spacer);
		}


		// temp turned off for now
		[DebuggerStepThrough]
		public void WriteLineSteps(string msg1, string msg2 = "", string spacer = " ", int msgBox=0)
		{
			writeMsg(msg1, msg2 + "\n", msgBox, spacer);
			return;
		}

		/// <summary>
		/// Adds a formatted message to the app msg text box and<br/>
		/// to the debug window.  Does not include a new line<br/>
		/// use Show() to have the message shown<br/>
		/// msgA = Message title (for app and debug)<br/>
		/// msgB = Message text (for app)<br/>
		/// msgD = Message text (for debug)<br/>
		/// loc  = Code location (optional)<br/>
		/// </summary>
		/// <param name="msgA">Message title (for app and debug)</param>
		/// <param name="msgB">Message text (for app)</param>
		/// <param name="msgD">Message text (for debug)</param>
		/// <param name="loc">Code location (optional)</param>
		/// <param name="colWidth">Width for the title column (app only) (optional)</param>
		[DebuggerStepThrough]
		public void WriteDebugMsg(string msgA, string msgB, string msgC = null, string msgD = null, int colWidth = -1, int msgBox=0)
		{
			writeMsg(msgA, msgB, msgC, msgBox, colWidth);
			Debug.Write(fmtMsg(msgA, msgD ?? msgB));
		}

		/// <summary>
		/// Adds a formatted message to the app msg text box and<br/>
		/// to the debug window.  Include a new line<br/>
		/// use Show() to have the message shown<br/>
		/// msgA = Message title (for app and debug)<br/>
		/// msgB = Message text (for app)<br/>
		/// msgD = Message text (for debug)<br/>
		/// loc  = Code location (optional)<br/>
		/// </summary>
		/// <param name="msgA">Message title (for app and debug)</param>
		/// <param name="msgB">Message text (for app)</param>
		/// <param name="msgC"></param>
		/// <param name="msgD">Message text (for debug)</param>
		/// <param name="loc">Code location (optional)</param>
		/// <param name="colWidth">Width for the title column (app only) (optional)</param>
		[DebuggerStepThrough]
		public void WriteDebugMsgLine(string msgA, string msgB, string msgC = null, string msgD = null, int colWidth = -1, int msgBox=0)
		{
			writeMsg(msgA, msgB + "\n", msgC, msgBox, colWidth);
			Debug.WriteLine(fmtMsg(msgA, msgD ?? msgB));
		}





	#endregion

	#region private methods
		
		// the primary "writeMsg"
		// msgBox
		// 0 == message box
		// 1 == status box
		// 2+ = code map box
		[DebuggerStepThrough]
		public void writeMsg(string msg, int msgBox)
		{
			if (msgBox == 0)
			{
				Messages.MessageBoxText += msg;
			}
			else if (msgBox == 1)
			{
				Messages.StatusBoxText = msg + Messages.StatusBoxText;
			}
			else
			{
				Messages.CodeMapText = msg + Messages.CodeMapText;
			}
			
		}

		[DebuggerStepThrough]
		private void clrMsg(int msgBox)
		{
			if (msgBox == 0)
			{
				Messages.MessageBoxText = "";
			}
			else
			{
				Messages.StatusBoxText = "";
			}
		}

		[DebuggerStepThrough]
		private void writeMsg(string msg1, string msg2, int msgBox)
		{
			writeMsg(fmtMsg(msg1, msg2), msgBox);
		}

		[DebuggerStepThrough]
		private void writeMsg(string msg1, string msg2, int msgBox, string spacer = " ")
		{
			writeMsg(fmtMsg(msg1, msg2, spacer), msgBox);
		}

		[DebuggerStepThrough]
		private void writeMsg(string msg1, string msg2, int msgBox, int colWidth = -1)
		{
			writeMsg(fmtMsg(msg1, msg2, colWidth), msgBox);
		}

		[DebuggerStepThrough]
		private void writeMsg(string msg1, string msg2, string spacer = "", int msgBox = 0, int colWidth = -1)
		{
			writeMsg(margin(spacer) + fmtMsg(msg1, msg2, colWidth), msgBox);
		}

		[DebuggerStepThrough]
		private string margin(string spacer)
		{
			if (marginSize == 0) return "";

			return spacer.Repeat(marginSize);
		}

		[DebuggerStepThrough]
		private string intramargin(string spacer)
		{
			if (marginSize == 0) return " ";

			return " "+spacer.Repeat(marginSize);
		}

		[DebuggerStepThrough]
		private string fmtMsg(string msg1, string msg2, string spacer)
		{
			// string mx = marginSize.ToString("000");
			string partA = msg1.IsVoid() ? msg1 : msg1;
			string partC = msg2.IsVoid() ? null : intramargin(spacer);
			string partB = msg2.IsVoid() ? msg2 : msg2;

			return $"{partA}{partC}{partB}";
		}

		// type x
		// [DebuggerStepThrough]
		private string fmtMsg(string msg1, string msg2, int colWidth = -1)
		{
			string partA = msg1.IsVoid() ? msg1 : $"{msg1}".PadRight(colWidth == -1 ? ColumnWidth : colWidth);
			string partB = msg2.IsVoid() ? msg2 : " " + $"{msg2}";

			// return partA + partB;
			return $"{partA}{partB}";
		}



	#endregion

	#region system overrides

		public override string ToString()
		{
			return "this is DebugSupport";
		}

	#endregion
	}
}