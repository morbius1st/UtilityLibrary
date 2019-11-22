using System;
using System.Diagnostics;
using System.Drawing;
using static UtilityLibrary.CsExtensions;

#if	CONSOLE
using EnvDTE;
using EnvDTE80;
#endif

#if FORMS
using System.Windows.Forms;
#endif



namespace UtilityLibrary
{
	public static class MessageUtilities2
	{
		public static string nl = Environment.NewLine;

		public static void logMsg2<T>(T var)
		{
			MessageUtilities.sendMsg(MessageUtilities.fmt(var));
		}

		public static void logMsg2(string msg1,
			string msg2 = ""
			)
		{
			MessageUtilities.logMsgDb2(msg1 ?? "", msg2 ?? "");
		}

		public static void logMsgLn2<T>(T var)
		{
			MessageUtilities.sendMsg(MessageUtilities.fmt(var));
			MessageUtilities.sendMsg(nl);
		}

		public static void logMsgLn2(string msg1 = "",
			string msg2 = ""
			)
		{
			MessageUtilities.logMsgDbLn2(msg1 ?? "", msg2 ?? "");
		}

		public static void logMsgLn2<T>(string msg1,
			T msg2
			)
		{
			if (msg2 == null) msg2 = default(T);
			MessageUtilities.logMsgDbLn2(msg1 ?? "", msg2);
		}

		public static void logMsgLn2(string msg1,
			int[] width,
			params string[] msgs
			)
		{
			MessageUtilities.logMsgDbLn2(msg1 ?? "", width, msgs);
		}
	}

	public static class MessageUtilities
	{
		private static int DefColumn  { get; set; } = 35;
		private static int DefColumn2 { get; set; } = 15;

		public static string nl = Environment.NewLine;

		public static int AltColumn { get; set; } = -1;

	#if FORMS
		public static RichTextBox RichTxtBox { get; set; }
	#endif


		public static OutputLocation OutLocation { get; set; } = OutputLocation.DEBUG;

		public enum OutputLocation
		{
			TEXT_BOX = 0,
			CONSOLE  = 1,
			DEBUG    = 2
		}

		public static void ClearConsole()
		{
		#if	CONSOLE
			DTE2 ide = (DTE2) System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE");

			OutputWindow w = ide.ToolWindows.OutputWindow;

			w.ActivePane.Activate();
			w.ActivePane.Clear();
		#endif
		}

		public static string fmtInt(int i)
		{
			return $"{i,-4}";
		}

		public static string fmtColor(Color c)
		{
			return "";
		}

		public static string fmtColorRGB(uint u)
		{
			Color c = Color.FromArgb((int) u);
			return $"{c.A:D3} {c.B:D3} {c.G:D3} {c.R:D3}";
		}

		public static string fmt(Color c)
		{
			return $"{c.ToArgb():x8}";
		}

		public static string fmt<T>(T var)
		{
			if (var is int)
			{
				return fmtInt(Convert.ToInt32(var));
			}

			return var.ToString();
		}

		public static void logMsgFmtln(string msg1)
		{
			logMsgFmtln(msg1, "");
		}

		public static void logMsgFmtln<T>(string msg1,
			T msg2
			)
		{
			logMsgDbLn2(msg1, msg2);
		}

		public static string logMsgDbS(string msg1,
			string msg2 = ""
			)
		{
			return logMsgFmtS(msg1 + "| ", msg2);
		}

		public static void logMsgDb2(string msg1,
			string msg2 = ""
			)
		{
			logMsgFmt(msg1 + "| ", msg2);
		}

		public static void logMsgDbLn2(string msg1,
			string msg2 = ""
			)
		{
			if (!string.IsNullOrEmpty(msg1) ||
				!string.IsNullOrEmpty(msg2))
			{
				logMsgFmt(msg1 + "| ", msg2);
			}

			logMsg(nl);
		}

		public static void logMsgDbLn2<T>(string msg1,
			T msg2
			)
		{
			if (!string.IsNullOrEmpty(msg1))
			{
				logMsgFmt(msg1 + "| ", msg2?.ToString() ?? "is null");
			}

			logMsg(nl);
		}


