using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;
using MyTicket.Domain.Entities;

namespace MyTicket.Application.Businesses.OrderTicket.Commands
{
    public class DeletePaymentCommand: IRequest<bool>
    {
        public Guid PaymentId { get; set; }
    }

    public class DeletePaymentCommandHandler : IRequestHandler<DeletePaymentCommand, bool>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        private readonly IClock _clock;
        public DeletePaymentCommandHandler(IMyTicketDbContext dbContext, IContext context, IClock clock)
        {
            _dbContext = dbContext;
            _context = context;
            _clock = clock;
        }

        public async Task<bool> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
        {
            if (
                    request.PaymentId == Guid.Empty
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

            _dbContext.Payments.Remove(getPayment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}