<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Main.Master" AutoEventWireup="true" CodeBehind="RegistrationConfirmation.aspx.cs" Inherits="TravelWithMe.RegistrationConfirmation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="Scripts/RegConfirmation.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div id="containerBox">
    
</div>
<div id="templates" style="display: none">
    <textarea id="templateRegConfirm">
        <div class="whiteBox padding2">
            <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Table2">
                <tbody>
                    <tr>
                        <td width="100%" align="left" valign="middle">
                            Congratulations! Your registration is completed successfully. We have sent your secret password on your mail. Please use this password for login.
                        </td>
                    </tr>
                    {#if $T.IsBusOperator}
                    <tr>
                        <td width="100%" align="left" valign="middle">
                            <b>Now you can start adding bus information.</b> But for publishing(Enable online booking) that bus you need to contact us.
                            Please contact us on mtokle@busswitch.com
                        </td>
                    </tr>
                    {#/if}
                </tbody>
            </table>
        </div>
    </textarea>
</div>
</asp:Content>
