using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HeuristicLab.Hive.Client.Common;
using HeuristicLab.Hive.Client.Communication;
using HeuristicLab.Hive.Client.Core.ConfigurationManager;
using HeuristicLab.Hive.Contracts;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Xml;

namespace HeuristicLab.Hive.Client.Core.JobStorage {
  public class JobStorageManager {
    
    private static List<JobStorageInfo> storedJobsList = new List<JobStorageInfo>();
    
    private static String path = System.IO.Directory.GetCurrentDirectory()+"\\plugins\\Hive.Client.Jobs\\";
    
    public static void PersistObjectToDisc(String serverIP, long serverPort, Guid jobId, byte[] job) {
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

      StoreJobList();

    }

    public static void CheckAndSubmitJobsFromDisc() {
      for(int index=storedJobsList.Count; index > 0; index--) {
        if (WcfService.Instance.ConnState == NetworkEnum.WcfConnState.Loggedin && (storedJobsList[index-1].ServerIP == WcfService.Instance.ServerIP && storedJobsList[index-1].ServerPort == WcfService.Instance.ServerPort)) {
          String filename = storedJobsList[index-1].ServerIP + "." + storedJobsList[index-1].ServerPort + "." + storedJobsList[index-1].JobID.ToString();          
          Logging.Instance.Info("JobStorrageManager", "Sending stored job " + storedJobsList[index - 1].JobID + " to the server");
          byte[] job = File.ReadAllBytes(path + filename + ".dat");

          if (WcfService.Instance.IsJobStillNeeded(storedJobsList[index - 1].JobID).StatusMessage == ApplicationConstants.RESPONSE_COMMUNICATOR_SEND_JOBRESULT) {
            ResponseResultReceived res = WcfService.Instance.SendStoredJobResultsSync(ConfigManager.Instance.GetClientInfo().Id, storedJobsList[index - 1].JobID, job, 1.00, null, true);
            if (!res.Success)
              Logging.Instance.Error("JobStorageManager", "sending of job failed: " + res.StatusMessage);
            else
              Logging.Instance.Info("JobStorrageManager", "Sending of job " + storedJobsList[index - 1].JobID + " done");      
          }
          ClientStatusInfo.JobsProcessed++;
                  
          

          storedJobsList.Remove(storedJobsList[index - 1]);
          File.Delete(path + filename + ".dat");       
          }
        }
    }

    public static void StoreJobList() {
      XmlSerializer serializer = new XmlSerializer(typeof(List<JobStorageInfo>));
      TextWriter writer = new StreamWriter(Path.Combine(path ,"list.xml"));
      serializer.Serialize(writer, storedJobsList);
      writer.Close();
    }

    static JobStorageManager() {
      Logging.Instance.Info("JobStorrageManager", "Restoring Joblist from Harddisk");
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
            
      XmlSerializer serializer = new XmlSerializer(typeof(List<JobStorageInfo>));
      if(File.Exists(Path.Combine(path ,"list.xml"))) {
        try {
          FileStream stream = new FileStream(Path.Combine(path, "list.xml"), FileMode.Open);
          XmlTextReader reader = new XmlTextReader(stream);
          storedJobsList = (List<JobStorageInfo>)serializer.Deserialize(reader);
          Logging.Instance.Info("JobStorrageManager", "Loaded " + storedJobsList.Count + " Elements");
        }
        catch (Exception e) {
          Logging.Instance.Error("JobStorrageManager", "Exception while loading the Stored Job List", e);
        }
      } else {
        Logging.Instance.Info("JobStorrageManager", "no stored jobs on harddisk, starting new list");
        storedJobsList = new List<JobStorageInfo>();
      }
    }
    
  }
}
