# Manoir.Inventory

Business domain repo for MaNoir household inventory: stock and possessions.

It owns business truth for consumables (pantry, household products), assets/equipment (appliances, tools, warranty and maintenance), and books/documents. This scope is aligned with the former `Vesta` agent from the legacy home-automation repository.

It is responsible for the domain business logic, its public contracts, its API, and its admin UI. It must not become an accumulation point for transverse logic that belongs to the Core, the Communication Hub, PlatformOps, or composed experiences.

## Repository Role

This repo is intended to contain primarily:

- `packages/MaNoir.Inventory/` (domain logic)
- `packages/MaNoir.Inventory.Contracts/`
- `apps/MaNoir.Inventory.Api/`
- `apps/MaNoir.Inventory.AdminUi/` as the .NET web host of the back office
- `ui/MaNoir.Inventory.AdminUi.Shared/` for shared frontend code
- `ui/MaNoir.Inventory.AdminUi.<Feature>/` for admin frontend modules
- `apps/MaNoir.Inventory.AgentLocal/` only if a strictly mono-domain agent is needed

## Domain Scope

Major objects:

- `Item`: consumable, asset, book, tool, or other; quantity, unit, low-stock threshold, expiration date, location, photos, purchase, warranty, serial number, maintenance schedule, tags, privacy level.
- `Container`: a storage location (cupboard, bin, shelf) inside a room.
- `Category`: a hierarchical classification for items.

Key workflows:

- stock movements (in/out) for consumables, with low-stock alerts;
- expiration tracking and notifications;
- maintenance scheduling for assets;
- CSV import and barcode lookup.

## What This Repository Must Not Become

This repo must not absorb:

- transverse platform primitives;
- Communication Hub ingestion or correlation responsibilities;
- deployment or control-plane logic;
- cross-domain orchestration agents;
- composed front experiences.

## Repository Layout

```text
/
	.github/
	docs/
	eng/
	apps/
	packages/
	ui/
	tests/
	ops/
```
