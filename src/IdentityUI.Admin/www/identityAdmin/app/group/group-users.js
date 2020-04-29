class GroupUsers {
    constructor(groupId, userId, canManageRoles, hasRoleManagmentPermission, hasRemovePermission) {
        this.groupId = groupId;
        this.userId = userId;

        this.canManageRoles = JSON.parse(canManageRoles);
        this.hasRoleManagmentPermission = hasRoleManagmentPermission;
        this.hasRemovePermission = hasRemovePermission;

        const addExistingUserModal = new AddExistingUser(groupId, () => {
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
            });

        this.$usersTable = $('#user-table');
        this.initTable();

        this.statusAlert = new StatusAlertComponent('#status-alert-container');

        $('#invite-user-button').on('click', () => {
            console.log("invite user");
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
                    title: "Group Role",
                    mRender: (data) => {
                        if (this.hasRoleManagmentPermission) {
                            if (this.canManageRoles.some(x => x.id === data.groupRoleId)){

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
                                    groupRoles: this.canManageRoles.map((obj) => {
                                        obj.selected = obj.id === data.groupRoleId;

                                        return obj;
                                    })
                                }

                                var output = Mustache.render(view, templateData);

                                return output;
                            }
                            else {
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
                                    groupRoles: this.canManageRoles.map((obj) => {
                                           return obj;
                                    })
                                }

                                templateData.groupRoles.push({ id: data.groupRoleId, name: data.groupRoleName, selected: true });

                                var output = Mustache.render(view, templateData);

                                return output;
                            }
                        }
                        else {
                            var view = '<span>{{groupRole}}</span>';
                            var output = Mustache.render(view, { type: data.groupRole });

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
                            return `<div><button class="btn btn-danger table-button leave" data-id=${data.id}>Leave</button></div>`
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

        this.$usersTable.on('click', 'button.leave', (event) => {
            let id = $(event.target).data("id");
            this.confirmationModal.show({ key: 'removeUser', id: id }, 'Are you sure that you want to leave the Group?');
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
    constructor(groupId, successCallback) {
        this.groupId = groupId;
        this.successCallback = successCallback;

        this.$addExisingUserModal = $('#add-existing-user-to-group-modal');
        this.$addExisingUserModal.on('hidden.bs.modal', () => {
            this.userSelectComponent.selectOption(null);

            this.hideErrors();
        });
        this.$addExisingUserModal.on('click', 'button.confirm', () => {
            this.add();
        });

        const $form = this.$addExisingUserModal.find('#add-existing-user-to-group-from');
        this.$userSelect = $form.find('#existing-user-select .select2-container')
        this.userSelectComponent = new SelectComponent($form, '#existing-user-select');

        this.errorAlert = new ErrorAlert($form);

        this.initUserSelect();
    }

    initUserSelect() {
        this.$userSelect.select2({
            ajax: {
                url: `/IdentityAdmin/Group/${this.groupId}/GroupUser/GetAvailable`,
                type: 'GET',
                dataType: 'json',
                delay: 250
            }
        });
    }

    showModal() {
        this.$addExisingUserModal.modal('show');
    }

    showErrors(errors) {
        if (errors[''] !== null && errors[''] !== undefined) {
            this.errorAlert.showErrors(errors['']);
        }

        this.userSelectComponent.showError(errors.userId);
    }

    hideErrors() {
        this.errorAlert.hide();

        this.userSelectComponent.hideError();
    }

    getData() {
        return {
            userId: this.userSelectComponent.value()
        }
    }

    add() {
        this.hideErrors();
        const data = this.getData();

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