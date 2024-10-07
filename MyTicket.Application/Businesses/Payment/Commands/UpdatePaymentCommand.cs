using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;
using MyTicket.Domain.Entities;

namespace MyTicket.Application.Businesses.OrderTicket.Commands
{
    public class UpdatePaymentCommand: IRequest<bool>
    {
        public Guid PaymentId { get; set; }
        public string ImagePayment { get; set; }
        public string Description { get; set; }
        public decimal TotalPayment { get; set; }
    }

    public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, bool>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        private readonly IClock _clock;
        public UpdatePaymentCommandHandler(IMyTicketDbContext dbContext, IContext context, IClock clock)
        {
            _dbContext = dbContext;
            _context = context;
            _clock = clock;
        }

        public async Task<bool> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
        {
            if (
                    request.PaymentId == Guid.Empty ||
                    string.IsNullOrEmpty(request.ImagePayment) ||
                    string.IsNullOrEmpty(request.Description) ||
                    request.TotalPayment == 0
                )
                throw new BadRequestException("Request is null");

            var getPayment = await _dbContext.Payments.
                                        FirstOrDefaultAsync(x => 
                                            x.PaymentId == request.PaymentId &&
                                            x.Status == StatusPaymentEnum.Received &&
                                            x.IsDeleted == false
                                        );

            if (getPayment == null)
                throw new BadRequestException("Event not found");

            getPayment.ImagePayment = request.ImagePayment;
            getPayment.Description = request.Description;
            getPayment.TotalPayment = request.TotalPayment;
            getPayment.Date = _clock.CurrentDate();

            _dbContext.Payments.Update(getPayment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}