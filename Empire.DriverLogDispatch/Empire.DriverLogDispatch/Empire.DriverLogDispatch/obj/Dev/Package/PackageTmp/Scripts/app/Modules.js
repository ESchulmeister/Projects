
//Kendo Grid - StopApiController/Get
function loadStops(runID) {
    if (runID == null) {
        if (isAdmin) {
            stops = [];
            return;
        }

        runID = (currentRunNumber == null) ? -100 : currentRunNumber;       // pass an impossible number if no run number is detected
    }

    if (!loadGrid(runID)) {
        return;
    }

    $('#btnClose').click(function () {
        $('#modStop').hide();
        clearSelectedRow();
    });

    $('#ddlStatus').change(function () {
        $('#btnSaveStop').prop('disabled', false).addClass('btn-primary');
    });

    $('#btnSaveStop').click(function () {
        if (!saveStop()) {
            return;
        }

        refreshRun();

        $('#modStop').hide();
    });

    $('#btnCloseStop').click(function () {
        $('#modStop').hide();
        clearSelectedRow();
    });
}

function refreshRun(callback) {
  //  return;

    let run = (selDriver == null) ? null : selDriver.CurrentRun;
    if (run == null && (selRunNumber == null || selRunNumber == 0)) {
        loadStops(0);
        if (callback != undefined && callback != null) {
            callback();
        }
        return;
    }
    
    if (run != null && run.CanBeCompleted != null) {
        enableDoneButton(run);
        if (callback != undefined && callback != null) {
            callback();
        }
        return;
    }

    if (selRunNumber == null) {
        selRunNumber = run.Number;
    }

    if (selRunNumber == 0) {
        loadStops(0);
        if (callback != undefined && callback != null) {
            callback();
        }
        return;
    }

    // Call the Run API to get the "Can be Completed" status
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: baseUrl + 'api/run?drvCd=&id=' + selRunNumber,
        success: function (data) {
            if (selDriver != null) {
                selDriver.CurrentRun = data;
            }
            enableDoneButton(data);

            if (callback != undefined && callback != null) {
                callback();
            }
        },

        error: function (xhr, status, error) {

            if (xhr.status != 200) {

                let err = JSON.parse(xhr.responseText);
                console.log(err.Message);
                showError('An application error has occurred');
                return;
            }
        }
    });

}

function enableDoneButton(run) {
    let willEnableDone = run.CanBeCompleted;

    if (willEnableDone) {
        $('#btnDone').prop('disabled', !willEnableDone);
        $('#btnDone').addClass('btn-info');
    }
    else {
        $('#btnDone').prop('disabled', 'disabled');
        $('#btnDone').removeClass('btn-info');
    }
}

//modCustomer - Edit
function editStop() {

    var row = $(this).closest("tr"),
        grid = $("#grid").data("kendoGrid"),
        data = grid.dataSource.options.data,
        dataItem = grid.dataItem(row);

    $("#divButtons").show();

    showStopPopup(grid, data, dataItem);
}

function onChangeSelection(e) {
    let selectedRow = (this.select() == null || this.select().length == 0) ? null : this.select()[0];

    if (selectedRow == null || selectedRow.sectionRowIndex == 0) {
        return;
    }
    this.element.find(".k-grid-content").animate({
        scrollTop: (this.select().offset() == null) ? 0 : this.select().offset().top
    }, 350);
}

function showStopPopup(grid, data, dataItem) {
    loadStatus();

    selStop = null;
    let currStatus = null;

    let stops = data.filter(function (x) { return x.SequenceNumber === dataItem.SequenceNumber; });
    selStop = (stops.length == 0) ? null : stops[0];

    $('#btnSaveStop').prop('disabled', true).removeClass('btn-primary');

    if (selStop != null) {

        let customer = selStop.Customer;
        let customerName = (customer == null) ? '' : customer.Name;
        let seqNumber = selStop.DisplaySequence;

        let modTitle = seqNumber + ' - ' + customerName;

        if (selStop.CurrentStatus != null) {
            currStatus = selStop.CurrentStatus.Key;
            $('#ddlStatus').val(currStatus);
        }

        preservedSequence = selStop.SequenceNumber;

        $('#stopPopupTitle').html(modTitle);

    }


    $('#modStop').show();


}

