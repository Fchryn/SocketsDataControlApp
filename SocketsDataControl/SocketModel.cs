using System;
using System.Collections.Generic;
using System.Linq;

namespace SocketsDataControl.Models
{
    public class SocketModel
    {
        public int Id { get; set; }
        public string OprID { get; set; } = "";
        public List<string> OprIDs { get; set; } = new List<string>();
        public int CavUse { get; set; } = 8;
        public string SocketPN { get; set; } = "";
        public int OutputQty { get; set; } = 0;
        public int TotalRun { get; set; } = 0;
        public int[] CavRuns { get; set; } = new int[8];
        public string[] CavStatus { get; set; } = new string[8];
        public string ImagePath { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int SequenceNumber { get; set; } = 1;

        private static readonly Dictionary<string, int> SocketLimits = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            {"1.HD-PICS33FJ-O-TQ44A", 10000},
            {"2.UHD-MKE14XXX-QF64", 10000},
            {"3.HD-PICS-SS20", 20000},
            {"4.HD-PICS-SS28", 20000},
            {"5.HD-PICS-SO14", 20000},
            {"6.HD-PICS-SO8", 10000},
            {"7.NS-MCU-TSSO020065044-010E", 20000},
            {"8.NS-MCU-TSSO020076057-001E", 20000},
            {"9.NS-MCU-SSOP028105057-006E", 20000},
            {"10.NS-MCU-SOP014150mil-002E", 20000},
            {"11.NS-MCU-LQFP064100100-007E", 10000},
            {"12.NS-MCU-TQFP044100100-010E", 10000},
            {"13.ND-NAND-TSOP048184120-001Y", 20000},
            {"14.NS-MCU-QFN028040040-016D", 60000},
            {"15.ND-QSPI-SOP008207mil-001E", 20000},
            {"16.ND-QSPI-SOP008207mil-002W", 20000},
            {"17.NS-MCU-TQFP048070070-009E", 10000},
            {"18.NS-MCU-TQFP048070070-005E", 10000},
            {"19.NS-MCU-SOP020127075-010E", 10000},
            {"20.NS-MCU-QFN032050050-053D", 60000}
        };

        private const int DEFAULT_CAVITY_MAX_LIMIT = 20000;

        public SocketModel()
        {
            for (int i = 0; i < 8; i++)
            {
                CavRuns[i] = 0;
                CavStatus[i] = "Good";
            }
        }

        public int CavityMaxLimit
        {
            get
            {
                if (SocketLimits.ContainsKey(SocketPN))
                    return SocketLimits[SocketPN];
                return DEFAULT_CAVITY_MAX_LIMIT;
            }
        }

        public int CavityWarningLimit
        {
            get
            {
                return (int)(CavityMaxLimit * 0.8);
            }
        }

        public string ToFileString()
        {
            string combinedOprIDs = OprIDs.Count > 0 ? string.Join(" dan ", OprIDs) : OprID;

            return $"Sequence Number : {SequenceNumber}; " +
                   $"Operator ID : {combinedOprIDs}; " +
                   $"Cavity Use : {CavUse}; " +
                   $"Socket PN : {SocketPN}; " +
                   $"Output Qty : {OutputQty}; " +
                   $"Total Qty Ic : {TotalRun}; " +
                   $"Cav1 Run : {CavRuns[0]}; " +
                   $"Cav2 Run : {CavRuns[1]}; " +
                   $"Cav3 Run : {CavRuns[2]}; " +
                   $"Cav4 Run : {CavRuns[3]}; " +
                   $"Cav5 Run : {CavRuns[4]}; " +
                   $"Cav6 Run : {CavRuns[5]}; " +
                   $"Cav7 Run : {CavRuns[6]}; " +
                   $"Cav8 Run : {CavRuns[7]}; " +
                   $"Status Cav1 : {CavStatus[0]}; " +
                   $"Status Cav2 : {CavStatus[1]}; " +
                   $"Status Cav3 : {CavStatus[2]}; " +
                   $"Status Cav4 : {CavStatus[3]}; " +
                   $"Status Cav5 : {CavStatus[4]}; " +
                   $"Status Cav6 : {CavStatus[5]}; " +
                   $"Status Cav7 : {CavStatus[6]}; " +
                   $"Status Cav8 : {CavStatus[7]}; " +
                   $"Created At : {CreatedAt:dd-MM-yyyy HH:mm:ss}";
        }

