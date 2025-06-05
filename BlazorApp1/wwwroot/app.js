// wwwroot/js/app.js

window.blazorHelpers = {
    preventEnterKeySubmission: function(elementId) {
        const inputElement = document.getElementById(elementId);
        if (inputElement) {
            inputElement.addEventListener('keydown', function(event) {
                if (event.key === 'Enter') {
                    event.preventDefault();
                }
            });
        }
    }
};