using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SocketsDataControl.Models;

namespace SocketsDataControl.FileManager
{
    public static class SocketFileManager
    {
        private static readonly string BaseDirectory = @"C:\SocketsDataControl";
        private static readonly string ImageBasePath = @"C:\SocketsDataControl\SocketsDataControl\Resources\images";

        private static string GetCurrentWeekFolder()
        {
            DateTime today = DateTime.Today;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
            string weekFolderName = $"SocketsDataControl({startOfWeek:dd-MM-yyyy})";
            return Path.Combine(BaseDirectory, weekFolderName);
        }

        private static string GetCurrentDataFilePath()
        {
            return Path.Combine(GetCurrentWeekFolder(), "SocketsDataControl.txt");
        }

        public static void Initialize()
        {
            try
            {
                if (!Directory.Exists(BaseDirectory))
                {
                    Directory.CreateDirectory(BaseDirectory);
                    Console.WriteLine($"Directory created: {BaseDirectory}");
                }

                if (!Directory.Exists(ImageBasePath))
                {
                    Directory.CreateDirectory(ImageBasePath);
                    Console.WriteLine($"Image directory created: {ImageBasePath}");
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
                    File.WriteAllText(currentDataFilePath, "SOCKETS DATA CONTROL - PRODUCTION RECORDS\n");
                    File.AppendAllText(currentDataFilePath, "==========================================\n\n");
                    Console.WriteLine($"Data file created: {currentDataFilePath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing file system: {ex.Message}");
                throw;
            }
        }

        public static void SaveToFile(SocketModel socket)
        {
            try
            {
                Initialize();

                string currentDataFilePath = GetCurrentDataFilePath();
                string record = socket.ToFileString();

                File.AppendAllText(currentDataFilePath, $"{record}\n");
                File.AppendAllText(currentDataFilePath, "--------------------------------------------------\n");

                Console.WriteLine($"Data saved to: {currentDataFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
                throw;
            }
        }

        public static SocketDataCollection LoadAllData()
        {
            var collection = new SocketDataCollection();

            try
            {
                if (!Directory.Exists(BaseDirectory))
                {
                    Console.WriteLine("Base directory not found, returning empty collection");
                    return collection;
                }

                var weekFolders = Directory.GetDirectories(BaseDirectory, "SocketsDataControl(*)", SearchOption.TopDirectoryOnly);

                foreach (var weekFolder in weekFolders.OrderBy(f => f))
                {
                    string dataFile = Path.Combine(weekFolder, "SocketsDataControl.txt");
                    if (File.Exists(dataFile))
                    {
                        LoadDataFromFile(dataFile, collection);
                    }
                }

                Console.WriteLine($"Loaded {collection.Sockets.Count} total records from all week folders");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data from all files: {ex.Message}");
            }

            return collection;
        }

        private static void LoadDataFromFile(string filePath, SocketDataCollection collection)
        {
            try
            {
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line) &&
                        !line.StartsWith("=") &&
                        !line.StartsWith("-") &&
                        !line.StartsWith("SOCKETS"))
                    {
                        try
                        {
                            var socket = SocketModel.FromFileString(line);
                            if (!string.IsNullOrEmpty(socket.SocketPN))
                            {
                                collection.Sockets.Add(socket);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error parsing line: {line} - {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data from file {filePath}: {ex.Message}");
            }
        }

        public static SocketModel GetLatestRecordByPN(string socketPN)
        {
            try
            {
                var allData = LoadAllData();
                var latestRecord = allData.GetLatestByPN(socketPN);

                if (latestRecord != null)
                {
                    Console.WriteLine($"Loaded latest record for {socketPN}: Sequence={latestRecord.SequenceNumber}, TotalRun={latestRecord.TotalRun}, CreatedAt={latestRecord.CreatedAt}");
                }
                else
                {
                    Console.WriteLine($"No records found for {socketPN}, returning null");
                }

                return latestRecord;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting latest record for {socketPN}: {ex.Message}");
                return null;
            }
        }

        public static string GetImagePath(string socketPN)
        {
            try
            {
                string[] extensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                foreach (var ext in extensions)
                {
                    string fullPath = Path.Combine(ImageBasePath, socketPN + ext);
                    if (File.Exists(fullPath))
                    {
                        return fullPath;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting image path: {ex.Message}");
                return null;
            }
        }

        public static List<SocketModel> GetRecordsByPN(string socketPN)
        {
            var allData = LoadAllData();
            return allData.GetAllByPN(socketPN);
        }

        public static int GetTotalQuantityByPN(string socketPN)
        {
            var allData = LoadAllData();
            var records = allData.GetAllByPN(socketPN);
            return records.Sum(r => r.TotalRun);
        }

        public static int GetLatestTotalQuantityByPN(string socketPN)
        {
            var latest = GetLatestRecordByPN(socketPN);
            return latest?.TotalRun ?? 0;
        }

        public static bool ResetSocketDataByPN(string socketPN, string adminOprID)
        {
            try
            {
                Initialize();

                string currentDataFilePath = GetCurrentDataFilePath();

                var allData = LoadAllData();
                int nextSequence = allData.GetNextSequenceNumber(socketPN);

                var resetSocket = new SocketModel
                {
                    OprID = $"SYSTEM_RESET_BY_{adminOprID}",
                    CavUse = 8,
                    SocketPN = socketPN,
                    OutputQty = 0,
                    TotalRun = 0,
                    CavRuns = new int[8],
                    CavStatus = new string[8] { "Good", "Good", "Good", "Good", "Good", "Good", "Good", "Good" },
                    CreatedAt = DateTime.Now,
                    SequenceNumber = nextSequence
                };

                SaveToFile(resetSocket);

                Console.WriteLine($"Socket data reset for: {socketPN} by admin: {adminOprID}, Sequence: {nextSequence}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting socket data for {socketPN}: {ex.Message}");
                throw new Exception($"Failed to reset socket data for {socketPN}: {ex.Message}", ex);
            }
        }

        public static List<string> GetWeekFolders()
        {
            try
            {
                if (!Directory.Exists(BaseDirectory))
                    return new List<string>();

                var weekFolders = Directory.GetDirectories(BaseDirectory, "SocketsDataControl(*)", SearchOption.TopDirectoryOnly);
                return weekFolders.OrderBy(f => f).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting week folders: {ex.Message}");
                return new List<string>();
            }
        }

        public static int GetNextSequenceNumber(string socketPN)
        {
            var allData = LoadAllData();
            return allData.GetNextSequenceNumber(socketPN);
        }

        public static List<SocketModel> GetLatestForAllSockets()
        {
            var allData = LoadAllData();
            return allData.GetLatestForAllSockets();
        }
    }
}