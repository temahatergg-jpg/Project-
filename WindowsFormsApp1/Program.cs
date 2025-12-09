using System;
using System.Windows.Forms;
using WindowsFormsApp1;

namespace WindowsFormsApp1
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (var loginForm = new LoginForm())
            {
                var dialogResult = loginForm.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    bool isAdmin = loginForm.IsAdmin;
                    Application.Run(new MainForm(isAdmin));
                }
            }
        }
    }
}
