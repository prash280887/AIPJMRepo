import { Routes, Route, BrowserRouter} from 'react-router-dom';
import './App.css';
import Login from './components/Login';
import Home from './components/Home';
import { Provider } from 'react-redux'; // {} use this 
import reduxStore from '../src/redux-store/redux-store';
import AboutUs from './components/AboutUs';

function App() {
  return (
    <div className="App">
    {/*   <div>env : {env} && envInternal : {envInternal}</div>
      <Suspense fallback={<div>Loading Microfrontend Card...</div>}>
      
      </Suspense> */}
      {/*add redux store provider tag*/} 
      <Provider store={reduxStore}> 
        <BrowserRouter>
          <Routes>
          <Route path="/" element={<Login />} />
          <Route path="/home" element={<Home />} />
           <Route path="/aboutus" element={<AboutUs />} />
          </Routes>
        </BrowserRouter>
        </Provider>
    </div>
  );
}

export default App;