function saveStop() {
    let selStatus = $("#ddlStatus").val();
    let foundStatuses = statuses.filter(function (s) { return s.Key == selStatus; });
    let status = (foundStatuses.length == 0) ? null : foundStatuses[0];

    selStop.CurrentStatus = status;

    $.each(stops, function (index, value) {
        let stop = stops[index];
        if (stop.SequenceNumber === selStop.SequenceNumber) {
            stop.CurrentStatus = status;
        }
    });


    updateStop();

    return true;
}

function showMap(grid, data, dataitem) {

    let mapUrl = null;

    var row = $(this).closest("tr"),
        grid = $("#grid").data("kendoGrid"),
        data = grid.dataSource.options.data,
        dataItem = grid.dataItem(row);


    let stops = data.filter(function (x) { return x.SequenceNumber === dataItem.SequenceNumber; });
    selStop = (stops.length == 0) ? null : stops[0];

    if (selStop != null) {

        let stop = selStop;
        let customer = stop.Customer;

        mapUrl = customer.MapUrl;
    }


    window.open(mapUrl);

}

//Update Stop - StopApiController/PUT
function updateStop() {
    $.ajax({
        type: 'PUT',
        dataType: 'json',
        data: { Key: selStop.Key, CustomerID: selStop.Customer.ID, Status: selStop.CurrentStatus.Code },
        url: baseUrl + 'api/stop',
        success: function (data) {
            let grid = $("#grid").data("kendoGrid");
            grid.refresh();

            timedRefresh(10);
        },

        error: function (xhr, status, error) {
            if (xhr.status != 200) {
                let err = JSON.parse(xhr.responseText);
                console.log(err.Message);
                showError('An application error has occurred');
                return;
            }
        }

    });
}

//LocationApiController/GET
function loadLocations() {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: baseUrl + 'api/location',
        success: function (data) {
            $('#ddlLocation').find('option').remove();
            for (let i = 0; i < data.length; i++) {
                let code = data[i].Code;
                $('#ddlLocation').append('<option value="' + code + '">' + code + '</option>');
            }

            let location = (preservedLocation == null || preservedLocation == '') ? userLocation : preservedLocation;
            if (location != null && location != '') {
                $('#ddlLocation').val(location);   //default to current user location
            }
            else {
                $('#ddlLocation')[0].selectedIndex = 0;
            }

            $('#hdnLocation').val($('#ddlLocation').val());
            loadDrivers();
        },

        error: function (xhr, status, error) {

            if (xhr.status != 200) {

                let err = JSON.parse(xhr.responseText);
                console.log(err.Message);

                showError('An application error has occurred');
                return;
            }
        }
    });
}

