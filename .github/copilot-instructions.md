# Manoir.Inventory - Project Guidelines

## Scope

This repository hosts the `Inventory` business domain of MaNoir (stock and possessions).

It is allowed to contain:

- the domain business logic;
- public contracts of the domain;
- the domain API;
- the domain admin UI;
- a local domain agent only if it is strictly mono-domain.

It must not become the default location for:

- platform-wide transverse primitives;
- Communication Hub ingestion or correlation responsibilities;
- PlatformOps or deployment control logic;
- cross-domain orchestration agents;
- composed front experiences.

## Architecture

This repo owns business truth for `Inventory`.

Use these rules:

- Domain logic stays in the domain, not in API, UI, or orchestration code.
- Admin UI and API consume the domain through explicit application surfaces.
- If a feature owns runtime control or deployment behavior, it belongs in PlatformOps.
- If a feature only orchestrates several domains, it belongs in an agents family, not here.
- If a concern is shared across all domains, challenge whether it belongs in Core instead.

## Packaging

Keep these rules:

- publish `MaNoir.Inventory.Contracts` when cross-repo public contracts are needed;
- publish `MaNoir.Inventory.Client` only when there is a real external consumption need;
- keep `Domain`, `Api`, `AdminUi`, and local agent implementation internal by default.

Do not publish internal implementation packages for convenience.

## Repository Layout

Prefer this root layout vocabulary:

- `apps/` for executable projects like `MaNoir.Inventory.Api`;
- `packages/` for `MaNoir.Inventory`, `MaNoir.Inventory.Contracts`, and reusable libraries;
- `apps/MaNoir.Inventory.AdminUi/` for the .NET admin host when a server host exists;
- `ui/` for frontend admin modules such as `MaNoir.Inventory.AdminUi.<Feature>` and shared frontend code;
- `tests/` for test projects.

Do not invent competing root folders such as `bo/`, `pages/`, `frontend/`, or `backend`.
