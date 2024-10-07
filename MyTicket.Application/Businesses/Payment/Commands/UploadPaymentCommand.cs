using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTicket.Application.Exceptions;
using MyTicket.Application.Interfaces;
using MyTicket.Domain.Entities;

namespace MyTicket.Application.Businesses.OrderTicket.Commands
{
    public class UploadPaymentCommand: IRequest<bool>
    {
        public Guid OrderTicketId { get; set; }
        public Guid UserId { get; set; }
        public string ImagePayment { get; set; }
        public string Description { get; set; }
        public decimal TotalPayment { get; set; }
    }

    public class UploadPaymentCommandHandler : IRequestHandler<UploadPaymentCommand, bool>
    {
        private readonly IMyTicketDbContext _dbContext;
        private readonly IContext _context;
        private readonly IClock _clock;
        public UploadPaymentCommandHandler(IMyTicketDbContext dbContext, IContext context, IClock clock)
        {
            _dbContext = dbContext;
            _context = context;
            _clock = clock;
        }

        public async Task<bool> Handle(UploadPaymentCommand request, CancellationToken cancellationToken)
        {
            if (
                    request.OrderTicketId == Guid.Empty ||
                    string.IsNullOrEmpty(request.ImagePayment) ||
                    string.IsNullOrEmpty(request.Description) ||
                    request.TotalPayment == 0
                )
                throw new BadRequestException("Request is null");

            var getOrder = await _dbContext.OrderTickets.Include(x => x.Event).
                                        FirstOrDefaultAsync(x => 
                                            x.OrderTicketId == request.OrderTicketId && 
                                            x.UserId == request.UserId &&
                                            x.IsPaid == false &&
                                            x.IsDeleted == false &&
                                            x.Event.IsDeleted == false
                                        );

            if (getOrder == null)
                throw new BadRequestException("Event not found");

            var savePayment = new Domain.Entities.Payment
            {
                PaymentId = Guid.NewGuid(),
                ImagePayment = request.ImagePayment,
                Description = request.Description,
                TotalPayment = request.TotalPayment,
                Date = _clock.CurrentDate(),
                Status = StatusPaymentEnum.Received,
                UserId = request.UserId,
                OrderTicketId = getOrder.OrderTicketId
            };

            getOrder.IsPaid = true;

            await _dbContext.Payments.AddAsync(savePayment);
            _dbContext.OrderTickets.Update(getOrder);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}