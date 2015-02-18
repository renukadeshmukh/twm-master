var SeatArrengementManager = function () {
    this.BusSeatArrTemplate = null;
    this.EditSeatArrangementDialog = null;
    this.DefaultSeatMaps = null;
    this.SeatMaps = null;
    this.EditedSeatMap = null;
    $.get("../Templates/EditSeatArrangementDialog.htm", function (data) {
        busManager.SAM.EditSeatArrangementDialog = data;
    });
    $.get("../Templates/BusSeatArrTemplate.htm", function (data) {
        busManager.SAM.BusSeatArrTemplate = data;
    });
    this.ShowSeatArrangement = function (loading) {
        if (busManager.selectedBusItem.SeatMap == null || busManager.selectedBusItem.SeatMap == undefined || busManager.selectedBusItem.SeatMap.NotSet) {
            $(loading).show();
            $.ajax({
                type: "GET",
                contentType: "application/json",
                url: 'get/BusService.svc/bus/GetBusSeatMap/' + Session.AuthId + '/' + busManager.selectedBusItem.BusTripId + '/' + Session.SessionId,
                dataType: "json",
                cache: false,
                success: function (data) {
                    $(loading).hide();
                    if (data != null) {
                        if (data.SeatMap == null || data.SeatMap == '') {
                            busManager.selectedBusItem.SeatMap = {
                                NotSet: 'Seat map not set!!! Please edit and select or create seatmap.'
                            };
                        } else {
                            busManager.selectedBusItem.SeatMap = JSON.parse(data.SeatMap);
                        }
                        $("#seatArrContainer").setTemplate(busManager.SAM.BusSeatArrTemplate);
                        $("#seatArrContainer").processTemplate(busManager.selectedBusItem);
                        if (!busManager.selectedBusItem.SeatMap.NotSet) {
                            busManager.SAM.DrawSeatMap("seatMapArea", { Id: busManager.selectedBusItem.SeatMapId, SeatMap: busManager.selectedBusItem.SeatMap });
                        }
                    } else {
                        showMessage('Error while getting seatmap!!');
                    }
                },
                error: function () {
                    showMessage('Error while getting user seatmap!!');
                    $(loading).hide();
                }
            });
        } else {
            $(loading).hide();
            $("#seatArrContainer").setTemplate(busManager.SAM.BusSeatArrTemplate);
            $("#seatArrContainer").processTemplate(busManager.selectedBusItem);
            busManager.SAM.DrawSeatMap("seatMapArea", { Id: busManager.selectedBusItem.SeatMapId, SeatMap: busManager.selectedBusItem.SeatMap });
        }
    };

    this.ShowEditSeatArrDialog = function (id) {
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
        busManager.SAM.SeatMaps = [];
        if (busManager.SAM.DefaultSeatMaps) {
            for (var sm = 0; sm < busManager.SAM.DefaultSeatMaps.length; sm++) {
                busManager.SAM.DefaultSeatMaps[sm].IsSelected = false;
                busManager.SAM.SeatMaps.push(busManager.SAM.DefaultSeatMaps[sm]);
            }
        }
        if (bus.SeatMap != null && !bus.SeatMap.NotSet) {
            var busSeatMap = { Id: bus.SeatMapId, SeatMap: bus.SeatMap };
            busSeatMap.IsSelected = true;
            busManager.SAM.SeatMaps.push(busSeatMap);
        }
        var options = {
            modal: true,
            resizable: false,
            closeOnEscape: true,
            position: [800, 10]
        };
        $("#dialogBoxContainer").setTemplate(busManager.SAM.EditSeatArrangementDialog);
        $("#dialogBoxContainer").processTemplate(bus);
        $("#dialogBoxContainer").dialog(options);
        $("#dialogBoxContainer").css("float", "right");
        $(".ui-dialog-titlebar").hide();
        $(".ui-widget-content").css("border", 0);
        $("#dialogBoxContainer").css("border", "3px solid #726E6E");

        $("#frmSeatArr :input[type=text]").click(function () {
            this.select();
        });

        $(".ico-close").click(function () {
            $("#dialogBoxContainer").dialog("close");
        });

        $("#frmSeatArr #btnCancel").click(function () {
            $("#dialogBoxContainer").dialog("close");
        });

        $("#frmSeatArr #btnSave").click(function () {
            busManager.SAM.SaveSeatMap();
        });

        $("#frmSeatArr .seatmapnavL").click(function () {
            var mapIndex = parseInt($(this).attr('MapIndex'));
            $("#frmSeatArr #btnEditSeatArr").attr('index', mapIndex.toString());
            busManager.SAM.DrawSeatMap("defaultArr", busManager.SAM.SeatMaps[mapIndex]);
            mapIndex = mapIndex == 0 ? busManager.SAM.SeatMaps.length - 1 : mapIndex - 1;
            $("#frmSeatArr .seatmapnavR").attr('MapIndex', mapIndex.toString());
            $("#frmSeatArr .seatmapnavL").attr('MapIndex', mapIndex.toString());

        });

        $("#frmSeatArr .seatmapnavR").click(function () {
            var mapIndex = parseInt($(this).attr('MapIndex'));
            $("#frmSeatArr #btnEditSeatArr").attr('index', mapIndex.toString());
            busManager.SAM.DrawSeatMap("defaultArr", busManager.SAM.SeatMaps[mapIndex]);
            mapIndex = ((mapIndex + 1) % busManager.SAM.SeatMaps.length);
            $("#frmSeatArr .seatmapnavR").attr('MapIndex', mapIndex.toString());
            $("#frmSeatArr .seatmapnavL").attr('MapIndex', mapIndex.toString());
        });

        busManager.SAM.DrawSeatMap("defaultArr", busManager.SAM.SeatMaps[busManager.SAM.SeatMaps.length - 1]);
        $("#frmSeatArr #btnEditSeatArr").attr('index', (busManager.SAM.SeatMaps.length - 1).toString());

        $("#defaultArr .defaultSeatMap").click(function () {
            var mapIndex = parseInt($("#frmSeatArr #btnEditSeatArr").attr('index'));
            for (var index = 0; index < busManager.SAM.SeatMaps.length; index++) {
                if (index == mapIndex) {
                    busManager.SAM.SeatMaps[index].IsSelected = true;
                } else {
                    busManager.SAM.SeatMaps[index].IsSelected = false;
                }
            }
            $("#defaultArr .selectedSeatMap").show();
        });
    }; //end of ShowEditBusDetailsDialog

    this.SaveSeatMap = function () {
        var seatMapRQ = {
            BusTripId: busManager.selectedBusItem.BusTripId,
            SeatMap: '',
            SeatMapId: 0
        };

        if (busManager.SAM.EditedSeatMap == null) {
            for (var index = 0; index < busManager.SAM.SeatMaps.length; index++) {
                if (busManager.SAM.SeatMaps[index].IsSelected) {
                    seatMapRQ.SeatMap = JSON.stringify(busManager.SAM.SeatMaps[index].SeatMap);
                    seatMapRQ.SeatMapId = busManager.SAM.SeatMaps[index].Id;
                }
            }
        } else {
            seatMapRQ.SeatMap = busManager.SAM.EditedSeatMap.SeatMap;
        }
        $("#dialogBoxContainer").dialog("close");
        EnableLoading = true;
        $.ajax({
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(seatMapRQ),
            url: 'get/BusService.svc/UpdateSeatMap/' + Session.AuthId + '/' + Session.SessionId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess) {
                    $("#dialogBoxContainer").dialog("close");
                    busManager.selectedBusItem.SeatMap = JSON.parse(seatMapRQ.SeatMap);
                    busManager.SAM.ShowSeatArrangement();
                } else {
                    showMessage(data.ErrorMessage ? data.ErrorMessage : 'Error while updating bus seatmap!! Please report!');
                }
            },
            error: function () {
                showMessage('Error while updating bus seatmap!! Please report!');
            }
        });
    };

    this.SetControlValues = function (seatMap) {
        if (seatMap != null) {
            $("#frmSeatArr #cmbBusType").val(seatMap.BusType);
            $("#frmSeatArr #cmbBusSize").val(seatMap.Decks[0].Size);
            $("#frmSeatArr #txtSeatCount").val(seatMap.SeatCount);
            $("#frmSeatArr #txtBerthCount").val(seatMap.BerthCount);
        }
    };

    this.DrawSeatMap = function (container, seatmap) {
        if (seatmap != null && !seatmap.NotSet) {
            if (!seatmap.SeatMap) {
                busManager.SAM.GetSeatMap(seatmap.Id, container);
                return;
            }
            busManager.SAM.SetControlValues(seatmap.SeatMap);
            if (seatmap.IsSelected) {
                $("#" + container + " .selectedSeatMap").show();
            } else {
                $("#" + container + " .selectedSeatMap").hide();
            }
            $("#" + container + " .deck [class*=Seat], #" + container + " .deck [class*=Berth]").remove();
            if (seatmap.SeatMap.Name != null && seatmap.SeatMap.Name != '' && seatmap.SeatMap.Name != undefined) {
                $("#" + container + " .seatmapheader div :first").html('<b>Seat map name:</b> ' + seatmap.SeatMap.Name);
            } else {
                $("#" + container + " .seatmapheader div :first").html('');
            }

            if (seatmap.SeatMap.Decks.length == 2) {
                $("#" + container + " .deck :last").show();
            } else {
                $("#" + container + " .deck :last").hide();
            }
            var y = 5;
            var x = 70;
            var stepY1 = [25, 30, 25, 0];
            var stepY2 = [55, 0, 0, 0];
            var stepX = 25;
            for (var dk = 0; dk < seatmap.SeatMap.Decks.length; dk++) {
                x = 70;
                y = 5;
                for (var i = 0; i < seatmap.SeatMap.Decks[dk].Seats.length; i++) {
                    var seat = seatmap.SeatMap.Decks[dk].Seats[i];
                    var seatDiv = "<div class='" + seat.SeatType + "' style='top: " + y + "px; left: " + x + "px; z-index:2'></div>";
                    $($("#" + container + " .deck")[dk]).append(seatDiv);
                    stepX = seat.SeatType.indexOf('BerthH') != -1 ? 45 : 25;
                    y = seat.SeatType.indexOf('BerthV') != -1 ? (y + stepY2[seat.Row]) : (y + stepY1[seat.Row]);
                    if (seat.SeatType.indexOf('BerthV') == -1 && seatmap.SeatMap.Decks[dk].Size == '2X1' && seat.Row == 1) {
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

    this.ShowDefaultSeatArr = function () {
        $("#defaultArr").slideDown();
        $("#editArea").slideUp();
        $("#frmSeatArr #cmbBusType").unbind('change');
        busManager.SAM.EditedSeatMap = null;
    };

    this.InitEditArea = function () {
        $("#defaultArr").slideUp();
        $("#editArea").slideDown();
        var seatmap = busManager.SAM.SeatMaps[parseInt($("#btnEditSeatArr").attr('index'))];
        busManager.SAM.EditedSeatMap = JSON.parse(JSON.stringify(seatmap));
        $(".seatarrangement").on("click", "#editArea .deck [class*=Seat],#editArea .deck [class*=Berth]", function (e) {
            $(this).remove();
        });

        $("#frmSeatArr #cmbBusType").change(function () {
            var busType = $(this).val();
            if (busType == 'Sleeper') {
                $("#editArea .deck").show();
            } else {
                $("#editArea .deck :first").show();
                $("#editArea .deck :last").hide();
            }
        });
        busManager.SAM.SetControlValues(busManager.SAM.EditedSeatMap.SeatMap);
        busManager.SAM.DrawSeatMap("editArea", busManager.SAM.EditedSeatMap);
        $(".dragobjects div").draggable({
            helper: "clone",
            start: function (event, ui) { busManager.SAM.CreateSeatContainers(event, ui); }
        });
    };

    this.CreateSeatContainers = function (evt, obj) {
        $("#editArea .deck [class*=container]").remove();
        var sclass = obj.helper[0].classList[0];
        var cols = 0;
        var rows = 0;
        var stepX = 0;
        var stepY = 0;
        var y = 5;
        var x = 70;
        if (sclass.indexOf("BerthH") != -1) {
            sclass = "bearthcontainerH";
            cols = 8;
            rows = 4;
            stepX = 45;
            stepY = [y, y + 25, y + 25 + 30, y + 25 + 30 + 25];
        } else if (sclass.indexOf("BerthV") != -1) {
            sclass = "bearthcontainerV";
            cols = 15;
            rows = 2;
            stepX = 25;
            stepY = [y, y + 25 + 30];
        } else {
            sclass = "seatcontainer";
            cols = 15;
            rows = 4;
            stepX = 25;
            stepY = [y, y + 25, y + 25 + 30, y + 25 + 30 + 25];
        }

        for (var row = 0; row < rows; row++) {
            x = 70;
            y = stepY[row];
            for (var col = 0; col < cols; col++) {
                var container = getContainer('1', sclass, x - 1, y - 1);
                $("#editArea .deck :first").append(container);
                container = getContainer('2', sclass, x - 1, y - 1);
                $("#editArea .deck :last").append(container);
                x = x + stepX;
            }
        }

        $("#editArea .deck [class*=container]").droppable({
            accept: "." + obj.helper[0].classList[0],
            hoverClass: "activeContainer",
            drop: function (event, ui) {
                var cln = $(ui.draggable).clone();
                cln[0].style.top = (parseInt(this.style.top) + 1) + "px";
                cln[0].style.left = (parseInt(this.style.left) + 1) + "px";
                if ($(this).attr('deck') == '1') {
                    $("#editArea .deck :first").append(cln[0]);
                } else {
                    $("#editArea .deck :last").append(cln[0]);
                }
                $("#editArea .deck [class*=container]").remove();
            }
        });

        function getContainer(deck, sc, left, top) {
            return "<div deck='" + deck + "' class='" + sc + "' style='top: " + top + "px; left: " + left + "px; z-index:1'></div>";
        }
    };

    this.FillSeats = function () {
        $("#editArea .deck [class*=Seat], #editArea .deck [class*=Berth]").remove();
        var bustype = $("#cmbBusType").val();
        var bussize = $("#cmbBusSize").val();
        var seatCnt = parseInt($("#txtSeatCount").val() == '' ? 0 : $("#txtSeatCount").val());
        var berthCnt = parseInt($("#txtBerthCount").val() == '' ? 0 : $("#txtBerthCount").val());
        var bstepX = 45, sstepX = 25;
        var y = 5;
        var x = 70;
        var stepY = [y, y + 25, y + 25 + 30, y + 25 + 30 + 25, y, y + 25, y + 25 + 30, y + 25 + 30 + 25];
        var rows = 0, cols = 0;
        if (bustype == 'Sleeper' && berthCnt > 0) {
            if (bussize == '2X2') {
                rows = 8;
            } else {
                rows = 6;
            }
        } else if (bustype == 'Seater' && seatCnt > 0) {
            if (bussize == '2X2') {
                rows = 4;
            } else {
                rows = 3;
            }
            berthCnt = 0;
        }
        cols = parseInt((seatCnt + berthCnt) / rows);
        var step = sstepX;
        var addedSeatCnt = 0;
        for (var col = 0; col < cols; col++) { // Draw seats
            var yIndex = 0;
            for (var row = 0; row < rows; row++) {
                if (bussize == '2X1' && (row == 2 || row == 5)) {
                    yIndex++;
                }
                y = stepY[yIndex++];
                var seat = getSeatElement("availableSeat", x, y);
                if (addedSeatCnt >= seatCnt) {
                    seat = getSeatElement("availableBerthH", x, y);
                    step = bstepX;
                }
                if (step == sstepX) addedSeatCnt++;
                if (row < 4) {
                    $("#editArea .deck :first").append(seat);
                } else {
                    $("#editArea .deck :last").append(seat);
                }
            }
            x = x + step;
        }

        function getSeatElement(sc, left, top) {
            return "<div class='" + sc + "' style='top: " + top + "px; left: " + left + "px; z-index:2'></div>";
        }
    };

    this.GetDefaultSeatMaps = function () {
        $.ajax({
            type: "GET",
            contentType: "application/json",
            url: 'get/BusService.svc/bus/GetDefaultSeatMaps/' + Session.AuthId + '/' + Session.SessionId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess) {
                    busManager.SAM.DefaultSeatMaps = data.SeatMaps;
                    busManager.SAM.GetSeatMap(busManager.SAM.DefaultSeatMaps[0].Id);
                } else {
                    //showMessage('Error while getting seatmaps!!');
                }
            },
            error: function () {
                //showMessage('Error while getting seatmaps!!');
            }
        });
    };

    this.GetSeatMap = function (seatMapId, container) {
        $.ajax({
            type: "GET",
            contentType: "application/json",
            url: 'get/BusService.svc/bus/GetSeatMap/' + Session.AuthId + '/' + Session.SessionId + '/' + seatMapId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess) {
                    var seatMapObj = findSeatMapById(seatMapId, busManager.SAM.DefaultSeatMaps);
                    if (seatMapObj)
                        seatMapObj.SeatMap = JSON.parse(data.SeatMap);
                    if (container)
                        busManager.SAM.DrawSeatMap(container, seatMapObj);
                } else {
                    showMessage('Error while getting seatmaps!!');
                }
            },
            error: function () {
                showMessage('Error while getting seatmaps!!');
            }
        });
    };

    function findSeatMapById(seatMapId, arr) {
        if (arr) {
            for (var i = 0; i < arr.length; i++) {
                if (arr[i].Id == seatMapId) {
                    return arr[i];
                }
            }
        }
        return null;
    }
};