//DriverAPIController/GET
function loadDrivers() {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: baseUrl + 'api/driver?locCode=' + $('#ddlLocation').val(),
        success: function (data) {
            drivers = data;
            $('#ddlDriver').find('option').remove();
            for (let i = 0; i < data.length; i++) {
                let driver = data[i];
                let runNumber = (driver.CurrentRun == null) ? '' : driver.CurrentRun.Number;
                $('#ddlDriver').append('<option value="' + runNumber + '">' + driver.FullName + '</option>');
            }

            if (data.length > 0) {
                if (preservedDriver != null && preservedDriver != '') {
                    let selDrivers = drivers.filter(function (d) {
                        return (d.CurrentRun != null && d.CurrentRun.Number == preservedDriver);
                    });
                    selDriver = (selDrivers == null || selDrivers.length == 0) ? null : selDrivers[0];
                }
                else {
                    selDriver = drivers[0];
                }

                if (selRunNumber == null) {
                    selRunNumber = (selDriver.CurrentRun == null) ? '' : selDriver.CurrentRun.Number;
                }
                $('#ddlDriver').val(selRunNumber);
                $('#hdnDriver').val($('#ddlDriver').val());

                $('#divGrid').css('overflow-y', 'auto');
                loadRunDetail();
                loadStops(selRunNumber);
                refreshRun();
            }
            else {
                $('#divGrid').height(minDivHeight);
                $('#divGrid').css('overflow-y', 'hidden');
                setGridScrolling();

                $('#grid').data("kendoGrid").dataSource.data([]);
                let grid = $("#grid").data("kendoGrid");

                grid.refresh();
                $("#divDriverTitle").hide();

                toggleDisplay(true);
            }
        },

        error: function (xhr, status, error) {

            if (xhr.status != 200) {

                let err = JSON.parse(xhr.responseText);
                console.log(err.Message);
                showError('An application error has occurred');
                return;
            }
        }
    });
}

function setGridScrolling() {
    $("#grid .k-grid-content").css({
        "overflow-y": "scroll"
    });
}


//Grid Title - driver/run detail
function loadRunDetail() {
    let runNumber = isAdmin ? $('#ddlDriver').val() : currentRunNumber;
    let truckNumber = (selDriver.CurrentRun == null) ? '' : selDriver.CurrentRun.TruckNumber;

    let driverName = selDriver.FullName + spaces; // (isAdmin || isImpersonating) ? selDriver.FullName + spaces : '';

    let prefix = isImpersonating ? impersonationPrefix + ' ' : '';
    let driverTitle = prefix + driverName +
        'Run: ' + runNumber + spaces +
        'Truck: ' + truckNumber;

    $("#divDriverTitle").html(driverTitle);

    loadNote();
}

function loadNote() {
    let note = (selDriver == null || selDriver.CurrentRun == null) ? '' : selDriver.CurrentRun.Note;
    $('#txtNoteRO').val(note);
    $('#txtNote').val(note);
}

//StatusApiController/Get
function loadStatus() {

    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: baseUrl + 'api/status',
        success: function (data) {
            statuses = data;
            $('#ddlStatus').find('option').remove();

            for (let i = 0; i < data.length; i++) {
                let currStatus = data[i];
                $('#ddlStatus').append('<option value="' + currStatus.Key + '">' + currStatus.Description + '</option>');
            }

            if (selStop != null && selStop.CurrentStatus != null) {
                $('#ddlStatus').val(selStop.CurrentStatus.Key);
            }
        },

        error: function (xhr, status, error) {

            if (xhr.status != 200) {

                let err = JSON.parse(xhr.responseText);
                console.log(err.Message);

                showError('An application error has occurred');
                return;
            }
        }
    });

}

function timedRefresh(timeoutPeriod) {
    setTimeout(function () {
        // force the reload of the Run to get the "Can be Completed" status
        if (selDriver != null && selDriver.CurrentRun != null) {
            selDriver.CurrentRun = null;
        }

        refreshRun(function () {
            loadStops(selRunNumber);
        });
    }, timeoutPeriod);
}


function clearSelectedRow() {
    let grid = $("#grid").data("kendoGrid");
    grid.clearSelection();
}

//Update run - RunApiController/PUT
function updateRun() {
    let signatureImage = null;

    currentRunNote = $('#txtNote').val();

    let run = selDriver.CurrentRun;
    let runNo = run.Number;
    run.Note = currentRunNote;

    if (signaturePad != null && (!signaturePad.isEmpty())) {
        signatureImage = signaturePad.toDataURL().replace(/^data:image\/(png|jpg);base64,/, '');
    }

    $.ajax({
        type: 'PUT',
        dataType: 'json',
        data: { Key: runNo, Note: currentRunNote, Signature: signatureImage },
        url: baseUrl + 'api/run',
        success: function (data) {
            onRunUpdateCompleted();
        },
        error: function (xhr, status, error) {

            if (xhr.status != 200) {
                let err = JSON.parse(xhr.responseText);
                console.log(err.Message);
                showError('An application error has occurred');
                return;
            }

            onRunUpdateCompleted();
        }
    });
}

