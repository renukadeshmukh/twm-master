var invManager = null;
$(document).ready(function () {
    invManager = new InventoryManager();
    var from = getParameterByName("from");
    var to = getParameterByName("to");
    var travelDate = getParameterByName("travelDate");
    invManager.SetPageControls(from, to, travelDate);
    invManager.SearchBuses(from, to, travelDate);
});

var InventoryManager = function () {
    this.Results = [];
    this.CheckoutManager = null;
    this.SeatMapTemplate = null;
    this.SelectedSeats = {};
    this.SelectedBus = null;
    this.SearchParams = {
        TravelDate: '',
        From: '',
        To: ''
    };
    $.get("../Templates/SeatMapTemplate.htm", function (data) {
        invManager.SeatMapTemplate = data;
    });

    this.SetPageControls = function (from, to, travelDate) {
        $("#txtTravelDate").val(travelDate);
        $("#txtFromCity").val(from);
        $("#txtToCity").val(to);
        $("#spnTravelDate").html(travelDate);
        $("#spnFrom").html(from);
        $("#spnTo").html(to);
        invManager.SearchParams.TravelDate = travelDate;
        invManager.SearchParams.From = from;
        invManager.SearchParams.To = to;
    };

    $("#resultsContainer").on("click", "[id*=btnBook]", function (e) {
        var busid = this.id.split("_")[1];
        if (invManager.SelectedSeats[busid] && invManager.SelectedSeats[busid].Price) {
            invManager.SelectedBus = invManager.findBusItinerary(busid);
            var bookingInfo = {
                TravelDate: invManager.SearchParams.TravelDate,
                SelectedItinerary: invManager.SelectedBus,
                SelectedSeats: invManager.SelectedSeats[busid],
                TotalAmount: invManager.SelectedSeats[busid].Price
            };
            bookingInfo.SelectedItinerary.SeatMap = JSON.stringify(bookingInfo.SelectedItinerary.SeatMap);
            EnableLoading = true;
            $.ajax({
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(bookingInfo),
                url: 'get/SessionService.svc/SaveBookingInfo?session=' + Session.SessionId + '&section=Bus',
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data.IsSuccess) {
                        window.location.href = "Checkout.aspx";
                    } else {
                        showMessage(data.ErrorMessage);
                    }
                },
                error: function () {
                    showMessage('Unknown error please report.');
                }
            });
        } else {
            showMessage("Please select at least one seat!!!");
        }
    });

    $("#resultsContainer").on("click", ".seatmaparea .deck [class*=Seat],.seatmaparea .deck [class*=Berth]", function (e) {
        var busid = this.id.split("_");
        var seatType = $(this).attr("class");
        if (!invManager.SelectedSeats[busid[0]]) {
            invManager.SelectedSeats[busid[0]] = [];
            invManager.SelectedSeats[busid[0]].Price = 0;
        }
        if (seatType.indexOf("booked") == -1) {
            var action = seatType.indexOf("selected") != -1 ? "available" : "selected";
            if (action == "selected" && invManager.SelectedSeats[busid[0]].length >= 5) {
                alert("You can select at most 5 seats per booking.");
                return;
            }
            if (seatType.indexOf("Seat") != -1) {
                $(this).attr("class", action + "Seat");
            } else if (seatType.indexOf("BerthH") != -1) {
                $(this).attr("class", action + "BerthH");
            } else if (seatType.indexOf("BerthV") != -1) {
                $(this).attr("class", action + "BerthV");
            }
            if (action == "selected") {
                invManager.SelectedSeats[busid[0]].push(busid[1]);
                invManager.SelectedSeats[busid[0]].Price = invManager.SelectedSeats[busid[0]].Price + parseFloat(busid[2]);
                var paxDetails = {
                    SeatNumber: busid[1]
                };
                if (invManager.CheckoutManager) {
                    invManager.CheckoutManager.AddPassenger(paxDetails);
                }
            } else {
                removeItem(invManager.SelectedSeats[busid[0]], busid[1]);
                invManager.SelectedSeats[busid[0]].Price = invManager.SelectedSeats[busid[0]].Price - parseFloat(busid[2]);
                if (invManager.CheckoutManager) {
                    invManager.CheckoutManager.RemovePassenger(busid[1]);
                }
            }
            $("#tblSeatDetails" + busid[0] + " #selectedSeats").html("<b>" + invManager.SelectedSeats[busid[0]].join(",") + "</b>");
            $("#tblSeatDetails" + busid[0] + " #totalAmount").html("<b>Rs. " + invManager.SelectedSeats[busid[0]].Price + "</b>");
        }
    });

    function removeItem(array, item) {
        for (var i in array) {
            if (array[i] == item) {
                array.splice(i, 1);
                break;
            }
        }
    }

    this.findBusItinerary = function (id) {
        for (var i in invManager.Results) {
            if (invManager.Results[i].BusTripId === id) {
                return invManager.Results[i];
            }
        }
        return null;
    }

    $("#searchBox #txtFromCity").autocomplete({
        source: "/get/ContentService.svc/SearchCity?session=" + Session.SessionId,
        minLength: 2,
        dataType: 'json',
        autoFocus: true
    });

    $("#searchBox #txtToCity").autocomplete({
        source: "/get/ContentService.svc/SearchCity?session=" + Session.SessionId,
        minLength: 2,
        dataType: 'json',
        autoFocus: true
    });
    $("#searchBox #txtTravelDate").datepicker({
        dateFormat: 'M d, yy',
        defaultDate: 7,
        minDate: "0",
        maxDate: "+6M",
        stepMonths: 1,
        numberOfMonths: 1
    });
    $("#btnSearch").click(function () {
        if ($('#frmSearchBox').valid()) {
            invManager.SetPageControls($("#txtFromCity").val(), $("#txtToCity").val(), $("#txtTravelDate").val());
            invManager.SearchBuses(invManager.SearchParams.From, invManager.SearchParams.To, invManager.SearchParams.TravelDate);
        }
    });

    this.SearchBuses = function (from, to, travelDate) {
        if (from && to && travelDate) {
            ShowLoading();
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/InventoryService.svc/SearchBuses?session=' + Session.SessionId + '&traveldate=' + travelDate + '&from=' + from + '&to=' + to,
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data.IsSuccess && data.BusItineraries) {
                        invManager.Results = data.BusItineraries;
                        showResults(invManager.Results);
                    } else {
                        showMessage(data.ErrorMessage);
                    }
                    HideLoading();
                },
                error: function () {
                    HideLoading();
                    showMessage('Error occured while getting results for you!! Please report!');
                }
            });
        } else {
            //            var errorMessage = [];
            //            errorMessage.push(from ? "" : "From city missing.");
            //            errorMessage.push(to ? "" : "To city missing.");
            //            errorMessage.push(travelDate ? "" : "Travel date missing.");
            //            showMessage(errorMessage.join('|'));
        }
    };

    var showResults = function (itins) {
        $("#resultsContainer").setTemplate($("#templateItinerary").val());
        $("#resultsContainer").processTemplate({ Results: itins });
        $("[id*=Itn]").hover(function () {
            if ($(this).css("border-left-width") != "3px") {
                $(this).css("border-left-width", "2px");
                $(this).css("border-left-color", "#b5b1b0");
            }
        }, function () {
            if ($(this).css("border-left-width") != "3px") {
                $(this).css("border-left-width", "1px");
                $(this).css("border-left-color", "#e5e5e5");
            }
        });
    };

    this.ShowSeats = function (busTripId) {
        var itn = GetItinerary(busTripId, invManager.Results);
        $("#show" + busTripId).hide();
        $("#hide" + busTripId).show();
        $("#Itn" + busTripId + " td").css("background", "#eeeff1");
        $("#Itn" + busTripId).css("border-left-width", "3px");
        $("#Itn" + busTripId).css("border-left-color", "#b5b1b0");
        $("#SeatDetails" + busTripId).slideToggle();
        $("#SeatDetails" + busTripId + " .loadingData").show();
        if (invManager.SelectedSeats) {
            invManager.SelectedSeats[busTripId] = [];
            invManager.SelectedSeats[busTripId].Price = 0;
        }
        if (itn) {
            var returnSeatMap = itn.SeatMap ? false : true;
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/InventoryService.svc/GetSeatMap?session=' + Session.SessionId + '&busId=' + busTripId + '&travelDate=' + invManager.SearchParams.TravelDate + '&returnSeatMap=' + returnSeatMap,
                dataType: "json",
                cache: false,
                success: function (data) {
                    $("#SeatDetails" + busTripId + " .loadingData").hide();
                    if (data != null && data.IsSuccess) {
                        if (returnSeatMap)
                            itn.SeatMap = JSON.parse(data.SeatMap);
                        itn.BookedSeats = data.BookedSeats;
                        $("#seatMapArea" + busTripId).setTemplate(invManager.SeatMapTemplate);
                        $("#seatMapArea" + busTripId).processTemplate(itn);
                        invManager.DrawSeatMap("seatMapArea", itn);
                    } else {
                        $("#SeatDetails" + busTripId + " .loadingData").hide();
                        showMessage(data.ErrorMessage);
                    }
                },
                error: function () {
                    $("#SeatDetails" + busTripId + " .loadingData").hide();
                    showMessage('Error while getting bus seatmap!!');
                }
            });
        }
    };



    this.HideSeats = function (busTripId) {
        $("#Itn" + busTripId).css("border-left-width", "1px");
        $("#Itn" + busTripId).css("border-left-color", "#e5e5e5");
        $("#show" + busTripId).show();
        $("#hide" + busTripId).hide();
        $("#Itn" + busTripId + " td").css("background", "#fff");
        $("#SeatDetails" + busTripId).slideToggle();
    };

    var GetItinerary = function (id, list) {
        if (list && id) {
            for (var idx = 0; idx < list.length; idx++) {
                if (list[idx].BusTripId == id) {
                    return list[idx];
                }
            }
        }
        return null;
    };

    this.DrawSeatMap = function (container, itin) {
        var seatmap = itin.SeatMap;
        var bookedSeats = itin.BookedSeats ? itin.BookedSeats : [];
        var selectedSeats = itin.SelectedSeats ? itin.SelectedSeats : [];
        var busId = itin.BusTripId;
        container = container + busId;
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
                    var seatDiv = "<div id='" + busId + "_" + seat.SeatNumber + "_" + itin.Fare + "' class='" + seatType + "' style='top: " + y + "px; left: " + x + "px; z-index:2' title='" + seat.SeatNumber + "'></div>";
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