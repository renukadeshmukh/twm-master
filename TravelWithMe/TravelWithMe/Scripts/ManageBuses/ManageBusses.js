var busManager = null;
$(document).ready(function () {
    busManager = new BusManager();
    busManager.GetBuses();
    busManager.SAM.GetDefaultSeatMaps();
});

var BusManager = function () {
    this.busShortInfoTemplate = null;
    this.busDetailsTemplate = null;
    this.busList = null;
    this.selectedBusItem = null;
    this.editBusTemplate = null;
    this.updateBusScheduleTemplate = null;
    this.DateRangeTemplate = null;
    this.SAM = new SeatArrengementManager();
    this.BR = new RatesManager();
    this.CP = new CityPointManager();
    this.BS = new BusScheduleManager();
    this.DM = new BusDetailsManager();
    this.busShortInfoTemplate = $("#templateBusShortInfo").val();
    this.busDetailsTemplate = $("#templateBusDetails").val();

    $.get("../Templates/AddUpdateBusDetailsDialog.htm", function (data) {
        busManager.editBusTemplate = data;
    });

    $.get("../Templates/DateRangeTemplate.htm", function (data) {
        busManager.DateRangeTemplate = data;
    });


    this.GetBuses = function () {
        EnableLoading = true;
        $.ajax({
            type: "GET",
            contentType: "application/json",
            url: 'get/BusService.svc/bus/getall/' + Session.AuthId + '/' + Session.SessionId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess && data.Buses != null && data.Buses != undefined) {
                    busManager.busList = data;
                    busManager.ShowBusList(data);
                } else {
                    showMessage(data.ErrorMessage);
                }
            },
            error: function () {
                showMessage('Error while getting your bus list!! Please report!');
            }
        });
    };

    this.ShowBusList = function () {
        $("#buslistContainer").setTemplate(busManager.busShortInfoTemplate);
        $("#buslistContainer").processTemplate(busManager.busList);
        $("[id*=busItem]").hover(function (element) {
            var id = element.currentTarget.id;
            if (id != 'busItem' + busManager.selectedBusItem.BusTripId) {
                $("#" + id + " tr td").css("background-color", "#e7ebf3");
            }
        }, function (element) {
            var id = element.currentTarget.id;
            if (id != 'busItem' + busManager.selectedBusItem.BusTripId) {
                $("#" + id + " tr td").css("background-color", "white");
            }
        });
        $("[id*=busItem]").click(function (element) {
            var idParts = element.currentTarget.id.split("busItem");
            busManager.SelectBus(idParts[1]);
        });
        busManager.SelectBus(busManager.busList.Buses[0].BusTripId);
        $("#buslist").mCustomScrollbar();
    };

    this.SelectBus = function (busTripId) {
        var lastSelectedBusItem = busManager.selectedBusItem;
        for (var i = 0; i < busManager.busList.Buses.length; i++) {
            if (busManager.busList.Buses[i].BusTripId == busTripId) {
                busManager.selectedBusItem = busManager.busList.Buses[i];
                new BusDetailsManager().ShowBusDetails(busManager.selectedBusItem);
                break;
            }
        }
        if (lastSelectedBusItem != null) {
            var lsItemId = 'busItem' + lastSelectedBusItem.BusTripId;
            $("#" + lsItemId + " tr td").css("background-color", "white");
            $("#" + lsItemId).css("border-left-width", "1px");
            $("#" + lsItemId).css("border-color", "#e5e5e5");
        }
        var sItemId = 'busItem' + busManager.selectedBusItem.BusTripId;
        $("#" + sItemId + " tr td").css("background-color", "#e7ebf3");
        $("#" + sItemId).css("border-left-width", "5px");
        $("#" + sItemId).css("border-color", "#bab7b7");
    };

    
};