I'm using the new Unity input system

To add new controls to a player just duplicate one of the Input templates
and change the bindings to whichever you want (keyboard, gamepad, ...) and
set which player uses which input template.

I created 2 inputs. One that uses WASD and another one that uses the arrows.
You can enable the 2º snake in the hierarchy, each of them is using a different inputs.

You can also spawn new snakes pressing button 'P' (for testing).
The snake instanced from pressing button P always uses the same input.


- Board is resizable. 
- There are functions to check safe directions which might be useful for an AI
- There is an easy way to setup a replay system (alreayd storing starting position + list of movements played by each snake).

- You can spawn snakes at runtime and it will spawn them in
empty squares (there is no check for the board being full since this is just for testing, if you try to spawn too many and they can't fit anywhere it will cause
and infinite loop) (maybe not since the snakes will be hitting eachother and the walls and dissappearing leaving new empty spaces...)


- The WIN / LOSE conditions are just a Console Debug text