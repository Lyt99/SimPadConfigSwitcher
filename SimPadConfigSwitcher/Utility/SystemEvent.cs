/*
 * The Code is from https://www.codeproject.com/Tips/559023/Directly-Hook-to-System-Events-using-managed-code
 * Using The Code Project Open License (CPOL) License
 */

namespace SimPadConfigSwitcher
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
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern System.IntPtr SetWinEventHook(uint eventMin, uint eventMax, System.IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        private delegate void WinEventDelegate(System.IntPtr hWinEventHook, uint eventType, System.IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        public event SystemEventEventHandler SystemEventHandler;
        public delegate void SystemEventEventHandler(System.IntPtr hWinEventHook, uint eventType, System.IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
        private uint m_event = 0;
        private WinEventDelegate m_delegate = null;

        private System.IntPtr m_foregroundHwnd = System.IntPtr.Zero;
        public SystemEvent(SystemEvents SystemEvent)
        {
            m_event = System.Convert.ToUInt32(SystemEvent);
            m_delegate = new WinEventDelegate(WinEventProc);
            try
            {
                SetWinEventHook(m_event, m_event, System.IntPtr.Zero, m_delegate, System.Convert.ToUInt32(0), System.Convert.ToUInt32(0), WINEVENT_OUTOFCONTEXT);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        public void WinEventProc(System.IntPtr hWinEventHook, uint eventType, System.IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if ((((SystemEventHandler != null)) && (SystemEventHandler.GetInvocationList().Length > 0)))
            {
                m_foregroundHwnd = hwnd;
                if (SystemEventHandler != null)
                {
                    SystemEventHandler(hWinEventHook, eventType, hwnd, idObject, idChild, dwEventThread, dwmsEventTime);
                }
            }
        }

        public System.IntPtr Hwnd
        {
            get { return m_foregroundHwnd; }
        }
    }
}
