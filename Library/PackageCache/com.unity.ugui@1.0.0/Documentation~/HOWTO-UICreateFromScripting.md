# Creating UI elements from scripting

If you are creating a dynamic UI where UI elements appear, disappear, or change based on user actions or other actions in the game, you may need to make a script that instantiates new UI elements based on custom logic.


## Creating a prefab of the UI element

In order to be able to easily instantiate UI elements dynamically, the first step is to create a prefab for the type of UI element that you want to be able to instantiate. Set up the UI element the way you want it to look in the Scene, and then drag the element into the Project View to make it into a prefab.

For example, a prefab for a button could be a Game Object with a Image component and a Button component, and a child Game Object with a Text component. Your setup might be different depending on your needs.

You might wonder why we don't have a API methods to create the various types of controls, including visuals and everything. The reason is that there are an infinite number of way e.g. a button could be setup. Does it use an image, text, or both? Maybe even multiple images? What is the text font, color, font size, and alignment? What sprite or sprites should the image use? By letting you make a prefab and instantiate that, you can set it up exactly the way you want. And if you later want to change the look and feel of your UI you can just change the prefab and then it will be reflected in your UI, including the dynamically created UI.


## Instantiating the UI element

Prefabs of UI elements are instantiated as normal using the Instantiate method. When setting the parent of the instantiated UI element, it's recommended to do it using the Transform.SetParent method with the worldPositionStays parameter set to false.


## Positioning the UI element

A UI Element is normally positioned using its Rect Transform. If the UI Element is a child of a Layout Group it will be automatically positioned and the positioning step can be skipped.

When positioning a Rect Transform it's useful to first determine it has or should have any stretching behavior or not. Stretching behavior happens when the anchorMin and anchorMax properties are not identical.

For a non-stretching Rect Transform, the position is set most easily by setting the anchoredPosition and the sizeDelta properties. The anchoredPosition specifies the position of the pivot relative to the anchors. The sizeDelta is just the same as the size when there's no stretching.

For a stretching Rect Transform, it can be simpler to set the position using the offsetMin and offsetMax properties. The offsetMin property specifies the corner of the lower left corner of the rect relative to the lower left anchor. The offsetMax property specifies the corner of the upper right corner of the rect relative to the upper right anchor.


## Customizing the UI Element

If you are instantiating multiple UI elements dynamically, it's unlikely that you'll want them all to look the same and do the same. Whether it's buttons in a menu, items in an inventory, or something else, you'll likely want the individual items to have different text or images and to do different things when interacted with.

This is done by getting the various components and changing their properties. See the scripting reference for the Image and Text components, and for how to work with UnityEvents from scripting.
