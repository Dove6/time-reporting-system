using System.Linq;
using AutoMapper;
using TRS.DataManager;
using TRS.Models.DomainModels;

namespace TRS.Models
{
    public class JsonModelProfile : Profile
    {
        public JsonModelProfile()
        {
            CreateMap<JsonModels.Activity, Project>()
                .ReverseMap();
            CreateMap<JsonModels.Subactivity, Category>()
                .ReverseMap();
            CreateMap<JsonModels.ReportEntry, ReportEntry>()
                .ReverseMap();
            CreateMap<JsonModels.ActivitySummary, AcceptedTime>()
                .ReverseMap();
            CreateMap<JsonModels.Report, Report>()
                .ForMember(dst => dst.Entries, opts => opts.Ignore())
                .ForMember(dst => dst.Owner,
                    opts => opts.MapFrom(src => JsonDataManager.GetOwnerFromReportFilename(src.Filename)))
                .ForMember(dst => dst.Month,
                    opts => opts.MapFrom(src => JsonDataManager.GetMonthFromReportFilename(src.Filename)));
            CreateMap<Report, JsonModels.Report>()
                .ForMember(dst => dst.Entries,
                    opts => opts.MapFrom(src => src.Entries.OrderBy(x => x.MonthlyIndex)))
                .ForMember(dst => dst.Filename,
                    opts => opts.MapFrom(src => JsonDataManager.GetReportFilename(src.Owner, src.Month)));
        }
    }
}
