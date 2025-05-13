# 🔧 PaleoPlatform – Backend

## Descrizione

Il backend di **PaleoPlatform** è stato sviluppato con **ASP.NET Core Web API** e rappresenta il motore principale della piattaforma. Gestisce l’autenticazione, le autorizzazioni basate sui ruoli, la logica di business e l’interazione con il database relazionale.

La piattaforma è progettata per offrire contenuti scientifici ed educativi nel campo della paleontologia, supportando articoli pubblicati da moderatori e amministratori, discussioni pubbliche in stile Reddit, commenti, un sistema di voti, gestione di eventi e vendita di prodotti.

## Tecnologie Utilizzate

- 🧱 **ASP.NET Core Web API** – Framework per la costruzione di API RESTful.
- 🔐 **ASP.NET Identity** – Sistema di autenticazione e gestione utenti con ruoli.
- 🗃️ **Entity Framework Core** – ORM per la gestione del database.
- 🛡️ **JWT (JSON Web Tokens)** – Autenticazione stateless sicura.
- 🐘 **SQL Server** – Database relazionale per la persistenza dei dati.
- 📦 **AutoMapper** – Mappatura tra entità e DTO.
- 📁 **Upload di File** – Supporto per immagini di copertina e contenuti media.

## Funzionalità Principali

- Registrazione e login con assegnazione ruolo predefinito (`Utente`).
- Gestione ruoli: `Utente`, `Moderatore`, `Amministratore`.
- Creazione, modifica e cancellazione di articoli da parte di moderatori e admin.
- Sistema di commenti e risposte annidate con upvote/downvote.
- Discussioni pubbliche con categorizzazione tramite topic (gestiti da admin/mod).
- Sezione eventi con prenotazione biglietti.
- Sezione shop con gestione dei prodotti e immagini.
- Rotte protette da autenticazione e autorizzazione tramite JWT.
- Supporto a utenti sospesi o eliminati, con commenti assegnati a uno user "deleted".

## Architettura

- API REST strutturate in controller (Articoli, Commenti, Discussioni, Utenti, Eventi, Prodotti).
- DTO (Data Transfer Object) per separare la logica di dominio dai dati esposti.
- Servizi centralizzati per la logica applicativa.
- Seed iniziale di ruoli e admin durante l'avvio.
