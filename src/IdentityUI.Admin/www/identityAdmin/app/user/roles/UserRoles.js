class UserRoles {
  constructor(userId) {
    this.$availableRolesSelect = $('#availableRolesForm select');
    this.$assignedRolesSelect = $('#assignedRolesForm select');

    this.statusAlert = new StatusAlertComponent('#statusAlert');

    this.getRoles(userId);

    $('#availableRolesForm button.submit').click(() => {
      this.addRoles(userId);
    });

    $('#assignedRolesForm button.submit').click(() => {
      this.removeRoles(userId);
    });
  }

  getRoles(userId) {
    const url = `/IdentityAdmin/User/GetRoles/${userId}`;

    Api.get(url)
      .done(resp => {
        this.setAssignedRoles(resp.assignedRoles);
        this.setAvailableRoles(resp.availableRoles);
      })
      .fail(resp => {
        console.log('Error getting user roles');
      });
  }

  setAvailableRoles(availableRoles) {
    this.$availableRolesSelect.empty();

    availableRoles.forEach(element => {
      this.$availableRolesSelect.append(
        $('<option />')
          .val(element.name)
          .text(element.name)
      );
    });
  }

  setAssignedRoles(assignedRoles) {
    this.$assignedRolesSelect.empty();

    assignedRoles.forEach(element => {
      this.$assignedRolesSelect.append(
        $('<option />')
          .val(element.name)
          .text(element.name)
      );
    });
  }

  addRoles(userId) {
    const data = { Roles: this.$availableRolesSelect.val() };
    const url = `/IdentityAdmin/User/AddRoles/${userId}`;

    this.statusAlert.hide();

    if (data.Roles.length > 0) {
      Api.post(url, data)
        .done(() => {
          this.statusAlert.showSuccess('Roles added');

          this.getRoles(userId);
        })
        .fail(resp => {
          this.statusAlert.showError('Faild to add roles');
        });
    } else {
      this.statusAlert.showError('No role selected');
    }
  }

  removeRoles(userId) {
    const data = { Roles: this.$assignedRolesSelect.val() };
    const url = `/IdentityAdmin/User/RemoveRoles/${userId}`;

    this.statusAlert.hide();
    if (data.Roles.length > 0) {
      Api.post(url, data)
        .done(() => {
          this.statusAlert.showSuccess('Roles removed');

          this.getRoles(userId);
        })
        .fail(() => {
          this.statusAlert.showError('Faild to remove roles');
        });
    } else {
      this.statusAlert.showError('No role selected');
    }
  }
}
