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
            CreateMap<JsonModels.ProjectListModel, ProjectList>();
            CreateMap<ProjectList, JsonModels.ProjectListModel>();
            CreateMap<JsonModels.ReportEntry, ReportEntry>();
            CreateMap<ReportEntry, JsonModels.ReportEntry>();
            CreateMap<JsonModels.AcceptedSummary, AcceptedSummary>()
                .ConstructUsing(src => new AcceptedSummary(src.Code));
            CreateMap<AcceptedSummary, JsonModels.AcceptedSummary>();
            CreateMap<JsonModels.ReportModel, Report>()
                .ConstructUsing(src => new Report(
                    JsonDataManager.GetUserFromFilename(src.Filename),
                    JsonDataManager.GetMonthFromFilename(src.Filename)
                ));
            CreateMap<Report, JsonModels.ReportModel>();
        }
    }
}
