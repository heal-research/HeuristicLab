using System;
namespace HEAL.Hive.Domain.DTOs.Entities {
  public class HiveUserDTO {
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
  }
}
