var Session = {
    AuthId: '', //AuthenticationId
    SessionId: '',
    ItnIndex: 0,
    FirstName: '', //User First Name
    LastName: '', //User Last Name     
    Email: '', //User Email
    LastPage: '',
    CurrentPage: '',
    AccountType: '',
    PhoneNumber: ''
};

var FBLoaded = false;
var accountType = "EndUser";
$(document).ready(function () {
    LoadSession();
    Session.LastPage = Session.CurrentPage;
    Session.CurrentPage = location.href;
    SaveSession();
    $("#formSignin :input[type=text]").click(function () {
        this.select();
    });
});


Date.prototype.toMMddyyyyString = function (delimeter) {
    var dateStr = "" + (this.getMonth() < 8 ? "0" + this.getMonth() + 1 : this.getMonth() + 1) + delimeter + (this.getDate() < 9 ? "0" + this.getDate() : this.getDate()) + delimeter + this.getFullYear();
    return dateStr;
};

var EnableLoading = false;

jQuery.validator.addMethod(
	"validdate",
	function (value, element) {
	    var check = false;
	    var re = /^\d{1,2}\/\d{1,2}\/\d{4}$/;
	    if (re.test(value)) {
	        var adata = value.split('/');
	        var mm = parseInt(adata[0], 10);
	        var dd = parseInt(adata[1], 10);
	        var yyyy = parseInt(adata[2], 10);
	        var xdata = new Date(yyyy, mm - 1, dd);
	        if ((xdata.getFullYear() == yyyy) && (xdata.getMonth() == mm - 1) && (xdata.getDate() == dd))
	            check = true;
	        else
	            check = false;
	    } else
	        check = false;
	    return this.optional(element) || check;
	},
	"Please enter a date in the format mm/dd/yyyy"
);

jQuery.validator.addMethod(
	"alphaonly",
	function (value, element) {
	    var check = false;
	    var alphaExp = /^[A-Za-z]+$/;
	    if (value.match(alphaExp)) {
	        check = true;
	    }
	    else {
	        check = false;
	    }
	    return this.optional(element) || check;
	},
	"Please enter only alphabets"
);

function ProcessLoginResponse(data) {
    if (data != null) {
        if (data.AuthenticationId != undefined && data.AuthenticationId != '') {
            Session.AuthId = data.AuthenticationId;
            Session.FirstName = data.Account.FirstName;
            Session.LastName = data.Account.LastName;
            Session.AccountType = data.Account.AccountType;
            Session.PhoneNumber = data.Account.PhoneNumber;
            SaveSession();
            $("#liSignIn").hide();
            $("#userName").html(data.Account.FirstName);
            $("#liMyAccount").show();
            if (Session.AccountType == "BusOperator") {
                $("#liManageBuses").show();
                $("#liManageBookings").show();
                $("#liBusOperators").hide();
                $("#liSettings").show();
            } else if (Session.AccountType == "EndUser") {
                $("#liManageBuses").hide();
                $("#liManageBookings").hide();
                $("#liBusOperators").hide();
                $("#liSettings").hide();
            } else {
                $("#liManageBuses").hide();
                $("#liBusOperators").show();
                $("#liManageBookings").hide();
            }
            if (loginSuccess != null) {
                loginSuccess();
            }
            //TODO: uncomment on release
            //if (window.location.protocol != 'https:') {
            //      window.location.href = "https:" + window.location.href.substring(window.location.protocol.length);
            //}
        } else {
            Session.AuthId = '';
            showMessage(data.ErrorMessage);
            if (loginFailure != null) {
                loginFailure();
            }
        }
    } else {
        Session.AuthId = '';
        showMessage("Login failed. Please check values and try again.");
        if (loginFailure != null) {
            loginFailure();
        }
    }
    SaveSession();
}

