class RoleAssignment {
    constructor(roleId) {
        this.roleId = roleId;

        this.$assignmentTable = $('#assignment-table');
        this.initTable();

        this.statusAlert = new StatusAlertComponent('#status-alert-container');
    }

    initTable() {
        this.$assignmentTable.DataTable({
            serverSide: true,
            processing: true,
            "targets": 'no-sort',
            "bSort": false,
            "order": [],
            ajax: {
                url: `/IdentityAdmin/Role/${this.roleId}/Assignment/Get`,
                type: 'GET'
            },
            columns: [
                {
                    data: null,
                    className: "checkbox-holder",
                    mRender: (data) => {
                        if (data.isAssigned) {
                            return `<input type="checkbox" class="form-control in-assignment styled-checkbox" data-id="${data.id}" id="${data.id}" checked/><label for="${data.id}"></label>`
                        }
                        else {
                            return `<input type="checkbox" class="form-control in-assignment styled-checkbox" id="${data.id}" data-id="${data.id}"/><label for="${data.id}"></label>`
                        }
                    }
                },
                {
                    data: "name",
                    title: "Name",
                    render: $.fn.dataTable.render.text()
                }
            ],
        });


        this.$assignmentTable.on('change', 'input.in-assignment', (event) => {
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
        this.$assignmentTable
            .DataTable()
            .clear()
            .draw();
    }

    remove(id) {
        this.statusAlert.hide();

        Api.post(`/IdentityAdmin/Role/${this.roleId}/Assignment/Remove/${id}`)
            .done(() => {
                this.reloadTable();
                this.statusAlert.showSuccess(`Role Assignment was removed`);
            })
            .fail((resp) => {
                this.statusAlert.showErrors(resp.responseJSON['']);
            });
    }

    getData(id) {
        return {
            roleId: id
        };
    }

    add(id) {
        this.statusAlert.hide();

        const data = this.getData(id)

        Api.post(`/IdentityAdmin/Role/${this.roleId}/Assignment/Add`, data)
            .done(() => {
                this.reloadTable();

                this.statusAlert.showSuccess('Role Assignment was added');
            })
            .fail((resp) => {
                this.statusAlert.showErrors(resp.responseJSON['']);
            });
    }
}