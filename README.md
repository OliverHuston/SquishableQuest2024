# SquishableQuest2024

## To do
	- enemy action and move needs a total rework (currently causes game to crash)
	- enemies should wait between attacks
	- ranged_default enemy behavior
	- enemy spawning and target assignment; changing target assignment based on behavior
	- Stat display shows special rules with pop up
	- Add character equipment system
	- Add character abilities system
	- multiple enemies should fight in initiative order
	- enemy death in combat
	- deathblow for hero melee attacks
	- pinning based on initiative
	- fix character stat display sequencing (need to check what matches the original game)
	- optimize pathfinding, move display
	- stat display backdrop turned into a still image for optimization

## Known Bugs
	- sometimes does not take the diagonal when moving around characters, resulting in an extra move expenditure
	- rarely move does not occur and character gets stuck
	- enemy models will move off map

## Safety Code to add
	- move anim should end/timeout after a certain amount of time to prevent characters from getting stuck

## Additional Features
	- add Dodge special rule (chance to avoid attack using initiative)?