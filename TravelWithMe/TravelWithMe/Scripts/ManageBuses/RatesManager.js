var RatesManager = function () {
    this.busRatesTemplate = null;
    this.BusRateDialogTemplete = null;

    $.get("../Templates/BusRatesTemplate.htm", function (data) {
        busManager.BR.busRatesTemplate = data;
    });
    $.get("../Templates/BusRateInfodialog.htm", function (data) {
        busManager.BR.BusRateDialogTemplete = data;
    });

    this.ShowBusRates = function (loading) {
        if (!busManager.selectedBusItem.BusRates) {
            $(loading).show();
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/BusService.svc/rate/GetAll/' + Session.AuthId + '/' + busManager.selectedBusItem.BusTripId + '/' + Session.SessionId,
                dataType: "json",
                cache: false,
                success: function (data) {
                    $(loading).hide();
                    if (data != null) {
                        if (data.Rates == null || !data.IsSuccess) {
                            showMessage(data.ErrorMessage);
                        } else {
                            busManager.selectedBusItem.BusRates = data.Rates;
                        }
                        $("#ratesContainer").setTemplate(busManager.BR.busRatesTemplate);
                        $("#ratesContainer").processTemplate(busManager.selectedBusItem);
                    }
                },
                error: function () {
                    showMessage('Error while getting rates!!');
                    $(loading).hide();
                }
            });
        } else {
            $(loading).hide();
            $("#ratesContainer").setTemplate(busManager.BR.busRatesTemplate);
            $("#ratesContainer").processTemplate(busManager.selectedBusItem);
        }
    };

    this.ShowRateDialog = function (rateId) {
        var busRate = null;
        if (busManager.selectedBusItem.BusRates) {
            for (var i = 0; i < busManager.selectedBusItem.BusRates.length; i++) {
                if (busManager.selectedBusItem.BusRates[i].RateId.toString() == rateId) {
                    busRate = busManager.selectedBusItem.BusRates[i];
                    break;
                }
            }
        }
        if (busRate == null) {
            busRate = {
                RateId: 0
            };
        }
        var options = {
            modal: true,
            resizable: false,
            closeOnEscape: true,
            position: [800, 70]
        };
        $("#dialogBoxContainer").setTemplate(busManager.BR.BusRateDialogTemplete);
        $("#dialogBoxContainer").processTemplate(busRate);
        $("#dialogBoxContainer").dialog(options);
        $("#dialogBoxContainer").css("float", "right");
        $(".ui-dialog-titlebar").hide();
        $(".ui-widget-content").css("border", 0);
        $("#dialogBoxContainer").css("border", "3px solid #726E6E");

        $("#frmBusRate :input[type=text]").click(function () {
            this.select();
        });

        $(".ico-close").click(function () {
            $("#dialogBoxContainer").dialog("close");
        });

        $("#frmBusRate #btnCancel").click(function () {
            $("#dialogBoxContainer").dialog("close");
        });
        var dateToday = new Date();
        $("#frmBusRate #txtDateFrom").datepicker({
            dateFormat: 'M d, yy',
            defaultDate: 0,
            minDate: dateToday,
            maxDate: "+6M",
            stepMonths: 1,
            numberOfMonths: 2,
            onSelect: function (selected) {
                $("#frmBusRate #txtDateTo").datepicker("option", "minDate", selected);
            }
        });
        $("#frmBusRate #txtDateTo").datepicker({
            dateFormat: 'M d, yy',
            defaultDate: 0,
            minDate: dateToday,
            maxDate: "+6M",
            stepMonths: 1,
            numberOfMonths: 2,
            onSelect: function (selected) {
                $("#frmBusRate #txtDateFrom").datepicker("option", "maxDate", selected);
            }
        });
        $("#txtWeekDayRate").focus();
        if (busRate.RateId != 0) {

        }
    };
    this.DeleteRate = function (rateId) {
        var busRate = null;
        var index = 0;
        var busTripId = '';
        for (var i = 0; i < busManager.selectedBusItem.BusRates.length; i++) {
            if (busManager.selectedBusItem.BusRates[i].RateId.toString() == rateId) {
                busRate = busManager.selectedBusItem.BusRates[i];
                busTripId = busManager.selectedBusItem.BusTripId;
                index = i;
                break;
            }
        }
        if (busTripId != '' && busRate != null && confirm("Are you sure that you want delete Rate applied from-" + busRate.DateFrom + "?")) {
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/BusService.svc/rate/delete/' + Session.AuthId + '/' + busTripId + '/' + rateId + '/' + Session.SessionId,
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data.IsSuccess) {
                        $("#Rate" + rateId).remove();
                        busManager.selectedBusItem.BusRates.splice(index, 1);
                        new RatesManager().ShowBusRates(busManager.selectedBusItem);
                    } else {
                        showMessage(data.ErrorMessage);
                    }
                },
                error: function () {
                    showMessage('Error while deleting rate!!');
                }
            });


        }
    };

    this.SaveBtn_click = function (rateId) {
        var rate = {
            RateId: rateId,
            DateFrom: $('#txtDateFrom').val(),
            DateTo: $('#txtDateTo').val(),
            WeekDayRate: $('#txtWeekDayRate').val(),
            WeekEndRate: $('#txtWeekEndRate').val()
        };

        var busTripId = busManager.selectedBusItem.BusTripId;

        if (busTripId != '' && rate != null) {
            if (rateId == '0') {
                new RatesManager().AddBusRate(rate, busTripId);
            }
            else {
                new RatesManager().UpdateBusRate(rate, busTripId);
            }
        }
    };

    this.UpdateBusRate = function (rate, busTripId) {
        $.ajax({
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(rate),
            url: 'get/BusService.svc/rate/update/' + Session.AuthId + '/' + busTripId + '/' + Session.SessionId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess) {
                    $("#dialogBoxContainer").dialog("close");
                    for (var i = 0; i < busManager.selectedBusItem.BusRates.length; i++) {
                        if (busManager.selectedBusItem.BusRates[i].RateId.toString() == rate.RateId) {
                            busManager.selectedBusItem.BusRates[i] = rate;
                            break;
                        }
                    }
                    new RatesManager().ShowBusRates(busManager.selectedBusItem);
                } else {
                    showMessage(data.ErrorMessage);
                }
            },
            error: function () {
                showMessage('Error while updating bus rate!!');
            }
        });
    };


    this.AddBusRate = function (rate, busTripId) {
        $.ajax({
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(rate),
            url: 'get/BusService.svc/rate/add/' + Session.AuthId + '/' + busTripId + '/' + Session.SessionId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess) {

                    $("#dialogBoxContainer").dialog("close");
                    rate.RateId = data.RateId;
                    busManager.selectedBusItem.BusRates.push(rate);
                    new RatesManager().ShowBusRates(busManager.selectedBusItem);
                } else {
                    showMessage(data.ErrorMessage);
                }
            },
            error: function () {
                showMessage('Error while adding bus rate!!');
            }
        });
    };
};