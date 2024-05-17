using AddressIPControlBackgroundService.DTO;
using System.Text.Json;

namespace AddressIPControlBackgroundService.Helpers
{
    public class NetManager
    {
        private readonly ILogger<Worker> _logger;
        private readonly string _fileName; 
        private readonly string _path;

        public string AddressIP { get; set; }

        public NetManager(ILogger<Worker> logger)
        {
            _logger = logger;
            _fileName = "AddressIP.json";
            _path = Path.Combine("/home/vento87/Desktop/_PublicAddress");
        }

        public async Task Load()
        {
            await GetFileAndFillProperty();
        }

        public async Task CheckMyPublicIPAddressAsync()
        {
            string publicAddress = await GetPublicIPAddressAsync();
            if (publicAddress.Equals("BRAK"))
                return;

            if (publicAddress.Equals(AddressIP))
                return;

            AddressIP = publicAddress;

            await SaveToFile(publicAddress);

            await SendEmail(publicAddress);
        }

        private async Task SendEmail(string publicAddress)
        {
            var emailSender = new EmailSender(_logger);

            string smtpServer = "smtp.office365.com";
            int port = 587;
            string fromEmail = "mail";
            string password = "hasło";
            string toEmail = "1987vento@gmail.com";
            string subject = $"[Home-BOT] [UWAGA] Twój adres IP został zmieniony!";
            string body = $"Twój nowy adres IP: {publicAddress}";

            await emailSender.SendEmailAsync(smtpServer, port, fromEmail, password, toEmail, subject, body);
        }

        private async Task<string> GetPublicIPAddressAsync()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    // Wysłanie żądania do serwisu ipify.org
                    HttpResponseMessage response = await httpClient.GetAsync("https://api.ipify.org");
                    response.EnsureSuccessStatusCode();
                    string publicIp = await response.Content.ReadAsStringAsync();
                    return publicIp;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Błąd podczas sprawdzania adresu IP: {ex.Message}");
                    return "BRAK";
                }
            }
        }

        private async Task GetFileAndFillProperty()
        {
            string filePath = Path.Combine(_path, _fileName);

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            if (!File.Exists(filePath))
            {
                string myIPAddress = await GetPublicIPAddressAsync();
                NetDTO model = new NetDTO()
                {
                    AddressIP = myIPAddress
                };

                AddressIP = model.AddressIP;

                string jsonString = JsonSerializer.Serialize(model);

                await File.WriteAllTextAsync(filePath, jsonString);
                _logger.LogInformation($"Utworzono plik {filePath}, w pliku zapisano adres IP: {myIPAddress}");
            }
            else
            {
                string jsonString = await File.ReadAllTextAsync(filePath);

                var model = JsonSerializer.Deserialize<NetDTO>(jsonString);
                if (model == null)
                    return;

                AddressIP = model.AddressIP;
                _logger.LogInformation($"Odczytano plik {filePath}");
            }
        }

        private async Task SaveToFile(string addressIP)
        {
            string filePath = Path.Combine(_path, _fileName);

            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            NetDTO model = new NetDTO()
            {
                AddressIP = addressIP
            };

            string jsonString = JsonSerializer.Serialize(model);

            await File.WriteAllTextAsync(filePath, jsonString);

            _logger.LogInformation($"Zapisano zmiany w pliku: adres IP: {addressIP}, plik: {filePath}");
        }
    }
}
