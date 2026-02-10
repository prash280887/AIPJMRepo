import React, { Suspense, useEffect, useState }  from 'react';
import axios from 'axios';
import '../App.css';  
import CtrlLoadingCard from './CtrlLoading';
import CtrlMarkdownRender from './CtrlMarkdownRender';
import CtrlHtmlRender from './CtrlHtmlRender';

import { marked } from "marked";

export interface HeaderProps {
    username: string;
 }

 export interface ProjectReporterProps {
    projectKeyId: string;
 }

export interface ProjectsInfo {
    orgUrl: string;
    personalAccessToken: string;
    projectName: string;
    projectDisplayName: string;
    projectBoardType: string;
    projectKeyId: string;
}

export interface AzurePromptRequest {
  azureBoardsConfig: AzureBoardConfig;
  userPrompt?: string;
  requestType?: string;
  maxTokens?: number;
}

export interface AzureBoardConfig {
    orgUrl?: string;
    personalAccessToken?: string;
    projectName?: string;
    projectDisplayName?: string;
    projectBoardType?: string;
    projectKeyId?: string;
}

export interface MailRequest {
  promptRequest ?: AzurePromptRequest;
  type: string; // e.g., "EMAIL", "SMS"
  content?: string;
  htmlContent?: string;
  subject: string;
  to?: string[];
  cc?: string[];
  bcc?: string[];
  isHtml?: boolean;
  attachments?: MailAttachment[];
}

export interface MailAttachment {
  contentsAsAttachment: string;
  fileName: string;
  mimeType: string;
  fileExtension: string;
}


