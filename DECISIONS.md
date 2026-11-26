# Decision log

Add an entry every time the team makes a real choice: a tool, a design, a trade-off.
About 1–2 entries a week. Newest at the top. Write the reasoning yourselves; AI may fix the wording.

## Template
```
## YYYY-MM-DD — [The decision, in a few words]
Options: [A] / [B] / [C]
Chose: [what] — [why]
AI: [what the AI suggested, and what you did with it — or "not used"]
Who: [name(s)]
```

## Example
```
## 2026-11-20 — Save progress locally first
Options: PlayerPrefs / JSON file / Cloud Save
Chose: JSON file now, Cloud Save later
Why: works offline, easy to move later
AI: suggested a SQLite database — rejected, too heavy for one save file
Who: Yuval
```

---
## 2026-11-3 — A choice of game type
```
Options: Any card game / Hord Survivel / Balancing act
Chose: Balancing act was chosen — this due to both curiocity to explore physics based movement, and an undertanding that physicality in video games is an addictive element, even in mobile games...
AI: Did not consult AI...  
Who: Denis Beigel
```

## 2026-11-19 — Additional service to be added to the game
```
Options: PointTracker / SoundService / SettingService
Chose: SettingService - A simple toggel would be something relativly easy to implement in addition to the connection it already has to the save service which needs to exist for the setting's state to be remembered... 
AI: An attempt was made but took longer then simply implementing...
Who: Denis Beigel
```

## 2026-11-26 — Economy numbers
```
Options: Larg values / Small values
Chose: Small - The choice was more in tune with the feel of the game, a simpler expirace without numbers reaching astronomical numbers, as you need to physicly collect coins in the level... This is just the start tho, the more complete echo sheet will have the speed at which you finish a level acounted for once gameplay is more nailed down (handeling, speeds and the like)
AI: Not yet...
Who: Denis Beigel
```
