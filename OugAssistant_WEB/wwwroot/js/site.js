// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function ajaxCall(url, method = 'GET', body = null) {
    return fetch(url,
        {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer your-token-here'
            },
            body: body ? JSON.stringify(body) : null
        })
        .then(response => {
            if (!response.ok)  throw response;
            else  return response.json();
            });

}