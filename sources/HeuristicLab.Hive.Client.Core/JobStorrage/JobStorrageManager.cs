using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HeuristicLab.Hive.Client.Common;
using HeuristicLab.Hive.Client.Communication;

namespace HeuristicLab.Hive.Client.Core.JobStorrage {
  public class JobStorrageManager {
    private static List<JobStorrageInfo> StoredJobsList = new List<JobStorrageInfo>();
    
    public static void PersistObjectToDisc(String serverIP, long serverPort, long jobId, byte[] job) {
      String filename = serverIP + "." + serverPort + "." + jobId.ToString();
      
      JobStorrageInfo info = new JobStorrageInfo { JobID = jobId, ServerIP = serverIP, ServerPort = serverPort, TimeFinished = DateTime.Now };
      try {
        Stream jobstream = File.Create("C:\\Program Files\\HeuristicLab 3.0\\plugins\\jobStorrage\\ "+filename + ".dat");
        jobstream.Write(job, 0, job.Length);
        StoredJobsList.Add(info);
        jobstream.Close();
        Logging.GetInstance().Info("JobStorrageManager", "Job " + info.JobID + " stored on the harddisc");
      }
      catch (Exception e) {
        Console.WriteLine(e);
      }       
    }

    public static void CheckAndSubmitJobsFromDisc() {
      foreach (JobStorrageInfo info in StoredJobsList) {
        if (WcfService.Instance.ConnState == NetworkEnum.WcfConnState.Loggedin && (info.ServerIP == WcfService.Instance.ServerIP && info.ServerPort == WcfService.Instance.ServerPort)) {
          Logging.GetInstance().Info("JobStorrageManager", "Sending stored job " + info.JobID + " to the server");
        }
      }
    }
  }
}
