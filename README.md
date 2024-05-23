# SquishableQuest2024

## To do
	- Display UI for remaing moves, attacks, stats, etc. for selected character
	- add ranged attack targeting for heroes
	- add attack effect
	- correct sequencing for room exploration (room reveals before destination reached in animation)
	- optimize pathfinding, move display

## Known Bugs
	- move squares are sometimes displayed on the other side of a complete block, but paths cannot be found
	- unusual paths generated when moving around an ENEMY character; the move expense seems nonetheless to be accurate?

## Safety Code to add
	- prevent move attempt if calculcated path is incomplete
	- move anim should end/timeout after a certain amount of time to prevent characters from getting stuck