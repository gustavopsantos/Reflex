# Event Trigger

The Event Trigger receives events from the Event System and calls registered functions for each event.

The Event Trigger can be used to specify functions you wish to be called for each Event System event. You can assign multiple functions to a single event and whenever the Event Trigger receives that event it will call those functions.

Note that attaching an Event Trigger component to a GameObject will make that object intercept all events, and no event bubbling will occur from this object!


## Events

Each of the [Supported Events](SupportedEvents.md) can optionally be included in the Event Trigger by clicking the Add New Event Type button.
