using AutoMapper;
using TRS.Models.DomainModels;
using TRS.Models.ViewModels;

namespace TRS.DataManager.JsonHelpers
{
    public class JsonProfile : Profile
    {
        public JsonProfile()
        {
            CreateMap<TRS.Models.JsonModels.ProjectModel, ProjectModel>();
            CreateMap<TRS.Models.JsonModels.ProjectListModel, ProjectListModel>();
            CreateMap<TRS.Models.JsonModels.ReportEntryModel, ReportEntryModel>();
            CreateMap<TRS.Models.JsonModels.AcceptedSummaryModel, AcceptedSummaryModel>();
            CreateMap<TRS.Models.JsonModels.ReportModel, ReportModel>()
                .AfterMap((_, dest) =>
                {
                    var index = 1;
                    foreach (var entry in dest.Entries)
                        entry.Id = index++;
                });
            CreateMap<ReportModel, TRS.Models.JsonModels.ReportModel>();
            CreateMap<ReportEntryModel, DailyReportEntryModel>();
        }
    }
}
