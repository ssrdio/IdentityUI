class AccountSessions{
    constructor() {
        this.statusAlert = new StatusAlertComponent('#status-alert-container');
        this.sessionTable = new AccountSessionTable(this.statusAlert);
    }
}

class AccountSessionTable {
    constructor(statusAlert) {
        this.statusAlert = statusAlert;
        this.$sessionTable = $('#accout-session-table');
        this.init();

        this.confirmationModal = new conformationModal(
            $(`#modalContainer`),
            onYesClick => {
                if (onYesClick === null || onYesClick === undefined) {
                    return;
                }

                if (onYesClick.key === 'removeSession') {
                    this.remove(onYesClick.id);
                }
            }
        );
    }

    init() {
        this.$sessionTable.DataTable({
            serverSide: true,
            processing: true,
            targets: 'no-sort',
            bSort: false,
            order: [],
            ajax: {
                url: `/api/Account/Session/Get`,
                type: 'GET'
            },
            columns: [
                {
                    data: 'ip',
                    title: 'IP',
                    className: "audit-action-type",
                    orderable: false,
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: 'created',
                    className: "audit-created",
                    title: 'Created',
                    orderable: true,
                    render: (data) => {
                        return `<span>${DateTimeUtils.toDisplayDateTime(data)}</span>`;
                    }
                },
                {
                    data: 'lastAccess',
                    orderable: true,
                    title: 'Last Access',
                    className: "audit-created",
                    render: (data) => {
                        return `<span>${DateTimeUtils.toDisplayDateTime(data)}</span>`;
                    }
                },
                {
                    data: 'userAgent',
                    className: "audit-created",
                    title: 'User Agent',
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: 'os',
                    title: 'Os',
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: 'device',
                    orderable: false,
                    title: 'Device',
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: 'id',
                    orderable: false,
                    defaultContent: '',
                    mRender: function (id) {
                        return `<div class="text-center pr-2"><button class='btn btn-primary table-button table-button-red remove-session' data-id='${id}'">Remove</button></div>`;
                    }
                }
            ],
        });

        this.$sessionTable.on('click', 'button.remove-session', (event) => {
            const id = $(event.target).data('id');

            this.confirmationModal.show({ key: 'removeSession', id: id }, 'Are you sure you want to remove this session?');
        });
    }

    reloadTable() {
        this.$sessionTable
            .DataTable()
            .clear()
            .draw();
    }


    remove(id) {
        Api.delete(`/api/Account/Session/Remove/${id}`)
            .done(() => {
                this.reloadTable();
            })
            .fail((response) => {
                this.statusAlert.showErrors(response.responseJSON);
            })
    }

}
