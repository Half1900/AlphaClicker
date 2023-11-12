using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;

namespace AlphaClicker
{
    public class WinApi
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(int character);

        [DllImport("user32.dll")]
        public static extern int SetCursorPos(int x, int y);

        // Get custom coords
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);






        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);



        // 导入 PostMessage 函数
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // 定义常量
        const uint WM_LBUTTONDOWN = 0x0201;
        const uint WM_LBUTTONUP = 0x0202;
        const uint WM_RBUTTONDOWN = 0x0204;
        const uint WM_RBUTTONUP = 0x0205;

        const uint WM_MBUTTONDOWN = 0x0207;
        const uint WM_MBUTTONUP = 0x0208;

        const int MK_LBUTTON = 0x0001;

        // 定义辅助函数
        static int MAKELONG(int low, int high)
        {
            return (high << 16) | (low & 0xffff);
        }



        private static IntPtr hWnd;

        /// <summary>
        /// 发送窗口的点击位置消息
        /// </summary>
        private static IntPtr lParam;

        private readonly static IntPtr wParam = new IntPtr(MK_LBUTTON);

        private static int x, y;

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }

        public static void DoClick(string button, bool useCustomCoords, int X, int Y,bool bg)
        {
            if (useCustomCoords)
            {
                if (bg)
                {
                    if (x != X || y != Y)
                    {
                        hWnd = WindowFromPoint(new POINT { X = X, Y = Y });
                        x = X;
                        y = Y;


                        RECT rect;
                        GetWindowRect(hWnd, out rect);

                        X -= rect.Left;
                        Y -= rect.Top;
                        lParam = new IntPtr(MAKELONG(X, Y));

                    }
                    switch (button)
                    {
                        case "左键":
                            SendMessage(hWnd, WM_LBUTTONDOWN, IntPtr.Zero, lParam);
                            SendMessage(hWnd, WM_LBUTTONUP, IntPtr.Zero, lParam);
                            break;
                        case "右键":
                            SendMessage(hWnd, WM_RBUTTONDOWN, IntPtr.Zero, lParam);
                            SendMessage(hWnd, WM_RBUTTONUP, IntPtr.Zero, lParam);
                            break;
                        case "中键":
                            SendMessage(hWnd, WM_MBUTTONDOWN, IntPtr.Zero, lParam);
                            SendMessage(hWnd, WM_MBUTTONUP, IntPtr.Zero, lParam);
                            break;
                    }
                    //SendMessage(hWnd, WM_LBUTTONDOWN, IntPtr.Zero, lParam);
                    //SendMessage(hWnd, WM_LBUTTONUP, IntPtr.Zero, lParam);
                }
                else
                {
                    SetCursorPos(X, Y);
                    switch (button)
                    {
                        case "左键":
                            mouse_event((uint)MOUSEEVENTF.LEFTDOWN | (uint)MOUSEEVENTF.LEFTUP, 0, 0, 0, 0);
                            break;
                        case "右键":
                            mouse_event((uint)MOUSEEVENTF.RIGHTDOWN | (uint)MOUSEEVENTF.RIGHTUP, 0, 0, 0, 0);
                            break;
                        case "中键":
                            mouse_event((uint)MOUSEEVENTF.MIDDLEDOWN | (uint)MOUSEEVENTF.MIDDLEUP, 0, 0, 0, 0);
                            break;
                    }
                }

            }
            else
            {
                switch (button)
                {
                    case "左键":
                        mouse_event((uint)MOUSEEVENTF.LEFTDOWN | (uint)MOUSEEVENTF.LEFTUP, 0, 0, 0, 0);
                        break;
                    case "右键":
                        mouse_event((uint)MOUSEEVENTF.RIGHTDOWN | (uint)MOUSEEVENTF.RIGHTUP, 0, 0, 0, 0);
                        break;
                    case "中键":
                        mouse_event((uint)MOUSEEVENTF.MIDDLEDOWN | (uint)MOUSEEVENTF.MIDDLEUP, 0, 0, 0, 0);
                        break;
                }
            }
             
        }
    }
}
