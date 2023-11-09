using UniRx;

public class EventManager
{
    static MessageBroker messageBroker;
    public static MessageBroker MessageBroker { get { return messageBroker ?? (messageBroker = new MessageBroker()); } }

    static public void Publish<T>(T message)
    { MessageBroker.Publish(message); }

    static public IObservable<T> OnEvent<T>()
    { return MessageBroker.Receive<T>(); }
}