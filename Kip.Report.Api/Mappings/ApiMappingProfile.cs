using AutoMapper;
using Kip.Report.Api.Models;
using Kip.Report.Domain.Models;

namespace Kip.Report.Api.Mappings;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<QueryRequest, Query>();
        
        CreateMap<ReportData, ReportDto>();

        CreateMap<Query, QueryResponse>()
            .ForMember(dest => dest.Report, opt => opt.MapFrom(src =>
                src.Report != null
                    ? new ReportDto
                    {
                        UserId = src.UserId,
                        CountSignIn = src.Report.CountSignIn
                    }
                    : null
            ));
    }
}
