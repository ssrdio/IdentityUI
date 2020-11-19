class AuditDetailsModal {
    constructor(actionTypes) {
        this.$modal = $('#audit-details-modal');
        this.$modal.on('hidden.bs.modal', () => {
            this.reset();
        });

        this.actionTypes = actionTypes;

        this.$auditDataContiner = this.$modal.find('#audit-data-container')

        this.loader = new DotLoader(this.$modal.find('#audit-details-loader'), this.$auditDataContiner);

        this.$actionType = this.$auditDataContiner.find('#action-type-container');

        this.$objectType = this.$auditDataContiner.find('#object-type-container');
        this.$objectIdentifier = this.$auditDataContiner.find('#object-identifier-container');
        this.$objectMetadata = this.$auditDataContiner.find('#object-metadata-container');

        this.$subjectType = this.$auditDataContiner.find('#subject-type-container');
        this.$subjectIdentifier = this.$auditDataContiner.find('#subject-identifier-container');
        this.$subjectMetadata = this.$auditDataContiner.find('#subject-metadata-container');

        this.$groupIdentifier = this.$auditDataContiner.find('#group-identifier-container');

        this.$resourceName = this.$auditDataContiner.find('#resource-name-container');
        this.$host = this.$auditDataContiner.find('#host-container');
        this.$remoteIp = this.$auditDataContiner.find('#remote-ip-container');
        this.$userAgent = this.$auditDataContiner.find('#user-agent-container');
        this.$traceIdentifier = this.$auditDataContiner.find('#trace-identifier-container');
        this.$appVersion = this.$auditDataContiner.find('#app-version-container');
        this.$metadata = this.$auditDataContiner.find('#metadata-container');

        this.$created = this.$auditDataContiner.find('#created-container');
    }

    showModal(id) {
        this.$modal.modal('show');
        this.get(id);
    }

    hideModal() {
        this.$modal.modal('hide');
        this.reset();
    }

    reset() {
        this.$actionType.text(null);

        this.$objectType.text(null);
        this.$objectIdentifier.text(null);
        this.$objectMetadata.text(null);

        this.$subjectType.text(null);
        this.$subjectIdentifier.text(null);
        this.$subjectMetadata.text(null);

        this.$groupIdentifier.text(null);

        this.$resourceName.text(null);
        this.$host.text(null);
        this.$remoteIp.text(null);
        this.$userAgent.text(null);
        this.$traceIdentifier.text(null);
        this.$appVersion.text(null);
        this.$metadata.text(null);

        this.$created.text(null);
    }

    set(data) {
        this.$actionType.text(data.actionType);

        this.$objectType.text(data.objectType);
        this.$objectIdentifier.text(data.objectIdentifier);
        this.$objectMetadata.jJsonViewer(data.objectMetadata);

        this.$subjectType.text(data.subjectType);
        this.$subjectIdentifier.text(data.subjectIdentifier);
        this.$subjectMetadata.jJsonViewer(data.subjectMetadata);

        this.$groupIdentifier.text(data.groupIdentifier);

        this.$resourceName.text(data.resourceName);
        this.$host.text(data.host);
        this.$remoteIp.text(data.remoteIp);
        this.$userAgent.text(data.userAgent);
        this.$traceIdentifier.text(data.traceIdentifier);
        this.$appVersion.text(data.appVersion);
        this.$metadata.jJsonViewer(data.metadata);

        this.$created.text(moment.utc(data.created).format("D.M.YYYY HH:mm:ssZ"));
    }

    get(id) {
        this.loader.show();

        Api.get(`/IdentityAdmin/Audit/Get/${id}`)
            .done((data) => {
                this.set(data)
            })
            .fail((resp) => {
                console.error('failed to get audit details');
            })
            .always(() => {
                this.loader.hide();
            })
    }
}

class AuditTable {
    constructor(actionTypes, subjectTypes) {
        this.$auditTable = $('#audit-table');
        this.actionTypes = actionTypes;

        this.statusAlert = new StatusAlertComponent('#status-alert-container');

        this.auditDetailsModal = new AuditDetailsModal();
        this.auditTableFilters = new AuditTableFilters(subjectTypes, actionTypes, this.statusAlert, () => {
            this.reloadTable();
        });

        this.init();

        $('#audit-table tbody').on('click', 'tr', (event) => {
            const data = this.$auditTable.DataTable().row(event.target).data();
            this.auditDetailsModal.showModal(data.id);
        });
    }

