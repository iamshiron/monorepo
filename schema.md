# HonamiSystem Database Schema

## Overview

The database is backed by **EF Core** with **PostgreSQL** (uses `pgvector` for vector embeddings). Authentication is handled via ASP.NET Core Identity. All entity tables inheriting from `BaseEntity` include common audit fields.

---

## Base Entity

All entities inheriting from `BaseEntity` include the following columns:

| Column      | Type       | Default                 | Description          |
| ----------- | ---------- | ----------------------- | -------------------- |
| `ID`        | `Guid`     | `Guid.CreateVersion7()` | Unique identifier    |
| `CreatedAt` | `DateTime` | `DateTime.UtcNow`       | Record creation time |
| `UpdatedAt` | `DateTime` | `DateTime.UtcNow`       | Last update time     |

---

## Interfaces

### IObjectStored

Implemented by `ImageHandle` and `FileHandle`.

| Property    | Type      | Description            |
| ----------- | --------- | ---------------------- |
| `ObjectKey` | `string`  | Object storage key     |
| `SizeKb`    | `decimal` | File size in kilobytes |

### ISummarizable

Implemented by `ImageHandle` and `FileHandle`.

| Property  | Type     | Description          |
| --------- | -------- | -------------------- |
| `Summary` | `string` | AI-generated summary |

### IAttachable

Implemented by `ImageHandle`, `FileHandle`, and `WidgetHandle`.

| Property    | Type          | Description                      |
| ----------- | ------------- | -------------------------------- |
| `ID`        | `Guid`        | Unique identifier                |
| `MessageID` | `Guid`        | FK to the parent message         |
| `Message`   | `ChatMessage` | Navigation to the parent message |

---

## Entities

### User

Extends `IdentityUser<Guid>` (ASP.NET Core Identity).

Table name: `Users`

| Column     | Type      | Constraints      | Description                 |
| ---------- | --------- | ---------------- | --------------------------- |
| `Id`       | `Guid`    | PK (Identity)    | Inherited from IdentityUser |
| `Name`     | `string`  | Required, Max 32 | Display name                |
| `UserName` | `string?` | Max 32           | Login username (overridden) |
| `Email`    | `string?` | —                | Inherited from IdentityUser |

**Navigation Collections:**

| Property     | Type                 | Description                      |
| ------------ | -------------------- | -------------------------------- |
| `Agents`     | `IList<Agent>`       | Agents created by this user      |
| `Chats`      | `IList<Chat>`        | Chats created by this user       |
| `ChatGroups` | `IList<ChatGroup>`   | Chat groups created by this user |
| `Personas`   | `IList<Persona>`     | Personas owned by this user      |
| `Skills`     | `IList<Skill>`       | Skills created by this user      |
| `Images`     | `IList<ImageHandle>` | Images uploaded by this user     |
| `Files`      | `IList<FileHandle>`  | Files uploaded by this user      |

---

### Persona

Not tracked by a `DbSet` directly — owned/accessed via `User.Personas` collection.

| Column          | Type            | Constraints            | Description                  |
| --------------- | --------------- | ---------------------- | ---------------------------- |
| `Name`          | `string`        | Required, Max 64       | Persona name                 |
| `Brief`         | `string?`       | Max 256                | Short description            |
| `Instruction`   | `string`        | Max 8192, Default `""` | System prompt / instructions |
| `Traits`        | `IList<string>` | —                      | Personality trait tags       |
| `SpeakingStyle` | `string?`       | —                      | Speaking style description   |

---

### Agent

Extends `BaseEntity`.

Table name: `Agents`

| Column           | Type            | Constraints       | Description                |
| ---------------- | --------------- | ----------------- | -------------------------- |
| `Name`           | `string`        | Required, Max 256 | Agent display name         |
| `Description`    | `string?`       | Max 256           | Agent description          |
| `PersonaID`      | `Guid?`         | FK → `Personas`   | Associated persona         |
| `RequiredTools`  | `IList<string>` | —                 | Tools the agent must have  |
| `SuggestedTools` | `IList<string>` | —                 | Tools the agent may use    |
| `CreatedByID`    | `Guid`          | FK → `Users`      | User who created the agent |

**Navigation Properties:**

| Property    | Type            | Description                         |
| ----------- | --------------- | ----------------------------------- |
| `Persona`   | `Persona?`      | The persona assigned to this agent  |
| `Memories`  | `IList<Memory>` | Memories associated with this agent |
| `CreatedBy` | `User`          | The user who created this agent     |

---

### Skill

Extends `BaseEntity`.

Table name: `Skills`

