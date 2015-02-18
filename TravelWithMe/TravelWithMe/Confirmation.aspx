<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Main.Master" AutoEventWireup="true" CodeBehind="Confirmation.aspx.cs" Inherits="TravelWithMe.Confirmation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/style.css" rel="stylesheet" type="text/css" media="print" />
    <script src="Scripts/Result/Confirmation.js" type="text/javascript"></script>
    <script src="Scripts/jquery.PrintArea.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div>
    <div id="bookingInfoContainer">
        
    </div>
</div>
<div id="templates" style="display: none">
    <textarea id="templateBookingInfo">
    {#if $T.BI.ShowSuccess}
    <div class="whiteBox width73P padding2 left" style="margin:2px 0px">
        <div style="color:Green; padding:10px; font-size:10pt;">
            <b>Congratulations! Your booking was successful.</b>
        </div>
    </div>
    {#/if}
    <div class="whiteBox width25P padding2 right" style="margin:2px 0px">
        <div style="margin:10px; font-size:10pt; float:left">
            Booking Id: <b id="bookingId">{$T.BI.BookingId}</b>
        </div>
        <div style="margin:10px; font-size:10pt; float:right">
                <a href="#" id="lnkPrint">Print</a>
        </div>
    </div>
    <div class="whiteBox width73P padding2 left">
        <div id="resultsContainer">
            <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Table2">
                <tbody>
                    <tr>
                        <th width="20%" align="left" valign="middle" colspan="4">
                            Bus and Journey details
                        </th>
                    </tr>
                    <tr>
                        <td width="20%" align="left" valign="middle">
                            <b>Bus name:</b>
                        </td>
                        <td width="30%" align="left" valign="middle">
                            {#if $T.SI.BusName}{$T.SI.BusName}{#/if}
                        </td>
                        <td width="20%" align="left" valign="middle">
                            <b>AC:</b>
                        </td>
                        <td width="30%" align="left" valign="middle">
                            {#if $T.SI.IsAC} Yes {#else} No {#/if}
                        </td>
                    </tr>
                    <tr>
                        <td width="20%" align="left" valign="middle">
                            <b>Operator:</b>
                        </td>
                        <td width="30%" align="left" valign="middle">
                            {#if $T.SI.OperatorName}{$T.SI.OperatorName}{#/if}
                        </td>
                        <td width="20%" align="left" valign="middle">
                            <b>Bus Type:</b>
                        </td>
                        <td width="30%" align="left" valign="middle">
                            {#if $T.SI.BusType}{$T.SI.BusType}{#/if}
                        </td>
                    </tr>
                    
                    <tr>
                        <td width="20%" align="left" valign="middle">
                            <b>Journey time:</b>
                        </td>
                        <td width="30%" align="left" valign="middle">
                            {#if $T.SI.JourneryTime}{$T.SI.JourneryTime}{#/if}
                        </td>
                        <td width="20%" align="left" valign="middle">
                            <b>Per seat price:</b>
                        </td>
                        <td width="30%" align="left" valign="middle">
                            {#if $T.SI.Fare}{$T.SI.Fare}{#/if}
                        </td>
                    </tr>
                    <tr>
                        <td width="20%" align="left" valign="middle">
                            <b>Email:</b>
                        </td>
                        <td width="30%" align="left" valign="middle">
                            {#if $T.BI.Email}{$T.BI.Email}{#/if}
                        </td>
                        <td width="20%" align="left" valign="middle">
                            <b>Mobile:</b>
                        </td>
                        <td width="30%" align="left" valign="middle">
                            {#if $T.BI.ContactNumber}{$T.BI.ContactNumber}{#/if}
                        </td>
                    </tr>
                    <tr>
                        <td width="20%" align="left" valign="middle">
                            <b>Boarding at:</b>
                        </td>
                        <td width="30%" align="left" valign="middle" colspan="3">
                            {#if $T.BI.PickupPoint && $T.BI.PickupPoint.CPName}{$T.BI.PickupPoint.CPName}{#/if}
                        </td>
                    </tr>
                    <tr>
                        <td width="20%" align="left" valign="middle">
                            <b>Alignting at:</b>
                        </td>
                        <td width="30%" align="left" valign="middle" colspan="3">
                            {#if $T.BI.DropOffPoint && $T.BI.DropOffPoint.CPName}{$T.BI.DropOffPoint.CPName}{#/if}
                        </td>
                    </tr>
                    <tr>
                        <td width="20%" align="left" valign="middle">
                            <b>Transaction Id:</b>
                        </td>
                        <td width="30%" align="left" valign="middle" colspan="3">
                            {#if $T.BI.TransactionId}{$T.BI.TransactionId}{#/if}
                        </td>
                    </tr>
                </tbody>
            </table>
            <table width="100%" cellspacing="0" cellpadding="0" class="itinerary">
                <tr>
                    <th width="60%" align="left" valign="middle">
                        Passenger Name
                    </th>
                    <th width="20%" align="center" valign="middle">
                        Age
                    </th>
                    <th width="20%" align="center" valign="middle">
                        Gender
                    </th>
                </tr>
                {#foreach $T.BI.Passengers as pax}
                <tr>
                    <td width="60%" align="left" valign="middle">
                        {#if $T.pax.FirstName}{$T.pax.FirstName}{#/if} {#if $T.pax.LastName}{$T.pax.LastName}{#/if}
                    </td>
                    <td width="20%" align="center" valign="middle">
                        {#if $T.pax.Age}{$T.pax.Age}{#/if}
                    </td>
                    <td width="20%" align="center" valign="middle">
                        {#if $T.pax.Gender}{$T.pax.Gender}{#/if}
                    </td>
                </tr>
                {#/for}
            </table>
            <table width="100%" cellspacing="0" cellpadding="0" class="itinerary">
                <tr>
                    <th width="100%" align="left" valign="middle">
                        <b>Important information:</b>
                    </th>
                </tr>
                <tr>
                    <td width="100%" align="left" valign="middle">
                        <ul>
                            <li>This e-ticket is not transferable.</li>
                            <li>This e-ticket is valid only for the details specified above.</li>
                            <li>One of the passenger on this e-ticket is required to carry the print out of this e-ticket along with Photo ID Card in Original like Voter Id Card, Pan Card, Driving Licence, Passport.</li>
                            <li>Corporation reserves the rights to change/cancel the bus service mentioned in the e-ticket.</li>
                            <li>Passenger will have to pay the fare difference if any in case of Changeof Service or in case of Revision of fare.</li>
                        </ul>
                    </td>
                </tr>
                <tr>
                    <td width="100%" align="center" valign="middle">
                        <b>Wish You Happy Journey</b>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="whiteBox width25P padding2 right">
        <div id="bookingSummary">
            <table width="100%" cellspacing="0" cellpadding="0" class="itinerary" id="Table4">
                <tbody>
                    <tr>
                        <th width="30%" align="left" valign="middle" colspan="2">
                            {$T.BSum.From} <span class="ico-fwd"></span>{$T.BSum.To}
                        </th>
                    </tr>
                    <tr>
                        <td width="40%">Travel Date:</td>
                        <td width="60%"><B>{#if $T.BSum.TravelDate}{$T.BSum.TravelDate}{#/if}</B></td>
                    </tr>
                    <tr>
                        <td width="40%">Dapt time:</td>
                        <td width="60%"><B>{#if $T.SI.DepartureTime}{$T.SI.DepartureTime.Hours}:{$T.SI.DepartureTime.Minutes} {$T.SI.DepartureTime.Meridian}{#/if}</B></td>
                    </tr>
                    <tr>
                        <td width="40%">Arrival time:</td>
                        <td width="60%"><B>{#if $T.SI.ArrivalTime}{$T.SI.ArrivalTime.Hours}:{$T.SI.ArrivalTime.Minutes} {$T.SI.ArrivalTime.Meridian} {#if $T.SI.ArrivalTime.Days==2}(Next Day){#/if}{#if $T.SI.ArrivalTime.Days>2}(Day-$T.SI.ArrivalTime.Days){#/if}{#/if}</B></td>
                    </tr>
                    <tr>
                        <td width="40%">Seat(s):</td>
                        <td width="60%" id="selectedSeats" style="word-wrap: break-word"><B>{#if $T.BSum.SelectedSeats}{$T.BSum.SelectedSeats}{#/if}</B></td>
                    </tr>
                    <tr>
                        <td width="40%">Total Amount:</td>
                        <td width="60%" id="totalAmount"><b>Rs. {#if $T.BSum.TotalAmount}{$T.BSum.TotalAmount}{#/if}</b></td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    
    </textarea>
</div>
</asp:Content>
