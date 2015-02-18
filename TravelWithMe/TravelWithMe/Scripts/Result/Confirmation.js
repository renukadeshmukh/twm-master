var confirmMgr = null;
$(document).ready(function () {
    var params = getQueryString();
    confirmMgr = new ConfiramtionManager();
    if (params.BookingId) {
        confirmMgr.GetBooking(params);
    } else {
        confirmMgr.GetBookingInfo();
    }
});

var ConfiramtionManager = function () {
    this.BookingInfo = null;

    this.GetBookingInfo = function () {
        if (confirmMgr.BookingInfo) {
            confirmMgr.RenderBookingInfo();
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
                        confirmMgr.BookingInfo = data.BookingInfo;
                        confirmMgr.BookingInfo.ShowSuccess = true;
                        confirmMgr.BookingInfo.BookedSeats = data.BookedSeats;
                        confirmMgr.BookingInfo.SelectedItinerary.SeatMap = JSON.parse(confirmMgr.BookingInfo.SelectedItinerary.SeatMap);
                        confirmMgr.RenderBookingInfo();
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

    this.GetBooking = function (params) {
        if (params) {
            EnableLoading = true;
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/InventoryService.svc/GetBooking?session=' + Session.SessionId + '&authId=' + (Session.AuthId ? Session.AuthId : "") + '&busTripId=' + params.BusTripId + '&bookingId=' + params.BookingId,
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data.IsSuccess && data.Booking) {
                        confirmMgr.BookingInfo = data.Booking;
                        confirmMgr.BookingInfo.ShowSuccess = false;
                        confirmMgr.RenderBookingInfo();
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

    this.RenderBookingInfo = function () {
        var obj = {
            BI: confirmMgr.BookingInfo,
            SI: confirmMgr.BookingInfo.SelectedItinerary,
            BSum: {
                BusTripId: confirmMgr.BookingInfo.SelectedItinerary.BusTripId,
                From: confirmMgr.BookingInfo.SelectedItinerary.From,
                To: confirmMgr.BookingInfo.SelectedItinerary.To,
                TravelDate: confirmMgr.BookingInfo.TravelDate,
                SelectedSeats: confirmMgr.BookingInfo.SelectedSeats.join(","),
                TotalAmount: confirmMgr.BookingInfo.TotalAmount
            }
        };
        $("#bookingInfoContainer").setTemplate($("#templateBookingInfo").val());
        $("#bookingInfoContainer").processTemplate(obj);
        $("#bookingId").html(obj.BI.BookingId);
        $("#lnkPrint").click(function () {
            $("#bookingInfoContainer").printArea();
        });
    };
};