const ProjectSelector : React.FC = () => {
       
      const [projectsInfo, setProjectsInfo] = React.useState<ProjectsInfo[] | null>(null);
      const [isLoading, setisLoading] = React.useState<boolean>(false);
      const [isCollapsed, setIsCollapsed] = useState(false);
      const [userPrompt, setUserPrompt] = React.useState<string>('');
      const [azurePromptRequest, setAzurePromptRequest] = React.useState<AzurePromptRequest>({
        azureBoardsConfig: {
          orgUrl: '', personalAccessToken: '',  projectDisplayName: '' ,  projectKeyId: '', projectName: '', projectBoardType: '' 
        }          
          , userPrompt: '',requestType : '',  maxTokens: 2000 });
    
    const refTxtProjectName = React.useRef<HTMLInputElement>(null);
    const [projectTitle, setProjectTitle] = React.useState<string>('');
    const [projectsMessage, setprojectsMessage] = React.useState<string>('');
    const [projectsMessageHtml, setprojectsMessageHtml] = React.useState<string>('');
    const [projectsMessageHtmlTasks, setprojectsMessageHtmlTasks] = React.useState<string>('');
    const [projectsMessageHtmlRisks, setprojectsMessageHtmlRisks] = React.useState<string>('');
    const [projectsMessageHtmlRecommendations, setprojectsMessageHtmlRecommendations] = React.useState<string>('');
  
    const [activeTab, setActiveTab] = useState<'reporter' | 'html' | 'htmlTasks' | 'htmlRisks' | 'htmlRecommendations'>('reporter');
    const [isProjectConfigPanelActive,setProjectConfigPanel ] = useState<boolean>(true);
    const loadProjectsData = async () => {       
        try {
        const response : any = await axios.get<any>(process.env.REACT_APP_AI_PROJECTS_INFO_URL || '<nourl>');   
        const projectdata : ProjectsInfo[] = response.data;
        setProjectsInfo(projectdata);
        }
        catch (error) {
            console.error("Error fetching data:", error);
        }   
        finally {
            console.log("Data fetch attempt completed.");
        }   
    };

useEffect(() => {
    loadProjectsData();
},[]);

    // Shared event handler
    const handleProjectChange = (value : any) => {       
        if(!value || value.trim() === ""){
          setProjectConfigPanel(true);
        }
        else{
          setProjectConfigPanel(false);
        }
       setAzurePromptRequest({ ...azurePromptRequest , azureBoardsConfig: { ...azurePromptRequest.azureBoardsConfig, projectKeyId: value , projectDisplayName : projectsInfo?.find(p => p.projectKeyId === value)?.projectDisplayName } });
       setprojectsMessage('');
       setprojectsMessageHtml('');  
       setprojectsMessageHtmlTasks('');
       setprojectsMessageHtmlRisks('');  
       setprojectsMessageHtmlRecommendations('');   
       console.log("Project changed to:", value);
    };



   const generateReport = (isDropdown : boolean) => {
     let title : string = "";
    if(isDropdown)
      title = projectsInfo?.find(p => p.projectKeyId === azurePromptRequest.azureBoardsConfig.projectKeyId)?.projectDisplayName || "";
    else
      title = refTxtProjectName.current?.value || "";
    
    console.log("Generating report for project:", title);
    setProjectTitle(title);
    setprojectsMessage('Generating report....');
    setprojectsMessageHtml('Generating report....'); 
    setprojectsMessageHtmlRisks('Generating report....');
    setprojectsMessageHtmlRecommendations('Generating report...');

    getProjectsReportData();
    getProjectsReportDataHtml();
    getProjectsReportDataHtmlTasks();
    getProjectsReportDataHtmlRisks();
    getProjectsReportDataHtmlRecommendations();
     }

// API CALLS - to be clubbed under report conditions later
      const getProjectsReportData = async () => {        
        try {
            setisLoading(true);               
           // Eg. const url = `https://localhost:61003/api/Agent/GetAzOpenAIAzureBoardResponse?projectKeyId=`+ projectKeyId;
          // console.log("fetching.."+ url); 
          const request  : AzurePromptRequest = {
            azureBoardsConfig :
              { ...azurePromptRequest.azureBoardsConfig }
            ,userPrompt: userPrompt, requestType : "SUMMARY" , 
            maxTokens: 2000
          }

          const response = await axios.post<string>(
                process.env.REACT_APP_AI_REPORT_URL || '<nourl>',
                request
            );      
        console.log("getProjectsReportData response:",response.data); 
        setprojectsMessage(response.data);      
        
        }
        catch (err) {
            console.error("Error fetching data:", err);
        }   
        finally {
             setisLoading(false);
            console.log("Data fetch attempt completed.");
        }   
    };


    

    const getProjectsReportDataHtml = async () => {
      try {

        setisLoading(true);
         const request  : AzurePromptRequest = {
          azureBoardsConfig :
             { ...azurePromptRequest.azureBoardsConfig }
          ,userPrompt: userPrompt, requestType : "HTML_STATS" , 
          maxTokens: 2000
        }       
        const response = await axios.post<string>(
          process.env.REACT_APP_AI_REPORT_URL || '<nourl>',
          request
        );
        console.log("getProjectsReportDataHtml Response:", response.data);
        setprojectsMessageHtml(response.data);
        return response.data;
      }
      catch (error) {
        console.error("Error posting Azure Board config:", error);
      }
      finally {
        setisLoading(false);
      }
    };

        const getProjectsReportDataHtmlTasks = async () => {
      try {

        setisLoading(true);
         const request  : AzurePromptRequest = {
          azureBoardsConfig :
             { ...azurePromptRequest.azureBoardsConfig }
          ,userPrompt: userPrompt, requestType : "DETAILED_TASK_ANALYSIS" , 
          maxTokens: 2000
        }       
        const response = await axios.post<string>(
          process.env.REACT_APP_AI_REPORT_URL || '<nourl>',
          request
        );
        console.log("getProjectsReportDataHtmlTasks Response:", response.data);
        setprojectsMessageHtmlTasks(response.data);
        return response.data;
      }
      catch (error) {
        console.error("Error posting Azure Board config:", error);
      }
      finally {
        setisLoading(false);
      }
    };

      const getProjectsReportDataHtmlRisks = async () => {
      try {

        setisLoading(true);
         const request  : AzurePromptRequest = {
          azureBoardsConfig :
             { ...azurePromptRequest.azureBoardsConfig }
          ,userPrompt: userPrompt, requestType : "RISKS_AND_BLOCKERS" , 
          maxTokens: 2000
        }       
        const response = await axios.post<string>(
          process.env.REACT_APP_AI_REPORT_URL || '<nourl>',
          request
        );
        console.log("getProjectsReportDataHtmlRisks Response:", response.data);
        setprojectsMessageHtmlRisks(response.data);
        return response.data;
      }
      catch (error) {
        console.error("Error posting Azure Board config:", error);
      }
      finally {
        setisLoading(false);
      }
    };

          const getProjectsReportDataHtmlRecommendations = async () => {
      try {

        setisLoading(true);
         const request  : AzurePromptRequest = {
          azureBoardsConfig :
             { ...azurePromptRequest.azureBoardsConfig }
          ,userPrompt: userPrompt, requestType : "RECOMMENDATIONS" , 
          maxTokens: 2000
        }       
        const response = await axios.post<string>(
          process.env.REACT_APP_AI_REPORT_URL || '<nourl>',
          request
        );
        console.log("getProjectsReportDataHtmlRecommendations Response:", response.data);
        setprojectsMessageHtmlRecommendations(response.data);
        return response.data;
      }
      catch (error) {
        console.error("Error posting Azure Board config:", error);
      }
      finally {
        setisLoading(false);
      }
    };

    const sendReport = async () => {
  try {
    setisLoading(true);
    let userEmail : string = localStorage.getItem('email') || '';
    const fullHtml : any = PrepareReportHtmlData();
    const request: MailRequest = {
      promptRequest: {
        azureBoardsConfig: {
          ...azurePromptRequest.azureBoardsConfig   
      
    },
      userPrompt: userPrompt,
      requestType: "SUMMARY", 
      maxTokens: 2000
    },
    subject: `aipjm - Automated Project Report (${projectTitle})`,
    to: [userEmail], 
    cc: [], // In future can pass CC email addresses and integrate at backend,
    bcc: [], // In future can pass BCC email addresses and integrate at backend,
    type: "EMAIL",
    content: `Please find the attached AI generated project management report - ${projectTitle}`,
    htmlContent: `<p>Please find the attached AI generated project management report for project - ${projectTitle}.<br/>Regards<br />WinBuild1 IntelliTrace Team</p>`,
    isHtml: true,
    attachments: [
      {
        contentsAsAttachment: fullHtml,
        fileName: `${projectTitle || 'ProjectReport'}.html`,
        mimeType: 'text/html',
        fileExtension: 'html'
      }
    ]
  };
console.log(request);

    const response = await axios.post(
      process.env.REACT_APP_SEND_REPORT_URL || '<nourl>',
      request,
      {
        headers: {
          "Content-Type": "application/json",
          "x-functions-key": process.env.REACT_APP_SEND_REPORT_APIKEY // Required if not using Anonymous
        }
      }
     );

    console.log("Sent:", response.data);
  } catch (error) {
      setisLoading(false);
    console.error("Error sending report:", error);
  } finally {
      setisLoading(false);
    console.log("Data fetch attempt completed.");
  }
}
    
// const sendReport = async () => {
//   try {
//     const request: AzurePromptRequest = {
//       azureBoardsConfig: {
//         ...azurePromptRequest.azureBoardsConfig
//       },
//       userPrompt: userPrompt,
//       requestType: "SUMMARY",
//       maxTokens: 2000
//     };

//     const response = await axios.post(
//       process.env.REACT_APP_SEND_REPORT_URL || '<nourl>',
//       request,
//       {
//         headers: {
//           "Content-Type": "application/json"
//           // "x-functions-key": "<YOUR_FUNCTION_KEY>" // Required if not using Anonymous
//         }
//       }
//     );

//     console.log("Sent:", response.data);
//   } catch (error) {
//     console.error("Error sending report:", error);
//   } finally {
//     console.log("Data fetch attempt completed.");
//   }
// }; 
const sanitizeHtml = (html: string) => {
  return html.replace(/```html\s*/gi, "").replace(/```/g, "");
};

//this meethod id redundant , same in reporting service backend for schedular
//expose : backend method as API and call here for html format consistency
const PrepareReportHtmlData = () => {
  const bodyMessageMarkdown = marked.parse(projectsMessage);

  const bodyMessageHtml = sanitizeHtml(projectsMessageHtml);
  const bodyMessageHtmlTasks = sanitizeHtml(projectsMessageHtmlTasks);
  const bodyMessageHtmlRisks = sanitizeHtml(projectsMessageHtmlRisks);
  const bodyMessageHtmlRecommendations = sanitizeHtml(projectsMessageHtmlRecommendations);
  const fullHtml = `
<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8" />
  <title>ReportExport</title>
  <style>
    body { font-family: Arial, sans-serif; padding: 24px; }
    pre { background: #f5f5f5; padding: 12px; }
    code { background: #eee; }
  </style>
</head>
<body>
  <h2>
  ${projectTitle}
  </h2>
  ${bodyMessageMarkdown}
  <hr>
  ${bodyMessageHtml}
    <hr>
  ${bodyMessageHtmlTasks}
    <hr>
  ${bodyMessageHtmlRisks}
    <hr>
  ${bodyMessageHtmlRecommendations}
</body>
</html>
`;

return fullHtml;
}

const downloadReport = () => {
  const fullHtml : any = PrepareReportHtmlData();
  const blob = new Blob([fullHtml], { type: "text/html" });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = "report.html";
  a.click();
  URL.revokeObjectURL(url);
};


  return(
  <div className={`layout ${isCollapsed ? "collapsed" : ""}`}>
    
    {/* LEFT PANEL */}
    <div className="left-panel">
      <button className="toggle-btn" onClick={() => setIsCollapsed(!isCollapsed)}>
        {isCollapsed ? "▶" : "◀"}
      </button>
      {!isCollapsed && (
        <div className="left-content">
           <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <h4 style={{ margin: 0 }}>Select Azure Project:</h4><span title="These projects are loaded from backend config settings i.e Account:ProjectName" style={{ cursor: 'pointer', fontSize: '18px' }}>ℹ️</span>
            </div>  
          <span></span>        
          <select  value={azurePromptRequest.azureBoardsConfig.projectKeyId}  onChange={(e) => handleProjectChange(e.target.value)} >
            <option value="" style={{ color: 'blue', fontWeight: 'bold' }}>--Connect Your Project--</option>
            {projectsInfo?.map((project, index) => (
              <option key={index} value={project.projectKeyId}>
                {project.projectDisplayName}
              </option>
            ))}
             </select>
            <button onClick={() => generateReport(true)} style={{ display: !azurePromptRequest.azureBoardsConfig.projectKeyId || azurePromptRequest.azureBoardsConfig.projectKeyId.trim() === "" ? 'none' : 'block' }}>Generate Report</button>
          <br/><br/>
          <div style={{ marginBottom: "20px" , display: isProjectConfigPanelActive ? "block" : "none" } }>
            <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <h4 style={{ margin: 0 }}>Connect Azure Project:</h4><span title={
              "Goto your azure devops project and pick your project url. Eg. If project url is https://dev.azure.com/PrashantAkhouri/DevOpsLibrary\n" +
              "Enter Org Url as https://dev.azure.com/PrashantAkhouri\n" +
              "Project Name as DevOpsLibrary\n" +
              "Generate Personal Access Token in ADO under your profile picture (top-right) with full read access."
            } style={{ cursor: 'pointer', fontSize: '18px' }}>ℹ️</span>
            </div>
            <input  type="text"  placeholder="Organization URL"  onChange={(e) => setAzurePromptRequest({ ...azurePromptRequest, azureBoardsConfig: { ...azurePromptRequest.azureBoardsConfig, orgUrl: e.target.value } })}  />
            <input type="password"   placeholder="Personal Access Token"  onChange={(e) => setAzurePromptRequest({ ...azurePromptRequest, azureBoardsConfig: { ...azurePromptRequest.azureBoardsConfig, personalAccessToken: e.target.value } })} />
            <input type="text" ref={refTxtProjectName} placeholder="Project Name" onChange={(e) => setAzurePromptRequest({ ...azurePromptRequest, azureBoardsConfig: { ...azurePromptRequest.azureBoardsConfig, projectName: e.target.value, projectDisplayName: e.target.value  } })} /> 
             <br />            
            <button onClick={() => generateReport(false)}  disabled={(!azurePromptRequest.azureBoardsConfig.orgUrl?.trim() || !azurePromptRequest.azureBoardsConfig.personalAccessToken?.trim() || !azurePromptRequest.azureBoardsConfig.projectName?.trim())}>Generate Report</button>
            </div>
             <br/><textarea  placeholder="Type additional ask (Optional)" onChange={(e) => setUserPrompt(e.target.value)} /> 
            <br />
            <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
            <h4 style={{ margin: 0 }}>Report Actions:</h4><span title="Download or email the generated report" style={{ cursor: 'pointer', fontSize: '18px' }}>ℹ️</span>
            </div>
          <br></br>
          <button title="Email the generated report as attachment to logged in and already registered email id(s)" disabled={!projectsMessage || projectsMessage.trim() === ""}  onClick={sendReport} >Send Report</button>
         <br /><br />
          <button title="Download the generated report as single html file" disabled={!projectsMessage || projectsMessage.trim() === ""}  onClick={downloadReport}>Download</button>
        </div>
      )}
    </div>

    {/* RIGHT PANEL */}
    <div className="right-panel">    
        <div className="project-report-header">{projectTitle}</div>         
        <br/>
        <div className="tabs-container">          
             <div className="tabs-content">
            <button  className={`tab ${activeTab === 'reporter' ? 'active' : ''}`} onClick={() => setActiveTab('reporter')}>Report Summary</button>
            &nbsp;&nbsp; <button  className={`tab ${activeTab === 'html' ? 'active' : ''}`} onClick={() => setActiveTab('html')}>Detailed Statistics</button>
            &nbsp;&nbsp; <button  className={`tab ${activeTab === 'htmlTasks' ? 'active' : ''}`} onClick={() => setActiveTab('htmlTasks')}>Detailed Tasks Analysis</button>
            &nbsp;&nbsp; <button  className={`tab ${activeTab === 'htmlRisks' ? 'active' : ''}`} onClick={() => setActiveTab('htmlRisks')}>Detailed Risks</button>
            &nbsp;&nbsp; <button  className={`tab ${activeTab === 'htmlRecommendations' ? 'active' : ''}`} onClick={() => setActiveTab('htmlRecommendations')}>Detailed Recommendations</button>
        
           <br/><br/> 
            {activeTab === 'reporter' && projectsMessage && (
              <Suspense fallback={<div>Generating Report. Please wait..!</div>}>
                <CtrlMarkdownRender projectsMessage={projectsMessage} />
              </Suspense>
            )}
            {activeTab === 'html' && projectsMessageHtml && (
              <Suspense fallback={<div>Generating Report. Please wait..!</div>}>
                <CtrlHtmlRender html={projectsMessageHtml} />
                </Suspense>
            )}
            {activeTab === 'htmlTasks' && projectsMessageHtmlTasks && (
                <Suspense fallback={<div>Generating Report. Please wait..!</div>}>
                <CtrlHtmlRender html={projectsMessageHtmlTasks} /> )
                </Suspense>
            )}
            {activeTab === 'htmlRisks' && projectsMessageHtmlRisks && (
                <Suspense fallback={<div>Generating Report. Please wait..!</div>}>
                <CtrlHtmlRender html={projectsMessageHtmlRisks} />
                </Suspense>
            )}
            {activeTab === 'htmlRecommendations' && projectsMessageHtmlRecommendations && (
               <Suspense fallback={<div>Generating Report. Please wait..!</div>}>
                <CtrlHtmlRender html={projectsMessageHtmlRecommendations} />
                </Suspense>
            )}
          </div>
        </div>
          <div>
                              
        </div>
    </div>
    <center>
        <CtrlLoadingCard isLoading={isLoading} spinMessage="Generating Report. Please wait..!" />
    </center>  
 </div>
    );
};

export default ProjectSelector;