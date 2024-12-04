// projname: UtilityLibrary
// itemname: ApiCalls.cs
// username: jeffs
// created:  1/9/2024

using System.Runtime.InteropServices;
using System;

namespace UtilityLibrary
{
	public class ApiCalls
	{

		// note codes:
		// 0x01 == escape
		public static void SendKeyCode(ushort code)
		{
			Input[] inputs = new Input[]
			{
				new Input
				{
					type = (int)InputType.Keyboard,
					u = new InputUnion
					{
						ki = new KeyboardInput
						{
							wVk = 0,
							wScan = code, // esc
							dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
							dwExtraInfo = GetMessageExtraInfo()
						}
					}
				},
				new Input
				{
					type = (int)InputType.Keyboard,
					u = new InputUnion
					{
						ki = new KeyboardInput
						{
							wVk = 0,
							wScan = code, // esc
							dwFlags = (uint)(KeyEventF.KeyUp | KeyEventF.Scancode),
							dwExtraInfo = GetMessageExtraInfo()
						}
					}
				}
			};

			SendInput((uint) inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
		}


		[DllImport("User32.dll", SetLastError = true)]
		private static extern int SendInput(uint nInputs, Input[] inputs, int cbSize);

		[DllImport("user32.dll")]
		private static extern IntPtr GetMessageExtraInfo();


		[StructLayout(LayoutKind.Sequential)]
		public struct MouseInput
		{
			public int dx;
			public int dy;
			public uint mouseData;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct HardwareInput
		{
			public uint uMsg;
			public ushort wParamL;
			public ushort wParamH;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct KeyboardInput
		{
			public ushort wVk;   // virtural-key code
			public ushort wScan; // a hardware scan code
			public uint dwFlags; // specifies various aspects of the keystroke
			public uint time;    // time stamp for the event.  if zero, system will provide
			public IntPtr dwExtraInfo;  // any additional value associated with the keystroke
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct InputUnion
		{
			[FieldOffset(0)] public MouseInput mi;
			[FieldOffset(0)] public KeyboardInput ki;
			[FieldOffset(0)] public HardwareInput hi;
		}

		public struct Input
		{
			public int type;
			public InputUnion u;
		}

		[Flags]
		public enum InputType
		{
			Mouse = 0,
			Keyboard = 1,
			Hardware = 2
		}

		[Flags]
		public enum KeyEventF
		{
			KeyDown = 0x0000,
			ExtendedKey = 0x0001,
			KeyUp = 0x0002,
			Unicode = 0x0004,
			Scancode = 0x0008
		}

		[Flags]
		public enum MouseEventF
		{
			Absolute = 0x8000,
			HWheel = 0x01000,
			Move = 0x0001,
			MoveNoCoalesce = 0x2000,
			LeftDown = 0x0002,
			LeftUp = 0x0004,
			RightDown = 0x0008,
			RightUp = 0x0010,
			MiddleDown = 0x0020,
			MiddleUp = 0x0040,
			VirtualDesk = 0x4000,
			Wheel = 0x0800,
			XDown = 0x0080,
			XUp = 0x0100
		}
	}
}