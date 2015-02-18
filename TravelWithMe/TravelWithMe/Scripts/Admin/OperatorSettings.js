var operatorSettings = null;
$(document).ready(function () {
    operatorSettings = new OperatorSettings();
    var qs = getQueryString("IsEmpty");

    if (qs.IsEmpty && qs.IsEmpty == "true") {
        operatorSettings.ShowSettings();
        operatorSettings.IsNew = true;
    } else {
        operatorSettings.GetBusOperatorInfo();
        operatorSettings.IsNew = false;
    }
});

var OperatorSettings = function () {
    this.BusOperator = null;
    this.IsNew = true;
    this.GetBusOperatorInfo = function () {
        EnableLoading = true;
        $.ajax({
            type: "GET",
            contentType: "application/json",
            url: 'get/AccountService.svc/GetBusOperatorInfo/' + Session.AuthId + '?session=' + Session.SessionId,
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data != null && data.IsSuccess && data.BusOperator != null) {
                    operatorSettings.BusOperator = data.BusOperator;
                    operatorSettings.ShowSettings();
                } else {
                    showMessage(data.ErrorMessage ? data.ErrorMessage : "Failed to retrive settings, if you have saved information please report.");
                    operatorSettings.ShowSettings();
                }
            },
            error: function () {
                showMessage('Error while getting your setting information!! Please report!');
            }
        });
    };

    this.SaveBusOperator = function () {
        var isvalid = $('#frmOpSettings').valid();
        if (isvalid) {
            operatorSettings.BusOperator = {
                CompanyName: $("#txtCompany").val(),
                Email: [$("#txtOpEmail").val()],
                PhoneNumber: [$("#txtOpPhoneNumber").val()],
                Addresses: [{
                    AddressType: "Company",
                    AddressLine1: $("#txtAddressLine1").val(),
                    AddressLine2: $("#txtAddressLine2").val(),
                    City: $("#txtCity").val(),
                    State: $("#txtState").val(),
                    Country: $("#txtCountry").val(),
                    ZipCode: $("#txtZipCode").val()
                }],
                BankAccount: {
                    BankName: $("#txtBankName").val(),
                    IFSCCode: $("#txtIFSCCode").val(),
                    Branch: $("#txtBankBranch").val(),
                    AccountHolderName: $("#txtAccHolderName").val(),
                    AccountNumber: $("#txtAccNumber").val()
                }
            };
            $.ajax({
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(operatorSettings.BusOperator),
                url: 'get/AccountService.svc/SaveBankOperatorInfo/' + Session.AuthId + '?session=' + Session.SessionId,
                dataType: "json",
                cache: false,
                success: function (data) {
                    if (data != null && data.IsSuccess) {
                        if (operatorSettings.IsNew) {
                            window.location.href = "RegistrationConfirmation.aspx";
                        }else {
                            showMessage("Information saved successfully!", true);
                        }
                    } else {
                        showMessage(data.ErrorMessage);
                    }
                },
                error: function () {
                    showMessage('Error while updating city points!!');
                }
            });
        }
    };

    this.ShowSettings = function () {
        if (!operatorSettings.BusOperator) {
            operatorSettings.BusOperator = {
                Email: [Session.Email],
                PhoneNumber: [Session.PhoneNumber]
            };
        }
        if (!operatorSettings.BusOperator.Addresses) {
            operatorSettings.BusOperator.Addresses = [{ Id: 0}];
        }
        if (!operatorSettings.BusOperator.BankAccount) {
            operatorSettings.BusOperator.BankAccount = {};
        }
        $("#settings").setTemplate($("#settingTemplate").val());
        $("#settings").processTemplate(operatorSettings.BusOperator);
        operatorSettings.AddValidations();
    };

    this.AddValidations = function () {
        $('#frmOpSettings').validate({
            rules: {
                txtCompany: {
                    required: true,
                    minlength: 3
                },
                txtOpEmail: {
                    required: true,
                    email: true
                },
                txtOpPhoneNumber: {
                    required: true,
                    minlength: 3
                },
                txtAddressLine1: {
                    required: true
                },
                txtCity: {
                    required: true
                },
                txtState: {
                    required: true
                },
                txtCountry: {
                    required: true
                },
                txtZipCode: {
                    required: true
                },
                txtBankName: {
                    required: true,
                    minlength: 3
                },
                txtIFSCCode: {
                    required: true
                },
                txtBankBranch: {
                    required: true
                },
                txtAccHolderName: {
                    required: true,
                    minlength: 3
                },
                txtAccNumber: {
                    required: true
                }
            },
            messages: {
                txtCompany: {
                    required: "Please enter company name.",
                    minlength: "Please enter valid company name."
                },
                txtOpEmail: {
                    required: "Please enter email id.",
                    email: "Please enter valid email id."
                },
                txtOpPhoneNumber: {
                    required: "Please enter valid phone number.",
                    minlength: "Please enter valid phone number"
                },
                txtAddressLine1: {
                    required: "Please enter company address information"
                },
                txtCity: {
                    required: "Please select valid city"
                },
                txtState: {
                    required: "Please select valid state"
                },
                txtCountry: {
                    required: "Please select valid country"
                },
                txtBankName: {
                    required: "Please enter bank name",
                    minlength: "Please enter valid bank name"
                },
                txtIFSCCode: {
                    required: "Please enter valid IFSC code of the bank."
                },
                txtBankBranch: {
                    required: "Please enter bank branch information"
                },
                txtAccHolderName: {
                    required: "Please enter bank account holder name.",
                    minlength: "Please enter valid name"
                },
                txtAccNumber: {
                    required: "Please enter bank account number"
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
    };
};