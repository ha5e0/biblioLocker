using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; 

using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Diagnostics;
using Microsoft.Win32;

namespace WindowsFormsApplication1
{
    class Lock
    {
        public bool isLocked = false;

        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);

        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        [DllImport("user32.dll")]
        public static extern int FindWindowEx(int parentHandle, int childAfter, string className, int windowTitle);

        [DllImport("user32.dll")]
        private static extern int GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern int ExitWindowsEx(ExitWindows uFlags);
            
        [Flags]
        public enum ExitWindows : uint
        {
            // ONE of the following five:
            LogOff = 0x00,
            ShutDown = 0x01,
            Reboot = 0x02,
            PowerOff = 0x08,
            RestartApps = 0x40,
            // plus AT MOST ONE of the following two:
            Force = 0x04,
            ForceIfHung = 0x10,
        }

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;

        protected static int HandleOfTaskBar
        {
            get
            {
                return FindWindow("Shell_TrayWnd", "");
            }
        }

        protected static int HandleOfStartButton
        {
            get
            {
                int handleOfDesktop = GetDesktopWindow();
                int handleOfStartButton = FindWindowEx(handleOfDesktop, 0, "button", 0);
                return handleOfStartButton;
            }
        }

        public void ShowTaskBar()
        {
            ShowWindow(HandleOfTaskBar, SW_SHOW);
            ShowWindow(HandleOfStartButton, SW_SHOW);
        }

        public void HideTaskBar()
        {
            ShowWindow(HandleOfTaskBar, SW_HIDE);
            ShowWindow(HandleOfStartButton, SW_HIDE);
        }

        public void KillCtrlAltDelete()
        {
            RegistryKey regkey;
            string keyValueInt = "1";
            string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";

            try
            {
                regkey = Registry.CurrentUser.CreateSubKey(subKey);
                regkey.SetValue("DisableTaskMgr", keyValueInt);
                regkey.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void EnableCTRLALTDEL()
        {
            try
            {
                string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
                RegistryKey rk = Registry.CurrentUser;
                RegistryKey sk1 = rk.OpenSubKey(subKey);
                if (sk1 != null)
                    rk.DeleteSubKeyTree(subKey);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void LogOff() {
            ExitWindowsEx(ExitWindows.LogOff | ExitWindows.Force);
        }

        public void lockOn()
        {
            hook();
            KillCtrlAltDelete();
            HideTaskBar();
            hideAllWidnows();
            isLocked = true;
        }
        
        public void lockOff()
        {
            unhook();
            EnableCTRLALTDEL();
            ShowTaskBar();
            showAllWidnows();
            isLocked = false;
        }







        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);

        [DllImport("user32.dll")]
        static extern int GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
                    int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode,
            IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);


                // Is Control being held down?
                bool control = ((GetAsyncKeyState(Keys.Control) & 0x80) != 0) ||
                               ((GetAsyncKeyState(Keys.Control) & 0x80) != 0);
                // Is Shift being held down?
                bool shift = ((GetAsyncKeyState(Keys.Shift) & 0x80) != 0) ||
                             ((GetAsyncKeyState(Keys.Shift) & 0x80) != 0);
                // Is Alt being held down?
                bool alt = ((GetAsyncKeyState(Keys.Alt) & 0x80) != 0) ||
                           ((GetAsyncKeyState(Keys.Alt) & 0x80) != 0);
                // Is CapsLock on?
                bool capslock = (GetAsyncKeyState(Keys.Capital) != 0);


                //Console.WriteLine((Keys)vkCode);
                //MessageBox.Show(((Keys)vkCode).ToString());


                if ((Keys)vkCode != Keys.Tab)
                {
                    //loginForm.setString(((Keys)vkCode).ToString());
                }
                else
                {
                    //return (IntPtr)1;
                }

                if (alt) return (IntPtr)1;

                if ((Keys)vkCode == Keys.RWin ||
                    (Keys)vkCode == Keys.LWin)
                {
                    return (IntPtr)1;
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public void hook()
        {
            _hookID = SetHook(_proc);
        }

        public void unhook()
        {
            UnhookWindowsHookEx(_hookID);
        }


        static Process[] processHandle;

        public void hideAllWidnows() {
            IntPtr hWnd;
            Process[] processRunning = Process.GetProcesses();
            processHandle = processRunning;
            foreach (Process process in processRunning)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    hWnd = process.MainWindowHandle;
                    ShowWindow(hWnd.ToInt32(), SW_HIDE);
                }
            }
        }

        public void showAllWidnows()
        {
            IntPtr hWnd;
            foreach (Process process in processHandle)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    hWnd = process.MainWindowHandle;
                    ShowWindow(hWnd.ToInt32(), SW_SHOW);
                }
            }
            Array.Clear(processHandle, 0, processHandle.Length);
        }

    }
}