        public static SocketModel FromFileString(string fileString)
        {
            var model = new SocketModel();

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
                                case "Sequence Number":
                                    model.SequenceNumber = int.TryParse(value, out int seqNum) ? seqNum : 1;
                                    break;
                                case "Operator ID":
                                    if (value.Contains("dan"))
                                    {
                                        var operators = value.Split(new[] { "dan" }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach (var op in operators)
                                        {
                                            model.OprIDs.Add(op.Trim());
                                        }
                                        model.OprID = string.Join(", ", model.OprIDs);
                                    }
                                    else
                                    {
                                        model.OprID = value;
                                        model.OprIDs.Add(value);
                                    }
                                    break;
                                case "Cavity Use":
                                    model.CavUse = int.TryParse(value, out int cavUse) ? cavUse : 8;
                                    break;
                                case "Socket PN":
                                    model.SocketPN = value;
                                    break;
                                case "Output Qty":
                                    model.OutputQty = int.TryParse(value, out int outputQty) ? outputQty : 0;
                                    break;
                                case "Total Qty Ic":
                                    model.TotalRun = int.TryParse(value, out int totalRun) ? totalRun : 0;
                                    break;
                                case "Cav1 Run":
                                    model.CavRuns[0] = int.TryParse(value, out int cav1) ? cav1 : 0;
                                    break;
                                case "Cav2 Run":
                                    model.CavRuns[1] = int.TryParse(value, out int cav2) ? cav2 : 0;
                                    break;
                                case "Cav3 Run":
                                    model.CavRuns[2] = int.TryParse(value, out int cav3) ? cav3 : 0;
                                    break;
                                case "Cav4 Run":
                                    model.CavRuns[3] = int.TryParse(value, out int cav4) ? cav4 : 0;
                                    break;
                                case "Cav5 Run":
                                    model.CavRuns[4] = int.TryParse(value, out int cav5) ? cav5 : 0;
                                    break;
                                case "Cav6 Run":
                                    model.CavRuns[5] = int.TryParse(value, out int cav6) ? cav6 : 0;
                                    break;
                                case "Cav7 Run":
                                    model.CavRuns[6] = int.TryParse(value, out int cav7) ? cav7 : 0;
                                    break;
                                case "Cav8 Run":
                                    model.CavRuns[7] = int.TryParse(value, out int cav8) ? cav8 : 0;
                                    break;
                                case "Status Cav1":
                                    model.CavStatus[0] = value;
                                    break;
                                case "Status Cav2":
                                    model.CavStatus[1] = value;
                                    break;
                                case "Status Cav3":
                                    model.CavStatus[2] = value;
                                    break;
                                case "Status Cav4":
                                    model.CavStatus[3] = value;
                                    break;
                                case "Status Cav5":
                                    model.CavStatus[4] = value;
                                    break;
                                case "Status Cav6":
                                    model.CavStatus[5] = value;
                                    break;
                                case "Status Cav7":
                                    model.CavStatus[6] = value;
                                    break;
                                case "Status Cav8":
                                    model.CavStatus[7] = value;
                                    break;
                                case "Created At":
                                    if (DateTime.TryParse(value, out DateTime createdAt))
                                        model.CreatedAt = createdAt;
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing file string: {ex.Message}");
            }

            return model;
        }

        public bool IsCavityAtLimit(int cavityIndex)
        {
            return cavityIndex >= 0 && cavityIndex < 8 && CavRuns[cavityIndex] >= CavityMaxLimit;
        }