function Login() {
    var isValid = $("#formSignin").valid();
    if (isValid) {
        var accountRequest = {
            Email: $('#txtEmail').val(),
            Password: $('#txtPassword').val()
        };
        EnableLoading = true;

        $.ajax({
            type: "POST",
            url: '/get/AccountService.svc/login?session=' + Session.SessionId,
            data: JSON.stringify(accountRequest),
            contentType: "application/json",
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess == true) {
                    Session.Email = $('#txtEmail').val();
                    ProcessLoginResponse(data);
                } else {
                    if (data.ErrorMessage) {
                        showMessage(data.ErrorMessage);
                    } else {
                        showMessage('Unknown error while signing in. Please try again.');
                    }
                }
            },
            error: function () {
                showMessage('Error while signing in. Please try again.');
            }
        });
        $("#accessForm").dialog("close");
    }
}

function ChangePassword() {
    var isValid = $("#formSignin").valid();
    if (isValid) {
        EnableLoading = true;
        var request = {
            CurrentPassword: $('#txtCurrentPwd').val(),
            NewPassword: $('#txtNewPassword').val()
        };
        $.ajax({
            type: "POST",
            url: '/get/AccountService.svc/changepassword?session=' + Session.SessionId + '&authId=' + Session.AuthId,
            data: JSON.stringify(request),
            contentType: "application/json",
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data) {
                    showMessage('Password successfully changed.', 'success_message');
                    $("#accessForm").dialog("close");
                }
                else {
                    showMessage('Error while changing password. Please check values and try again.');
                }
            },
            error: function () {
                showMessage('Error while changing password. Please check values and try again.');
            }
        });
    }
}

function showLogin(e, position) {
    $("#login").show();
    $("#changePassword").hide();
    $("#signup").hide();
    var options = position ? {
        width: 350,
        modal: true,
        resizable: false,
        position: [e.pageX - 320, e.pageY + 10]
    } : {
        width: 350,
        modal: true,
        resizable: false,
        position: [520, 30]
    };

    $("#accessForm").dialog(options);

    $(".ui-dialog-titlebar").hide();
    $(".ui-widget-content").css("border", 0);
    $("#accessForm").css("border", "4px solid #DADADA");
}

function showChangePassword(e, position) {
    $("#changePassword").show();
    $("#login").hide();
    $("#signup").hide();

    var options = position ? {
        width: 350,
        modal: true,
        resizable: false,
        position: [e.pageX - 320, e.pageY + 10]
    } : {
        width: 350,
        modal: true,
        resizable: false,
        position: [520, 100]
    };

    $("#accessForm").dialog(options);

    $(".ui-dialog-titlebar").hide();
    $(".ui-widget-content").css("border", 0);
    $("#accessForm").css("border", "4px solid #DADADA");
}

function SaveSession() {
    document.cookie = "Session=" + JSON.stringify(Session);
}

function LoadSession() {
    var cookieStr = GetCookie("Session");

    if (cookieStr != undefined && cookieStr != '') {
        var object = JSON.parse(cookieStr);

        if (object != null)
            Session = object;
    }
}

function ShowLoading() {
    $('#divLoadingMain').show();
    $('#divLoadingMain').dialog({
        resizable: false,
        minHeight: 50,
        closeOnEscape: false,
        modal: true
    });
    $(".ui-dialog-titlebar").hide();
    $(".ui-widget-content").css("border", 0);
}

function HideLoading() {
    $('#divLoadingMain').dialog('close');
}

function GetCookie(c_name) {
    var i, x, y, ARRcookies = document.cookie.split(";");
    for (i = 0; i < ARRcookies.length; i++) {
        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
        x = x.replace(/^\s+|\s+$/g, "");
        if (x == c_name) {
            return unescape(y);
        }
    }
    return null;
}

var loginSuccess = null;
var loginFailure = null;

function getQueryString() {
    var result = {}, queryString = location.search.substring(1),
      re = /([^&=]+)=([^&]*)/g, m;

    while (m = re.exec(queryString)) {
        result[decodeURIComponent(m[1])] = decodeURIComponent(m[2]);
    }

    return result;
}

