using MediatR;
using MyTicket.Application.Interfaces;
using MyTicket.Domain.Entities.Payment;

namespace Project.Application.Businesses.Payment.Commands
{
    public class CreatePaymentHistoryCommand: IRequest<string>
    {
        public Guid UserId { get; set; }
        public Guid OrderTicketId { get; set; }
        public decimal PricePayment { get; set; }
    }

    public class CreatePaymentHistoryCommandHandler : IRequestHandler<CreatePaymentHistoryCommand, string>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        public CreatePaymentHistoryCommandHandler(IMyTicketDbContext dbContext, IContext context)
        {
            _dbContext = dbContext;
            _context = context;
        }

        public async Task<string> Handle(CreatePaymentHistoryCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
            {
                throw new Exception("You must be logged in to pay");
            }

            var paymentHistory = new PaymentHistory();
            paymentHistory.Id = Guid.NewGuid();
            paymentHistory.UserId = request.UserId;
            paymentHistory.OrderTicketId = request.OrderTicketId;
            paymentHistory.PricePayment = request.PricePayment;
            paymentHistory.Status = PaymentStatus.WaitingForPayment;

            await _dbContext.PaymentHistories.AddAsync(paymentHistory);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return paymentHistory.Id.ToString();
        }
    }
}