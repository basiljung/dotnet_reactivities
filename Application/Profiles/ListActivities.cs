using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles
{
    public class ListActivities
    {

        public class Query : IRequest<Result<List<UserActivityDto>>>
        {
            public string Username { get; set; }
            public string Predicate { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<UserActivityDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _mapper = mapper;
                _context = context;
                _userAccessor = userAccessor;
            }


            public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activities = new List<UserActivityDto>();
                var currentDate = DateTime.UtcNow;
                var query = _context.ActivityAttendees.Where(aa => aa.AppUser.UserName == request.Username).Select(aa => aa.Activity).ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider).AsQueryable();
                switch (request.Predicate)
                {
                    case "past":
                        activities = await query.Where(aa => aa.Date <= currentDate).ToListAsync(cancellationToken: cancellationToken);
                        break;
                    case "hosting":
                        activities = await _context.ActivityAttendees.Where(aa => aa.AppUser.UserName == request.Username).Where(aa => aa.IsHost).Select(aa => aa.Activity).ProjectTo<UserActivityDto>(_mapper.ConfigurationProvider, new { currentUsername = _userAccessor.GetUsername() }).ToListAsync(cancellationToken: cancellationToken);
                        break;
                    default:
                        activities = await query.Where(aa => aa.Date >= currentDate).ToListAsync(cancellationToken: cancellationToken);
                        break;
                }
                return Result<List<UserActivityDto>>.Success(activities);

            }
        }

    }
}