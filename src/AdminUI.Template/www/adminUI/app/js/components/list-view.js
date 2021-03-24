class ListView {
    constructor(options) {
        const $container = options.$container;
        this.newItemPlaceholder = options.newItemPlaceholder;
        this.allowEdit = options.allowEdit ?? true;
        this.allowEmptyItems = options.allowEmptyItems ?? false;
        this.predefinedItems = options.predefinedItems;

        this.$listViewContainer = $('<div class="list-group"></div>');

        $container.find('div.list-view').append(this.$listViewContainer);

        this.$errorSpan = $container.find('span.error');

        this.items = options.items;

        this.statusAlert = new StatusAlertComponent('#status-alert');

        this.init();
    }

    init() {
        this.$listViewContainer.empty();

        if (Array.isArray(this.items)) {
            this.items.forEach((item) => {
                let $item = this.getDefaultItem(item);

                this.$listViewContainer.append($item);
            });
        }

        let template = `
            <div class="list-group-item list-group-item-action p-0">
                <div class="row p-0">
                    <div class="col-11 p-0">
                    <input type="text" class="list-view-input" placeholder="{{placeholder}}" />
                    </div>
                    <div class="col-1 ">
                        <button type="button" class="list-view-button" tabindex="-1">
                            <i class="fas fa-plus"></i>
                        </button>
                    </div>
                </div>
            </div>`;

        let addNewitem = Mustache.render(template, { placeholder: this.newItemPlaceholder });
        let $addNewitem = $(addNewitem);

        $addNewitem.on('click', 'button', () => {
            let $input = $addNewitem.find('input');
            let $row = $addNewitem.find('.row');

            if ($input.val() != "") {
                $row.removeClass('error-row');
                let $newItem = this.getDefaultItem($input.val());

                $addNewitem.before($newItem);
                $input.val(null);
                this.statusAlert.showWarning('You have some unsaved changes on this page');
            } else {
                $row.addClass('error-row');
            }
        });

        $addNewitem.on('keypress', 'input', (event) => {
            if (event.which !== 13) {
                return;
            }

            let $input = $addNewitem.find('input');

            let $newItem = this.getDefaultItem($input.val());

            $addNewitem.before($newItem);
            $input.val(null);
        });

        this.$listViewContainer.append($addNewitem);
    }

    getDefaultItem(value) {
        let defaultTemplate = `
            <div class="list-group-item list-group-item-action p-0">
                <div class="row">
                    <span class="col-10 list-view-item">{{value}}</span>
                    <div class="col-1">
                        {{#edit}}
                        <button type="button" class="list-view-button edit" tabindex="-1">
                            <i class="fas fa-edit"></i>
                        </button>
                        {{/edit}}
                    </div>
                    <div class="col-1">
                        <button type="button" class="list-view-button remove" tabindex="-1">
                            <i class="fas fa-trash-alt"></i>
                        </button>
                    </div>
                </div>
            </div>`;

        let defaultItem = Mustache.render(defaultTemplate, { value: value, edit: this.allowEdit });
        let $defaultItem = $(defaultItem);

        $defaultItem.on('click', 'button.remove', () => {
            $defaultItem.remove();
            this.statusAlert.showWarning('You have some unsaved changes on this page');
        });

        if (this.allowEdit) {
            $defaultItem.on('click', 'button.edit', (event) => {
                let $item = $(event.delegateTarget);

                $item.replaceWith(this.getEditItem(value))
            });
        }

        return $defaultItem;
    }

    getEditItem(value) {
        let currentValue = value;
        let confirmTemplate = `
            <div class="list-group-item list-group-item-action p-0">
                <div class="row">
                    <input type="text" class="list-view-input col-10" value="{{value}}" />
                    <div class="col-1">
                        <button type="button" class=" list-view-button confirm">
                            <i class="fas fa-check"></i>
                        </button>
                    </div>
                    <div class="col-1">
                        <button type="button" class="list-view-button remove" tabindex="-1">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                </div>
            </div>`;

        let editItem = Mustache.render(confirmTemplate, { value: value });
        let $editItem = $(editItem);

        $editItem.find('input').focus();

        $editItem.on('click', 'button.remove', (event) => {
            let $item = $(event.delegateTarget);
            $item.replaceWith(this.getDefaultItem(currentValue));
        });

        $editItem.on('click', 'button.confirm', (event) => {
            let $item = $(event.delegateTarget);

            $item.replaceWith(this.getDefaultItem($item.find('input').val()));
            this.statusAlert.showWarning('You have some unsaved changes on this page');
        });

        $editItem.on('keypress', 'input', (event) => {
            if (event.which !== 13) {
                return;
            }

            let $item = $(event.delegateTarget);

            $item.replaceWith(this.getDefaultItem($item.find('input').val()));
        })

        return $editItem;
    }

    showError(error) {
        this.$errorSpan.text(error);
        this.$errorSpan.show();
    }

    hideError() {
        this.$errorSpan.text('');
        this.$errorSpan.hide();
    }

    setValue(items) {
        this.items = items;
        this.init();
    }

    getValue() {
        let items = this.$listViewContainer.find('.list-view-item')
            .map((index, item) => {
                return $(item).text();
            })
            .toArray();

        return items;
    }
}

