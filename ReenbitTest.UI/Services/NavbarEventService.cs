namespace ReenbitTest.UI.Services
{
    public class NavbarEventService
    {
        public event Action? OnNavbarToggle;

        public void ToggleNavbar()
        {
            OnNavbarToggle?.Invoke();
        }
    }
}