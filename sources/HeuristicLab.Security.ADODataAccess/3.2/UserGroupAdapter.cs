using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeuristicLab.DataAccess.ADOHelper;
using HeuristicLab.Security.Contracts.BusinessObjects;
using HeuristicLab.Security.DataAccess;
using HeuristicLab.Security.ADODataAccess.TableAdapterWrapper;

namespace HeuristicLab.Security.ADODataAccess {
  class UserGroupAdapter: DataAdapterBase<
      dsSecurityTableAdapters.UserGroupTableAdapter, 
      UserGroup, 
      dsSecurity.UserGroupRow>,
      IUserGroupAdapter {
    public UserGroupAdapter() :
      base(new UserGroupAdapterWrapper()) {
    }

    private ManyToManyRelationHelper<
      dsSecurityTableAdapters.PermissionOwner_UserGroupTableAdapter,
      dsSecurity.PermissionOwner_UserGroupRow> manyToManyRelationHelper = null;

    private ManyToManyRelationHelper<dsSecurityTableAdapters.PermissionOwner_UserGroupTableAdapter,
      dsSecurity.PermissionOwner_UserGroupRow> ManyToManyRelationHelper {
      get {
        if (manyToManyRelationHelper == null) {
          manyToManyRelationHelper =
            new ManyToManyRelationHelper<dsSecurityTableAdapters.PermissionOwner_UserGroupTableAdapter,
              dsSecurity.PermissionOwner_UserGroupRow>(new PermissionOwner_UserGroupAdapterWrapper(), 0);
        }

        manyToManyRelationHelper.Session = Session as Session;

        return manyToManyRelationHelper;
      }
    }

    private IPermissionOwnerAdapter permOwnerAdapter = null;

    private IPermissionOwnerAdapter PermOwnerAdapter {
      get {
        if (permOwnerAdapter == null)
          permOwnerAdapter =
            this.Session.GetDataAdapter<PermissionOwner, IPermissionOwnerAdapter>();

        return permOwnerAdapter;
      }
    }

    protected override dsSecurity.UserGroupRow ConvertObj(UserGroup group, 
      dsSecurity.UserGroupRow row) {
      if (group != null && row != null) {
        row.PermissionOwnerId = group.Id;

        return row;
      } else {
        return null;
      }
    }

    protected override UserGroup ConvertRow(dsSecurity.UserGroupRow row,
      UserGroup group) {
      if (group != null && row != null) {
        group.Id = row.PermissionOwnerId;
        PermOwnerAdapter.GetById(group);

        ICollection<Guid> permissionOwners =
          ManyToManyRelationHelper.GetRelationships(group.Id);

        group.Members.Clear();
        foreach (Guid permissionOwner in permissionOwners) {
          PermissionOwner permOwner =
            PermOwnerAdapter.GetByIdPolymorphic(permissionOwner);

          group.Members.Add(permOwner);
        }

        return group;
      } else {
        return null;
      }
    }

    #region IUserGroupAdapter Members

    protected override void doUpdate(UserGroup group) {
      if (group != null) {
        PermOwnerAdapter.Update(group);

        base.doUpdate(group);

        List<Guid> relationships =
          new List<Guid>();
        foreach (PermissionOwner permOwner in group.Members) {
          PermOwnerAdapter.UpdatePolymorphic(permOwner);

          relationships.Add(permOwner.Id);
        }

        ManyToManyRelationHelper.UpdateRelationships(group.Id,
          relationships, 0);
      }
    }

    protected override bool doDelete(UserGroup group) {
      if (group != null) {
        //delete all relationships
        ManyToManyRelationHelper.UpdateRelationships(group.Id,
          new List<Guid>(), 0);

        return base.doDelete(group) &&
          PermOwnerAdapter.Delete(group);
      }

      return false;
    }

    public UserGroup GetByName(string name) {
      UserGroup group = new UserGroup();
      PermissionOwner permOwner =
        PermOwnerAdapter.GetByName(name);

      if (permOwner != null)
        return GetById(permOwner.Id);
      else
        return null;
    }

    public ICollection<UserGroup> MemberOf(Guid permissionOwnerId) {
      return base.FindMultiple(
        delegate() {
          return Adapter.GetDataByMemberOf(permissionOwnerId);
        }
      );
    }

    #endregion
  }
}
