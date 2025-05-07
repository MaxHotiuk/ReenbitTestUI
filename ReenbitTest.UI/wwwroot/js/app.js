
// Function to get the window width for responsive drawer behavior
window.getWindowWidth = function () {
    return window.innerWidth;
};

window.addEventListener('resize', function () {
    // This can be used if you want to call a .NET method on window resize
    // DotNet.invokeMethodAsync('YourAssemblyName', 'UpdateDrawerState', window.innerWidth);
});

window.preventDefault = function (event) {
    event.preventDefault();
    return true;
};
window.scrollFunctions = {
    scrollToBottom: function () {
        window.scrollTo({
            top: document.body.scrollHeight,
            behavior: 'smooth'
        });
    },
    
    getWindowWidth: function () {
        return window.innerWidth;
    }
};

window.keyboardFunctions = {
    initialize: function() {
        if ('visualViewport' in window) {
            // Modern approach using Visual Viewport API
            window.visualViewport.addEventListener('resize', this.handleViewportResize);
        } else {
            // Fallback for older browsers
            window.addEventListener('resize', this.handleWindowResize);
        }
    },
    
    handleViewportResize: function() {
        // This is more accurate for keyboard appearance
        const viewport = window.visualViewport;
        const windowHeight = window.innerHeight;
        
        // If viewport height is significantly less than window height, keyboard is likely open
        if (viewport.height < windowHeight * 0.8) {
            document.body.classList.add('keyboard-open');
            
            // Ensure the input field is in view
            const messageInput = document.querySelector('.message-input');
            if (messageInput) {
                setTimeout(() => {
                    messageInput.scrollIntoView({ behavior: 'smooth', block: 'end' });
                }, 100);
            }
        } else {
            document.body.classList.remove('keyboard-open');
        }
    },
    
    handleWindowResize: function() {
        // Fallback method - less reliable but works on older devices
        const currentHeight = window.innerHeight;
        const originalHeight = window.outerHeight;
        
        if (currentHeight < originalHeight * 0.8) {
            document.body.classList.add('keyboard-open');
        } else {
            document.body.classList.remove('keyboard-open');
        }
    },
    
    cleanup: function() {
        if ('visualViewport' in window) {
            window.visualViewport.removeEventListener('resize', this.handleViewportResize);
        } else {
            window.removeEventListener('resize', this.handleWindowResize);
        }
    }
};