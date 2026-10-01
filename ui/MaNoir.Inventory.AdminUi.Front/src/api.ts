import { getPublicBasePath } from './runtimeConfig';

export interface InventoryCounts {
  products: number;
  assets: number;
  containers: number;
  categories: number;
  lowStock: number;
  expiring: number;
}

// The Inventory API is mounted on the same host as the Admin UI, under /v1.0/inventory.
const apiBaseUrl = (
  import.meta.env.VITE_INVENTORY_API_URL
  ?? (import.meta.env.DEV ? '' : getPublicBasePath())
).replace(/\/$/, '');

async function getJson<T>(path: string, signal?: AbortSignal): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    headers: { Accept: 'application/json' },
    signal,
  });

  if (!response.ok) {
    throw new Error(`API ${response.status}`);
  }

  return response.json() as Promise<T>;
}

async function count(path: string, signal?: AbortSignal): Promise<number> {
  const items = await getJson<unknown[]>(path, signal);
  return Array.isArray(items) ? items.length : 0;
}

export async function checkInventoryHealth(signal?: AbortSignal): Promise<void> {
  const response = await fetch(`${apiBaseUrl}/healthz`, { signal });

  if (!response.ok) {
    throw new Error(`API health ${response.status}`);
  }
}

export async function getInventoryCounts(signal?: AbortSignal): Promise<InventoryCounts> {
  const [products, assets, containers, categories, lowStock, expiring] = await Promise.all([
    count('/v1.0/inventory/products', signal),
    count('/v1.0/inventory/assets', signal),
    count('/v1.0/inventory/containers', signal),
    count('/v1.0/inventory/categories', signal),
    count('/v1.0/inventory/stock/low', signal),
    count('/v1.0/inventory/stock/expiring?withinDays=7', signal),
  ]);

  return { products, assets, containers, categories, lowStock, expiring };
}
