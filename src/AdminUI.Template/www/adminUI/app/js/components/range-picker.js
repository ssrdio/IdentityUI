class RangePicker {
    constructor($container, statusAlert, onChange) {
        this.statusAlert = statusAlert;

        this.$dateTimeRangePicker = $container.find('.date-time-range-picker');
        this.$dateTimeFrom = $container.find('.date-time-from-picker');
        this.$dateTimeTo = $container.find('.date-time-to-picker');

        this.onChange = onChange;

        this.init();
    }

    showError(error) {
        if (this.statusAlert !== undefined && this.statusAlert !== null) {
            this.statusAlert.showError(error);
        }
    }

    hideErrors() {
        if (this.statusAlert !== undefined && this.statusAlert !== null) {
            this.statusAlert.hide();
        }
    }

    getFrom() {
        if (this.from === null || this.from === undefined) {
            return null;
        }

        return moment(this.from.format()).utc().format();
    }

    getTo() {
        if (this.to === null || this.to === undefined) {
            return null;
        }

        return moment(this.to.format()).utc().format();
    }

    change() {
        if (this.onChange === null || this.onChange === undefined) {
            return;
        }

        this.onChange();
    }

    reset() {
        this.from = null;
        this.to = null;

        this.$dateTimeFrom.val('');
        this.$dateTimeTo.val('');

        this.$dateTimeFrom.data('daterangepicker').setStartDate(moment());
        this.$dateTimeFrom.data('daterangepicker').setEndDate(moment());

        this.$dateTimeTo.data('daterangepicker').setStartDate(moment());
        this.$dateTimeTo.data('daterangepicker').setEndDate(moment());

        this.$dateTimeRangePicker.data('daterangepicker').setStartDate(moment());
        this.$dateTimeRangePicker.data('daterangepicker').setEndDate(moment());

        this.hideErrors();

        this.onChange();
    }

    init() {
        this.$dateTimeRangePicker.daterangepicker({
            timePicker: true,
            timePicker24Hour: true,
            autoUpdateInput: true,
            alwaysShowCalendars: true,
            locale: {
                format: 'DD.MM.YYYY HH:mm',
                cancelLabel: 'Reset'
            },
            ranges: {
                'Today': [moment(), moment()],
                'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                'This Month': [moment().startOf('month'), moment().endOf('month')],
                'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
            }
        });

        this.$dateTimeFrom.daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            timePicker: true,
            timePicker24Hour: true,
            autoUpdateInput: false,
            locale: {
                format: 'DD.MM.YYYY HH:mm',
                cancelLabel: 'Reset'
            },
        });

        this.$dateTimeTo.daterangepicker({
            singleDatePicker: true,
            showDropdowns: true,
            timePicker: true,
            timePicker24Hour: true,
            autoUpdateInput: false,
            locale: {
                format: 'DD.MM.YYYY HH:mm',
                cancelLabel: 'Reset'
            },
        });

        this.$dateTimeRangePicker.on('apply.daterangepicker', (ev, picker) => {
            this.hideErrors();

            this.$dateTimeFrom.val(picker.startDate.format('DD.MM.YYYY HH:mm'));
            this.$dateTimeFrom.data('daterangepicker').setStartDate(picker.startDate);
            this.$dateTimeFrom.data('daterangepicker').setEndDate(picker.startDate);
            this.from = picker.startDate;

            this.$dateTimeTo.val(picker.endDate.format('DD.MM.YYYY HH:mm'));
            this.$dateTimeTo.data('daterangepicker').setStartDate(picker.endDate);
            this.$dateTimeTo.data('daterangepicker').setEndDate(picker.endDate);
            this.to = picker.endDate;

            this.change();
        });

        this.$dateTimeRangePicker.on('cancel.daterangepicker', (ev, picker) => {
            this.hideErrors();

            this.$dateTimeFrom.val('');
            this.$dateTimeTo.val('');

            this.$dateTimeRangePicker.data('daterangepicker').setStartDate(moment());
            this.$dateTimeRangePicker.data('daterangepicker').setEndDate(moment());

            this.from = null;
            this.to = null;

            this.change();
        });

        this.$dateTimeFrom.on('apply.daterangepicker', (ev, picker) => {
            this.hideErrors();

            if (this.to !== undefined && this.to !== null && this.to < picker.startDate) {
                this.$dateTimeFrom.data('daterangepicker').setStartDate(this.from);
                this.$dateTimeFrom.data('daterangepicker').setEndDate(this.from);

                this.showError(`From can not be smaller than to`);
            }
            else {
                this.$dateTimeFrom.val(picker.startDate.format('DD.MM.YYYY HH:mm'));
                this.$dateTimeRangePicker.data('daterangepicker').setStartDate(picker.startDate);
                this.from = picker.startDate;

                this.change();
            }
        });

        this.$dateTimeFrom.on('cancel.daterangepicker', (ev, picker) => {
            this.$dateTimeFrom.val('');
            this.hideErrors();

            this.$dateTimeFrom.data('daterangepicker').setStartDate(moment());
            this.$dateTimeFrom.data('daterangepicker').setEndDate(moment());

            this.$dateTimeRangePicker.data('daterangepicker').setStartDate(this.$dateTimeRangePicker.data('daterangepicker').endDate);

            this.from = null;

            this.change();
        });

        this.$dateTimeTo.on('apply.daterangepicker', (ev, picker) => {
            this.hideErrors();

            if (this.from !== undefined && this.from !== null && picker.startDate < this.from) {
                this.$dateTimeTo.data('daterangepicker').setStartDate(this.from);
                this.$dateTimeTo.data('daterangepicker').setEndDate(this.from);

                this.showError(`To can not be smaller than from`);
            }

            this.$dateTimeTo.val(picker.startDate.format('DD.MM.YYYY HH:mm'));
            this.$dateTimeRangePicker.data('daterangepicker').setEndDate(picker.startDate);
            this.to = picker.startDate;

            this.change();
        });


        this.$dateTimeTo.on('cancel.daterangepicker', (ev, picker) => {
            this.hideErrors();
            this.$dateTimeTo.val('');

            this.$dateTimeTo.data('daterangepicker').setStartDate(moment());
            this.$dateTimeTo.data('daterangepicker').setEndDate(moment());

            this.$dateTimeRangePicker.data('daterangepicker').setEndDate(this.$dateTimeRangePicker.data('daterangepicker').startDate);

            this.to = null;

            this.change();
        });
    }
}