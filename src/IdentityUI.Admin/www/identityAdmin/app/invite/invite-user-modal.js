class InviteUserModal {
    constructor(successCallback) {
        this.successCallback = successCallback;

        this.$inviteUserModal = $('#invite-user-modal');
        this.$inviteUserModal.on('hidden.bs.modal', () => {
            this.emailInputComponent.value(null);

            this.rolesSelectComponent.selectOption(null);
            this.rolesSelectComponent.triggerChange();
            this.groupSelectComponent.selectOption(null);
            this.groupSelectComponent.triggerChange();
            this.groupRolesSelectComponent.selectOption(null);
            this.groupRolesSelectComponent.triggerChange();

            this.hideErrors();
        });
        this.$inviteUserModal.on('click', 'button.confirm', () => {
            this.add();
        });

        const $form = this.$inviteUserModal.find('#invite-user-form');
        this.emailInputComponent = new InputComponent($form, '#email-input');

        this.$rolesSelect = $form.find('#role-select .select2-container')
        this.rolesSelectComponent = new SelectComponent($form, '#role-select');

        this.$groupSelect = $form.find('#group-select .select2-container')
        this.groupSelectComponent = new SelectComponent($form, '#group-select');

        this.$groupRolesSelect = $form.find('#group-role-select .select2-container')
        this.groupRolesSelectComponent = new SelectComponent($form, '#group-role-select');

        this.initRoleSelect();
        this.initGroupSelect();
        this.initGroupRoleSelect();

        this.errorAlert = new ErrorAlert($form);
    }

    initRoleSelect() {
        this.$rolesSelect.select2({
            ajax: {
                url: `/IdentityAdmin/Invite/GetRoles`,
                type: 'GET',
                dataType: 'json',
                delay: 250
            }
        });
    }

    initGroupSelect() {
        this.$groupSelect.select2({
            ajax: {
                url: `/IdentityAdmin/Invite/GetGroups`,
                type: 'GET',
                dataType: 'json',
                delay: 250
            }
        });
    }

    initGroupRoleSelect() {
        this.$groupRolesSelect.select2({
            ajax: {
                url: `/IdentityAdmin/Invite/GetGroupRoles`,
                type: 'GET',
                dataType: 'json',
                delay: 250
            }
        });
    }

    showModal() {
        this.$inviteUserModal.modal('show');
    }

    showErrors(errors) {
        if (errors[''] !== null && errors[''] !== undefined) {
            this.errorAlert.showErrors(errors['']);
        }

        this.emailInputComponent.showError(errors.Email);
        this.rolesSelectComponent.showError(errors.RoleId);
        this.groupSelectComponent.showError(errors.GroupId);
        this.groupRolesSelectComponent.showError(errors.GroupRoleId);
    }

    hideErrors() {
        this.errorAlert.hide();

        this.emailInputComponent.hideError();
        this.rolesSelectComponent.hideError();
        this.groupSelectComponent.hideError();
        this.groupRolesSelectComponent.hideError();
    }

    getData() {
        return {
            email: this.emailInputComponent.value(),
            roleId: this.rolesSelectComponent.value(),
            groupId: this.groupSelectComponent.value(),
            groupRoleId: this.groupRolesSelectComponent.value(),
        }
    }

    add() {
        this.hideErrors();
        const data = this.getData();

        Api.post(`/IdentityAdmin/Invite/Add`, data)
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