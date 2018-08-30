using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Services.Hive.DataTransfer {
  [Serializable]
  public enum JobState {
    /// <summary>
    /// A job is online as long as he is no other state.
    /// </summary>
    Online,

    /// <summary>
    /// A job is in StatisticsPending if its deletion has been requested,
    /// but the final generation of statistics hasn't been performed yet.
    /// </summary>
    StatisticsPending,

    /// <summary>
    /// A job is in DeletionPending if its deletion has been requested,
    /// the final generation of statistics has already been performed,
    /// but the eventual deletion of the job is still pending. 
    /// </summary>
    DeletionPending
  };

  public static class JobStateExtensions {
    /// <summary>
    /// This job is online
    /// </summary>
    public static bool IsOnline(this JobState jobState) {
      return jobState == JobState.Online;
    }

    /// <summary>
    /// This job is still existent, but already flagged for deletion.
    /// Usually the flag is set by a user, not a resource or the janitor or something else.
    /// </summary>
    public static bool IsFlaggedForDeletion(this JobState jobState) {
      return jobState == JobState.StatisticsPending || jobState == JobState.DeletionPending;
    }
  }
}
