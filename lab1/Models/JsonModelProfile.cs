using System.Collections.Generic;
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
            CreateMap<JsonModels.Project, Project>()
                .ConstructUsing(src => new Project(src.Code));
            CreateMap<Project, JsonModels.Project>();
            CreateMap<JsonModels.ProjectListModel, ProjectList>()
                .ForMember(dst => dst.Projects, opts => opts.MapFrom(src => src.Activities));
            CreateMap<ProjectList, JsonModels.ProjectListModel>()
                .ForMember(dst => dst.Activities, opts => opts.MapFrom(src => src.Projects));
            CreateMap<JsonModels.ReportEntry, ReportEntry>();
            CreateMap<ReportEntry, JsonModels.ReportEntry>();
            CreateMap<JsonModels.AcceptedSummary, AcceptedSummary>()
                .ConstructUsing(src => new AcceptedSummary(src.Code));
            CreateMap<AcceptedSummary, JsonModels.AcceptedSummary>();
            CreateMap<JsonModels.ReportModel, Report>()
                .ConstructUsing(src => new Report(
                    JsonDataManager.GetUserFromFilename(src.Filename),
                    JsonDataManager.GetMonthFromFilename(src.Filename)
                ))
                .AfterMap((_, dst) =>
                {
                    dst.Entries = dst.Entries.OrderBy(x => x.Date)
                        .Select((x, i) =>
                        {
                            x.IndexForDate = i;
                            return x;
                        }).ToList();
                });
            CreateMap<Report, JsonModels.ReportModel>()
                .ForMember(dst => dst.Entries,
                    opts => opts.MapFrom(src => src.Entries.OrderBy(x => x.Date.Date).ThenBy(x => x.IndexForDate)));
            CreateMap<ReportWithoutEntries, Report>()
                .ForMember(dst => dst.Entries, opts =>
                {
                    opts.PreCondition((_, dst, _) => dst.Entries == null);
                    opts.MapFrom(_ => new List<ReportEntry>());
                });
            CreateMap<Report, ReportWithoutEntries>();
        }
    }
}
