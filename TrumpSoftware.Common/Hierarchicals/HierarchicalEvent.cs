using System;
using System.Collections.Generic;
using System.Linq;
using TrumpSoftware.Common.Exceptions;

namespace TrumpSoftware.Common.Hierarchical
{
    /*public abstract class HierarchicalEvent<TSender, TData>
        where TSender : IHierarchical
    {
        #region HandlersBySubscribers

        private readonly IList<SubscriberHandlerPair<TSender, TData>> _handlersBySubscribers;

        #endregion

        #region PropagationDirection

        public EventsPropagationDirection PropagationDirection { get; private set; }

        #endregion

        protected HierarchicalEvent(EventsPropagationDirection propagationDirection)
        {
            PropagationDirection = propagationDirection;
            _handlersBySubscribers = new List<SubscriberHandlerPair<TSender, TData>>();
        }

        public void Subscribe(IHierarchical subscriber, HierarchcalEventHandler<TSender, TData> handler)
        {
            if (subscriber == null) throw new ArgumentNullException("subscriber");
            if (handler == null) throw new ArgumentNullException("handler");
            _handlersBySubscribers.Add(new SubscriberHandlerPair<TSender, TData>
            {
                Subscriber = subscriber,
                Handler = handler
            });
        }

        public void Unsubscribe(IHierarchical subscriber, HierarchcalEventHandler<TSender, TData> handler)
        {
            var subscriberHandlerPair = _handlersBySubscribers.FirstOrDefault(x => x.Subscriber == subscriber && x.Handler == handler);
            if (subscriberHandlerPair == null)
                return;
            _handlersBySubscribers.Remove(subscriberHandlerPair);
        }

        public void Publish(TSender source, TData data)
        {
            if (ReferenceEquals(source, null))
                throw new ArgumentNullException("source");
            IEnumerable<IHierarchical> hierarchicals;
            switch (PropagationDirection)
            {
                case EventsPropagationDirection.ToAncestors:
                    hierarchicals = source.GetAncestors<IHierarchical>();
                    break;
                case EventsPropagationDirection.ToDescendants:
                    hierarchicals = source.GetDescendants<IHierarchical>();
                    break;
                default:
                    throw new UnhandledCaseException(typeof(EventsPropagationDirection), PropagationDirection);
            }
            foreach (var hierarchical in hierarchicals)
            {
                var subscriberHandlerPairs = _handlersBySubscribers.Where(x => x.Subscriber == hierarchical).ToList();
                foreach (var subscriberHandlerPair in subscriberHandlerPairs)
                    subscriberHandlerPair.Handler(source, data);
            }
        }
    }*/
}