| Column        | Type     | Constraints                      | Description                |
| ------------- | -------- | -------------------------------- | -------------------------- |
| `Name`        | `string` | Required, Max 64                 | Skill name                 |
| `Description` | `string` | Required, Max 255                | Skill description          |
| `Instruction` | `string` | Required, Max 2048, Default `""` | Skill instructions         |
| `CreatedByID` | `Guid`   | Required, FK → `Users`           | User who created the skill |

**Navigation Properties:**

| Property    | Type   | Description                     |
| ----------- | ------ | ------------------------------- |
| `CreatedBy` | `User` | The user who created this skill |

---

### ChatGroup

Extends `BaseEntity`.

Table name: `ChatGroups`

| Column        | Type     | Constraints      | Description                |
| ------------- | -------- | ---------------- | -------------------------- |
| `Name`        | `string` | Required, Max 64 | Group name                 |
| `CreatedByID` | `Guid`   | FK → `Users`     | User who created the group |

**Navigation Properties:**

| Property    | Type          | Description               |
| ----------- | ------------- | ------------------------- |
| `Chats`     | `IList<Chat>` | Chats within this group   |
| `CreatedBy` | `User`        | The user who created this |

---

### Chat

Extends `BaseEntity`.

Table name: `Chats`

| Column        | Type      | Constraints       | Description               |
| ------------- | --------- | ----------------- | ------------------------- |
| `Title`       | `string`  | Required, Max 64  | Chat title                |
| `Description` | `string?` | Max 256           | Chat description          |
| `ChatGroupID` | `Guid?`   | FK → `ChatGroups` | Optional group assignment |
| `CreatedByID` | `Guid`    | FK → `Users`      | User who created the chat |

**Navigation Collections:**

| Property            | Type                          | Description                        |
| ------------------- | ----------------------------- | ---------------------------------- |
| `ChatGroup`         | `ChatGroup?`                  | The group this chat belongs to     |
| `Memories`          | `IList<Memory>`               | Memories associated with this chat |
| `Messages`          | `IList<ChatMessage>`          | Messages in this chat              |
| `Attachments`       | `IList<MessageAttachment>`    | Attachments in this chat           |
| `UserParticipants`  | `IList<ChatParticipantAgent>` | User participants in this chat     |
| `AgentParticipants` | `IList<ChatParticipantAgent>` | Agent participants in this chat    |
| `CreatedBy`         | `User`                        | The user who created this chat     |

> **Note:** `UserParticipants` is typed as `IList<ChatParticipantAgent>` in code — this may be a typo and should likely be `IList<ChatParticipantUser>`.

---

### ChatMessage

Extends `BaseEntity`.

Table name: `ChatMessages`

| Column            | Type     | Constraints            | Description                      |
| ----------------- | -------- | ---------------------- | -------------------------------- |
| `ChatID`          | `Guid`   | Required, FK → `Chats` | The chat this message belongs to |
| `AgentID`         | `Guid?`  | FK → `Agents`          | Agent sender (nullable)          |
| `UserID`          | `Guid?`  | FK → `Users`           | User sender (nullable)           |
| `Content`         | `string` | Required, Max 1024     | Message content text             |
| `ParentMessageID` | `Guid?`  | FK → `ChatMessages`    | Parent message for threading     |

**Navigation Properties:**

| Property        | Type                       | Description                      |
| --------------- | -------------------------- | -------------------------------- |
| `Chat`          | `Chat`                     | The chat this message belongs to |
| `Agent`         | `Agent?`                   | The agent that sent this message |
| `User`          | `User?`                    | The user that sent this message  |
| `ParentMessage` | `ChatMessage?`             | Parent message (for threading)   |
| `Attachments`   | `IList<MessageAttachment>` | Attachments on this message      |

**Computed Properties:**

| Property   | Type   | Description                                     |
| ---------- | ------ | ----------------------------------------------- |
| `IsUser`   | `bool` | `true` if `UserID` has a value                  |
| `IsAgent`  | `bool` | `true` if `AgentID` has a value                 |
| `IsSystem` | `bool` | `true` if neither `UserID` nor `AgentID` is set |

> **Note:** Exactly one of `UserID`, `AgentID`, or neither (system message) should be set per row.

---

### ChatParticipantUser

Standalone entity (does not extend `BaseEntity`). Links a `User` to a `Chat`.

Table name: `ChatParticipants`

| Column   | Type   | Constraints            | Description             |
| -------- | ------ | ---------------------- | ----------------------- |
| `UserID` | `Guid` | Required, FK → `Users` | The user participant    |
| `ChatID` | `Guid` | Required, FK → `Chats` | The chat they belong to |

