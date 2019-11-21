class SetNewPassword {
    constructor(userId) {
        this.password = new InputComponent($('#resetPasswordForm'), '#password');
        this.confirmPassword = new InputComponent($('#resetPasswordForm'), '#confirmPassword');

        this.statusAlert = new StatusAlertComponent('#credentialsContainer');

        $('#resetPasswordForm button.submit').click(() => {
            this.resetPassword(userId);
        });
    }

    getData() {
        return {
            password: this.password.value(),
            confirmPassword: this.confirmPassword.value(),
        }
    }

    resetPassword(userId) {
        const data = this.getData();
        const url = `/IdentityAdmin/User/SetNewPassword/${userId}`;

        this.hideErrors();
        this.statusAlert.hide();

        Api.post(url, data)
            .done(() => {
                this.statusAlert.showSuccess("Password changed");
            })
            .fail((resp) => {
                var erros = resp.responseJSON[''];
                console.log(erros);
                if (erros !== null && erros !== undefined) {
                    this.statusAlert.showError(resp.responseJSON);   
                }

                this.showErrors(resp.responseJSON);
            });
    }

    showErrors(resp) {
        this.password.showError(resp.Password);
        this.confirmPassword.showError(resp.ConfirmPassword);
    }

    hideErrors() {
        this.password.hideError();
        this.confirmPassword.hideError();
    };
}