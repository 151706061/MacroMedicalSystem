<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SIQEntryTooltip.ascx.cs" Inherits="Macro.ImageServer.Web.Application.Pages.Queues.StudyIntegrityQueue.SIQEntryTooltip" %>

<style type="text/css">
    .WrapAtRightEdge
    {
    	overflow-y:auto;
    	overflow-x:auto; 
    	word-break:break-all;
    }
    
</style>

<asp:Panel runat="server" ID="Container" >
    <table>
        <tr>
            <td colspan="2">
                <asp:HyperLink runat="server" ID="StudyLink" Target="_blank"/><span> </span>
                <asp:Label runat="server" ID="Note" ForeColor="Red" ></asp:Label>
            </td>
        </tr>
        <tr>
            <td><%= Labels.SIQ_ReconcileDialog_StudyLocation %></td>
            <td><asp:Label runat="server" ID="FilesystemPath" CssClass="WrapAtRightEdge"/></td>
        </tr>

        <tr>
            <td><%= Labels.SIQ_ReconcileDialog_ConflictingImageLocation %></td>
            <td><asp:Label runat="server" ID="ReconcilePath"  CssClass="WrapAtRightEdge"/></td>
        </tr>
    </table>
</asp:Panel>