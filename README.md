# Modded Bug Fix
Fixes bugs in other mods that likely won't be updated anymore.

# Which mods does this affect?
Currently this mod fixes bugs in the following mods:
 * Frost and Gunfire
   * Fixes a bug where the Swindler's Barter item doesn't correctly reroll the shop and instead consumes all of the player's money.
   * Fixes exceptions caused by Mini Mushboom enemies having missing animations.
   * Fixes exceptions caused by the Bloodied Key when the owner is not in a room.
   * Fixes the mod not loading for languages other than English.
   * Fixes exceptions caused by Greed, Sack, Bullet Sponge and Mirror Guon Stone when destroyed while not picked up.
   * Fixes some enemies having collision while spawning.
 * Prismatism
   * Fixes exceptions caused by the Parrot's Feather when the owner is not in a room.
   * Fixes exceptions caused by the Medal of Bravery when the owner is not in a room with enemies.
   * Fixes the mod not loading for languages other than English.
   * Fixes Maiden Plating not working as intended (only shooting one projectile, softlocking the game when falling into pits, etc).
   * Fixes Two of Hearts causing heart pickups to be more rare when destroyed while not picked up.
 * Retrash's Item Collection
   * Fixes exceptions caused by reloading the Soul Orb when Retrash's Item Collection is installed.
   * Fixes the mod not loading for languages other than English.
 * A Bleaker Item Pack
   * Fixes exceptions caused by the Carrot when not picked up.
   * Fixes exceptions caused by Chamber of Frog bullets when no enemies are nearby, when the projectiles don't have an owner or when the owner isn't in a room.
   * Fixes exceptions caused by Winchester's Hat when not picked up.
   * Fixes the mod not loading for languages other than English.
   * Fixes the Golden Circlet messing with pickup drop chances.
   * Fixes the Lead Crown causing exceptions when destroyed, as well as not disabling its effects when destroyed while picked up.
   * Fixes Chamber of Frogs, Catcher's Mitts, Protractor, Trick Shot, Fitted Tank Barrel, Pendant of the First Order, Golden Circlet, Cheese Ammolet, Distribullets, Gatling Gullets, Hungry Clips, Heroic Cape, Popcorn, Repurposed Shell Casing, White Bullet Cell, Strawberry Jam, Bleaker, Winchester's Hat, Jammomancer's Hat, Showoff Bullets, Healthy Bullets and Friendship Bracelet not disabling some of their effects when destroyed while picked up.
   * Fixes Suspicious Looking Bell, Shades's Shades, Ammoconda's Nest, Rewind and Overpill causing exceptions when destroyed while not picked up.
   * Fixes exceptions caused by Shades's Shades when not picked up.
   * Fixes projectiles of Star Splitter and Pouch Launcher not working as intended.
   * Fixes Baby Good Shellicopter causing exceptions and slowdowns after loading a new floor.
   * Fixes exceptions caused by Gundromeda Pain when in a room without enemies.
   * Fixes Gatling Gullets breaking when hitting certain enemies.
   * Fixes exceptions caused by Spill-O' Jar when not picked up.
 * Oddments
   * Fixes exceptions caused by the Spider Boots during floor generation.
   * Fixes exceptions caused by enemies with no animator standing in goop when Oddments is installed.
   * Fixes Cubic Shells, Hideous Bullets, Radar Bullets, Space Slug, Toothy Bullets, Rejuvenation Rack, Hellfire's Rounds, Frost Leech, The Wholly Ghost and Member Card causing exceptions when destroyed while not picked up.
   * Fixes the Fortune Cookie not disabling its passive effect correctly when desroyed while picked up.
 * The Reference Collection
   * Fixes Expensive Bullets, Cheesy Bullets and Determination Bullets causing exceptions when destroyed while not picked up. 
   * Fixes Expensive Bullets giving a 50% damage up on pickup instead of a 5% damage up.
   * Fixes the Snowfox weapons breaking after transforming.

General fixes:
 * Fixes the `AudioAnimatorListener.Start()` error on startup.
 * Fixes `AIAnimator.OnDestroy()` sometimes causing exceptions.