using AutoMapper;
using Kip.Report.Domain.Models;
using Kip.Report.Infrastructure.Models;
using NpgsqlTypes;

namespace Kip.Report.Infrastructure.Mappings;

public class InfrastructureMappingProfile : Profile
{
    public InfrastructureMappingProfile()
    {
        CreateMap<Query, ReportQueryDbo>()
            .ForMember(q => q.Period, (opt) => opt.MapFrom(src => 
                new NpgsqlRange<DateTimeOffset>(src.From, src.To)
            ));

        CreateMap<ReportQueryDbo, Query>()
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.Period.LowerBound))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.Period.UpperBound));
    }
}
