# Input Modules

An Input Module is where the main logic of an event system can be configured and customized. Out of the box there are two provided Input Modules, one designed for Standalone, and one designed for Touch input. Each module receives and dispatches events as you would expect on the given configuration.

Input modules are where the 'business logic' of the Event System take place. When the Event System is enabled it looks at what Input Modules are attached and passes update handling to the specific module.

Input modules are designed to be extended or modified based on the input systems that you wish to support. Their purpose is to map hardware specific input (such as touch, joystick, mouse, motion controller) into events that are sent via the messaging system.

The built in Input Modules are designed to support common game configurations such as touch input, controller input, keyboard input, and mouse input. They send a variety of events to controls in the application, if you implement the specific interfaces on your MonoBehaviours. All of the UI components implement the interfaces that make sense for the given component.
