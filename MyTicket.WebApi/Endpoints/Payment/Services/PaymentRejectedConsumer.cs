using Microsoft.AspNetCore.Identity.UI.Services;
using MyTicket.WebApi.Endpoints.Payment.Models.Service;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MyTicket.WebApi.Endpoints.Payment.Services;

public class PaymentRejectedConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ConnectionFactory _factory;

    public PaymentRejectedConsumer(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "MyUser",
            Password = "MyPassword",
            VirtualHost = "/"
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();
        
        channel.QueueDeclare(queue: "payment-rejected", durable: false, exclusive: false, autoDelete: true, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {
            using (var scope = _serviceProvider.CreateScope())  // Membuat scope baru
            {
                var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();  // Mengambil IEmailSender scoped service

                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var rejectedPaymentRequest = JsonConvert.DeserializeObject<PaymentConsumerModel>(message);

                if (rejectedPaymentRequest != null && rejectedPaymentRequest.Email != null)
                {
                    var toEmail = rejectedPaymentRequest.Email;
                    var subject = "Information from MyTicket";
                    var bodyText = $"Hello {rejectedPaymentRequest.FullName}, event {rejectedPaymentRequest.EventName} - ticket ({rejectedPaymentRequest.TicketNumber}) has been rejected because of {rejectedPaymentRequest.RejectedReason}!";

                    // Kirim email
                    await emailSender.SendEmailAsync(toEmail, subject, bodyText);
                }
            }
        };

        channel.BasicConsume(queue: "payment-rejected", autoAck: true, consumer: consumer);

        // Ini akan menjaga listener tetap berjalan sampai service dihentikan
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);  // Delay kecil untuk menghindari polling intensif
        }
    }
}