        public bool IsCavityNearLimit(int cavityIndex)
        {
            return cavityIndex >= 0 && cavityIndex < 8 && CavRuns[cavityIndex] >= CavityWarningLimit;
        }

        public List<int> GetCavitiesAtLimit()
        {
            var cavitiesAtLimit = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                if (IsCavityAtLimit(i))
                {
                    cavitiesAtLimit.Add(i + 1);
                }
            }
            return cavitiesAtLimit;
        }

        public List<int> GetCavitiesNearLimit()
        {
            var cavitiesNearLimit = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                if (IsCavityNearLimit(i) && !IsCavityAtLimit(i))
                {
                    cavitiesNearLimit.Add(i + 1);
                }
            }
            return cavitiesNearLimit;
        }

        public static int GetCavityMaxLimitForSocket(string socketPN)
        {
            if (SocketLimits.ContainsKey(socketPN))
                return SocketLimits[socketPN];
            return DEFAULT_CAVITY_MAX_LIMIT;
        }

        public static int GetCavityWarningLimitForSocket(string socketPN)
        {
            return (int)(GetCavityMaxLimitForSocket(socketPN) * 0.8);
        }
    }

    public class SocketDataCollection
    {
        public List<SocketModel> Sockets { get; set; } = new List<SocketModel>();

        public static readonly List<string> PredefinedSockets = new List<string>
        {
            "1.HD-PICS33FJ-O-TQ44A",
            "2.UHD-MKE14XXX-QF64",
            "3.HD-PICS-SS20",
            "4.HD-PICS-SS28",
            "5.HD-PICS-SO14",
            "6.HD-PICS-SO8",
            "7.NS-MCU-TSSO020065044-010E",
            "8.NS-MCU-TSSO020076057-001E",
            "9.NS-MCU-SSOP028105057-006E",
            "10.NS-MCU-SOP014150mil-002E",
            "11.NS-MCU-LQFP064100100-007E",
            "12.NS-MCU-TQFP044100100-001E",
            "13.ND-NAND-TSOP048184120-001Y",
            "14.NS-MCU-QFN028040040-016D",
            "15.ND-QSPI-SOP008207mil-001E",
            "16.ND-QSPI-SOP008207mil-002W",
            "17.NS-MCU-TQFP048070070-009E",
            "18.NS-MCU-TQFP048070070-005E",
            "19.NS-MCU-SOP020127075-010E",
            "20.NS-MCU-QFN032050050-053D"
        };

        public SocketModel GetLatestByPN(string pn)
        {
            return Sockets
                .Where(s => s.SocketPN == pn)
                .OrderByDescending(s => s.SequenceNumber)
                .ThenByDescending(s => s.CreatedAt)
                .FirstOrDefault();
        }

        public List<SocketModel> GetAllByPN(string pn)
        {
            return Sockets
                .Where(s => s.SocketPN == pn)
                .OrderByDescending(s => s.SequenceNumber)
                .ThenByDescending(s => s.CreatedAt)
                .ToList();
        }

        public int GetNextSequenceNumber(string pn)
        {
            var latest = GetLatestByPN(pn);
            return latest != null ? latest.SequenceNumber + 1 : 1;
        }

        public void Add(SocketModel socket)
        {
            socket.Id = Sockets.Count > 0 ? Sockets.Max(s => s.Id) + 1 : 1;
            Sockets.Add(socket);
        }

        public List<SocketModel> GetLatestForAllSockets()
        {
            var result = new List<SocketModel>();
            var distinctPNs = Sockets.Select(s => s.SocketPN).Distinct();

            foreach (var pn in distinctPNs)
            {
                var latest = GetLatestByPN(pn);
                if (latest != null)
                {
                    result.Add(latest);
                }
            }

            return result.OrderBy(s => s.SocketPN).ToList();
        }
    }
}