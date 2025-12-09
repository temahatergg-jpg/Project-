using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public class LoginForm : Form
    {
        private readonly TextBox _txtLogin;
        private readonly TextBox _txtPassword;
        private readonly Button _btnAdmin;
        private readonly Button _btnGuest;
        private readonly Label _lblInfo;

        public bool IsAdmin { get; private set; }

        public LoginForm()
        {
            Text = "Вход в систему задач";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(360, 200);

            var lblLogin = new Label
            {
                Text = "Логин администратора:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            _txtLogin = new TextBox
            {
                Location = new Point(20, 40),
                Width = 300
            };

            var lblPassword = new Label
            {
                Text = "Пароль:",
                Location = new Point(20, 70),
                AutoSize = true
            };

            _txtPassword = new TextBox
            {
                Location = new Point(20, 90),
                Width = 300,
                UseSystemPasswordChar = true
            };

            _btnAdmin = new Button
            {
                Text = "Войти как администратор",
                Location = new Point(20, 130),
                Width = 180
            };
            _btnAdmin.Click += BtnAdminOnClick;

            _btnGuest = new Button
            {
                Text = "Войти как гость",
                Location = new Point(210, 130),
                Width = 110
            };
            _btnGuest.Click += BtnGuestOnClick;

            _lblInfo = new Label
            {
                Text = string.Empty,
                ForeColor = Color.Red,
                Location = new Point(20, 165),
                AutoSize = true
            };

            Controls.Add(lblLogin);
            Controls.Add(_txtLogin);
            Controls.Add(lblPassword);
            Controls.Add(_txtPassword);
            Controls.Add(_btnAdmin);
            Controls.Add(_btnGuest);
            Controls.Add(_lblInfo);
        }

        private void BtnAdminOnClick(object sender, EventArgs e)
        {
            string login = _txtLogin.Text.Trim();
            string password = _txtPassword.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                _lblInfo.Text = "Введите логин и пароль администратора.";
                return;
            }

            bool isAdmin = Db.CheckAdmin(login, password);
            if (!isAdmin)
            {
                _lblInfo.Text = "Неверный логин или пароль, либо пользователь не администратор.";
                return;
            }

            IsAdmin = true;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnGuestOnClick(object sender, EventArgs e)
        {
            IsAdmin = false;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
