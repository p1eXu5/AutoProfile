using System;

namespace p1eXu5.AutoProfile.Tests.Fakes
{
    using Attributes;

    [ MapTo( typeof(Model) ) ]
    public class ModelDtoB
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Int64 Date { get; set; }
    }
}
