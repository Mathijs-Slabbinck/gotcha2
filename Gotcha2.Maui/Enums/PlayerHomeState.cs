namespace Gotcha2.Maui.Enums
{
    // Drives which of the three sibling layouts is visible on the PlayerHome page.
    // Loading covers the initial-load and refresh windows where nothing should render yet.
    public enum PlayerHomeState
    {
        Loading,
        Pending,
        Active,
        Past
    }
}
