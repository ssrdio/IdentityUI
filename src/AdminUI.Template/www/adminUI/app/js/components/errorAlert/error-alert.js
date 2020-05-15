class ErrorAlert {
    constructor($parent) {
        this.$errorAlert = $parent.find('.alert.alert-danger');
        this.$errorAlertBody = this.$errorAlert.find('.alert-body');

        $parent.on('click', 'button.close', () => {
            this.hide();
        })
    }

    showError(error) {
        this.$errorAlertBody.text(error);
        this.$errorAlert.show();
    }

    showErrors(errors) {
        let text = '';

        errors.forEach((error) => {
            text += error + '</br>';
        });

        this.$errorAlertBody.html(text);
        this.$errorAlert.show();
    }

    hide() {
        this.$errorAlert.hide();
    }
}