    init() {
        this.$auditTable.DataTable({
            serverSide: true,
            processing: true,
            order: [[6, 'desc']],
            lengthChange: false,
            pageLength: 20,
            searching: false,
            ajax: {
                url: '/IdentityAdmin/Audit/Get',
                type: 'GET',
                data: (params) => {
                    const orderByTypes = Object.freeze({
                        'Dessending': 1,
                        'Assending': 2,
                    });

                    let customParams = {
                        draw: params.draw,
                        start: params.start,
                        length: params.length,
                    }

                    switch (params.order.find(x => x.column === 6).dir) {
                        case "desc": {
                            customParams.orderBy = orderByTypes.Dessending;
                            break;
                        }
                        case "asc": {
                            customParams.orderBy = orderByTypes.Assending;
                            break;
                        }
                        default: {
                            customParams.orderBy = orderByTypes.Dessending;
                            break;
                        }
                    }

                    const filters = this.auditTableFilters.getFilters();

                    const finalParams = { ...customParams, ...filters };

                    return finalParams;
                }
            },
            columns: [
                {
                    data: 'id',
                    orderable: false,
                    visible: false,
                },
                {
                    title: "Type",
                    orderable: false,

                    data: "actionType",
                    render: $.fn.dataTable.render.text()
                },
                {
                    title: "Resource Name",
                    orderable: false,

                    data: "resourceName",
                    render: $.fn.dataTable.render.text()
                },
                {
                    title: "Object Type",
                    orderable: false,

                    data: "objectType",
                    render: $.fn.dataTable.render.text()
                },
                {
                    title: "Subject Type",
                    orderable: false,

                    data: "subjectType",
                    render: $.fn.dataTable.render.text()
                },
                {
                    title: "Subject Identifier",
                    orderable: false,

                    data: "subjectIdentifier",
                    render: $.fn.dataTable.render.text()
                },
                {
                    data: null,
                    title: "Created",
                    render: (data) => {
                        return `<span>${moment.utc(data.created).format("D.M.YYYY HH:mm:ssZ")}</span>`;
                    }
                }
            ]
        });
    }

    reloadTable() {
        this.$auditTable.DataTable()
            .clear()
            .draw();
    }
}

class AuditTableFilters {
    constructor(subjectTypes, actionTypes, statusAlert, onChange) {
        this.onChange = onChange;
        this.statusAlert = statusAlert;

        const $objectTypeContainer = $('#object-type');
        this.$objectTypeSelect = $objectTypeContainer.find('select');

        $objectTypeContainer.on('click', 'button.reset-button', () => {
            this.resetObjectType();
        });

        const $objectIdentifierContainer = $('#object-identifier');
        this.$objectIdentifierSelect = $objectIdentifierContainer.find('select');

        $objectIdentifierContainer.on('click', 'button.reset-button', () => {
            this.resetObjectIdentifierSelect();
        });

        const $subjectTypeContainer = $('#subject-type');
        this.$subjectTypeSelect = $subjectTypeContainer.find('select');

        $subjectTypeContainer.on('click', 'button.reset-button', () => {
            this.resetSubjectType();
        });

        const $subjectIdentifierContainer = $('#subject-identifier');
        this.$subjectIdentifierSelect = $subjectIdentifierContainer.find('select');

        $subjectIdentifierContainer.on('click', 'button.reset-button', () => {
            this.resetSubjectIdentifier();
        });

        const $actionTypeContainer = $('#action-type');
        this.$actionTypeSelect = $actionTypeContainer.find('select');

        $actionTypeContainer.on('click', "button.reset-button", () => {
            this.resetActionType();
        });

        const $resourceNameContainer = $('#resource-name');
        this.$resourceNameSelect = $resourceNameContainer.find('select');

        $resourceNameContainer.on('click', 'button.reset-button', () => {
            this.resetResourceName();
        });

        const $dateTimeRangePicker = $('#date-time-range-container');

        this.daterangePicker = new RangePicker($dateTimeRangePicker, this.statusAlert, () => {
            this.change();
        });

        $dateTimeRangePicker.on('click', 'button.reset-button', () => {
            this.daterangePicker.reset();
        });

        $('#reset-all-button').on('click', () => {
            this.reset();
        })

        this.initObjectTypeSelect();
        this.initObjectIdentifierSelect();

        this.initSubjectTypeSelect(subjectTypes);
        this.initSubjectIdentifierSelect();

        this.initActionTypeSelect(actionTypes);

        this.initResourceNameSelect();
    }

