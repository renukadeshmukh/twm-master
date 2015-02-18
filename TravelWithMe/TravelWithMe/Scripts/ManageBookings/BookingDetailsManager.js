var BookingDetailsManager = function () {
    this.TravelDate = '';
    this.IsEditMode = false;
    this.ShowBusDetails = function (bus) {
        $("#detailsContainer").setTemplate(bookingManager.busDetailsTemplate);
        $("#detailsContainer").processTemplate(bus);
        $("#scrollBusDetails").mCustomScrollbar();
        $("#txtTravelDate").datepicker({
            dateFormat: 'M d, yy',
            defaultDate: 7,
            minDate: "0",
            maxDate: "+6M",
            stepMonths: 1,
            numberOfMonths: 1
        });

        $("#txtTravelDate").change(function () {
            bookingManager.BDM.IsEditMode = false;
            bookingManager.Bookings = null;
            $("#liListView").removeClass("active");
            $("#liMapView").addClass("active");
            bookingManager.BDM.TravelDate = $(this).val();
            if (bookingManager.BDM.TravelDate) {
                bookingManager.BDM.ShowSeatMapStatus();
                bookingManager.BDM.GetBusBookings();
                $(".toptabs").show();
            }
        });

        $("#tabMapView").click(function () {
            bookingManager.BDM.IsEditMode = false;
            $("#liListView").removeClass("active");
            $("#liMapView").addClass("active");
            var itin = {
                SeatMap: bookingManager.selectedBusItem.SeatMap,
                BookedSeats: bookingManager.selectedBusItem.BookedSeats,
                SelectedSeats: [],
                BusTripId: bookingManager.selectedBusItem.BusTripId,
                Fare: bookingManager.selectedBusItem.Fare
            };
            reDrawSeats(itin);
        });

        $("#tabListView").click(function () {
            bookingManager.BDM.IsEditMode = false;
            $("#liMapView").removeClass("active");
            $("#liListView").addClass("active");
            $("#dvBookedSeatMap").setTemplate($("#tempateBookingList").val());
            $("#dvBookedSeatMap").processTemplate(bookingManager);
            bookingManager.BindBookingRows(bookingManager.Bookings);
            $("#lnkPrint").click(function () {
                $("#dvBookedSeatMap").printArea();
            });
        });
    };

    $("#detailsContainer").on("click", ".seatmaparea .deck [class*=Seat],.seatmaparea .deck [class*=Berth]", function (e) {
        var busid = this.id.split("_");
        var seatType = $(this).attr("class");
        if (!bookingManager.BDM.IsEditMode) {
            var bookingId = getBookingId(busid[1]);
            bookingManager.BDM.IsEditMode = true;
            var tripInfo = null;
            if (bookingId == 0) {
                bookingManager.BookingInfo = {
                    BookingId: 0,
                    SelectedSeats: [],
                    TotalAmount: 0,
                    Email: '',
                    ContactNumber: '',
                    Passengers: [],
                    PickupPoint: {},
                    DropOffPoint: {}
                };
                tripInfo = {
                    BI: bookingManager.BookingInfo,
                    CountryCode: '+91',
                    PhoneNumber: '',
                    CityPoints: bookingManager.selectedBusItem.CityPoints
                };
                $("#tripInfoContainer").setTemplate($("#templateTripInformation").val());
                $("#tripInfoContainer").processTemplate(tripInfo);
            } else {
                bookingManager.BDM.ShowBooking(bookingId);
                return;
            }
        }
        if (seatType.indexOf("booked") == -1) {
            var action = seatType.indexOf("selected") != -1 ? "available" : "selected";
            if (seatType.indexOf("Seat") != -1) {
                $(this).attr("class", action + "Seat");
            } else if (seatType.indexOf("BerthH") != -1) {
                $(this).attr("class", action + "BerthH");
            } else if (seatType.indexOf("BerthV") != -1) {
                $(this).attr("class", action + "BerthV");
            }
            if (action == "selected") {
                bookingManager.BookingInfo.SelectedSeats.push(busid[1]);
                bookingManager.BookingInfo.TotalAmount = bookingManager.BookingInfo.TotalAmount + parseFloat(busid[2]);
                var paxDetails = {
                    SeatNumber: busid[1]
                };
                bookingManager.BDM.AddPassenger(paxDetails);
            } else {
                removeItem(bookingManager.BookingInfo.SelectedSeats, busid[1]);
                bookingManager.BookingInfo.TotalAmount = bookingManager.BookingInfo.TotalAmount - parseFloat(busid[2]);
                bookingManager.BDM.RemovePassenger(busid[1]);
            }
            $(".seatBookInfo #selectedSeats").html("<b>" + bookingManager.BookingInfo.SelectedSeats.join(",") + "</b>");
            $(".seatBookInfo #totalAmount").html("<b>Rs. " + bookingManager.BookingInfo.TotalAmount + "</b>");
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

    this.ShowSeatMapStatus = function () {
        if (bookingManager.BDM.TravelDate) {
            bookingManager.BookingInfo = null;
            var returnSeatMap = bookingManager.selectedBusItem.SeatMap ? false : true;
            EnableLoading = true;
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/InventoryService.svc/GetSeatMap?session=' + Session.SessionId + '&busId=' + bookingManager.selectedBusItem.BusTripId + '&travelDate=' + bookingManager.BDM.TravelDate + '&returnSeatMap=' + returnSeatMap,
                dataType: "json",
                cache: false,
                success: function (data) {
                    bookingManager.BDM.IsEditMode = false;
                    if (data != null && data.IsSuccess && (bookingManager.selectedBusItem.SeatMap || data.SeatMap)) {
                        if (returnSeatMap) {
                            bookingManager.selectedBusItem.SeatMap = JSON.parse(data.SeatMap);
                            bookingManager.selectedBusItem.CityPoints = data.CityPoints;
                        }
                        bookingManager.selectedBusItem.Fare = data.Fare;
                        bookingManager.selectedBusItem.BookedSeats = data.BookedSeats;
                        var itin = {
                            SeatMap: bookingManager.selectedBusItem.SeatMap,
                            BookedSeats: bookingManager.selectedBusItem.BookedSeats,
                            BusTripId: bookingManager.selectedBusItem.BusTripId,
                            Fare: bookingManager.selectedBusItem.Fare
                        };
                        reDrawSeats(itin);
                    } else {
                        data.ErrorMessage = data.ErrorMessage ? data.ErrorMessage : "SeatMap not set to this bus.";
                        $("#dvBookedSeatMap").setTemplate($("#bookedSeatDetails").val());
                        $("#dvBookedSeatMap").processTemplate({ BusTripId: bookingManager.selectedBusItem.BusTripId, Error: data.ErrorMessage });
                    }
                },
                error: function () {
                    showMessage('Error while getting bus seatmap!!');
                }
            });
        }
    };

    function getBookingId(seatNumber) {
        if (bookingManager.selectedBusItem.BookedSeats) {
            for (var i = 0; i < bookingManager.selectedBusItem.BookedSeats.length; i++) {
                if (bookingManager.selectedBusItem.BookedSeats[i].SeatNumber == seatNumber) {
                    return bookingManager.selectedBusItem.BookedSeats[i].BookingId;
                }
            }
        }
        return 0;
    }

    function getBooking(bookingId) {
        if (bookingId && bookingId > 0) {
            for (var i = 0; i < bookingManager.Bookings.length; i++) {
                if (bookingManager.Bookings[i].BookingId == bookingId) {
                    return JSON.parse(JSON.stringify(bookingManager.Bookings[i]));
                }
            }
        }
        return null;
    }

    this.AddPassenger = function (pax) {
        $("#tempDiv").setTemplate($("#templatePax").val());
        $("#tempDiv").processTemplate(pax);
        $("#tblPassengers").append($("#tempDiv").html());
        $("#tempDiv").html('');
        bookingManager.BDM.AssignValidations();
    };

    this.RemovePassenger = function (seatNumber) {
        $("#trPaxsRow" + seatNumber).remove();
    };

    this.AssignValidations = function () {
        $('#frmBookSeats').validate({
            rules: {
                txtEmailId: {
                    email: true
                },
                txtCountryCode: {
                    required: true
                },
                txtPhoneNumber: {
                    required: true,
                    number: true,
                    minlength: 10,
                    maxlength: 10
                }
            },
            messages: {
                txtEmailId: {
                    email: "Enter valid email id."
                },
                txtCountryCode: {
                    required: "Valid code required."
                },
                txtPhoneNumber: {
                    required: "Valid mobile number required.",
                    number: "Not a valid mobile number.",
                    minlength: "Mobile number should be 10 digit number.",
                    maxlength: "Mobile number should be 10 digit number."
                }
            },
            onfocusout:
                    function (element) {
                        var _this = this;
                        setTimeout(function () {
                            if (!_this.checkable(element) && (element.name in _this.submitted || !_this.optional(element)))
                                _this.element(element);
                            _this = null;
                        }, 250);
                    }
        });
    }; // end of assign validations

    this.BookNow = function () {
        var isvalid = $('#frmBookSeats').valid();
        if (!isvalid) return;
        if (bookingManager.BDM.TravelDate && bookingManager.BookingInfo.SelectedSeats && bookingManager.BookingInfo.SelectedSeats.length > 0 && bookingManager.BookingInfo.TotalAmount) {
            bookingManager.BookingInfo.TravelDate = bookingManager.BDM.TravelDate;
            bookingManager.BookingInfo.Email = $("#txtEmailId").val();
            bookingManager.BookingInfo.SelectedItinerary = { BusTripId: bookingManager.selectedBusItem.BusTripId };
            bookingManager.BookingInfo.ContactNumber = $("#txtCountryCode").val() + ' ' + $("#txtPhoneNumber").val();
            bookingManager.BookingInfo.Passengers = getPaxDetails();
            bookingManager.BookingInfo.PickupPoint = { CPId: $('#cmbBoardingPoint').val(), CPName: $('#cmbBoardingPoint :selected').text() };
            bookingManager.BookingInfo.DropOffPoint = { CPId: $('#cmbAligntingPoint').val(), CPName: $('#cmbAligntingPoint :selected').text() };
            EnableLoading = true;
            $.ajax({
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(bookingManager.BookingInfo),
                url: 'get/InventoryService.svc/BookSeats?session=' + Session.SessionId + '&authId=' + (Session.AuthId ? Session.AuthId : ""),
                dataType: "json",
                cache: false,
                success: function (data) {
                    bookingManager.BDM.IsEditMode = false;
                    var itin = {
                        SeatMap: bookingManager.selectedBusItem.SeatMap,
                        BookedSeats: bookingManager.selectedBusItem.BookedSeats,
                        SelectedSeats: [],
                        BusTripId: bookingManager.selectedBusItem.BusTripId,
                        Fare: bookingManager.selectedBusItem.Fare
                    };
                    var tripInfo = null;
                    if (data != null && data.IsSuccess) {
                        if (!bookingManager.Bookings) {
                            bookingManager.Bookings = [];
                        }
                        tripInfo = {
                            PrintTicket: { BookingId: data.BookingId, BusTripId: bookingManager.selectedBusItem.BusTripId }
                        };
                        for (var i = 0; i < bookingManager.BookingInfo.SelectedSeats.length; i++) {
                            itin.BookedSeats.push({ BookingId: data.BookingId, SeatNumber: bookingManager.BookingInfo.SelectedSeats[i] });
                        }
                        bookingManager.BookingInfo.BookingId = data.BookingId;
                        bookingManager.Bookings.push(bookingManager.BookingInfo);
                    } else {
                        tripInfo = {
                            Error: data.ErrorMessage ? data.ErrorMessage : "Unknown eror!!!"
                        };
                        if (data.BookedSeats) {
                            itin.BookedSeats = data.BookedSeats;
                            bookingManager.selectedBusItem.BookedSeats = data.BookedSeats;
                        }
                    }
                    reDrawSeats(itin);
                    $("#tripInfoContainer").setTemplate($("#templateTripInformation").val());
                    $("#tripInfoContainer").processTemplate(tripInfo);
                    bookingManager.BookingInfo = null;
                },
                error: function () {
                    showMessage('Unknown error please report.');
                }
            });
        } else {
            showMessage("Please select at least one seat!!!");
        }
    };

    function getPaxDetails() {
        var passengers = [];
        $('[id*=trPaxsRow]').each(function () {
            var pax = {
                FirstName: $(this).find("[id*='txtFName']").val(),
                LastName: $(this).find("[id*='txtLName']").val(),
                Age: $(this).find("[id*='txtAge']").val(),
                Gender: $(this).find("[id*='cmbGender']").val()
            };
            pax.FirstName = pax.FirstName ? pax.FirstName : 'abc';
            pax.LastName = pax.LastName ? pax.LastName : 'xyz';
            pax.Age = pax.Age ? pax.Age : 25;
            passengers.push(pax);
        });
        return passengers;
    }

    this.ShowBookingSummary = function (bookingId) {
        if (!bookingManager.BDM.IsEditMode) {
            var seats = getBookedSeats(bookingId);
            for (var s = 0; s < seats.length; s++) {
                changeSeatStatus(seats[s], 'selected');
            }
        }
    };

    this.HideBookingSummary = function (bookingId) {
        if (!bookingManager.BDM.IsEditMode) {
            var seats = getBookedSeats(bookingId);
            for (var s = 0; s < seats.length; s++) {
                changeSeatStatus(seats[s], 'booked');
            }
        }
    };

    function getBookedSeats(bookingId) {
        var seats = [];
        if (bookingManager.selectedBusItem.BookedSeats) {
            for (var i = 0; i < bookingManager.selectedBusItem.BookedSeats.length; i++) {
                if (bookingManager.selectedBusItem.BookedSeats[i].BookingId == bookingId) {
                    seats.push(bookingManager.selectedBusItem.BookedSeats[i].SeatNumber);
                }
            }
        }
        return seats;
    };

    function changeSeatStatus(seatNumber, status) {
        var seatType = $(".deck [id*=" + bookingManager.selectedBusItem.BusTripId + "_" + seatNumber + "_]").attr("class");
        if (seatType.indexOf("Seat") != -1) {
            $(".deck [id*=" + bookingManager.selectedBusItem.BusTripId + "_" + seatNumber + "_]").attr("class", status + "Seat");
        } else if (seatType.indexOf("BerthH") != -1) {
            $(".deck [id*=" + bookingManager.selectedBusItem.BusTripId + "_" + seatNumber + "_]").attr("class", status + "BerthH");
        } else if (seatType.indexOf("BerthV") != -1) {
            $(".deck [id*=" + bookingManager.selectedBusItem.BusTripId + "_" + seatNumber + "_]").attr("class", status + "BerthV");
        }
    }

    this.GetBusBookings = function () {
        if (bookingManager.BDM.TravelDate) {
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/InventoryService.svc/GetBusBookings?session=' + Session.SessionId + '&authId=' + Session.AuthId + '&traveldate=' + bookingManager.BDM.TravelDate + '&busTripId=' + bookingManager.selectedBusItem.BusTripId,
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data.IsSuccess && data.Bookings) {
                        bookingManager.Bookings = data.Bookings;
                    }
                },
                error: function () {
                }
            });
        }
    };

    this.CancelChanges = function () {
        bookingManager.BDM.IsEditMode = false;
        bookingManager.BookingInfo = null;
        var itin = {
            SeatMap: bookingManager.selectedBusItem.SeatMap,
            BookedSeats: bookingManager.selectedBusItem.BookedSeats,
            SelectedSeats: [],
            BusTripId: bookingManager.selectedBusItem.BusTripId,
            Fare: bookingManager.selectedBusItem.Fare
        };
        reDrawSeats(itin);
    };

    this.SaveBooking = function (bookingId) {
        if (bookingId && bookingManager.BookingInfo && $('#frmBookSeats').valid()) {
            if (bookingManager.BDM.TravelDate && bookingManager.BookingInfo.SelectedSeats && bookingManager.BookingInfo.SelectedSeats.length > 0 && bookingManager.BookingInfo.TotalAmount) {
                bookingManager.BookingInfo.Email = $("#txtEmailId").val();
                bookingManager.BookingInfo.ContactNumber = $("#txtCountryCode").val() + ' ' + $("#txtPhoneNumber").val();
                bookingManager.BookingInfo.Passengers = getPaxDetails();
                bookingManager.BookingInfo.PickupPoint = { CPId: $('#cmbBoardingPoint').val(), CPName: $('#cmbBoardingPoint :selected').text() };
                bookingManager.BookingInfo.DropOffPoint = { CPId: $('#cmbAligntingPoint').val(), CPName: $('#cmbAligntingPoint :selected').text() };
                EnableLoading = true;
                $.ajax({
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify(bookingManager.BookingInfo),
                    url: 'get/InventoryService.svc/UpdateBooking?session=' + Session.SessionId + '&authId=' + (Session.AuthId ? Session.AuthId : ""),
                    dataType: "json",
                    cache: false,
                    success: function (data) {
                        bookingManager.BDM.IsEditMode = false;
                        var itin = {
                            SeatMap: bookingManager.selectedBusItem.SeatMap,
                            BookedSeats: bookingManager.selectedBusItem.BookedSeats,
                            SelectedSeats: [],
                            BusTripId: bookingManager.selectedBusItem.BusTripId,
                            Fare: bookingManager.selectedBusItem.Fare
                        };
                        var tripInfo = null;
                        if (data != null && data.IsSuccess) {
                            tripInfo = {
                                PrintTicket: { BookingId: bookingManager.BookingInfo.BookingId, BusTripId: bookingManager.selectedBusItem.BusTripId }
                            };
                            for (var r = 0; r < bookingManager.selectedBusItem.BookedSeats.length; r++) {
                                if (bookingManager.selectedBusItem.BookedSeats[r].BookingId == bookingManager.BookingInfo.BookingId) {
                                    bookingManager.selectedBusItem.BookedSeats.splice(r, 1);
                                }
                            }
                            for (var i = 0; i < bookingManager.BookingInfo.SelectedSeats.length; i++) {
                                bookingManager.selectedBusItem.BookedSeats.push({ BookingId: data.BookingId, SeatNumber: bookingManager.BookingInfo.SelectedSeats[i] });
                            }
                            itin.BookedSeats = bookingManager.selectedBusItem.BookedSeats;
                            for (r = 0; r < bookingManager.Bookings.length; r++) {
                                if (bookingManager.Bookings[r].BookingId == bookingManager.BookingInfo.BookingId) {
                                    bookingManager.Bookings[r] = bookingManager.BookingInfo;
                                }
                            }
                        } else {
                            tripInfo = {
                                Error: data.ErrorMessage ? data.ErrorMessage : "Unknown eror!!!"
                            };
                            if (data.BookedSeats) {
                                itin.BookedSeats = data.BookedSeats;
                                bookingManager.selectedBusItem.BookedSeats = data.BookedSeats;
                            }
                        }
                        reDrawSeats(itin);
                        $("#tripInfoContainer").setTemplate($("#templateTripInformation").val());
                        $("#tripInfoContainer").processTemplate(tripInfo);
                        bookingManager.BookingInfo = null;
                    },
                    error: function () {
                        showMessage('Unknown error please report.');
                    }
                });
            } else {
                showMessage("Please select at least one seat!!!");
            }
        }
    };

    function reDrawSeats(itin) {
        if (itin) {
            $("#dvBookedSeatMap").setTemplate($("#bookedSeatDetails").val());
            $("#dvBookedSeatMap").processTemplate(itin);
            bookingManager.DrawSeatMap("seatMapArea", itin);
            $(".seatmaparea .deck [class*=Seat],.seatmaparea .deck [class*=Berth]").hover(function () {
                var seatNumber = this.id.split('_')[1];
                bookingManager.BDM.ShowBookingSummary(getBookingId(seatNumber));
            }, function () {
                var seatNumber = this.id.split('_')[1];
                bookingManager.BDM.HideBookingSummary(getBookingId(seatNumber));
            });
        }
    }

    this.DeleteBooking = function () {
        if (bookingManager.BookingInfo) {
            EnableLoading = true;
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/InventoryService.svc/DeleteBooking?session=' + Session.SessionId + '&authId=' + (Session.AuthId ? Session.AuthId : "") + '&busTripId=' + bookingManager.selectedBusItem.BusTripId + '&bookingId=' + bookingManager.BookingInfo.BookingId,
                dataType: "json",
                cache: false,
                success: function (data) {
                    bookingManager.BDM.IsEditMode = false;
                    var itin = {
                        SeatMap: bookingManager.selectedBusItem.SeatMap,
                        BookedSeats: bookingManager.selectedBusItem.BookedSeats,
                        SelectedSeats: [],
                        BusTripId: bookingManager.selectedBusItem.BusTripId,
                        Fare: bookingManager.selectedBusItem.Fare
                    };
                    if (data != null && data.IsSuccess) {
                        for (var r = 0; r < bookingManager.selectedBusItem.BookedSeats.length; r++) {
                            if (bookingManager.selectedBusItem.BookedSeats[r].BookingId == bookingManager.BookingInfo.BookingId) {
                                bookingManager.selectedBusItem.BookedSeats.splice(r, 1);
                            }
                        }
                        itin.BookedSeats = bookingManager.selectedBusItem.BookedSeats;
                        for (r = 0; r < bookingManager.Bookings.length; r++) {
                            if (bookingManager.Bookings[r].BookingId == bookingManager.BookingInfo.BookingId) {
                                bookingManager.Bookings.splice(r, 1);
                            }
                        }
                    }
                    reDrawSeats(itin);
                    bookingManager.BookingInfo = null;
                },
                error: function () {
                    showMessage('Unknown error please report.');
                }
            });
        }
    };

    this.EditBooking = function (bookingId) {
        if (bookingId) {

            $("#liListView").removeClass("active");
            $("#liMapView").addClass("active");
            var itin = {
                SeatMap: bookingManager.selectedBusItem.SeatMap,
                BookedSeats: bookingManager.selectedBusItem.BookedSeats,
                SelectedSeats: [],
                BusTripId: bookingManager.selectedBusItem.BusTripId,
                Fare: bookingManager.selectedBusItem.Fare
            };
            reDrawSeats(itin);
            bookingId = parseInt(bookingId);
            bookingManager.BDM.ShowBookingSummary(bookingId);
            bookingManager.BDM.IsEditMode = true;
            bookingManager.BDM.ShowBooking(bookingId);
        }
    };

    this.ShowBooking = function (bookingId) {
        bookingManager.BookingInfo = getBooking(bookingId);
        if (!bookingManager.BookingInfo) {
            bookingManager.BDM.GetBooking(bookingId);
            return;
        }
        if (bookingManager.Bookings) {
            var tripInfo = {
                BI: bookingManager.BookingInfo,
                CountryCode: bookingManager.BookingInfo.ContactNumber.split(' ')[0],
                PhoneNumber: bookingManager.BookingInfo.ContactNumber.split(' ')[1],
                CityPoints: bookingManager.selectedBusItem.CityPoints,
                Email: bookingManager.BookingInfo.Email
            };
            $("#tripInfoContainer").setTemplate($("#templateTripInformation").val());
            $("#tripInfoContainer").processTemplate(tripInfo);
            for (var p = 0; p < bookingManager.BookingInfo.Passengers.length; p++) {
                bookingManager.BookingInfo.Passengers[p].SeatNumber = bookingManager.BookingInfo.SelectedSeats[p];
                bookingManager.BDM.AddPassenger(bookingManager.BookingInfo.Passengers[p]);
            }
            $(".seatBookInfo #selectedSeats").html("<b>" + bookingManager.BookingInfo.SelectedSeats.join(",") + "</b>");
            $(".seatBookInfo #totalAmount").html("<b>Rs. " + bookingManager.BookingInfo.TotalAmount + "</b>");
        }
    };

    this.GetBooking = function (bookingId) {
        if (bookingId) {
            EnableLoading = true;
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/InventoryService.svc/GetBooking?session=' + Session.SessionId + '&authId=' + (Session.AuthId ? Session.AuthId : "") + '&busTripId=' + bookingManager.selectedBusItem.BusTripId + '&bookingId=' + bookingId,
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data.IsSuccess && data.Booking) {
                        if (!bookingManager.Bookings) {
                            bookingManager.Bookings = [];
                        }
                        bookingManager.Bookings.push(data.Booking);
                        bookingManager.BDM.ShowBooking(bookingId);
                    } else {
                        showMessage(data.ErrorMessage ? data.ErrorMessage : 'Unknown error please report.');
                    }
                },
                error: function () {
                    showMessage('Unknown error please report.');
                }
            });
        }
    };
};