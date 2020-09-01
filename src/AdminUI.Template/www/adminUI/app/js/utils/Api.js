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
    },

    put: function (url, data, token) {
        return $.ajax({
            type: 'PUT',
            url: url,
            headers: {
                RequestVerificationToken: token
            },
            contentType: 'application/json',
            data: JSON.stringify(data)
        });
    },

    delete: function (url, token) {
        return $.ajax({
            type: 'DELETE',
            url: url,
            headers: {
                RequestVerificationToken: token
            },
            contentType: 'application/json'
        });
    },

    upload: function (url, data, token) {
        var form = new FormData();
        form.append("file", data);

        return $.ajax({
            type: 'POST',
            url: url,
            contentType: false,
            processData: false,
            headers: {
                RequestVerificationToken: token
            },
            data: form
        });
    },
}