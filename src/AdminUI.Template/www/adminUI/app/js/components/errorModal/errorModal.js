class errorModal {
    constructor($parent) {
        this.$errorModal = undefined;
        this.data = undefined;

        this.$errorModal = $parent.find('.modal.error');

        this.$content = this.$errorModal.find('#modalBody');
    }

    show(text) {
        if (text !== null && text !== undefined) {
            this.$content.text(text);
        }

        this.$errorModal.modal('show');
    }

    hide() {
        this.$errorModal.modal('hide');
    }
}