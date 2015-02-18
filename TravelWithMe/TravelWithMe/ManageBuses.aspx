<%@ Page Title="" Language="C#" MasterPageFile="~/Master/Main.Master" AutoEventWireup="true"
    CodeBehind="ManageBuses.aspx.cs" Inherits="TravelWithMe.ManageBuses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="Styles/jquery.mCustomScrollbar.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery.mousewheel.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.mCustomScrollbar.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-timepicker-addon.js" type="text/javascript"></script>
    <script src="Scripts/ManageBuses/ManageBusses.js" type="text/javascript"></script>
    <script src="Scripts/ManageBuses/BusDetailsManager.js" type="text/javascript"></script>
    <script src="Scripts/ManageBuses/BusScheduleManager.js" type="text/javascript"></script>
    <script src="Scripts/ManageBuses/CityPointManager.js" type="text/javascript"></script>
    <script src="Scripts/ManageBuses/RatesManager.js" type="text/javascript"></script>
    <script src="Scripts/ManageBuses/SeatArrengementManager.js" type="text/javascript"></script>
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
                            <div class="addDetails" title="Add new bus">
                            </div>
                            <a href="javascript:void(0);" onclick="new BusDetailsManager().ShowEditBusDetailsDialog('{$T.BusTripId}');">ADD NEW BUS</a>
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
        <div id="detailsContainer" style="height: 100%">
        </div>
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
                            {#if $T.bus.FromLoc}{$T.bus.FromLoc.Name}{#/if}<span class="ico-fwd"></span>{#if
                            $T.bus.FromLoc}{$T.bus.ToLoc.Name}{#/if}
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
                            <th width="20%" align="left" valign="middle" colspan="2">
                                Bus details <span class="greytxt" id="spBusStatus">[ <i title="Use this id for all communications with our customer service">{$T.BusTripId}</i> | {#if $T.IsPublished}<i title="You can change only rate of the bus!">Locked</i>{#else}<i title="You can change/add all bus information!">Unlocked</i>{#/if} | {#if $T.IsEnabled}<i title="Online customers can book this bus!">Enabled</i>{#else}<i title="Online customers can't book this bus!">Disabled</i>{#/if} ]</span>
                            </th>
                            <th width="20%" align="left" valign="middle">
                                <a href="#" id="lnkPublish" onclick="new BusDetailsManager().SetBusPublishStatus('{$T.BusTripId}','true');" {#if $T.IsPublished}style="display: none"{#/if}>Lock</a>
                                <a href="#" id="lnkUnPublish" onclick="new BusDetailsManager().SetBusPublishStatus('{$T.BusTripId}','false');" {#if $T.IsPublished == false}style="display: none"{#/if}>Unlock</a>
                            </th>
                            <th width="20%" align="left" valign="middle">
                                <a href="#" id="lnkEnableBus" onclick="new BusDetailsManager().SetBusStatus('{$T.BusTripId}','true');" {#if $T.IsEnabled}style="display: none"{#/if}>Enable</a>
                                <a href="#" id="lnkDisableBus" onclick="new BusDetailsManager().SetBusStatus('{$T.BusTripId}','false');" {#if $T.IsEnabled == false}style="display: none"{#/if}>Disable</a>
                            </th>
                            <th width="3%" align="left" valign="middle" class="setting" onclick="new BusDetailsManager().ShowEditBusDetailsDialog('{$T.BusTripId}');">
                                <div class="editdetails" title="Edit bus basic details"  />
                            </th>
                        </tr>
                        <tr>
                            <td width="20%" align="left" valign="middle">
                                Bus Name
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <b>{$T.BusName}</b>
                            </td>
                            <td width="20%" align="left" valign="middle">
                                Bus Type
                            </td>
                            <td width="27%" align="left" valign="middle" colspan="2">
                                <b>{#if $T.IsAC}AC{#else}Non AC{#/if} {$T.BusType}</b>
                            </td>
                        </tr>
                        <tr>
                            <td width="20%" align="left" valign="middle">
                                Departs From
                            </td>
                            <td width="30%" align="left" valign="middle">
                                {#if $T.FromLoc}<b>{$T.FromLoc.Name}</b>{#/if}
                            </td>
                            <td width="20%" align="left" valign="middle">
                                Arrive At
                            </td>
                            <td width="27%" align="left" valign="middle"  colspan="2">
                                {#if $T.ToLoc}<b>{$T.ToLoc.Name}<b>{#/if}
                            </td>
                        </tr>
                        <tr>
                            <td width="20%" align="left" valign="middle">
                                Departure Time
                            </td>
                            <td width="30%" align="left" valign="middle">
                                <b>{$T.DepartureTime.Hours}:{$T.DepartureTime.Minutes} {$T.DepartureTime.Meridian}</b>
                            </td>
                            <td width="20%" align="left" valign="middle">
                                Arrival Time
                            </td>
                            <td width="27%" align="left" valign="middle"  colspan="2">
                                <b>{$T.ArrivalTime.Hours}:{$T.ArrivalTime.Minutes} {$T.ArrivalTime.Meridian} {#if $T.ArrivalTime.Days == 2}(next day){#/if}{#if $T.ArrivalTime.Days > 2}day-{$T.ArrivalTime.Days}{#/if}</b>
                            </td>
                        </tr>
                    </tbody>
                </table>
                </div>
                <div id="scrollBusDetails" class="busScroller">
                    <div style="height:1000px">
                    <table width="30%" id="SeatArr-0" class="showHideButtonContainer">
                        <tr>
                            <td width="100%" class="showHideButton"><div style="float:left">Show</div><div style="float:left">&nbsp;seat arrangement</div><div class="showdetails"></div></td>
                        </tr>
                    </table>
                    <div id="s-0" style="display: none;">
                        <div class="loadingData">
                            <div></div>
                        </div>
                        <div id="seatArrContainer">
                        </div>
                    </div>

                    <table width="30%" id="BusSchedule-1" class="showHideButtonContainer">
                        <tr>
                            <td width="100%" class="showHideButton"><div style="float:left">Show</div><div style="float:left">&nbsp;bus schedule</div><div class="showdetails"></div></td>
                        </tr>
                    </table>
                    <div id="s-1"  style="display: none;">
                     <div class="loadingData">
                            <div></div>
                     </div>
                      <div id="busSchContainer">
                            </div>
                   
                    </div>
                    

                    <table width="30%" id="BusStop-2" class="showHideButtonContainer">
                        <tr>
                            <td width="100%" class="showHideButton"><div style="float:left">Show</div><div style="float:left">&nbsp;bus stops</div><div class="showdetails"></div></td>
                        </tr>
                    </table>
                    <div id="s-2"  style="display: none;">
                     <div class="loadingData">
                            <div></div>
                        </div>
                            <div id="cpContainer">
                            </div>
                    </div>
                   

                    <table width="30%" id="BusRate-3" class="showHideButtonContainer">
                        <tr>
                            <td width="100%" class="showHideButton"><div style="float:left">Show</div><div style="float:left">&nbsp;bus rates</div><div class="showdetails"></div></td>
                        </tr>
                    </table>
                    <div id="s-3" style="display: none;">
                        <div class="loadingData">
                            <div></div>
                        </div>
                        <div id="ratesContainer">
                        </div>
                    </div>
                </div>
            </div>
        </textarea>
        <div id="tempDiv"></div>
    </div>
</asp:Content>
