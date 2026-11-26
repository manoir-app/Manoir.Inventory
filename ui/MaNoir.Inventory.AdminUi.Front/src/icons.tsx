import type * as React from 'react';

const stroke = {
  fill: 'none',
  stroke: 'currentColor',
  strokeLinecap: 'round' as const,
  strokeLinejoin: 'round' as const,
  strokeWidth: 1.6,
};

const glyphs = {
  grid: <><rect height="5.5" rx="1.6" width="5.5" x="3" y="3" /><rect height="5.5" rx="1.6" width="5.5" x="11.5" y="3" /><rect height="5.5" rx="1.6" width="5.5" x="3" y="11.5" /><rect height="5.5" rx="1.6" width="5.5" x="11.5" y="11.5" /></>,
  package: <><path d="m10 2.5 6.5 3.5v8L10 17.5 3.5 14V6L10 2.5Z" /><path d="m3.5 6 6.5 3.5L16.5 6M10 9.5v8" /></>,
  tag: <><path d="M3 3h6.5l7.5 7.5-6.5 6.5L3 9.5V3Z" /><circle cx="6.5" cy="6.5" r="1.2" /></>,
  layers: <><path d="m10 3 7 3.5-7 3.5-7-3.5L10 3Z" /><path d="m3 10 7 3.5 7-3.5M3 13.5 10 17l7-3.5" /></>,
} satisfies Record<string, React.ReactNode>;

export type IconKind = keyof typeof glyphs;

export function Icon({ kind }: { kind: IconKind }) {
  return (
    <svg aria-hidden="true" height="20" viewBox="0 0 20 20" width="20" {...stroke}>
      {glyphs[kind]}
    </svg>
  );
}

export function SunIcon() {
  return (
    <svg aria-hidden="true" height="20" viewBox="0 0 20 20" width="20" {...stroke}>
      <circle cx="10" cy="10" r="3.2" />
      <path d="M10 2.5v1.8M10 15.7v1.8M2.5 10h1.8M15.7 10h1.8M4.7 4.7l1.3 1.3M14 14l1.3 1.3M4.7 15.3 6 14M14 6l1.3-1.3" />
    </svg>
  );
}

export function MoonIcon() {
  return (
    <svg aria-hidden="true" height="20" viewBox="0 0 20 20" width="20" {...stroke}>
      <path d="M16.5 11.6A6.8 6.8 0 0 1 8.4 3.5a6.8 6.8 0 1 0 8.1 8.1Z" />
    </svg>
  );
}
