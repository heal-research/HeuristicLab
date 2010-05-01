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
using HeuristicLab.Tracing;

namespace HeuristicLab.Hive.Client.Core.JobStorage {
  public class JobStorageManager {
    
    private static List<JobStorageInfo> storedJobsList = new List<JobStorageInfo>();
    
    private static String path = System.IO.Directory.GetCurrentDirectory()+"\\Hive.Client.Jobs\\";
    
    public static void PersistObjectToDisc(String serverIP, long serverPort, Guid jobId, byte[] job) {
      String filename = serverIP + "." + serverPort + "." + jobId.ToString();
      JobStorageInfo info = new JobStorageInfo { JobID = jobId, ServerIP = serverIP, ServerPort = serverPort, TimeFinished = DateTime.Now };
      
      Stream jobstream = null;
      try {
        jobstream = File.Create(path + filename + ".dat");
        jobstream.Write(job, 0, job.Length);
        storedJobsList.Add(info);        
        Logger.Info("Job " + info.JobID + " stored on the harddisc");
      }
      catch (Exception e) {
        Logger.Error("Exception: ", e);
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
          Logger.Info("Sending stored job " + storedJobsList[index - 1].JobID + " to the server");
          try {
            byte[] job = File.ReadAllBytes(path + filename + ".dat");
            if (WcfService.Instance.IsJobStillNeeded(storedJobsList[index - 1].JobID).StatusMessage == ApplicationConstants.RESPONSE_COMMUNICATOR_SEND_JOBRESULT) {
              ResponseResultReceived res = WcfService.Instance.SendStoredJobResultsSync(ConfigManager.Instance.GetClientInfo().Id, storedJobsList[index - 1].JobID, job, 1.00, null, true);
              if (!res.Success)
                Logger.Error("sending of job failed: " + res.StatusMessage);
              else
                Logger.Info("Sending of job " + storedJobsList[index - 1].JobID + " done");
            }
            ClientStatusInfo.JobsProcessed++;
            storedJobsList.Remove(storedJobsList[index - 1]);
            File.Delete(path + filename + ".dat"); 
          }
          catch (Exception e) {
            Logger.Error("Job not on hdd but on list - deleting job from list ", e);
            storedJobsList.Remove(storedJobsList[index - 1]);
            StoreJobList();
          }
      
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
      Logger.Info("Restoring Joblist from Harddisk");
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
            
      XmlSerializer serializer = new XmlSerializer(typeof(List<JobStorageInfo>));
      FileStream stream = null;
      if(File.Exists(Path.Combine(path ,"list.xml"))) {
        try {
          stream = new FileStream(Path.Combine(path, "list.xml"), FileMode.Open);
          XmlTextReader reader = new XmlTextReader(stream);
          storedJobsList = (List<JobStorageInfo>)serializer.Deserialize(reader);
          Logger.Info("Loaded " + storedJobsList.Count + " Elements");
        }
        catch (Exception e) {
          Logger.Error("Exception while loading the Stored Job List", e);
        } finally {
          if(stream != null) 
            stream.Dispose();
        }
      } else {
        Logger.Info("no stored jobs on harddisk, starting new list");
        storedJobsList = new List<JobStorageInfo>();
      }
    }
    
  }
}
