using System.Globalization;
using AutoMapper;
using Trs.Extensions;
using Trs.Models.DomainModels;
using Trs.Models.RestModels;

namespace Trs.Models;

public class RestModelProfile : Profile
{
    public RestModelProfile()
    {
        CreateMap<Category, CategoryModel>()
            .ReverseMap();
        CreateMap<Project, ProjectListResponseEntry>();
        CreateMap<Project, ManagedProjectListResponseEntry>()
            .ForMember(dst => dst.BudgetLeft,
                opt => opt.MapFrom(
                    src => src.Budget - src.AcceptedTime!.Sum(x => x.Time)));
        CreateMap<ProjectCreationRequest, Project>()
            .ForMember(dst => dst.Categories,
                opt => opt.Ignore());
        CreateMap<Project, ProjectDetailsResponse>()
            .ForMember(dst => dst.BudgetLeft,
                opt => opt.MapFrom(
                    src => src.Budget - src.AcceptedTime!.Sum(x => x.Time)))
            .ForMember(dst => dst.AcceptedTime,
                opt => opt.Ignore());
        CreateMap<ProjectUpdateRequest, Project>();
        CreateMap<AcceptedTimeUpdateRequest, AcceptedTime>();
        CreateMap<ReportEntry, ReportEntryResponse>()
            .ForMember(dst => dst.Date,
                opt => opt.MapFrom(
                    src => $"{src.ReportMonth}-{src.DayOfMonth}"));
        CreateMap<ReportEntryUpdateRequest, ReportEntry>();
        CreateMap<ReportEntryCreationRequest, ReportEntry>();
        CreateMap<User, UserModel>();
    }
}
