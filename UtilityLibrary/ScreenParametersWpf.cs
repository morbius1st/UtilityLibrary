#region + Using Directives

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

#endregion

namespace UtilityLibrary
{
	public class ScreenParameters
	{
	#region + consts

		private const double NativeScreenDpi = 96.0;
		private const int    CCHDEVICENAME   = 32;

	#endregion

	#region + properties

		public int GetNativeScreenDpi
		{
			get => (int) NativeScreenDpi;
		}

	#endregion

	#region + public

		// private method to get the handle of the window
		// this keeps this class contained / not dependant
		public static IntPtr GetWindowHandle(Window win)
		{
			WindowInteropHelper winHelper = new WindowInteropHelper(win);
			winHelper.EnsureHandle();

			return new WindowInteropHelper(win).Handle;
		}

		// the actual screen DPI adjusted for the scaling factor
		public static int GetScreenDpi(Window win)
		{
			return GetDpiForWindow(GetWindowHandle(win));
		}

		// this is the ratio of the current screen Dpi
		// and the base Dpi
		public static double GetScreenScaleFactor(Window win)
		{
			// ReSharper disable once PossibleLossOfFraction
			return (GetScreenDpi(win) / NativeScreenDpi);
		}

		// this is the conversion factor between screen coordinates 
		// and sizes and their actual actual coordinate and size
		// e.g. for a screen set to 125%, this factor applied 
		// to the native screen dimensions, will provide the 
		// actual screen dimensions
		public static double GetScreenScalingFactor(Window win)
		{
			// ReSharper disable once PossibleLossOfFraction
			return (1 / (GetScreenDpi(win) / NativeScreenDpi));
		}

		// get the dimensions of the physical / native screen
		// ignoring any applied scaling
		public static Rectangle GetNativeScreenSize(Window win)
		{
			MONITORINFOEX mi = GetMonitorInfo(GetWindowHandle(win));

			return ConvertRectToRectangle(mi.rcMonitor);
		}

		public static Rectangle GetNativeWorkArea(Window win)
		{
			MONITORINFOEX mi = GetMonitorInfo(GetWindowHandle(win));

			return ConvertRectToRectangle(mi.rcWorkArea);
		}

		// get the screen dimensions taking the screen scaling into account
		public static Rectangle GetScaledScreenSize(Window win)
		{
			double ScalingFactor = GetScreenScalingFactor(win);

			Rectangle rc = GetNativeScreenSize(win);

			if (ScalingFactor == 1) return rc;

			return rc.Scale(ScalingFactor);
		}

		// get the screen dimensions taking the screen scaling into account
		public static Rectangle GetScaledWorkArea(Window win)
		{
			double ScalingFactor = GetScreenScalingFactor(win);

			Rectangle rc = GetNativeWorkArea(win);

			if (ScalingFactor == 1) return rc;

			return rc.Scale(ScalingFactor);
		}

		internal static MONITORINFOEX GetMonitorInfo(IntPtr ptr)
		{
			IntPtr hMonitor = MonitorFromWindow(ptr, 0);

			MONITORINFOEX mi = new MONITORINFOEX();
			mi.Init();
			GetMonitorInfo(hMonitor, ref mi);

			return mi;
		}

		public static Rectangle ConvertRectToRectangle(RECT rc)
		{
			return new Rectangle(rc.Left, rc.Top,
				rc.Right - rc.Left, rc.Bottom - rc.Top);
		}

	#endregion

	#region + Dll Imports

		[DllImport("user32.dll")]
		internal static extern UInt16 GetDpiForWindow(IntPtr hwnd);


		[DllImport("user32.dll")]
		internal static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);


		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

	#endregion

	#region + Dll Enums

		internal enum dwFlags : uint
		{
			MONITORINFO_PRIMARY = 1
		}

	#endregion

	#region + Dll Structs

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal struct MONITORINFOEX
		{
			public uint cbSize;
			public RECT rcMonitor;
			public RECT rcWorkArea;
			public dwFlags Flags;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
			public string DeviceName;

			public void Init()
			{
				this.cbSize = 40 + 2 * CCHDEVICENAME;
				this.DeviceName = String.Empty;
			}
		}

		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

	#endregion
	}

	public static class RectangleEx
	{
		internal static Rectangle Scale(this Rectangle rc, double scaleFactor)
		{
			return new Rectangle(
				(int) (rc.Left   * scaleFactor),
				(int) (rc.Top    * scaleFactor),
				(int) (rc.Width  * scaleFactor),
				(int) (rc.Height * scaleFactor));
		}

		public static string ToString(this Rectangle rc)
		{
			return string.Format("top|{0,5:D} left|{1,5:D} height|{2,5:D} width|{3,5:D}",
				rc.Top, rc.Left, rc.Height, rc.Width);
		}
	}
}