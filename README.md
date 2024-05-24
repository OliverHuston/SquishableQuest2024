# SquishableQuest2024

## To do
	- add attack damage calculation
	- (add attack damage number UI display)
	- Display UI for remaing moves, attacks, stats, etc. for selected character
	- correct sequencing for room exploration (room reveals before destination reached in animation)
	- optimize pathfinding, move display

## Known Bugs
	- often unusual paths generated when moving around an ENEMY character; the move expense seems nonetheless to be accurate?
	- rarely move does not occur and character gets stuck

## Safety Code to add
	- prevent move attempt if calculcated path is incomplete
	- move anim should end/timeout after a certain amount of time to prevent characters from getting stuck