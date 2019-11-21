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
				this.password.$input.val('')
				this.confirmPassword.$input.val('')
			})
            .fail((resp) => {
                var errors = resp.responseJSON[''];
                if (errors !== null && errors !== undefined) {
                    this.statusAlert.showErrors(errors);
                }

                this.showErrors(resp.responseJSON);
				this.password.$input.val('')
				this.confirmPassword.$input.val('')
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