    showError(error) {
        this.statusAlert.showError(error);
    }

    hideError() {
        this.statusAlert.hide();
    }

    change() {
        if (this.onChange === null || this.onChange === undefined) {
            return;
        }

        this.onChange();
    }

    getFilters() {
        return {
            objectType: this.$objectTypeSelect.val(),
            objectIdentifier: this.$objectIdentifierSelect.val(),
            subjectType: this.$subjectTypeSelect.val(),
            subjectIdentifier: this.$subjectIdentifierSelect.val(),
            actionType: this.$actionTypeSelect.val(),
            resourceName: this.$resourceNameSelect.val(),
            from: this.daterangePicker.getFrom(),
            to: this.daterangePicker.getTo(),
        }
    }

    initObjectTypeSelect() {
        this.$objectTypeSelect.select2({
            ajax: {
                url: `/IdentityAdmin/Audit/GetObjectTypes`,
                delay: 100,
                data: (params) => {
                    params.page = params.page ?? 1;

                    return params;
                }
            }
        });

        this.$objectTypeSelect.on('change', () => {
            this.hideError();

            this.selectedObjectType = this.$objectTypeSelect.val();
            this.resetObjectIdentifierSelect();
            this.$objectIdentifierSelect.prop('disabled', false);

            this.change();
        });
    }

    initObjectIdentifierSelect() {
        this.$objectIdentifierSelect.select2({
            ajax: {
                url: `/IdentityAdmin/Audit/GetObjectIdentifiers`,
                delay: 100,
                data: (params) => {
                    params.page = params.page ?? 1;

                    params.objectType = this.$objectTypeSelect.val();

                    return params;
                }
            }
        });

        this.$objectIdentifierSelect.on('change', () => {
            this.hideError();

            this.change();
        });
    }

    initSubjectTypeSelect(subjectTypes) {
        this.$subjectTypeSelect.select2({
            data: subjectTypes
        });

        this.$subjectTypeSelect.val(null).trigger('change');

        this.$subjectTypeSelect.on('change', () => {
            this.hideError();

            this.change();
        });
    }

    initSubjectIdentifierSelect() {
        this.$subjectIdentifierSelect.select2({
            ajax: {
                url: `/IdentityAdmin/Audit/GetSubjectIdentifiers`,
                delay: 100,
                data: (params) => {
                    params.page = params.page ?? 1;

                    return params;
                }
            }
        });

        this.$subjectIdentifierSelect.on('change', () => {
            this.hideError();

            this.change();
        });
    }

    initActionTypeSelect(actionTypes) {
        this.$actionTypeSelect.select2({
            data: actionTypes,
        });

        this.$actionTypeSelect.val(null).trigger('change');

        this.$actionTypeSelect.on('change', () => {
            this.hideError();

            this.change();
        })
    }

    initResourceNameSelect() {
        this.$resourceNameSelect.select2({
            ajax: {
                url: `/IdentityAdmin/Audit/GetResourceNames`,
                delay: 100,
                data: (params) => {
                    params.page = params.page ?? 1;

                    return params;
                }
            }
        });

        this.$resourceNameSelect.on('change', () => {
            this.hideError();

            this.change();
        })
    }

    reset() {
        const onChange = this.onChange;

        this.onChange = null;

        this.resetObjectType();

        this.resetSubjectType();
        this.resetSubjectIdentifier();

        this.resetActionType();
        this.resetResourceName();

        this.daterangePicker.reset();

        this.onChange = onChange;
        this.change();
    }

    resetObjectType() {
        const onChange = this.onChange;
        this.onChange = null;

        this.$objectTypeSelect.val(null).trigger('change');
        //TODO: remove options

        this.resetObjectIdentifierSelect();
        this.$objectIdentifierSelect.prop('disabled', true);

        this.onChange = onChange;
        this.change();
    }

    resetObjectIdentifierSelect() {
        this.$objectIdentifierSelect.val(null).trigger('change');
        //TODO: remove options
    }

    resetSubjectType() {
        this.$subjectTypeSelect.val(null).trigger('change');
        //TODO: remove options
    }

    resetSubjectIdentifier() {
        this.$subjectIdentifierSelect.val(null).trigger('change');
        //TODO: remove options
    }

    resetActionType() {
        this.$actionTypeSelect.val(null).trigger('change');
        //TODO: remove options
    }

    resetResourceName() {
        this.$resourceNameSelect.val(null).trigger('change');
        //TODO: remove options
    }
}