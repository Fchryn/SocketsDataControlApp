using System;
using System.Windows.Forms;
using SocketsDataControl.UserControllers;

namespace SocketsDataControl
{
    public partial class RegisterForm : Form
    {
        private readonly UserController userController = new UserController();
        public string RegisteredUsername { get; private set; }

        public RegisterForm()
        {
            InitializeComponent();
            txtPass.UseSystemPasswordChar = true;
            txtConfirm.UseSystemPasswordChar = true;
            checkPass.CheckedChanged += checkPass_CheckedChanged;
            checkConfirmPass.CheckedChanged += checkConfirmPass_CheckedChanged;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void btnRegister_Click_1(object sender, EventArgs e)
        {
            try
            {
                string oprId = txtOprID.Text.Trim();
                string password = txtPass.Text;
                string confirmPassword = txtConfirm.Text;

                if (string.IsNullOrEmpty(oprId) || string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("Operator ID dan Password harus diisi!", "Peringatan",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (password != confirmPassword)
                {
                    MessageBox.Show("Password dan Konfirmasi Password tidak cocok!", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtConfirm.Focus();
                    txtConfirm.SelectAll();
                    return;
                }

                bool success = userController.Register(oprId, password, confirmPassword);
                if (success)
                {
                    RegisteredUsername = oprId;
                    MessageBox.Show("Registrasi berhasil! Silakan login.", "Success",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);

                txtPass.Clear();
                txtConfirm.Clear();
                txtPass.Focus();
            }
        }

        private void lnkLogin_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void checkPass_CheckedChanged(object sender, EventArgs e)
        {
            bool show = checkPass.Checked;
            txtPass.UseSystemPasswordChar = !show;
        }

        private void checkConfirmPass_CheckedChanged(object sender, EventArgs e)
        {
            bool show = checkConfirmPass.Checked;
            txtConfirm.UseSystemPasswordChar = !show;
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            txtOprID.Focus();
        }
    }
}
