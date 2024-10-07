using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Businesses.Payment.Models;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;
using MyTicket.Domain.Entities;

namespace MyTicket.Application.Businesses.OrderTicket.Commands
{
    public class UpdateStatusPaymentCommand: IRequest<UpdateStatusCommandResponse>
    {
        public Guid PaymentId { get; set; }
        public StatusPaymentEnum Status { get; set; }
        public string? RejectedReason { get; set; }
    }

    public class UpdateStatusPaymentCommandHandler : IRequestHandler<UpdateStatusPaymentCommand, UpdateStatusCommandResponse>
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

        public async Task<UpdateStatusCommandResponse> Handle(UpdateStatusPaymentCommand request, CancellationToken cancellationToken)
        {
            if (
                    request.PaymentId == Guid.Empty
                )
                throw new BadRequestException("Request is null");

            var getPayment = await _dbContext.Payments
                                        .Include(x => x.User)
                                        .Include(x => x.OrderTicket.Event)
                                        .FirstOrDefaultAsync(x => 
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

            var response = new UpdateStatusCommandResponse
            {
                Email = getPayment.User.Email,
                FullName = getPayment.User.FullName,
                EventName = getPayment.OrderTicket.Event.Name,
                TicketNumber = getPayment.OrderTicket.TicketNumber,
                RejectedReason = !string.IsNullOrEmpty(getPayment.RejectedReason) ? getPayment.RejectedReason : ""
            };

            return response;
        }
    }
}