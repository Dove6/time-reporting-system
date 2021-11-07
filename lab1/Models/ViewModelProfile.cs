using System;
using System.Linq;
using AutoMapper;
using TRS.Models.DomainModels;
using TRS.Models.ViewModels;

namespace TRS.Models
{
    public class ViewModelProfile : Profile
    {
        public ViewModelProfile()
        {
            CreateMap<Project, ProjectModel>()
                .ForMember(dest => dest.Subactivities,
                    dest => dest.MapFrom(src =>
                        string.Join('\n', src.Subactivities.Select(x => x.Code))));
            CreateMap<ProjectModel, Project>()
                .ConstructUsing(src => new Project(src.Code))
                .ForMember(dest => dest.Subactivities,
                    dest => dest.MapFrom(src =>
                        src.Subactivities.Split('\n',
                                16,
                                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(x => new CategoryModel { Code = x })));
            CreateMap<Category, CategoryModel>();
            CreateMap<CategoryModel, Category>()
                .ConstructUsing(src => new Category(src.Code));
            CreateMap<ReportEntry, DailyReportEntry>()
                .ForMember(dst => dst.Id, opts => opts.MapFrom(src => src.IndexForDate));
        }
    }
}
