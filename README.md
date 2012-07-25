sharepoint-schema-evolution
===========================

Migrations for Microsoft SharePoint 2010 data: Content Types, List, Columns, ListItems etc in "Ruby on Rails"-way. (AcitveRecord::Migration)

Write small migrations steps which help to deploy and synchronize data schema beetween production, test and development environment.

Designed for SharePoint 2010, written in C#.

`AddListWithNameEvolutionToWeb.cs:`

      using  FTC.SharePoint.Evolution.Core;
  
      [SPEvolutionAttribute(1)]
      public class AddListWithNameEvolutionToWeb : SPEvolution
      {
          const string LIST_NAME = "EvoList";
          public override void Up()
          {
               this.EvoContext.AddGenericList(LIST_NAME, "list description");
          }
          public override void Down()
          {
               this.EvoContext.DeleteList(LIST_NAME)
          }
      }
      
`AddFieldToList.cs:`

    [SPEvolutionAttribute(2)]
    public class AddFieldToList : SPEvolution
    {
        const string LIST_NAME = "EvoList";
        public override void Up()
        {

            if (this.EvoContext.Web.Lists.TryGetList( LIST_NAME) != null)
            {
                this.EvoContext.Web.Lists.TryGetList( LIST_NAME).Fields.Add("newfield", SPFieldType.Integer, false);
            }
        }
        public override void Down()
        {
            SPList list = this.EvoContext.Web.Lists.TryGetList(LIST_NAME);
            if (list != null)
            {
                list.Fields.Delete("newfield");
            }
        }
    }
    
`Program.cs:`
    
    class Program
    {
        static void Main(string[] args)
        {
            using (SPSite site = new SPSite("http://sharepoint/"))
            using (SPWeb web = site.RootWeb) 
            {
                 SPEvolutionManager man = SPEvolutionFactory.GetManagerForWeb(Assembly.GetExecutingAssembly(), web);
                 man.EnableSharePointLogs();
                 man.MakeEvolution(); // apply 2 evolutions step; if you want downgrade scheme use man.MakeEvolution(0) for example.                
            }
        }
    }
    
Also you can use SharePoint.EvolutionCli.exe or _layouts/ web helper for integrate in your deployment process.