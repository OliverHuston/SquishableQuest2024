# SquishableQuest2024

## To do
	- Keep track of spent moves; have character move along path (i.e. not direct to target)
	- Display UI for remaing moves, attacks, stats, etc. for selected character
	- add end turn system
	- add melee attack targeting for heroes
	- add ranged attack targeting for heroes
	- correct sequencing for room exploration (room reveals before destination reached in animation)

## Possible changes
	- move currently allows diagonals with 1 adjacent friend or foe; if this is kept, the Traverse function no longer needs to take CharacterType as an input; 
	however, maybe change this to only allow diagonal with friendlies?

## Known Bugs
	- sometimes, character will rotate but not move and remains frozen; location updates, but IEnumeratior MoveAnimation apparently never ends