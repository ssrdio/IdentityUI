class RoleAssignment {
    constructor(roleId) {
        this.roleId = roleId;

        this.$addAssignmentModal = $('#add-assignment-modal');
        this.$addAssignmentModal.on('hidden.bs.modal', () => {
            this.assignRoleSelectComponent.selectOption(null);

            this.hideAddAssignmentErrors();
        });
        this.$addAssignmentModal.on('click', 'button.confirm', () => {
            this.add();
        });

        this.$assignmentTable = $('#assignment-table');
        this.initTable();

        const $addAssignmentForm = this.$addAssignmentModal.find('#add-assignment-form');
        this.$assignRoleSelect = $addAssignmentForm.find('#assign-role-select .select2-container');
        this.assignRoleSelectComponent = new SelectComponent($addAssignmentForm, '#assign-role-select');

        this.addAssignmentErrorAlert = new ErrorAlert($addAssignmentForm);

        this.initAssigneGroupRoleSelect();

        this.statusAlert = new StatusAlertComponent('#status-alert-container');

        this.confirmationModal = new conformationModal(
            $('#modal-container'),
            onYesClick => {
                if (onYesClick === null || onYesClick === undefined) {
                    return;
                }

                if (onYesClick.key === 'removeAssignment') {
                    this.remove(onYesClick.id);
                }
            });

        $('#add-assignment-button').on('click', () => {
            this.$addAssignmentModal.modal('show');
        });
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

        this.$assignmentTable.on('click', 'button.remove', (event) => {
            let id = $(event.target).data("id");
            this.confirmationModal.show({ key: 'removeAssignment', id: id }, 'Are you sure that you want to remove Role Assignment?');
        });
    }

    reloadTable() {
        this.$assignmentTable
            .DataTable()
            .clear()
            .draw();
    }

    initAssigneGroupRoleSelect() {
        this.$assignRoleSelect.select2({
            ajax: {
                url: `/IdentityAdmin/Role/${this.roleId}/Assignment/GetUnassigned`,
                type: 'GET',
                dataType: 'json',
                delay: 250
            }
        });
    }

    remove(id) {
        this.statusAlert.hide();

        Api.post(`/IdentityAdmin/Role/${this.roleId}/Assignment/Remove/${id}`)
            .done(() => {
                this.reloadTable();
                this.statusAlert.showSuccess(`Role Assignment was removed`);
            })
            .fail((resp) => {
                this.statusAlert.showErrors(resp.responseJSON);
            });
    }

    showAddAssignmentErrors(errors) {
        if (errors[''] !== null && errors[''] !== undefined) {
            this.addAssignmentErrorAlert.showErrors(errors['']);
        }

        this.assignRoleSelectComponent.showError(errors.roleId);
    }

    hideAddAssignmentErrors() {
        this.addAssignmentErrorAlert.hide();

        this.assignRoleSelectComponent.hideError();
    }

    getData() {
        return {
            roleId: this.assignRoleSelectComponent.value()
        };
    }

    add() {
        this.hideAddAssignmentErrors();
        this.statusAlert.hide();

        const data = this.getData()

        Api.post(`/IdentityAdmin/Role/${this.roleId}/Assignment/Add`, data)
            .done(() => {
                this.$addAssignmentModal.modal('hide');
                this.reloadTable();

                this.statusAlert.showSuccess('Role Assignment was added');
            })
            .fail((resp) => {
                this.showAddAssignmentErrors(resp.responseJSON);
            });
    }
}