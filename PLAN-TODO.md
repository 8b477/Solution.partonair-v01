# Plan TODO — Partonair v0.2

Source : `TODO partonair2.txt`. Décisions validées :
- Visibilité : **2 options séparées** (`IsPublic` = visible par les autres / `ShowSelfInDirectory` = je vois ma propre carte dans mes recherches).
- Recruteur : **champs structurés** à la place de « Compétences ».
- Blocage : la personne bloquée voit **l'historique en lecture seule** avec un bandeau.
- Notifications temps réel : **nouvelle demande d'ami** uniquement pour cette itération.

Ordre conseillé : **0 → 1 → 2 → 3 → 4 → 5 → 6** (le bug temps réel d'abord, il conditionne les notifs).

Légende : `[BDD]` migration EF Core · `[API]` backend · `[FRONT]` React/Tailwind

---

## 0. 🔥 Bug : les messages ne s'affichent pas en temps réel

**Cause identifiée** : `src/pages/messages/Messages.jsx` (handler `connection.on("MessageReceived")`).
La connexion SignalR est créée une seule fois (`useEffect` sur `[auth?.userId]`), donc le handler garde la valeur de `friendId` du **premier rendu**. Si on arrive sur `/messages` puis qu'on clique sur un ami, `friendId` reste `undefined` dans le handler et le filtre `belongsToConversation` rejette **tous** les messages, y compris ceux qu'on envoie soi-même (l'envoi passe par le hub, qui renvoie le message à l'expéditeur).

- [x] [FRONT] Ajouter `const friendIdRef = useRef(friendId)` synchronisé dans un `useEffect([friendId])`, et lire `friendIdRef.current` dans le handler
- [x] [FRONT] Vider `history` au changement de conversation pour ne pas afficher l'ancienne pendant le chargement
- [x] [FRONT] Après `onreconnected`, recharger l'historique REST (messages manqués pendant la coupure)
- [x] [FRONT] Sortir la connexion SignalR dans un `RealtimeProvider` (contexte global, une connexion par session) → réutilisée par la messagerie **et** les notifications (§4)
- [x] [API] `MessageHub` → `RealtimeHub` sur `/hubs/realtime`
- [x] [API] Envoi REST de secours : diffuse aussi `MessageReceived` via `IHubContext`
- [ ] Test manuel : 2 navigateurs (ou 1 normal + 1 privé, `localStorage` partagé entre onglets !) → A envoie, A et B voient le message sans F5

> ⚠️ Deux onglets du **même** navigateur partagent `localStorage.auth` : le second login écrase le token du premier. Pour tester A ↔ B, utiliser une fenêtre privée ou un autre navigateur.

---

## 1. Page « Mon profil » (après création + édition)

### 1.1 Redirection après création
- [x] [FRONT] `Profile-setup.jsx` : `navigate("/profile")` au lieu de `navigate("/profiles")`
- [x] [FRONT] Mettre aussi `role` à jour dans le contexte auth après création (`updateProfileId` → `updateAuth({ profileId, role })`), sinon `auth.role` reste `Visitor` jusqu'au prochain login

### 1.2 Données / BDD
- [x] [BDD] `ProfileUser` : ajouter
  - `ShowSelfInDirectory` (bool, défaut `true`)
  - `AvatarSeed` (string, défaut = `Guid` aléatoire à la création)
  - `AvatarRegenCount` (int) + `AvatarRegenDate` (DateOnly) → quota journalier
- [x] [API] `ProfileViewDTO` / `ProfileUpdateDTO` / `PublicProfileViewDTO` / `MyContactViewDTO` : exposer `ShowSelfInDirectory`, `AvatarSeed`, `AvatarRegenRemaining`
- [x] [API] `LoginDefaultResponse` : ajouter `AvatarSeed` (pour la nav)

### 1.3 Endpoints
- [x] [API] `PUT /api/Profile/{id}` : vérifier que le profil appartient à l'utilisateur du JWT (aujourd'hui rien ne l'empêche de modifier le profil d'un autre)
- [x] [API] ~~`PATCH /api/User/me/name`~~ → intégré à `PUT /api/Profile/me` (+ `GET /api/Profile/me`) : modifier `User.Name` + mettre à jour les `Contact.ContactName` dénormalisés → renvoie le nouveau nom, le front met à jour `auth.userName`
- [x] [API] `POST /api/Profile/me/avatar/regenerate` :
  - reset du compteur si `AvatarRegenDate != aujourd'hui (UTC)`
  - refus `429` si `AvatarRegenCount >= 10`
  - sinon nouveau seed aléatoire, `count++`, renvoie `{ avatarSeed, remaining }`
