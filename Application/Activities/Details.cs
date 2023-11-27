using Application.Core;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application
{
    public class Details
    {
        public class Query : IRequest<Result<Activity>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<Activity>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Result<Activity>> _logger;
            public Handler(DataContext context, ILogger<Result<Activity>> logger)
            {
                _logger = logger;
                _context = context;
            }

            public async Task<Result<Activity>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.FindAsync(request.Id);

                //if the id doesnt exist the activity is null
                // if the id exists the activity is a Domain.Activity, it is the object with the values found in the DB_Context

                _logger.LogInformation($"Basil--------");
                _logger.LogInformation($"activity: {activity}");

                if (activity != null)
                    return Result<Activity>.Success(activity);

                return Result<Activity>.Failure("Id not found");
            }
        }
    }
}