function showMessage(message, type) {
    if (type == undefined)
        type = 'error_message';
    $("#divMessage").addClass(type);
    $("#divMessage").html(message);
    $("#divMessage").show();
    setTimeout(function () { $("#divMessage").hide(); $("#divMessage").removeClass(type); }, 5000);
    $('html, body').animate({ scrollTop: 0 }, 'slow');
}

function CheckLogin(showLoginDialog) {
    if (Session != null && Session.AuthId) {
        EnableLoading = true;
        $.ajax({
            type: "GET",
            url: '/get/AccountService.svc/validate/' + Session.AuthId + '?session=' + Session.SessionId,
            cache: false,
            dataType: "json",
            success: function (data) {
                if (showLoginDialog) {
                    loginFailure = function () {
                        showLogin(null, false);
                    };
                }
                ProcessLoginResponse(data);
            },
            error: function () {
                Session.AuthId = '';
                SaveSession();
                showMessage('Session Ivalid.');
            }
        });
    }
    else {
        if (showLoginDialog) {
            showLogin(null, false);
        }
    }
}

$(document).ready(function () {
    $('#divLoadingMain')
    .hide() // hide it initially
    .ajaxStart(function () {
        if (EnableLoading) {
            ShowLoading();
        }
    })
    .ajaxStop(function () {
        HideLoading();
        EnableLoading = false;
    });

    LoadSession();

    CheckLogin();

    $("#lnkChangePassword").click(function (e) {
        showChangePassword(e, false);
    });

    $("#lnkDashboard").click(function (e) {
        window.location.href = "/Dashboard.aspx";
    });


    $(".ico-close", "#accessForm").click(function () {
        $("#accessForm").dialog("close");
    });

    $("#lnkSignin").click(function (e) {
        showLogin(e, false);
    });

    $("#lnkLogin").click(function (e) {
        showLogin(e, false);
    });
    
    $("#btnChangePwd").click(function (e) {
        ChangePassword();
    });

    $("#lnkLogout").click(function (e) {
        SignOut();
    });

    

    function SignOut() {
        EnableLoading = true;
        $.ajax({
            type: "GET",
            url: '/get/AccountService.svc/logout/' + Session.AuthId + '?session=' + Session.SessionId,
            cache: false,
            dataType: "json",
            success: function (data) {

            },
            error: function () {

            }
        });
        Session.AuthId = '';
        Session.SessionId = '';
        Session.AccountType = '';
        SaveSession();
        $("#userName").html('Guest');
        $("#liSignIn").show();
        $("#liMyAccount").hide();
        window.location.href = "Home.aspx";
    }


    $("#lnkSignInNow").click(function () {
        $("#signup").hide();
        $("#login").show();
    });

    $("#lnkRegister").click(function () {
        accountType = "EndUser";
        $("#signup").show();
        $("#login").hide();
    });

    $("#lnkRegisterOp").click(function () {
        accountType = "BusOperator";
        $("#signup").show();
        $("#login").hide();
    });

    $("#lnkForgotPwd").click(function () {
        var isValid = $('#txtEmail').val() != null && $('#txtEmail').val() != '';
        if (isValid) {
            EnableLoading = true;
            $.ajax({
                type: "GET",
                url: '/get/AccountService.svc/forgotpasswordbyemail?session=' + Session.SessionId + '&email=' + $('#txtEmail').val(),
                cache: false,
                dataType: "json",
                success: function (data) {
                    var end = new Date().getTime();
                    if (data != null && data.IsSuccess) {
                        showMessage('Password resent to your email address. Please check your email and try again with new password.', 'success_message');
                    }
                    else {
                        showMessage('Error while sending password. Please check email address and try again.');
                    }
                    $("#accessForm").dialog("close");
                },
                error: function () {
                    showMessage('Error while sending password. Please check email address and try again.');
                    $("#accessForm").dialog("close");
                }
            });
        }
        else {
            $("#formSignin").valid();
        }
    });


    $("#btnRegister").click(function () {
        var isValid = $("#formSignin").valid();
        if (isValid) {
            EnableLoading = true;
            var account = {
                FirstName: $('#txtFirstName').val(),
                LastName: $('#txtLastName').val(),
                Email: $('#txtEmail1').val(),
                PhoneNumber: $('#txtPNCode').val() + ' ' + $('#txtPNumber').val(),
                AccountType: accountType
            };
            $.ajax({
                type: "POST",
                url: '/get/AccountService.svc/register?session=' + Session.SessionId,
                contentType: "application/json",
                data: JSON.stringify(account),
                cache: false,
                dataType: "json",
                success: function (data) {
                    Session.Email = $('#txtEmail1').val();
                    ProcessLoginResponse(data);
                    $("#accessForm").dialog("close");
                    if (accountType == "BusOperator" && data.IsSuccess && !data.ErrorMessage) {
                        window.location.href = "OperatorSettings.aspx?IsEmpty=true";
                    }else {
                        window.location.href = "RegistrationConfirmation.aspx";
                    }
                },
                error: function () {
                    showMessage('Error while registering user. Please try again.');
                }
            });
        }
    });


    $("#btnSignin").click(function () {
        Login();
    });

    $("#txtPassword").keyup(function (event) {
        if (event.keyCode == 13) {
            $("#btnSignin").click();
        }
    });

    $(".siteLogo").click(function () {
        window.location.href = "/Home.aspx";
    });

    $('#formSignin').validate({
        rules: {
            txtFirstName: "required",
            txtLastName: "required",
            txtEmail1: {
                required: true,
                email: true
            },
            txtPassword: "required",
            txtMobileCode: "required",
            txtEmail: {
                required: true,
                email: true
            },
            txtCurrentPwd: "required",
            txtNewPassword: {
                required: true,
                minlength: 5
            },
            txtNewPassword2: {
                required: true,
                minlength: 5,
                equalTo: "#txtNewPassword"
            },
            chkTerms: "required"
        },
        messages: {
            txtPassword: "Password is required",
            txtMobileCode: "Please enter mobile verification code.",
            txtEmail: {
                required: "Email is required",
                email: "Please enter a valid email"
            },
            txtFirstName: "First Name is required",
            txtLastName: "Last Name is required",
            txtEmail1: {
                required: "Email is required",
                email: "Please enter a valid email"
            },
            txtCurrentPwd: "Please enter current password.",
            txtNewPassword: {
                required: "Please enter new password.",
                minlength: "Password should be atleast 5 characters"
            },
            txtNewPassword2: {
                required: "Please re-enter new password.",
                minlength: "Password should be atleast 5 characters",
                equalTo: "Enter the same password as above"
            },
            chkTerms: "Please accept terms of use"
        }
    });
});