- [x] [API] `GetByRoleAsyncService` : recevoir l'id de l'utilisateur courant et exclure **son propre profil** si `ShowSelfInDirectory == false`

### 1.4 Avatar
- [x] [FRONT] `Profile-avatar.jsx` : prendre une prop `seed` (fallback actuel sur le prénom si absent). Aujourd'hui le seed est le prénom → **tous les « Jean » ont le même avatar**
- [x] [FRONT] Passer `avatarSeed` partout où `ProfileAvatar` est utilisé (Profiles, Profile-details, Network, Messages, Nav)

### 1.5 UI `Profile.jsx` (refonte en cards Tailwind, thème `glass-panel` / `electric-*`)
- [x] Corriger le texte en dur `Nom user` dans le header et la comparaison `auth.role == ROLE_LABELS.Company`
- [x] **Card header** : grand avatar DiceBear + bouton « 🎲 Nouvel avatar (x/10 restants) » désactivé à 0, nom, badge rôle (libellés FR), badges Public/Privé
- [x] **Card « Visibilité »** : 2 toggles avec explication
  - « Profil public » : les autres te voient dans l'annuaire
  - « M'afficher dans mes recherches » : tu vois ta propre carte dans la liste
- [x] **Card « À propos »** : description
- [x] **Card « Compétences »** (Employee) ou **« Ce que je recherche »** (Company, cf. §3) : tags/chips
- [x] **Card « CV »** + **Card « Évaluation »** (étoiles)
- [x] Mode édition : bouton « Modifier » par card (ou global) → formulaire inline, validation identique au backend (description ≥ 20, skills ≤ 500), toast succès/erreur
- [x] Layout : grille `md:grid-cols-3` (header pleine largeur, visibilité en colonne latérale)

---

## 2. Navbar : avatar au lieu du nom

- [x] [FRONT] `Nav.jsx` : remplacer le bouton `{auth.userName}` par un bouton rond `ProfileAvatar` (h-10 w-10, ring `electric-400` au hover/actif), `aria-label="Mon profil"`, `title={auth.userName}`
- [x] [FRONT] L'espacer du bouton Déconnexion (`gap-4` + séparateur vertical léger), le rendre visible aussi en mobile
- [x] [FRONT] Mettre l'avatar à jour en direct après régénération (seed stocké dans le contexte auth)

---

## 3. Profil Company / Recruteur : champs structurés

- [x] [BDD] `ProfileUser` : ajouter (nullable, utilisés uniquement si `Role == Company`)
  - `SoughtPosition` (string ≤ 150) — poste / profil recherché
  - `ContractTypes` (string, enum flags ou CSV : CDI, CDD, Freelance, Stage, Alternance)
  - `Location` (string ≤ 100) + `RemotePolicy` (enum : OnSite, Hybrid, FullRemote)
  - `Sector` (string ≤ 100)
- [x] [API] DTO create/update/view + mapper ; validation conditionnelle :
  - Employee → `Skills` obligatoire
  - Company → `SoughtPosition` obligatoire, `Skills` optionnel (« compétences recherchées »)
- [x] [FRONT] `Profile-setup.jsx` : afficher le bloc de champs selon le rôle choisi
- [x] [FRONT] `Profile.jsx` / `Profile-details.jsx` / cartes `Profiles.jsx` : afficher ces champs pour les recruteurs
- [ ] [FRONT] (bonus) filtres contrat/remote dans l'annuaire des recruteurs

