# Copilot Instructions — WPF Notepad Homework

Acest repository este pentru o aplicație WPF tip Notepad++, implementată incremental pentru învățare și prezentare la laborator.

## Obiectiv principal
- Respectă cerințele temei exact, fără a adăuga funcționalități extra înainte de finalizarea celor obligatorii.
- Păstrează codul ușor de explicat de către student.
- Evită depunctarea: nu pune toată logica aplicației în code-behind.

## Arhitectură cerută (Clean + MVVM)
Folosește această structură minimă:
- `src/NotepadClone.Presentation` (WPF)
  - `Views` (XAML)
  - `ViewModels`
  - `Commands`
  - `Converters` (doar dacă sunt necesare)
- `src/NotepadClone.Application`
  - Use-cases / coordinare logică aplicație
  - Interfețe pentru servicii (`IFileService`, `IFolderService`, `IDialogService`, `IClipboardService`)
- `src/NotepadClone.Domain`
  - Entități și reguli de domeniu (`EditorDocument`, `SearchScope`, etc.)
- `src/NotepadClone.Infrastructure`
  - Implementări concrete pentru I/O fișiere, explorare foldere, clipboard, dialoguri

Dacă proiectul rămâne într-un singur proiect WPF (pentru simplitate), păstrează aceleași separări prin foldere și namespace-uri.

## Reguli obligatorii de implementare
1. Toate acțiunile de meniu și toolbar folosesc `ICommand` (RelayCommand).
2. Code-behind se limitează la wiring UI minim (ex: `InitializeComponent`).
3. Orice acces la disc/clipboard/dialoguri trece prin servicii injectate.
4. Fiecare tab reprezintă un document cu stare proprie:
   - Titlu
   - Cale fișier (opțională)
   - Text
   - Indicator modificat/nesalvat (`IsDirty`)
5. Documentele noi se numesc `File 1`, `File 2`, ...
6. Documentele nesalvate au marcaj vizibil în titlu (ex: `*`).
7. La `Close` și `Close all` se cere salvare dacă documentul este modificat.
8. `Find`, `Replace`, `Replace All` trebuie să suporte explicit:
   - `Selected tab`
   - `All tabs`
9. `Open` are filtru implicit pentru `.txt`, dar permite și alte tipuri (`*.*`).
10. Folder Explorer trebuie să permită:
    - expand/collapse directoare
    - dublu click pe fișier -> deschidere în tab nou
    - click dreapta pe director: `New file`, `Copy path`, `Copy folder`, `Paste folder`
    - `Paste folder` dezactivat când nu există folder copiat

## Stil de cod
- Nume clare, fără variabile cu o literă.
- Metode mici și ușor de explicat.
- Evită logica duplicată.
- Nu introduce comentarii inutile; codul trebuie să fie auto-explicativ.
- Nu hardcoda string-uri de UI dacă pot fi centralizate.

## Ordinea de lucru
Urmează strict pașii din `PLAN.md`.
- Nu începe milestone-ul următor până când milestone-ul curent compilează și rulează.
- După fiecare milestone:
  - build verde
  - test manual minim pe funcționalitatea nouă
  - actualizare scurtă în progres (ce merge, ce rămâne)

## Criterii de acceptanță pentru fiecare feature
Pentru orice funcționalitate implementată, verifică:
- Funcționează din meniu.
- Funcționează și din toolbar (unde există buton dedicat).
- Nu rupe celelalte funcționalități.
- Respectă separarea MVVM și servicii.

## Despre funcționalitățile opționale (+1p)
Nu implementa opționalele înainte de a finaliza toate cerințele obligatorii.
După finalizare, opționalele se adaugă în milestone separat.

## Focus pentru prezentare
Codul trebuie să poată fi explicat clar:
- fluxul unei comenzi (UI -> Command -> Service -> rezultat)
- motivul separării pe layere
- cum este prevenită pierderea datelor nesalvate
- cum se aplică scope-ul `Selected tab` vs `All tabs`
