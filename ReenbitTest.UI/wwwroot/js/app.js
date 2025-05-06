
// Function to get the window width for responsive drawer behavior
window.getWindowWidth = function () {
    return window.innerWidth;
};

// Add event listener for window resize to update drawer state
window.addEventListener('resize', function () {
    // This can be used if you want to call a .NET method on window resize
    // DotNet.invokeMethodAsync('YourAssemblyName', 'UpdateDrawerState', window.innerWidth);
});

window.scrollToBottom = function (elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

window.preventDefault = function (event) {
    event.preventDefault();
    return true;
};

window.blazorHelpers = {
    scrollElementToBottom: function (elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.scrollTop = element.scrollHeight;
        }
    }
};