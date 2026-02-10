import React, { useEffect }  from 'react';
import Header from './Header';
import ProjectDashboard from './ProjectDashboard';


export interface HeaderProps {
    username: string;
    email: string;
 }

const Home : React.FC = () => {
    
    const userlocalName = localStorage.getItem("username") ;
    const useremail =  localStorage.getItem("email");

    console.log("User from Local Storage in Home:", userlocalName);
let user : any = userlocalName;
let email : any = useremail;
    const headerInfo: HeaderProps = {
        username: user,
        email: email
    };

    useEffect(() => {
        
        if(!user){
            console.log("No user found, redirecting to login.");
            window.location.href = '/';
        }

    }, []);
 
    return(
        <div className="container"> 
        <Header headerProps={headerInfo}/>
        <br />
       <ProjectDashboard />
        </div>
    );

};

export default Home;