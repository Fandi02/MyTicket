using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;
using MyTicket.Domain.Entities;

namespace MyTicket.Application.Businesses.OrderTicket.Commands
{
    public class UpdateStatusPaymentCommand: IRequest<bool>
    {
        public Guid PaymentId { get; set; }
        public StatusPaymentEnum Status { get; set; }
        public string? RejectedReason { get; set; }
    }

    public class UpdateStatusPaymentCommandHandler : IRequestHandler<UpdateStatusPaymentCommand, bool>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        private readonly IClock _clock;
        public UpdateStatusPaymentCommandHandler(IMyTicketDbContext dbContext, IContext context, IClock clock)
        {
            _dbContext = dbContext;
            _context = context;
            _clock = clock;
        }

        public async Task<bool> Handle(UpdateStatusPaymentCommand request, CancellationToken cancellationToken)
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

            getPayment.Status = request.Status;
            getPayment.RejectedReason = !string.IsNullOrEmpty(request.RejectedReason) ? request.RejectedReason : "";

            _dbContext.Payments.Update(getPayment);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}