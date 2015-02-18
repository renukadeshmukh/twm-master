var BusScheduleManager = function () {
    this.updateBusScheduleTemplate = null;
    this.BusScheduleTemplate = null;
    this.DateRangeId = 1000;
    $.get("../Templates/UpdateBusSchedule.htm", function (data) {
        busManager.BS.updateBusScheduleTemplate = data;
    });
    $.get("../Templates/BusScheduleTemplate.htm", function (data) {
        busManager.BS.BusScheduleTemplate = data;
    });

    this.ShowBusSchedule = function (loading) {
        if (busManager.selectedBusItem.BusSchedule == null || busManager.selectedBusItem.BusSchedule == undefined) {
            $(loading).show();
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/BusService.svc/get/frequency/' + Session.AuthId + '/' + busManager.selectedBusItem.BusTripId + '/' + Session.SessionId,
                dataType: "json",
                cache: false,
                success: function (data) {
                    $(loading).hide();
                    if (data != null) {
                        if (data.BusSchedule == null || !data.IsSuccess) {
                            showMessage(data.ErrorMessage ? data.ErrorMessage : "Bus schedule information not found.");
                        } else {
                            busManager.selectedBusItem.BusSchedule = data.BusSchedule;
                        }
                        $("#busSchContainer").setTemplate(busManager.BS.BusScheduleTemplate);
                        $("#busSchContainer").processTemplate(busManager.selectedBusItem);
                    }
                },
                error: function () {
                    showMessage('Error while getting bus schedule!!');
                    $(loading).hide();
                }
            });
        } else {
            $(loading).hide();
            $("#busSchContainer").setTemplate(busManager.BS.BusScheduleTemplate);
            $("#busSchContainer").processTemplate(busManager.selectedBusItem);
        }
    };

    this.ShowUpdateBusScheduleDialog = function (id) {
        var bus = null;
        for (var i = 0; i < busManager.busList.Buses.length; i++) {
            if (busManager.busList.Buses[i].BusTripId == id) {
                bus = busManager.busList.Buses[i];
                break;
            }
        }
        if (bus == null) {
            bus = {
                BusTripId: '0'
            };
        }
        var options = {
            modal: true,
            resizable: false,
            closeOnEscape: true,
            position: [800, 70]
        };
        $("#dialogBoxContainer").setTemplate(busManager.BS.updateBusScheduleTemplate);
        $("#dialogBoxContainer").processTemplate(bus);
        $("#dialogBoxContainer").dialog(options);
        $("#dialogBoxContainer").css("float", "right");
        $(".ui-dialog-titlebar").hide();
        $(".ui-widget-content").css("border", 0);
        $("#dialogBoxContainer").css("border", "3px solid #726E6E");

        $("#frmEditBusSchedule :input[type=text]").click(function () {
            this.select();
        });

        $(".ico-close").click(function () {
            $("#dialogBoxContainer").dialog("close");
        });

        $("#frmEditBusSchedule #btnCancel").click(function () {
            $("#dialogBoxContainer").dialog("close");
        });

        $("#cmbBusFrequency").change(function () {
            var frq = $(this).val();
            if (frq == 'SpecificWeekDays') {
                $("#dvWeekDays").show();
                $("#dvDateRanges").hide();
            } else if (frq == 'SpecificDates') {
                $("#dvWeekDays").hide();
                $("#dvDateRanges").show();
            } else {
                $("#dvWeekDays").hide();
                $("#dvDateRanges").hide();
            }
        });

        if (bus.BusTripId != '0') {
            if (bus.BusSchedule) {
                if (bus.BusSchedule.Frequency != 'SpecificWeekDays') {
                    $("#dvWeekDays").hide();
                }
                if (bus.BusSchedule.Frequency != 'SpecificDates') {
                    $("#dvDateRanges").hide();
                }
                if (bus.BusSchedule.Weekdays) {
                    for (var i = 0; i < bus.BusSchedule.Weekdays.length; i++) {
                        $("#wd" + bus.BusSchedule.Weekdays[i]).prop('checked', true);
                    }
                }
                if (bus.BusSchedule.Frequency == 'SpecificDates') {
                    for (var i = 0; i < bus.BusSchedule.DateRanges.length; i++) {
                        $("#tempDiv").setTemplate(busManager.DateRangeTemplate);
                        $("#tempDiv").processTemplate(bus.BusSchedule.DateRanges[i]);
                        $("#tblDateRanges").append($("#tempDiv").html());
                        $("#tempDiv").html('');
                        if (busManager.BS.DateRangeId <= bus.BusSchedule.DateRanges[i].RangeId) {
                            busManager.BS.DateRangeId = bus.BusSchedule.DateRanges[i].RangeId;
                        }
                    }
                    $("#frmEditBusSchedule [id*=txtDate]").datepicker({
                        dateFormat: 'M d, yy',
                        defaultDate: 0,
                        maxDate: "+6M",
                        stepMonths: 1,
                        numberOfMonths: 2
                    });
                }
            } else {
                $("#dvWeekDays").hide();
                $("#dvDateRanges").hide();
            }
        }
        busManager.BS.AssignValidations();
    }; //end of ShowEditBusDetailsDialog

    this.AddDateRangeElement = function () {
        var dateRange = { RangeId: ++busManager.BS.DateRangeId };
        $("#tempDiv").setTemplate(busManager.DateRangeTemplate);
        $("#tempDiv").processTemplate(dateRange);
        $("#tblDateRanges").append($("#tempDiv").html());
        $("#tempDiv").html('');
        $("#frmEditBusSchedule [id*=txtDate]").datepicker({
            dateFormat: 'M d, yy',
            defaultDate: 7,
            minDate: "0",
            maxDate: "+6M",
            stepMonths: 1,
            numberOfMonths: 2
        });
        busManager.BS.AssignValidations();
    };

    this.RemoveDateRangeElement = function (rangeId) {
        if (confirm("Are you sure that you want delete this date range?") && rangeId != null) {
            $("#Range_" + rangeId).remove();
        }
    };

    this.UpdateBusSchedule = function (id) {
        var isvalid = $('#frmEditBusSchedule').valid();
        if (!isvalid) return;
        var bus = null;
        for (var i = 0; i < busManager.busList.Buses.length; i++) {
            if (busManager.busList.Buses[i].BusTripId == id) {
                bus = busManager.busList.Buses[i];
                break;
            }
        }
        var freq = $("#cmbBusFrequency").val();
        var busSch = null;
        if (freq == 'Daily') {
            busSch = busManager.BS.GetDailyFrequencyFromDialog();
        }
        else if (freq == 'SpecificWeekDays') {
            busSch = busManager.BS.GetSpecificWeekdaysFrequencyFromDialog();
        }
        else if (freq == 'SpecificDates') {
            busSch = busManager.BS.GetDateRangeFrequencyFromDialog();
        }
        $.ajax({
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(busSch),
            url: 'get/BusService.svc/Update/frequency/' + Session.AuthId + '/' + busManager.selectedBusItem.BusTripId + '/' + Session.SessionId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess) {
                    busManager.selectedBusItem.BusSchedule = busSch;
                    busManager.BS.ShowBusSchedule("[id*=s-1] .loadingData");
                } else {
                    showMessage(data.ErrorMessage ? data.ErrorMessage : "Faild to update bus schedule!");
                }
                $("#dialogBoxContainer").dialog("close");
            },
            error: function () {
                showMessage('Error while updating bus schedule!!');
                $("#dialogBoxContainer").dialog("close");
            }
        });
    };

    this.GetDailyFrequencyFromDialog = function () {
        var busSch = {
            Frequency: 'Daily',
            Weekdays: null,
            DateRanges: null
        };
        return busSch;
    };

    this.GetSpecificWeekdaysFrequencyFromDialog = function () {
        var selectedDays = [];
        $("#dvWeekDays input:checked").each(function () { selectedDays.push(this.value); });
        var busSch = {
            Frequency: 'SpecificWeekDays',
            Weekdays: selectedDays,
            DateRanges: null
        };
        return busSch;
    };

    this.GetDateRangeFrequencyFromDialog = function () {
        var dateRanges = [];
        $("[id*=Range_]").each(function () {
            var rangeId = this.id.split('_')[1];
            var dateRange = {
                From: $("#txtDateFrom" + rangeId).val(),
                To: $("#txtDateTo" + rangeId).val()
            };
            dateRanges.push(dateRange);
        });
        var busSch = {
            Frequency: 'SpecificDates',
            Weekdays: null,
            DateRanges: dateRanges
        };
        return busSch;
    };

    this.AssignValidations = function () {
        $('#frmEditBusSchedule').validate();
        $('[id*="txtDateFrom"]').each(function () {
            $(this).rules('add', {
                required: true,
                messages: {
                    required: "Please select start date."
                }
            });
        });
        $('[id*="txtDateTo"]').each(function () {
            $(this).rules('add', {
                required: true,
                messages: {
                    required: "Please select start date."
                }
            });
        });
    }; // end of assign validations
};