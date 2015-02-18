var reportManager = null;
$(document).ready(function () {
    reportManager = new ReportManager();
    reportManager.GetBookings();
    reportManager.DisplayBuses();
});

var ReportManager = function () {
    this.SeatMapTemplate = null;
    this.BookingInfoPopUpTemplate = null;
    this.Bookings = null;
    this.BusList = null;

    // AutoComplete event for Search booking Textbox 
    $("#frmSearchBox #txtSearch").autocomplete({
        source: function (request, response) {
            var passengers = reportManager.GetPassengersDetailsForBustripId(1);
            var filteredList = [];
            for (var p in passengers)           //Go through every item in the array
            {
                var matches = false;     //Does this meet our criterium?
                matches = new RegExp(request.term, 'i').test(passengers[p]);
                if (matches) {
                    filteredList.push(passengers[p]);
                }
            }
            response(filteredList);
        },
        minLength: 2,
        dataType: 'json',
        autoFocus: true,
        id: "searchAutoComplete"
    });

    $("#txtSearch").change(function () {
        document.title = this.value;
    });

    // Ajax call to get the bookings for a AuthId
    this.GetBookings = function () {
        $.ajax({
            type: "GET",
            contentType: "application/json",
            url: "get/BookingService.svc/booking/getall/" + Session.AuthId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null)
                    reportManager.Bookings = data;
            },
            error: function () {
                showMessage('Error while getting user requests!!');
            }
        });
    };

    // Ajax call to get the buses for a AuthId
    this.DisplayBuses = function () {
        $.ajax({
            type: "GET",
            contentType: "application/json",
            url: 'get/BusService.svc/bus/getall/' + Session.AuthId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess && data.Buses != null && data.Buses != undefined) {
                    reportManager.BusList = data.Buses;
                    reportManager.ShowBusList(data.Buses);
                    reportManager.AddClickEventForSeatArrangementForBuses(data.Buses);
                } else {
                    showMessage(data.ErrorMessage);
                }
            },
            error: function () {
                showMessage('Error while getting user requests!!');
            }
        });
    };

    this.SetSeatMap = function (busTripId) {
        $.ajax({
            type: "GET",
            contentType: "application/json",
            url: 'get/BusService.svc/bus/GetSeatMap/' + Session.AuthId + '/' + busTripId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null) {
                    $("SeatDetails").hide();
                    $("#SeatDetails" + " .loadingData").show();
                    $("#seatMapArea").setTemplate(reportManager.SeatMapTemplate);
                    var seatMap = JSON.parse(data.SeatMap);
                    var itinerary = {
                        "BusTripId": busTripId,
                        "SeatMap": seatMap
                    };
                    $("#seatMapArea").processTemplate({ "Itinerary": itinerary });
                    var bookedSeats = reportManager.GetBookedSeatsForBusTripId(busTripId);
                    reportManager.DrawSeatMap("seatMapArea", busTripId, seatMap, bookedSeats);
                    $("#SeatDetails" + " .loadingData").hide();
                    $("SeatDetails").show();

                    //Attach the BookingDetails popup to all the seats
                    $(".deck [class*=Seat], .deck [class*=Berth]").hover(function (element) {
                        var id = this.id.split('_');
                        var seatPopupDetails = reportManager.GetSeatPopupDetailsForBustripIdSeatNumber(id[0], id[1]);
                        if (seatPopupDetails != null) {
                            $("#popUpContainer").processTemplate({ "seatPopupDetails": seatPopupDetails });
                            $("#popupDialog").css("top", element.pageY);
                            $("#popupDialog").css("left", element.pageX);
                            $("#popupDialog").show();
                        }
                    },
                    function () {
                        $("#popupDialog").hide();
                    });

                    //Attach the click event to all seats to display the itinerary information 
                    $(".deck [class*=Seat], .deck [class*=Berth]").click(function (element) {
                        var id = this.id.split('_');
                        var booking = reportManager.GetBookingForBustripIdSeatNumber(id[0], id[1]);
                        $("#itineraryContainer").setTemplate($("#templateItinerary").val());
                        $("#itineraryContainer").processTemplate(booking);
                    });

                    //Attach click event to seats so that all the seats in a particular clicked booking are highlighted 
                    $(".deck [class*=Seat], .deck [class*=Berth]").click(function (element) {
                        var bookedSeats = reportManager.GetBookedSeatsForBusTripId(busTripId);
                        reportManager.RepopulateSeats(busTripId, seatMap, bookedSeats);
                        var id = this.id.split('_');
                        var seatPopupDetails = reportManager.GetSeatPopupDetailsForBustripIdSeatNumber(id[0], id[1]);
                        $("#containerPax").setTemplate($("#templatePax").val());
                        $("#containerPax").processTemplate(seatPopupDetails);
                        var bookedSeatList = seatPopupDetails.BookedSeats.split(',');
                        if (bookedSeatList != null && bookedSeatList.length > 0) {
                            for (var i = 0; i < bookedSeatList.length; i++) {
                                var seat = $("#" + id[0] + "_" + bookedSeatList[i]);
                                if (seat.hasClass('bookedSeat'))
                                    seat.attr('class', "selectedSeat");
                                else if (seat.hasClass('bookedBerthH'))
                                    seat.attr('class', "selectedBerthH");
                                else if (seat.hasClass('bookedBerthV'))
                                    seat.attr('class', "selectedBerthV");
                            }
                        }
                        $("#btnEditPassengerDetails").click(function () {
                            if (this.innerHTML == 'Done') {
                            $("#")
                            }
                            else {
                                $("span.gvShow").attr("class", "gvHide");
                                $("input.gvHide").attr("class", "gvShow");
                                this.innerHTML = "Done";
                            }
                        });
                    });




                } else {
                    showMessage('Error while getting seatmap!!');
                }
            },
            error: function () {
                showMessage('Error while getting user seatmap!!');
            }
        });
    };

    this.RepopulateSeats = function (busTripId, seatmap, bookedSeats) {
        for (var dk = 0; dk < seatmap.Decks.length; dk++) {
            for (var i = 0; i < seatmap.Decks[dk].Seats.length; i++) {
                var seat = seatmap.Decks[dk].Seats[i];
                var seatType = seat.SeatType;

                for (var b = 0; b < bookedSeats.length; b++) {
                    if (bookedSeats[b] == seat.SeatNumber) {
                        if (seatType.indexOf('BerthH') != -1) {
                            seatType = "bookedBerthH";
                        } else if (seatType.indexOf('BerthV') != -1) {
                            seatType = "bookedBerthV";
                        } else {
                            seatType = "bookedSeat";
                        }
                        $("#" + busTripId + "_" + seat.SeatNumber).attr('class', seatType);
                    }
                }
            }
        }
    };

    this.GetPassengersDetailsForBustripId = function (busTripId) {
        var passengers = [];
        if (reportManager.Bookings != null && reportManager.Bookings.length > 0) {
            for (var i = 0; i < reportManager.Bookings.length; i++) {
                var selectedItinerary = reportManager.Bookings[i].SelectedItinerary;
                if (selectedItinerary.BusTripId == busTripId && reportManager.Bookings[i].Passengers != null && reportManager.Bookings[i].Passengers.length > 0) {
                    for (var p = 0; p < reportManager.Bookings[i].Passengers.length; p++) {
                        var passenger = reportManager.Bookings[i].Passengers[p];
                        passengers.push(passenger.FirstName + ' ' + passenger.MiddleName + ' ' + passenger.LastName);
                    }
                }
            }
        }


        return passengers;
    };

    this.GetSeatPopupDetailsForBustripIdSeatNumber = function (busTripId, seatNumber) {
        if (reportManager.Bookings != null && reportManager.Bookings.length > 0) {
            for (var i = 0; i < reportManager.Bookings.length; i++) {
                var selectedItinerary = reportManager.Bookings[i].SelectedItinerary;
                if (selectedItinerary.BusTripId == busTripId && selectedItinerary.BookedSeats != null && selectedItinerary.BookedSeats.length > 0 && reportManager.IfArrayContains(selectedItinerary.BookedSeats, seatNumber)) {
                    {
                        var strBookedSeats = "";
                        for (var b = 0; b < selectedItinerary.BookedSeats.length; b++) {
                            strBookedSeats += selectedItinerary.BookedSeats[b] + ",";
                        }
                        var seatpopupDetails = { "BookingId": reportManager.Bookings[i].BookingId, "BookedSeats": strBookedSeats, "Passengers": reportManager.Bookings[i].Passengers };
                        return seatpopupDetails;
                    }
                }
            }
        }
        return null;
    };

    this.GetBookingForBustripIdSeatNumber = function (busTripId, seatNumber) {
        if (reportManager.Bookings != null && reportManager.Bookings.length > 0) {
            for (var i = 0; i < reportManager.Bookings.length; i++) {
                var selectedItinerary = reportManager.Bookings[i].SelectedItinerary;
                if (selectedItinerary.BusTripId == busTripId && selectedItinerary.BookedSeats != null && selectedItinerary.BookedSeats.length > 0 && reportManager.IfArrayContains(selectedItinerary.BookedSeats, seatNumber)) {
                    {
                        return reportManager.Bookings[i];
                    }
                }
            }
        }
        return null;
    };

    this.IfArrayContains = function (array, value) {
        if (array != null && array.length > 0) {
            for (var i = 0; i < array.length; i++) {
                if (array[i] == value)
                    return true;
            }
        }
        return false;
    };

    $.get("../Templates/BookingReportTemplate.htm", function (data) {
        reportManager.SeatMapTemplate = data;
        $("#seatMapArea").setTemplate(reportManager.SeatMapTemplate);

    });

    $.get("../Templates/BookingInfoPopUp.htm", function (data) {
        reportManager.BookingInfoPopUpTemplate = data;
        $("#popUpContainer").setTemplate(reportManager.BookingInfoPopUpTemplate);

    });

    this.ShowBusList = function () {
        $("#displayBasicBusInfo").setTemplate($("#templatebasicBusInfo").val());
        $("#displayBasicBusInfo").processTemplate({ BusList: reportManager.BusList });
        $("#displayBasicBusInfo").mCustomScrollbar();
    };

    this.AddClickEventForSeatArrangementForBuses = function (buses) {
        for (var i = 0; i < buses.length; i++) {
            $("#busDetails_" + buses[i].BusTripId).click(function () {
                var id = this.id.split('_');
                $("#SeatDetails" + " .loadingData").show();

                var bus = reportManager.GetBusByTripId(id[1]);
                reportManager.SetSeatMap(bus.BusTripId);
                $('#trSearch').show();

            });
        }
    };

    this.GetBookingById = function (bookingId) {
        if (reportManager.Bookings != null && reportManager.Bookings.length > 0) {
            for (var i = 0; i < reportManager.Bookings.length; i++) {
                if (reportManager.Bookings[i].BookingId == bookingId)
                    return reportManager.Bookings[i];
            }
        }
        return null;
    };

    this.GetBusByTripId = function (busTripid) {
        if (reportManager.BusList != null && reportManager.BusList.length > 0) {
            for (var i = 0; i < reportManager.BusList.length; i++) {
                if (reportManager.BusList[i].BusTripId == busTripid)
                    return reportManager.BusList[i];
            }
        }
        return null;
    };

    this.GetBookedSeatsForBusTripId = function (busTripId) {
        var bookedSeatIds = new Array();
        if (reportManager.Bookings != null && reportManager.Bookings.length > 0) {
            for (var i = 0; i < reportManager.Bookings.length; i++) {
                if (reportManager.Bookings[i].SelectedItinerary.BusTripId == busTripId) {
                    if (reportManager.Bookings[i].SelectedItinerary.BookedSeats != null && reportManager.Bookings[i].SelectedItinerary.BookedSeats.length > 0) {
                        for (var j = 0; j < reportManager.Bookings[i].SelectedItinerary.BookedSeats.length; j++) {
                            bookedSeatIds.push(reportManager.Bookings[i].SelectedItinerary.BookedSeats[j]);
                        }
                    }
                }
            }
        }
        return bookedSeatIds;
    };
    $('.datePicker').datepicker({
        dateFormat: 'M d, yy',
        defaultDate: 7,
        minDate: "0",
        maxDate: "+6M",
        stepMonths: 1,
        numberOfMonths: 1
    });

    this.DrawSeatMap = function (container, busTripId, seatmap, bookedSeats) {
        var id = 1;
        var fare = 1;
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
                        if (bookedSeats[b] == seat.SeatNumber) {
                            if (seatType.indexOf('BerthH') != -1) {
                                seatType = "bookedBerthH";
                            } else if (seatType.indexOf('BerthV') != -1) {
                                seatType = "bookedBerthV";
                            } else {
                                seatType = "bookedSeat";
                            }
                        }
                    }
                    var seatDiv = "<div id='" + busTripId + "_" + seat.SeatNumber + "' class='" + seatType + "' style='top: " + y + "px; left: " + x + "px; z-index:2' title='" + seat.SeatNumber + "'></div>";
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
};

