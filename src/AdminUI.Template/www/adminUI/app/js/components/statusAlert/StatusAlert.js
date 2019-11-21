class StatusAlertComponent {
    constructor(container) {
        this.$successMessage = $(`${container} .alert-success .alertMessage`);
        this.$errorMessage = $(`${container} .alert-danger .alertMessage`);
        this.$success = $(`${container} .alert-success`);
        this.$error = $(`${container} .alert-danger`);
        this.$snackbar = $(`${container} .snackbar`);
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

    showSuccess(success) {
        this.$successMessage.text(success);
        this.$success.show();
        this.$snackbar[0].className = "show";
        setTimeout(() => { this.$snackbar[0].className = this.$snackbar[0].className.replace("show", ""); }, 3000);
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

    hideSuccess() {
        this.$success.hide();
        this.$snackbar[0].className.replace("show", "");
    }

    hide() {
        this.hideError();
        this.hideSuccess();
        this.$snackbar[0].className.replace("show", "");
        this.$snackbar[0].className.replace("showError", "");
    }
}