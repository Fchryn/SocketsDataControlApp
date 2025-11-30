using System;
using System.Collections.Generic;
using System.Linq;

namespace SocketsDataControl.UserModels
{
    public class UserModel
    {
        public int Id { get; set; }
        public string OprID { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public bool IsAdmin()
        {
            return Role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                   Id == 1 || Id == 2;
        }

        public bool IsSuperAdmin()
        {
            return Id == 1 || Id == 2;
        }

        public string ToFileString()
        {
            return $"User ID : {Id}; " +
                   $"Operator ID : {OprID}; " +
                   $"Password Hash : {PasswordHash}; " +
                   $"Role : {Role}; " +
                   $"Is Active : {IsActive}; " +
                   $"Register At : {CreatedAt:dd-MM-yyyy HH:mm:ss}";
        }

        public static UserModel FromFileString(string fileString)
        {
            var model = new UserModel();

            try
            {
                var parts = fileString.Split(';');
                foreach (var part in parts)
                {
                    if (part.Contains(":"))
                    {
                        var keyValue = part.Split(':');
                        if (keyValue.Length == 2)
                        {
                            string key = keyValue[0].Trim();
                            string value = keyValue[1].Trim();

                            switch (key)
                            {
                                case "User ID":
                                    model.Id = int.TryParse(value, out int id) ? id : 0;
                                    break;
                                case "Operator ID":
                                    model.OprID = value;
                                    break;
                                case "Password Hash":
                                    model.PasswordHash = value;
                                    break;
                                case "Role":
                                    model.Role = value;
                                    break;
                                case "Is Active":
                                    model.IsActive = bool.TryParse(value, out bool isActive) ? isActive : true;
                                    break;
                                case "Register At":
                                    if (DateTime.TryParse(value, out DateTime registerAt))
                                        model.CreatedAt = registerAt;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing user file string: {ex.Message}");
            }

            return model;
        }

        public UserModel Clone()
        {
            return new UserModel
            {
                Id = this.Id,
                OprID = this.OprID,
                PasswordHash = this.PasswordHash,
                Role = this.Role,
                IsActive = this.IsActive,
                CreatedAt = this.CreatedAt
            };
        }
    }

    public class UserLoginRecord
    {
        public DateTime LoginTime { get; set; }
        public string OprID { get; set; } = "";
        public string Role { get; set; } = "";

        public string ToFileString()
        {
            return $"Login Time : {LoginTime:dd-MM-yyyy HH:mm:ss}; " +
                   $"Operator ID : {OprID}; " +
                   $"Role : {Role}";
        }

        public static UserLoginRecord FromFileString(string fileString)
        {
            var record = new UserLoginRecord();

            try
            {
                var parts = fileString.Split(';');
                foreach (var part in parts)
                {
                    if (part.Contains(":"))
                    {
                        var keyValue = part.Split(':');
                        if (keyValue.Length == 2)
                        {
                            string key = keyValue[0].Trim();
                            string value = keyValue[1].Trim();

                            switch (key)
                            {
                                case "Login Time":
                                    if (DateTime.TryParse(value, out DateTime loginTime))
                                        record.LoginTime = loginTime;
                                    break;
                                case "Operator ID":
                                    record.OprID = value;
                                    break;
                                case "Role":
                                    record.Role = value;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing login record: {ex.Message}");
            }

            return record;
        }
    }

    public class ResetRequestRecord
    {
        public DateTime RequestTime { get; set; }
        public string SocketPN { get; set; } = "";
        public string RequestedBy { get; set; } = "";
        public string ApprovedBy { get; set; } = "";
        public string Status { get; set; } = "Pending";

        public string ToFileString()
        {
            return $"Reset Request Time : {RequestTime:dd-MM-yyyy HH:mm:ss}; " +
                   $"Socket PN : {SocketPN}; " +
                   $"Requested By : {RequestedBy}; " +
                   $"Approved By : {ApprovedBy}; " +
                   $"Status : {Status}";
        }

        public static ResetRequestRecord FromFileString(string fileString)
        {
            var record = new ResetRequestRecord();

            try
            {
                var parts = fileString.Split(';');
                foreach (var part in parts)
                {
                    if (part.Contains(":"))
                    {
                        var keyValue = part.Split(':');
                        if (keyValue.Length == 2)
                        {
                            string key = keyValue[0].Trim();
                            string value = keyValue[1].Trim();

                            switch (key)
                            {
                                case "Reset Request Time":
                                    if (DateTime.TryParse(value, out DateTime requestTime))
                                        record.RequestTime = requestTime;
                                    break;
                                case "Socket PN":
                                    record.SocketPN = value;
                                    break;
                                case "Requested By":
                                    record.RequestedBy = value;
                                    break;
                                case "Approved By":
                                    record.ApprovedBy = value;
                                    break;
                                case "Status":
                                    record.Status = value;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing reset request record: {ex.Message}");
            }

            return record;
        }
    }

    public class UserDataCollection
    {
        public List<UserModel> Users { get; set; } = new List<UserModel>();
        public List<UserLoginRecord> LoginRecords { get; set; } = new List<UserLoginRecord>();
        public List<ResetRequestRecord> ResetRequests { get; set; } = new List<ResetRequestRecord>();
        public int NextId { get; set; } = 1;

        public void Save(UserModel user)
        {
            var existing = Users.FirstOrDefault(u => u.Id == user.Id);
            if (existing != null)
            {
                existing.OprID = user.OprID;
                existing.PasswordHash = user.PasswordHash;
                existing.Role = user.Role;
                existing.IsActive = user.IsActive;
                existing.CreatedAt = user.CreatedAt;
            }
            else
            {
                var existingOprID = Users.FirstOrDefault(u => u.OprID.Equals(user.OprID, StringComparison.OrdinalIgnoreCase));
                if (existingOprID != null)
                {
                    throw new InvalidOperationException($"Operator ID '{user.OprID}' sudah terdaftar");
                }

                if (user.Id == 0)
                {
                    user.Id = NextId++;
                }
                else
                {
                    if (user.Id >= NextId)
                    {
                        NextId = user.Id + 1;
                    }
                }

                user.CreatedAt = DateTime.Now;
                Users.Add(user);
            }
        }

        public void AddLoginRecord(UserLoginRecord loginRecord)
        {
            loginRecord.LoginTime = DateTime.Now;
            LoginRecords.Add(loginRecord);
        }

        public void AddResetRequest(ResetRequestRecord resetRequest)
        {
            resetRequest.RequestTime = DateTime.Now;
            ResetRequests.Add(resetRequest);
        }

        public UserModel GetByOprID(string oprId)
        {
            return Users.FirstOrDefault(u => u.OprID.Equals(oprId, StringComparison.OrdinalIgnoreCase) && u.IsActive);
        }

        public UserModel GetById(int id)
        {
            return Users.FirstOrDefault(u => u.Id == id && u.IsActive);
        }

        public bool OprIDExists(string oprId)
        {
            return Users.Any(u => u.OprID.Equals(oprId, StringComparison.OrdinalIgnoreCase) && u.IsActive);
        }

        public List<UserModel> GetAdmins()
        {
            return Users.Where(u => u.IsAdmin() && u.IsActive).OrderBy(u => u.Id).ToList();
        }

        public List<UserModel> GetSuperAdmins()
        {
            return Users.Where(u => u.IsSuperAdmin() && u.IsActive).OrderBy(u => u.Id).ToList();
        }

        public ResetRequestRecord GetPendingResetRequest(string socketPN)
        {
            return ResetRequests
                .Where(r => r.SocketPN == socketPN && r.Status == "Pending")
                .OrderByDescending(r => r.RequestTime)
                .FirstOrDefault();
        }

        public List<UserModel> GetActiveUsers()
        {
            return Users.Where(u => u.IsActive).OrderBy(u => u.Id).ToList();
        }
    }
}