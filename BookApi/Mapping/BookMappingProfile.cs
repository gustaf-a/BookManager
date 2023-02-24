using AutoMapper;
using Entities.ModelsEf;
using Entities.ModelsSql;
using Shared;
using Shared.DataTransferObjects;

namespace BookApi.Mapping;

public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        //EF Core Repository
        CreateMap<BookEf, BookDto>()
            .ForMember(bdto => bdto.Publish_date, opt => opt.MapFrom(bef => bef.PublishDate));

        CreateMap<BookForCreationDto, BookEf>()
            .ForMember(b => b.Id, act => act.Ignore())
            .ForMember(b => b.PublishDate, opt => opt.MapFrom(bdto => bdto.Publish_date));

        CreateMap<BookForUpdateDto, BookEf>()
            .ForMember(b => b.Id, act => act.Ignore())
            .ForMember(b => b.PublishDate, opt => opt.MapFrom(bdto => bdto.Publish_date));

        //SQLite Repository
        CreateMap<Book, BookDto>()
            .ForMember(bdto => bdto.Publish_date, opt => opt.MapFrom(b => b.PublishDate.ToDateOnlyString()))
            .ReverseMap();

        CreateMap<BookForCreationDto, Book>()
            .ForMember(b => b.Id, act => act.Ignore())
            .ForMember(b => b.PublishDate, opt => opt.MapFrom(bdto => bdto.Publish_date.ToDateOnly()));

        CreateMap<BookForUpdateDto, Book>()
            .ForMember(b => b.Id, act => act.Ignore())
            .ForMember(b => b.PublishDate, opt => opt.MapFrom(bdto => bdto.Publish_date.ToDateOnly()));

        CreateMap<Book, BookSqlite>()
            .ForMember(bsql => bsql.Publish_date, opt => opt.MapFrom(b => b.PublishDate.ToDateOnlyString()));

        CreateMap<BookSqlite, Book>()
            .ForMember(bsql => bsql.PublishDate, opt => opt.MapFrom(b => b.Publish_date.ToDateOnly()));
    }
}
