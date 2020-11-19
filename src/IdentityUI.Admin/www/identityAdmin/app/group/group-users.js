class GroupUsers {
    constructor(groupId, userId, canManageRoles, canAssigneRoles, hasRoleManagmentPermission, hasRemovePermission, canChangeOwnRole) {
        this.groupId = groupId;
        this.userId = userId;

        this.canManageRoles = JSON.parse(canManageRoles);
        this.canAssigneRoles = JSON.parse(canAssigneRoles);
        this.hasRoleManagmentPermission = hasRoleManagmentPermission;
        this.hasRemovePermission = hasRemovePermission;
        this.canChangeOwnRole = canChangeOwnRole;

        const addExistingUserModal = new AddExistingUser(groupId, this.canAssigneRoles, () => {
            this.reloadTable();

            this.statusAlert.showSuccess('Added existing user to group');
        });

        $('#add-existing-user-button').on('click', () => {
            addExistingUserModal.showModal();
        });

        this.confirmationModal = new conformationModal(
            $('#modal-container'),
            onYesClick => {
                if (onYesClick === null || onYesClick === undefined) {
                    return;
                }

                if (onYesClick.key === 'removeUser') {
                    this.remove(onYesClick.id);
                }
                else if (onYesClick.key === 'leave') {
                    this.leave();
                }
            });

        this.$usersTable = $('#user-table');
        this.initTable();

        this.statusAlert = new StatusAlertComponent('#status-alert-container');

        const inviteUserModal = new InviteToGroupModel(this.groupId, this.canAssigneRoles, () => {
            this.statusAlert.showSuccess("User was invited");
        });

        $('#invite-user-button').on('click', () => {
            inviteUserModal.showModal();
        });
    }

    initTable() {
        this.$usersTable.DataTable({
            serverSide: true,
            processing: true,
            "targets": 'no-sort',
            "bSort": false,
            "order": [],
            ajax: {
                url: `/IdentityAdmin/Group/${this.groupId}/GroupUser/Get`,
                type: 'GET'
            },
            columns: [
                {
                    data: "username",
                    title: "Username",
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: null,
                    className: "table-input",
                    title: "Group Role",
                    mRender: (data) => {
                        if (this.hasRoleManagmentPermission) {
                            if (!this.canManageRoles.some(x => x.id === data.groupRoleId) || (this.userId === data.userId && this.canChangeOwnRole === false))
                            {
                                let view = `<select class="form-control" data-id={{groupUserId}} disabled>
                                            {{#groupRoles}}
                                                {{#selected}}
                                                    <option value='{{id}}' selected>{{name}}</option>
                                                {{/selected}}
                                                {{^selected}}
                                                    <option value='{{id}}'>{{name}}</option>
                                                {{/selected}}
                                            {{/groupRoles}}
                                        </select>`;

                                let templateData = {
                                    groupUserId: data.id,
                                    groupRoles: this.canAssigneRoles.map((obj) => {
                                        obj.selected = obj.id === data.groupRoleId;

                                        return obj;
                                    })
                                }

                                if (!templateData.groupRoles.some(x => x.id === data.groupRoleId)) {
                                    templateData.groupRoles.push({ id: data.groupRoleId, name: data.groupRoleName, selected: true });
                                }

                                var output = Mustache.render(view, templateData);

                                return output;
                            }
                            else {

                                let view = `<select class="form-control" data-id={{groupUserId}}>
                                            {{#groupRoles}}
                                                {{#selected}}
                                                    <option value='{{id}}' selected>{{name}}</option>
                                                {{/selected}}
                                                {{^selected}}
                                                    <option value='{{id}}'>{{name}}</option>
                                                {{/selected}}
                                            {{/groupRoles}}
                                        </select>`;

                                let templateData = {
                                    groupUserId: data.id,
                                    groupRoles: this.canAssigneRoles.map((obj) => {
                                        obj.selected = obj.id === data.groupRoleId;

                                        return obj;
                                    })
                                }

                                if (!templateData.groupRoles.some(x => x.id === data.groupRoleId)) {
                                    templateData.groupRoles.push({ id: data.groupRoleId, name: data.groupRoleName, selected: true });
                                }

                                var output = Mustache.render(view, templateData);

                                return output;
                            }
                        }
                        else {
                            var view = '<span>{{groupRole}}</span>';
                            var output = Mustache.render(view, { groupRole: data.groupRoleName });

                            return output
                        }
                    }
                },
                {
                    data: null,
                    className: "dt-head-center",
                    width: "160px",
                    mRender: (data) => {
                        if (data.userId === this.userId) {
                            return `<div><button class="btn btn-danger table-button leave">Leave</button></div>`
                        }

                        if (this.hasRemovePermission && this.canManageRoles.some(x => x.id === data.groupRoleId)) {
                            return `
                                <div>
                                    <button class="btn btn-danger table-button remove" data-id=${data.id}>Remove</button>
                                </div>
                            `;
                        }

                        return ``;
                    }
                }
            ],
        });

        this.$usersTable.on('click', 'button.remove', (event) => {
            let id = $(event.target).data("id");
            this.confirmationModal.show({ key: 'removeUser', id: id }, 'Are you sure that you want to remove User from Group?');
        });

        this.$usersTable.on('click', 'button.leave', () => {
            this.confirmationModal.show({ key: 'leave' }, 'Are you sure that you want to leave the Group?');
        });

        this.$usersTable.on('change', 'select', (event) => {
            let $select = $(event.target);

            let id = $select.data('id');
            let value = $select.val();

            this.changeRole(id, value);
        });
    }

    reloadTable() {
        this.$usersTable
            .DataTable()
            .clear()
            .draw();
    }

    remove(id) {
        this.statusAlert.hide();

        Api.post(`/IdentityAdmin/Group/${this.groupId}/GroupUser/Remove/${id}`)
            .done(() => {
                this.reloadTable();
                this.statusAlert.showSuccess('User was removed from group');
            })
            .fail((resp) => {
                this.reloadTable();
                this.statusAlert.showErrors(resp.responseJSON['']);
            })
    }

    leave() {
        this.statusAlert.hide();

        Api.post(`/IdentityAdmin/Group/${this.groupId}/GroupUser/Leave`)
            .done(() => {
                window.location.href = '/';
            })
            .fail((resp) => {
                this.statusAlert.showErrors(resp.responseJSON['']);
            })
    }

    changeRole(id, roleId) {
        this.statusAlert.hide();

        Api.post(`/IdentityAdmin/Group/${this.groupId}/GroupUser/ChangeRole/${id}?roleId=${roleId}`)
            .done(() => {
                this.reloadTable();
                this.statusAlert.showSuccess('User role was changed');
            })
            .fail((resp) => {
                this.reloadTable();
                this.statusAlert.showErrors(resp.responseJSON['']);
            })
    }
}

