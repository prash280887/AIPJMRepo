import { useState } from "react"; 
import { useNavigate } from "react-router-dom";
import contextUser from './contextUser';
import AzAuthLogin from "./AzAuthLogin";
import axios from "axios";
import AppLogo from "./Logo";

const Login : React.FC = () => {

     const navigate = useNavigate();
    let [username, setUsername] = useState<string>('');
    let [password, setPassword] = useState<string>(''); 

 async function submitForm(event: React.FormEvent) {
        event.preventDefault();
        //CLEAR ANY EXISITNG sTOARGE
        localStorage.removeItem("username");
        localStorage.removeItem("authToken");
        localStorage.removeItem("email");
          try {
    const requestBody = {
      username: username,
      password: password,
    };

 const response  = await axios.post(process.env.REACT_APP_SERVICE_API_URL || '<nourl>', requestBody, {
      headers: {
        "Content-Type": "application/json",
      },
      // Include credentials if your API requires cookies/auth
      // withCredentials: true,
    });
   console.log(response.data);
  
   console.log(response)
    const data = response?.data;

    if (data.isValid) {
      console.log("Login successful!", data.user);
      console.log("JWT token:", data.token);
  
      // Store token in localStorage or state and redirect to home
       localStorage.setItem("jwtToken", data.token);
       localStorage.setItem("username", data.user.given_name + ' ' + data.user.family_name);
       localStorage.setItem("email", data.user.unique_name);
       navigate('home');
    } else {
         alert("Login Failed");
      console.error("Login failed:", data.message);
      
    }

    return data;
  } catch (error) {
    console.error("Error calling ValidateUser API:", error);
    throw error;
  }
        // if(username === "admin" && password  === "password") {
        //     alert("Login Successful");
        //     //1. using Props in Navigate to pass data to another component
        //      //  navigate('home', {state: {user: username}});
        //      navigate('home');

        //     //set User in Local Storage
        //     localStorage.setItem("username", username);

        //     //useContext can also be used to pass data to another component
        //     //setUsername(username);

        // } else {
        //     alert("Login Failed");
        // };
    }

    return (
        <contextUser.Provider value={username}>
            <div className="login-page-wrapper">
                <div className="login-header">
                    <h1 className="login-title">aipjm</h1>
                    <p className="login-subtitle">AI Project Management Reports</p>          
                  </div>
                <div className="login-container">
                    <div><AppLogo size={110} /></div>                    
                    <form className="login-form" onSubmit={submitForm}>
                        <div className="form-group">
                            <input 
                                type="text" 
                                placeholder="Username" 
                                value={username} 
                                onChange={e => setUsername(e.target.value)}
                                required
                            />
                        </div>
                        <div className="form-group">
                            <input 
                                type="password" 
                                placeholder="Password" 
                                value={password} 
                                onChange={e => setPassword(e.target.value)}
                                required
                            />
                        </div>
                        <button type="submit" className="login-button">Sign In</button>
                    </form>

                    <div className="login-divider">
                        <span className="login-divider-text">OR</span>
                    </div>

                    <div className="login-social"><center><AzAuthLogin /></center></div>
                </div>
                <p style={{ fontSize: '0.95rem', color: '#333436ff', marginTop: '4px' }}>Generates intelligent project insights powered by AI</p>
             
                <div className="login-footer">
                    <p>Demo Credentials: admin@aipjm.com / password</p>
                </div>
            </div>
        </contextUser.Provider>
    );

        };


export default Login;