using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HeuristicLab.Hive.Client.Core.JobStorrage {
  public class JobStorrageManager {
    private static List<JobStorrageInfo> StoredJobsList = new List<JobStorrageInfo>();
    
    public static void PersistObjectToDisc(String serverIP, long serverPort, long jobId, byte[] job) {
      String filename = serverIP + serverPort + jobId.ToString();
      
      JobStorrageInfo info = new JobStorrageInfo { JobID = jobId, ServerIP = serverIP, ServerPort = serverPort, TimeFinished = DateTime.Now };
      try {
        Stream jobstream = File.Create("jobStorrage\\"+filename + ".dat");
        jobstream.Write(job, 0, job.Length);
        StoredJobsList.Add(info);
        jobstream.Close();
      }
      catch (Exception e) {
        Console.WriteLine(e);
      }       
    }
  }
}
