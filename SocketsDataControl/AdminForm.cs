using SocketsDataControl.Controllers;
using SocketsDataControl.Models;
using SocketsDataControl.UserControllers;
using SocketsDataControl.UserModels;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SocketsDataControl
{
    public partial class AdminForm : Form
    {
        private readonly UserController userController = new UserController();
        private readonly SocketController socketController;
        private UserModel currentAdmin;
        private bool isSuperAdminLoggedIn = false;

        public string CurrentSocketPN { get; set; } = "";
        public bool ResetApproved { get; private set; } = false;

        public AdminForm()
        {
            InitializeComponent();
            socketController = new SocketController();
            InitializeAdminForm();
        }

        private void InitializeAdminForm()
        {
            txtPass.UseSystemPasswordChar = true;

            btnResetData.Enabled = false;
            btnResetData.BackColor = Color.LightGray;

            LoadAdminInfo();
            LoadPendingRequests();
        }

        private void LoadAdminInfo()
        {
            try
            {
                var admins = userController.GetAdmins();
                var superAdmins = userController.GetSuperAdmins();

                string info = $"Admin Terdaftar: {admins.Count}/2\n";
                info += $"Super Admin: {superAdmins.Count}/2\n\n";

                foreach (var admin in admins)
                {
                    info += $"- {admin.OprID} (ID: {admin.Id}) {(admin.IsSuperAdmin() ? "- SUPER ADMIN" : "- ADMIN")}\n";
                }

                info += $"\nInfo: Hanya Super Admin (ID 1 & 2) yang bisa melakukan reset data";
                MessageBox.Show(info, "Info Admin", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading admin info");
                Console.WriteLine($"Error loading admin info: {ex.Message}");
            }
        }

        private void LoadPendingRequests()
        {
            try
            {
                var pendingRequests = userController.GetResetRequests()
                    .Where(r => r.Status == "Pending")
                    .OrderByDescending(r => r.RequestTime)
                    .ToList();

                if (pendingRequests.Count > 0)
                {
                    foreach (var request in pendingRequests)
                    {
                        var item = new ListViewItem(request.SocketPN);
                        item.SubItems.Add(request.RequestedBy);
                        item.SubItems.Add(request.RequestTime.ToString("dd-MM-yyyy HH:mm:ss"));
                        item.Tag = request;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading pending requests: {ex.Message}");
            }
        }

        private void btnLoginAdmin_Click_1(object sender, EventArgs e)
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
                if (user != null && userController.IsUserAdmin(user))
                {
                    currentAdmin = user;
                    isSuperAdminLoggedIn = userController.IsUserSuperAdmin(user);

                    string welcomeMessage = $"Login berhasil! Selamat datang {(isSuperAdminLoggedIn ? "Super Admin" : "Admin")} {user.OprID}";

                    MessageBox.Show(welcomeMessage, "Success",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UpdateUIAfterLogin(user);
                }
                else
                {
                    MessageBox.Show("Akses ditolak! Hanya admin yang dapat mengakses fitur ini.\n\n" +
                                  "Admin yang valid: User dengan role Admin atau Super Admin", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);

                    txtPass.Clear();
                    txtPass.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login gagal: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateUIAfterLogin(UserModel user)
        {
            txtOprID.Enabled = false;
            txtPass.Enabled = false;
            btnLoginAdmin.Enabled = false;
            checkPass.Enabled = false;

            if (isSuperAdminLoggedIn)
            {
                btnResetData.Enabled = true;
                btnResetData.BackColor = Color.FromArgb(255, 128, 128);
            }

            if (!string.IsNullOrEmpty(CurrentSocketPN))
            {
                MessageBox.Show($"Socket Terpilih: {CurrentSocketPN}");
            }
            else
            {
                MessageBox.Show("Tidak ada socket yang dipilih di Main Form");
            }

            MessageBox.Show($"Login sebagai: {(isSuperAdminLoggedIn ? "Super Admin" : "Admin")} {user.OprID} (ID: {user.Id})");

            LoadPendingRequests();
        }

        private void CheckSocketStatus()
        {
            if (string.IsNullOrEmpty(CurrentSocketPN)) return;

            try
            {
                var latestData = socketController.LoadLatestByPN(CurrentSocketPN);
                if (latestData != null)
                {
                    var cavitiesAtLimit = latestData.GetCavitiesAtLimit();
                    var cavitiesNearLimit = latestData.GetCavitiesNearLimit();

                    string status = $"Status {CurrentSocketPN}:\n";

                    if (cavitiesAtLimit.Count > 0)
                    {
                        status += $"🚨 Cavity {string.Join(", ", cavitiesAtLimit)} mencapai batas maksimal!\n";
                    }

                    if (cavitiesNearLimit.Count > 0)
                    {
                        status += $"⚠ Cavity {string.Join(", ", cavitiesNearLimit)} mendekati batas maksimal!\n";
                    }

                    if (cavitiesAtLimit.Count == 0 && cavitiesNearLimit.Count == 0)
                    {
                        status += "✅ Semua cavity dalam kondisi normal";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking socket status: {ex.Message}");
            }
        }

        private void btnResetData_Click_1(object sender, EventArgs e)
        {
            if (!isSuperAdminLoggedIn)
            {
                MessageBox.Show("Hanya Super Admin (ID 1 & 2) yang dapat melakukan reset data!", "Akses Ditolak",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(CurrentSocketPN))
            {
                MessageBox.Show("Tidak ada socket yang dipilih untuk direset!\n\n" +
                              "Silakan pilih socket di Main Form terlebih dahulu.", "Peringatan",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DialogResult result = MessageBox.Show(
                    $"⚠️ PERINGATAN KRITIS! \n\n" +
                    $"Anda akan mereset SEMUA data socket {CurrentSocketPN} ke nilai default.\n\n" +
                    $"Data yang akan direset:\n" +
                    $"- Semua Cavity Run menjadi 0\n" +
                    $"- Semua Status Cavity menjadi 'Good'\n" +
                    $"- Total Run menjadi 0\n" +
                    $"- Output Qty menjadi 0\n\n" +
                    $"‼️ Tindakan ini TIDAK DAPAT DIBATALKAN! ‼️\n\n" +
                    $"Apakah Anda yakin ingin melanjutkan?",
                    "KONFIRMASI RESET DATA",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    DialogResult confirmResult = MessageBox.Show(
                        $"Konfirmasi akhir RESET DATA:\n\n" +
                        $"Socket: {CurrentSocketPN}\n" +
                        $"Admin: {currentAdmin.OprID}\n\n" +
                        $"Tekan YES untuk melanjutkan reset atau NO untuk membatalkan.",
                        "KONFIRMASI AKHIR",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Exclamation);

                    if (confirmResult == DialogResult.Yes)
                    {
                        ResetSocketData(CurrentSocketPN);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saat reset data: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnApproveReset_Click(object sender, EventArgs e)
        {
            if (currentAdmin == null)
            {
                MessageBox.Show("Silakan login terlebih dahulu!", "Peringatan",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        private void ResetSocketData(string socketPN)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnResetData.Enabled = false;

                var resetSocket = new SocketModel
                {
                    OprID = $"SYSTEM_RESET_BY_{currentAdmin.OprID}",
                    CavUse = 8,
                    SocketPN = socketPN,
                    OutputQty = 0,
                    TotalRun = 0,
                    CavRuns = new int[8],
                    CavStatus = new string[8] { "Good", "Good", "Good", "Good", "Good", "Good", "Good", "Good" },
                    CreatedAt = DateTime.Now
                };

                socketController.Save(resetSocket);

                MessageBox.Show($"✅ Reset data berhasil untuk socket:\n{socketPN}\n\n" +
                              $"Semua data telah direset ke nilai default.\n" +
                              $"Reset oleh: {currentAdmin.OprID}",
                              "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LogResetActivity(socketPN);

                ResetApproved = true;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error saat reset data: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.DialogResult = DialogResult.Cancel;
            }
            finally
            {
                Cursor = Cursors.Default;
                btnResetData.Enabled = isSuperAdminLoggedIn;
            }
        }

        private void LogResetActivity(string socketPN)
        {
            try
            {
                string logMessage = $"RESET ACTIVITY - Socket: {socketPN}, Admin: {currentAdmin.OprID}, Time: {DateTime.Now:dd-MM-yyyy HH:mm:ss}";
                Console.WriteLine(logMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging reset activity: {ex.Message}");
            }
        }

        private void checkPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPass.UseSystemPasswordChar = !checkPass.Checked;
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            txtOprID.Focus();
        }

        private void btnRefreshRequests_Click(object sender, EventArgs e)
        {
            LoadPendingRequests();
        }
    }
}