function onRunUpdateCompleted() {
    let isRunCompleted = (signaturePad != null && (!signaturePad.isEmpty()));
    if (!isRunCompleted) {
        return;
    }

    selDriver = null;
    selRunNumber = null;
    selStop = null;
    currentRunNumber = null;

    if (isAdmin) {
        loadDrivers(); // saving the signature completes the run and removes the Driver from the list
    }

    timedRefresh(10);
}

function clearError(error) {
    $('#errorDiv').hide();
    $('#errorDiv').html('');
}

function showError(error) {
    $('#errorDiv').show();
    $('#errorDiv').html(error);
}

function toggleDisplay(bHide) {
    if (bHide) {
        $('#txtNoteRO').hide();
        $('#btnDone').hide();
        $('#divNotes').hide()

        if (isImpersonating) {
            $('#divDriverTitle').html(impersonationPrefix + ' ' + $('#ddlImperDriver option:selected').text());
            $('#divDriverTitle').show();
        }
        else {
            $('#divDriverTitle').hide();
        }
    }
    else {
        $('#txtNoteRO').show();
        $('#btnDone').show();
        $('#divNotes').show()
        $('#divDriverTitle').show();
    }
}

function clearSignature() {
    if (signaturePad != null) {
        signaturePad.clear();
    }

}

function loadImpersonationList() {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: baseUrl + 'api/driver',
        success: function (data) {
            $('#ddlImperDriver').find('option').remove();
            for (let i = 0; i < data.length; i++) {
                let driver = data[i];
                let driverCode = driver.DriverCode;
                let driverDisplay = driver.FullName + ' (' + driverCode + ')';
                $('#ddlImperDriver').append('<option value="' + driverCode + '">' + driverDisplay + '</option>');
            }
        },

        error: function (xhr, status, error) {
            if (xhr.status != 200) {
                let err = JSON.parse(xhr.responseText);
                console.log(err.Message);
                showError('An application error has occurred');
                return;
            }
        }
    });
}

function doImpersonate() {

    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: baseUrl + 'api/run?ID=0&drvCd=' + $('#ddlImperDriver').val(),
        success: function (data) {
            onImpersonationDriverLoaded(data);
        },
        error: function (xhr, status, error) {
            if (xhr.status != 200 && xhr.status != 404) {
                let err = JSON.parse(xhr.responseText);
                console.log(err.Message);
                showError('An application error has occurred');
                return;
            }

            onImpersonationDriverLoaded(null);
        }
    });
}


