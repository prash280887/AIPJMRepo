import { useEffect, useRef } from "react";

type HtmlRendererProps = {
  html: string;
};


const CtrlHtmlRender: React.FC<HtmlRendererProps> = ({ html }) => {
 
   const ref = useRef<HTMLDivElement>(null);
    const htmlBody = html.replace(/```html\s*/gi, "").replace(/```/g, "");
    const fullHtml = htmlBody;
  //   const fullHtml = `
  // <!DOCTYPE html>
  // <html>
  // <head>
  //   <meta charset="UTF-8" />
  //   <title>Markdown Export</title>
  //   <style>
  //     body { font-family: Arial, sans-serif; padding: 24px; }
  //     pre { background: #f5f5f5; padding: 12px; }
  //     code { background: #eee; }
  //   </style>
  // </head>
  // <body>
  //   ${htmlBody}
  // </body>
  // </html>
  // `;
  
  console.log(htmlBody);

useEffect(() => {
  if (!ref.current) return;

  const scripts = Array.from(ref.current.querySelectorAll("script"));

  const loadExternalScripts = () =>
    Promise.all(
      scripts
        .filter(s => s.src)
        .map(s =>
          new Promise(resolve => {
            const script = document.createElement("script");
            script.src = s.src;
            script.async = false;
            script.onload = resolve;
            document.body.appendChild(script);
          })
        )
    );

  const runInlineScripts = () => {
    scripts
      .filter(s => !s.src)
      .forEach(s => {
        try {
          new Function(s.innerHTML)();
        } catch (e) {
          console.error("Inline script failed", e);
        }
      });
  };

  loadExternalScripts().then(runInlineScripts);
}, [htmlBody]);


 
  return (
         <div  ref={ref}  dangerouslySetInnerHTML={{ __html: fullHtml }}/>
     );
};

export default CtrlHtmlRender;
