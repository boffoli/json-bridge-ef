# JsonBridgeEF

## 📌 1. Overview
**JsonBridgeEF** è un'applicazione che consente di **mappare dati JSON su un database relazionale**, applicando trasformazioni per adattare le discrepanze tra i dati di origine e il database target.
Il mapping sarà in futuro definito via **GUI**, ma attualmente è simulato tramite un sistema di **regole predefinite**.

---

## 📌 2. Project Structure
L'applicazione è organizzata per separare chiaramente il **database di mapping**, il **database target** e i **dati di test**.

- **`Data/`** → Contiene la gestione del database di mapping e del database target.
  - `JsonBridgeEFContext.cs` → Database per salvare le regole di mapping.
  - `Target/` → Contiene la gestione del database target.
    - `TargetDatabaseService.cs` → Gestisce l'inserimento dei dati nel database target.
    - `TargetSchema.sql` → Definisce lo schema SQL del database target.
- **`Services/`** → Contiene la logica di trasformazione dei dati JSON.
  - `MappingService.cs` → Applica le regole di mapping e inserisce i dati nel DB.
- **`TestData/`** → Contiene dati e strumenti per testare l'applicazione.
  - `Target/` → Strumenti per popolare il database di test.
    - `TestDataSeeder.cs` → Popola il database target con dati di test.
  - `Source/` → Contiene i file JSON di test.
    - `schema.json` → Definisce la struttura dei dati di origine.
    - `input_test.json` → Esempio di dati JSON con discrepanze.
  - `OperatorMappingEmulator.cs` → Simula il mapping definito via GUI.
- **`Program.cs`** → Punto di ingresso per importare i dati JSON.
- **`SetupTargetDatabase.cs`** → Configurazione iniziale del database target.
- **`README.md`** → Documentazione del progetto.

---

## 📌 3. Database Schema
Il database di destinazione è relazionale e organizzato in più tabelle con chiavi esterne:

### **📊 Schema delle Tabelle**
```
Persone (ID, Nome, Cognome, Eta, Sesso)
  └── Indirizzi (ID, PersonaID [FK], Via, Città, CAP)
  └── Contatti (ID, PersonaID [FK], Tipo, Valore)
  └── Ordini (ID, PersonaID [FK], DataOrdine, Totale)
        └── DettagliOrdini (ID, OrdineID [FK], Prodotto, Quantità, Prezzo)
```

### **📝 Descrizione delle Tabelle**
- **`Persone`** → Contiene le informazioni anagrafiche.
- **`Indirizzi`** → Ogni persona può avere più indirizzi.
- **`Contatti`** → Ogni persona può avere più contatti (telefono, email).
- **`Ordini`** → Ogni persona può avere più ordini.
- **`DettagliOrdini`** → Ogni ordine può contenere più prodotti.

---

## 📌 4. JSON Schema
Il formato JSON dei dati di origine può differire dallo Schema del Modello.  
**Esempio di schema JSON:**

```json
{
  "full_name": "Mario Giovanni Rossi",
  "birth_year": 1990,
  "gender": "M",
  "residence": {
    "street": "Via Roma 10",
    "city": "Milano",
    "postal_code": "20121"
  }
}
```

---

## 📌 5. Operatori di Trasformazione
Poiché i dati JSON non corrispondono perfettamente al database, utilizziamo **operatori di trasformazione**.

| JSON Field        | Target Table | Target Column   | Transformation Needed | Description |
|------------------|-------------|----------------|---------------------|-------------|
| `full_name` | `Persone` | `Nome`, `Cognome` | **ExtractWord** | Dividere il nome completo in nome e cognome. |
| `birth_year` | `Persone` | `Eta` | **Convert** | Convertire l'anno di nascita in età (`2024 - birth_year`). |
| `gender` | `Persone` | `Sesso` | **Replace** | M -> Maschio, F -> Femmina, X -> Non specificato. |
| `residence.street` | `Indirizzi` | `Via` | **Rename** | `street` → `Via`. |
| `residence.city` | `Indirizzi` | `Citta` | **Rename** | `city` → `Citta`. |
| `residence.postal_code` | `Indirizzi` | `CAP` | **Rename** | `postal_code` → `CAP`. |

---

## 📌 6. Esecuzione

### **🏰 1. Setup iniziale (solo la prima volta)**
```sh
dotnet run --project SetupTargetDatabase.cs
```
🔹 **Crea il database target e inserisce dati di test**.

---

### **📂 2. Importazione JSON (ogni volta che serve)**
```sh
dotnet run
```
🔹 **Legge i file JSON e li inserisce nel database target.**

---

## 📌 7. Controllo Dati
Dopo l'importazione, puoi verificare i dati con SQLite:
```sh
sqlite3 target.db "SELECT * FROM Persone;"
sqlite3 target.db "SELECT * FROM Indirizzi;"
sqlite3 target.db "SELECT * FROM Contatti;"
sqlite3 target.db "SELECT * FROM Ordini;"
sqlite3 target.db "SELECT * FROM DettagliOrdini;"
```

---

## 📌 8. Conclusione
**JsonBridgeEF** permette di **importare dati JSON nel database relazionale**, gestendo **discrepanze strutturali** tramite trasformazioni automatiche.  
In futuro, il mapping sarà gestito via GUI. 🚀

