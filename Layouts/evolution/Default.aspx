<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="FTC.SharePoint.Evolution.Layouts.AppliedEvos" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<h2><asp:Literal Id="h2" runat="server" Text="<%$Resources:SPEvoContents, Administration_MainTitle%>" /></h2>

<p><asp:Literal Id="Literal1" runat="server" Text="<%$Resources:SPEvoContents, Administration_Info%>" /></p>

<%  if (AppliedToWeb != null)
{ %>
    <table class="ms-listviewtable" style="width:50%">
        <thead>
            <tr class="ms-viewheadertr ms-vhltr">
                <th class="ms-vh2"><div class="ms-vh-div"><asp:Literal Id="th1" runat="server" Text="<%$Resources:SPEvoContents, Administration_VersionTitle%>" /></div></th>
                <th class="ms-vh2"><div class="ms-vh-div"><asp:Literal Id="th2" runat="server" Text="<%$Resources:SPEvoContents, Administration_ActionTitle%>" /></div></th>
            </tr>
        </thead>
        <tbody>
            <% foreach (long l in AppliedToWeb) { %>
            <tr class="ms-itmhover">
                <td class="ms-vb2">#<%=l %></td>
                <td class="ms-vb2"><a href="<%=SiteUrl %>/_layouts/evolution/?to=<%=l%>">Откатиться до этой версии</a></td>
            </tr>
            <% } %>
        </tbody>
    </table>
<% } %>

<asp:Literal Id="Literal2" runat="server" Text="<%$Resources:SPEvoContents, Administration_AssemblyToLoad%>" />
<asp:TextBox ID="assembly" runat="server" />

<asp:Literal Id="Literal3" runat="server" Text="<%$Resources:SPEvoContents, Administration_VersionToEvolution%>" />
<asp:TextBox ID="TextBox1" runat="server" />

<asp:Button ID="MakeEvoBtn" runat="server" Text="<%$Resources:SPEvoContents, Administration_ExecuteEvolutionButtonTitle%>" />

<%=Debug %>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
<asp:Literal Id="pagetitle" runat="server" Text="<%$Resources:SPEvoContents, Administration_MainTitle%>" />
</asp:Content>

