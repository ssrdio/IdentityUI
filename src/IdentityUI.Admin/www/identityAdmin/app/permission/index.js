class PermissionIndex {
    constructor() {

        this.$addPermissionModal = $('#add-permission-modal');
        this.$addPermissionModal.on('hidden.bs.modal', () => {
            this.hideModalErrors();

            this.clearModalForm();
        });
        this.$addPermissionModal.on('click', 'button.confirm', () => {
            this.add();
        });

        const $addPermissionForm = this.$addPermissionModal.find('#add-permission-form');
        this.nameInput = new InputComponent($addPermissionForm, '#name-input');

        this.modalErrorAlert = new ErrorAlert($addPermissionForm);

        this.statusAlert = new StatusAlertComponent('#status-alert');

        this.$permissionTable = $('#permission-table');
        this.initTable();

        $('#add-group-button').on('click', () => {
            this.$addPermissionModal.modal('show');
        });
    }

    initTable() {
        this.$permissionTable.DataTable({
            serverSide: true,
            processing: true,
            "targets": 'no-sort',
            "bSort": false,
            "order": [],
            ajax: {
                url: `/IdentityAdmin/Permission/Get`,
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
                    mRender: function (data) {
                        return `
                                <div>
                                    <a class="btn btn-primary table-button" href="/IdentityAdmin/Permission/Details/${data.id}">Details</a>
                                </div>
                            `;
                    }
                }
            ],
        });
    }

    showModalErrors(errors) {
        if (errors[''] !== null && errors[''] != undefined) {
            this.modalErrorAlert.showErrors(errors['']);
        }

        this.nameInput.showError(errors.Name);
    }

    hideModalErrors() {
        this.modalErrorAlert.hide();

        this.nameInput.hideError();
    }

    clearModalForm() {
        this.nameInput.value(null);
    }

    reloadTable() {
        this.$permissionTable
            .DataTable()
            .clear()
            .draw();
    }

    getData() {
        return {
            name: this.nameInput.value(),
        }
    }

    add() {
        const data = this.getData();

        Api.post(`/IdentityAdmin/Permission/Add`, data)
            .done(() => {
                this.$addPermissionModal.modal('hide');

                this.reloadTable();
                this.statusAlert.showSuccess('Permission was added');
            })
            .fail((resp) => {
                this.showModalErrors(resp.responseJSON);
            });
    }
}