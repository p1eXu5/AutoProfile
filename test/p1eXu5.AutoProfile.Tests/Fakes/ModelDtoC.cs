using p1eXu5.AutoMapperAttributes.Attributes;
using System;

namespace p1eXu5.AutoProfile.Tests.Fakes
{
    [ MapFrom( typeof(Model), ReverseMap = true ) ]
    public class ModelDtoC
    {
        public int Id { get; set; }
        public Int64 Date { get; set; }
    }
}
