using System;
namespace Business
{
    public interface IEventRegister
    {
        void RegisterEventHandler<T>(short aggregateTypeId, short messageTypeId, Action<T> eventHandler);
    }
}
