# SquishableQuest2024

## To do
	- Display UI for remaing moves, attacks, stats, etc. for selected character
	- add melee attack targeting for heroes
	- add ranged attack targeting for heroes
	- correct sequencing for room exploration (room reveals before destination reached in animation)
	- optimize pathfinding, move display

## Known Bugs
	- unusual paths generated when moving around an ENEMY character; the move expense seems nonetheless to be accurate?

## Old Bugs
	- sometimes, character will rotate but not move and remains frozen; location updates, but IEnumeratior MoveAnimation apparently never ends
	- Sometimes moving to an exit cell does not trigger room open?