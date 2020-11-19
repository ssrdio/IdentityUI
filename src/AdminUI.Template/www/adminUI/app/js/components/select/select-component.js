class SelectComponent {

    constructor($parent, selector) {
        this.$el = $parent.find(selector);

        this.$select = this.$el.find('select');
        this.$error = this.$el.find('.error');

        this.$select2 = this.$el.find('.select2-container').select2();

        this.handleTabEvents();
    }

    enable() {
        this.$select2.removeClass('select2-container--disabled');
        this.$select.prop('disabled', false);
    }

    disable() {
        this.$select2.addClass('select2-container--disabled');
        this.$select.prop('disabled', true);
    }

    value() {
        return this.$select.val();
    }

    show() {
        this.$el.show();
    }

    hide() {
        this.$el.hide();
    }

    showError(text) {
        this.$error.show();
        this.$error.text(text);
    }

    hideError() {
        this.$error.hide();
        this.$error.text('');
    }

    empty() {
        this.$select2.empty();
        this.$select.empty();
    }

    addOptions(options) {
        if (this.$select2 !== null && this.$select2 !== undefined) {
            options.forEach((option) => {
                this.$select2.append($('<option />').val(option.id).text(option.text));
            });
        }
        else if (this.$select !== null && this.$select !== undefined) {
            options.forEach((option) => {
                console.log(option)
                this.$select.append($('<option />').val(option.id).text(option.text));
            });
        }
    }

    append($option) {
        if (this.$select2 !== null && this.$select2 !== undefined) {
            this.$select2.append($option);
        }
        else if (this.$select !== null && this.$select !== undefined) {
            this.$select.append($option);
        }
    }

    addSelectedOption(option) {
        if (this.$select2 !== null && this.$select2 !== undefined) {
            this.$select2.append($('<option />').val(option.id).text(option.text));
            this.$select2.val(option.text);
        }
        else if (this.$select !== null && this.$select !== undefined) {
            this.$select.append($('<option />').val(option.id).text(option.text));
            this.$select.val(option.text);
        }
    }

    selectOption(value) {
        if (this.$select2 !== null && this.$select2 !== undefined) {
            this.$select2.val(value);
        }
        else if (this.$select !== null && this.$select !== undefined) {
            this.$select.val(value);
        }
    }

    triggerChange() {
        if (this.$select2 !== null && this.$select2 !== undefined) {
            this.$select2.trigger('change')
        }
        else if (this.$select !== null && this.$select !== undefined) {
            this.$select.trigger('change')
        }
    }

    handleTabEvents() {
        // on first focus (bubbles up to document), open the menu
        $(document).on('focus', '.select2-selection.select2-selection--single', e => {
            $(e.target).closest(".select2-container").siblings('select:enabled').select2('open');
        });

        // steal focus during close - only capture once and stop propogation
        $('select.select2').on('select2:closing', e => {
            $(e.target).data("select2").$selection.one('focus focusin', event => {
                event.stopPropagation();
            });
        });
    }

}