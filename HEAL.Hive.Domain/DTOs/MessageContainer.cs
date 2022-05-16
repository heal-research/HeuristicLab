using System;

namespace HEAL.Hive.Domain.DTOs {
  public class MessageContainer {

    public MessageContainer() {}

    public MessageContainer(MessageType message) {
      Message = message;
      TaskId = Guid.Empty;
    }

    public MessageContainer(MessageType message, Guid taskId) {
      Message = message;
      TaskId = taskId;
    }

    public enum MessageType {
      // *** commands from hive server ***
      CalculateTask, // drone should calculate a task. the task is already assigned to the drone
      StopTask,   // drone should stop the task and submit results
      StopAll,   // stop all and submit results
      AbortTask,  // drone should shut the task down immediately without submitting results
      AbortAll,  // drone should abort all task immediately
      PauseTask,  // pause the task and submit the results   
      PauseAll,  // pause all task and submit results
      Restart,   // restart operation after Sleep
      Sleep,     // disconnect from server, but don't shutdown
      ShutdownDrone,  // drone should shutdown immediately without submitting results
      SayHello,  // drone should say hello, because job is unknown to the server
      NewHBInterval, // change the polling to a new interval
      ShutdownComputer, // shutdown the computer the drone runs on
    };

    public MessageType Message { get; set; }
    public Guid TaskId { get; set; }

  }
}
