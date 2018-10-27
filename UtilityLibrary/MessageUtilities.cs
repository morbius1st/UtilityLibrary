using System;
using System.CodeDom;
using System.Diagnostics;
using System.Drawing;
using System.Text;
//using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using static UtilityLibrary.MessageUtilities.OutputLocation;
using static UtilityLibrary.MessageUtilities;


namespace UtilityLibrary
{
	public static class MessageUtilities2
	{
		public static string nl = Environment.NewLine;

		public static void logMsg2<T>(T var)
		{
			sendMsg(fmt(var));
		}

		public static void logMsg2(string msg1, string msg2 = "")
		{
			logMsgDb2(msg1??"", msg2??"");
		}
		
		public static void logMsgLn2<T>(T var)
		{
			sendMsg(fmt(var));
			sendMsg(nl);
		}

		public static void logMsgLn2(string msg1, string msg2 = "")
		{
			logMsgDbLn2(msg1??"", msg2??"");
		}

		public static void logMsgLn2<T>(string msg1, T msg2)
		{
			if (msg2 == null) msg2 = default(T);
			logMsgDbLn2(msg1??"", msg2);
		}

		public static void logMsgLn2(string msg1, int[] width, params string[] msgs)
		{
			logMsgDbLn2(msg1??"", width, msgs);
		}


	}

    public static class MessageUtilities
    {
		private static int DefColumn { get; set; } = 35;
		private static int DefColumn2 { get; set; } = 15;

	    public static string nl = Environment.NewLine;

//		private static RichTextBox Rtb { get; set; }

		public static OutputLocation OutLocation { get; set; } = DEBUG;

		public enum OutputLocation
		{
			TEXT_BOX = 0,
			CONSOLE = 1,
			DEBUG = 2
		}
	