**Navigation Properties:**

| Property | Type   | Description                        |
| -------- | ------ | ---------------------------------- |
| `User`   | `User` | The user participating in the chat |
| `Chat`   | `Chat` | The chat this entry belongs to     |

---

### ChatParticipantAgent

Standalone entity (does not extend `BaseEntity`). Links an `Agent` to a `Chat`.

Table name: `ChatParticipantAgents`

| Column         | Type            | Constraints             | Description                  |
| -------------- | --------------- | ----------------------- | ---------------------------- |
| `AgentID`      | `Guid`          | Required, FK → `Agents` | The agent participant        |
| `ChatID`       | `Guid`          | Required, FK → `Chats`  | The chat they belong to      |
| `AllowedTools` | `IList<string>` | —                       | Tools permitted in this chat |

**Navigation Properties:**

| Property | Type    | Description                         |
| -------- | ------- | ----------------------------------- |
| `Agent`  | `Agent` | The agent participating in the chat |
| `Chat`   | `Chat`  | The chat this entry belongs to      |

---

### MessageAttachment

Standalone entity (does not extend `BaseEntity`). Polymorphic join table linking a `ChatMessage` to an `ImageHandle`, `FileHandle`, or `WidgetHandle`.

Table name: `MessageAttachments`

| Column           | Type    | Constraints                   | Description                  |
| ---------------- | ------- | ----------------------------- | ---------------------------- |
| `MessageID`      | `Guid`  | Required, FK → `ChatMessages` | Parent message               |
| `FileHandleID`   | `Guid?` | FK → file handles             | Referenced file (nullable)   |
| `ImageHandleID`  | `Guid?` | FK → image handles            | Referenced image (nullable)  |
| `WidgetHandleID` | `Guid?` | FK → widget handles           | Referenced widget (nullable) |

**Navigation Properties:**

| Property       | Type            | Description                  |
| -------------- | --------------- | ---------------------------- |
| `Message`      | `ChatMessage`   | The message this attaches to |
| `FileHandle`   | `FileHandle?`   | The file attachment          |
| `ImageHandle`  | `ImageHandle?`  | The image attachment         |
| `WidgetHandle` | `WidgetHandle?` | The widget attachment        |

**Computed Properties:**

| Property   | Type          | Description                                                |
| ---------- | ------------- | ---------------------------------------------------------- |
| `ChatID`   | `Guid`        | Delegates to `Message.ChatID`                              |
| `Chat`     | `Chat`        | Delegates to `Message.Chat`                                |
| `IsFile`   | `bool`        | `true` if `FileHandleID` has a value                       |
| `IsImage`  | `bool`        | `true` if `ImageHandleID` has a value                      |
| `IsWidget` | `bool`        | `true` if `WidgetHandleID` has a value                     |
| `Handle`   | `IAttachable` | Resolves to `FileHandle`, `ImageHandle`, or `WidgetHandle` |

> **Note:** Exactly one of `FileHandleID`, `ImageHandleID`, or `WidgetHandleID` should be set per row.

---

### ImageHandle

Extends `BaseEntity`. Implements `IAttachable`, `IObjectStored`, `ISummarizable`.

Table name: `Images` (DbSet: `Images`)

| Column           | Type            | Constraints                   | Description                |
| ---------------- | --------------- | ----------------------------- | -------------------------- |
| `ObjectKey`      | `string`        | Required, Max 256             | Object storage key         |
| `BlurHash`       | `string`        | Required, Max 256             | BlurHash placeholder       |
| `Sha256Hash`     | `string`        | Required, Max 256             | SHA-256 content hash       |
| `Width`          | `int`           | —                             | Image width in pixels      |
| `Height`         | `int`           | —                             | Image height in pixels     |
| `SizeKb`         | `decimal`       | —                             | File size in kilobytes     |
| `PrimaryColor`   | `Color32`       | Required                      | Dominant color             |
| `SecondaryColor` | `Color32`       | Required                      | Secondary color            |
| `Palette`        | `List<Color32>` | Default `[]`                  | Color palette              |
| `Summary`        | `string`        | Required, Max 256             | AI-generated image summary |
| `CreatedByID`    | `Guid`          | Required, FK → `Users`        | Uploading user             |
| `MessageID`      | `Guid`          | Required, FK → `ChatMessages` | Parent message             |

**Navigation Properties:**

| Property    | Type          | Description                     |
| ----------- | ------------- | ------------------------------- |
| `CreatedBy` | `User`        | The user who uploaded this      |
| `Message`   | `ChatMessage` | The message this is attached to |

---

### FileHandle

