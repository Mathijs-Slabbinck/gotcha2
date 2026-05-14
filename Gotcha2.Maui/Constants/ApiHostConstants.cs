namespace Gotcha2.Maui.Constants
{
    public static class ApiHostConstants
    {
        // Dev PC's LAN IPv4. Phone + PC must be on the same WiFi.
        // API must bind to "*:{Port}" (see launchSettings.json) so it accepts non-loopback traffic.
        // Update this if the LAN IP changes (ipconfig → Wireless LAN adapter WiFi → IPv4 Address).
        private const string Host = "192.168.2.11";
        private const int Port = 5132;
        public static string BaseUrl => $"http://{Host}:{Port}";
    }
}
