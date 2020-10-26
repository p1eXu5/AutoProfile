using p1eXu5.AutoMapperAttributes.Attributes;
using System;

namespace p1eXu5.AutoProfile.Tests.Fakes
{
    [ MapFrom( typeof(Model) ) ]
    public class ModelDtoA
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Int64 Date { get; set; }
    }
}
