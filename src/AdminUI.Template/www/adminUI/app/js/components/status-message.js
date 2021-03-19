class StatusMessage {
    constructor($parent) {
        this.modalStatus = $parent.find('.modal-body-status');

        this.errorTitle = this.modalStatus.find('.error-title');
        this.warningTitle = this.modalStatus.find('.warning-title');
        this.successTitle = this.modalStatus.find('.success-title');

        this.body = $parent.find('.status-body');
    }

    showSuccess(title, message) {
        this.modalStatus.show();

        this.warningTitle.hide();
        this.errorTitle.hide();

        this.successTitle.text(title);
        this.successTitle.show();

        this.setBody(message);
    }

    showWarning(title, message) {
        this.modalStatus.show();

        this.successTitle.hide();
        this.errorTitle.hide();

        this.warningTitle.text(title);
        this.warningTitle.show();

        this.setBody(message);
    }

    showError(title, message) {
        this.modalStatus.show();

        this.warningTitle.hide();
        this.successTitle.hide();

        this.errorTitle.text(title);
        this.errorTitle.show();

        this.setBody(message);
    }

    setBody(data) {
        this.body.empty();

        let text = '';

        if (Array.isArray(data)) {
            data.forEach((message) => {
                text += message + '</br>';
            });
        }
        else {
            text = data;
        }

        this.body.append(text);
    }

    hide() {
        this.modalStatus.hide();

        this.successTitle.hide();
        this.warningTitle.hide();
        this.errorTitle.hide();

        this.body.empty();
    }
}
