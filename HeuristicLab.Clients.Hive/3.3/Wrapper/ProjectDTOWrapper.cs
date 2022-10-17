using System;
using System.Collections.Generic;
using HEAL.Hive.SwaggerClient;

namespace HeuristicLab.Clients.Hive.Wrapper {
  public class ProjectDTOWrapper : Project {

    private ProjectDTO ProjectDTO { get; set; }

    public IEnumerable<Guid> AssignedComputingResources { get; set; }

    public ProjectDTOWrapper(ProjectDTO projectDTO) {
      this.ProjectDTO = projectDTO;
      this.Id = projectDTO.Id;
      this.Name = projectDTO.Name;
      this.Description = projectDTO.Description;
      this.ParentProjectId = projectDTO.ParentProjectId;
      this.AssignedComputingResources = projectDTO.AssignedComputingResources;
    }
  }
}
