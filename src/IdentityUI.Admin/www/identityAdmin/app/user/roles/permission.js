class RolePermission {
    constructor(roleId) {
        this.roleId = roleId;

        this.$addPermissionModal = $('#add-permission-modal');
        this.$addPermissionModal.on('hidden.bs.modal', () => {
            this.permissionSelectComponent.selectOption(null);

            this.hideAddPermissionErrors();
        });
        this.$addPermissionModal.on('click', 'button.confirm', () => {
            this.add();
        });

        this.$permissionTable = $('#permission-table');
        this.initTable();

        const $addPermissionForm = this.$addPermissionModal.find('#add-permission-from');

        this.$permissionSelect = $addPermissionForm.find('#add-permission-select .select2-container');
        this.permissionSelectComponent = new SelectComponent($addPermissionForm, ('#add-permission-select'));
        this.initPermissionSelect();

        this.addPermissionErrorAlert = new ErrorAlert($addPermissionForm);

        this.statusAlert = new StatusAlertComponent('#status-alert-container');

        this.confirmationModal = new conformationModal(
            $('#modal-container'),
            onYesClick => {
                if (onYesClick === null || onYesClick === undefined) {
                    return;
                }

                if (onYesClick.key === 'removePermission') {
                    this.remove(onYesClick.id);
                }
            });

        $('#add-permission-button').on('click', () => {
            this.$addPermissionModal.modal('show');
        });
    }

    initTable(){
        this.$permissionTable.DataTable({
            serverSide: true,
            processing: true,
            "targets": 'no-sort',
            "bSort": false,
            "order": [],
            ajax: {
                url: `/IdentityAdmin/Role/${this.roleId}/Permission/Get`,
                type: 'GET'
            },
            columns: [
                {
                    data: "name",
                    title: "Name",
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: null,
                    className: "dt-head-center",
                    width: "160px",
                    render: function (data) {
                        return `
                            <div >
                                <button class="btn btn-danger table-button remove" data-id="${data.id}">Remove</button>
                            </div>`
                    }
                }
            ],
        });

        this.$permissionTable.on('click', 'button.remove', (event) => {
            let id = $(event.target).data("id");
            this.confirmationModal.show({ key: 'removePermission', id: id }, 'Are you sure that you want to remove Permission from Role?');
        });
    }

    reloadTable() {
        this.$permissionTable
            .DataTable()
            .clear()
            .draw();
    }

    initPermissionSelect() {
        this.$permissionSelect.select2({
            ajax: {
                url: `/IdentityAdmin/Role/${this.roleId}/Permission/GetAvailable`,
                type: 'GET',
                dataType: 'json',
                delay: 250
            }
        });
    }

    remove(id) {
        this.statusAlert.hide();

        Api.post(`/IdentityAdmin/Role/${this.roleId}/Permission/Remove/${id}`)
            .done(() => {
                this.reloadTable();
                this.statusAlert.showSuccess(`Permission was removed from role`);
            })
            .fail((resp) => {
                this.statusAlert.showErrors(resp.responseJSON['']);
            });
    }

    showAddpermissionErrors(errors) {
        if (errors[''] !== null && errors[''] !== undefined) {
            this.addPermissionErrorAlert.showErrors(errors['']);
        }

        this.permissionSelectComponent.showError(errors.permissionId);
    }

    hideAddPermissionErrors() {
        this.addPermissionErrorAlert.hide();

        this.permissionSelectComponent.hideError();
    }

    getData() {
        return {
            permissionId: this.permissionSelectComponent.value()
        };
    }

    add() {
        this.hideAddPermissionErrors();
        this.statusAlert.hide();

        const data = this.getData()

        Api.post(`/IdentityAdmin/Role/${this.roleId}/Permission/Add`, data)
            .done(() => {
                this.$addPermissionModal.modal('hide');
                this.reloadTable();

                this.statusAlert.showSuccess('Permission was added to role');
            })
            .fail((resp) => {
                this.showAddpermissionErrors(resp.responseJSON);
            });
    }
}