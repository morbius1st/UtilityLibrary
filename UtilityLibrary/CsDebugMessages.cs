// #define DML0 // not yet used
// #define DML1 // do not use here ** defined in properties *** start and end
// #define DML2 // turns on or off bool flags / button enable flags only / listbox idex set
// #define DML3 // various status messages
// #define DML4 // update status status messages
// #define DML5 // orator routines

#region + Using Directives
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;



#endregion


// user name: jeffs
// created:   6/23/2024 5:59:36 PM

// namespace DebugCode
namespace UtilityLibrary
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

	// [DebuggerStepThrough]
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

		private const int MODULE_WIDTH = -100;  // space for module tree / margin for comments
		// private const int MODULE_WIDTH_A = -35; // left module column
		// private const int MODULE_WIDTH_B = -35; // center module column
		// private const int MODULE_WIDTH_C = 35; // right module column

		private const int MODULE_WIDTH_CHARS = 8;

		private const int MODULE_WIDTH_DL = 61;
		private const int MODULE_WIDTH_DLL = 30;
		private const int MODULE_WIDTH_DLR = MODULE_WIDTH_DL - MODULE_WIDTH_DLL - 1;
		private const int MODULE_WIDTH_DR = -(MODULE_WIDTH + MODULE_WIDTH_DL + MODULE_WIDTH_CHARS);




		private const int CALLER_WIDTH = -70;
		private const int PRIOR_CALLER_WIDTH = -20;
		private const int MSG1_WIDTH = -8;
		private const int NOTES2_WIDTH = -16;

		public const string IN_OUT_STRING = "in-out";

		// dmx[x,0] = tab depth
		// dmx[x,1] = output location (per ShowWhere)
		public static int[,] dmx;

		private static int priorMarginIdx = -1;

		public static bool Suspend { get; set; } = false;

		private static int sfA = 2;
		private static int sfB = 3;
		private static int sfC = 1;

	#if WPF
		public static void init(int qty, IWin iw)
	#else
		public static void init(int qty)
	#endif

		{
			Quantity = qty;

		#if WPF
			Iw = iw;
		#else
			sfA=1;
			sfB=2;
			sfC=3;
		#endif
			configDebugMsgList();
		}

		// [DebuggerStepThrough]
		public static bool Start0(bool highlightlocation = false,
			string msg1 = null, string notes1 = null,
			[CallerMemberName] string caller = null,
			[CallerFilePath] string module = null, 
			string suffix = null)
		{
			if (highlightlocation) highlightLocation(0, caller);

			StackFrame sf = new StackFrame(sfA, false);

			string priorCaller = sf?.GetMethod()?.Name ?? "null";

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
			DbxLineEx(0, suffix, msg1, 0, 1, ShowWhere.NONE, notes1, caller, null, priorCaller, priorPath);

			if (highlightlocation) showDmx(0, "\n", ShowWhere.NONE);

			return true;
		}

		public static bool Start0(string suffix, bool highlightlocation = false,
			string msg1 = null, string notes1 = null,
			[CallerMemberName] string caller = null,
			[CallerFilePath] string module = null)
		{

			if (highlightlocation) highlightLocation(0, caller);

			string priorCaller = new StackFrame(2, false).GetMethod().Name;

			string priorPath = getPriorPath();

			DbxLineEx(0, suffix, msg1, 0, 1, ShowWhere.NONE, notes1, caller, null, priorCaller, priorPath);

			if (highlightlocation) showDmx(0, "\n", ShowWhere.NONE);

			return true;
		}

		// [DebuggerStepThrough]
		public static bool End0(string msg = null,
			string notes1 = null,
			[CallerMemberName] string caller = null,
			[CallerFilePath] string module = null, string suffix = null)
		{
			string priorPath = getPriorPath();

			DbxLineEx(0, suffix, msg, -1, 0, ShowWhere.NONE, notes1, caller, null, null, priorPath);

			return true;

		}

		[DebuggerStepThrough]
		public static bool End0(string suffix, bool highlightlocation, string msg = null,
			string notes1 = null,
			[CallerMemberName] string caller = null,
			[CallerFilePath] string module = null)
		{
			string priorPath = getPriorPath();

			if (highlightlocation) highlightLocation(0, caller);

			DbxLineEx(0, suffix, msg, -1, 0, ShowWhere.NONE, notes1, caller, null, null, priorPath);

			if (highlightlocation) showDmx(0, "\n", ShowWhere.NONE);

			return true;

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
			bool highlightlocation=false,
			[CallerMemberName] string caller = null
			// , [CallerFilePath] string module = null
			)
		{
			if (highlightlocation) highlightLocation(0, caller);

			// string priorPath = getPriorPath();
			string priorPath = $"{"}  ",-MODULE_WIDTH}";

			DbxLineEx(0, "status", 0, 0, ShowWhere.NONE, msg != null ? $"{msg}" : null, caller, null, null, priorPath);

			return true;
			;
		}

		// [DebuggerStepThrough]
		private static string getPriorPath(string priorCaller = null, [CallerMemberName] string caller = null)
		{
			if (Suspend) return "";

			StackFrame stackA = new StackFrame(sfA, false);
			StackFrame stackB = new StackFrame(sfB, false);
			StackFrame stackC = new StackFrame(sfC, false);

			// string className3 = stackB.GetMethod()?.ReflectedType.Name ?? " ";
			// string callername3 = stackB.GetMethod()?.Name ?? " ";
			// string className2 = stackC.GetMethod()?.Name ?? " ";

			// string className2 = stackA.GetMethod()?.ReflectedType.Name ?? "null";


			// string nameA1 = stackA.GetMethod()?.Name ?? "null";
			string nameB1 = stackB.GetMethod()?.Name ?? " ";
			string nameB2 = stackB.GetMethod()?.ReflectedType.Name ?? " ";

			string nameC1 = stackC.GetMethod()?.Name ?? " ";
			string nameC2 = stackC.GetMethod()?.ReflectedType.Name ?? " ";

			if (false)
			{

				// string nameA1 = stackA.GetMethod()?.Name ?? "null";
				nameB1 = stackB.GetMethod()?.Name ?? " ";
				nameC1 = stackC.GetMethod()?.Name ?? " ";

				// string nameA2 = stackA.GetMethod()?.ReflectedType.Name ?? "null";
				nameB2 = stackB.GetMethod()?.ReflectedType.Name ?? " ";
				nameC2 = stackC.GetMethod()?.ReflectedType.Name ?? " ";

				Debug.WriteLine($"\nsfA {sfA} | sfB {sfB} | sfC {sfC}");
				Debug.WriteLine($"prior caller {priorCaller} | caller {caller}");
				// Debug.WriteLine($"stack A ({sfA}) | name    {nameA1,-30} | ref type name *** {nameA2}");
				Debug.WriteLine($"stack B ({sfB}) | name ** {nameB1,-30} | ref type name   * {nameB2}");
				Debug.WriteLine($"stack C ({sfC}) | name    {nameC1,-30} | ref type name     {nameC2}");


				Debug.WriteLine($"\n{$"{nameC2}.{nameC1}",-50}->{$"{nameB2}.{nameB1}",50} | ");
			}


			// string callername2 = stack2.GetMethod().Name;
			// these are this method
			// string className0 = stack0.GetMethod().ReflectedType.Name;
			// string callername0 = stack0.GetMethod().Name;

			int dlx = -(MODULE_WIDTH_DL - nameC1.Length);

			string dll = $"{{0, -{MODULE_WIDTH_DL - nameC1.Length}}}";
			string dlr = $"{{0, {nameC1.Length}}}";

			string s1 = string.Format(dll, dlx);
			string s2 = string.Format(dlr, nameC1.IsVoid() ? " " : nameC1);

			return $"{nameC2, -MODULE_WIDTH_DLL}.{nameC1, -MODULE_WIDTH_DLR} -> {$"{nameB2}",MODULE_WIDTH_DR} }} ";
			// return $"{$"{nameC2}.{nameC1}",MODULE_WIDTH_DL} -> {$"{nameB2}",MODULE_WIDTH_DR} }} ";
			// return $"{$"{nameC2}.{nameC1}",50} -> {$"{nameB2}.{nameB1}",-50} | ";

			// return $"{className3, MODULE_WIDTH_A} . {callername3, MODULE_WIDTH_B} -> {className2,MODULE_WIDTH_C} }}  ";
		}

		// [DebuggerStepThrough]
		private static void highlightLocation(int idx, string msg)
		{
			msg = $"{" ".Repeat(MODULE_WIDTH_DL)}{$"****** at {msg} ******",(MODULE_WIDTH+MODULE_WIDTH_DL)}\n";
			showDmx(idx, msg, ShowWhere.NONE);
		}

		// [DebuggerStepThrough]
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
			string termination = "\n", string suffix = null
			)
		{
			if (dmx == null ||
				dmx[idx, 0] < 0) return;

			string zx = null;


			// if (module != null)
			// {
			// 	module = Path.GetFileNameWithoutExtension(module);
			// }
			// else if (priorPath != null)
			// {
			// 	module = priorPath;
			// }

			if (priorCaller.IsVoid())
			{
				StackFrame sf = new StackFrame(sfB, false);
				priorCaller = sf.GetMethod()?.Name ?? "";
			}

			if (!module.IsVoid())
			{
				module = getPriorPath();
			} 
			else 
			{
				module = priorPath;
			}

			
			Dbx(idx, msg1, termination, chgIdxPre, chgIdxPost, where, notes1, module, zx, caller, priorCaller, suffix);

		}

		public static void DbxLineEx(int idx, string suffix, 
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
			DbxLineEx(idx, msg1, chgIdxPre, chgIdxPost, where, notes1, caller, module, priorCaller, priorPath,
				termination, suffix );

		}



		/*
		 *             = -100                 -70                 -8                   -16
		 *  >{module, MODULE_WIDTH}{caller, CALLER_WIDTH}{msg1, MSG1_WIDTH}{notes2,NOTES2_WIDTH} {notes1}
		 */
		// [DebuggerStepThrough]
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
			string priorCaller = null,
			string suffix = null)
		{
			if (dmx[idx, 0] < 0) return;

			string s1 = " ";
			string s4 = "  ";


			dmx[idx, 0] = dmx[idx, 0] + chgIdxPre < 0 ? 0 : dmx[idx, 0] + chgIdxPre;

			int post = dmx[idx, 0] + chgIdxPost < 0 ? 0 : dmx[idx, 0] + chgIdxPost;

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
				s1 = " - ";
				s4 = "";
			}

			priorMarginIdx = post;

			string margin2 = "";

			if (dmx[idx, 0] > 0)
			{
				margin2 = "|  ".Repeat(dmx[idx, 0]);
			}

			// the final string
			string m;

			m = $"{module,MODULE_WIDTH}";

			caller = $"{margin2}{s1}{caller}{s4} {suffix}";
			// caller = $"{margin2}{s1}{caller}{s4}";

			m += $"{caller,CALLER_WIDTH}";

			m += $"{msg1,MSG1_WIDTH}";

			// if (!priorCaller.IsVoid())
			// {
			// 	m += $" {priorCaller}";
			// }

			if (!notes2.IsVoid())
			{
				m += $" {notes2,NOTES2_WIDTH}";
			}

			if (!notes1.IsVoid())
			{
				m += $" {notes1}";
			}

			// ShowWhere w = where == ShowWhere.NONE ? (ShowWhere) dmx[idx, 1] : where;

			dmx[idx, 0] = post;

			showDmx(idx, m + t1, where);
		}


		// [DebuggerStepThrough]
		private static void showDmx(int idx, string msg, ShowWhere where)
		{
			if (Suspend) return;

			ShowWhere w = where == ShowWhere.NONE ? (ShowWhere) dmx[idx, 1] : where;

			if (w == ShowWhere.CONSOLE || w == ShowWhere.DBG_CONS)
			{
				Console.Write(msg);
			}

			if (w == ShowWhere.DEBUG  || w == ShowWhere.DBG_CONS || w == ShowWhere.DBG_TBX)
			{
				Debug.Write(msg);
			}

		#if WPF

			if (w == ShowWhere.WPF_TBX || w == ShowWhere.DBG_TBX)
			{
				Iw.DebugMsg(msg);
			}
			
		#endif
		}

		[DebuggerStepThrough]
		public static void DbxMsg(int idx, string msg1, ShowWhere where = ShowWhere.NONE)
		{
			showDmx(idx, msg1, where);
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



		// [DebuggerStepThrough]
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
		public static void DbxIncIdx(int idx)
		{
			dmx[idx, 0] += 1;
		}

		[DebuggerStepThrough]
		public static void DbxInc0()
		{
			dmx[0, 0] += 1;
		}

		[DebuggerStepThrough]
		public static void DbxDecIdx(int idx)
		{
			if (dmx[idx, 0]>0) dmx[idx, 0] -= 1;
		}

		[DebuggerStepThrough]
		public static void DbxDec0()
		{
			if (dmx[0, 0]>0) dmx[0, 0] -= 1;
		}

		[DebuggerStepThrough]
		public static void DbxChgIdx(int idx, int value)
		{
			dmx[idx, 0] += value;
		}

		// [DebuggerStepThrough]
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

		// [DebuggerStepThrough]
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
			if (priorPath.IsVoid())
			{
				priorPath = getPriorPath();
			}

			DbxLineEx(idx, msg1, chgIdxPre, chgIdxPost, where, msg2, mx, "", priorCaller, priorPath, "");

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
		public static void DbxMsgLine(int idx, string msg1, ShowWhere where = ShowWhere.NONE)
		{
			DbxMsg(idx, msg1+"\n", where);
		}


	}

}