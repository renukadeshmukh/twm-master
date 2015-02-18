<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Main.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="TravelWithMe.Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="Scripts/Result/Home.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="whiteBox left width66P padding2"  style="height: 460px">
    <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Table1">
        <tbody>
            <tr> 
                <th width="30%" align="left" valign="middle">
                    Simple online bus inventory management tool for bus operators <span style="font-size: large; color: green">(Free)</span>. <a href="#" id="lnkLogin">Sign Up</a> now!
                </th>
            </tr>
            <tr>
                <td width="30%" align="left" valign="middle" style="font-size: 11pt">
                    <ul>
                        <li>
                            Easy to manage bus information.
                            <ul>
                                <li>Manage Bus Schedule</li>
                                <li>Change bus rates at any time.</li>
                                <li>Manage bus seat arrangement</li>
                                <li>Manage bus pickup and drop-off points</li>
                            </ul>
                        </li>
                        <li>
                            Easy to manage bus bookings.
                            <ul>
                                <li>Book bus seats</li>
                                <li>Cancel bookings</li>
                                <li>Change/Edit bookings</li>
                                <li>Easy to manage passenger information like name, contact information.</li>
                                <li>Booking console on Mobile(Android) for bus operators and agents. ( Coming soon )</li>
                            </ul>
                        </li>
                        <li>
                            We will provide you the reports which will help you to manage your earnings.( Coming soon )
                        </li>
                        <li>
                            Online agents will help you to sell your bus inventory. ( Coming soon )
                        </li>
                    </ul>
                </td>
            </tr>
        </tbody>
    </table>
    <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Table3">
        <tbody>
            <tr> 
                <th width="30%" align="left" valign="middle">
                    Online bus ticket booking site for everyone.
                </th>
            </tr>
            <tr>
                <td width="30%" align="center" valign="middle">
                    <h1>COMING SOON</h1>
                </td>
            </tr>
        </tbody>
    </table>
</div>
<div class="whiteBox right width32P padding2">
    <form id="frmSearchBox">
    <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="searchBox">
        <tbody>
            <tr>
                <th width="50%" align="left" valign="middle" colspan="2">
                    Tell us about your journey details
                </th>
            </tr>
            <tr>
                <td width="50%" align="left" valign="middle">
                    From
                </td>
                <td width="50%" align="left" valign="middle">
                    <input name="txtFromCity" type="text" class="width150 mrs" placeholder="Type to select city" id="txtFromCity" value="" />
                </td>
            </tr>
            <tr>
                <td width="50%" align="left" valign="middle">
                    To
                </td>
                <td width="50%" align="left" valign="middle">
                    <input name="txtToCity" type="text" class="width150 mrs" placeholder="Type to select city" id="txtToCity" value="" />
                </td>
            </tr>
            <tr>
                <td width="50%" align="left" valign="middle">
                    Travel date
                </td>
                <td width="50%" align="left" valign="middle">
                    <input name="txtTravelDate" type="text" class="width150 mrs" placeholder="Click to select travel date" id="txtTravelDate" value="" />
                </td>
            </tr>
            <tr>
                <td width="50%" align="left" valign="middle">
                </td>
                <td width="50%" align="left" valign="middle">
                    <div class="btnholder">
                        <a href="#" class="button_orange" id="btnSearch">Search</a>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
    </form>
    <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="tblRecentSearches">
        
    </table>
</div>
<div id="templateContainer" style="display: none">
    <textarea id="templateRecentSearch">
        <tr>
            <th width="50%" align="left" valign="middle" colspan="2">
                Recent searches
            </th>
        </tr>
        {#foreach $T.Searches as RS}
        <tr>
            <td width="50%" align="left" valign="middle">
                {$T.RS.From.Name} <span class="ico-fwd"></span> {$T.RS.To.Name}
            </td>
            <td width="50%" align="center" valign="middle">
                <input name="txtTDate_{$T.RS.Id}" type="text" placeholder="Click to select travel date" class="width150 mrs" id="txtTDate_{$T.RS.From.Name}_{$T.RS.To.Name}" value="" />
            </td>
        </tr>
        {#/for}
    </textarea>
    <div id="tempDiv">
    </div>
</div>
</asp:Content>
