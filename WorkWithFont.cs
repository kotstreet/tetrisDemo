using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public static class WorkWithFont
{
    [StructLayout(LayoutKind.Sequential)]
    public struct COORD
    {
        public short X;
        public short Y;

        public COORD(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class CONSOLE_FONT_INFO_EX
    {
        public CONSOLE_FONT_INFO_EX()
        {
            cbSize = Marshal.SizeOf<CONSOLE_FONT_INFO_EX>();
        }

        public int cbSize;
        public int nFont;
        public COORD dwFontSize;
        public int FontFamily;
        public int FontWeight;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string FaceName;
    }


    public const int STD_OUTPUT_HANDLE = -11;

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public extern static bool GetCurrentConsoleFontEx(
           IntPtr hConsoleOutput,
           bool bMaximumWindow,
           [In, Out] CONSOLE_FONT_INFO_EX lpConsoleCurrentFont);


    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool SetCurrentConsoleFontEx(
       IntPtr ConsoleOutput,
       bool MaximumWindow,
       CONSOLE_FONT_INFO_EX ConsoleCurrentFontEx
       );

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int dwType);
}
