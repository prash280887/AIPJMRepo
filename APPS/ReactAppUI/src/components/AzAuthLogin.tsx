import { useMsal } from "@azure/msal-react";
import { useNavigate } from "react-router-dom";

function AzAuthLogin() {
    const { instance } = useMsal();
     const navigate = useNavigate();
    // Azure Pop-up Login
    const handleLogin = () => {
        
             instance.ssoSilent(
                {
                    scopes : ["User.Read"]
                }
             ).then((response) => {
                   console.log("SSO Login successful : ", response);
                    localStorage.setItem("jwtToken", response.accessToken);
                    localStorage.setItem("username", response.account?.name || ""); //full name
                    localStorage.setItem("email", response.account?.username || ""); //email

                    console.log("Acess token : ", response.accessToken);
                    console.log("username: ", response.account?.name);
                    navigate('home');
             })
        .catch((err) => {        
        instance.loginPopup().then((response) => {
            console.log("SSO Login successful : ", response);
                    localStorage.setItem("jwtToken", response.accessToken);
                    localStorage.setItem("username", response.account?.name || "");
                    localStorage.setItem("email", response.account?.username || "");

                    console.log("Acess token : ", response.accessToken);
                    console.log("username: ", response.account?.name);
                    navigate('home');
        }).catch((error) => {
            console.error("Login failed:", error);
        });
    });    
}
    return (  
        <div> 
        <button onClick={handleLogin}>Azure AD Login</button>
        <p style={{ fontSize: '0.95rem', color: '#4a4b4dff', marginTop: '4px' }}>(login via Winwire A/c)</p>
        </div>     
    );

}

export default AzAuthLogin;