function loadGrid(runID) {
    $.ajax({
        type: 'GET',
        dataType: 'json',
        url: baseUrl + 'api/stop?rid=' + runID,
        success: function (data) {
            stops = data;

            for (let i = 0; i < stops.length; i++) {
                let stop = stops[i];
                if (!stop.CurrentStatus.WillDisplay) {
                    stop.CurrentStatus.Description = '';
                }
            }

            const isOnTablet = isTablet();
            const isLandscape = (window.innerWidth > window.innerHeight);
            const maxDivHeight = (isOnTablet && isLandscape) ? 120 : (window.innerHeight < 800) ? 200 : 360;
            const lineHeight = 30;

            let dataHeight = data.length * lineHeight;

            let divHeight = (dataHeight > maxDivHeight) ? maxDivHeight : dataHeight;
            if (divHeight < minDivHeight) {
                divHeight = minDivHeight;
            }
            $('#divGrid').height(divHeight);

            let height = (dataHeight < divHeight) ? divHeight : dataHeight;

            let grid = $("#grid").kendoGrid({

                dataSource: {
                    data: data,
                    schema: {
                        model: {
                            fields: {
                                ID: { type: 'number' },
                                Name: { type: 'string' },
                                Address: { type: 'string' },
                                City: { type: 'string' }
                            }
                        }
                    },

                    pageSize: 200,

                },

                scrollable: false,
                selectable: "row",
                mobile: "true",
                filterable: false,
                groupable: false,
                height: height,
                noRecords: true,
                pageable: {
                    info: true,
                    previousNext: false,
                    numeric: false,
                    messages: {
                        display: ''
                    }
                },

                change: onChangeSelection,
                columns: [

                    {
                        field: "SequenceNumber",
                        attributes: { class: "text-center;" },
                        template: '<div>#= SequenceNumber#</div>',
                        width: 45,
                        headerTemplate: '<span class="k-i-kpi">Stop #</span>',
                    },

                    {
                        field: "CurrentStatus.Description",
                        width: 60,
                        attributes: { class: "text-center" },
                        template: '<div><a href="\\#" id="custStatusLink">#= CurrentStatus.Description#</a></div>',
                        headerTemplate: ' <span class="k-i-kpi">Status</span>'
                    },

                    {
                        field: "Customer.Name",
                        width: 250,
                        attributes: { class: "text-left" },
                        template: '<div><a href="\\#" id="custNameLink">#= Customer.Name#</a></div>',
                        headerTemplate: ' <span class="k-i-kpi">Customer</span>'
                    },

                    {
                        field: 'Customer.FullAddress',
                        attributes: { class: "text-left" },
                        width: 300,
                        template: '<div><a href = "\\#" id = "custAddressLink" >#= Customer.FullAddress#</a ></div > ',
                        headerTemplate: '<span class="k-i-kpi">Address </span>'
                    },


                ]
            });

            let custGrid = grid.data("kendoGrid");
            custGrid.element.on("click", "#custNameLink", editStop);
            custGrid.element.on("click", "#custStatusLink", editStop);
            custGrid.element.on("click", "#custAddressLink", showMap);

            if (preservedSequence != null && preservedSequence != '') {
                custGrid.select('tr:eq(' + preservedSequence + ')');
            }

            $('#txtNoteRO').css('min-width', $('#divGrid').width()) + 10;

            if (data.length == 0) {
                toggleDisplay(true);
            }
            else {
                toggleDisplay(false);
            }

            setGridScrolling();

            return true;
        },

        error: function (xhr, status, error) {

            if (xhr.status != 200) {

                let err = JSON.parse(xhr.responseText);
                console.log(err.Message);
                showError('An application error has occurred');
                return false;
            }
        }
    });

}

function onImpersonationDriverLoaded(run) {
    cachedDriver = selDriver;

    let driverDisplay = $('#ddlImperDriver option:selected').text();
    let parts = driverDisplay.split(' ');
    selDriver = {
        FullName: parts[0] + ' ' + parts[1],
        FirstName: parts[0],
        LastName: parts[1],
        DriverCode: parts[2].replace('(', '').replace(')', '')
    };

    selDriver.CurrentRun = run;
    selRunNumber = (selDriver.CurrentRun == null) ? 0 : selDriver.CurrentRun.Number;
    isImpersonating = true;

    onDriverChange();

    $('#divAdmin').hide();

    $('#divStopImpersonation').show();
    $('#btnRefresh').hide();
}

function stopImpersonation() {

    isImpersonating = false;

    $('#divStopImpersonation').hide();

    $('#divAdmin').show();      // Impersonation only available to Admins anyway
    $('#btnRefresh').show();

    let driverDisplay = $('#ddlDriver option:selected').text();

    selDriver = cachedDriver;
    selRunNumber = (selDriver == null || selDriver.CurrentRun == null) ? $('#ddlDriver').val() : selDriver.CurrentRun.Number;  // the Driver dropdown doesn't change during Impersonation
    cachedDriver = null;

    onDriverChange();
}

function onDriverChange() {
    loadRunDetail();
    loadStops(selRunNumber);
}