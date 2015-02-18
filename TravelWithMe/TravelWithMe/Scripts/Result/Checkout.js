var checkoutMgr = null;
$(document).ready(function () {
    checkoutMgr = new CheckoutManager();
    invManager.CheckoutManager = checkoutMgr;
    checkoutMgr.GetBookingInfo();
});

var CheckoutManager = function () {
    this.BookingInfo = null;

    this.GetBookingInfo = function () {
        if (checkoutMgr.BookingInfo) {
            checkoutMgr.RenderBookingInfo();
        } else {
            EnableLoading = true;
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/SessionService.svc/GetBookingInfo?session=' + Session.SessionId,
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data.IsSuccess) {
                        checkoutMgr.BookingInfo = data.BookingInfo;
                        checkoutMgr.BookingInfo.BookedSeats = data.BookedSeats;
                        checkoutMgr.BookingInfo.SelectedItinerary.SeatMap = JSON.parse(checkoutMgr.BookingInfo.SelectedItinerary.SeatMap);
                        checkoutMgr.RenderBookingInfo();
                    } else {
                        showMessage(data.ErrorMessage ? data.ErrorMessage : "Unknown Error");
                    }
                },
                error: function () {
                    showMessage('Unknown error please report.');
                }
            });
        }
    };

    this.RenderBookingInfo = function () {
        var itn = checkoutMgr.BookingInfo.SelectedItinerary;
        itn.SelectedSeats = checkoutMgr.BookingInfo.SelectedSeats;
        itn.BookedSeats = checkoutMgr.BookingInfo.BookedSeats ? checkoutMgr.BookingInfo.BookedSeats : [];
        $("#resultsContainer").setTemplate($("#templateItinerary").val());
        $("#resultsContainer").processTemplate({ Results: [itn] });

        if (checkoutMgr.BookingInfo.ContactNumber) {
            var cn = checkoutMgr.BookingInfo.ContactNumber.split(' ');
            itn.CountryCode = cn[0];
            itn.PhoneNumber = cn[1];
        }
        itn.Email = checkoutMgr.BookingInfo.Email;
        itn.PickupPoint = checkoutMgr.BookingInfo.PickupPoint;
        itn.DropOffPoint = checkoutMgr.BookingInfo.DropOffPoint;
        $("#seatMapArea" + itn.BusTripId).setTemplate($("#templateSeatMap").val());
        $("#seatMapArea" + itn.BusTripId).processTemplate(itn);
        if (!checkoutMgr.BookingInfo.ContactNumber && Session) {
            $("#txtEmailId").val(Session.Email);
            if (Session.PhoneNumber) {
                var pnumber = Session.PhoneNumber.split(' ');
                $("#txtCountryCode").val(pnumber[0]);
                $("#txtPhoneNumber").val(pnumber[1] ? pnumber[1] : '');
            } else {
                $("#txtCountryCode").val('+91');
            }
        }

        invManager.SelectedSeats[itn.BusTripId] = [];
        invManager.SelectedSeats[itn.BusTripId].Price = 0;
        for (var i = 0; i < itn.SelectedSeats.length; i++) {
            invManager.SelectedSeats[itn.BusTripId].push(itn.SelectedSeats[i]);
            invManager.SelectedSeats[itn.BusTripId].Price = invManager.SelectedSeats[itn.BusTripId].Price + checkoutMgr.BookingInfo.SelectedItinerary.Fare;
        }
        checkoutMgr.BookingInfo.TotalAmount = invManager.SelectedSeats[itn.BusTripId].Price;
        var bookingSummary = {
            BusTripId: itn.BusTripId,
            From: itn.From,
            To: itn.To,
            TravelDate: checkoutMgr.BookingInfo.TravelDate,
            SelectedSeats: invManager.SelectedSeats[itn.BusTripId].join(","),
            TotalAmount: invManager.SelectedSeats[itn.BusTripId].Price
        };
        $("#bookingSummary").setTemplate($("#templateBookingSummary").val());
        $("#bookingSummary").processTemplate(bookingSummary);
        invManager.DrawSeatMap("seatMapArea", itn);

        for (var px = 0; px < invManager.SelectedSeats[itn.BusTripId].length; px++) {
            var paxList = checkoutMgr.BookingInfo.Passengers;
            var paxDetails = {
                SeatNumber: invManager.SelectedSeats[itn.BusTripId][px],
                FirstName: paxList && paxList[px] ? paxList[px].FirstName : '',
                LastName: paxList && paxList[px] ? paxList[px].LastName : '',
                Age: paxList && paxList[px] ? paxList[px].Age : '',
                Gender: paxList && paxList[px] ? paxList[px].Gender : ''
            };
            checkoutMgr.AddPassenger(paxDetails);
        }
        checkoutMgr.AssignValidations();
    };

    this.AddPassenger = function (pax) {
        $("#tempDiv").setTemplate($("#templatePax").val());
        $("#tempDiv").processTemplate(pax);
        $("#tblPassengers").append($("#tempDiv").html());
        $("#tempDiv").html('');
        checkoutMgr.AssignValidations();
    };

    this.RemovePassenger = function (seatNumber) {
        $("#trPaxsRow" + seatNumber).remove();
    };

    this.PayNow = function (busid) {
        var isvalid = $('#frmCheckOut').valid();
        if (!isvalid) return;
        if (busid && invManager.SelectedSeats[busid] && invManager.SelectedSeats[busid].length > 0 && invManager.SelectedSeats[busid].Price) {
            checkoutMgr.BookingInfo.SelectedSeats = invManager.SelectedSeats[busid];
            checkoutMgr.BookingInfo.TotalAmount = invManager.SelectedSeats[busid].Price;
            var tempitin = checkoutMgr.BookingInfo.SelectedItinerary;
            checkoutMgr.BookingInfo.SelectedItinerary = { BusTripId: checkoutMgr.BookingInfo.SelectedItinerary.BusTripId };
            checkoutMgr.BookingInfo.Email = $("#txtEmailId").val();
            checkoutMgr.BookingInfo.ContactNumber = $("#txtCountryCode").val() + ' ' + $("#txtPhoneNumber").val();
            checkoutMgr.BookingInfo.Passengers = getPaxDetails();
            checkoutMgr.BookingInfo.PickupPoint = { CPId: $('#cmbBoardingPoint').val(), CPName: $('#cmbBoardingPoint :selected').text() };
            checkoutMgr.BookingInfo.DropOffPoint = { CPId: $('#cmbAligntingPoint').val(), CPName: $('#cmbAligntingPoint :selected').text() };
            EnableLoading = true;
            $.ajax({
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(checkoutMgr.BookingInfo),
                url: 'get/InventoryService.svc/BookSeats?session=' + Session.SessionId + '&authId=' + (Session.AuthId ? Session.AuthId : ""),
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data.IsSuccess) {
                        window.location.href = "Confirmation.aspx";
                    } else {
                        if (data.SelectedSeats) {
                            checkoutMgr.BookingInfo.SelectedSeats = data.SelectedSeats;
                        }
                        if (data.BookedSeats) {
                            checkoutMgr.BookingInfo.BookedSeats = data.BookedSeats;
                        }
                        checkoutMgr.BookingInfo.SelectedItinerary = tempitin;
                        checkoutMgr.RenderBookingInfo();
                        showMessage(data.ErrorMessage ? data.ErrorMessage : "Unknown eror!!!");
                    }
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
            passengers.push(pax);
        });
        return passengers;
    }

    this.AssignValidations = function () {
        $('#frmCheckOut').validate({
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
        $('[id*="txtFName"]').each(function () {
            $(this).rules('add', {
                required: true,
                minlength: 3,
                alphaonly: true,
                messages: {
                    required: "Please enter first name",
                    minlength: "Your first name must consist of at least 3 characters",
                    alphaonly: "Not a valid name."
                }
            });
        });
        $('[id*="txtLName"]').each(function () {
            $(this).rules('add', {
                required: true,
                minlength: 3,
                alphaonly: true,
                messages: {
                    required: "Please enter Last name(Surname)",
                    minlength: "Your last name must consist of at least 3 characters",
                    alphaonly: "Not a valid name."
                }
            });
        });
        $('[id*="txtAge"]').each(function () {
            $(this).rules('add', {
                required: true,
                number: true,
                maxlength: 3,
                messages: {
                    required: "Please enter age.",
                    number: "Not a valid age.",
                    maxlength: "Not a valid age."
                }
            });
        });
    }; // end of assign validations
};