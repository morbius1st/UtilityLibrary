// #define DML0 // not yet used
// #define DML1 // do not use here ** defined in properties *** start and end
// #define DML2 // turns on or off bool flags / button enable flags only / listbox idex set
// #define DML3 // various status messages
// #define DML4 // update status status messages
// #define DML5 // orator routines

#region + Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using UtilityLibrary;

#endregion

// user name: jeffs
// created:   6/23/2024 5:59:36 PM

namespace DebugCode
{
	public interface IWin
	{
		void DebugMsgLine(string msg);
		void DebugMsg(string msg);

	}
	
	public enum ShowWhere
	{
		NONE = -1,
		DEBUG = 0,    // debug window
		CONSOLE = 1,  // console window
		DBG_CONS = 2, // debug and console windows
		WPF_TBX = 3,  // wpf text box
		DBG_TBX = 4,  // debug and text box
	}

	[DebuggerStepThrough]
	public class DM
	{
	#if WPF
		public static IWin Iw { get; set; }
	#endif

		// number of debug channels
		public static int Quantity { get; set; }

		// typical preface width

		private const int ALT_PREFACE_WIDTH = -45; // when using file path and caller name
		private static int prefaceWidth = ALT_PREFACE_WIDTH;

		private const int MODULE_WIDTH = -110;
		private const int MODULE_WIDTH_A = -30;
		private const int MODULE_WIDTH_B = -45;
		private const int MODULE_WIDTH_C = 25;


		private const int CALLER_WIDTH = -70;
		private const int PRIOR_CALLER_WIDTH = -20;
		private const int MSG1_WIDTH = -8;
		private const int NOTES2_WIDTH = -16;

		public const string IN_OUT_STRING = "in-out";

		// dmx[x,0] = tab depth
		// dmx[x,1] = output location (per ShowWhere)
		public static int[,] dmx;

		private static int priorMarginIdx = -1;

	#if WPF
		public static void init(int qty, IWin iw)
	#else
		public static void init(int qty)
	#endif

		{
			Quantity = qty;

		#if WPF
			Iw = iw;
		#endif

			configDebugMsgList();
		}

		[DebuggerStepThrough]
		public static bool Start0(bool highlightlocation = false,
			string msg1 = null, string notes1 = null,
			[CallerMemberName] string caller = null,
			[CallerFilePath] string module = null)
		{
			if (highlightlocation) highlightLocation(0, caller);

			string priorCaller = new StackFrame(2, false).GetMethod().Name;

			/*
			StackFrame stack0 = new StackFrame(0, false);
			StackFrame stack1 = new StackFrame(1, false);
			StackFrame stack2 = new StackFrame(2, false);

			string className2 = stack2.GetMethod().ReflectedType.Name;
			string className1 = stack1.GetMethod().ReflectedType.Name;
			string callername1 = stack1.GetMethod().Name;
			// these are this method
			// string className0 = stack0.GetMethod().ReflectedType.Name;
			// string callername0 = stack0.GetMethod().Name;

			string priorpath = $"{className2} {priorCaller} -> {className1} {callername1}";
			*/

			string priorPath = getPriorPath();

			// DbxLineEx(0, "start", 0, 1, ShowWhere.NONE, notes1, caller, module, priorCaller);
			DbxLineEx(0, msg1, 0, 1, ShowWhere.NONE, notes1, caller, null, priorCaller, priorPath);

			return true;
			;
		}

		[DebuggerStepThrough]
		private static string getPriorPath()
		{
			StackFrame stack3 = new StackFrame(3, false);
			StackFrame stack2 = new StackFrame(2, false);

			string className3 = stack3.GetMethod().ReflectedType.Name;
			string callername3 = stack3.GetMethod().Name;
			string className2 = stack2.GetMethod().ReflectedType.Name;
			// string callername2 = stack2.GetMethod().Name;
			// these are this method
			// string className0 = stack0.GetMethod().ReflectedType.Name;
			// string callername0 = stack0.GetMethod().Name;

			return $"{className3, MODULE_WIDTH_A} . {callername3, MODULE_WIDTH_B} -> {className2,MODULE_WIDTH_C} |  ";
		}

		[DebuggerStepThrough]
		public static bool End0(string msg = null,
			string notes1 = null,
			[CallerMemberName] string caller = null,
			[CallerFilePath] string module = null)
		{
			string priorPath = getPriorPath();

			DbxLineEx(0, msg, -1, 0, ShowWhere.NONE, notes1, caller, null, null, priorPath);

			return true;
			;
		}

