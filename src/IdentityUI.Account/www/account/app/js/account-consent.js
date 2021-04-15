class AccountConsent{
    constructor() {
        this.statusAlert = new StatusAlertComponent('#status-alert-container');
        this.consentTable = new AccountConsentTable(this.statusAlert);
    }
}

class AccountConsentTable {
    constructor(statusAlert) {
        this.statusAlert = statusAlert;
        this.$consentTable = $('#accout-consent-table');
        this.init();

        this.confirmationModal = new conformationModal(
            $(`#modalContainer`),
            onYesClick => {
                if (onYesClick === null || onYesClick === undefined) {
                    return;
                }

                if (onYesClick.key === 'removeConsent') {
                    this.remove(onYesClick.id);
                }
            }
        );
    }

    init() {
        this.$consentTable.DataTable({
            serverSide: true,
            processing: true,
            targets: 'no-sort',
            bSort: false,
            order: [],
            ajax: {
                url: `/api/Account/Consent/Get`,
                type: 'GET'
            },
            columns: [
                {
                    data: 'client',
                    title: 'Client',
                    className: "audit-created",
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: 'type',
                    className: "audit-created",
                    title: 'Type',
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: 'scopes',
                    title: 'Scope',
                    className: "audit-action-type",
                    mRender: (scopes) => {
                        let scopeArray = JSON.parse(scopes);
                        let scopeText = '';
                        scopeArray.forEach((element, index) => {
                            if (index === 0) {
                                scopeText += element;
                            }
                            else {
                                scopeText += `, ` + element;
                            }
                        });

                        let template = '<span>{{text}}</span>';

                        let span = Mustache.render(template, { text: scopeText });

                        return span;
                    }
                },
                {
                    data: 'createdDate',
                    className: "audit-created",
                    title: 'Created',
                    render: (data) => {
                        return `<span>${DateTimeUtils.toDisplayDateTime(data)}</span>`;
                    }
                },
                {
                    data: 'id',
                    orderable: false,
                    defaultContent: '',
                    mRender: function (id) {
                        return `<div class="text-center pr-2"><button class='btn btn-primary table-button table-button-red remove-consent' data-id='${id}'">Remove</button></div>`;
                    }
                }
            ],
        });

        this.$consentTable.on('click', 'button.remove-consent', (event) => {
            const id = $(event.target).data('id');

            this.confirmationModal.show({ key: 'removeConsent', id: id }, 'Are you sure you want to remove this consent?');
        });
    }

    reloadTable() {
        this.$consentTable
            .DataTable()
            .clear()
            .draw();
    }


    remove(id) {
        Api.delete(`/api/Account/Consent/Remove/${id}`)
            .done(() => {
                this.reloadTable();
            })
            .fail((response) => {
                this.statusAlert.showErrors(response.responseJSON);
            })
    }

}
