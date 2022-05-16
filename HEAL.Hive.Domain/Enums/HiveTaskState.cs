namespace HEAL.Hive.Domain.Enums {
  public enum HiveTaskState {
    Offline,
    Waiting,
    Transferring,
    Calculating,
    Paused,
    Finished,
    Aborted,
    Failed
  };
}