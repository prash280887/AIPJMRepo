import React from "react";

const AppLogo: React.FC<{ size?: number }> = ({ size = 100 }) => {
  return (
    <svg
      width={size}
      height={size}
      viewBox="0 0 64 64"
      fill="none"
      xmlns="http://www.w3.org/2000/svg"
      aria-label="AI Project Management Report Logo"
    >
      <defs>
        <linearGradient id="grad" x1="0" y1="0" x2="1" y2="1">
          <stop offset="0%" stopColor="#7f00ff" />
          <stop offset="100%" stopColor="#e100ff" />
        </linearGradient>
      </defs>

      {/* Outer circle */}
      <circle cx="32" cy="32" r="30" fill="url(#grad)" />

      {/* Bar chart */}
      <rect x="18" y="34" width="6" height="12" rx="2" fill="#ffffff" />
      <rect x="29" y="28" width="6" height="18" rx="2" fill="#ffffff" />
      <rect x="40" y="22" width="6" height="24" rx="2" fill="#ffffff" />

      {/* AI nodes */}
      <circle cx="21" cy="18" r="2.5" fill="#ffffff" />
      <circle cx="43" cy="14" r="2.5" fill="#ffffff" />
      <circle cx="52" cy="28" r="2.5" fill="#ffffff" />

      {/* AI connections */}
      <line x1="21" y1="20.5" x2="29" y2="28" stroke="#ffffff" strokeWidth="1.5" />
      <line x1="43" y1="16.5" x2="40" y2="22" stroke="#ffffff" strokeWidth="1.5" />
      <line x1="52" y1="30.5" x2="46" y2="30" stroke="#ffffff" strokeWidth="1.5" />
    </svg>
  );
};

export default AppLogo;