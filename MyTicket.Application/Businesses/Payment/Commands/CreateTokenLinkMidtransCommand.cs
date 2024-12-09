using System.Net.Http.Headers;
using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyTicket.Application.Interfaces;
using MyTicket.Domain.Entities.Payment;
using Project.Application.Businesses.Payment.Models;

namespace Project.Application.Businesses.Payment.Commands
{
    public class CreateTokenLinkMidtransCommand: IRequest<PaymentMidtransResponse>
    {
        public string OrderId { get; set; }
    }

    public class CreateTokenLinkMidtransCommandHandler : IRequestHandler<CreateTokenLinkMidtransCommand, PaymentMidtransResponse>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        private readonly IConfiguration _configuration;
        public CreateTokenLinkMidtransCommandHandler(
                IMyTicketDbContext dbContext, 
                IContext context, 
                IConfiguration configuration
            )
        {
            _dbContext = dbContext;
            _context = context;
            _configuration = configuration;
        }

        public async Task<PaymentMidtransResponse> Handle(CreateTokenLinkMidtransCommand request, CancellationToken cancellationToken)
        {
            var paymentHistory = await _dbContext.PaymentHistories
                    .Where(p => p.Id == Guid.Parse(request.OrderId) && p.Status == PaymentStatus.WaitingForPayment)
                    .FirstOrDefaultAsync();

            if (paymentHistory == null) { throw new InvalidOperationException($"Project payment not found"); }

            var textBytes = System.Text.Encoding.UTF8.GetBytes(_configuration["MidransSettings:ServerKey"]);
            var authString = System.Convert.ToBase64String(textBytes);
            var client = new HttpClient();
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_configuration["MidransSettings:Uri"]),
                Headers =
                    {
                        { "accept", "application/json" },
                        { "authorization", "Basic "+authString },
                    },
                Content =
                new StringContent("{\"" +
                    "transaction_details\":{" +
                        "\"order_id\":\"" + request.OrderId + "\"," +
                        "\"gross_amount\":" + paymentHistory.PricePayment + "" +
                    "}," +
                        "\"credit_card\":{" +
                            "\"secure\":true" +
                        "}" +
                    "}")
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };

            using (var response = await client.SendAsync(httpRequest))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                try
                {
                    var result = JsonSerializer.Deserialize<PaymentMidtransResponse>(body);
                    paymentHistory.Token = result.token;
                    paymentHistory.RedirectUrl = result.redirect_url;

                    _dbContext.PaymentHistories.Update(paymentHistory);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    return result;
                }
                catch
                {
                    throw;
                }
            }
        }
    }
}