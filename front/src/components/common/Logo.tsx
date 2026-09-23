import './Logo.css';

interface LogoProps {
  size?: 'sm' | 'md' | 'lg';
}

export function Logo({ size = 'md' }: LogoProps) {
  return (
    <div className={`logo logo--${size}`}>
      <span className="logo__mark" aria-hidden="true">
        <svg width="100%" height="100%" viewBox="0 0 48 48" fill="none">
          <rect x="4" y="4" width="40" height="40" rx="12" fill="url(#logo-grad)" />
          <path
            d="M16 18h16M16 24h16M16 30h10"
            stroke="#fff"
            strokeWidth="3.2"
            strokeLinecap="round"
          />
          <defs>
            <linearGradient id="logo-grad" x1="4" y1="4" x2="44" y2="44">
              <stop stopColor="#7c3aed" />
              <stop offset="1" stopColor="#4f46e5" />
            </linearGradient>
          </defs>
        </svg>
      </span>
      <span className="logo__text">Quizer</span>
    </div>
  );
}
