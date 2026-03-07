# Notepad Clone — WPF Homework Project

Aplicație WPF tip Notepad++ implementată în C# pe .NET 8, cu arhitectură Clean + MVVM.

## 🎯 Obiectiv
Dezvoltare incrementală a unei aplicații de editare text multifile, cu funcționalități moderne:
- Gestionare multi-tab cu stare document
- Operații pe fișiere (New/Open/Save/Save As)
- Folder Explorer arborescent
- Search & Replace (scoped: tab curent / toate taburile)
- Meniu contextual pe foldere (New file, Copy path, Copy/Paste folder)

## 📦 Tehnologii
- **.NET 8** (Windows-only)
- **WPF** (Windows Presentation Foundation)
- **CommunityToolkit.Mvvm** pentru MVVM pattern
- **Microsoft.Extensions.DependencyInjection** pentru IoC

## 📁 Arhitectură
Proiectul respectă separarea pe layere conform Clean Architecture:

```
NotepadClone/
├── Domain/                    # Entități și reguli de domeniu
│   └── EditorDocument.cs
├── Application/               # Use-cases și interfețe servicii
│   ├── Interfaces/
│   │   ├── IFileService.cs
│   │   ├── IDialogService.cs
│   │   ├── IFolderService.cs
│   │   └── IClipboardService.cs
│   └── Services/
├── Infrastructure/            # Implementări concrete (I/O, dialoguri)
│   └── Services/
│       ├── FileService.cs
│       ├── DialogService.cs
│       ├── FolderService.cs
│       └── ClipboardService.cs
├── Presentation/              # UI și ViewModels (MVVM)
│   ├── Views/
│   │   └── MainWindow.xaml
│   ├── ViewModels/
│   │   └── MainViewModel.cs
│   ├── Commands/
│   └── Converters/
└── App.xaml                   # Bootstrap aplicație + DI setup
```

**Regula de aur:** Fără logică în code-behind (doar `InitializeComponent` și wiring UI minim).

## ✅ Progres

### Milestone 0 — Setup proiect ✔️
- [x] Soluție WPF pe .NET 8 creată
- [x] Structură pe layere (Domain, Application, Infrastructure, Presentation)
- [x] CommunityToolkit.Mvvm + Microsoft.Extensions.DependencyInjection instalate
- [x] Dependency Injection configurat în App.xaml.cs
- [x] Toate serviciile cu interfețe implementate
- [x] Build success + aplicația pornește fără erori

### Milestone 1 — Shell UI + tab nou implicit ✔️
- [x] Layout principal (meniu, toolbar, Folder Explorer, TabControl)
- [x] Meniuri: File, Search, View, Help cu toate submeniurile
- [x] Toolbar cu butoane pentru comenzi frecvente
- [x] Tab gol creat automat la pornire (nume: `File 1`)
- [x] Toate comenzile File funcționale (New, Open, Save, Save As)
- [x] Toggle pentru Folder Explorer (View menu)
- [x] Tracking IsDirty pentru documente modificate

### Milestone 2 — Document lifecycle ✔️
- [x] `New file` → tab nou
- [x] `Open file...` → dialog + încărcare în tab
- [x] `Save` / `Save As` → persistență pe disc
- [x] Titluri tab corecte + marker pentru unsaved (`*`)

### Milestone 3 — Close flows ✔️
- [x] `Close current file` cu buton X pe fiecare tab
- [x] Prompt salvare pentru documente modificate
- [x] `Close all files` cu protecție date per document
- [x] `Exit` cu verificare completă documente nesalvate

### Milestone 4 — Folder Explorer
- [ ] Tree view cu directoare/fișiere
- [ ] Expand/collapse lazy
- [ ] Dublu-click fișier → tab nou

### Milestone 5 — Context menu pe folder
- [ ] `New file`, `Copy path`, `Copy folder`, `Paste folder`
- [ ] `Paste folder` disabled când nu există sursă validă

### Milestone 6 — Search & Replace
- [ ] `Find...`, `Replace...`, `Replace All...`
- [ ] Scope: `Selected tab` / `All tabs`

### Milestone 7 — View & Help
- [ ] `View -> Standard / Folder Explorer` toggle
- [ ] `Help -> About` cu informații student

### Milestone 8 — Validare finală + demo

## 🚀 Cum să rulezi
```powershell
cd NotepadClone
dotnet build
dotnet run
```

## 📚 Documentație helper
- [PLAN.md](../PLAN.md) — Pași detaliați de implementare
- [.github/copilot-instructions.md](../.github/copilot-instructions.md) — Reguli arhitectură pentru Copilot

## 🎓 Barem temă (10p)
1. Operații pe fișiere: **3p**
2. Operații pe structură directoare: **3p**
3. Operații de căutare și înlocuire text: **2p**
4. Funcționalități de vizualizare: **1p**
5. Oficiu: **1p**

**Depunctări:**
- -2p dacă logica este toată în code-behind
- -3p dacă tema e prezentată cu întârziere

**Bonus:**
- +1p funcționalități opționale (find next/previous, copy/cut/paste, uppercase/lowercase, go to line, remove empty lines, set/unset readonly)

---
**Status curent:** Milestones 0-3 completate ✅ — File operations + Close flows funcționale! Ready for Milestone 4 (Folder Explorer)
