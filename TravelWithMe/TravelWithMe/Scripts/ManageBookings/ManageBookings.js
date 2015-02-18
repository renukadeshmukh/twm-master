var bookingManager = null;
$(document).ready(function () {
    bookingManager = new BookingManager();
    bookingManager.GetBuses();
});

var BookingManager = function () {
    var timeout = null;
    this.busShortInfoTemplate = null;
    this.busDetailsTemplate = null;
    this.busList = null;
    this.selectedBusItem = null;
    this.Bookings = null;
    this.BookingInfo = null;
    this.busShortInfoTemplate = $("#templateBusShortInfo").val();
    this.busDetailsTemplate = $("#templateBusDetails").val();
    this.BDM = new BookingDetailsManager();
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
                    bookingManager.busList = data;
                    bookingManager.ShowBusList(data);
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
        $("#buslistContainer").setTemplate(bookingManager.busShortInfoTemplate);
        $("#buslistContainer").processTemplate(bookingManager.busList);
        $("[id*=busItem]").hover(function (element) {
            var id = element.currentTarget.id;
            if (id != 'busItem' + bookingManager.selectedBusItem.BusTripId) {
                $("#" + id + " tr td").css("background-color", "#e7ebf3");
            }
        }, function (element) {
            var id = element.currentTarget.id;
            if (id != 'busItem' + bookingManager.selectedBusItem.BusTripId) {
                $("#" + id + " tr td").css("background-color", "white");
            }
        });
        $("[id*=busItem]").click(function (element) {
            var idParts = element.currentTarget.id.split("busItem");
            bookingManager.SelectBus(idParts[1]);
        });
        bookingManager.SelectBus(bookingManager.busList.Buses[0].BusTripId);
        $("#buslist").mCustomScrollbar();
    };

    this.SelectBus = function (busTripId) {
        bookingManager.BDM.IsEditMode = false;
        bookingManager.Bookings = null;
        bookingManager.BDM.TravelDate = null;
        bookingManager.BookingInfo = null;
        var lastSelectedBusItem = bookingManager.selectedBusItem;
        for (var i = 0; i < bookingManager.busList.Buses.length; i++) {
            if (bookingManager.busList.Buses[i].BusTripId == busTripId) {
                bookingManager.selectedBusItem = bookingManager.busList.Buses[i];
                bookingManager.BDM.ShowBusDetails(bookingManager.selectedBusItem);
                break;
            }
        }
        if (lastSelectedBusItem != null) {
            var lsItemId = 'busItem' + lastSelectedBusItem.BusTripId;
            $("#" + lsItemId + " tr td").css("background-color", "white");
            $("#" + lsItemId).css("border-left-width", "1px");
            $("#" + lsItemId).css("border-color", "#e5e5e5");
        }
        var sItemId = 'busItem' + bookingManager.selectedBusItem.BusTripId;
        $("#" + sItemId + " tr td").css("background-color", "#e7ebf3");
        $("#" + sItemId).css("border-left-width", "5px");
        $("#" + sItemId).css("border-color", "#bab7b7");
    };

    this.DrawSeatMap = function (container, itin) {
        var seatmap = itin.SeatMap;
        var bookedSeats = itin.BookedSeats ? itin.BookedSeats : [];
        var selectedSeats = itin.SelectedSeats ? itin.SelectedSeats : [];
        var busId = itin.BusTripId;
        if (seatmap) {
            $("#" + container + " .deck [class*=Seat], #" + container + " .deck [class*=Berth]").remove();
            if (seatmap.Name != null && seatmap.Name != '' && seatmap.Name != undefined) {
                $("#" + container + " .seatmapheader div :first").html('<b>Seat map name:</b> ' + seatmap.Name);
            } else {
                $("#" + container + " .seatmapheader div :first").html('');
            }

            if (seatmap.Decks.length == 2) {
                $("#" + container + " .deck :last").show();
            } else {
                $("#" + container + " .deck :last").hide();
            }
            var y = 5;
            var x = 70;
            var stepY1 = [25, 30, 25, 0];
            var stepY2 = [55, 0, 0, 0];
            var stepX = 25;
            for (var dk = 0; dk < seatmap.Decks.length; dk++) {
                x = 70;
                y = 5;
                for (var i = 0; i < seatmap.Decks[dk].Seats.length; i++) {
                    var seat = seatmap.Decks[dk].Seats[i];
                    var seatType = seat.SeatType;
                    for (var b = 0; b < bookedSeats.length; b++) {
                        if (bookedSeats[b].SeatNumber == seat.SeatNumber) {
                            if (seatType.indexOf('BerthH') != -1) {
                                seatType = "bookedBerthH";
                            } else if (seatType.indexOf('BerthV') != -1) {
                                seatType = "bookedBerthV";
                            } else {
                                seatType = "bookedSeat";
                            }
                        }
                    }
                    for (var s = 0; s < selectedSeats.length; s++) {
                        if (selectedSeats[s] == seat.SeatNumber) {
                            if (seatType.indexOf('BerthH') != -1) {
                                seatType = "selectedBerthH";
                            } else if (seatType.indexOf('BerthV') != -1) {
                                seatType = "selectedBerthV";
                            } else {
                                seatType = "selectedSeat";
                            }
                        }
                    }
                    var seatDiv = "<div id='" + busId + "_" + seat.SeatNumber + "_" + itin.Fare + "' class='" + seatType + "' style='top: " + y + "px; left: " + x + "px; z-index:1' title='" + seat.SeatNumber + "'></div>";
                    $($("#" + container + " .deck")[dk]).append(seatDiv);
                    stepX = seat.SeatType.indexOf('BerthH') != -1 ? 45 : 25;
                    y = seat.SeatType.indexOf('BerthV') != -1 ? (y + stepY2[seat.Row]) : (y + stepY1[seat.Row]);
                    if (seat.SeatType.indexOf('BerthV') == -1 && seatmap.Decks[dk].Size == '2X1' && seat.Row == 1) {
                        y = y + stepY1[seat.Row];
                    }
                    if (seat.Row == 3) {
                        y = 5;
                        x = x + stepX;
                    }
                }
            }
        }
    };

    this.SearchBookings = function () {
        var searchRequestFields = ['BookingId', 'ContactNumber', 'Email'];
        var searchPaxFields = ['FirstName', 'LastName'];
        var searchText = $('#txtSearchBookings').val();
        if (!searchText) {
            bookingManager.BindBookingRows(bookingManager.Bookings);
            return;
        }
        var filterdResults = [];
        var searchTerms = searchText.split(" ");
        for (var i in bookingManager.Bookings)           //Go through every item in the array
        {
            var item = bookingManager.Bookings[i];
            var matches = false;     //Does this till we meet our criterium?
            for (var fldIndex = 0; fldIndex < searchRequestFields.length; fldIndex++)    //Match all the requirements
            {
                var fieldName = searchRequestFields[fldIndex];
                for (var termIndex = 0; termIndex < searchTerms.length; termIndex++) {
                    if (searchTerms[termIndex] == "") continue;
                    matches = new RegExp(searchTerms[termIndex], 'i').test(item[fieldName]);
                    if (matches) break;
                }
                if (matches) break;
            }
            if (!matches) { // match the search term with the passenger fields.
                for (var paxCnt = 0; paxCnt < item.Passengers.length; paxCnt++) {
                    var pax = item.Passengers[paxCnt];
                    for (var paxFldIndex = 0; paxFldIndex < searchPaxFields.length; paxFldIndex++)    //Match all the requirements
                    {
                        var paxFieldName = searchPaxFields[paxFldIndex];
                        for (var termI = 0; termI < searchTerms.length; termI++) {
                            if (searchTerms[termI] == "") continue;
                            matches = new RegExp(searchTerms[termI], 'i').test(pax[paxFieldName]);
                            if (matches) break;
                        }
                        if (matches) break;
                    }
                    if (matches) break;
                }
            }
            if (matches)
                filterdResults.push(item);  //Add the item to the result
        }
        bookingManager.BindBookingRows(filterdResults);
    };

    this.CreateSearchTerm = function () {
        if (timeout) {
            clearTimeout(timeout);
        }

        timeout = setTimeout(function () {
            bookingManager.SearchBookings();
        }, 500);
    };

    this.BindBookingRows = function (bookings) {
        if (bookings) {
            $("[id*=BookingRow_]").remove();
            $("#tempDiv").setTemplate($("#templateBookingRow").val());
            $("#tempDiv").processTemplate({ Bookings: bookings });
            $("#tblBookings").append($("#tempDiv").html());
            $("#tempDiv").html('');
            $("#tblBookings tr").hover(function () {
                $("td", this).css("background-color", "#ebeef3");
            }, function () {
                $("td", this).css("background-color", "white");
            });
        }
    };
};