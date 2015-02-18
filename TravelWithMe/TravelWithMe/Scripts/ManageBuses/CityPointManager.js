var CityPointManager = function () {
    this.cpTemplate = null;
    this.CityPointDailogTemplate = null;
    $.get("../Templates/CityPointDialog.htm", function (data) {
        busManager.CP.CityPointDailogTemplate = data;
    });

    $.get("../Templates/CityPointsTemplate.htm", function (data) {
        busManager.CP.cpTemplate = data;
    });

    this.ShowCityPoints = function (loading) {
        if (busManager.selectedBusItem.CityPoints == null || busManager.selectedBusItem.CityPoints == undefined) {
            $(loading).show();
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/BusService.svc/cp/getAll/' + Session.AuthId + '/' + busManager.selectedBusItem.BusTripId + '/' + Session.SessionId,
                dataType: "json",
                cache: false,
                success: function (data) {
                    $(loading).hide();
                    if (data != null) {
                        if (!data.IsSuccess) {
                            showMessage(data.ErrorMessage);
                        } else {
                            busManager.selectedBusItem.CityPoints = data.CityPoints;
                        }
                        $("#cpContainer").setTemplate(busManager.CP.cpTemplate);
                        $("#cpContainer").processTemplate(busManager.selectedBusItem);
                    }
                },
                error: function () {
                    showMessage('Error while getting city points!!');
                    $(loading).hide();
                }
            });
        } else {
            $(loading).hide();
            $("#cpContainer").setTemplate(busManager.CP.cpTemplate);
            $("#cpContainer").processTemplate(busManager.selectedBusItem);
        }
    };

    this.ShowCityPointDialog = function (cpid) {
        var cityPoint = null;
        if (busManager.selectedBusItem.CityPoints) {
            for (var i = 0; i < busManager.selectedBusItem.CityPoints.length; i++) {
                if (busManager.selectedBusItem.CityPoints[i].CPId.toString() == cpid) {
                    cityPoint = busManager.selectedBusItem.CityPoints[i];
                    break;
                }
            }
        }
        if (cityPoint == null) {
            cityPoint = {
                CPId: 0
            };
        }
        var options = {
            modal: true,
            resizable: false,
            closeOnEscape: true,
            position: [800, 70]
        };
        $("#dialogBoxContainer").setTemplate(busManager.CP.CityPointDailogTemplate);
        $("#dialogBoxContainer").processTemplate(cityPoint);
        $("#dialogBoxContainer").dialog(options);
        $("#dialogBoxContainer").css("float", "right");
        $(".ui-dialog-titlebar").hide();
        $(".ui-widget-content").css("border", 0);
        $("#dialogBoxContainer").css("border", "3px solid #726E6E");

        $("#frmCityPoint :input[type=text]").click(function () {
            this.select();
        });

        $(".ico-close").click(function () {
            $("#dialogBoxContainer").dialog("close");
        });

        $("#frmCityPoint #btnCancel").click(function () {
            $("#dialogBoxContainer").dialog("close");
        });

        $("#txtCPCity", ".formbox").autocomplete({
            source: "/get/ContentService.svc/SearchCity?session=" + Session.SessionId,
            minLength: 2,
            dataType: 'json',
            autoFocus: true
        });

        $("#txtCPName", ".formbox").autocomplete({
            source: "/get/ContentService.svc/SearchCityPoint?session=" + Session.SessionId,
            minLength: 2,
            dataType: 'json',
            autoFocus: true
        });

        $("#txtCPDeptTime").timepicker({
            timeFormat: "hh:mm TT"
        });
        busManager.CP.AssignValidations();
    };
    this.DeleteCityPoint = function (cpid) {
        var cityPoint = null;
        var busTripId = '';
        var index = 0;

        for (var i = 0; i < busManager.selectedBusItem.CityPoints.length; i++) {
            if (busManager.selectedBusItem.CityPoints[i].CPId.toString() == cpid) {
                cityPoint = busManager.selectedBusItem.CityPoints[i];
                busTripId = busManager.selectedBusItem.BusTripId;
                index = i;
                break;
            }
        }
        if (busTripId != '' && cityPoint != null && confirm("Are you sure that you want delete city point -" + cityPoint.CPName + "?")) {
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/BusService.svc/cp/del/' + Session.AuthId + '/' + busTripId + '/' + cpid + '/' + Session.SessionId,
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data.IsSuccess) {
                        $("#CP" + cpid).remove();
                        busManager.selectedBusItem.CityPoints.splice(index, 1);
                        busManager.CP.ShowCityPoints(busManager.selectedBusItem);
                    } else {
                        showMessage(data.ErrorMessage);
                    }
                },
                error: function () {
                    showMessage('Error while deleting city points!!');
                }
            });

        }
    };

    this.SaveBtn_click = function (cpId) {
        var isvalid = $('#frmCityPoint').valid();
        if (!isvalid) return;
        var cpTime = null;
        var days = parseInt($('#cmbCPDay').val());
        var arrTime = $('#txtCPDeptTime').val();
        if (arrTime != null) {
            var arrTimeArr = arrTime.split(':');
            var cpTime = {
                Hours: arrTimeArr[0],
                Minutes: arrTimeArr[1].split(' ')[0],
                Meridian: arrTimeArr[1].split(' ')[1],
                Days: days
            };
        }
        var cp = {
            CPId: cpId,
            CPTime: cpTime,
            CPName: $('#txtCPName').val(),
            IsDropOffPoint: $('#chkCPDropoff').attr('checked') ? true : false,
            IsPickupPoint: $('#chkCPPickup').attr('checked') ? true : false,
            CityName: $('#txtCPCity').val()
        };

        var busTripId = busManager.selectedBusItem.BusTripId;

        if (busTripId != '' && cp != null) {
            busManager.CP.UpdateCP(cp, busTripId);
        }
        //busManager.CP.ShowCityPoints("[id*=s-2] .loadingData");
    };

    this.UpdateCP = function (cp, busTripId) {
        $.ajax({
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(cp),
            url: 'get/BusService.svc/cp/update/' + Session.AuthId + '/' + busTripId + '/' + Session.SessionId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess) {
                    $("#dialogBoxContainer").dialog("close");
                    var foundIndex = null;
                    if (busManager.selectedBusItem.CityPoints) {
                        for (var i = 0; i < busManager.selectedBusItem.CityPoints.length; i++) {
                            if (busManager.selectedBusItem.CityPoints[i].CPId == cp.CPId) {
                                foundIndex = i;
                                break;
                            }
                        }
                    } else {
                        busManager.selectedBusItem.CityPoints = [];
                    }
                    cp.CPId = data.CPId;
                    if (foundIndex) {
                        busManager.selectedBusItem.CityPoints[foundIndex] = cp;
                    } else {
                        busManager.selectedBusItem.CityPoints.push(cp);
                    }
                    busManager.CP.ShowCityPoints("[id*=s-2] .loadingData");
                } else {
                    showMessage(data.ErrorMessage);
                }
            },
            error: function () {
                showMessage('Error while updating city points!!');
            }
        });
    };

    this.AssignValidations = function () {
        $('#frmCityPoint').validate({
            rules: {
                txtCPCity: {
                    required: true
                },
                txtCPName: {
                    required: true
                },
                txtCPDeptTime: {
                    required: true
                }
            },
            messages: {
                txtCPCity: {
                    required: "Please enter city"
                },
                txtCPName: {
                    required: "Please enter city point name."
                },
                txtCPDeptTime: {
                    required: "Please select bus departure time from city point."
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
};