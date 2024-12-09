using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyTicket.Application.Interfaces;
using MyTicket.Domain.Entities.Payment;
using Project.Application.Businesses.Payment.Models;

namespace Project.Application.Businesses.Payment.Commands
{
    public class VerifyMidtransPaymentCommand: IRequest<string>
    {
        public string transaction_time { get; set; }
        public string transaction_status { get; set; }
        public string transaction_id { get; set; }
        public string status_message { get; set; }
        public string status_code { get; set; }
        public string signature_key { get; set; }
        public string payment_type { get; set; }
        public string order_id { get; set; }
        public string merchant_id { get; set; }
        public string masked_card { get; set; }
        public string gross_amount { get; set; }
        public string fraud_status { get; set; }
        public string eci { get; set; }
        public string currency { get; set; }
        public string channel_response_message { get; set; }
        public string channel_response_code { get; set; }
        public string card_type { get; set; }
        public string bank { get; set; }
        public string approval_code { get; set; }
    }

    public class VerifyMidtransPaymentCommandHandler : IRequestHandler<VerifyMidtransPaymentCommand, string>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        private readonly IConfiguration _configuration;
        private static readonly string[] _acceptedTransaction = new string[] { "settlement", "capture" };
        public VerifyMidtransPaymentCommandHandler(
                IMyTicketDbContext dbContext, 
                IContext context, 
                IConfiguration configuration
            )
        {
            _dbContext = dbContext;
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> Handle(VerifyMidtransPaymentCommand request, CancellationToken cancellationToken)
        {
            var paymentHistory = await _dbContext.PaymentHistories
                    .Where(p => p.Id == Guid.Parse(request.order_id) && p.Status == PaymentStatus.WaitingForPayment)
                    .FirstOrDefaultAsync();
            if (paymentHistory == null) { throw new InvalidOperationException("Project payment not found"); }

            var paymentKey =
                paymentHistory.Id.ToString() +
                "200" +
                paymentHistory.PricePayment.ToString("0.00").Replace(",", ".") +
                _configuration["MidransSettings:ServerKey"];
            var byteKey = Encoding.UTF8.GetBytes(paymentKey);

            string hashedKey = "";
            using (SHA512 hmac = SHA512.Create())
            {
                var byteHashedKey = hmac.ComputeHash(byteKey);
                foreach (byte b in byteHashedKey)
                {
                    hashedKey += string.Format("{0:x2}", b);
                }

                if (hashedKey != request.signature_key ||
                !_acceptedTransaction.Any(p => p == request.transaction_status) ||
                request.fraud_status != "accept")
                {
                    throw new Exception("Transaction failed! " + request.status_message);
                }
            }

            //Get status from Midtrans
            var textBytes = System.Text.Encoding.UTF8.GetBytes(_configuration["MidransSettings:ServerKey"]);
            var authString = System.Convert.ToBase64String(textBytes);
            var client = new HttpClient();
            var httpRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(string.Format(_configuration["MidransSettings:CheckStatusUri"], paymentHistory.Id.ToString())),
                Headers =
                    {
                        { "accept", "application/json" },
                        { "authorization", "Basic "+authString },
                    },

            };

            using (var response = await client.SendAsync(httpRequest))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                try
                {
                    var result = JsonSerializer.Deserialize<VerifyPaymentMidtrans>(body);

                    if (hashedKey != request.signature_key ||
                        !_acceptedTransaction.Any(p => p == request.transaction_status) ||
                        request.fraud_status != "accept")
                    {
                        throw new Exception("Transaction failed! " + result.status_message);
                    }

                    paymentHistory.Status = PaymentStatus.PaymentComplete;
                    _dbContext.PaymentHistories.Update(paymentHistory);
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    return hashedKey;
                }
                catch
                {
                    throw;
                }
            }   
        }
    }
}