		public static void ClearConsole()
		{
#if DEBUG
			
			DTE2 ide = (DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject("VisualStudio.DTE");

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

		public static string logMsgDbS(string msg1, string msg2 = "")
		{
			return logMsgFmtS(msg1 + "| ", msg2);
		}

		public static void logMsgDb2(string msg1, string msg2 = "")
		{
			logMsgFmt(msg1 + "| ", msg2);
		}

		public static void logMsgDbLn2(string msg1, string msg2 = "")
		{
			logMsgFmt(msg1 + "| ", msg2);
			logMsg(nl);
		}

		public static void logMsgDbLn2<T>(string msg1, T msg2)
		{
			logMsgFmt(msg1 + "| ", msg2.ToString());
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

		public static void logMsgDbLn2(string msg1, int[] width, params string[] msgs)
		{
			string msg2 = "";
			string msgTemp;
			int chk = -1;

			if (msgs != null)
			{ chk = (1 + msgs.Length) / 2 - 1; }

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

		private static void logMsgFmtln<T>(string msg1, T var1, int column = 0, int shift = 0)
		{
			logMsgFmt(msg1, fmt(var1), column, shift);
			logMsg(nl);
		}

		private static void logMsgFmt(string msg1)
		{
			logMsgFmt(msg1, "");
		}

		private static string logMsgFmtS<T>(string msg1,T var1,int column = 0,int shift = 0)
		{
			return fmtMsg(msg1,fmt(var1),column,shift);
		}

		private static void logMsgFmt<T>(string msg1, T var1, int column = 0, int shift = 0)
		{
			logMsg(fmtMsg(msg1, fmt(var1), column, shift));
		}

		private static void logMsgFmt<T>(string msg1, T var1, Color color, Font font, int column = 0, int shift = 0)
		{
			logMsg(fmtMsg(msg1, fmt(var1), column, shift), color, font);
		}

		private static string fmtMsg<T>(string msg1, T var1, int column = 0, int shift = 0)
		{
			if (column == 0) { column = DefColumn; }

			return string.Format(" ".Repeat(shift) + "{0," + column + "}{1}", msg1, fmt(var1));
		}

		private static void logMsgln<T1, T2>(T1 var1, T2 var2)
		{
			logMsg(fmt(var1));
			logMsgln(fmt(var2));
		}

		private static void logMsgln<T>(T var)
		{
			sendMsg(fmt(var));
			sendMsg(nl);
		}

		public static void logMsg<T1, T2>(T1 var1, T2 var2)
		{
			logMsg(fmt(var1));
			logMsg(fmt(var2));
		}

		public static void logMsg<T>(T var)
		{
			sendMsg(fmt(var));
		}

		private static void logMsg<T>(T var, Color color, Font font)
		{
			sendMsg(fmt(var), color, font);
		}

		public static void sendMsg(string msg, Color color = new Color(), Font font = null)
		{
//			if (OutLocation == 0 && Rtb == null) { OutLocation = DEBUG; }

			OutputLocation a = OutLocation;

			switch (OutLocation)
			{
//			case TEXT_BOX:
//				Rtb.AppendText(msg, color, font);
//				break;
			case CONSOLE:
				Console.Write(msg);
				break;
			default:
			case DEBUG:
				Debug.Write(msg);
				break;
			}
		}

		public static string[] splitPath(string fileAndPath)
		{
			return fileAndPath.Split('\\');
		}
	}

	public static class StringExtensions
	{

		public static string Repeat(this string s, int quantity)
		{
			if (quantity <= 0) return "";

			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < quantity; i++)
			{
				sb.Append(s);
			}

			return sb.ToString();
		}

		public static int CountSubstring(this string s, string substring)
		{
			int count = 0;
			int i = 0;
			while ((i = s.IndexOf(substring, i)) != -1)
			{
				i += substring.Length;
				count++;
			}

			return count;
		}

		public static int IndexOfToOccurance(this string s, string substring,
			int occurance, int start = 0)
		{
			if (s.Trim().Length == 0) { return -1; }
			if (occurance < 0) { return -1; }

			int pos = start;

			for (int count = 0; count < occurance; count++)
			{
				pos = s.IndexOf(substring, pos);

				if (pos == -1) { return pos; }

				pos += substring.Length;
			}
			return pos - substring.Length;
		}

		public static string GetSubDirectory(this string path, int requestedDepth)
		{
			requestedDepth++;
			if (requestedDepth == 0) { return "\\"; }

			path = path.TrimEnd('\\');

			int depth = path.CountSubstring("\\");

			if (requestedDepth > depth) { requestedDepth = depth; }

			int pos = path.IndexOfToOccurance("\\", requestedDepth);

			if (pos < 0) { return ""; }

			pos = path.IndexOfToOccurance("\\", requestedDepth + 1);

			if (pos < 0) { pos = path.Length; }

			return path.Substring(0, pos);
		}

		public static string GetSubDirectoryName(this string path, int requestedDepth)
		{
			requestedDepth++;

			path = path.TrimEnd('\\');

			if (requestedDepth > path.CountSubstring("\\")) { return ""; }

			string result = path.GetSubDirectory(requestedDepth - 1);

			if (result.Length == 0) { return ""; }

			int pos = path.IndexOfToOccurance("\\", requestedDepth) + 1;

			return result.Substring(pos);
		}
		
		public static string PadCenter(this string str, int totalLength, char padChar = '\u00A0')
		{
			int padAmount = totalLength - str.Length;

			if (padAmount <= 1)
			{
				if (padAmount == 1)
				{
					return str.PadRight(totalLength);
				}
				return str;
			}

			int padLeft = padAmount/2 + str.Length;

			return str.PadLeft(padLeft).PadRight(totalLength);
		}

	}
//
//	public static class RichTextBoxExtensions
//	{
//		public static void AppendText(this RichTextBox rtb, string text, Color color, Font font)
//		{
//			rtb.SuspendLayout();
//
//			if (color != new Color() || font != null)
//			{
//				rtb.SelectionStart = rtb.TextLength;
//				rtb.SelectionLength = 0;
//
//				if (color != null)
//				{
//					rtb.SelectionColor = color;
//				}
//				if (font != null)
//				{
//					rtb.SelectionFont = font;
//				}
//				rtb.AppendText(text);
//				rtb.SelectionColor = rtb.ForeColor;
//
//			}
//			else
//			{
//				rtb.AppendText(text);
//			}
//
//			rtb.ScrollToCaret();
//			rtb.ResumeLayout();
//		}
//	}
}


