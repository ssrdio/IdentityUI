class InviteUserModal {
    constructor(groupId, addUrl, successCallback) {
        this.groupId = groupId;
        this.successCallback = successCallback;
        this.addUrl = addUrl;

        this.$inviteUserModal = $('#invite-user-modal');
        this.$inviteUserModal.on('hidden.bs.modal', () => {
            this.userSelectComponent.value(null);

            this.hideErrors();
        });
        this.$inviteUserModal.on('click', 'button.confirm', () => {
            this.add();
        });

        const $form = this.$inviteUserModal.find('#invite-user-form');
        this.emailInputComponent = new InputComponent($form, '#email-input')

        this.errorAlert = new ErrorAlert($form);
    }

    showModal() {
        this.$inviteUserModal.modal('show');
    }

    showErrors(errors) {
        if (errors[''] !== null && errors[''] !== undefined) {
            this.errorAlert.showErrors(errors['']);
        }

        this.emailInputComponent.showError(errors.email);
    }

    hideErrors() {
        this.errorAlert.hide();

        this.emailInputComponent.hideError();
    }

    getData() {
        return {
            email: this.emailInputComponent.value(),
            groupId: this.groupId,
        }
    }

    add() {
        this.hideErrors();
        const data = this.getData();

        Api.post(this.addUrl, data)
            .done(() => {
                this.$inviteUserModal.modal('hide');

                if (this.successCallback) {
                    this.successCallback();
                }
            })
            .fail((resp) => {
                this.showErrors(resp.responseJSON);
            })
    }
}