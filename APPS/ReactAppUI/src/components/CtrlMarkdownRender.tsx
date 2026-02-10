import React from 'react';
import ReactMarkdown from 'react-markdown';



const CtrlMarkdownRender : React.FC<any> = (props) => {

    return(
        <div><ReactMarkdown>{props.projectsMessage}</ReactMarkdown> 
        </div>
    );

};

export default CtrlMarkdownRender;