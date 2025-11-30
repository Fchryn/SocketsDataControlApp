using SocketsDataControl.FileManager;
using SocketsDataControl.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SocketsDataControl.UserControllers
{
    public class UserController
    {
        private UserDataCollection dataCollection;
        private readonly object _lockObject = new object();

        public UserController()
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                lock (_lockObject)
                {
                    dataCollection = UserFileManager.LoadAllData();
                    Console.WriteLine($"User data loaded successfully: {dataCollection.Users.Count} users found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user data: {ex.Message}");
                dataCollection = new UserDataCollection();
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        public bool Register(string oprId, string password, string confirmPass)
        {
            lock (_lockObject)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(oprId))
                        throw new Exception("Opr ID tidak boleh kosong");

                    if (string.IsNullOrWhiteSpace(password))
                        throw new Exception("Password tidak boleh kosong");

                    if (password != confirmPass)
                        throw new Exception("Password tidak sama");

                    if (password.Length < 4)
                        throw new Exception("Password minimal 4 karakter");

                    LoadData();

                    if (dataCollection.OprIDExists(oprId))
                        throw new Exception($"Opr ID '{oprId}' sudah terdaftar");

                    var user = new UserModel
                    {
                        OprID = oprId.Trim(),
                        PasswordHash = HashPassword(password),
                        Role = "User",
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };

                    dataCollection.Save(user);
                    UserFileManager.SaveToFile(user);

                    Console.WriteLine($"User registered successfully: {oprId}, ID: {user.Id}");
                    return true;
                }
                catch (InvalidOperationException ex)
                {
                    throw new Exception(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Gagal registrasi: {ex.Message}");
                }
            }
        }

        public UserModel Login(string oprId, string password)
        {
            lock (_lockObject)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(oprId))
                        throw new Exception("Opr ID tidak boleh kosong");

                    if (string.IsNullOrWhiteSpace(password))
                        throw new Exception("Password tidak boleh kosong");

                    LoadData();

                    var user = dataCollection.GetByOprID(oprId.Trim());
                    if (user == null)
                        throw new Exception("Opr ID tidak ditemukan");

                    string inputHash = HashPassword(password);
                    if (user.PasswordHash != inputHash)
                        throw new Exception("Password salah");

                    var loginRecord = new UserLoginRecord
                    {
                        OprID = user.OprID,
                        Role = user.Role,
                        LoginTime = DateTime.Now
                    };

                    dataCollection.AddLoginRecord(loginRecord);
                    UserFileManager.SaveLoginRecord(loginRecord);

                    Console.WriteLine($"User logged in successfully: {oprId}");
                    return user.Clone();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Login gagal: {ex.Message}");
                }
            }
        }

        public bool RequestReset(string socketPN, string requestedBy)
        {
            try
            {
                var existingRequest = dataCollection.GetPendingResetRequest(socketPN);
                if (existingRequest != null)
                {
                    throw new Exception($"Sudah ada permintaan reset pending untuk socket {socketPN}");
                }

                var resetRequest = new ResetRequestRecord
                {
                    SocketPN = socketPN,
                    RequestedBy = requestedBy,
                    Status = "Pending",
                    RequestTime = DateTime.Now
                };

                UserFileManager.SaveResetRequestRecord(resetRequest);
                dataCollection.AddResetRequest(resetRequest);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Gagal membuat permintaan reset: {ex.Message}");
            }
        }

        public bool ApproveReset(string socketPN, string approvedBy)
        {
            try
            {
                var pendingRequest = dataCollection.GetPendingResetRequest(socketPN);

                pendingRequest.Status = "Approved";
                pendingRequest.ApprovedBy = approvedBy;

                UserFileManager.SaveResetRequestRecord(pendingRequest);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Gagal menyetujui reset: {ex.Message}");
            }
        }

        public List<UserModel> GetAdmins()
        {
            LoadData();
            return dataCollection.GetAdmins();
        }

        public List<UserModel> GetSuperAdmins()
        {
            LoadData();
            return dataCollection.GetSuperAdmins();
        }

        public List<UserModel> GetActiveUsers()
        {
            LoadData();
            return dataCollection.GetActiveUsers();
        }

        public bool IsUserAdmin(UserModel user)
        {
            return user != null && user.IsAdmin();
        }

        public bool IsUserSuperAdmin(UserModel user)
        {
            return user != null && user.IsSuperAdmin();
        }

        public List<ResetRequestRecord> GetResetRequests()
        {
            LoadData();
            return dataCollection.ResetRequests.OrderByDescending(r => r.RequestTime).ToList();
        }

        public ResetRequestRecord GetPendingResetRequest(string socketPN)
        {
            LoadData();
            return dataCollection.GetPendingResetRequest(socketPN);
        }

        public int GetNextUserId()
        {
            LoadData();
            return dataCollection.NextId;
        }
    }
}
