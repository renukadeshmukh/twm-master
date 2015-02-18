<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Main.Master" AutoEventWireup="true"
    CodeBehind="Results.aspx.cs" Inherits="TravelWithMe.Results" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="Scripts/Result/Results.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="whiteBox left width25P padding2">
        <form id="frmSearchBox">
        <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="searchBox">
            <tbody>
                <tr>
                    <th width="30%" align="left" valign="middle" colspan="2">
                        Modify Search
                    </th>
                </tr>
                <tr>
                    <td width="30%" align="left" valign="middle">
                        From
                    </td>
                    <td width="70%" align="left" valign="middle">
                        <input name="txtFromCity" type="text" class="width150 mrs" id="txtFromCity" value="Latur-(LUR)" />
                    </td>
                </tr>
                <tr>
                    <td width="30%" align="left" valign="middle">
                        To
                    </td>
                    <td width="70%" align="left" valign="middle">
                        <input name="txtToCity" type="text" class="width150 mrs" id="txtToCity" value="Pune-(PUN)" />
                    </td>
                </tr>
                <tr>
                    <td width="30%" align="left" valign="middle">
                        Date
                    </td>
                    <td width="70%" align="left" valign="middle">
                        <input name="txtTravelDate" type="text" class="width150 mrs" id="txtTravelDate" value="Mar 15, 2013" />
                    </td>
                </tr>
                <tr>
                    <td width="30%" align="left" valign="middle">
                    </td>
                    <td width="70%" align="left" valign="middle">
                        <div class="btnholder">
                            <a href="#" class="button_orange" id="btnSearch">Search</a>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
        </form>
        <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Table2" style="display:none">
            <tbody>
                <tr>
                    <th width="100%" align="left" valign="middle" colspan="2">
                        Filters
                    </th>
                </tr>
                <tr>
                    <td width="100%" align="left" valign="middle" colspan="2">
                        <div width="100%" class="showHideButton">
                            <div style="float: left">
                                Show</div>
                            <div style="float: left">
                                &nbsp;Operators</div>
                            <div class="showdetails">
                            </div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div class="whiteBox right width73P padding2">
        <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Table3">
            <tbody>
                <tr>
                    <td width="30%" align="left" valign="middle" colspan="2">
                        <span id="spnFrom"></span> <span class="ico-fwd"></span><span id="spnTo"></span> | <span id="spnTravelDate"></span> 
                    </td>
                </tr>
                <tr>
                    <td width="30%" align="left" valign="middle" colspan="2" style="display:none">
                        <div class="filterdiv">
                            <div class="closeTag" title="Remove this filter">
                            </div>
                            <div>
                                Sleeper</div>
                        </div>
                        <div class="filterdiv">
                            <div class="closeTag" title="Remove this filter">
                            </div>
                            <div>
                                AC</div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
        <div id="resultsContainer">
        </div>
    </div>
    <div id="templateContainer" style="display: none">
        <textarea id="templateItinerary">
        <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Table1">
            <tbody>
                {#if $T.Results.length == 0}
                <tr>
                    <th width="25%" align="center" valign="middle">
                        Sorry! No results found for this route.
                    </th>
                </tr>
                {#else}
                <tr>
                    <th width="25%" align="left" valign="middle">
                        Bus/Operator
                    </th>
                    <th width="20%" align="left" valign="middle">
                        Bus Type
                    </th>
                    <th width="20%" align="left" valign="middle">
                        Dept/Arr
                    </th>
                    <th width="20%" align="left" valign="middle">
                        Time/Seats
                    </th>
                    <th width="15%" align="center" valign="middle">
                        Price
                    </th>
                </tr>
                {#/if}
            </tbody>
        </table>
        {#foreach $T.Results as itn}
        <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Itn{$T.itn.BusTripId}">
            <tbody>
                <tr>
                    <td width="25%" align="left" valign="middle" style="padding:10px 5px">
                        {$T.itn.OperatorName}
                    </td>
                    <td width="20%" align="left" valign="middle" style="padding:10px 5px">
                        {$T.itn.BusType} 
                    </td>
                    <td width="20%" align="left" valign="middle" style="padding:10px 5px">
                        {#if $T.itn.DepartureTime}{$T.itn.DepartureTime.Hours}:{$T.itn.DepartureTime.Minutes} {$T.itn.DepartureTime.Meridian}{#/if}
                    </td>
                    <td width="20%" align="left" valign="middle" style="padding:10px 5px">
                        {$T.itn.JourneryTime} 
                    </td>
                    <td width="15%" align="center" valign="middle" style="padding:10px 5px">
                        Rs. {$T.itn.Fare}
                    </td>
                </tr>
                <tr>
                    <td width="25%" align="left" valign="middle">
                        {$T.itn.BusName}
                    </td>
                    <td width="20%" align="left" valign="middle">
                        {#if $T.itn.IsAC} AC {#else} Non AC {#/if}
                    </td>
                    <td width="20%" align="left" valign="middle">
                        {#if $T.itn.ArrivalTime}{$T.itn.ArrivalTime.Hours}:{$T.itn.ArrivalTime.Minutes} {$T.itn.ArrivalTime.Meridian} {#if $T.itn.ArrivalTime.Days==2}(Next Day){#/if}{#if $T.itn.ArrivalTime.Days>2}(Day-$T.itn.ArrivalTime.Days){#/if}{#/if}
                    </td>
                    <td width="20%" align="left" valign="middle">
                        {#if $T.itn.SeatsAvailable && $T.itn.SeatsAvailable > 0}{$T.itn.SeatsAvailable} Available{#/if}
                    </td>
                    <td width="15%" align="center" valign="middle">
                        <div class="smalltxt">
                            <a href="#" class="showSeatsButton" onclick="invManager.HideSeats('{$T.itn.BusTripId}'); return false;" id="hide{$T.itn.BusTripId}" style="display: none;">Hide seats</a> 
                            <a href="#" class="showSeatsButton" onclick="invManager.ShowSeats('{$T.itn.BusTripId}'); return false;" id="show{$T.itn.BusTripId}">Show seats</a>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="5" align="left" valign="top">
                        <div id="SeatDetails{$T.itn.BusTripId}" style="display: none;">
                            <div class="loadingData"><div></div></div>
                            <div id="seatMapArea{$T.itn.BusTripId}">
                                
                            </div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
        {#/for}
        </textarea>
        <div id="tempDiv">
        </div>
    </div>
</asp:Content>
