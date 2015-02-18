<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    Inherits="Logging.Exceptions" CodeBehind="~/Exceptions.aspx.cs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/redmond/jquery-ui-1.7.1.custom.css" rel="stylesheet" type="text/css" />
    <link href="Styles/flexigrid/flexigrid.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-min.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-min.js" type="text/javascript"></script>
    <script src="Scripts/flexigrid.js" type="text/javascript"></script>
    <script type="text/javascript">
    //<![CDATA[
        var _parseData = null;
        function ShowExceptionDetails(id) {
            var rowObj = null;
            $.each(_parseData.rows, function (i, j) {
                if (j.id == id) {
                    rowObj = j;
                }
            });
            var width = $(document).width() - 300;
            if (rowObj != null) {
                var root = $('#dialog-exceptions');
                var tableContainer = $('table', root);
                tableContainer.empty();
                $.each(_parseData.cellNames, function (i, j) {
                    var name = j;
                    var val = rowObj.cell[i];
                    if (j == 'Id')
                        val = id;
                    tableContainer.append('<tr><td valign="top">' + name + '</td><td valign="top">&nbsp;:&nbsp;</td><td valign="top"><div style="white-space:pre-line;max-width:' + width + 'px;overflow-x:hidden">' + val + '</div></td></tr>');
                });
                root.dialog({ closeOnEscape: true, hide: 'slide', show: 'slide', maxHeight: 450, height: 450, width: 'auto', resizable: true, position: 'center', modal: true });
            }
        }


        function GoToLogs(sessionid) {
            var _sessionid = sessionid.toString().split(',')[0];
            var redirectUrl = '<%=Page.ResolveUrl("~/Logs.aspx")%>' + '?sessionId=' + _sessionid;
            window.open(redirectUrl);
        }

        /* On DOM Ready */
        $(function () {

            var queryString = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < queryString.length; i++) {
                var value = queryString[i].split('=')[1];
            }
            if (!$.browser.msie) {
                history.replaceState({}, "Exceptions", window.location.href.split("?")[0]);
            } 
            /* Create grid for Excetions*/
            $("#exceptionslogs").flexigrid({
                url: '<%=Page.ResolveUrl("~/Services/ReportService.svc/Exceptions")%>',
                dataType: 'json',
                preProcess: function (a) {
                    if (typeof (a) == 'string') { _parseData = $.parseJSON(a); } else { _parseData = a; }
                    var pageNumber = _parseData.page;
                    var pageSize = this.rp;
                    $.each(_parseData.rows, function (i, j) {
                        j.cell[0] = '<a href="#" style="padding:0px;" title="click to see details" onclick="ShowExceptionDetails(\'' + j.id + '\');">' + j.id + '</a>';                        
                    });
                    return _parseData;
                },
                colModel: [
				{ display: 'Id', name: 'Id', width: 30, sortable: false, align: 'left' },
                { display: 'Session Id', name: 'SessionId', width: 200, sortable: false, align: 'left' },
                { display: 'Title', name: 'Title', width: 150, sortable: false, align: 'left' },
				{ display: 'Severity', name: 'Severity', width: 40, sortable: false, align: 'left' },
				{ display: 'Timestamp', name: 'Timestamp', width: 120, sortable: false, align: 'left' },
				{ display: 'Machine Name', name: 'MachineName', width: 100, sortable: false, align: 'left' },
                { display: 'Exception Type', name: 'ExceptionType', width: 150, sortable: false, align: 'left' },
                { display: 'Message', name: 'Message', width: 300, sortable: false, align: 'left' },
				{ display: 'Source', name: 'Source', width: 100, sortable: false, align: 'left' },
				{ display: 'App Domain', name: 'AppDomainName', width: 200, sortable: false, align: 'left' },
				],
                searchitems: [{ display: '', name: '' }, ],
                sortname: 'Id',
                sortorder: 'Timestamp',
                usepager: true,
                /*title: 'Exceptions Logs',*/
                showTableToggleBtn: false,
                height: 'auto',
                striped: true, //apply odd even stripes
                novstripe: false,
                resizable: true, //resizable table
                errormsg: 'Connection Error',
                nowrap: true, //
                page: 1, //current page
                total: 1, //total pages
                useRp: true, //use the results per page select box
                rp: 15, // results per page
                rpOptions: [10, 15, 20, 25, 30],
                pagestat: 'Displaying {from} to {to} of {total} items',
                procmsg: 'Processing, please wait ...',
                query: '',
                qtype: '',
                nomsg: 'No items',
                minColToggle: 1, //minimum allowed column to be hidden
                showToggleBtn: false, //show or hide column toggle popup
                hideOnSubmit: false,
                autoload: false,
                blockOpacity: 0,
                onToggleCol: false,
                CustomSearch: function () {
                    var search = $('<table style="width: 100%;">' +
                        '<tr>' +
                        '<td><table><tr><td style="width: 100px;">Message</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="Message" size="30" title="Text to search in exceptionm message"/></td></tr></table></td>' +
                        '<td><table><tr><td style="width: 100px;">Search Text</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="SearchText" size="30" title="Search text in StackTrace & AdditionalInfo"/></td></tr></table></td>' +
                        '<td><table><tr><td style="width: 100px;">Timestamp</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="TimeStampF" size="30"/></td></tr></table></td>' +
                        '<td><table><tr><td style="width: 100px;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;and</td><td>&nbsp;&nbsp;&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="TimeStampT" size="30"/></td></tr></table></td>' +
                        '</tr>' +
                        '<tr>' +
                        '<td><table><tr><td style="width: 100px;">Machine Name</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="MachineName" size="30" title="Machine name"/></td></tr></table></td>' +
                        '<td><table><tr><td style="width: 100px;">Id</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="Id" size="30" title="Exception Identifier"/></td></td></tr></table></td>' +
                        '<td><table><tr><td style="width: 100px;">SessionId</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="SessionId" size="30"/></td></tr></table></td>' +
                        '<td><table><tr><td style="width: 100px;">Exception Type</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="ExceptionType" size="30" title="Type of the exception"/></td></tr></table></td>' +
                        '</tr>' +
                        '<tr>' +
                        '<td><table><tr><td style="width: 100px;">Source</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="Source" size="30" title="Source"/></td></tr></table></td>' +
                        '<td><table><tr><td style="width: 100px;">Target Site</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="TargetSite" size="30" title="Target of occurrence"/></td></tr></table></td>' +
                        '<td><table><tr><td style="width: 100px;">AppDomain</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="AppDomainName" size="30" title="Application domain identity"/></td></tr></table></td>' +
//                        '<td class="dkdiv"><table><tr><td style="width: 100px;">DK</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="Dk" size="30" title="Dk Identifier"/></td></tr></table></td>' +
                        '</tr>' +
                        '</table>');

                    var today = new Date();
                    today.setDate(today.getDate() + 1);
                    $('input[name=q][data-field=TimeStampF],input[name=q][data-field=TimeStampT]', search).datepicker({ hideIfNoPrevNext: true, defaultDate: '+1d', maxDate: '+1d', showButtonPanel: false, changeMonth: true });
                    return search;
                }
            });

            if (value != null) {
                $('.pSearch').click();
                $("input[data-field='SessionId']").val(value);
            }
            $('div#searchOptions input[value=Search]').click();
            $('input:button').button().css('font-weight', 'bold');

        });
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div id="div-exceptionslogs">
        <table id="exceptionslogs" style="display: none;">
        </table>
        <div id="dialog-exceptions" title="Exception Detail" style="display: none;">
            <fieldset>
                <table>
                </table>
            </fieldset>
        </div>
    </div>
</asp:Content>
