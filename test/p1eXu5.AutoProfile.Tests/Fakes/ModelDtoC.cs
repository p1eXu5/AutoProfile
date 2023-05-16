namespace p1eXu5.AutoProfile.Tests.Fakes
{
    using Attributes;

    [MapFrom(typeof(Model), ReverseMap = true)]
    public class ModelDtoC
    {
        public int Id { get; set; }

        public Int64 Date { get; set; }
    }
}
