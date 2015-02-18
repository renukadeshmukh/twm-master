var home = null;
$(document).ready(function () {
    home = new Home();
    home.GetRecentSearches();
    home.AssignValidations();
});

var Home = function () {
    this.RecentSearches = []; 
    this.GetRecentSearches = function () {
        $.ajax({
            type: "GET",
            contentType: "application/json",
            url: 'get/ContentService.svc/GetRecentSearches?session=' + Session.SessionId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess && data.RecentSearches) {
                    home.RecentSearches = data.RecentSearches;
                    $("#tblRecentSearches").setTemplate($("#templateRecentSearch").val());
                    $("#tblRecentSearches").processTemplate({ Searches: home.RecentSearches });
                    $("[id*=txtTDate]").datepicker({
                        dateFormat: 'M d, yy',
                        defaultDate: 7,
                        minDate: "0",
                        maxDate: "+6M",
                        stepMonths: 1,
                        numberOfMonths: 1
                    });

                    $("[id*=txtTDate_]").change(function () {
                        var fromTo = this.id.split('_');
                        var travelDate = $(this).val();
                        var from = fromTo[1];
                        var to = fromTo[2];
                        if (from && to && travelDate) {
                            window.location.href = "Results.aspx?from=" + from + "&to=" + to + "&travelDate=" + travelDate;
                        } else {
                            var errorMessage = [];
                            errorMessage.push(from ? "" : "From city missing.");
                            errorMessage.push(to ? "" : "To city missing.");
                            errorMessage.push(travelDate ? "" : "Travel date missing.");
                            showMessage(errorMessage.join('|'));
                        }
                    });
                }
            },
            error: function () {
                showMessage('Error occured while getting results for you!! Please report!');
            }
        });
        
    };

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
            var travelDate = $("#txtTravelDate").val();
            var from = $("#txtFromCity").val();
            var to = $("#txtToCity").val();
            window.location.href = "Results.aspx?from=" + from + "&to=" + to + "&travelDate=" + travelDate;
        }
    });

    this.AssignValidations = function () {
        $('#frmSearchBox').validate({
            rules: {
                txtFromCity: {
                    required: true
                },
                txtToCity: {
                    required: true
                },
                txtTravelDate: {
                    required: true
                }
            },
            messages: {
                txtFromCity: {
                    required: "Please enter city name."
                },
                txtToCity: {
                    required: "Please enter city name."
                },
                txtTravelDate: {
                    required: "Please select travel date."
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
    };
};