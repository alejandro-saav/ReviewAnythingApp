// wwwroot/js/app.js

window.blazorHelpers = {
    preventEnterKeySubmission: function(elementId) {
        const inputElement = document.getElementById(elementId);
        if (inputElement) {
            inputElement.addEventListener('keydown', function(event) {
                if (event.key === 'Enter') {
                    event.preventDefault();
                }
            },);
        }
    },
};

// Resize textarea when it gets wider
window.autoResizeTextarea = function(id) {
    const textarea = document.getElementById(id);
    if (!textarea) return;
    textarea.style.height = 'auto';
    textarea.style.height = textarea.scrollHeight + 'px';
    textarea.addEventListener('input', () => {
        textarea.style.height = 'auto';
        textarea.style.height = textarea.scrollHeight + 'px';
    },);

    // Store reference for future reset
    window._autoResizeTextareaElement = textarea;
};

// Reset size of text area
window.resetTextareaHeight = function () {
    const textarea = window._autoResizeTextareaElement;
    if (textarea) {
        textarea.style.height = ''; // Resets to default height (usually one line)
    }
};

// window.googleAuth = {
//     renderButton: function(dotNetHelper) {
//         google.accounts.id.initialize({
//             client_id: "",
//             callback: function(response) {
//                 dotNetHelper.invokeMethodAsync("GoogleSignInSucceeded", response.credential);
//             }
//         });
//        
//         google.accounts.id.renderButton(document.getElementById("googleButtonDiv"), { theme: "outline", size: "large"});
//     }
// }

