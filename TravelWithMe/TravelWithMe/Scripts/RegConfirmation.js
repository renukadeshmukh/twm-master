var confirmMgr = null;
$(document).ready(function () {
    $("#containerBox").setTemplate($("#templateRegConfirm").val());
    $("#containerBox").processTemplate({IsBusOperator:Session.AccountType == 'BusOperator' ? true : false});
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
    };
};