class SendTestEmailModal {
    constructor(emailId, email, successCallback) {
        this.emailId = emailId;
        this.email = email;
        this.successCallback = successCallback;

        this.$sendTesEmailModal = $('#send-test-email-modal');
        this.$sendTesEmailModal.on('hidden.bs.modal', () => {
            this.hideErrors();
        });

        const $sendTestEmailForm = this.$sendTesEmailModal.find('#send-test-email-form');
        this.emailInputComponent = new InputComponent($sendTestEmailForm, '#email-input');

        this.errorAlert = new ErrorAlert($sendTestEmailForm);

        $('#send-test-email').on('click', () => {
            this.$sendTesEmailModal.modal('show');
        });

        this.$sendTesEmailModal.on('click', 'button.confirm', () => {
            this.sendTestEmail();
        });
    }

    showErrors(errors) {
        if (errors[''] !== null && errors[''] !== undefined) {
            this.errorAlert.showErrors(errors['']);
        }

        this.emailInputComponent.showError(errors.Email);
    }

    hideErrors() {
        this.errorAlert.hide();
        this.emailInputComponent.hideError();
    }

    getData() {
        return {
            email: this.emailInputComponent.value()
        };
    }

    sendTestEmail() {
        this.hideErrors();
        const data = this.getData();

        Api.post(`/IdentityAdmin/Setting/Email/SendTest/${this.emailId}`, data)
            .done(() => {
                this.$sendTesEmailModal.modal('hide');
                if (this.successCallback) {
                    this.successCallback();
                }
            })
            .fail((resp) => {
                this.emailInputComponent.value(null);
                this.showErrors(resp.responseJSON);
            });
    }
}