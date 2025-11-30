using SocketsDataControl.UserModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SocketsDataControl.FileManager
{
    public static class UserFileManager
    {
        private static readonly string BaseDirectory = @"C:\SocketsDataControl";
        private static bool _isInitialized = false;
        private static readonly object _lockObject = new object();

        private static string GetCurrentWeekFolder()
        {
            DateTime today = DateTime.Today;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
            string weekFolderName = $"UsersDataControl({startOfWeek:dd-MM-yyyy})";
            return Path.Combine(BaseDirectory, weekFolderName);
        }

        private static string GetCurrentDataFilePath()
        {
            return Path.Combine(GetCurrentWeekFolder(), "UsersDataControl.txt");
        }

        public static void Initialize()
        {
            lock (_lockObject)
            {
                if (_isInitialized) return;

                try
                {
                    if (!Directory.Exists(BaseDirectory))
                    {
                        Directory.CreateDirectory(BaseDirectory);
                        Console.WriteLine($"Directory created: {BaseDirectory}");
                    }

                    string currentDataFilePath = GetCurrentDataFilePath();
                    string currentWeekFolder = Path.GetDirectoryName(currentDataFilePath);

                    if (!Directory.Exists(currentWeekFolder))
                    {
                        Directory.CreateDirectory(currentWeekFolder);
                        Console.WriteLine($"Week folder created: {currentWeekFolder}");
                    }

                    if (!File.Exists(currentDataFilePath))
                    {
                        File.WriteAllText(currentDataFilePath, "USERS DATA CONTROL - REGISTRATION & LOGIN RECORDS\n");
                        File.AppendAllText(currentDataFilePath, "==================================================\n\n");

                        CreateDefaultAdmins();

                        Console.WriteLine($"User data file created: {currentDataFilePath}");
                    }
                    else
                    {
                        EnsureDefaultAdmins();
                    }

                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing user file system: {ex.Message}");
                    throw;
                }
            }
        }

        private static string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private static void CreateDefaultAdmins()
        {
            try
            {
                var admin1 = new UserModel
                {
                    Id = 1,
                    OprID = "705299",
                    PasswordHash = HashPassword("OSIMulyono"),
                    Role = "Admin",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };
                SaveToFileDirect(admin1);

                var admin2 = new UserModel
                {
                    Id = 2,
                    OprID = "705347",
                    PasswordHash = HashPassword("OSIFachryan"),
                    Role = "Admin",
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };
                SaveToFileDirect(admin2);

                Console.WriteLine("Default admin users created");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating default admins: {ex.Message}");
            }
        }

        private static void EnsureDefaultAdmins()
        {
            try
            {
                var allData = LoadAllData();

                var admin1 = allData.GetById(1);
                if (admin1 == null)
                {
                    admin1 = new UserModel
                    {
                        Id = 1,
                        OprID = "705299",
                        PasswordHash = HashPassword("OSIMulyono"),
                        Role = "Admin",
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };
                    allData.Save(admin1);
                    SaveToFileDirect(admin1);
                    Console.WriteLine("Admin1 created");
                }

                var admin2 = allData.GetById(2);
                if (admin2 == null)
                {
                    admin2 = new UserModel
                    {
                        Id = 2,
                        OprID = "705347",
                        PasswordHash = HashPassword("OSIFachryan"),
                        Role = "Admin",
                        CreatedAt = DateTime.Now,
                        IsActive = true
                    };
                    allData.Save(admin2);
                    SaveToFileDirect(admin2);
                    Console.WriteLine("Admin2 created");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ensuring default admins: {ex.Message}");
            }
        }

        private static void SaveToFileDirect(UserModel user)
        {
            string currentDataFilePath = GetCurrentDataFilePath();
            string record = user.ToFileString();
            File.AppendAllText(currentDataFilePath, $"{record}\n");
            File.AppendAllText(currentDataFilePath, "--------------------------------------------------\n");
        }

        public static void SaveToFile(UserModel user)
        {
            try
            {
                Initialize();

                var existingUser = GetUserByOprID(user.OprID);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    throw new InvalidOperationException($"Operator ID '{user.OprID}' sudah terdaftar");
                }

                string currentDataFilePath = GetCurrentDataFilePath();
                string record = user.ToFileString();

                File.AppendAllText(currentDataFilePath, $"{record}\n");
                File.AppendAllText(currentDataFilePath, "--------------------------------------------------\n");

                Console.WriteLine($"User data saved to: {currentDataFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user to file: {ex.Message}");
                throw;
            }
        }

        public static void SaveLoginRecord(UserLoginRecord loginRecord)
        {
            try
            {
                Initialize();

                string currentDataFilePath = GetCurrentDataFilePath();
                string record = loginRecord.ToFileString();

                File.AppendAllText(currentDataFilePath, $"{record}\n");
                File.AppendAllText(currentDataFilePath, "--------------------------------------------------\n");

                Console.WriteLine($"Login record saved to: {currentDataFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving login record: {ex.Message}");
                throw;
            }
        }

        public static void SaveResetRequestRecord(ResetRequestRecord resetRequest)
        {
            try
            {
                Initialize();

                string currentDataFilePath = GetCurrentDataFilePath();
                string record = resetRequest.ToFileString();

                File.AppendAllText(currentDataFilePath, $"{record}\n");
                File.AppendAllText(currentDataFilePath, "--------------------------------------------------\n");

                Console.WriteLine($"Reset request record saved to: {currentDataFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving reset request record: {ex.Message}");
                throw;
            }
        }

        public static UserDataCollection LoadAllData()
        {
            var collection = new UserDataCollection();

            try
            {
                if (!Directory.Exists(BaseDirectory))
                {
                    Console.WriteLine("Base directory not found, returning empty collection");
                    return collection;
                }

                var weekFolders = Directory.GetDirectories(BaseDirectory, "UsersDataControl(*)", SearchOption.TopDirectoryOnly);

                foreach (var weekFolder in weekFolders.OrderBy(f => f))
                {
                    string dataFile = Path.Combine(weekFolder, "UsersDataControl.txt");
                    if (File.Exists(dataFile))
                    {
                        LoadDataFromFile(dataFile, collection);
                    }
                }

                if (collection.Users.Count > 0)
                {
                    collection.NextId = collection.Users.Max(u => u.Id) + 1;
                }

                Console.WriteLine($"Loaded {collection.Users.Count} user records, {collection.LoginRecords.Count} login records, and {collection.ResetRequests.Count} reset requests from all files");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading user data from all files: {ex.Message}");
            }

            return collection;
        }

        private static void LoadDataFromFile(string filePath, UserDataCollection collection)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line) &&
                        !line.StartsWith("=") &&
                        !line.StartsWith("-") &&
                        !line.StartsWith("USERS"))
                    {
                        try
                        {
                            if (line.Contains("Login Time"))
                            {
                                var loginRecord = UserLoginRecord.FromFileString(line);
                                if (!string.IsNullOrEmpty(loginRecord.OprID))
                                {
                                    collection.LoginRecords.Add(loginRecord);
                                }
                            }
                            else if (line.Contains("Reset Request Time"))
                            {
                                var resetRequest = ResetRequestRecord.FromFileString(line);
                                if (!string.IsNullOrEmpty(resetRequest.SocketPN))
                                {
                                    collection.ResetRequests.Add(resetRequest);
                                }
                            }
                            else if (line.Contains("User ID"))
                            {
                                var user = UserModel.FromFileString(line);
                                if (!string.IsNullOrEmpty(user.OprID) && user.Id > 0)
                                {
                                    var existingUser = collection.Users.FirstOrDefault(u => u.Id == user.Id);
                                    if (existingUser == null)
                                    {
                                        collection.Users.Add(user);
                                    }
                                    else
                                    {
                                        existingUser.OprID = user.OprID;
                                        existingUser.PasswordHash = user.PasswordHash;
                                        existingUser.Role = user.Role;
                                        existingUser.IsActive = user.IsActive;
                                        existingUser.CreatedAt = user.CreatedAt;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing user line: {line} - {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data from file {filePath}: {ex.Message}");
            }
        }

        public static UserModel GetUserByOprID(string oprId)
        {
            var allData = LoadAllData();
            return allData.GetByOprID(oprId);
        }

        public static List<string> GetWeekFolders()
        {
            try
            {
                if (!Directory.Exists(BaseDirectory))
                    return new List<string>();

                var weekFolders = Directory.GetDirectories(BaseDirectory, "UsersDataControl(*)", SearchOption.TopDirectoryOnly);
                return weekFolders.OrderBy(f => f).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting week folders: {ex.Message}");
                return new List<string>();
            }
        }

        public static List<UserModel> GetAllActiveUsers()
        {
            var allData = LoadAllData();
            return allData.GetActiveUsers();
        }
    }
}