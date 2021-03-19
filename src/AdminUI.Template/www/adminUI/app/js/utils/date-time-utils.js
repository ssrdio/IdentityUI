var DateTimeUtils = {
    dateTimeFormat: function () {
        return "D.M.YYYY HH:mm:ss"
    },

    toDisplayDateTime: function (date) {
        if (date === null || date === undefined) {
            return '';
        }

        return moment(date).format(this.dateTimeFormat());
    }
}