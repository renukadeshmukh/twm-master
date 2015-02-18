<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Main.Master" AutoEventWireup="true" CodeBehind="ManageBookings.aspx.cs" Inherits="TravelWithMe.ManageBookings" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/jquery.mCustomScrollbar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery.mousewheel.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.mCustomScrollbar.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-timepicker-addon.js" type="text/javascript"></script>
    <script src="Scripts/ManageBookings/ManageBookings.js" type="text/javascript"></script>
    <script src="Scripts/ManageBookings/BookingDetailsManager.js" type="text/javascript"></script>
    <link href="Styles/style.css" rel="stylesheet" type="text/css" media="print" />
    <script src="Scripts/jquery.PrintArea.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="whiteBox buslist left">
        <div style="margin: 2px">
            <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Table1">
                <tbody>
                    <tr>
                        <th width="60%" align="left" valign="middle">
                            Bus List
                        </th>
                        <th width="30%" align="left" valign="middle">
                        </th>
                    </tr>
                </tbody>
            </table>
        </div>
        <div id="buslist" class="scrollbox">
            <div id="buslistContainer">
            </div>
        </div>
    </div>
    <div class="whiteBox busdetails right">
        <form id="frmBookSeats">
            <div id="detailsContainer" style="height: 100%">
            </div>
        </form>
    </div>
    <div id="templateContainer" style="display: none">
        <textarea id="templateBusShortInfo">
            {#foreach $T.Buses as bus}
            <table width="99%" height="75px" cellspacing="0" cellpadding="0" class="itinerary" id="busItem{$T.bus.BusTripId}">
                <tbody>
                    <tr>
                        <td width="50%" align="left" valign="middle">
                            {$T.bus.BusName}
                        </td>
                        <td width="50%" align="left" valign="middle">
                            {#if $T.bus.IsAC}AC{#else}Non AC{#/if} {$T.bus.BusType}
                        </td>
                    </tr>
                    <tr>
                        <td width="50%" align="left" valign="middle">
                            {#if $T.bus.FromLoc}{$T.bus.FromLoc.Name}{#/if}<span class="ico-fwd"></span>{#if $T.bus.FromLoc}{$T.bus.ToLoc.Name}{#/if}
                        </td>
                        <td width="50%" align="left" valign="middle">
                            {$T.bus.DepartureTime.Hours}:{$T.bus.DepartureTime.Minutes} {$T.bus.DepartureTime.Meridian}-{$T.bus.ArrivalTime.Hours}:{$T.bus.ArrivalTime.Minutes} {$T.bus.ArrivalTime.Meridian}
                            {#if $T.bus.ArrivalTime.Days == 2}(next day){#/if}{#if $T.bus.ArrivalTime.Days > 2}day-{$T.bus.ArrivalTime.Days}{#/if}
                        </td>
                    </tr>
                </tbody>
            </table>
            {#/for}
        </textarea>
        <textarea id="templateBusDetails">
            <div  style="height:100%" class="whiteBoxContent">
                <div>
                    <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="busDetails{$T.BusTripId}">
                    <tbody>
                        <tr>
                            <th width="25%" align="left" valign="middle">
                                Bus details
                            </th>
                            <th width="25%" align="right" valign="middle">
                                
                            </th>
                            <th width="25%" align="rigth" valign="middle">
                                Travel Date
                            </th>
                            <th width="25%" align="left" valign="middle">
                                <input name="txtTravelDate" type="text" placeholder="Click to select travel date" class="width150 mrs" id="txtTravelDate" value="" />
                            </th>
                        </tr>
                        <tr>
                            <td width="25%" align="left" valign="middle" colspan="2">
                                {$T.BusName}
                            </td>
                            <td width="25%" align="left" valign="middle" colspan="2">
                                {#if $T.IsAC}AC{#else}Non AC{#/if} {$T.BusType}
                            </td>
                        </tr>
                        <tr>
                            <td width="25%" align="left" valign="middle" colspan="2">
                                {#if $T.FromLoc}{$T.FromLoc.Name}{#/if}<span class="ico-fwd"></span>{#if $T.FromLoc}{$T.ToLoc.Name}{#/if}
                            </td>
                            <td width="25%" align="left" valign="middle" colspan="2">
                                {$T.DepartureTime.Hours}:{$T.DepartureTime.Minutes} {$T.DepartureTime.Meridian}-{$T.ArrivalTime.Hours}:{$T.ArrivalTime.Minutes} {$T.ArrivalTime.Meridian}
                                {#if $T.ArrivalTime.Days == 2}(next day){#/if}{#if $T.ArrivalTime.Days > 2}day-{$T.ArrivalTime.Days}{#/if}
                            </td>
                        </tr>
                    </tbody>
                </table>
                </div>
                <div id="scrollBusDetails" class="busScroller">
                <div style="height:1000px">
                    <ul class="toptabs border_b"  style="display:none">
                        <li class="active" id="liMapView"><a id="tabMapView" href="#">Map View</a></li>
                        <li id="liListView"><a id="tabListView" href="#">List View</a></li>
                    </ul>
                    <div id="dvBookedSeatMap" style="margin:3px 0px">
                        
                    </div>
                </div>
                </div>
            </div>
        </textarea>
        <textarea id="bookedSeatDetails">
            <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="seatArrengment{$T.BusTripId}">
                    <tbody>
                        <tr>
                            {#if $T.Error}
                            <td width="10%" align="center" valign="middle" colspan="6">
                                <b>{$T.Error}</b>
                            </td>
                            {#else}
                            <td width="10%" align="left" valign="middle" colspan="6">
                                <div>
                                    <div id="seatMapArea">
                                        <div class="seatmaparea" style="width: 70%">
                                            <div class="deck">
                                                <div class="cabin">
                                                    <div class="lowerDeck"></div>
                                                </div>
                                            </div>
                                            <div class="deck">
                                                <div class="cabin">
                                                    <div class="upperDeck"></div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="seatBookInfo" style="width: 28%">
                                            <table width="100%" style="table-layout: fixed">
                                                <tr>
                                                    <td width="60%">Available seats</td>
                                                    <td width="40%"><div class="avialMarker"></div></td>
                                                </tr>
                                                <tr>
                                                    <td width="60%">Reserved seats</td>
                                                    <td width="40%"><div class="reserveMarker"></div></td>
                                                </tr>
                                                <tr>
                                                    <td width="60%">Selected seats</td>
                                                    <td width="40%"><div class="selectedMarker"></div></td>
                                                </tr>
                                                {#if $T.SeatMap.BerthCount != 0}
                                                <tr>
                                                    <td width="60%">Berth Count</td>
                                                    <td width="40%">{$T.SeatMap.BerthCount}</td>
                                                </tr>
                                                {#/if}
                                                {#if $T.SeatMap.SeatCount != 0}
                                                <tr>
                                                    <td width="60%">Seats Count</td>
                                                    <td width="40%">{$T.SeatMap.SeatCount}</td>
                                                </tr>
                                                {#/if}
                                                <tr>
                                                    <td width="60%">Seat(s):</td>
                                                    <td width="40%" id="selectedSeats" style="word-wrap: break-word"></td>
                                                </tr>
                                                <tr>
                                                    <td width="60%">Total:</td>
                                                    <td width="40%" id="totalAmount"></td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="2">
                                                        <a href="#" class="showSeatsButton" onclick="bookingManager.BDM.ShowSeatMapStatus();">Refresh Bookings</a>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </td>
                            {#/if}
                        </tr>
                    </tbody>
                </table>
                <div id="tripInfoContainer">
                
                </div>
        </textarea>
        <textarea id="tempateBookingList">
            <table>
            <tr>
                <td>
                    <div class="border_b pam">
                        <div class="header_search">
                            Search Bookings:
                            <input name="txtSearchBookings" type="text" class="searchfield" id="txtSearchBookings" onkeyup="bookingManager.CreateSearchTerm();" />
                         </div>
                        <div class="clear">
                        </div>
                    </div>
                </td>
                <td>
                    <div>
                        <a href="#" id="lnkPrint">Print</a>
                    </div>
                </td>
            </tr>
            </table>
            <div class="clear">
            </div>
            <table id="tblBookings" width="100%" border="0" cellspacing="0" cellpadding="0" class="itinerary">
                <tr>
                    <th width="5%" align="left" valign="middle">
                        #
                    </th>
                    <th width="40%" align="left" valign="middle">
                        Name
                    </th>
                    <th width="15%" align="left" valign="middle">
                        Seat(s)
                    </th>
                    <th width="20%" align="center" valign="middle">
                        Phone #
                    </th>
                    <th width="10%" align="center" valign="middle">
                        Total
                    </th>
                    <th width="10%" align="center" valign="middle">
                        Action
                    </th>
                </tr>
            </table>
        
        </textarea>
        <textarea id="templateBookingRow">
            {#foreach $T.Bookings as BK}
            <tr id="BookingRow_{$T.BK.BookingId}">
                <td width="5%" align="left" valign="middle">
                    {$T.BK.BookingId}
                </td>
                <td width="40%" align="left" valign="middle">
                    {#if $T.BK.Passengers}{$T.BK.Passengers[0].FirstName} {$T.BK.Passengers[0].LastName}{#/if}
                </td>
                <td width="25%" align="left" valign="middle">
                    {#if $T.BK.SelectedSeats}
                        {#foreach $T.BK.SelectedSeats as s}
                        <span class="frequencyTag">{$T.s}</span>
                        {#/for}
                    {#/if}
                </td>
                <td width="20%" align="center" valign="middle">
                    {$T.BK.ContactNumber}
                </td>
                <td width="10%" align="center" valign="middle">
                    Rs. {$T.BK.TotalAmount}
                </td>
                <td width="10%" align="center" valign="middle">
                    <a href="#" class="showSeatsButton" onclick="bookingManager.BDM.EditBooking('{$T.BK.BookingId}');">Edit</a> 
                </td>
            </tr>
            {#/for}
        </textarea>
        <textarea id="templateTripInformation">
            {#if $T.PrintTicket || $T.Error}
            <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Table3">
                <tbody>
                    {#if $T.PrintTicket}
                    <tr>
                        <td width="20%" align="left" valign="middle">
                            Booking Id:
                        </td>
                        <td width="20%" align="left" valign="middle">
                            {$T.PrintTicket.BookingId}
                        </td>
                        <td width="60%" align="center" valign="middle">
                            <a href="Confirmation.aspx?BookingId={$T.PrintTicket.BookingId}&BusTripId={$T.PrintTicket.BusTripId}" target="_blank">Show Ticket</a>
                        </td>
                    </tr>
                    {#/if}
                    {#if $T.Error}
                    <tr>
                        <td width="20%" align="left" valign="middle" colspan="3">
                            <span style="color:Red">Booking failed, Error: {$T.Error}</span>
                        </td>
                    </tr>
                    {#/if}
                </tbody>
            </table>
            {#else}
            <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Table2">
                <tbody>
                        <tr>
                            <td width="15%" align="left" valign="middle">
                                Boarding at:
                            </td>
                            <td width="40%" align="left" valign="middle" colspan="3">
                                <select name="cmbBoardingPoint" id="cmbBoardingPoint">
                                    {#foreach $T.CityPoints as CP}
                                    {#if $T.CP.IsPickupPoint}
                                    <option value="{$T.CP.CPId}"  {#if $T.PickupPoint && $T.CP.CPId == $T.PickupPoint.CPId} selected="selected" {#/if}>{$T.CP.CPName}({$T.CP.CityName}) - {#if $T.CP.CPTime}{$T.CP.CPTime.Hours}:{$T.CP.CPTime.Minutes} {$T.CP.CPTime.Meridian} {#if $T.CP.CPTime.Days==2}(Next Day){#/if}{#if $T.CP.CPTime.Days>2}(Day-$T.CP.CPTime.Days){#/if}{#/if}</option>
                                    {#/if}
                                    {#/for}
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <td width="15%" align="left" valign="middle">
                                Alignting at:
                            </td>
                            <td width="40%" align="left" valign="middle" colspan="3">
                                <select name="cmbAligntingPoint" id="cmbAligntingPoint">
                                    {#foreach $T.CityPoints as CP}
                                    {#if $T.CP.IsDropOffPoint}
                                    <option value="{$T.CP.CPId}" {#if $T.DropOffPoint && $T.CP.CPId == $T.DropOffPoint.CPId} selected="selected" {#/if}>{$T.CP.CPName}({$T.CP.CityName}) - {#if $T.CP.CPTime}{$T.CP.CPTime.Hours}:{$T.CP.CPTime.Minutes} {$T.CP.CPTime.Meridian} {#if $T.CP.CPTime.Days==2}(Next Day){#/if}{#if $T.CP.CPTime.Days>2}(Day-$T.CP.CPTime.Days){#/if}{#/if}</option>
                                    {#/if}
                                    {#/for}
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <td width="15%" align="left" valign="middle">
                                Email:
                            </td>
                            <td width="40%" align="left" valign="middle">
                                <input name="txtEmailId" type="text" class="width150 mrs" id="txtEmailId" value="{#if $T.Email}{$T.Email}{#/if}" />
                            </td>
                            <td width="5%" align="left" valign="middle">
                                Mobile:
                            </td>
                            <td width="40%" align="left" valign="middle">
                                <input name="txtCountryCode" type="text" class="width45 mrs" id="txtCountryCode" value="{#if $T.CountryCode}{$T.CountryCode}{#/if}" />
                                <input name="txtPhoneNumber" type="text" class="width100 mrs" id="txtPhoneNumber" value="{#if $T.PhoneNumber}{$T.PhoneNumber}{#/if}" />
                            </td>
                        </tr>
                </tbody>
            </table>
            <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="tblPassengers">
                <tr>
                    <td width="35%" align="left" valign="middle" colspan="4">
                        <b>Passenger information:</b>
                    </td>
                </tr>
                <tr>
                    <th width="35%" align="left" valign="middle">
                        First Name
                    </th>
                    <th width="35%" align="left" valign="middle">
                        Last Name
                    </th>
                    <th width="10%" align="center" valign="middle">
                        Age
                    </th>
                    <th width="20%" align="center" valign="middle">
                        Gender
                    </th>
                </tr>
            </table>
            <div class="btnholder">
                {#if $T.BI.BookingId === 0} <a href="#" class="button_orange" id="btnPay" onclick="bookingManager.BDM.BookNow()">Book Now</a>{#/if}
            </div>
            <div class="btnholder">
                {#if $T.BI.BookingId != 0} <a href="#" class="button_orange" id="btnSave" onclick="bookingManager.BDM.SaveBooking('{$T.BI.BookingId}')">Save</a>{#/if}
            </div>
            <div class="btnholder">
                {#if $T.BI.BookingId != 0} <a href="#" class="button_orange" id="btnDelete" onclick="bookingManager.BDM.DeleteBooking('{$T.BI.BookingId}')">Delete</a>{#/if}
            </div>
            <div class="btnholder">
                <a href="#" class="button_orange" id="btnCancelChanges" onclick="bookingManager.BDM.CancelChanges()">Cancel</a>
            </div>
            {#/if}
        </textarea>
        <textarea id="templatePax">
            <tr id="trPaxsRow{$T.SeatNumber}">
                <td width="35%" align="left" valign="middle">
                    <input name="txtFName{$T.SeatNumber}" type="text" class="width150 mrs" id="txtFName{$T.SeatNumber}" value="{#if $T.FirstName}{$T.FirstName}{#/if}" />
                </td>
                <td width="35%" align="left" valign="middle">
                    <input name="txtLName{$T.SeatNumber}" type="text" class="width150 mrs" id="txtLName{$T.SeatNumber}" value="{#if $T.LastName}{$T.LastName}{#/if}" />
                </td>
                <td width="10%" align="center" valign="middle">
                    <input name="txtAge{$T.SeatNumber}" type="text" class="width20 mrs" id="txtAge{$T.SeatNumber}" value="{#if $T.Age}{$T.Age}{#/if}" />
                </td>
                <td width="20%" align="center" valign="middle">
                    <select name="cmbGender{$T.SeatNumber}" id="cmbGender{$T.SeatNumber}">
                        <option {#if $T.Gender == 'Male'}selected="selected"{#/if}>Male</option>
                        <option {#if $T.Gender == 'Female'}selected="selected"{#/if}>Female</option>
                    </select>
                </td>
            </tr>
        </textarea>
        <div id="tempDiv"></div>
    </div>
</asp:Content>