Extends `BaseEntity`. Implements `IAttachable`, `IObjectStored`, `ISummarizable`.

Table name: `Files` (DbSet: `Files`)

| Column        | Type       | Constraints                   | Description                                |
| ------------- | ---------- | ----------------------------- | ------------------------------------------ |
| `ObjectKey`   | `string`   | Required, Max 256             | Object storage key                         |
| `Summary`     | `string`   | Required, Max 256             | AI-generated file summary                  |
| `SizeKb`      | `decimal`  | Required                      | File size in kilobytes                     |
| `Type`        | `FileType` | Required                      | File type enum (`Text`, `Pdf`, `Markdown`) |
| `CreatedByID` | `Guid`     | Required, FK → `Users`        | Uploading user                             |
| `MessageID`   | `Guid`     | Required, FK → `ChatMessages` | Parent message                             |

**Enum: FileType**

| Value      | Description   |
| ---------- | ------------- |
| `Text`     | Plain text    |
| `Pdf`      | PDF document  |
| `Markdown` | Markdown file |

**Navigation Properties:**

| Property    | Type          | Description                     |
| ----------- | ------------- | ------------------------------- |
| `CreatedBy` | `User`        | The user who uploaded this      |
| `Message`   | `ChatMessage` | The message this is attached to |

---

### WidgetHandle

Extends `BaseEntity`. Implements `IAttachable`.

Table name: `Widgets` (DbSet: `Widgets`)

| Column      | Type     | Constraints                   | Description               |
| ----------- | -------- | ----------------------------- | ------------------------- |
| `WidgetKey` | `string` | Required, Max 256             | Widget type identifier    |
| `Metadata`  | `jsonb`  | Required                      | Arbitrary widget metadata |
| `MessageID` | `Guid`   | Required, FK → `ChatMessages` | Parent message            |

**Navigation Properties:**

| Property  | Type          | Description                     |
| --------- | ------------- | ------------------------------- |
| `Message` | `ChatMessage` | The message this is attached to |

---

### Memory

Extends `BaseEntity`.

Table name: `Memories`

| Column      | Type     | Constraints             | Description                     |
| ----------- | -------- | ----------------------- | ------------------------------- |
| `Key`       | `string` | Required, Max 64        | Memory key / identifier         |
| `Content`   | `string` | Required, Max 255       | Memory content text             |
| `Embedding` | `Vector` | Required (pgvector)     | Vector embedding of the content |
| `AgentID`   | `Guid`   | Required, FK → `Agents` | Associated agent (required)     |
| `ChatID`    | `Guid?`  | FK → `Chats`            | Optional chat context           |

**Navigation Properties:**

| Property | Type    | Description                          |
| -------- | ------- | ------------------------------------ |
| `Agent`  | `Agent` | The agent this memory belongs to     |
| `Chat`   | `Chat?` | Optional chat context for the memory |

> **Note:** Every memory must belong to an `Agent`. The `ChatID` is optional, used for local/scoped context.

---

## Identity Tables (ASP.NET Core Identity)

Managed by `IdentityDbContext`. Table names are explicitly configured:

| Table Name   | Entity Type               | Description         |
| ------------ | ------------------------- | ------------------- |
| `Users`      | `User`                    | Application users   |
| `Roles`      | `IdentityRole<Guid>`      | User roles          |
| `UserRoles`  | `IdentityUserRole<Guid>`  | User-role mappings  |
| `UserClaims` | `IdentityUserClaim<Guid>` | User claims         |
| `UserLogins` | `IdentityUserLogin<Guid>` | External login info |
| `RoleClaims` | `IdentityRoleClaim<Guid>` | Role claims         |
| `UserTokens` | `IdentityUserToken<Guid>` | User tokens         |

---

## Entity Relationships

```
User ──────────────────────────────────────────────────────────────────────────
  │  1:N           1:N           1:N          1:N          1:N
  ├──→ Agent ──→ Memory          │             │            │
  │         │                    │             │            │
  │         ├──→ Persona         │             │            │
  │         ├──→ RequiredTools   │             │            │
  │         └──→ SuggestedTools  │             │            │
  │                              │             │            │
  ├──→ ChatGroup ──→ Chat ──────┤             │            │
  │                  │          │             │            │
  │                  ├──→ Memory│             │            │
  │                  ├──→ ChatMessage ──→ MessageAttachment │
  │                  │          │    │        │             │
  │                  │          │    ├──→ ImageHandle ─────┤
  │                  │          │    ├──→ FileHandle ──────┤
  │                  │          │    └──→ WidgetHandle ────┤
  │                  │          │                         │
  │                  ├──→ ChatParticipantUser             │
  │                  └──→ ChatParticipantAgent            │
  │                                                     │
  ├──→ Skill ───────────────────────────────────────────┘
  ├──→ ImageHandle (via CreatedBy)
  └──→ FileHandle  (via CreatedBy)
```

