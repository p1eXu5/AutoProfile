using AutoMapper;
using Microsoft.Extensions.Logging;

namespace p1eXu5.AutoMapperAttributes.Contracts
{
    public interface IAutoProfile
    {
        public Profile Instance { get; }
        public ILogger Logger { get; }
    }
}
