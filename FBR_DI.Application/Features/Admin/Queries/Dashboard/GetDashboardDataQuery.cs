using FBR_DI.Application.DTOs.Dashboard;
using FBR_DI.Application.ResultPattern;
using MediatR;

namespace FBR_DI.Application.Features.Admin.Queries.Dashboard;

public class GetDashboardDataQuery : IRequest<Result<DashboardDto>>
{
}
