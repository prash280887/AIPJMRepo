import React  from 'react';
import ProjectSelector from './ProjectSelector';

const ProjectDashboard : React.FC = () => {
         return(
        <div style={{marginTop: '50px' , marginLeft: '5px',marginRight: '5px', width: '100%'}}>        
        <ProjectSelector></ProjectSelector>
        </div>
   
    );

};

export default ProjectDashboard;