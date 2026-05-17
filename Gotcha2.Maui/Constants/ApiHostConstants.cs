namespace Gotcha2.Maui.Constants
{
    public static class ApiHostConstants
    {
        /* VS Dev Tunnel URL — stable across networks, works for Windows + Android emulator + physical phone.
         * Requires the API to be running with the matching launch profile and the tunnel to be active in VS.
         * If you recreate the tunnel (or it's non-persistent), update this URL. */
        public static string BaseUrl => "https://g64ps2vr-7278.euw.devtunnels.ms";
    }
}
