using System;
using System.Windows.Forms;

namespace SocketsDataControl
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                SocketsDataControl.FileManager.UserFileManager.Initialize();
                SocketsDataControl.FileManager.SocketFileManager.Initialize();

                Console.WriteLine("Application initialized successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing application: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            while (true)
            {
                using (var loginForm = new LoginForm())
                {
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        using (var mainForm = new MainForm(loginForm.LoggedInUser))
                        {
                            mainForm.ShowDialog();

                            if (mainForm.LogoutRequested)
                                continue;
                            else
                                break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }
}