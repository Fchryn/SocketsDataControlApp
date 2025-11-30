using System;
using System.Windows.Forms;
using SocketsDataControl.UserModels;
using SocketsDataControl.UserControllers;

namespace SocketsDataControl
{
    public partial class LoginForm : Form
    {
        private readonly UserController userController = new UserController();
        public UserModel LoggedInUser { get; private set; }

        public LoginForm()
        {
            InitializeComponent();
            txtPass.UseSystemPasswordChar = true;
            checkPass.CheckedChanged += checkPass_CheckedChanged;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            try
            {
                string username = txtOprID.Text.Trim();
                string password = txtPass.Text;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Username dan Password harus diisi!", "Peringatan",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var user = userController.Login(username, password);
                LoggedInUser = user;

                string welcomeMessage = $"Login berhasil! Selamat datang {user.OprID}";
                if (userController.IsUserSuperAdmin(user))
                {
                    welcomeMessage += " (Super Admin)";
                }
                else if (userController.IsUserAdmin(user))
                {
                    welcomeMessage += " (Admin)";
                }

                MessageBox.Show(welcomeMessage, "Success",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login gagal: " + ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtPass.Clear();
                txtPass.Focus();
            }
        }

        private void lnkRegister_Click(object sender, EventArgs e)
        {
            using (var reg = new RegisterForm())
            {
                if (reg.ShowDialog() == DialogResult.OK)
                {
                    txtOprID.Text = reg.RegisteredUsername;
                    txtPass.Focus();
                }
            }
        }

        private void checkPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPass.UseSystemPasswordChar = !checkPass.Checked;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtOprID.Focus();
        }
    }
}