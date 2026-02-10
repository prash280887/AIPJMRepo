import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useSelector } from 'react-redux';
import '../Header.css';
import AppLogo from './Logo';

const Header: React.FC<any> = ({ headerProps }) => {
  const [open, setOpen] = useState(false);

  const env = process.env.REACT_APP_ENVIRONMENT;
  const username = headerProps?.username || '[User]';
   const useremail = headerProps?.email || '[email]';
  const initial = username.charAt(0).toUpperCase();
  console.log('env : '+  env);
  const logOut = () => {
    localStorage.removeItem('username');
    localStorage.removeItem('email');
    localStorage.removeItem('jwtToken');
    window.location.href = '/';
  };

  return (
    <header className="app-header">
      {/* Left */}
      <AppLogo size={50} />
     <div className="header-left"> 
        <span className="app-title">
          AI Project Management Report
          {/* <span className="env-badge">{env}</span> */}
        </span> 
      </div>

      {/* Right */}
      <div className="header-right">
        <div className="user-menu">
          <button
            className="avatar-btn"
            onClick={() => setOpen(!open)}
          >
            {initial}
          </button>

          {open && (
            <><div className="dropdown">
              <div className="dropdown-header">
              Hi, <strong>{username}</strong><br />({useremail})
            </div>
              <Link to="/home" className="dropdown-item">Home</Link>
              <Link to="/aboutus" className="dropdown-item">About Us</Link>
                 {/* Logout as link */}
              <Link to="/"  className="dropdown-item logout-link" onClick={logOut}>Sign out</Link>
            </div>
            </>
            )}
        </div>
      </div>
    </header>
  );
};

export default Header;
