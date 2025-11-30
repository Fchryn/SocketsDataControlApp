using SocketsDataControl.Controllers;
using SocketsDataControl.Models;
using SocketsDataControl.UserControllers;
using SocketsDataControl.UserModels;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SocketsDataControl
{
    public partial class MainForm : Form
    {
        private readonly SocketController controller;
        private readonly UserController userController;
        private int[] cavRuns = new int[8];
        private string[] cavStatus = new string[8];
        private int currentCavUse = 8;
        private string currentSocketPN = "";
        private bool isLoadingData = false;
        private UserModel currentUser;

        private Button btnRequestReset;
        private Label lblResetStatus;

        public bool LogoutRequested { get; private set; } = false;

        public MainForm(UserModel loggedInUser)
        {
            InitializeComponent();
            currentUser = loggedInUser;
            controller = new SocketController(currentUser.OprID);
            userController = new UserController();

            InitializeCustomComponents();

            if (currentUser != null && !string.IsNullOrEmpty(currentUser.OprID))
            {
                txtOprID.Text = currentUser.OprID;
                txtOprID.ReadOnly = true;
                txtOprID.BackColor = SystemColors.Control;
            }
        }

        private void InitializeCustomComponents()
        {
            var socketList = controller.GetAllPredefinedSockets();
            combBoxSocketPN.Items.AddRange(socketList);

            var timer = new Timer { Interval = 1000 };
            timer.Tick += (s, e) => lblDateTime.Text = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "SE Asia Standard Time").ToString("dd-MM-yyyy HH:mm:ss");
            timer.Start();

            ResetToDefaults();

            btnPlus.Click += btnPlus_Click;
            combBoxSocketPN.SelectedIndexChanged += combBoxSocketPN_SelectedIndexChanged;
            btnSave.Click += BtnSave_Click;
            btnAddStatus.Click += BtnAddStatus_Click;
            txtCavUse.TextChanged += TxtCavUse_TextChanged;

            txtCavUse.Text = "8";
            txtOutput.Text = "0";
            txtTotalRun.Text = "0";

            pictureBoxSocket.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxSocket.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxSocket.BackColor = Color.White;

            UpdateCavFields(8);

            if (combBoxSocketPN.Items.Count > 0)
            {
                combBoxSocketPN.SelectedIndex = 0;
            }

            CheckResetRequests();
            DisplayUserWelcome();
        }

        private void DisplayUserWelcome()
        {
            if (currentUser != null)
            {
                string welcomeMessage = $"Selamat datang, {currentUser.OprID}!";
                if (userController.IsUserSuperAdmin(currentUser))
                {
                    welcomeMessage += " (Super Admin)";
                }
                else if (userController.IsUserAdmin(currentUser))
                {
                    welcomeMessage += " (Admin)";
                }
                else
                {
                    welcomeMessage += " (User)";
                }

                MessageBox.Show(welcomeMessage, "Welcome",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ResetToDefaults()
        {
            for (int i = 0; i < 8; i++)
            {
                cavRuns[i] = 0;
                cavStatus[i] = "Good";
            }
            currentCavUse = 8;
            txtTotalRun.Text = "0";
        }

        private void BtnAdmin_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(currentSocketPN))
                {
                    MessageBox.Show("Pilih Socket PN terlebih dahulu sebelum membuka Admin Panel!", "Peringatan",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var adminForm = new AdminForm())
                {
                    adminForm.CurrentSocketPN = currentSocketPN;

                    var result = adminForm.ShowDialog();

                    if (result == DialogResult.OK && adminForm.ResetApproved)
                    {
                        ResetLocalData();
                        LoadSocketData(currentSocketPN);

                        MessageBox.Show($"Data socket {currentSocketPN} telah direset oleh Admin!", "Info",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);

                        CheckResetRequests();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening admin panel: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRequestReset_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentSocketPN))
            {
                MessageBox.Show("Pilih Socket PN terlebih dahulu!", "Peringatan",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var latestData = controller.LoadLatestByPN(currentSocketPN);
                if (latestData != null)
                {
                    var cavitiesAtLimit = latestData.GetCavitiesAtLimit();

                    if (cavitiesAtLimit.Count == 0)
                    {
                        MessageBox.Show($"Tidak dapat request reset!\n\n" +
                                      $"Socket {currentSocketPN} belum mencapai batas maksimal.\n" +
                                      $"Reset hanya dapat dilakukan ketika cavity mencapai batas maksimal.",
                                      "Tidak Dapat Reset",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    DialogResult result = MessageBox.Show(
                        $"Anda akan mengajukan permintaan reset untuk:\n\n" +
                        $"Socket: {currentSocketPN}\n" +
                        $"Cavity yang mencapai limit: {string.Join(", ", cavitiesAtLimit)}\n\n" +
                        $"Permintaan akan dikirim ke Admin untuk persetujuan.\n" +
                        $"Apakah Anda yakin?",
                        "REQUEST RESET",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        userController.RequestReset(currentSocketPN, currentUser.OprID);

                        MessageBox.Show($"Permintaan reset untuk {currentSocketPN} telah dikirim!\n\n" +
                                      $"Silakan tunggu persetujuan dari Admin.",
                                      "Request Dikirim",
                                      MessageBoxButtons.OK, MessageBoxIcon.Information);

                        CheckResetRequests();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error requesting reset: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CheckResetRequests()
        {
            if (string.IsNullOrEmpty(currentSocketPN)) return;

            try
            {
                var pendingRequest = userController.GetPendingResetRequest(currentSocketPN);
                if (pendingRequest != null)
                {
                    lblResetStatus.Text = $"⏳ Reset Requested - Menunggu Admin";
                    lblResetStatus.ForeColor = Color.Orange;
                    btnRequestReset.Enabled = false;
                    btnRequestReset.BackColor = Color.Gray;
                }
                else
                {
                    //lblResetStatus.Text = "";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking reset requests: {ex.Message}");
            }
        }

        private void ResetLocalData()
        {
            for (int i = 0; i < 8; i++)
            {
                cavRuns[i] = 0;
                cavStatus[i] = "Good";
            }
            currentCavUse = 8;
            txtCavUse.Text = "8";
            txtOutput.Text = "0";
            txtTotalRun.Text = "0";

            UpdateCavFields(currentCavUse);
        }

        private void BtnLogout_Click_1(object sender, EventArgs e)
        {
            try
            {
                SaveCurrentData();

                DialogResult result = MessageBox.Show(
                    "Apakah Anda yakin ingin logout?",
                    "Konfirmasi Logout",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    LogoutRequested = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during logout: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCurrentData()
        {
            try
            {
                if (combBoxSocketPN.SelectedItem == null) return;

                for (int i = 0; i < currentCavUse; i++)
                {
                    var statusCombo = this.Controls.Find($"cmbStatus{i + 1}", true).FirstOrDefault() as ComboBox;
                    if (statusCombo != null)
                    {
                        if (statusCombo.SelectedItem != null)
                            cavStatus[i] = statusCombo.SelectedItem.ToString();
                        else if (!string.IsNullOrEmpty(statusCombo.Text))
                            cavStatus[i] = statusCombo.Text;
                    }
                }

                var model = new SocketModel
                {
                    OprID = currentUser.OprID,
                    CavUse = currentCavUse,
                    SocketPN = currentSocketPN,
                    OutputQty = ParseInt(txtOutput.Text),
                    TotalRun = ParseInt(txtTotalRun.Text),
                    CavRuns = cavRuns,
                    CavStatus = cavStatus,
                    ImagePath = controller.GetImagePath(currentSocketPN) ?? "",
                    CreatedAt = DateTime.Now
                };

                controller.Save(model);

                Console.WriteLine("Data tersimpan otomatis sebelum logout");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data before logout: {ex.Message}");
            }
        }

        private void CheckCavityLimits()
        {
            if (string.IsNullOrEmpty(currentSocketPN)) return;

            var model = new SocketModel
            {
                SocketPN = currentSocketPN,
                CavRuns = cavRuns
            };

            var (nearLimit, atLimit) = controller.CheckCavityLimits(model);

            if (nearLimit.Count > 0)
            {
                string cavityList = string.Join(", ", nearLimit);
                MessageBox.Show($"⚠ PERINGATAN: Cavity {cavityList} mendekati batas maksimal!\n" +
                              $"Batas maksimal untuk {currentSocketPN}: {SocketModel.GetCavityMaxLimitForSocket(currentSocketPN):N0}",
                              "Cavity Mendekati Batas",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
            }

            if (atLimit.Count > 0)
            {
                string cavityList = string.Join(", ", atLimit);
                MessageBox.Show($"🚨 PERINGATAN KRITIS: Cavity {cavityList} telah mencapai batas maksimal {SocketModel.GetCavityMaxLimitForSocket(currentSocketPN):N0}!\n\n" +
                              "Tolong lakukan pengecekan kondisi socket dan jika diperlukan lakukan pergantian.",
                              "Cavity Limit Tercapai",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void TxtCavUse_TextChanged(object sender, EventArgs e)
        {
            if (isLoadingData) return;

            int newCavUse = ParseCavUse();
            if (newCavUse == -1)
            {
                txtCavUse.Text = currentCavUse.ToString();
                return;
            }

            if (newCavUse != currentCavUse)
            {
                currentCavUse = newCavUse;
                UpdateCavFields(currentCavUse);
                txtOutput.Text = "0";
            }
        }

        private void btnPlus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentSocketPN))
            {
                MessageBox.Show("Pilih Socket PN terlebih dahulu.");
                return;
            }

            int outputQty = ParseInt(txtOutput.Text);

            if (outputQty <= 0)
            {
                MessageBox.Show("Output quantity harus lebih dari 0");
                return;
            }

            try
            {
                var model = new SocketModel
                {
                    SocketPN = currentSocketPN,
                    CavRuns = cavRuns
                };

                var cavitiesAtLimit = model.GetCavitiesAtLimit();
                if (cavitiesAtLimit.Count > 0)
                {
                    MessageBox.Show($"Tidak dapat menambah output!\n\n" +
                                  $"Cavity {string.Join(", ", cavitiesAtLimit)} telah mencapai batas maksimal.\n" +
                                  $"Tolong lakukan pengecekan kondisi socket dan jika diperlukan lakukan pergantian.",
                                  "Tidak Dapat Menambah Output",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    return;
                }

                int currentTotalRun = ParseInt(txtTotalRun.Text);
                int newTotalRun = currentTotalRun + outputQty;

                cavRuns = controller.DistributeToCav(currentCavUse, outputQty, cavRuns);

                newTotalRun = controller.CalculateTotalRunFromCavRuns(cavRuns);
                txtTotalRun.Text = newTotalRun.ToString();

                if (newTotalRun >= 240000)
                {
                    MessageBox.Show("⚠ Has been qty 240000 - counter restarted");
                    int overflow = newTotalRun - 240000;
                    txtTotalRun.Text = overflow.ToString();
                }

                UpdateCavFields(currentCavUse);
                CheckCavityLimits();
                txtOutput.Text = "0";

                Console.WriteLine($"Added {outputQty} to Total Run. New Total: {newTotalRun}");
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error processing output: {ex.Message}");
            }
        }

        private void UpdateCavFields(int cavUse)
        {
            for (int i = 0; i < 8; i++)
            {
                var runBox = this.Controls.Find($"txtCavRun{i + 1}", true).FirstOrDefault() as TextBox;
                var statusCombo = this.Controls.Find($"cmbStatus{i + 1}", true).FirstOrDefault() as ComboBox;

                if (i < cavUse)
                {
                    if (runBox != null)
                    {
                        runBox.Text = cavRuns[i].ToString();
                        runBox.Enabled = true;
                        runBox.BackColor = System.Drawing.SystemColors.Window;
                    }

                    if (statusCombo != null)
                    {
                        statusCombo.Enabled = true;
                        statusCombo.Items.Clear();

                        if (!string.IsNullOrEmpty(cavStatus[i]) && cavStatus[i].Equals("Good", StringComparison.OrdinalIgnoreCase))
                        {
                            statusCombo.Items.Add("Fail");
                            statusCombo.Text = "Good";
                        }
                        else if (!string.IsNullOrEmpty(cavStatus[i]) && cavStatus[i].Equals("Fail", StringComparison.OrdinalIgnoreCase))
                        {
                            statusCombo.Items.Clear();
                            statusCombo.Text = "Fail";
                        }
                        else
                        {
                            statusCombo.Items.AddRange(new object[] { "Good", "Fail" });

                            if (string.IsNullOrEmpty(cavStatus[i]))
                            {
                                cavStatus[i] = "Good";
                                statusCombo.Text = "Good";
                            }
                            else
                            {
                                statusCombo.Text = cavStatus[i];
                            }
                        }
                    }
                }
                else
                {
                    if (runBox != null)
                    {
                        runBox.Text = cavRuns[i].ToString();
                        runBox.Enabled = false;
                        runBox.BackColor = System.Drawing.SystemColors.Control;
                    }

                    if (statusCombo != null)
                    {
                        statusCombo.Enabled = false;
                        statusCombo.Items.Clear();
                        statusCombo.Text = cavRuns[i] > 0 ? "Good" : "";
                        statusCombo.BackColor = System.Drawing.SystemColors.Control;
                    }
                }
            }
        }

        private void BtnAddStatus_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < currentCavUse; i++)
            {
                cavStatus[i] = "Good";
            }
            UpdateCavFields(currentCavUse);
        }

        private void combBoxSocketPN_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combBoxSocketPN.SelectedItem == null) return;

            string pn = combBoxSocketPN.SelectedItem.ToString();
            currentSocketPN = pn;

            LoadSocketImage(pn);
            LoadSocketData(pn);
            UpdateSocketLimitInfo(pn);
            CheckResetRequests();
        }

        private void UpdateSocketLimitInfo(string socketPN)
        {
            string limitInfo = controller.GetSocketLimitInfo(socketPN);
            MessageBox.Show($"{socketPN}\n{limitInfo}", "Info Batas Socket",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LoadSocketImage(string pn)
        {
            try
            {
                if (pictureBoxSocket.Image != null)
                {
                    pictureBoxSocket.Image.Dispose();
                    pictureBoxSocket.Image = null;
                }

                string imagePath = controller.GetImagePath(pn);

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    try
                    {
                        using (var tempImage = Image.FromFile(imagePath))
                        {
                            pictureBoxSocket.Image = new Bitmap(tempImage);
                        }
                        Console.WriteLine($"Image loaded successfully: {imagePath}");
                    }
                    catch (Exception imgEx)
                    {
                        Console.WriteLine($"Error loading image {imagePath}: {imgEx.Message}");
                        pictureBoxSocket.Image = CreatePlaceholderImage("Gambar\nCorrupt");
                    }
                }
                else
                {
                    Console.WriteLine($"No image found for: {pn}");
                    pictureBoxSocket.Image = CreatePlaceholderImage("Gambar\nTidak Ditemukan");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadSocketImage: {ex.Message}");
                pictureBoxSocket.Image = CreatePlaceholderImage("Error\nLoad Gambar");
            }
        }

        private Bitmap CreatePlaceholderImage(string text)
        {
            var bmp = new Bitmap(200, 150);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                using (var font = new Font("Arial", 10, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.DarkRed))
                using (var format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    g.DrawString(text, font, brush, new RectangleF(0, 0, bmp.Width, bmp.Height), format);
                }
                g.DrawRectangle(Pens.DarkGray, 0, 0, bmp.Width - 1, bmp.Height - 1);
            }
            return bmp;
        }

        private void LoadSocketData(string pn)
        {
            isLoadingData = true;

            try
            {
                var latest = controller.LoadLatestByPN(pn);
                if (latest != null)
                {
                    LoadFromSocketModel(latest);
                    Console.WriteLine($"Loaded data from file for {pn}: Sequence={latest.SequenceNumber}, CavUse={latest.CavUse}, TotalRun={latest.TotalRun}, OprID={latest.OprID}");
                }
                else
                {
                    ResetToDefaults();
                    Console.WriteLine($"No existing data found for {pn}, resetting to defaults");
                }

                txtOutput.Text = "0";
                UpdateCavFields(currentCavUse);
                CheckCavityLimits();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
            finally
            {
                isLoadingData = false;
            }
        }

        private void LoadFromSocketModel(SocketModel model)
        {
            currentCavUse = model.CavUse > 0 ? model.CavUse : 8;
            txtCavUse.Text = currentCavUse.ToString();
            txtTotalRun.Text = model.TotalRun.ToString();

            if (model.CavRuns != null && model.CavRuns.Length == 8)
            {
                Array.Copy(model.CavRuns, cavRuns, 8);
            }

            if (model.CavStatus != null && model.CavStatus.Length == 8)
            {
                for (int i = 0; i < 8; i++)
                {
                    cavStatus[i] = string.IsNullOrEmpty(model.CavStatus[i]) ? "Good" : model.CavStatus[i];
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (combBoxSocketPN.SelectedItem == null)
            {
                MessageBox.Show("Pilih Socket PN terlebih dahulu.");
                return;
            }

            try
            {
                for (int i = 0; i < currentCavUse; i++)
                {
                    var statusCombo = this.Controls.Find($"cmbStatus{i + 1}", true).FirstOrDefault() as ComboBox;
                    if (statusCombo != null)
                    {
                        if (statusCombo.SelectedItem != null)
                            cavStatus[i] = statusCombo.SelectedItem.ToString();
                        else if (!string.IsNullOrEmpty(statusCombo.Text))
                            cavStatus[i] = statusCombo.Text;
                    }
                }

                for (int i = currentCavUse; i < 8; i++)
                {
                    cavStatus[i] = "Good";
                }

                int outputQty = ParseInt(txtOutput.Text);
                int totalRun = ParseInt(txtTotalRun.Text);

                var model = new SocketModel
                {
                    OprID = currentUser.OprID,
                    CavUse = currentCavUse,
                    SocketPN = currentSocketPN,
                    OutputQty = outputQty,
                    TotalRun = totalRun,
                    CavRuns = cavRuns,
                    CavStatus = cavStatus,
                    ImagePath = controller.GetImagePath(currentSocketPN) ?? "",
                    CreatedAt = DateTime.Now
                };

                var cavitiesAtLimit = model.GetCavitiesAtLimit();
                var cavitiesNearLimit = model.GetCavitiesNearLimit();

                if (cavitiesAtLimit.Count > 0 || cavitiesNearLimit.Count > 0)
                {
                    string message = "PERINGATAN:\n";

                    if (cavitiesAtLimit.Count > 0)
                    {
                        message += $"🚨 Cavity {string.Join(", ", cavitiesAtLimit)} telah mencapai batas maksimal!\n";
                    }

                    if (cavitiesNearLimit.Count > 0)
                    {
                        message += $"⚠ Cavity {string.Join(", ", cavitiesNearLimit)} mendekati batas maksimal!\n";
                    }

                    DialogResult result = MessageBox.Show(message,
                        "Konfirmasi Save Data",
                        MessageBoxButtons.YesNo,
                        cavitiesAtLimit.Count > 0 ? MessageBoxIcon.Error : MessageBoxIcon.Warning);

                    if (result != DialogResult.Yes)
                    {
                        return;
                    }
                }

                controller.Save(model);

                int totalAllUsers = controller.GetTotalQuantityByPN(currentSocketPN);
                string successMessage = $"✅ Data berhasil disimpan!\n\n" +
                                      $"📅 Saved at: {model.CreatedAt:dd-MM-yyyy HH:mm:ss}\n" +
                                      $"👤 Operator: {model.OprID}\n" +
                                      $"🔌 Socket: {model.SocketPN}\n" +
                                      $"📊 Total Run: {model.TotalRun:N0}\n" +
                                      $"📈 Output Qty: {model.OutputQty:N0}\n\n" +
                                      $"📋 Total Quantity untuk {currentSocketPN} (Semua User): {totalAllUsers:N0}";

                if (cavitiesAtLimit.Count > 0 || cavitiesNearLimit.Count > 0)
                {
                    successMessage += $"\n\n⚠ PERINGATAN: ";
                    if (cavitiesAtLimit.Count > 0)
                    {
                        successMessage += $"Cavity {string.Join(", ", cavitiesAtLimit)} telah mencapai batas maksimal! ";
                    }
                    if (cavitiesNearLimit.Count > 0)
                    {
                        successMessage += $"Cavity {string.Join(", ", cavitiesNearLimit)} mendekati batas maksimal!";
                    }
                }

                MessageBox.Show(successMessage, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                txtOutput.Text = "0";

                Console.WriteLine($"Data saved. Running total: {totalRun}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Gagal menyimpan data: " + ex.Message, "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int ParseCavUse()
        {
            if (string.IsNullOrEmpty(txtCavUse.Text))
            {
                return 8;
            }

            if (!int.TryParse(txtCavUse.Text, out int cavUse))
            {
                MessageBox.Show("Cavity Use harus angka antara 1 sampai 8.", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            if (cavUse < 1 || cavUse > 8)
            {
                MessageBox.Show("Nilai Cavity Use harus antara 1 sampai 8.", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
            return cavUse;
        }

        private int ParseInt(string s)
        {
            int v = 0;
            int.TryParse(s, out v);
            return v;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Welcome message already displayed in InitializeCustomComponents
        }
    }
}