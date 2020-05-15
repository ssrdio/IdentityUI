class GroupAttributes {
    constructor(groupId) {
        this.groupId = groupId;

        this.$attributeTable = $('#attribute-table');
        this.initAttributeTable();

        this.$addAttributeModal = $('#add-group-attribute-modal');
        this.$addAttributeModal.on('hidden.bs.modal', () => {
            this.hideModalErrors();

            this.modalKeyInput.value(null);
            this.modalValueInput.value(null);
        });
        this.$addAttributeModal.on('click', 'button.confirm', () => {
            this.add();
        });

        const $addAttributeForm = this.$addAttributeModal.find('#add-group-attribute-form');
        this.modalKeyInput = new InputComponent($addAttributeForm, '#attribute-key');
        this.modalValueInput = new InputComponent($addAttributeForm, '#attribute-value');

        this.addAttributeErrorAlert = new ErrorAlert($addAttributeForm);

        this.statusAlert = new StatusAlertComponent('#status-alert-container');
        this.confirmationModal = new conformationModal(
            $('#modal-container'),
            onYesClick => {
                if (onYesClick === null || onYesClick === undefined) {
                    return;
                }

                if (onYesClick.key === 'removeAttribute') {
                    this.remove(onYesClick.id);
                }
            });

        $('#add-attribute-button').on('click', () => {
            this.statusAlert.hide();
            this.$addAttributeModal.modal('show');
        })
    }

    initAttributeTable() {
        this.$attributeTable.DataTable({
            serverSide: true,
            processing: true,
            "targets": 'no-sort',
            "bSort": false,
            "order": [],
            ajax: {
                url: `/IdentityAdmin/Group/${this.groupId}/GroupAttribute/Get`,
                type: 'GET'
            },
            columns: [
                {
                    data: "key",
                    title: "Key",
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: null,
                    title: "Value",
                    className: "table-input",
                    mRender: (data) => {
                        let view = `<input class="form-control mr-1 attribute-{{id}}" value="{{value}}"/>`
                        let output = Mustache.render(view, { id: data.id, value: data.value });

                        return output;
                    }
                },
                {
                    data: null,
                    className: "attributes-center",

                    mRender: function (data) {
                        return `<div>
                                    <button class='btn btn-primary table-button edit' data-id='${data.id}'">Edit</button>
                                    <button class='btn btn-danger table-button remove' data-id='${data.id}'">Remove</button>
                                </div>`;
                    }
                }
            ],
        });

        this.$attributeTable.on('click', 'button.edit', (event) => {
            let id = $(event.target).data('id');
            this.editAttribute(id);
        });

        this.$attributeTable.on('click', 'button.remove', (event) => {
            let id = $(event.target).data('id');
            this.confirmationModal.show({ key: 'removeAttribute', id: id }, 'Are you sure that you want to remove Attribute?');
        });
    }

    reloadAttributeTable() {
        this.$attributeTable
            .DataTable()
            .clear()
            .draw();
    }

    showModalErrors(errors) {
        if (errors[''] !== null && errors[''] !== undefined) {
            this.addAttributeErrorAlert.showErrors(errors['']);
        }

        this.modalKeyInput.showError(errors.Key);
        this.modalValueInput.showError(errors.Value);
    }

    hideModalErrors() {
        this.addAttributeErrorAlert.hide();

        this.modalKeyInput.hideError();
        this.modalValueInput.hideError();
    }

    getAddData() {
        return {
            key: this.modalKeyInput.value(),
            value: this.modalValueInput.value()
        }
    }

    add() {
        this.hideModalErrors();
        let data = this.getAddData();

        Api.post(`/IdentityAdmin/Group/${this.groupId}/GroupAttribute/Add`, data)
            .done(() => {
                this.$addAttributeModal.modal('hide');
                this.statusAlert.showSuccess('Attribute was added');
                this.reloadAttributeTable();
            })
            .fail((resp) => {
                this.showModalErrors(resp.responseJSON);
            })
    }

    getEditData(id) {
        let value = this.$attributeTable.find(`input.attribute-${id}`).val();

        return {
            value: value,
        }
    }

    editAttribute(id) {
        this.statusAlert.hide();

        let data = this.getEditData(id);

        Api.post(`/IdentityAdmin/Group/${this.groupId}/GroupAttribute/Edit/${id}`, data)
            .done(() => {
                this.statusAlert.showSuccess('Attribute was updated');
                this.reloadAttributeTable();
            })
            .fail((resp) => {
                this.statusAlert.showErrors(resp.responseJSON);
            })
    }

    remove(id) {
        this.statusAlert.hide();

        Api.post(`/IdentityAdmin/Group/${this.groupId}/GroupAttribute/Remove/${id}`)
            .done(() => {
                this.statusAlert.showSuccess('Attribute was deleted');
                this.reloadAttributeTable();
            })
            .fail((resp) => {
                this.statusAlert.showErrors(resp.responseJSON);
            });
    }
}