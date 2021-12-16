using AutoMapper;
using Trs.Models.DomainModels;
using Trs.Models.ViewModels;

namespace Trs.Models;

public class ViewModelProfile : Profile
{
    public ViewModelProfile()
    {
        CreateMap<Project, ProjectModel>()
            .ForMember(dest => dest.Categories,
                dest => dest.MapFrom(src =>
                    string.Join('\n', src.Categories.Select(x => x.Code))));
        CreateMap<Project, ProjectWithUserSummaryModel>()
            .ForMember(dest => dest.Categories,
                dest => dest.MapFrom(src =>
                    string.Join('\n', src.Categories.Select(x => x.Code))));
        CreateMap<ProjectModel, Project>()
            .ForMember(dest => dest.Categories,
                dest => dest.MapFrom(src =>
                    src.Categories.Split('\n',
                            16,
                            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(x => new CategoryModel { Code = x })));
        CreateMap<Category, CategoryModel>()
            .ReverseMap();
        CreateMap<ReportEntry, ReportEntryModel>()
            .ReverseMap();
        CreateMap<ReportEntry, ReportEntryForEditingModel>();

        CreateMap<ReportEntryModel, ReportEntryForEditingModel>();
        CreateMap<ReportEntryModel, ReportEntryForAddingModel>();
        CreateMap<UserModel, UserSelectListModel>();

        CreateMap<ReportEntryUpdatableModel, ReportEntryModel>();
        CreateMap<ProjectUpdatableModel, ProjectModel>();
    }
}
