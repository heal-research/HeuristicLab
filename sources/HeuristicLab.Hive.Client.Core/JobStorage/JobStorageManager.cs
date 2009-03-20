using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HeuristicLab.Hive.Client.Common;
using HeuristicLab.Hive.Client.Communication;
using HeuristicLab.Hive.Client.Core.ConfigurationManager;
using HeuristicLab.Hive.Contracts;

namespace HeuristicLab.Hive.Client.Core.JobStorage {
  public class JobStorageManager {
    
    private static List<JobStorageInfo> storedJobsList = new List<JobStorageInfo>();
    //Todo: execution path
    //Todo: Choose a better directory name 
    private static String path = "C:\\Program Files\\HeuristicLab 3.0\\plugins\\jobStorrage\\";
    
    public static void PersistObjectToDisc(String serverIP, long serverPort, long jobId, byte[] job) {
      String filename = serverIP + "." + serverPort + "." + jobId.ToString();

      JobStorageInfo info = new JobStorageInfo { JobID = jobId, ServerIP = serverIP, ServerPort = serverPort, TimeFinished = DateTime.Now };
            
      Stream jobstream = null;
      try {
        jobstream = File.Create(path + filename + ".dat");
        jobstream.Write(job, 0, job.Length);
        storedJobsList.Add(info);
        Logging.Instance.Info("JobStorageManager", "Job " + info.JobID + " stored on the harddisc");
      }
      catch (Exception e) {
        Logging.Instance.Error("JobStorageManager", "Exception: ", e);
      }
      finally {
        if(jobstream!=null)
          jobstream.Close();
      }
    }

    public static void CheckAndSubmitJobsFromDisc() {
      for(int index=storedJobsList.Count; index > 0; index--) {
        if (WcfService.Instance.ConnState == NetworkEnum.WcfConnState.Loggedin && (storedJobsList[index-1].ServerIP == WcfService.Instance.ServerIP && storedJobsList[index-1].ServerPort == WcfService.Instance.ServerPort)) {
          String filename = storedJobsList[index-1].ServerIP + "." + storedJobsList[index-1].ServerPort + "." + storedJobsList[index-1].JobID.ToString();
          Logging.Instance.Info("JobStorrageManager", "Sending stored job " + storedJobsList[index - 1].JobID + " to the server");
          byte[] job = File.ReadAllBytes(path + filename + ".dat");
          
          //Todo: ask server first if he really wants the job...
          ResponseResultReceived res = WcfService.Instance.SendStoredJobResultsSync(ConfigManager.Instance.GetClientInfo().ClientId, storedJobsList[index-1].JobID, job, 1.00, null, true);
          //TODO: has to be fixed from server side
          //if (res.Success == true) {
          Logging.Instance.Info("JobStorrageManager", "Sending of job " + storedJobsList[index - 1].JobID + " done");  
          storedJobsList.Remove(storedJobsList[index - 1]);
          File.Delete(path + filename + ".dat");
            
       //   }
        }
      }
    }
  }
}
