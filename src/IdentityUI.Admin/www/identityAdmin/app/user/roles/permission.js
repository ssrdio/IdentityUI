class RolePermission {
    constructor(roleId) {
        this.roleId = roleId;

        this.$permissionTable = $('#permission-table');
        this.initTable();

        this.statusAlert = new StatusAlertComponent('#status-alert-container');
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
                    data: null,
                    className: "checkbox-holder",
                    mRender: (data) => {
                        if (data.inRole) {
                            return `<input type="checkbox" class="form-control in-role styled-checkbox" data-id="${data.id}" id="${data.id}" checked/><label for="${data.id}"></label>`
                        }
                        else {
                            return `<input type="checkbox" class="form-control in-role styled-checkbox" id="${data.id}" data-id="${data.id}"/><label for="${data.id}"></label>`
                        }
                    }
                },
                {
                    data: "name",
                    title: "Name",
                    render: $.fn.dataTable.render.text()
                },
            ],
        });

        this.$permissionTable.on('change', 'input.in-role', (event) => {
            let add = event.currentTarget.checked;

            let id = $(event.target).data('id');

            if (add === true) {
                this.add(id);
            }
            else if (add === false) {
                this.remove(id);
            }
            else {
                console.log('error');
            }
        });
    }

    reloadTable() {
        this.$permissionTable
            .DataTable()
            .clear()
            .draw();
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

    getData(id) {
        return {
            permissionId: id
        };
    }

    add(id) {
        this.statusAlert.hide();
        const data = this.getData(id)

        Api.post(`/IdentityAdmin/Role/${this.roleId}/Permission/Add`, data)
            .done(() => {
                this.reloadTable();

                this.statusAlert.showSuccess('Permission was added to role');
            })
            .fail((resp) => {
                this.statusAlert.showError(resp.responseJSON['']);
            });
    }
}