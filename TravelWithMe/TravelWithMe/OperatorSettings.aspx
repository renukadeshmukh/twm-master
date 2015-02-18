<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Main.Master" AutoEventWireup="true"
    CodeBehind="OperatorSettings.aspx.cs" Inherits="TravelWithMe.OperatorSettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="Scripts/Admin/OperatorSettings.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="whiteBox padding2">
        <div class="whiteBoxHeader">
            Bus operator settings
        </div>
    </div>
    <div class="whiteBox padding2">
        <div id="settings">
        </div>
    </div>
    <div id="templateContainer" style="display: none">
        <textarea id="settingTemplate">
            <form id="frmOpSettings">
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="itinerary">
                    <tbody>
                        <tr>
                            <th width="20%" align="left" valign="middle" colspan="2">
                                Company Information
                            </th>
                        </tr
                        <tr>
                            <td width="20%" align="left" valign="middle">
                                Company Name
                            </td>
                            <td width="80%" align="left" valign="middle">
                                <input name="txtCompany" type="text" class="width200 mrs" id="txtCompany" value="{$T.CompanyName}" />
                            </td>
                        </tr>
                    </tbody>
                </table>
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="itinerary">
                    <tbody>
                        {#foreach $T.Email as EM}
                        <tr>
                            <td width="20%" align="left" valign="middle">
                                Email
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <input name="txtOpEmail" type="text" class="width200 mrs" id="txtOpEmail" value="{$T.EM}" />
                            </td>
                            <td width="20%" align="left" valign="middle">
                                <a href=""  style="display:none">Add Email</a>
                            </td>
                            <td width="30%" align="left" valign="middle">
                            </td>
                        </tr>
                        {#/for}
                    </tbody>
                </table>
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="itinerary">
                    <tbody>
                        {#foreach $T.PhoneNumber as PN}
                        <tr>
                            <td width="20%" align="left" valign="middle">
                                Phone Number
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <input name="txtOpPhoneNumber" type="text" class="width200 mrs" id="txtOpPhoneNumber" value="{$T.PN}" />
                            </td>
                            <td width="20%" align="left" valign="middle">
                                <a href=""  style="display:none">Add phone number</a>
                            </td>
                            <td width="30%" align="left" valign="middle">
                            </td>
                        </tr>
                        {#/for}
                    </tbody>
                </table>
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="itinerary">
                    <tbody>
                        <tr>
                            <th width="50%" align="left" valign="middle">
                                Address Information
                            </th>
                            <th width="50%" align="left" valign="middle">
                                <a href="" style="display:none">Add Address</a>
                            </th>
                        </tr>
                    </tbody>
                </table>
                {#foreach $T.Addresses as ADD}
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="itinerary">
                    <tbody>
                        <tr>
                            <td width="20%" align="left" valign="middle">
                                Address Line1
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <input name="txtAddressLine1" type="text" class="width200 mrs" id="txtAddressLine1" value="{$T.ADD.AddressLine1}" />
                            </td>
                            <td width="20%" align="left" valign="middle">
                                Address Line2
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <input name="txtAddressLine2" type="text" class="width200 mrs" id="txtAddressLine2" value="{$T.ADD.AddressLine2}" />
                            </td>
                        </tr>
                        <tr>
                            <td width="20%" align="left" valign="middle">
                                State
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <input name="txtState" type="text" class="width200 mrs" id="txtState" value="{$T.ADD.State}" />
                            </td>
                            <td width="20%" align="left" valign="middle">
                                City
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <input name="txtCity" type="text" class="width200 mrs" id="txtCity" value="{$T.ADD.City}" />
                            </td>
                        </tr>
                        <tr>
                            <td width="20%" align="left" valign="middle">
                                Zipcode
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <input name="txtZipCode" type="text" class="width200 mrs" id="txtZipCode" value="{$T.ADD.ZipCode}" />
                            </td>
                            <td width="20%" align="left" valign="middle">
                                Country
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <input name="txtCountry" type="text" class="width200 mrs" id="txtCountry" value="{$T.ADD.Country}" />
                            </td>
                        </tr>
                    </tbody>
                </table>
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="itinerary">
                    <tbody>
                        <tr>
                            <th width="20%" align="left" valign="middle" colspan="4">
                                Bank account Information
                            </th>
                        </tr>
                        <tr>
                            <td width="20%" align="left" valign="middle">
                                Bank name
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <input name="txtBankName" type="text" class="width200 mrs" id="txtBankName" value="{$T.BankAccount.BankName}" />
                            </td>
                            <td width="20%" align="left" valign="middle">
                                IFSC code
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <input name="txtIFSCCode" type="text" class="width200 mrs" id="txtIFSCCode" value="{$T.BankAccount.IFSCCode}" />
                            </td>
                        </tr>
                        <tr>
                            <td width="20%" align="left" valign="middle">
                                Account holder name
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <input name="txtAccHolderName" type="text" class="width200 mrs" id="txtAccHolderName" value="{$T.BankAccount.AccountHolderName}" />
                            </td>
                            <td width="20%" align="left" valign="middle">
                                Account Number
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <input name="txtAccNumber" type="text" class="width200 mrs" id="txtAccNumber" value="{$T.BankAccount.AccountNumber}" />
                            </td>
                        </tr>
                        <tr>
                            <td width="20%" align="left" valign="middle">
                                Branch name and address
                            </td>
                            <td width="30%" align="left" valign="middle" colspan="3">
                                <input name="txtBankBranch" type="text" class="width90P mrs" id="txtBankBranch" value="{$T.BankAccount.Branch}" />
                            </td>
                        </tr>
                        
                    </tbody>
                </table>
                <table width="100%" border="0" cellspacing="0" cellpadding="0" class="itinerary">
                    <tbody>
                       <tr>
                            <td width="20%" align="left" valign="middle">
                                <div class="btnholder">
                                    <a href="#" class="button_orange" id="btnSearch" onclick="operatorSettings.SaveBusOperator()">Save</a>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </form>
        </textarea>
    </div>
</asp:Content>
