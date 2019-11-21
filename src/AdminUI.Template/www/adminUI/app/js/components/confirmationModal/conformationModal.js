class conformationModal {
    constructor($parent, onYesClick) {
        this.$confirmationModal = undefined;
        this.data = undefined;

        this.$confirmationModal = $parent.find('.modal.confirmation');

        this.$content = this.$confirmationModal.find('#modalBody');

        this.$yesButton = this.$confirmationModal.find('button.confirm');
        this.onYesClick = onYesClick;

        this.$yesButton.on('click', () => {
            if (this.onYesClick) {
                this.onYesClick(this.data);
                this.hide();
            }
        });
    }

    show(data, text) {
        if (text !== null && text !== undefined) {
            this.$content.text(text);
        }

        this.data = data;
        this.$confirmationModal.modal('show');
    }

    hide() {
        this.$confirmationModal.modal('hide');
    }
}