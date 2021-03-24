class StatusAlertComponent {
    constructor(container) {
        this.$successMessage = $(`${container} .alert-success .alertMessage`);
        this.$errorMessage = $(`${container} .alert-danger .alertMessage`);
        this.$warningMessage = $(`${container} .alert-warning .alertMessage`);
        this.$success = $(`${container} .alert-success`);
        this.$error = $(`${container} .alert-danger`);
        this.$warning = $(`${container} .alert-warning`);
        this.$snackbar = $(`${container} .snackbar`);

        this.$warning.on('click', 'button', () => {
            this.hideWarning();
        });
        this.$success.on('click', 'button', () => {
            this.hideSuccess();
        });
        this.$error.on('click', 'button', () => {
            this.hideError();
        });
    }

    showErrors(errors) {
        let text = '';

        errors.forEach((error) => {
            text += error + '</br>';
        });

        this.$errorMessage.html(text);
        this.$error.show();

        this.$snackbar[0].className = "showError";
    //    setTimeout(() => { this.$snackbar[0].className = this.$snackbar[0].className.replace("showError", ""); }, 5000);
    }

    showError(error) {
        this.$errorMessage.text(error);
        this.$error.show();
        this.$snackbar[0].className = "showError";
    //    setTimeout(() => { this.$snackbar[0].className = this.$snackbar[0].className.replace("showError", ""); }, 5000);
    }

    showWarning(warning) {
        this.$warningMessage.text(warning);
        this.$warning.show();
        this.$snackbar[0].className = "showWarning";
    }

    showSuccess(success) {
        this.$successMessage.text(success);
        this.$success.show();
        this.$snackbar[0].className = "show";
        setTimeout(() => { this.hideSuccess() }, 3000);
    }

    showSuccesses(success) {
        let text = '';

        success.forEach((element) => {
            text += element + '</br>'
        });

        this.$successMessage.html(text);
        this.$success.show();
        this.$snackbar[0].className = "show";
        setTimeout(() => { this.$snackbar[0].className = this.$snackbar[0].className.replace("show", ""); }, 3000);
    }

    hideError() {
        this.$error.hide();
        this.$snackbar[0].className.replace("showError", "");
    }

    hideWarning() {
        this.$warning.hide();
        this.$snackbar[0].className.replace("showWarning", "");
    }

    hideSuccess() {
        this.$success.hide();
        this.$snackbar[0].className.replace("show", "");
    }

    hide() {
        this.hideError();
        this.hideSuccess();
        this.hideWarning();
        this.$snackbar[0].className.replace("show", "");
        this.$snackbar[0].className.replace("showError", "");
        this.$snackbar[0].className.replace("showWarning", "");
    }
}