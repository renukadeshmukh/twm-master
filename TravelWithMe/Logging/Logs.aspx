<%@ Page Title="About Us" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    Inherits="Logging.Logs" CodeBehind="~/Logs.aspx.cs" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
    <link href="Styles/redmond/jquery-ui-1.7.1.custom.css" rel="stylesheet" type="text/css" />
    <link href="Styles/flexigrid/flexigrid.css" rel="stylesheet" type="text/css" />
    <script src="Scripts/jquery-min.js" type="text/javascript"></script>
    <script src="Scripts/jquery-ui-min.js" type="text/javascript"></script>
    <script src="Scripts/flexigrid.js" type="text/javascript"></script>
    <script type="text/javascript">
    //<![CDATA[
        var _parseData = null;
        function ShowLogDetails(id) {
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
                    if (j == 'LogID')
                        val = id;
                    tableContainer.append('<tr><td valign="top">' + name + '</td><td valign="top">&nbsp;:&nbsp;</td><td valign="top"><div style="white-space:pre-line;max-width:' + width + 'px;overflow-x:hidden">' + val + '</div></td></tr>');
                });
                root.dialog({ closeOnEscape: true, hide: 'slide', show: 'slide', maxHeight: 450, height: 450, width: 'auto', resizable: true, position: 'center', modal: true });
            }
        }
        function ShowRequestXml(id)
        { }
        function ShowResponseXml(id)
        { }
        function GoToExceptions(sessionid) {
            var redirectUrl = '<%=Page.ResolveUrl("~/Exceptions.aspx")%>' + '?sessionId=' + sessionid;
            window.open(redirectUrl);
        }
        /* On DOM Ready */

        $(function () {

            var queryString = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < queryString.length; i++) {
                var value = queryString[i].split('=')[1];
            }

            if (!$.browser.msie) {
                history.replaceState({}, "Logs", window.location.href.split("?")[0]);
            }

            /* Create grid for Excetions*/
            $("#exceptionslogs").flexigrid({
                url: '<%=Page.ResolveUrl("~/Services/ReportService.svc/Logs")%>',
                dataType: 'json',
                preProcess: function (a) {
                    if (typeof (a) == 'string') { _parseData = $.parseJSON(a); } else { _parseData = a; }
                    $.each(_parseData.rows, function (i, j) {
                        j.cell[0] = '<a href="#" style="padding:0px;" title="click to see details" onclick="ShowLogDetails(\'' + j.id + '\');">' + j.id + '</a>';
                        j.cell[8] = '<a href="XmlHandler.ashx?t=request&id=' + j.id + '" target="_blank" style="padding:0px;" title="click to see request xml"> Click Here </a>';
                        j.cell[9] = '<a href="XmlHandler.ashx?t=response&id=' + j.id + '" target="_blank" style="padding:0px;" title="click to see response xml"> Click Here </a>';
                    });
                    return _parseData;
                },
                colModel: [
				{ display: 'Id', name: 'LogID', width: 50, sortable: false, align: 'left' },
                { display: 'SessionId', name: 'SessionId', width: 200, sortable: false, align: 'left' },
                { display: 'Timestamp', name: 'Timestamp', width: 135, sortable: false, align: 'left' },
				{ display: 'Machine Name', name: 'MachineName', width: 100, sortable: false, align: 'left' },
                { display: 'ServiceName', name: 'ServiceName', width: 150, sortable: false, align: 'left' },
                { display: 'Title', name: 'Title', width: 200, sortable: false, align: 'left' },
				{ display: 'Status', name: 'Status', width: 50, sortable: false, align: 'left' },
                { display: 'TimeTaken', name: 'TimeTaken', width: 75, sortable: false, align: 'left' },
                { display: 'Request', name: '', width: 50, sortable: false, align: 'left' },
                { display: 'Response', name: '', width: 50, sortable: false, align: 'left' }
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
                showToggleBtn: true, //show or hide column toggle popup
                hideOnSubmit: true,
                autoload: false,
                blockOpacity: 0,
                onToggleCol: true,
                CustomSearch: function () {
                    var search = $('<table style="width: 100%;">' +
                                        '<tr>' +
                                            '<td><table><tr><td style="width: 100px;">Id</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="Id" size="30" title="Log Identifier"/></td></td></tr></table></td>' +
                                            '<td><table><tr><td style="width: 100px;">Search Text</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="SearchText" size="30" title="Search text in Title, Message & AdditionalInfo"/></td></tr></table></td>' +
                    //'<td><table><tr><td style="width: 100px;">Timestamp</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="TimeStampF" size="30" /></td><td>&nbsp;and&nbsp;</td><td>&nbsp;&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="TimeStampT" size="30" /></td></tr></table></td>' +
                                            '<td><table><tr><td style="width: 100px;">Timestamp</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="TimeStampF" size="30"/></td></tr></table></td>' +
                                            '<td><table><tr><td style="width: 100px;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;and</td><td>&nbsp;&nbsp;&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="TimeStampT" size="30"/></td></tr></table></td>' +
                                        '</tr>' +
                                        '<tr>' +
                                            '<td><table><tr><td style="width: 100px;">Machine Name</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="MachineName" size="30" title="Machine name"/></td></tr></table></td>' +
                                            '<td><table><tr><td style="width: 100px;">SessionId</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="SessionId" size="30" title="Session Identifier"/></td></tr></table></td>' +
                    //'<td><table><tr><td style="width: 100px;">Request Time Range</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="TimeMin" size="30" /></td><td>&nbsp;and&nbsp;</td><td>&nbsp;&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="TimeMax" size="30" /></td></tr></table></td>' +
                                            '<td><table><tr><td style="width: 100px;">Request Time Range</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="TimeMin" size="30"/></td></tr></table></td>' +
                                            '<td><table><tr><td style="width: 100px;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;and</td><td>&nbsp;&nbsp;&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="TimeMax" size="30"/></td></tr></table></td>' +
                                        '</tr>' +
                                        '<tr>' +
                                            '<td><table><tr><td style="width: 100px;">ServiceName</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="ServiceName" size="30" title="Service Name"/></td></tr></table></td>' +
                                            '<td><table><tr><td style="width: 100px;">Title</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="Title" size="30" title="Title"/></td></tr></table></td>' +
                                            '<td><table><tr><td style="width: 100px;">Status</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="Status" size="30" title="Call Status"/></td></tr></table></td>' +
                                            '<td class="dkdiv"><table><tr><td style="width: 100px;">TimeTaken more than</td><td>&nbsp;:&nbsp;</td><td><input type="text" class="qsbox" name="q" data-field="TimeTaken" size="30" title="TimeTaken"/></td></tr></table></td>' +
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
            $('input:button').button().css('font-weight', 'bold');
            $('div#searchOptions input[value=Search]').click();
        });
    </script>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <div id="div-exceptionslogs">
        <table id="exceptionslogs" style="display: none;">
        </table>
        <div id="dialog-exceptions" title="Log Detail" style="display: none;">
            <fieldset>
                <table>
                </table>
            </fieldset>
        </div>
    </div>
</asp:Content>
