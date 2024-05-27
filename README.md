# SquishableQuest2024

## To do
	- add Enemy scriptable object for specialized statlines
	- add enemy AI: move and attacks on turn, with different behavior mode presets and special enemy statlines (i.e. simplified
	armor bonuses, etc.); each enemy has a hero they are current targeting that is assigned at placement; characters moving out of 
	range may allow enemy to change target
	- Stat display includes character portrait and shows special rules (stat display can also be turned into a still image)
	- add equipment stats to attack calculations
	- enemy death in combat
	- deathblow for hero melee attacks
	- correct sequencing for room exploration (room reveals before destination reached in animation)
	- optimize pathfinding, move display

## Known Bugs
	- often unusual paths generated when moving around an ENEMY character; the move expense seems nonetheless to be accurate?
	- rarely move does not occur and character gets stuck

## Safety Code to add
	- move anim should end/timeout after a certain amount of time to prevent characters from getting stuck

## Additional Features
	- add Dodge special rule (chance to avoid attack using initiative)?