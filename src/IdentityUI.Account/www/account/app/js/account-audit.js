class AccountAudit{
    constructor(actionTypes) {
        this.statusAlert = new StatusAlertComponent('#status-alert-container');

        this.auditTable = new AccountAuditTable(actionTypes, this.statusAlert);

        const $actionsDropdown = $('#actions-dropdown');

        $actionsDropdown.on('click', 'button.reset-filters', () => {
            this.auditTable.resetFilters();
        })

        $actionsDropdown.on('click', 'button.export', () => {
            this.export();
        });
    }

    export() {
        this.statusAlert.hide();

        const validParams = Object.fromEntries(Object.entries(this.auditTable.getFilters()).filter(([_, v]) => v != null));

        fetch(`/Account/Audit/Export?${new URLSearchParams(validParams).toString()}`)
            .then((resp) => {
                if (!resp.ok) {
                    this.statusAlert.showError('Failed to export audit');
                    return;
                }

                let header = resp.headers.get("content-disposition");
                let filename = 'audit.json';

                if (header && header.indexOf('attachment') !== -1) {
                    let utfFilenameRegex = /filename[^;\n]*=UTF-\d['"]*((['"]).*?[.]$\2|[^;\n]*)?/;

                    let utfMatches = utfFilenameRegex.exec(header);

                    if (utfMatches !== null && utfMatches[1] !== undefined) {
                        filename = utfMatches[1].replace(/['"]/g, '');
                        filename = decodeURIComponent(filename);
                    }
                    else {
                        let asciiFilenameReged = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;

                        let asciiMatches = asciiFilenameReged.exec(header);
                        if (asciiMatches !== null && asciiMatches[1] !== undefined) {
                            filename = asciiMatches[1].replace(/['"]/g, '');
                        }
                    }
                }

                resp.blob()
                    .then((blob) => {
                        const url = window.URL.createObjectURL(blob);
                        const a = document.createElement('a');
                        a.style.display = 'none';
                        a.href = url;
                        a.download = filename;

                        document.body.appendChild(a);
                        a.click();
                        window.URL.revokeObjectURL(url);
                    });
            })
    }
}

class AccountAuditDetailsModal {
    constructor(actionTypes, groupId) {
        this.$modal = $('#audit-details-modal');
        this.$modal.on('hidden.bs.modal', () => {
            this.reset();
        });

        this.groupId = groupId;
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

        Api.get(`/Account/Audit/Get/${id}`)
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

class AccountAuditTable {
    constructor(actionTypes, statusAlert) {
        this.$auditTable = $('#accout-audit-table');
        this.actionTypes = actionTypes;
        this.statusAlert = statusAlert;

        this.auditDetailsModal = new AccountAuditDetailsModal();
        this.auditTableFilters = new AccountAuditTableFilters(actionTypes, this.statusAlert, () => {
            this.reloadTable();
        });

        this.auditCommentModal = new AccountAuditCommentModal();
        this.init();
    }

    init() {
        this.$auditTable.DataTable({
            serverSide: true,
            processing: true,
            order: [[2, 'desc']],
            lengthChange: false,
            pageLength: 20,
            searching: false,
            ajax: {
                url: `/Account/Audit/Get`,
                type: 'GET',
                data: (params) => {
                    const orderByTypes = Object.freeze({
                        'Dessending': 1,
                        'Assending': 2,
                    });

                    let customParams = {
                        draw: params.draw,
                        start: params.start,
                    }

                    switch (params.order.find(x => x.column === 2).dir) {
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

                    this.$auditTable.DataTable().page.len(filters.length);

                    const finalParams = { ...customParams, ...filters };

                    return finalParams;
                }
            },
            columns: [
                {
                    title: "Type",
                    orderable: false,
                    data: "actionType",
                    className: "audit-action-type",
                    render: $.fn.dataTable.render.text()
                },
                {
                    title: "Resource Name",
                    orderable: false,
                    data: "resourceName",
                    render: $.fn.dataTable.render.text()
                },
                {
                    title: "Created",
                    orderable: true,
                    data: "created",
                    className: "audit-created",
                    render: (data) => {
                        return `<span>${DateTimeUtils.toDisplayDateTime(data)}</span>`;
                    }
                },
                {
                    title: "",
                    orderable: false,
                    data: null,
                    className: "audit-action-buttons",
                    render: (data) => {
                        let dropDown = `
                                <div class="dropdown">
                                  <button type="button" class="btn btn-primary table-button action-table-button" data-toggle="dropdown">
                                    Action
                                    <i class="fas fa-sort-down"></i>
                                  </button>
                                  <div class="dropdown-menu">
                                    <button class="dropdown-item audit-details" data-id="${data.id}">Details</button>
                                    <button class="dropdown-item audit-comments" data-id="${data.id}">Comments</button>
                                  </div>
                                </div>`;
                        return dropDown;
                    }
                }
            ]
        });

        this.$auditTable.on('click', 'button.audit-details', (event) => {
            const id = $(event.target).data('id');

            this.auditDetailsModal.showModal(id);
        });

        this.$auditTable.on('click', 'button.audit-comments', (event) => {
            const id = $(event.target).data('id');

            this.auditCommentModal.show(id);
        });
    }

    resetFilters() {
        this.auditTableFilters.reset();
    }

    getFilters() {
        return this.auditTableFilters.getFilters();
    }

    reloadTable() {
        this.$auditTable.DataTable()
            .clear()
            .draw();
    }
}

class AccountAuditTableFilters {
    constructor(actionTypes, statusAlert, onChange) {
        this.onChange = onChange;
        this.statusAlert = statusAlert;


        const $actionTypeContainer = $('#action-type');
        this.$actionTypeSelect = $actionTypeContainer.find('select');

        $actionTypeContainer.on('click', "button.reset-button", () => {
            this.resetActionType();
        });

        const $dateTimeRangePicker = $('#date-time-range-container');

        this.daterangePicker = new RangePicker($dateTimeRangePicker, this.statusAlert, () => {
            this.change();
        });

        this.$pageLenghtContainer = $('#page-lenght-select');
        this.$pageLenghtSelect = this.$pageLenghtContainer.find('select');

        $dateTimeRangePicker.on('click', 'button.reset-button', () => {
            this.daterangePicker.reset();
        });

        this.initActionTypeSelect(actionTypes);
        this.initPageLenghtSelect();
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
            actionType: this.$actionTypeSelect.val(),
            from: this.daterangePicker.getFrom(),
            to: this.daterangePicker.getTo(),
            length: this.$pageLenghtSelect.val()
        }
    }

    initActionTypeSelect(actionTypes) {
        this.$actionTypeSelect.select2({
            placeholder: "Select action type",
            allowClear: true,
            data: actionTypes,
        });

        this.$actionTypeSelect.val(null).trigger('change');

        this.$actionTypeSelect.on('change', () => {
            this.hideError();

            this.change();
        })
    }

    initPageLenghtSelect() {
        var data = [
            {
                id: 10,
                text: '10',
                selected: true
            },
            {
                id: 20,
                text: '20'
            },
            {
                id: 50,
                text: '50'
            },
            {
                id: 100,
                text: '100'
            }]

        this.$pageLenghtSelect.select2({
            data: data,
            minimumResultsForSearch: Infinity
        });

        this.$pageLenghtSelect.on('change', () => {
            this.pageLength = this.$pageLenghtSelect.val();

            this.onChange();
        });
    }

    reset() {
        const onChange = this.onChange;

        this.onChange = null;

        this.resetActionType();

        this.daterangePicker.reset();

        this.onChange = onChange;
        this.change();
    }

    resetActionType() {
        this.$actionTypeSelect.val(null).trigger('change');
    }
}

class AccountAuditCommentModal {
    constructor() {
        this.$modal = $('#audit-comments-modal');

        this.$auditCommentsContainer = this.$modal.find('#audit-comment-container');
        this.loader = new DotLoader(this.$modal.find('#audit-comment-loader'), this.$auditCommentsContainer);

        const $auditCommentForm = this.$modal.find('#add-audit-comment-form');
        this.commentArea = new TextAreaComponent($auditCommentForm, '.comment-text-area');

        this.statusMessage = new StatusMessage(this.$modal.find('.audit-comment-modal-body'));

        $auditCommentForm.on('click', 'button.submit-button', () => {
            this.addComment();
        });

        this.$modal.on('hidden.bs.modal', () => {
            this.reset();
        });
    }

    show(id) {
        this.id = id;
        this.init();

        this.$modal.modal('show');
    }

    hide() {
        this.$modal.modal('hide');
    }

    init() {
        this.getComments();
    }

    reset() {
        this.$auditCommentsContainer.empty();
        this.commentArea.value(null);
    }

    showComments(data) {
        this.$auditCommentsContainer.empty();

        if (data.length === 0) {
            const noCommentsTemplate = `
                <div class="no-comments">
                    <p>No comments have been added for this audit</p>
                </div>`

            this.$auditCommentsContainer.append($(noCommentsTemplate));
            this.loader.hide();
            return;
        }

        const commentTemplate = `
            <div class="comment-body">
                <div class="d-flex">
                    <h4>{{user}}</h4>
                    <span class="d-flex ml-auto comment-created">{{created}}</span>
                </div>
                <p>{{comment}}</p>
            </div>`;

        data.forEach((element, index) => {
            const obj = {
                user: element.user,
                created: DateTimeUtils.toDisplayDateTime(element.created),
                comment: element.comment,
            };

            let newComment = Mustache.render(commentTemplate, obj);
            this.$auditCommentsContainer.append($(newComment));
        });

        this.loader.hide();
    }

    showErrors(errors) {
        if (errors[''] !== undefined && errors[''] !== null) {
            this.statusMessage.showError('Error', errors['']);
        }

        this.commentArea.showError(errors.comment);
    }

    hideErrors() {
        this.statusMessage.hide();

        this.commentArea.hideError();
    }

    getData() {
        return {
            comment: this.commentArea.value()
        };
    }

    addComment() {
        this.hideErrors();
        this.loader.show();

        Api.post(`/Account/Audit/AddComment/${this.id}`, this.getData())
            .done(() => {
                this.commentArea.value(null);
                this.getComments();
            })
            .fail((response) => {
                this.showErrors(response.responseJSON);
                this.loader.hide();
            });
    }

    getComments() {
        this.hideErrors();
        this.loader.show();

        Api.get(`/Account/Audit/GetComments/${this.id}`)
            .done((data) => {
                this.showComments(data);
            })
            .fail((response) => {
                this.showErrors(response.responseJSON);
                this.loader.hide();
            });
    }
}