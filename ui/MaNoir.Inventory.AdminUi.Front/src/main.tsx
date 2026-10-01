import { StrictMode, useEffect, useState } from 'react';
import type * as React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Navigate, Route, Routes, useLocation, useNavigate } from 'react-router-dom';
import {
  Card,
  DefaultAdminShell,
  PageHeader,
  SidebarNav,
  ShellHeader,
  StatusDot,
} from '@manoir-app/core-admin-ui-kit';
import { checkInventoryHealth, getInventoryCounts, type InventoryCounts } from './api';
import { getRouterBasePath } from './runtimeConfig';
import '@manoir-app/core-admin-ui-kit/styles.css';
import manoirLogo from './assets/logo.svg';
import './app.css';

type PageId = 'overview' | 'products' | 'lowStock' | 'assets' | 'containers' | 'categories';

// Paths are relative to the router basename (the public prefix, /inventory once deployed).
// Keep them in sync with the relativePath values of manoir.plugin.yaml and InventoryPluginDescriptorProvider.
const pages: Record<PageId, { path: string; title: string; description: string }> = {
  overview: { path: '/overview', title: "Vue d'ensemble", description: "L'état de votre inventaire en un coup d'œil." },
  products: { path: '/stock/products', title: 'Produits', description: 'La gestion des produits consommables sera bientôt disponible ici.' },
  lowStock: { path: '/stock/low', title: 'Stock bas et péremptions', description: 'Le suivi des stocks bas et des dates de péremption sera bientôt disponible ici.' },
  assets: { path: '/possessions/assets', title: 'Biens et équipements', description: 'La gestion de vos biens, garanties et entretiens sera bientôt disponible ici.' },
  containers: { path: '/organisation/containers', title: 'Rangements', description: 'La gestion de vos rangements sera bientôt disponible ici.' },
  categories: { path: '/organisation/categories', title: 'Catégories', description: 'La gestion des catégories sera bientôt disponible ici.' },
};

const sections: { id: string; title: string; items: PageId[] }[] = [
  { id: 'inventory', title: 'Inventaire', items: ['overview'] },
  { id: 'stock', title: 'Stock', items: ['products', 'lowStock'] },
  { id: 'possessions', title: 'Possessions', items: ['assets'] },
  { id: 'organisation', title: 'Organisation', items: ['containers', 'categories'] },
];

function createNavigationSections(activePath: string, navigate: ReturnType<typeof useNavigate>) {
  const navigateInternally = (path: string) => (event: React.MouseEvent<HTMLElement>) => {
    event.preventDefault();
    navigate(path);
  };

  return sections.map((section) => ({
    id: section.id,
    title: section.title,
    items: section.items.map((id) => ({
      id,
      label: pages[id].title,
      href: pages[id].path,
      active: activePath === pages[id].path,
      onClick: navigateInternally(pages[id].path),
    })),
  }));
}

function SummaryCard({ detail, label, value }: { detail: string; label: string; value: string }) {
  return (
    <Card className="inv-summary-card">
      <div className="inv-summary-label">{label}</div>
      <div className="inv-summary-value">{value}</div>
      <div className="inv-summary-detail">{detail}</div>
    </Card>
  );
}

function OverviewPage() {
  const [apiState, setApiState] = useState<'checking' | 'online' | 'offline'>('checking');
  const [counts, setCounts] = useState<InventoryCounts | null>(null);

  useEffect(() => {
    const controller = new AbortController();

    Promise.all([getInventoryCounts(controller.signal), checkInventoryHealth(controller.signal)])
      .then(([loadedCounts]) => {
        setCounts(loadedCounts);
        setApiState('online');
      })
      .catch((error: unknown) => {
        if (error instanceof DOMException && error.name === 'AbortError') {
          return;
        }

        setApiState('offline');
      });

    return () => controller.abort();
  }, []);

  const apiStatusLabel = apiState === 'checking' ? 'Vérification...' : apiState === 'online' ? 'En ligne' : 'Hors ligne';
  const apiStatusTone = apiState === 'online' ? 'success' : apiState === 'offline' ? 'danger' : 'warning';
  const display = (value: number | undefined) => (value === undefined ? '—' : String(value));

  return (
    <div className="inv-page">
      <PageHeader
        actions={<StatusDot label={apiStatusLabel} tone={apiStatusTone} />}
        description={pages.overview.description}
        eyebrow="Inventaire"
        title={pages.overview.title}
      />

      <section className="inv-summary-grid" aria-label="Synthèse de l'inventaire">
        <SummaryCard detail={`${display(counts?.categories)} catégories`} label="Produits" value={display(counts?.products)} />
        <SummaryCard detail={`${display(counts?.expiring)} périment sous 7 jours`} label="Stock bas" value={display(counts?.lowStock)} />
        <SummaryCard detail="Appareils, outils, livres" label="Biens" value={display(counts?.assets)} />
        <SummaryCard detail="Placards, bacs, étagères" label="Rangements" value={display(counts?.containers)} />
      </section>
    </div>
  );
}

function PlaceholderPage({ page }: { page: (typeof pages)[PageId] }) {
  return (
    <div className="inv-page">
      <PageHeader description={page.description} eyebrow="En préparation" title={page.title} variant="page" />
      <Card className="inv-placeholder-card">
        <div className="inv-summary-label">Bientôt disponible</div>
        <p>Cette page est prête pour accueillir ses premiers contrôles.</p>
      </Card>
    </div>
  );
}

function RoutedApp() {
  const location = useLocation();
  const navigate = useNavigate();
  const navigationSections = createNavigationSections(location.pathname, navigate);

  return (
    <DefaultAdminShell
      className="inv-admin-shell"
      contentPadding="compact"
      logoutLabel="Se déconnecter"
      navigationAriaLabel="Navigation principale"
      navigationItems={[]}
      onLogout={() => undefined}
      serverLabel="Serveur courant"
      serverMeta="MaNoir 0.1.0"
      serverName="Espace principal"
      serverStatus="En ligne"
      serverStatusTone="success"
      sidebarBrand="MaNoir"
      sidebarEyebrow="Espace authentifié"
      sidebarNavigation={<SidebarNav aria-label="Navigation principale" className="front-auth-sidebar-nav" sections={navigationSections} />}
      topBarBrand="MaNoir"
      topBarLogo={<img alt="Logo MaNoir" src={manoirLogo} />}
      topBarNavigation={<ShellHeader aria-label="Contexte inventaire" className="front-domain-shell-header" compactBreakpoint={760} items={[{ id: 'inventory', label: 'Inventaire', active: true }]} />}
      userLabel="Administrateur"
      userValue="Michael"
    >
      <Routes>
        <Route element={<Navigate replace to={pages.overview.path} />} path="/" />
        <Route element={<OverviewPage />} path={pages.overview.path} />
        <Route element={<PlaceholderPage page={pages.products} />} path={pages.products.path} />
        <Route element={<PlaceholderPage page={pages.lowStock} />} path={pages.lowStock.path} />
        <Route element={<PlaceholderPage page={pages.assets} />} path={pages.assets.path} />
        <Route element={<PlaceholderPage page={pages.containers} />} path={pages.containers.path} />
        <Route element={<PlaceholderPage page={pages.categories} />} path={pages.categories.path} />
        <Route element={<Navigate replace to={pages.overview.path} />} path="*" />
      </Routes>
    </DefaultAdminShell>
  );
}

function App() {
  return <BrowserRouter basename={getRouterBasePath()}><RoutedApp /></BrowserRouter>;
}

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
);
