class UserRoles {
    constructor(userId) {
        this.userId = userId;
        this.dragAndDropComponent = new DragAndDropComponent(
            (itemId) => { this.removeRoles(itemId) },
            (itemId) => { this.addRoles(itemId) },
            "Available Roles",
            "Assigned Roles"
        );
        this.getRoles(this.userId);
  }

  getRoles(userId) {
    const url = `/IdentityAdmin/User/GetRoles/${userId}`;

    Api.get(url)
      .done(resp => {
        this.dragAndDropComponent.initAvailableItems(resp.availableRoles);
          this.dragAndDropComponent.initAssignedItems(resp.assignedRoles);
          this.dragAndDropComponent.hideLoader();
      })
      .fail(resp => {
        this.dragAndDropComponent.showError('Error getting user roles');
        console.log('Error getting user roles');
      });
  }

  addRoles(itemId) {
      const data = { Roles: [itemId] };
      const url = `/IdentityAdmin/User/AddRoles/${this.userId}`;

      this.dragAndDropComponent.hideErrors();
    if (data.Roles.length > 0) {
      Api.post(url, data)
        .done(() => {
        })
          .fail(resp => {
              this.dragAndDropComponent.showError('Failed to add roles');
        });
    } else {
        this.dragAndDropComponent.showError('No role selected');
    }
  }

  removeRoles(itemId) {
    const data = { Roles: [itemId] };
    const url = `/IdentityAdmin/User/RemoveRoles/${this.userId}`;


      this.dragAndDropComponent.hideErrors();
    if (data.Roles.length > 0) {
      Api.post(url, data)
        .done(() => {
        })
        .fail(() => {
            this.dragAndDropComponent.showError('Failed to remove roles');
        });
    } else {
        this.dragAndDropComponent.showError('No role selected');
    }
  }
}