class PredefinedListView {
    constructor(options) {
        const $container = options.$container;
        this.newItemPlaceholder = options.newItemPlaceholder;
        this.predefinedItems = options.predefinedItems;

        this.$listViewContainer = $('<div class="list-group"></div>');
        $container.find('div.list-view').append(this.$listViewContainer);
        this.$errorSpan = $container.find('span.error');

        this.statusAlert = new StatusAlertComponent('#status-alert');

        this.items = options.items;

        this.init();
    }

    init() {
        this.$listViewContainer.empty();

        if (!Array.isArray(this.items)) {
            this.items = Array();
        }

        let template = `
            <div class="list-group-item list-group-item-action p-0">
                <div class="row p-0">
                    <div class="col-12 p-0">
                    <select class="list-view-select">
                        <option value="__placeholder__" disabled selected hidden class="placeholder-option">{{placeholder}}</option>
                        {{#selectOptions}}
                            {{#show}}
                            <option value="{{value}}">{{value}}</option>
                            {{/show}}
                            {{#hidden}}
                            <option value="{{value}}" hidden>{{value}}</option>
                            {{/hidden}}
                        {{/selectOptions}}
                    <select>
                    </div>
                    <div class="col-1 custom-select-drodown-icon">
                        <button type="button" class="list-view-button">
                            <i class="fas fa-angle-down"></i>
                        </button>
                    </div>
                </div>
            </div>`;

        let selectOptions = Array();
        this.predefinedItems.forEach((element) => {
            if (this.items.some(x => x === element)) {
                selectOptions.push({ value: element, hidden: true });
            }
            else {
                selectOptions.push({ value: element, show: true });
            }
        });

        let addNewitem = Mustache.render(template, {
            placeholder: this.newItemPlaceholder,
            selectOptions: selectOptions
        });
        let $addNewitem = $(addNewitem);
        let $select = $addNewitem.find('select');

        this.items.forEach((item) => {
            let $item = this.getDefaultItem(item, $select);

            this.$listViewContainer.append($item);
        });

        $addNewitem.on('change', 'select', (event) => {
            let selected = $select.val();

            $select.find(`option[value="${selected}"]`).attr('hidden', true);

            let $newItem = this.getDefaultItem(selected, $select);

            $addNewitem.before($newItem);

            $select.val('__placeholder__');

            this.statusAlert.showWarning('You have some unsaved changes on this page');
        });

        this.$listViewContainer.append($addNewitem);
    }

    getDefaultItem(value, $select) {
        let defaultTemplate = `
            <div class="list-group-item list-group-item-action p-0">
                <div class="row">
                    <span class="col-10 list-view-item">{{value}}</span>
                    <div class="col-1 p-0">
                    </div>
                    <div class="col-1">
                        <button type="button" class="list-view-button remove">
                            <i class="fas fa-trash-alt"></i>
                        </button>
                    </div>
                </div>
            </div>`;

        let defaultItem = Mustache.render(defaultTemplate, { value: value });
        let $defaultItem = $(defaultItem);

        $defaultItem.on('click', 'button.remove', () => {
            this.statusAlert.showWarning('You have some unsaved changes on this page');
            $defaultItem.remove();
            $select.find(`option[value="${value}"]`).attr('hidden', false);
        });

        return $defaultItem;
    }

    showError(error) {
        this.$errorSpan.text(error);
        this.$errorSpan.show();
    }

    hideError() {
        this.$errorSpan.text('');
        this.$errorSpan.hide();
    }

    setValue(items) {
        this.items = items;
        this.init();
    }

    getValue() {
        let items = this.$listViewContainer.find('.list-view-item')
            .map((index, item) => {
                return $(item).text();
            })
            .toArray();

        return items;
    }
}