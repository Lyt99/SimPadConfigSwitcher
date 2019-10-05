/*
 * The Code is from https://www.codeproject.com/Tips/559023/Directly-Hook-to-System-Events-using-managed-code
 * Using The Code Project Open License (CPOL) License
 */

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SimPadConfigSwitcher.Utility
{
    public enum SystemEvents : uint
    {
        EVENT_SYSTEM_FOREGROUND = 3, //Active Foreground Window
        EVENT_SYSTEM_CAPTURESTART = 8, //Active Foreground Window Mouse Capture
        EVENT_OBJECT_CREATE = 32768, //An object has been created. The system sends this event for the following user interface elements: caret, header control, list-view control, tab control, toolbar control, tree view control, and window object.
        EVENT_OBJECT_DESTROY = 32769, //An object has been destroyed. The system sends this event for the following user interface elements: caret, header control, list-view control, tab control, toolbar control, tree view control, and window object. 
        EVENT_OBJECT_FOCUS = 32773 //An object has received the keyboard focus. The system sends this event for the following user interface elements: list-view control, menu bar, pop-up menu, switch window, tab control, tree view control, and window object.
    }


    public class SystemEvent
    {
        private const uint WINEVENT_OUTOFCONTEXT = 0;
        
        // Win APi
        [DllImport("user32.dll")]
        private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);
        [DllImport("user32.dll")]
        private static extern bool UnhookWinEvent(IntPtr hWinEventHook);
        public const int PROCESS_ALL_ACCESS = 0x000F0000 | 0x00100000 | 0xFFF;
        [DllImport("User32.dll")]
        public extern static int GetWindowThreadProcessId(IntPtr hWnd, ref int lpdwProcessId);
        [DllImport("Kernel32.dll")]
        public extern static IntPtr OpenProcess(int fdwAccess, int fInherit, int IDProcess);
        [DllImport("Kernel32.dll")]
        public extern static bool CloseHandle(IntPtr hObject);
        [DllImport("psapi.dll")]
        static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, [Out] StringBuilder lpBaseName, [In] [MarshalAs(UnmanagedType.U4)] int nSize);

        private IntPtr hWinEventHook;
        private SystemEvents eventType;

        private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        public event SystemEventEventHandler SystemEventHandler;

        public delegate void SystemEventEventHandler(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        private uint m_event = 0;
        private WinEventDelegate m_delegate = null;

        public SystemEvent(SystemEvents SystemEvent)
        {
            eventType = SystemEvent;
        }

        public void Start()
        {
            m_event = Convert.ToUInt32(eventType);
            m_delegate = new WinEventDelegate(WinEventProc);

            hWinEventHook = SetWinEventHook(m_event, m_event, IntPtr.Zero, m_delegate, Convert.ToUInt32(0), Convert.ToUInt32(0), WINEVENT_OUTOFCONTEXT);
        }

        public void Stop()
        {
            if(hWinEventHook != IntPtr.Zero)
            {
                UnhookWinEvent(hWinEventHook);
            }

            hWinEventHook = IntPtr.Zero;
            this.Hwnd = IntPtr.Zero;
        }

        public void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if ((((SystemEventHandler != null)) && (SystemEventHandler.GetInvocationList().Length > 0)))
            {
                Hwnd = hwnd;
                SystemEventHandler?.Invoke(hWinEventHook, eventType, hwnd, idObject, idChild, dwEventThread, dwmsEventTime);
            }
        }

        public string GetApplicationPath()
        {
            if (this.Hwnd == IntPtr.Zero) return string.Empty;

            int pId = 0;

            GetWindowThreadProcessId(this.Hwnd, ref pId);
            IntPtr pHandle = OpenProcess(1040, 0, pId);

            StringBuilder sb = new StringBuilder(512);
            GetModuleFileNameEx(pHandle, IntPtr.Zero, sb, 512);

            CloseHandle(pHandle);

            return sb.ToString();
        }

        public IntPtr Hwnd { get; private set; } = IntPtr.Zero;

        ~SystemEvent()
        {
            this.Stop();
        }
    }
}
