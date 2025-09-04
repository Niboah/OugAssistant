// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function ougFetch(url, method = 'GET', body = null) {
    return fetch('OugAssistant'+url,
        {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer your-token-here'
            },
            body: body ? JSON.stringify(body) : null
        })
        .then(async response => {
            if (!response.ok) {
                const errorText = await response.text(); // capturar el error del cuerpo si existe
                throw new Error(`Error ${response.status}: ${errorText || response.statusText}`);
            }

            if (response.status === 204) {
                // No Content
                return true;
            }

            const contentType = response.headers.get('content-type');
            if (contentType && contentType.includes('application/json')) {
                return response.json();
            } else {
                return response.text(); // fallback si no es JSON
            }
        })
        .catch(error => {
            console.error('ajaxCall error:', error);
            throw error; // relanzar para que lo maneje quien llamó a la función
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