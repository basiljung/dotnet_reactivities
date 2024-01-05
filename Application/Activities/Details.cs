using Application.Activities;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application
{
    public class Details
    {
        public class Query : IRequest<Result<ActivityDto>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ActivityDto>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Result<ActivityDto>> _logger;
            public readonly IMapper _mapper;
            public Handler(DataContext context, ILogger<Result<ActivityDto>> logger, IMapper mapper)
            {
                _mapper = mapper;
                _logger = logger;
                _context = context;
            }

            public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activity = await _context.Activities.ProjectTo<ActivityDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(x => x.Id == request.Id);

                //if the id doesnt exist the activity is null
                // if the id exists the activity is a Domain.Activity, it is the object with the values found in the DB_Context

                _logger.LogInformation($"Basil--------");
                _logger.LogInformation($"activity: {activity}");

                if (activity != null)
                    return Result<ActivityDto>.Success(activity);

                return Result<ActivityDto>.Failure("Id not found");
            }
        }
    }
}