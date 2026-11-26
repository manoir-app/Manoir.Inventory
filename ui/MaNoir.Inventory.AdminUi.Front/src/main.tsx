import { StrictMode, useEffect, useState } from 'react';
import type * as React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Navigate, Route, Routes, useLocation, useNavigate } from 'react-router-dom';
import {
  Card,
  DefaultAdminShell,
  InlineTabs,
  PageHeader,
  SidebarNav,
  StatusPill,
} from '@manoir-app/core-admin-ui-kit';
import { checkInventoryHealth, getInventoryCounts, type InventoryCounts } from './api';
import { getRouterBasePath } from './runtimeConfig';
import { Icon, MoonIcon, SunIcon, type IconKind } from './icons';
import { useShellPreferences } from './shellPreferences';
import '@manoir-app/core-admin-ui-kit/styles.css';
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

// One rail entry per section; the pages of the active section become pill tabs above the content.
const sections: { id: string; title: string; icon: IconKind; pages: PageId[] }[] = [
  { id: 'inventory', title: 'Inventaire', icon: 'grid', pages: ['overview'] },
  { id: 'stock', title: 'Stock', icon: 'package', pages: ['products', 'lowStock'] },
  { id: 'possessions', title: 'Possessions', icon: 'tag', pages: ['assets'] },
  { id: 'organisation', title: 'Organisation', icon: 'layers', pages: ['containers', 'categories'] },
];

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

  const apiStatusLabel = apiState === 'checking' ? 'Vérification…' : apiState === 'online' ? 'En ligne' : 'Hors ligne';
  const apiStatusTone = apiState === 'online' ? 'success' : apiState === 'offline' ? 'danger' : 'neutral';
  const display = (value: number | undefined) => (value === undefined ? '—' : String(value));

  return (
    <div className="inv-page">
      <PageHeader
        actions={<StatusPill tone={apiStatusTone}>{apiStatusLabel}</StatusPill>}
        description={pages.overview.description}
        title="Inventaire"
        variant="page"
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
      <PageHeader description={page.description} title={page.title} variant="page" />
    </div>
  );
}

function RoutedApp() {
  const location = useLocation();
  const navigate = useNavigate();
  const { theme, setTheme, sidebarCollapsed, setSidebarCollapsed } = useShellPreferences();

  const navigateInternally = (path: string) => (event: React.MouseEvent<HTMLElement>) => {
    event.preventDefault();
    navigate(path);
  };

  const activeSection = sections.find((section) => section.pages.some((id) => pages[id].path === location.pathname));
  const sidebarSections = [{
    id: 'sections',
    items: sections.map((section) => {
      const firstPath = pages[section.pages[0]].path;
      return {
        id: section.id,
        label: section.title,
        icon: <Icon kind={section.icon} />,
        active: section.id === activeSection?.id,
        href: firstPath,
        onClick: navigateInternally(firstPath),
      };
    }),
  }];

  return (
    <DefaultAdminShell
      className="inv-admin-shell"
      contentPadding="compact"
      logoutLabel="Se déconnecter"
      navigationAriaLabel="Navigation principale"
      navigationItems={[]}
      onLogout={() => undefined}
      onSidebarCollapsedChange={setSidebarCollapsed}
      serverLabel="Serveur courant"
      serverMeta="MaNoir 0.1.0"
      serverName="Espace principal"
      serverStatus="En ligne"
      serverStatusTone="success"
      sidebarBrand="MaNoir"
      sidebarCollapsed={sidebarCollapsed}
      sidebarEyebrow="Espace authentifié"
      sidebarFooterActions={(
        <button
          aria-label="Changer de thème"
          className="inv-icon-action"
          onClick={() => setTheme((current) => (current === 'dark' ? 'light' : 'dark'))}
          title="Changer de thème"
          type="button"
        >
          {theme === 'dark' ? <SunIcon /> : <MoonIcon />}
        </button>
      )}
      sidebarNavigation={<SidebarNav aria-label="Navigation principale" className="inv-sidebar-nav" collapsed={sidebarCollapsed} sections={sidebarSections} />}
      sidebarToggleLabel="Réduire ou agrandir le menu"
      topBarBrand="MaNoir"
      topBarLogo="M"
      userLabel="Administrateur"
      userValue="Michael"
    >
      {activeSection && activeSection.pages.length > 1 ? (
        <div className="inv-section-tabs">
          <InlineTabs
            aria-label={activeSection.title}
            items={activeSection.pages.map((id) => ({
              id,
              label: pages[id].title,
              href: pages[id].path,
              onSelect: () => navigate(pages[id].path),
            }))}
            onValueChange={() => {}}
            value={activeSection.pages.find((id) => pages[id].path === location.pathname) ?? ''}
            variant="pill"
          />
        </div>
      ) : null}
      <Routes>
        <Route element={<Navigate replace to={pages.overview.path} />} path="/" />
        <Route element={<OverviewPage />} path={pages.overview.path} />
        {(Object.keys(pages) as PageId[]).filter((id) => id !== 'overview').map((id) => (
          <Route element={<PlaceholderPage page={pages[id]} />} key={id} path={pages[id].path} />
        ))}
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
