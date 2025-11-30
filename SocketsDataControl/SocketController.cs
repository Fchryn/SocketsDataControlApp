using SocketsDataControl.FileManager;
using SocketsDataControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocketsDataControl.Controllers
{
    public class SocketController
    {
        private const int MAX_QTY = 240000;
        private SocketDataCollection dataCollection;
        private string currentOprID;

        public SocketController(string oprId = "")
        {
            currentOprID = oprId;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dataCollection = SocketFileManager.LoadAllData();
                Console.WriteLine("All socket data loaded from file");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading socket data: {ex.Message}");
                dataCollection = new SocketDataCollection();
            }
        }

        public int AddOutput(int qty, int currentTotalRun)
        {
            if (qty < 0) qty = 0;
            int newTotalRun = currentTotalRun + qty;

            if (newTotalRun >= MAX_QTY)
            {
                int overflow = newTotalRun - MAX_QTY;
                newTotalRun = overflow;
                return overflow;
            }
            return newTotalRun;
        }

        public int[] DistributeToCav(int cavUse, int outputQty, int[] existingCavRuns = null)
        {
            if (cavUse < 1) cavUse = 1;
            if (cavUse > 8) cavUse = 8;

            int[] runs = new int[8];

            if (existingCavRuns != null && existingCavRuns.Length == 8)
            {
                Array.Copy(existingCavRuns, runs, 8);
            }

            if (outputQty > 0)
            {
                int baseQty = outputQty / cavUse;
                int remainder = outputQty % cavUse;

                for (int i = 0; i < cavUse; i++)
                {
                    runs[i] += baseQty + (i < remainder ? 1 : 0);
                }
            }

            return runs;
        }

        public void Save(SocketModel model)
        {
            try
            {
                int nextSequence = SocketFileManager.GetNextSequenceNumber(model.SocketPN);
                model.SequenceNumber = nextSequence;

                SocketFileManager.SaveToFile(model);
                dataCollection.Add(model);

                Console.WriteLine($"Socket data saved for {model.SocketPN} by {model.OprID}, Sequence: {nextSequence}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save socket data: {ex.Message}", ex);
            }
        }

        public SocketModel LoadLatestByPN(string pn)
        {
            try
            {
                SocketModel fromFile = SocketFileManager.GetLatestRecordByPN(pn);

                if (fromFile == null)
                {
                    Console.WriteLine($"No existing data found for {pn}, creating default");
                    fromFile = new SocketModel
                    {
                        OprID = currentOprID,
                        SocketPN = pn,
                        CavUse = 8,
                        OutputQty = 0,
                        TotalRun = 0,
                        CavRuns = new int[8],
                        CavStatus = new string[8] { "Good", "Good", "Good", "Good", "Good", "Good", "Good", "Good" },
                        CreatedAt = DateTime.Now,
                        SequenceNumber = 1
                    };
                }
                else
                {
                    Console.WriteLine($"Loaded latest data for {pn}: Sequence={fromFile.SequenceNumber}, TotalRun={fromFile.TotalRun}, CreatedAt={fromFile.CreatedAt}");
                    fromFile.OprID = currentOprID;
                }

                return fromFile;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data for {pn}: {ex.Message}");
                return new SocketModel
                {
                    OprID = currentOprID,
                    SocketPN = pn,
                    CavUse = 8,
                    OutputQty = 0,
                    TotalRun = 0,
                    CavRuns = new int[8],
                    CavStatus = new string[8] { "Good", "Good", "Good", "Good", "Good", "Good", "Good", "Good" },
                    CreatedAt = DateTime.Now,
                    SequenceNumber = 1
                };
            }
        }

        public int GetLatestTotalQuantityByPN(string socketPN)
        {
            try
            {
                return SocketFileManager.GetLatestTotalQuantityByPN(socketPN);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting latest total quantity for {socketPN}: {ex.Message}");
                return 0;
            }
        }

        public int CalculateTotalRunFromCavRuns(int[] cavRuns)
        {
            int total = 0;
            for (int i = 0; i < cavRuns.Length; i++)
            {
                total += cavRuns[i];
            }
            return total;
        }

        public string[] GetAllPredefinedSockets()
        {
            return SocketDataCollection.PredefinedSockets.ToArray();
        }

        public string GetImagePath(string socketPN)
        {
            return SocketFileManager.GetImagePath(socketPN);
        }

        public int GetTotalQuantityByPN(string socketPN)
        {
            return SocketFileManager.GetTotalQuantityByPN(socketPN);
        }

        public bool ResetSocketData(string socketPN)
        {
            try
            {
                var allData = SocketFileManager.LoadAllData();
                int nextSequence = allData.GetNextSequenceNumber(socketPN);

                var resetSocket = new SocketModel
                {
                    OprID = "SYSTEM_RESET",
                    CavUse = 8,
                    SocketPN = socketPN,
                    OutputQty = 0,
                    TotalRun = 0,
                    CavRuns = new int[8],
                    CavStatus = new string[8] { "Good", "Good", "Good", "Good", "Good", "Good", "Good", "Good" },
                    CreatedAt = DateTime.Now,
                    SequenceNumber = nextSequence
                };

                Save(resetSocket);

                Console.WriteLine($"Socket data reset for: {socketPN}, Sequence: {nextSequence}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting socket data for {socketPN}: {ex.Message}");
                throw new Exception($"Failed to reset socket data for {socketPN}: {ex.Message}", ex);
            }
        }

        public List<SocketModel> GetSocketHistory(string socketPN)
        {
            try
            {
                return SocketFileManager.GetRecordsByPN(socketPN)
                    .OrderByDescending(s => s.SequenceNumber)
                    .ThenByDescending(s => s.CreatedAt)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting socket history: {ex.Message}");
                return new List<SocketModel>();
            }
        }

        public string GetSocketLimitInfo(string socketPN)
        {
            int maxLimit = SocketModel.GetCavityMaxLimitForSocket(socketPN);
            int warningLimit = SocketModel.GetCavityWarningLimitForSocket(socketPN);
            return $"Batas Maksimal: {maxLimit:N0} | Batas Peringatan: {warningLimit:N0}";
        }

        public (List<int> nearLimit, List<int> atLimit) CheckCavityLimits(SocketModel model)
        {
            var nearLimit = model.GetCavitiesNearLimit();
            var atLimit = model.GetCavitiesAtLimit();
            return (nearLimit, atLimit);
        }

        public List<SocketModel> GetLatestForAllSockets()
        {
            try
            {
                return SocketFileManager.GetLatestForAllSockets();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting latest for all sockets: {ex.Message}");
                return new List<SocketModel>();
            }
        }

        public int GetNextSequenceNumber(string socketPN)
        {
            try
            {
                return SocketFileManager.GetNextSequenceNumber(socketPN);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting next sequence number for {socketPN}: {ex.Message}");
                return 1;
            }
        }
    }
}