		[DebuggerStepThrough]
		public static bool InOut0(string msg = null,
			string notes1 = null,
			[CallerMemberName] string caller = null,
			[CallerFilePath] string module = null)
		{
			string priorPath = getPriorPath();

			DbxLineEx(0, msg, 0, 0, ShowWhere.NONE, notes1, $"< {caller}", null, null, priorPath);

			return true;
			;
		}


		[DebuggerStepThrough]
		public static bool Stat0(string msg = null,
			[CallerMemberName] string caller = null,
			[CallerFilePath] string module = null)
		{
			string priorPath = getPriorPath();
			DbxLineEx(0, "", 0, 0, ShowWhere.NONE, msg != null ? $"status {msg}" : null, caller, null, null, priorPath);

			return true;
			;
		}


		[DebuggerStepThrough]
		private static void highlightLocation(int idx, string msg)
		{
			msg = $"\n*** at {msg} ***\n\n";
			showDmx(idx, msg, ShowWhere.NONE);
		}

		[DebuggerStepThrough]
		public static void DbxSetDefaultWhere(int idx, ShowWhere where)
		{
			dmx[idx, 1] = (int) where;
		}


		[DebuggerStepThrough]
		public static void DbxSetIdx(int idx, int column)
		{
			dmx[idx, 0] = column;
		}


		[DebuggerStepThrough]
		public static void DbxChgIdx(int idx, int value)
		{
			dmx[idx, 0] += value;
		}

		[DebuggerStepThrough]
		public static void DbxLineEx1(int idx, string msg1,
			int chgIdxPre = 0,
			int chgIdxPost = 0,
			ShowWhere where = ShowWhere.NONE,
			string msg2 = null,
			[CallerMemberName] string mx = null,
			[CallerFilePath] string sx = null,
			string priorCaller = null
			)
		{
			if (dmx[idx, 0] < 0) return;

			string zx = null;

			if (sx != null)
			{
				sx = Path.GetFileNameWithoutExtension(sx) + " . ";
			}

			if (mx != null)
			{
				sx += mx;

				// if (msg1.StartsWith("start") || msg1.StartsWith("end"))
				// {
				// 	zx = $" ({caller})";
				// }
			}

			prefaceWidth = ALT_PREFACE_WIDTH;

			Dbx(idx, msg1, "\n", chgIdxPre, chgIdxPost, where, msg2, sx, zx, null, priorCaller);
		}


		[DebuggerStepThrough]
		public static void DbxLineEx(int idx,
			string msg1,
			int chgIdxPre = 0,
			int chgIdxPost = 0,
			ShowWhere where = ShowWhere.NONE,
			string notes1 = null,                    // addl notes
			[CallerMemberName] string caller = null, // caller
			[CallerFilePath] string module = null,   // module
			string priorCaller = null,
			string priorPath = null,
			string termination = "\n"
			)
		{
			if (dmx == null ||
				dmx[idx, 0] < 0) return;

			string zx = null;

			if (module != null)
			{
				module = Path.GetFileNameWithoutExtension(module);
			}
			else if (priorPath != null)
			{
				module = priorPath;
			}

			Dbx(idx, msg1, termination, chgIdxPre, chgIdxPost, where, notes1, module, zx, caller, priorCaller);

		}

		[DebuggerStepThrough]
		public static void DbxEx(int idx, string msg1,
			int chgIdxPre = 0,
			int chgIdxPost = 0,
			ShowWhere where = ShowWhere.NONE,
			string msg2 = null,
			[CallerMemberName] string mx = null,
			[CallerFilePath] string sx = null,
			string priorCaller = null,
			string priorPath = null)
		{

			DbxLineEx(idx, msg1, chgIdxPre, chgIdxPost, where, msg2, mx, sx, priorCaller, priorPath, "");

			//
			// if (dmx[idx, 0] < 0) return;
			//
			// string zx = null;
			//
			// if (sx != null)
			// {
			// 	sx = Path.GetFileNameWithoutExtension(sx) + " . ";
			// }
			//
			// if (mx != null)
			// {
			// 	sx += mx;
			//
			// 	if (msg1.StartsWith("start") || msg1.StartsWith("end"))
			// 	{
			// 		zx = $" ({mx})";
			// 	}
			// }
			//
			// Dbx(idx, msg1, null, chgIdxPre, chgIdxPost, where, msg2, sx, zx);
		}


