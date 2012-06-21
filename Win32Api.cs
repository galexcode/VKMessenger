using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text;

namespace IMV
{
    // Desktop Windows Manager �����
    internal class DwmApi
    {
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hWnd, DWM_BLURBEHIND pBlurBehind); // �������� ����� Aero

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, MARGINS pMargins); // ��������� � ��������� ������� ������������

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled(); // ��������� ������� �� ����� Aero

        [StructLayout(LayoutKind.Sequential)]
        public class MARGINS
        {
            public int cxLeftWidth, // ����� �� ����� ������� ����
                cxRightWidth, // ����� �� ������ ������� ����
                cyTopHeight, // ������ �� ������� ������� ����
                cyBottomHeight; // ������ �� ������ ������� ����

            public MARGINS(int left, int top, int right, int bottom)
            {
                cxLeftWidth = left; cyTopHeight = top;
                cxRightWidth = right; cyBottomHeight = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class DWM_BLURBEHIND
        {
            public uint dwFlags;
            public bool fEnable;
            public IntPtr hRegionBlur;
            public bool fTransitionOnMaximized;

            public const uint DWM_BB_ENABLE = 0x00000001;
            public const uint DWM_BB_BLURREGION = 0x00000002;
            public const uint DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004;
        }
    }
}