class AddExistingUser {
    constructor(groupId, canAssigneRoles, successCallback) {
        this.groupId = groupId;
        this.canAssigneRoles = canAssigneRoles;
        this.successCallback = successCallback;

        this.$addExisingUserModal = $('#add-existing-user-to-group-modal');
        this.$addExisingUserModal.on('hidden.bs.modal', () => {
            this.userSelectComponent.selectOption();
            this.userSelectComponent.triggerChange();
            this.rolesSelectComponent.selectOption();
            this.rolesSelectComponent.triggerChange();

            this.hideErrors();
        });
        this.$addExisingUserModal.on('click', 'button.confirm', () => {
            this.add();
        });

        const $form = this.$addExisingUserModal.find('#add-existing-user-to-group-from');
        this.$userSelect = $form.find('#existing-user-select .select2-container')
        this.userSelectComponent = new SelectComponent($form, '#existing-user-select');

        this.$rolesSelect = $form.find('#group-role-select .select2-container')
        this.rolesSelectComponent = new SelectComponent($form, '#group-role-select');

        this.errorAlert = new ErrorAlert($form);

        this.initUserSelect();
        this.initRolesSelct();
    }

    initUserSelect() {
        this.$userSelect.select2({
            ajax: {
                url: `/IdentityAdmin/Group/${this.groupId}/GroupUser/GetAvailable`,
                type: 'GET',
                dataType: 'json',
                delay: 250
            },
            placeholder: 'Select User'
        });
    }

    initRolesSelct() {
        let data = this.canAssigneRoles.map((element) => {
            return {
                id: element.id,
                text: element.name
            }
        });

        this.$rolesSelect.select2({
            data: data,
            placeholder: 'Select Group Role'
        });

        this.rolesSelectComponent.selectOption(null);
    }

    showModal() {
        this.$addExisingUserModal.modal('show');
    }

    showErrors(errors) {
        if (errors[''] !== null && errors[''] !== undefined) {
            this.errorAlert.showErrors(errors['']);
        }

        this.userSelectComponent.showError(errors.UserId);
        this.rolesSelectComponent.showError(errors.GroupRoleId);
    }

    hideErrors() {
        this.errorAlert.hide();

        this.userSelectComponent.hideError();
        this.rolesSelectComponent.hideError();
    }

    getData() {
        return {
            userId: this.userSelectComponent.value(),
            groupRoleId: this.rolesSelectComponent.value(),
        }
    }

    add() {
        this.hideErrors();
        const data = this.getData();

        console.log(data);

        Api.post(`/IdentityAdmin/Group/${this.groupId}/GroupUser/AddExisting`, data)
            .done(() => {
                this.$addExisingUserModal.modal('hide');

                if (this.successCallback) {
                    this.successCallback();
                }
            })
            .fail((resp) => {
                this.showErrors(resp.responseJSON);
            })
    }
}