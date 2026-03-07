# PLAN — WPF Notepad Homework (implementare incrementală)

Acest plan este gândit pentru începători în WPF: un singur pas funcțional la un moment dat.
Regula de bază: fiecare milestone trebuie să compileze și să poată fi demonstrat înainte de pasul următor.

---

## 📊 Status Progres

| Milestone | Status | Data Finalizare |
|-----------|--------|-----------------|
| Milestone 0 — Setup proiect | ✅ Completat | 2026-03-07 |
| Milestone 1 — Shell UI + tab nou implicit | 🔄 În lucru | — |
| Milestone 2 — Document lifecycle | ⏳ Planificat | — |
| Milestone 3 — Close flows | ⏳ Planificat | — |
| Milestone 4 — Folder Explorer | ⏳ Planificat | — |
| Milestone 5 — Context menu pe folder | ⏳ Planificat | — |
| Milestone 6 — Search & Replace | ⏳ Planificat | — |
| Milestone 7 — View & Help | ⏳ Planificat | — |
| Milestone 8 — Validare finală | ⏳ Planificat | — |

---

## Milestone 0 — Setup proiect ✅
### Scop
Inițializezi aplicația WPF și structura care evită logica în code-behind.

### Task-uri
1. ✅ Creează soluția WPF pe .NET 8.
2. ✅ Creează folderele/namespace-urile pentru Presentation, Application, Domain, Infrastructure.
3. ✅ Configurează dependențele minime (inclusiv toolkit MVVM dacă alegi).
4. ✅ Configurează o clasă de bootstrap pentru injectare servicii.

### Done când
- ✅ Aplicația pornește fără erori.
- ✅ Structura de arhitectură este vizibilă în proiect.

### Ce s-a implementat
- Soluție WPF pe .NET 8 cu proiect NotepadClone
- Pachete instalate: CommunityToolkit.Mvvm 8.4.0, Microsoft.Extensions.DependencyInjection 10.0.3
- Structură completă pe layere:
  - **Domain**: `EditorDocument` (entitate document cu proprietăți observabile)
  - **Application/Interfaces**: `IFileService`, `IDialogService`, `IFolderService`, `IClipboardService`
  - **Infrastructure/Services**: implementări concrete pentru toate interfețele
  - **Presentation/ViewModels**: `MainViewModel` cu DI complet configurat
  - **Presentation/Views**: `MainWindow` cu wiring minim în code-behind
- Bootstrap DI în `App.xaml.cs` cu injecție automată servicii + ViewModels
- Build success, aplicație rulează fără erori

---

## Milestone 1 — Shell UI + tab nou implicit
### Scop
Interfața principală și comportamentul inițial al aplicației.

### Task-uri
1. Creează layout principal:
   - meniu sus
   - toolbar
   - panou stânga pentru Folder Explorer
   - zonă taburi editor
2. Adaugă meniurile cerute:
   - File
   - Search
   - View
   - Help
3. La pornirea aplicației, creează automat un tab nou gol.
4. Denumește documentele noi `File 1`, `File 2`, etc.

### Done când
- La start există un tab gol nou.
- UI-ul de bază este complet și stabil.

---

## Milestone 2 — Operații fișiere (New/Open/Save/Save As)
### Scop
Flux complet de lucru pe documentul curent.

### Task-uri
1. `New file` creează tab gol nou.
2. `Open file...` citește fișier text în tab nou:
   - filtru implicit `.txt`
   - opțiune pentru toate fișierele
3. `Save file` salvează documentul curent:
   - dacă nu are cale, face flow de Save As
4. `Save file as...` salvează documentul la calea aleasă.
5. Titlul tabului arată numele fișierului dacă este salvat pe disc.
6. Documentele modificate și nesalvate au marcaj vizibil (ex: `*`).

### Done când
- Poți crea, deschide, salva și resalva corect documente.
- Titlurile taburilor sunt corecte.

---

## Milestone 3 — Închidere tab curent și close all
### Scop
Protecția datelor nesalvate.

### Task-uri
1. `Close current file` (din tab activ).
2. Dacă tabul are modificări nesalvate, cere confirmare pentru salvare.
3. `Close all files` cu același comportament de confirmare pentru fiecare document modificat.
4. `Exit` trece prin aceeași verificare de documente nesalvate.

### Done când
- Nu se pierd modificări fără confirmare explicită.

---

## Milestone 4 — Folder Explorer (arbore)
### Scop
Navigare în structura de directoare și deschidere rapidă fișiere.

### Task-uri
1. Afișează arbore de directoare și fișiere.
2. Directoarele trebuie să se poată expand/collapse.
3. Dublu click pe fișier -> deschidere conținut în tab nou.

### Done când
- Arborele funcționează fluent pe directoare reale.
- Deschiderea prin dublu click este funcțională.

---

## Milestone 5 — Meniu contextual pe directoare
### Scop
Operații de bază pe foldere din Explorer.

### Task-uri
1. Click dreapta pe director -> meniu:
   - New file
   - Copy path
   - Copy folder
   - Paste folder
2. `New file` creează fișier nou în folderul selectat.
3. `Copy path` copiază calea directorului în clipboard.
4. `Copy folder` memorează folderul sursă pentru copiere recursivă.
5. `Paste folder` copiază recursiv folderul memorat în folderul țintă.
6. Dacă nu există sursă validă pentru paste, `Paste folder` este inactiv.

### Done când
- Toate cele 4 opțiuni merg conform cerinței.

---

## Milestone 6 — Find / Replace / Replace All
### Scop
Căutare și înlocuire pe scope selectabil.

### Task-uri
1. Dialog/zonă pentru `Find...`.
2. Dialog/zonă pentru `Replace...`.
3. `Replace All...`.
4. Scope selectabil:
   - `Selected tab`
   - `All tabs`
5. Refolosește aceeași logică de căutare în servicii, nu direct în code-behind.

### Done când
- Toate cele 3 acțiuni funcționează pe ambele scope-uri.

---

## Milestone 7 — View + Help
### Scop
Finalizarea meniurilor cerute.

### Task-uri
1. `View -> Standard`.
2. `View -> Folder Explorer` (toggle vizibilitate panou explorer).
3. `Help -> About` cu:
   - nume student (placeholder)
   - grupă (placeholder)
   - link email instituțional (placeholder)

### Done când
- Meniurile View și Help sunt conforme.

---

## Milestone 8 — Validare pe barem + pregătire prezentare
### Scop
Asiguri punctaj maxim pe cerințele obligatorii.

### Task-uri
1. Rulează scenarii complete pentru fiecare punct din temă.
2. Verifică faptul că logica principală NU este în code-behind.
3. Pregătește 1-2 demo scenarii scurte pentru:
   - fișiere
   - directoare
   - find/replace
4. Pregătește explicații pentru arhitectură și fluxul comenzilor.

### Done când
- Poți demonstra cap-coadă funcționalitățile obligatorii fără blocaje.

---

## După obligatoriu (opțional +1p)
Adaugă într-un milestone separat doar după finalizare:
- find next / find previous
- copy / cut / paste
- uppercase / lowercase pe text selectat
- go to line
- remove empty lines
- set/unset readonly

---

## Ritm recomandat (3 săptămâni)
- Săptămâna 1: Milestone 0-3
- Săptămâna 2: Milestone 4-6
- Săptămâna 3: Milestone 7-8 + opționale (dacă ai timp)

Dacă întârzii, prioritizează strict cerințele obligatorii ca să eviți depunctarea mare.
