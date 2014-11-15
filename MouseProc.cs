using System;
using System.Threading;
using System.Runtime.InteropServices;

namespace SerialBT
{
	public class MouseProc
	{
		#region DllImport
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetCursorPos(out POINT lpPoint);

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int X;
			public int Y;

			public POINT(int x, int y)
			{
				this.X = x;
				this.Y = y;
			}
		}

		[DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
		static extern void SetCursorPos(int X, int Y);

		[DllImport("USER32.dll", CallingConvention = CallingConvention.StdCall)]
		static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		/// <summary>マウス左ボタンDown</summary>
		private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
		/// <summary>マウス左ボタンUp</summary>
		private const int MOUSEEVENTF_LEFTUP = 0x0004;
		/// <summary>マウス右ボタンDown</summary>
		private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
		/// <summary>マウス右ボタンUp</summary>
		private const int MOUSEEVENTF_RIGHTUP = 0x0010;

		#endregion

		public MouseProc()
		{
		}

		/// <summary>
		/// カーソル位置を設定する
		/// </summary>
		/// <param name="moveX">X座標移動分</param>
		/// <param name="moveY">Y座標移動分</param>
		public void SetCursor(int moveX, int moveY)
		{
			POINT p;
			if (GetCursorPos(out p))
			{
				SetCursorPos(p.X + moveX, p.Y + moveY);
			}

		}

		/// <summary>
		/// 左クリック
		/// </summary>
		public void LeftClick()
		{
			mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
			mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
		}

		/// <summary>
		/// 右クリック
		/// </summary>
		public void RightClick()
		{
			mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
			mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
		}
	}
}