class InviteToGroupModel {
    constructor(groupId, canAssignRoles, successCallback) {
        this.groupId = groupId;
        this.canAssignRoles = canAssignRoles;
        this.successCallback = successCallback;

        this.$inviteUserModal = $('#invite-user-modal');
        this.$inviteUserModal.on('hidden.bs.modal', () => {
            this.emailInputComponent.value(null);
            this.rolesSelectComponent.selectOption(null);
            this.rolesSelectComponent.triggerChange();

            this.hideErrors();
        });
        this.$inviteUserModal.on('click', 'button.confirm', () => {
            this.add();
        });

        const $form = this.$inviteUserModal.find('#invite-user-form');
        this.emailInputComponent = new InputComponent($form, '#email-input');

        this.$rolesSelect = $form.find('#group-role-select .select2-container')
        this.rolesSelectComponent = new SelectComponent($form, '#group-role-select');

        this.errorAlert = new ErrorAlert($form);

        this.initRolesSelct();
    }

    initRolesSelct() {
        let data = this.canAssignRoles.map((element) => {
            return {
                id: element.id,
                text: element.name
            }
        });

        this.$rolesSelect.select2({
            data: data,
            placeholder: 'Select Group Role'
        });

        this.rolesSelectComponent.selectOption(null);
    }

    showModal() {
        this.$inviteUserModal.modal('show');
    }

    showErrors(errors) {
        if (errors[''] !== null && errors[''] !== undefined) {
            this.errorAlert.showErrors(errors['']);
        }

        this.emailInputComponent.showError(errors.Email);
        this.rolesSelectComponent.showError(errors.GroupRoleId);
    }

    hideErrors() {
        this.errorAlert.hide();

        this.emailInputComponent.hideError();
        this.rolesSelectComponent.hideError();
    }

    getData() {
        return {
            email: this.emailInputComponent.value(),
            groupRoleId: this.rolesSelectComponent.value(),
        }
    }

    add() {
        this.hideErrors();
        const data = this.getData();

        Api.post(`/IdentityAdmin/Group/${this.groupId}/GroupInvite/Invite`, data)
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