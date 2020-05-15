class GroupIndex {
    constructor() {
        this.$addGroupModal = $('#add-group-modal');
        this.$addGroupModal.on('hidden.bs.modal', () => {
            this.hideModalErrors();

            this.clearModalForm();
        });
        this.$addGroupModal.on('click', 'button.confirm', () => {
            this.addGroup();
        });

        const $addGroupForm = this.$addGroupModal.find('#add-group-form');
        this.nameInput = new InputComponent($addGroupForm, '#name-input');

        this.modalErrorAlert = new ErrorAlert($addGroupForm);

        this.statusAlert = new StatusAlertComponent('#status-alert');

        this.$groupTable = $('#group-table');
        this.initTable();

        this.confirmationModal = new conformationModal(
            $('#modal-container'),
            onYesClick => {
                if (onYesClick === null || onYesClick === undefined) {
                    return;
                }

                if (onYesClick.key === 'removeGroup') {
                    this.remove(onYesClick.id);
                }
            });

        $('#add-group-button').on('click', () => {
            this.$addGroupModal.modal('show');
        });
    }

    initTable() {
        this.$groupTable.DataTable({
            serverSide: true,
            processing: true,
            "targets": 'no-sort',
            "bSort": false,
            "order": [],
            ajax: {
                url: `/IdentityAdmin/Group/Get`,
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
                    width: "250px",
                    mRender: function (data) {
                        return `
                                <div>
                                    <a class="btn btn-primary table-button" href="/IdentityAdmin/Group/Users/${data.id}">Details</a>
                                    <button class="btn btn-danger table-button remove" data-id="${data.id}">Remove</button>
                                </div>
                            `;
                    }
                }
            ],
        });

        this.$groupTable.on('click', 'button.remove', (event) => {
            let id = $(event.target).data("id");
            this.confirmationModal.show({ key: 'removeGroup', id: id }, 'Are you sure that you want to remove Group?');
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
        this.$groupTable
            .DataTable()
            .clear()
            .draw();
    }

    getData() {
        return {
            name: this.nameInput.value(),
        }
    }

    addGroup() {
        const data = this.getData();

        Api.post(`/IdentityAdmin/Group/Add`, data)
            .done(() => {
                this.$addGroupModal.modal('hide');

                this.reloadTable();
                this.statusAlert.showSuccess('Group was added');
            })
            .fail((resp) => {
                this.showModalErrors(resp.responseJSON);
            });
    }

    remove(id) {
        this.statusAlert.hide();

        Api.post(`/IdentityAdmin/Group/Remove/${id}`)
            .done(() => {
                this.reloadTable();
                this.statusAlert.showSuccess('Group was removed');
            })
            .fail((resp) => {
                this.reloadTable();
                this.statusAlert.showErrors(resp.responseJSON['']);
            })
    }
}