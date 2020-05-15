class InviteIndex {
    constructor() {
        this.$inviteTable = $('#invite-table');
        this.initTable();

        this.statusAlert = new StatusAlertComponent('#status-alert-container');

        const inviteUserModal = new InviteUserModal(() => {
            this.reloadTable();
            this.statusAlert.showSuccess("User was invited");
        });

        this.confirmationModal = new conformationModal(
            $('#modal-container'),
            onYesClick => {
                if (onYesClick === null || onYesClick === undefined) {
                    return;
                }

                if (onYesClick.key === 'removeInvite') {
                    this.remove(onYesClick.id);
                }
            });

        $('#invite-user-button').on('click', () => {
            inviteUserModal.showModal();
        });
    }

    initTable() {
        this.$inviteTable.DataTable({
            serverSide: true,
            processing: true,
            "targets": 'no-sort',
            "bSort": false,
            "order": [],
            ajax: {
                url: `/IdentityAdmin/Invite/Get`,
                type: 'GET'
            },
            columns: [
                {
                    data: "email",
                    title: "Email",
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: "status",
                    title: "Status",
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: "role",
                    title: "Role",
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: "group",
                    title: "Group",
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: "groupRole",
                    title: "Group Role",
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: "expiresAt",
                    title: "Expires At",
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

        this.$inviteTable.on('click', 'button.remove', (event) => {
            let id = $(event.target).data("id");
            this.confirmationModal.show({ key: 'removeInvite', id: id }, 'Are you sure that you want to remove Invite?');
        });
    }

    reloadTable() {
        this.$inviteTable
            .DataTable()
            .clear()
            .draw();
    }

    remove(id) {
        this.statusAlert.hide();

        Api.post(`/IdentityAdmin/Invite/Remove/${id}`)
            .done(() => {
                this.reloadTable();
                this.statusAlert.showSuccess('Invite was removed');
            })
            .fail((resp) => {
                this.reloadTable();
                this.statusAlert.showErrors(resp.responseJSON['']);
            })
    }
}