---

## 4. Notification de nouvelle demande d'ami

- [x] [API] Hub unique `RealtimeHub` (fait au lot 0)
- [x] [API] Dans `ContactController.CreateRequestAsync`, après succès : `IHubContext<…>.Clients.User(receiverId).SendAsync("FriendRequestReceived", { contactId, fromUserId, fromUserName, avatarSeed })`
- [x] [API] ~~Endpoint `count`~~ → le badge réutilise `GET /api/Contact/requests/incoming` (chargé à la connexion et après reconnexion)
- [x] [FRONT] `RealtimeProvider` (§0) : écoute `FriendRequestReceived` → incrémente `pendingCount` + affiche un toast « X veut t'ajouter » avec bouton « Voir »
- [x] [FRONT] Composant `Toast` réutilisable (pile en bas à droite, auto-dismiss 5 s)
- [x] [FRONT] `Nav.jsx` : badge rouge avec compteur sur « Réseau »
- [x] [FRONT] `Network.jsx` :
  - ouvrir par défaut l'onglet « Demandes reçues » s'il y en a
  - onglet mis en avant (pastille + couleur) quand `incoming > 0`
  - recharger la liste à la réception d'un `FriendRequestReceived`
  - décrémenter le badge après accepter/refuser

---

## 4 bis. Notifications « non consulté » (messages)

