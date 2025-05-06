namespace ReenbitTest.UI.Services
{
    public class NavbarEventService
    {
        // Event to notify when navbar should be toggled
        public event Action? OnNavbarToggle;

        // Method to trigger the event
        public void ToggleNavbar()
        {
            OnNavbarToggle?.Invoke();
        }
    }
}