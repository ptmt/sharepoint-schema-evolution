using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using FTC.SharePoint.Evolution.Core;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace FTC.SharePoint.Evolution.Layouts
{
    /// <summary>
    /// TODO: add multilanguage support (resx files)
    /// </summary>
    public partial class AppliedEvos : LayoutsPageBase
    {
        public string Debug { get; set; }
        public string SiteUrl { get { return SPContext.Current.Web.Url; } }
        public List<long> AppliedToWeb {get;set;}
        //public Button but;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (SPContext.Current.Web.AllProperties[SPEvolutionProviderConstants.LAST_ASSEMBLY_PROPERTY] != null)
            {
                assembly.Text = SPContext.Current.Web.AllProperties[SPEvolutionProviderConstants.LAST_ASSEMBLY_PROPERTY].ToString();
            }
            if (Request.QueryString["to"] != null && !String.IsNullOrEmpty(assembly.Text))
            {
                try 
                {
                MakeEvolution(assembly.Text,Request.QueryString["to"].ToString(), SPContext.Current.Web);
                }
                catch (Exception ee) 
                {
                    Debug = "Evolution may be applied or may be not";
                }
                
            }
           
            if (SPContext.Current.Web.AllProperties[SPEvolutionProviderConstants.PROPERTY_NAME] != null)
            {                
                AppliedToWeb = SPEvolutionProvider.DeserializeStringToLongList(SPContext.Current.Web.AllProperties[SPEvolutionProviderConstants.PROPERTY_NAME].ToString());
                //but.Click +=new EventHandler(but_Click);
            }
            else
            {
                Debug = "Empty";
            }
        }

        public void  but_Click(object sender, EventArgs e)
        {
 	        throw new NotImplementedException();
        }

        public void MakeEvolution(string assembly, string version, SPWeb web)
        {
          //  if (web.AllProperties[SPEvolutionProviderConstants.LAST_ASSEMBLY_PROPERTY] != null) {
                SPEvolutionManager evolMan = SPEvolutionFactory.GetManagerForWeb(assembly, web);
                evolMan.EnableSharePointLogs();
             //   if (isDebug)
                    evolMan.EnableDebug();
                if (version == "prev")
                {
                    evolMan.MakeEvolutionStepRollback();
                }
                else
                {
                    if (version != "-1")
                        evolMan.MakeEvolution(Convert.ToInt16(version));
                    else
                        evolMan.MakeEvolution();
                }
            
        }
    }
}