		/// <summary>
		/// lists the prompt followed by the description / message pairs
		/// the message is adjusted to be the width provided
		/// do not provide a width for the last message
		/// </summary>
		/// <param name="msg1">The preface message</param>
		/// <param name="width">the width to use for each message parameter</param>
		/// <param name="msgs">a list of description / message pairs</param>
		public static void logMsgDbLn2(string msg1,
			int[] width,
			params string[] msgs
			)
		{
			string msg2 = "";
			string msgTemp;
			int    chk = -1;

			if (msgs != null)
			{
				chk = (1 + msgs.Length) / 2 - 1;
			}

			if (width == null || chk != width.Length)
			{
				logMsgFmtln(msg1 + "| ", "malformed parameters");
				return;
			}

			int m = 0;
			int c = 0;

			do
			{
				if (msgs[m] == null) break;
				msgTemp = msgs[m++];

				int w = msgTemp.Length + 2;


				if (m < msgs.Length)
				{
					msgTemp += "| " + msgs[m++];
				}

				if (c != width.Length)
				{
					w += width[c++];

					msg2 += string.Format("{0,-" + w + "}", msgTemp);
				}
				else
				{
					msg2 += msgTemp;
				}
			}
			while (m < msgs.Length);

			logMsgFmt(msg1 + "| ", msg2);
			logMsg(nl);
		}

		private static void logMsgFmtln<T>(string msg1,
			T var1,
			int column = 0,
			int shift = 0
			)
		{
			logMsgFmt(msg1, fmt(var1), column, shift);
			logMsg(nl);
		}

		private static void logMsgFmt(string msg1)
		{
			logMsgFmt(msg1, "");
		}

		private static string logMsgFmtS<T>(string msg1,
			T var1,
			int column = 0,
			int shift = 0
			)
		{
			return fmtMsg(msg1, fmt(var1), column, shift);
		}

		private static void logMsgFmt<T>(string msg1,
			T var1,
			int column = 0,
			int shift = 0
			)
		{
			logMsg(fmtMsg(msg1, fmt(var1), column, shift));
		}

		private static void logMsgFmt<T>(string msg1,
			T var1,
			Color color,
			Font font,
			int column = 0,
			int shift = 0
			)
		{
			logMsg(fmtMsg(msg1, fmt(var1), column, shift), color, font);
		}

		public static string fmtMsg<T>(string msg1,
			T var1,
			int column = 0,
			int shift = 0
			)
		{
			if (column == 0) { column = DefColumn; }

			if (AltColumn > 0) column = AltColumn;

			return string.Format(CsExtensions.Repeat(" ", shift) + "{0," + column + "}{1}", msg1, fmt(var1));
		}

		private static void logMsgln<T1, T2>(T1 var1,
			T2 var2
			)
		{
			logMsg(fmt(var1));
			logMsgln(fmt(var2));
		}

		private static void logMsgln<T>(T var)
		{
			sendMsg(fmt(var));
			sendMsg(nl);
		}

		public static void logMsg<T1, T2>(T1 var1,
			T2 var2
			)
		{
			logMsg(fmt(var1));
			logMsg(fmt(var2));
		}

		public static void logMsg<T>(T var)
		{
			sendMsg(fmt(var));
		}

		private static void logMsg<T>(T var,
			Color color,
			Font font
			)
		{
			sendMsg(fmt(var), color, font);
		}

		public static void sendMsg(string msg,
			Color color = new Color(),
			Font font = null
			)
		{
		#if FORMS
			if (OutLocation == OutputLocation.TEXT_BOX && RichTxtBox == null)
		#endif
//			{ OutLocation = OutputLocation.DEBUG; }
//			OutputLocation a = OutLocation;

			switch (OutLocation)
			{
			case OutputLocation.TEXT_BOX:
			#if FORMS
					RichTxtBox.AppendText(msg, color, font);
			#endif
				break;

			case OutputLocation.CONSOLE:
				Console.Write(msg);
				break;
			default:
			case OutputLocation.DEBUG:
				Debug.Write(msg);
				break;
			}
		}

		public static string[] splitPath(string fileAndPath)
		{
			return fileAndPath.Split('\\');
		}
	}


#if FORMS
	public static class rtbExtension
	{
		public static void AppendText(this RichTextBox rtb, string msg,
			Color color,
			Font font = null)
		{
			Color currColor = rtb.ForeColor;
			Font  currFont  = rtb.Font;

			if (!color.IsEmpty) rtb.ForeColor = color;
			if (font != null) rtb.Font        = font;

			rtb.AppendText(msg);

			if (!color.IsEmpty) rtb.ForeColor = currColor;
			if (font != null) rtb.Font        = currFont;
		}
	}
#endif
}