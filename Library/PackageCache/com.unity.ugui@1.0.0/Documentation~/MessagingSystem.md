
# Messaging System

The new UI system uses a messaging system designed to replace SendMessage. The system is pure C# and aims to address some of the issues present with SendMessage. The system works using custom interfaces that can be implemented on a MonoBehaviour to indicate that the component is capable of receiving a callback from the messaging system. When the call is made a target GameObject is specified; the call will be issued on all components of the GameObject that implement the specified interface that the call is to be issued against. The messaging system allows for custom user data to be passed, as well as how far through the GameObject hierarchy the event should propagate; that is should it just execute for the specified GameObject, or should it also execute on children and parents. In addition to this the messaging framework provides helper functions to search for and find GameObjects that implement a given messaging interface.

The messaging system is generic and designed for use not just by the UI system but also by general game code. It is relatively trivial to add custom messaging events and they will work using the same framework that the UI system uses for all event handling.

## Defining A Custom Message

If you wish to define a custom message it is relatively simple. In the UnityEngine.EventSystems namespace there is a base interface called 'IEventSystemHandler'. Anything that extends from this can be considered as a target for receiving events via the messaging system.

````
public interface ICustomMessageTarget : IEventSystemHandler
{
    // functions that can be called via the messaging system
    void Message1();
    void Message2();
}
````

Once this interface is defined then it can be implemented by a MonoBehaviour. When implemented it defines the functions that will be executed if the given message is issued against this MonoBehaviours GameObject.

````
public class CustomMessageTarget : MonoBehaviour, ICustomMessageTarget
{
    public void Message1()
    {
        Debug.Log ("Message 1 received");
    }

    public void Message2()
    {
        Debug.Log ("Message 2 received");
    }
}
````

Now that a script exists that can receive the message we need to issue the message. Normally this would be in response to some loosely coupled event that occurs. For example, in the UI system we issue events for such things as PointerEnter and PointerExit, as well as a variety of other things that can happen in response to user input into the application.

To send a message a static helper class exists to do this. As arguments it requires a target object for the message, some user specific data, and a functor that maps to the specific function in the message interface you wish to target.

````
ExecuteEvents.Execute<ICustomMessageTarget>(target, null, (x,y)=>x.Message1());
````

This code will execute the function Message1 on any components on the GameObject target that implement the ICustomMessageTarget interface. The scripting documentation for the ExecuteEvents class covers other forms of the Execute functions, such as Executing in children or in parents.
