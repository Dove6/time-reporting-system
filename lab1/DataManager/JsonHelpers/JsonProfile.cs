using AutoMapper;
using TRS.Models.DomainModels;
using TRS.Models.ViewModels;

namespace TRS.DataManager.JsonHelpers
{
    public class JsonProfile : Profile
    {
        public JsonProfile()
        {
            CreateMap<Models.JsonModels.Project, Project>()
                .ConstructUsing(src => new Project(src.Code));
            CreateMap<Models.JsonModels.ProjectListModel, ProjectListModel>();
            CreateMap<Models.JsonModels.ReportEntry, ReportEntry>();
            CreateMap<Models.JsonModels.AcceptedSummary, AcceptedSummary>()
                .ConstructUsing(src => new AcceptedSummary(src.Code));
            CreateMap<Models.JsonModels.ReportModel, Report>()
                .ConstructUsing(src => new Report(
                    JsonDataManager.GetUserFromFilename(src.Filename),
                    JsonDataManager.GetMonthFromFilename(src.Filename)
                ));
            CreateMap<Report, Models.JsonModels.ReportModel>();
            CreateMap<ReportEntry, DailyReportEntryModel>();
        }
    }
}