### Relationship Summary

| From                   | To                  | Type | FK                | Description                           |
| ---------------------- | ------------------- | ---- | ----------------- | ------------------------------------- |
| `Agent`                | `User`              | N:1  | `CreatedByID`     | Agent is created by a user            |
| `Agent`                | `Persona`           | N:1  | `PersonaID`       | Agent has an optional persona         |
| `Agent`                | `Memory`            | 1:N  | —                 | Agent has many memories               |
| `ChatGroup`            | `User`              | N:1  | `CreatedByID`     | Group is created by a user            |
| `ChatGroup`            | `Chat`              | 1:N  | —                 | Group contains many chats             |
| `Chat`                 | `User`              | N:1  | `CreatedByID`     | Chat is created by a user             |
| `Chat`                 | `ChatGroup`         | N:1  | `ChatGroupID`     | Chat optionally belongs to a group    |
| `Chat`                 | `Memory`            | 1:N  | —                 | Chat has many memories                |
| `Chat`                 | `ChatMessage`       | 1:N  | —                 | Chat has many messages                |
| `Chat`                 | `MessageAttachment` | 1:N  | —                 | Chat has many attachments             |
| `ChatMessage`          | `Chat`              | N:1  | `ChatID`          | Message belongs to a chat             |
| `ChatMessage`          | `User`              | N:1  | `UserID`          | Optional user sender                  |
| `ChatMessage`          | `Agent`             | N:1  | `AgentID`         | Optional agent sender                 |
| `ChatMessage`          | `ChatMessage`       | N:1  | `ParentMessageID` | Optional parent (threading)           |
| `ChatMessage`          | `MessageAttachment` | 1:N  | —                 | Message has many attachments          |
| `ChatParticipantUser`  | `User`              | N:1  | `UserID`          | User participating in a chat          |
| `ChatParticipantUser`  | `Chat`              | N:1  | `ChatID`          | The chat the user belongs to          |
| `ChatParticipantAgent` | `Agent`             | N:1  | `AgentID`         | Agent participating in a chat         |
| `ChatParticipantAgent` | `Chat`              | N:1  | `ChatID`          | The chat the agent belongs to         |
| `MessageAttachment`    | `ChatMessage`       | N:1  | `MessageID`       | Attachment belongs to a message       |
| `MessageAttachment`    | `FileHandle`        | N:1  | `FileHandleID`    | Optional file reference               |
| `MessageAttachment`    | `ImageHandle`       | N:1  | `ImageHandleID`   | Optional image reference              |
| `MessageAttachment`    | `WidgetHandle`      | N:1  | `WidgetHandleID`  | Optional widget reference             |
| `ImageHandle`          | `User`              | N:1  | `CreatedByID`     | Image uploaded by a user              |
| `ImageHandle`          | `ChatMessage`       | N:1  | `MessageID`       | Image attached to a message           |
| `FileHandle`           | `User`              | N:1  | `CreatedByID`     | File uploaded by a user               |
| `FileHandle`           | `ChatMessage`       | N:1  | `MessageID`       | File attached to a message            |
| `WidgetHandle`         | `ChatMessage`       | N:1  | `MessageID`       | Widget attached to a message          |
| `Memory`               | `Agent`             | N:1  | `AgentID`         | Memory belongs to an agent (required) |
| `Memory`               | `Chat`              | N:1  | `ChatID`          | Optional chat context                 |
| `Skill`                | `User`              | N:1  | `CreatedByID`     | Skill is created by a user            |
| `User`                 | `Persona`           | 1:N  | —                 | User owns personas                    |

---

## DbSets

Registered in `HonamiSystemDb` context:

| DbSet                   | Entity                 |
| ----------------------- | ---------------------- |
| `Agents`                | `Agent`                |
| `Chats`                 | `Chat`                 |
| `ChatGroups`            | `ChatGroup`            |
| `ChatParticipants`      | `ChatParticipantUser`  |
| `ChatParticipantAgents` | `ChatParticipantAgent` |
| `ChatMessages`          | `ChatMessage`          |
| `Memories`              | `Memory`               |
| `MessageAttachments`    | `MessageAttachment`    |
| `Personas`              | `Persona`              |
| `Skills`                | `Skill`                |
| `Images`                | `ImageHandle`          |
| `Files`                 | `FileHandle`           |
| `Widgets`               | `WidgetHandle`         |
