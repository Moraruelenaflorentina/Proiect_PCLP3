# Story Engine – Motor de Poveste Interactivă

## Structura soluției

```
StoryEngine.sln
├── Story.Model          – Clase de date (StoryDefinition, StoryBlock, etc.)
├── Story.Engine         – Logica de joc (GameEngine, GameState, ConditionEvaluator)
├── Story.Persistence    – Citire/scriere ZIP + validare
├── Story.Player.WinForms – Aplicația de CITIRE/RULARE a poveștii
└── Story.Editor.WinForms – Aplicația de EDITARE a poveștii
```

## Cerințe

- .NET 8.0 SDK (Windows)
- Visual Studio 2022 (recomandat) sau Rider

## Cum se compilează

```
dotnet build StoryEngine.sln
```

Sau deschide `StoryEngine.sln` în Visual Studio și apasă **Build → Build Solution**.

## Cum se rulează

### Player (cititor)
```
dotnet run --project Story.Player.WinForms
```
→ **File → Open Story** → selectează `MareaEvadare_Example.zip`

### Editor
```
dotnet run --project Story.Editor.WinForms
```

## HUD – Progress Bars

Proprietățile de stare care conțin `player.life` / `health` → bara **Health** (verde)  
Proprietățile care conțin `player.energy` / `stamina` → bara **Stamina** (albastru)  
Proprietățile care conțin `player.level` / `level` → bara **Level** (auriu)

Poți forța asocierea prin câmpul `hudType`:
```json
{ "key": "player.life", "hudType": "health", ... }
{ "key": "player.energy", "hudType": "stamina", ... }
{ "key": "player.level", "hudType": "level", ... }
```

## Formatul fișierului de poveste (.zip)

```
poveste.zip
├── story.json        ← fișierul principal
└── images/
    ├── fundal.jpg    ← imagini de fundal pentru blocuri
    └── icon_cheie.png ← iconițe pentru decizii
```

## Adăugarea imaginilor

### Editor
- Selectează un bloc → **Select Image...** pentru imaginea de fundal
- Editează o decizie → **Select Icon...** pentru iconița deciziei

### Manual (în JSON)
```json
{ "id": "forest.clearing", "backgroundImage": "images/forest.jpg", ... }
{ "text": "Ia sabia", "icon": "images/icon_sword.png", ... }
```

## Condițiile deciziilor (AST JSON)

```json
// Condiție simplă
{ "type": "COMPARISON", "property": "inventory.money", "operator": ">=", "value": 5 }

// AND
{ "type": "AND", "conditions": [ {...}, {...} ] }

// OR  
{ "type": "OR", "conditions": [ {...}, {...} ] }
```

Operatori acceptați: `<`, `<=`, `>`, `>=`, `==`, `!=`

## Efectele deciziilor

```json
{ "type": "ADD", "property": "player.life", "value": -20 }   // scade viața cu 20
{ "type": "SET", "property": "story.hasKey", "value": 1 }    // setează direct la 1
```
