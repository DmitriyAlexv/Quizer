import './Spinner.css';

interface SpinnerProps {
  size?: 'sm' | 'md' | 'lg';
  label?: string;
}

export function Spinner({ size = 'md', label }: SpinnerProps) {
  return (
    <div className={`spinner spinner--${size}`} role="status" aria-live="polite">
      <span className="spinner__ring" aria-hidden="true" />
      {label && <span className="spinner__label">{label}</span>}
    </div>
  );
}
