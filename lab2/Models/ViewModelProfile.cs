using AutoMapper;
using Trs.Models.DomainModels;
using Trs.Models.ViewModels;

namespace Trs.Models;

public class ViewModelProfile : Profile
{
    public ViewModelProfile()
    {
        CreateMap<Project, ProjectModel>()
            .ForMember(dst => dst.Categories,
                dst => dst.MapFrom(src =>
                    string.Join('\n', src.Categories.Select(x => x.Code))));
        CreateMap<Project, ProjectWithUserSummaryModel>()
            .ForMember(dst => dst.Categories,
                dst => dst.MapFrom(src =>
                    string.Join('\n', src.Categories.Select(x => x.Code))));
        CreateMap<ProjectModel, Project>()
            .ForMember(dst => dst.Categories,
                dst => dst.MapFrom(src =>
                    src.Categories.Split('\n',
                            16,
                            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        .Select(x => new CategoryModel { Code = x })));
        CreateMap<Category, CategoryModel>()
            .ReverseMap();
        CreateMap<ReportEntry, ReportEntryModel>()
            .ForMember(dst => dst.Code,
                opt => opt.MapFrom(src =>
                    src.ProjectCode))
            .ForMember(dst => dst.Subcode,
                opt => opt.MapFrom(src =>
                    src.Category.Code));
        CreateMap<ReportEntryModel, ReportEntry>()
            .ForMember(dst => dst.ProjectCode,
                opt => opt.MapFrom(src => src.Code))
            .ForMember(dst => dst.Description,
                opt => opt.NullSubstitute(string.Empty));
        CreateMap<ReportEntry, ReportEntryForEditingModel>()
            .ForMember(dst => dst.Code,
                opt => opt.MapFrom(src =>
                    src.ProjectCode))
            .ForMember(dst => dst.Subcode,
                opt => opt.MapFrom(src =>
                    src.Category.Code));

        CreateMap<ReportEntryModel, ReportEntryForEditingModel>();
        CreateMap<ReportEntryModel, ReportEntryForAddingModel>();
        CreateMap<UserModel, UserSelectListModel>();

        CreateMap<ReportEntryUpdatableModel, ReportEntryModel>();
        CreateMap<ProjectUpdatableModel, ProjectModel>();
    }
}
