using AutoMapper;
using BookApi.Mapping;

namespace BookApiTests;

public class MappingTests
{
    [Fact]
    public void AutoMapper_Configuration_IsValid()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<BookMappingProfile>());

        config.AssertConfigurationIsValid();
    }
}