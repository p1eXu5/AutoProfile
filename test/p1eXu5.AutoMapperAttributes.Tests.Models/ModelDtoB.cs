using p1eXu5.AutoMapperAttributes.Attributes;
using System;

namespace p1eXu5.AutoMapperAttributes.Tests.Models
{
    [ MapTo( typeof(Model) ) ]
    public class ModelDtoB
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Int64 Date { get; set; }
    }
}
