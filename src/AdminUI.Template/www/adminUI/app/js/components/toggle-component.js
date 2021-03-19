class ToggleComponent {
    constructor($parent, selector) {
        this.$el = $parent.find(selector);

        this.$input = this.$el.find('input[type=checkbox]');
        this.$error = this.$el.find('.error');
    }

    enable() {
        this.$input.prop('disabled', false);
    }

    disable() {
        this.$input.prop('disabled', true);
    }

    setValue(text) {
        if (text === true) {
            this.$input.prop('checked', true);
        }
        else if (text === false) {
            this.$input.prop('checked', false);
        }
    }

    getValue() {
        return this.$input.prop('checked');
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
}