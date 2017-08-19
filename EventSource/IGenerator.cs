using System;
using System.Collections.Generic;
using Business;

namespace EventSource
{
    public interface IGenerator
    {
        /// <summary>
        /// Register IEvent generators.
        /// </summary>
        /// <param name="aggregateTypeId"></param>
        /// <param name="messageTypeId"></param>
        /// <param name="eventGenerator">The function that returns the IEvent type based on aggregateTypeId and messageTypeId.</param>
        /// <typeparam name="TResult">The type this function will return.</typeparam>
        void RegisterEventGenerator<TResult>(short aggregateTypeId, short messageTypeId, Func<int, TResult> eventGenerator);

        /// <summary>
        /// Kicks off generation of IEvents.
        /// </summary>
        /// <returns>The generated IEnumerable of IEvents.</returns>
        /// <param name="limit">How many IEvents to generate.</param>
        IEnumerable<IEvent> Get(int limit);
    }
}
