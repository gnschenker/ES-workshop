using System;

namespace SampleProject.Domain
{
    public interface IState
    {
        Guid Id { get; }
        int Version { get; }
        void Modify(object e);
    }
}