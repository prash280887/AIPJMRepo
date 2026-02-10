import React from 'react';
import { Link } from 'react-router-dom';
import '../App.css'; 
import ProfileCard from './ProfileCard';
import AppLogo from './Logo';
import Header from './Header';
import { HeaderProps } from './Home';

const AboutUs : React.FC<any> = () => {

    return (
            <div>   

             <div style={{ marginTop: '20px' }}>
               <Link to="/home" style={{ 
                 color: '#0ea5e9', 
                 textDecoration: 'none', 
                 fontSize: '1.1rem',
                 fontWeight: '500'
               }}>
                 ← Go to Home Page
               </Link>
             </div>
                <div className="login-header">
                    <h1 className="login-title">aipjm</h1>
                      <AppLogo size={80} />
                    <p className="login-subtitle">AI Project Management Reports</p> 
                      <div>This site is developed by Team IntelliTrace using React , .NET 8 Microservices and Azure AI.
                  </div>
             <ProfileCard/>

                </div>          
      </div>
    );

};
export default AboutUs;