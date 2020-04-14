class session {
  constructor(modalContainer, sessionContainer, userId) {
    this.userId = userId;

    this.table = $('#sessionsTable').DataTable({
      serverSide: true,
      processing: true,
      columnDefs: [{ width: '120px', targets: [3] }],
      targets: 'no-sort',
      bSort: false,
      order: [],
      ajax: {
        url: `/IdentityAdmin/User/GetSessions/${this.userId}`,
        type: 'GET'
      },
      columns: [
        {
            data: 'ip',
            title: 'IP',
            render: $.fn.dataTable.render.text()
        },
        {
            data: 'created',
            title: 'Created',
            render: $.fn.dataTable.render.text()
        },
        {
            data: 'lastAccess',
            title: 'Last Access',
            render: $.fn.dataTable.render.text()
        },
        {
          data: 'id',
          defaultContent: '',
          mRender: function(id) {
              return `<div class="text-center pr-2"><button class='btn btn-primary table-button table-button-red logoutSession' data-id='${id}'">Logout user</button></div>`;
          }
        }
      ],
      //language: {
      //  //Custom loading indicator
      //  loadingRecords: "<i class='fa fa-spinner fa-spin'></i>",
      //  processing: "<div class='loader'></div>"
      //}
    });

    this.confirmationModal = new conformationModal(
      $(`#${modalContainer}`),
      onYesClick => {
        if (onYesClick === null || onYesClick === undefined) {
          return;
        }

        if (onYesClick.key === 'logoutUser') {
          this.logoutUser();
        } else if (onYesClick.key === 'logoutSession') {
          this.logoutSession(onYesClick.id);
        }
      }
    );

    $(`#${sessionContainer} div.logoutUser`).click(() => {
      this.confirmationModal.show(
        { key: 'logoutUser' },
        'Logout all user sessions'
      );
    });

    $(`#${sessionContainer}`).on('click', 'button.logoutSession', event => {
      const id = $(event.target).data('id');

      this.confirmationModal.show(
        { key: 'logoutSession', id: id },
        'Logout session'
      );
    });

    this.errorModal = new errorModal($(`#${modalContainer}`));
  }

  logoutUser() {
    var data = { userId: this.userId };

    Api.post(`/IdentityAdmin/User/LogoutUser`, data)
      .done(() => {
        this.table.clear().draw();
      })
      .fail(resp => {
        if (resp === undefined || resp === null) {
          this.errorModal.show('error');
        } else {
          const error = resp.responseJSON[''];

          this.errorModal.show(error);
        }
      });
  }

  logoutSession(id) {
    var data = { userId: this.userId, sessionId: id };

    Api.post('/IdentityAdmin/User/LogoutSession', data)
      .done(() => {
        this.table.clear().draw();
      })
      .fail(resp => {
        if (resp === undefined || resp === null) {
          this.errorModal.show('error');
        } else {
          const error = resp.responseJSON[''];

          this.errorModal.show(error);
        }
      });
  }
}