		[DebuggerStepThrough]
		public static void DbxLine(int idx, string msg1,
			int chgIdxPre = 0,
			int chgIdxPost = 0,
			ShowWhere where = ShowWhere.NONE,
			string msg2 = null)
		{
			if (dmx[idx, 0] < 0) return;

			string s = MethodBase.GetCurrentMethod().DeclaringType.Name;

			Dbx(idx, msg1, "\n", chgIdxPre, chgIdxPost, where, msg2);
		}


		[DebuggerStepThrough]
		private static void showDmx(int idx, string msg, ShowWhere where)
		{
			ShowWhere w = where == ShowWhere.NONE ? (ShowWhere) dmx[idx, 1] : where;

			if (w == ShowWhere.CONSOLE || w == ShowWhere.DBG_CONS)
			{
				Console.Write(msg);
			}

			if (w == ShowWhere.DEBUG  || w == ShowWhere.DBG_CONS || w == ShowWhere.DBG_TBX)
			{
				Debug.Write(msg);
			}

			if (w == ShowWhere.WPF_TBX || w == ShowWhere.DBG_TBX)
			{
				Iw.DebugMsg(msg);
			}
		}

		[DebuggerStepThrough]
		public static void configDebugMsgList()
		{
			// setup the whole list
			dmx = new int[Quantity, 2];

			// 0 through 49
			for (var i = 0; i < Quantity; i++)
			{
				dmx[i, 0] = -1;
				dmx[i, 1] = (int) ShowWhere.DEBUG; // default location for all
			}
		}


		[DebuggerStepThrough]
		public static void Dbx(int idx,
			string msg1, // primary notes
			string t1,   // newline or not
			int chgIdxPre = 0,
			int chgIdxPost = 0,
			ShowWhere where = ShowWhere.NONE,
			string notes1 = null, // addl notes 1
			string module = null, // module name
			string notes2 = null, // addl notes 2
			string caller = null, // caller name
			string priorCaller = null)
		{
			if (dmx[idx, 0] < 0) return;

			string s1 = " ";
			// string s2 = "";
			// string s3;
			string s4 = "  ";

			// s2= $"{dmx[idx, 0],2:F0} +";

			dmx[idx, 0] = dmx[idx, 0] + chgIdxPre < 0 ? 0 : dmx[idx, 0] + chgIdxPre;

			int post = dmx[idx, 0] + chgIdxPost < 0 ? 0 : dmx[idx, 0] + chgIdxPost;

			// s2 += $"{chgIdxPre,2:F0} ={dmx[idx, 0],2:F0} +{chgIdxPost,2:F0} ={post,2:F0} ";

			// if (msg1.StartsWith(IN_OUT_STRING))
			// {
			// 	// s2 += " > < ";
			// }
			// else 
			if (post > priorMarginIdx)
			{
				// s2 += " >>  ";
				s1 = "> ";
			}
			else if (post < priorMarginIdx)
			{
				// s2 += "  << ";
				s1 = "< ";
			}
			else if (caller.StartsWith("<"))
			{
				s1 = ">";
			}
			else
			{
				s1 = "   ";
				s4 = "";
				// s2 += "     ";
			}

			// priorMarginIdx = dmx[idx, 0];
			priorMarginIdx = post;

			// string margin1 = "";
			string margin2 = "";

			if (dmx[idx, 0] > 0)
			{
				// margin1 = "  |".Repeat(dmx[idx, 0]);
				margin2 = "|  ".Repeat(dmx[idx, 0]);
			}

			string m;

			m = $"{module,MODULE_WIDTH}";

			caller = $"{margin2}{s1}{caller}{s4}";

			m += $"{caller,CALLER_WIDTH}";

			m += $"{msg1,MSG1_WIDTH}";

			// m += $"{priorCaller,PRIOR_CALLER_WIDTH}";

			// m += $"|{s2}";

			// s3 = $"{margin2}{msg1,-8}";
			//
			// m += $"{s3,MSG1_WIDTH} ";

			if (!notes2.IsVoid())
			{
				m += $" {notes2,NOTES2_WIDTH}";
			}

			if (!notes1.IsVoid())
			{
				m += $" {notes1}";
			}

			ShowWhere w = where == ShowWhere.NONE ? (ShowWhere) dmx[idx, 1] : where;

			dmx[idx, 0] = post;

			showDmx(idx, m + t1, w);
		}

		[DebuggerStepThrough]
		public static void DbxMsgLine(int idx, string msg1, ShowWhere where = ShowWhere.NONE)
		{
			DbxMsg(idx, msg1+"\n", where);
		}

		[DebuggerStepThrough]
		public static void DbxMsg(int idx, string msg1, ShowWhere where = ShowWhere.NONE)
		{
			showDmx(idx, msg1, where);
		}

	}

}