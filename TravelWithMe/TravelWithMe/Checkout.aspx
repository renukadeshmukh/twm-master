<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Main.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="TravelWithMe.Checkout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <script src="Scripts/Result/Results.js" type="text/javascript"></script>
    <script src="Scripts/Result/Checkout.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="frmCheckOut">
        <div class="whiteBox width73P padding2 left">
            <div id="resultsContainer">
            </div>
        </div>
        <div class="whiteBox width25P padding2 right">
            <div id="bookingSummary">
            
            </div>
        </div>
    </form>
    <div id="templateContainer" style="display: none">
        <textarea id="templateItinerary">
        <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Table1">
            <tbody>
                <tr>
                    <th width="25%" align="left" valign="middle">
                        Bus/Operator
                    </th>
                    <th width="20%" align="left" valign="middle">
                        Bus Type
                    </th>
                    <th width="25%" align="left" valign="middle">
                        Dept/Arr
                    </th>
                    <th width="20%" align="left" valign="middle">
                        Time/Seats
                    </th>
                    <th width="10%" align="left" valign="middle">
                        Price
                    </th>
                </tr>
            </tbody>
        </table>
        {#foreach $T.Results as itn}
        <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Itn{$T.itn.BusTripId}">
            <tbody>
                <tr>
                    <td width="25%" align="left" valign="middle">
                        {$T.itn.OperatorName}
                    </td>
                    <td width="20%" align="left" valign="middle">
                        {$T.itn.BusType} 
                    </td>
                    <td width="25%" align="left" valign="middle">
                        {#if $T.itn.DepartureTime}{$T.itn.DepartureTime.Hours}:{$T.itn.DepartureTime.Minutes} {$T.itn.DepartureTime.Meridian}{#/if}
                    </td>
                    <td width="20%" align="left" valign="middle">
                        {$T.itn.JourneryTime} 
                    </td>
                    <td width="10%" align="left" valign="middle">
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
                    <td width="25%" align="left" valign="middle">
                        {#if $T.itn.ArrivalTime}{$T.itn.ArrivalTime.Hours}:{$T.itn.ArrivalTime.Minutes} {$T.itn.ArrivalTime.Meridian} {#if $T.itn.ArrivalTime.Days==2}(Next Day){#/if}{#if $T.itn.ArrivalTime.Days>2}(Day-$T.itn.ArrivalTime.Days){#/if}{#/if}
                    </td>
                    <td width="20%" align="left" valign="middle">
                        {$T.itn.SeatsAvailable} 
                    </td>
                </tr>
                <tr>
                    <td colspan="5" align="left" valign="top">
                        <div id="SeatDetails{$T.itn.BusTripId}">
                            <div id="seatMapArea{$T.itn.BusTripId}">
                                
                            </div>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
        {#/for}
        </textarea>
        <textarea id="templateSeatMap">
            <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="seatArrengment{$T.BusTripId}">
                <tbody>
                    <tr>
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
                                            
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
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
                <a href="#" class="button_orange" id="btnPay" onclick="checkoutMgr.PayNow('{$T.BusTripId}')">Book Now</a>
            </div>
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
        <textarea id="templateBookingSummary">
            <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Table4">
                <tbody>
                    <tr>
                        <th width="30%" align="left" valign="middle" colspan="2">
                            {$T.From} <span class="ico-fwd"></span>{$T.To} | {$T.TravelDate}
                        </th>
                    </tr>
                </tbody>
            </table>
            <table width="100%" style="table-layout: fixed" id="tblSeatDetails{$T.BusTripId}">
                <tr>
                    <td width="40%">Seat(s):</td>
                    <td width="60%" id="selectedSeats" style="word-wrap: break-word"><B>{#if $T.SelectedSeats}{$T.SelectedSeats}{#/if}</B></td>
                </tr>
                <tr>
                    <td width="40%">Total:</td>
                    <td width="60%" id="totalAmount"><b>Rs. {#if $T.TotalAmount}{$T.TotalAmount}{#/if}</b></td>
                </tr>
            </table>
        </textarea>
        <div id="tempDiv">
        </div>
    </div>
</asp:Content>