- [x] [BDD] `Message.ReadAt` + index `IX_Messages_Unread` (migration `AddMessageReadAt`, l'historique existant est marqué lu)
- [x] [API] `GET /api/Message/unread` → `{ total, conversations: [{ userId, count }] }`
- [x] [API] `POST /api/Message/{otherUserId}/read` → marque lus les messages reçus de ce contact
- [x] [API] `MessageViewDTO` : + `ReadAt`, `SenderName`, `SenderAvatarSeed`
- [x] [FRONT] Toast « Nouveau message de X » (aperçu + « Répondre ») si la conversation n'est pas ouverte
- [x] [FRONT] Badge non-lus sur « Messages » (nav) + par ami dans la liste de la messagerie
- [x] [FRONT] Ouvrir une conversation la marque lue ; message reçu dans la conversation ouverte = lu directement

---

## 5. Messagerie : distinguer « Moi » / l'autre

- [x] [FRONT] `Messages.jsx` : dans chaque bulle, afficher l'en-tête
  - mes messages : `Moi · 14:32` (aligné à droite)
  - reçus : `Prénom Nom · 14:32` + mini avatar à gauche
- [x] [FRONT] Regrouper les messages consécutifs du même auteur (en-tête affiché seulement sur le premier du groupe)
- [x] [FRONT] Séparateurs de date (« Aujourd'hui », « Hier », « 12 oct. »)
- [x] [API] (fait au 4 bis) `SenderName` dans `MessageViewDTO` pour ne pas dépendre de la liste d'amis côté front

---

## 6. Blocage : comportement côté bloqueur et bloqué

Règles : A bloque B → B ne peut plus écrire à A, B ne peut plus envoyer de demande à A, **seul A** peut débloquer.

### Backend
- [x] [API] Nouveau `GET /api/Contact/relationship/{otherUserId}` → `{ status: "None" | "Friends" | "PendingOutgoing" | "PendingIncoming" | "BlockedByMe" | "BlockedByOther" }`
- [x] [API] Nouveau `GET /api/Contact/conversations` (amis + bloqués dans les deux sens, `Status` = `Accepted` / `BlockedByMe` / `BlockedByOther`) ; `friends` reste inchangé pour le Réseau
- [x] [API] `GetConversationAsyncService` : si A a bloqué B, autoriser **B** (et A) à lire l'historique, avec un flag `{ messages, canSend: false, blockedBy: "me" | "other" }`. Garder le refus d'envoi dans `SendAsyncService`
- [x] [API] `CreateAsyncService` : message d'erreur explicite si bloqué (déjà refusé, vérifier le code `409` + message)
- [x] [API] `UnlockContactRequestAsyncService` : déjà limité au bloqueur (relation `Sender == moi`) ✔️ (test auto à écrire : pas de projet de tests pour l'instant)
- [x] [API] Événement temps réel `RelationshipChanged` (blocage/déblocage) envoyé aux deux utilisateurs → messagerie, réseau et fiche profil se rafraîchissent

### Frontend
- [x] [FRONT] `Messages.jsx` : si `canSend === false`
  - bandeau `« {nom} t'a bloqué, tu ne peux plus lui écrire »` (B) / `« Tu as bloqué {nom} — Débloquer »` (A)
  - input + bouton Envoyer désactivés, historique visible
  - contact affiché grisé dans la liste de gauche
- [x] [FRONT] `Profile-details.jsx` : appeler `relationship/{userId}` et adapter le bouton (« Ajouter » / « Demande envoyée » / « Amis » / « Débloquer » / masqué si `BlockedByOther`)
- [x] [FRONT] `Network.jsx` : onglet « Bloqués » = uniquement ceux que **j'ai** bloqués (déjà le cas) ✔️

---

## 7. Évaluations (avis entre membres)

Règles : seuls les **amis non bloqués** peuvent évaluer · **un avis par personne** (modifiable / supprimable par son auteur) · note **0 à 5** + commentaire **10 à 500** caractères · l'évalué ne peut que lire · notification temps réel.

- [x] [BDD] Index unique `(fk_eval_sender, fk_eval_receiver)` + `ProfileUsers.rating_average` / `rating_count` (migration `AddEvaluationRatings`, dédoublonne et initialise les stats existantes)
- [x] [API] Ancienne couche Evaluation (non sécurisée, inutilisée) remplacée : `[Authorize]`, auteur déduit du JWT
- [x] [API] `GET /api/Evaluation/user/{userId}` → moyenne, nombre, liste, mon avis, `canEvaluate`
- [x] [API] `PUT /api/Evaluation/user/{userId}` (création ou modification) · `DELETE /api/Evaluation/user/{userId}`
- [x] [API] Moyenne / nombre / `Stars` recalculés sur le profil à chaque changement ; exposés dans l'annuaire, la fiche et « Mon profil »
- [x] [API] Temps réel : `EvaluationReceived` (toast) et `EvaluationRemoved` vers le membre évalué
- [x] [FRONT] `StarRating` (affichage, demi-étoiles) + `StarRatingInput` (0 à 5)
- [x] [FRONT] `EvaluationsSection` : résumé, formulaire (ami uniquement), mon avis mis en avant, liste avec avatar / nom / rôle / date
- [x] [FRONT] Fiche publique : note dans l'en-tête + section avis · « Mon profil » : avis reçus (lecture seule, live) · cartes de l'annuaire : note + nombre d'avis
- [x] [FRONT] Toast « X t'a laissé un avis ★★★★☆ »

---

## Tests / validation (à chaque lot)
- [ ] `dotnet build` + `npm run lint` + `npm run build`
- [ ] Migrations : une seule migration par lot (`AddProfileVisibilityAndAvatar`, `AddRecruiterFields`), testée sur la BDD Docker
- [ ] Scénarios manuels A/B (2 navigateurs) : message temps réel, demande d'ami → toast + badge, blocage → bandeau + lecture seule, quota avatar (11ᵉ clic refusé)

## Décisions complémentaires
- ✅ Un seul hub `RealtimeHub` (`/hubs/realtime`) pour messages + notifications, une seule connexion WebSocket par session.
- ✅ Listes recruteur génériques (enums back, extensibles plus tard) :
  - **Contrat** (multi-choix) : CDI, CDD, Freelance, Stage, Alternance
  - **Télétravail** : Sur site, Hybride, Full remote
  - **Secteur** : Informatique / Tech, Commerce / Vente, Marketing / Communication, Finance / Comptabilité, RH, Industrie / Production, Santé / Social, BTP / Construction, Logistique / Transport, Autre
