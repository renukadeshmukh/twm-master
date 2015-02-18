var BusDetailsManager = function () {
    this.ShowBusDetails = function (bus) {
        $("#detailsContainer").setTemplate(busManager.busDetailsTemplate);
        $("#detailsContainer").processTemplate(bus);
        $(".showHideButtonContainer").click(function () {
            var ids = this.id.split('-');
            $("[id*=s-" + ids[1] + "]").toggle('slow');
            var action = $("#" + this.id + " .showHideButton div:first-child").html();
            if (action == "Show") {
                $("#" + this.id).css("border-left-width", "3px");
                $("#" + this.id).css("border-color", "#bab7b7");
                $("#" + this.id).css("background-color", "#e7ebf3");
                $("#" + this.id + " .showHideButton div:first-child").html('Hide');
                $("#" + this.id + " .showHideButton div:last-child").removeClass('showdetails').addClass('hidedetails');

                if (ids[0] == 'BusSchedule') {
                    busManager.BS.ShowBusSchedule("[id*=s-" + ids[1] + "] .loadingData");
                } else if (ids[0] == 'BusRate') {
                    busManager.BR.ShowBusRates("[id*=s-" + ids[1] + "] .loadingData");
                } else if (ids[0] == 'BusStop') {
                    busManager.CP.ShowCityPoints("[id*=s-" + ids[1] + "] .loadingData");
                } else if (ids[0] == 'SeatArr') {
                    busManager.SAM.ShowSeatArrangement("[id*=s-" + ids[1] + "] .loadingData");
                }
            } else {
                $("#" + this.id).css("border-left-width", "1px");
                $("#" + this.id).css("border-color", "#e5e5e5");
                $("#" + this.id).css("background-color", "white");
                $("#" + this.id + " .showHideButton div:first-child").html('Show');
                $("#" + this.id + " .showHideButton div:last-child").removeClass('hidedetails').addClass('showdetails');
            }
        });

        $(".showHideButtonContainer").hover(function () {
            var action = $("#" + this.id + " .showHideButton div:first-child").html();
            if (action == "Show") {
                $("#" + this.id).css("background-color", "#e7ebf3");
            }
        }, function () {
            var action = $("#" + this.id + " .showHideButton div:first-child").html();
            if (action == "Show") {
                $("#" + this.id).css("background-color", "white");
            }
        });

        $("#scrollBusDetails").mCustomScrollbar();
    };

    this.DeleteBus = function (busTripId) {
        var index = 0;
        for (var i = 0; i < busManager.busList.Buses.length; i++) {
            if (busManager.busList.Buses[i].BusTripId == busTripId) {
                index = i;
                break;
            }
        }
        if (busTripId != '' && confirm("Are you sure that you want delete this bus ?")) {
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/BusService.svc/bus/del/' + Session.AuthId + '/' + busTripId + '/' + Session.SessionId,
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data.IsSuccess) {
                        $("#busItem" + busTripId).remove();
                        busManager.busList.Buses.splice(index, 1);
                        if (busManager.busList.Buses.length > 0)
                            busManager.DM.ShowBusDetails(busManager.busList.Buses[0]);
                    } else {
                        showMessage(data.ErrorMessage);
                    }
                },
                error: function () {
                    showMessage('Error while deleting bus!!');
                }
            });
        }
    };

    this.SaveBtn_click = function (busTripId) {
        var isvalid = $('#frmAddOrEditBusDetials').valid();
        if (!isvalid) return;
        var isAc = $('#chkAUBAC').attr('checked') ? true : false;

        var deptBusTime = null;
        var arrBusTime = null;
        var days = parseInt($('#cmbAUBArrDay').val());
        var arrTime = $('#txtAUBTimeArriv').val();

        if (arrTime != null) {
            var arrTimeArr = arrTime.split(':');
            arrBusTime = {
                Hours: arrTimeArr[0],
                Minutes: arrTimeArr[1].split(' ')[0],
                Meridian: arrTimeArr[1].split(' ')[1],
                Days: days
            };
        }
        var deptTime = $('#txtAUBTimeDept').val();
        if (deptTime != null) {
            var deptTimeArr = deptTime.split(':');
            deptBusTime = {
                Hours: deptTimeArr[0],
                Minutes: deptTimeArr[1].substring(0, 2),
                Meridian: deptTimeArr[1].substring(2),
                Days: 0
            };
        }

        var busDetails = {
            BusName: $('#txtAUBBusName').val(),
            FromLoc: {
                Name: $('#txtAUBFromCity').val()
            },
            ToLoc: {
                Name: $('#txtAUBToCity').val()
            },
            DepartureTime: deptBusTime,
            ArrivalTime: arrBusTime,
            IsAC: isAc,
            BusType: $('#cmbAUBBusType').val()
        };

        if (busTripId == '0') {
            busManager.DM.AddBus(busDetails);

        }
        else {
            busManager.DM.UpdateBus(busDetails, busTripId);
        }
    };

    this.UpdateBus = function (busDetails, busTripId) {
        $.ajax({
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(busDetails),
            url: 'get/BusService.svc/bus/update/' + Session.AuthId + '/' + busTripId + '/' + Session.SessionId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess) {
                    $("#dialogBoxContainer").dialog("close");
                    busManager.selectedBusItem.BusName = busDetails.BusName;
                    busManager.selectedBusItem.FromLoc = busDetails.FromLoc;
                    busManager.selectedBusItem.ToLoc = busDetails.ToLoc;
                    busManager.selectedBusItem.DepartureTime = busDetails.DepartureTime;
                    busManager.selectedBusItem.ArrivalTime = busDetails.ArrivalTime;
                    busManager.selectedBusItem.IsAC = busDetails.IsAC;
                    busManager.selectedBusItem.BusType = busDetails.BusType;
                    busManager.ShowBusList();
                    busManager.SelectBus(busDetails.BusTripId);
                } else {
                    showMessage(data.ErrorMessage);
                }
            },
            error: function () {
                showMessage('Error while updating bus details!!');
            }
        });
    };

    this.AddBus = function (busDetails) {
        $.ajax({
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(busDetails),
            url: 'get/BusService.svc/bus/add/' + Session.AuthId + '/' + Session.SessionId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess && data.BusTripId != null && data.BusTripId != '0') {
                    $("#dialogBoxContainer").dialog("close");
                    if (busManager.busList == null || busManager.busList.Buses == null) {
                        busManager.busList = { Buses: [] };
                    }
                    busDetails.BusTripId = data.BusTripId;
                    busDetails.IsEnabled = false;
                    busManager.busList.Buses.push(busDetails);
                    busManager.ShowBusList();
                    busManager.SelectBus(busDetails.BusTripId);
                } else {
                    showMessage(data.ErrorMessage);
                }
            },
            error: function () {
                showMessage('Error while adding bus!');
            }
        });
    };

    this.ShowEditBusDetailsDialog = function (id) {
        var bus = null;
        if (busManager.busList && busManager.busList.Buses) {
            for (var i = 0; i < busManager.busList.Buses.length; i++) {
                if (busManager.busList.Buses[i].BusTripId == id) {
                    bus = busManager.busList.Buses[i];
                    break;
                }
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

        $("#dialogBoxContainer").setTemplate(busManager.editBusTemplate);
        $("#dialogBoxContainer").processTemplate(bus);
        $("#dialogBoxContainer").dialog(options);
        $("#dialogBoxContainer").css("float", "right");
        $(".ui-dialog-titlebar").hide();
        $(".ui-widget-content").css("border", 0);
        $("#dialogBoxContainer").css("border", "3px solid #726E6E");
        $("[id*=txtAUBTime]").timepicker({
            timeFormat: "hh:mm TT"
        });
        $("#txtAUBFromCity", ".formbox").autocomplete({
            source: "/get/ContentService.svc/SearchCity?session=" + Session.SessionId,
            minLength: 2,
            dataType: 'json',
            autoFocus: true
        });

        $("#txtAUBToCity", ".formbox").autocomplete({
            source: "/get/ContentService.svc/SearchCity?session=" + Session.SessionId,
            minLength: 2,
            dataType: 'json',
            autoFocus: true
        });

        $("#frmAddOrEditBusDetials :input[type=text]").click(function () {
            this.select();
        });

        $(".ico-close").click(function () {
            $("#dialogBoxContainer").dialog("close");
        });

        $("#frmAddOrEditBusDetials #btnCancel").click(function () {
            $("#dialogBoxContainer").dialog("close");
        });

        $('#frmAddOrEditBusDetials').validate({
            rules: {
                txtAUBFromCity: {
                    required: true,
                    validCity: true
                },
                txtAUBToCity: {
                    required: true,
                    validCity: true,
                    sameCity: true
                },
                txtAUBBusName: {
                    required: true,
                    minlength: 3
                },
                txtAUBTimeDept: {
                    required: true
                },
                txtAUBTimeArriv: {
                    required: true
                }
            },
            messages: {
                txtAUBFromCity: {
                    required: "Select from city",
                    validCity: "Select valid city"
                },
                txtAUBToCity: {
                    required: "Select to city",
                    validCity: "Select valid city",
                    sameCity: "Choose different city"
                },
                txtAUBBusName: {
                    required: "Please enter bus name",
                    minlength: "Bus name must consist of at least 3 characters"
                },
                txtAUBTimeDept: {
                    required: "Please select departure time."
                },
                txtAUBTimeArriv: {
                    required: "Please select arrival time."
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
        }); // end of form validate

        function validateCity(text) {
            return true;
        };

        $.validator.addMethod("validCity", function (value, element) {
            return validateCity(value);
        }, "Select City name");

        $.validator.addMethod("sameCity", function (value, element) {
            return $("#txtAUBFromCity", ".formbox").val() != $("#txtAUBToCity", ".formbox").val();
        }, "From/To City Same.");
    }; //end of ShowEditBusDetailsDialog

    this.SetBusStatus = function (busTripId, isEnabled) {
        if (busTripId) {
            EnableLoading = true;
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/BusService.svc/bus/SetBusStatus/' + Session.AuthId + '/' + busTripId + '/' + Session.SessionId + '?isEnabled=' + isEnabled,
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data.IsSuccess) {
                        if (isEnabled == 'true') {
                            busManager.selectedBusItem.IsEnabled = true;
                            $("#lnkEnableBus").hide();
                            $("#lnkDisableBus").show();
                            $($("#spBusStatus i")[2]).html('Enabled');
                            $($("#spBusStatus i")[2]).prop('title', 'Online customers can book this bus!');
                        } else {
                            busManager.selectedBusItem.IsEnabled = false;
                            $("#lnkEnableBus").show();
                            $("#lnkDisableBus").hide();
                            $($("#spBusStatus i")[2]).html('Disabled');
                            $($("#spBusStatus i")[2]).prop('title', 'Online customers can not book this bus!');
                        }
                    } else {
                        showMessage(data.ErrorMessage);
                    }
                },
                error: function () {
                    showMessage('Error while changing  bus status!!');
                }
            });
        }
    };

    this.SetBusPublishStatus = function (busTripId, isPublished) {
        if (busTripId) {
            if (isPublished == 'false' && (Session.AccountType == 'BusOperator' || Session.AccountType == 'EndUser')) {
                showMessage("Sorry! You are not allowed to Un lock this bus.");
                $("#lnkUnPublish").hide();
                return;
            }
            EnableLoading = true;
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/BusService.svc/bus/SetBusPublishStatus/' + Session.AuthId + '/' + busTripId + '/' + Session.SessionId + '?isPublished=' + isPublished,
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data.IsSuccess) {
                        if (isPublished == 'true') {
                            busManager.selectedBusItem.IsPublished = true;
                            $("#lnkPublish").hide();
                            if (!(Session.AccountType == 'BusOperator' || Session.AccountType == 'EndUser')) {
                                $("#lnkUnPublish").show();
                            }
                            $($("#spBusStatus i")[1]).html('Locked');
                            $($("#spBusStatus i")[2]).prop('title', 'You can change only rate of the bus!');
                        } else {
                            busManager.selectedBusItem.IsPublished = false;
                            $("#lnkPublish").show();
                            $("#lnkUnPublish").hide();
                            $($("#spBusStatus i")[1]).html('Unlocked');
                            $($("#spBusStatus i")[2]).prop('title', 'You can change/add all bus information!');
                        }
                    } else {
                        showMessage(data.ErrorMessage);
                    }
                },
                error: function () {
                    showMessage('Error while changing  bus status!!');
                }
            });
        }
    };
}