Date.prototype.addDays = function (number) { this.setDate(this.getDate() + number); return this; };
Date.prototype.addWeeks = function (number) { this.setDate(this.getDate() + (number * 7)); return this; };
Date.prototype.addMonths = function (number) { this.setMonth(this.getMonth() + number); return this; };
Date.prototype.addYears = function (number) { this.setYear(this.getYear() + number); return this; };
Date.prototype.toMMddyyyyString = function (delimeter) {
    var dateStr = (this.getMonth() <= 8 ? "0" + (this.getMonth() + 1) : (this.getMonth() + 1)) + delimeter + "" + (this.getDate() < 9 ? "0" + this.getDate() : this.getDate()) + delimeter + "" + this.getFullYear();
    return dateStr;
};
Date.prototype.toddMMyyyyString = function () {
    var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var dateFormatter = ["st", "nd", "rd", "th"];
    var currentDate = this.getDate();
    var dateStr = currentDate + dateFormatter[currentDate % 10 < 4 ? currentDate % 10 != 0 ? parseInt(currentDate / 10) != 1 ? currentDate % 10 - 1 : 3 : 3 : 3] + " " + monthNames[this.getMonth()] + " " + this.getFullYear();
    return dateStr;
};

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regexS = "[\\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.search);
    if (results == null)
        return "";
    else
        return decodeURIComponent(results[1].replace(/\+/g, " "));
}