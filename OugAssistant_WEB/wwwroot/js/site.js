// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function ajaxCall(url, method = 'GET', body = null) {
    return fetch('OugAssistant'+url,
        {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer your-token-here'
            },
            body: body ? JSON.stringify(body) : null
        })
        .then(response => {
            if (!response.ok) throw "AjaxCall fail";
            else if (response.status == '204') return true;
            else if (response.bodyUsed) return response.json();
            else return response.json();
        });
}

function changeTheme() {
    if(document.documentElement.getAttribute('data-bs-theme')=='dark')
        document.documentElement.setAttribute('data-bs-theme', 'light');
    else document.documentElement.setAttribute('data-bs-theme', 'dark');
}

(() => {
    document.getElementById('changeThemeBtn').addEventListener('click', changeTheme);
})();