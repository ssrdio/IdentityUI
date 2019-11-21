var Api = {
    get: function (url, token) {
        return $.ajax({
            type: 'GET',
            url: url,
            headers: {
                RequestVerificationToken: token
            },
            contentType: 'application/json'
        });
    },

    post: function (url, data, token) {
        return $.ajax({
            type: 'POST',
            url: url,
            headers: {
                RequestVerificationToken: token
            },
            contentType: 'application/json',
            data: JSON.stringify(data)
        });
    }
}