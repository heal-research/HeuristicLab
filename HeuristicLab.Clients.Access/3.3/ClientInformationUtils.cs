#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2013 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using HeuristicLab.Algorithms.Benchmarks;
using HeuristicLab.Data;
using HeuristicLab.Optimization;

namespace HeuristicLab.Clients.Access {
  public static class ClientInformationUtils {

    public static Client CollectClientInformation() {
      Client client = new Client();
      OperatingSystem os = new OperatingSystem();
      ClientType cType = new ClientType();
      cType.Name = Settings.Default.ClientTypeName;

      client.Id = GetUniqueMachineId();
      client.HeuristicLabVersion = GetHLVersion();
      client.Name = GetMachineName();
      client.MemorySize = GetPhysicalMemory().GetValueOrDefault();
      client.NumberOfCores = GetNumberOfCores();
      os.Name = GetOperatingSystem();
      client.OperatingSystem = os;
      client.ProcessorType = GetCpuInfo();
      client.ClientType = cType;
      client.ClientConfiguration = GetClientConfiguration();
      client.Timestamp = DateTime.Now;
      client.PerformanceValue = RunBenchmark();

      return client;
    }

    public static ClientConfiguration GetClientConfiguration() {
      try {
        string filePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
        byte[] fileContent = File.ReadAllBytes(filePath);
        byte[] hashBytes;
        using (SHA1 sha1 = SHA1.Create()) hashBytes = sha1.ComputeHash(fileContent);
        StringBuilder sb = new StringBuilder();
        foreach (byte b in hashBytes) sb.Append(b.ToString("x2"));
        return new ClientConfiguration { Hash = sb.ToString() };
      }
      catch {
        return null;
      }
    }

    public static string GetHLVersion() {
      FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
      return versionInfo.FileVersion;
    }

    public static bool IsClientHeuristicLab() {
      return Process.GetCurrentProcess().ProcessName == Settings.Default.HLExeName;
    }

    public static int GetNumberOfCores() {
      return Environment.ProcessorCount;
    }

    public static string GetOperatingSystem() {
      return Environment.OSVersion.VersionString;
    }

    public static string GetMachineName() {
      return Environment.MachineName;
    }

    public static Guid GetUniqueMachineId() {
      Guid id;
      try {
        id = GetUniqueMachineIdFromMac();
      }
      catch {
        // fallback if something goes wrong...        
        id = new Guid(Environment.MachineName.GetHashCode(), 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
      }
      return id;
    }

    /// <summary>
    /// returns total physical memory of the machine in MB
    /// </summary>
    public static int? GetPhysicalMemory() {
      long? res = GetWMIValue("Win32_ComputerSystem", "TotalPhysicalMemory");
      if (res != null)
        return (int)(res / 1024 / 1024);
      else
        return null;
    }

    /// <summary>
    /// returns CPU frequence of the machine in Mhz
    /// </summary>
    public static string GetCpuInfo() {
      string name = GetWMIString("Win32_Processor", "Name");
      string manufacturer = GetWMIString("Win32_Processor", "Manufacturer");
      return manufacturer + " " + name;
    }

    /// <summary>
    /// Generate a guid based on mac address of the first found nic (yes, mac addresses are not unique...)
    /// and the machine name.
    /// Format:
    /// 
    ///  D1      D2  D3  Res.   D4
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// |n a m e|0 0|0 0|0 0 mac address|
    /// +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    /// 
    /// The mac address is saved in the last 48 bits of the Data 4 segment 
    /// of the guid (first 2 bytes of Data 4 are reserved).
    /// D1 contains the hash of the machinename. 
    /// </summary>    
    private static Guid GetUniqueMachineIdFromMac() {
      ManagementClass mgtClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
      ManagementObjectCollection mgtCol = mgtClass.GetInstances();

      foreach (ManagementObject mgtObj in mgtCol) {
        foreach (var prop in mgtObj.Properties) {
          if (prop.Value != null && prop.Name == "MACAddress") {
            try {
              //simply take the first nic
              string mac = prop.Value.ToString();
              byte[] b = new byte[8];
              string[] macParts = mac.Split(':');
              if (macParts.Length == 6) {
                for (int i = 0; i < macParts.Length; i++) {
                  b[i + 2] = (byte)((ParseNybble(macParts[i][0]) << 4) | ParseNybble(macParts[i][1]));
                }

                // also get machine name and save it to the first 4 bytes                
                Guid guid = new Guid(Environment.MachineName.GetHashCode(), 0, 0, b);
                return guid;
              } else
                throw new Exception("Error getting mac addresse");
            }
            catch {
              throw new Exception("Error getting mac addresse");
            }
          }
        }
      }
      throw new Exception("Error getting mac addresse");
    }

    /// <summary>
    /// return numeric value of a single hex-char
    /// (see: http://stackoverflow.com/questions/854012/how-to-convert-hex-to-a-byte-array)
    /// </summary>    
    private static int ParseNybble(char c) {
      if (c >= '0' && c <= '9') {
        return c - '0';
      }
      if (c >= 'A' && c <= 'F') {
        return c - 'A' + 10;
      }
      if (c >= 'a' && c <= 'f') {
        return c - 'a' + 10;
      }
      throw new ArgumentException("Invalid hex digit: " + c);
    }

    private static long? GetWMIValue(string clazz, string property) {
      ManagementClass mgtClass = new ManagementClass(clazz);
      ManagementObjectCollection mgtCol = mgtClass.GetInstances();

      foreach (ManagementObject mgtObj in mgtCol) {
        foreach (var prop in mgtObj.Properties) {
          if (prop.Value != null && prop.Name == property) {
            try {
              return long.Parse(prop.Value.ToString());
            }
            catch {
              return null;
            }
          }
        }
      }
      return null;
    }

    private static string GetWMIString(string clazz, string property) {
      ManagementClass mgtClass = new ManagementClass(clazz);
      ManagementObjectCollection mgtCol = mgtClass.GetInstances();

      foreach (ManagementObject mgtObj in mgtCol) {
        foreach (var prop in mgtObj.Properties) {
          if (prop.Value != null && prop.Name == property) {
            try {
              return prop.Value.ToString();
            }
            catch {
              return string.Empty;
            }
          }
        }
      }
      return string.Empty;
    }

    public static double RunBenchmark() {
      Linpack linpack = new Linpack();
      ResultCollection results = new ResultCollection();
      linpack.Run(new System.Threading.CancellationToken(), results);
      DoubleValue mflops = (DoubleValue)results["Mflops/s"].Value;
      return mflops.Value;
    }
  }
}
